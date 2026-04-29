using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;
using UnityEngine.Networking;

public class APIRequestHandler : MonoBehaviour
{
	private const long m_Interval = 100L;

	private const long m_ExpireLimitTick = 10000L;

	private const long _APIRequestRetryIntervalTick = 60000L;

	public const int restAPIDefaultTimeout = 10;

	private long restAPICallSerialNumber = 1L;

	public Dictionary<ServerType, (string host, int port)> ServerAddrs = new Dictionary<ServerType, (string, int)>();

	private Queue<APIRequestKV> _apiRequestQueue = new Queue<APIRequestKV>();

	private bool m_UseHttps;

	private bool _canSendRequest = true;

	private long _lastSendTryTick;

	public bool CanSendRequest => _canSendRequest;

	private bool isAuthenticated => true;

	private void Awake()
	{
		m_UseHttps = true;
		AddServInfo(ServerType.Auth, "mimesisapi.relugameservice.com", 22226);
	}

	public void Initialize()
	{
	}

	public void EnqueueAPI<V>(IMsg msg, Action<IResMsg> callback) where V : IResMsg, new()
	{
		if (_canSendRequest)
		{
			APIRequestKV<V> item = new APIRequestKV<V>
			{
				RequestPkt = msg,
				Callback = delegate(V resMsg)
				{
					callback?.Invoke(resMsg);
				}
			};
			_apiRequestQueue.Enqueue(item);
		}
	}

	private void Update()
	{
		if (!_canSendRequest && Hub.s.timeutil.GetCurrentTickMilliSec() - _lastSendTryTick >= 60000)
		{
			_canSendRequest = true;
		}
		while (_apiRequestQueue.Count > 0 && _canSendRequest)
		{
			_apiRequestQueue.Dequeue().SendMsg();
		}
	}

	public bool AddServInfo(ServerType serverType, string host, int port)
	{
		ServerAddrs.Add(serverType, (host, port));
		return true;
	}

	public void ClearServInfo(ServerType serverType)
	{
		if (ServerAddrs.ContainsKey(serverType))
		{
			ServerAddrs.Remove(serverType);
		}
	}

	public async UniTask<(bool, string)> SendToRestServer(string ipAddr, int port, string path, HttpMethod method, Dictionary<string, string> queries, string body)
	{
		UriBuilder uriBuilder = new UriBuilder
		{
			Host = ipAddr,
			Path = path,
			Scheme = (m_UseHttps ? "https" : "http")
		};
		if (port != 0)
		{
			uriBuilder.Port = port;
		}
		if (queries != null)
		{
			uriBuilder.Query = string.Join("&", queries.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value));
		}
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, uriBuilder.Uri);
		httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, "text/plain");
		APIResponse aPIResponse;
		try
		{
			aPIResponse = await SendAPI(httpRequestMessage, 10);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		long responseCode = aPIResponse.ResponseCode;
		if (responseCode < 500 && responseCode < 400)
		{
			if (responseCode == 200)
			{
				return (true, aPIResponse.Body);
			}
			throw new Exception($"Invalid responseCode. Code : {aPIResponse.ResponseCode}");
		}
		return (false, string.Empty);
	}

	public bool ExistServerAddress()
	{
		return ServerAddrs.Count > 0;
	}

	public async UniTask<T> SendIMsg<T>(TargetServerType serverType, IMsg msg, int timeout = 10) where T : IResMsg, new()
	{
		if (!ExistServerAddress())
		{
			Logger.RError("CDN Info not loaded. send failed.");
			return new T
			{
				errorCode = MsgErrorCode.InvalidErrorCode
			};
		}
		Interlocked.Increment(ref restAPICallSerialNumber);
		APIUriMapper.IUriInfo urnInfo = Hub.s.urimapper.GetUriInfo(msg);
		if (urnInfo == null)
		{
			return new T
			{
				errorCode = MsgErrorCode.InvalidErrorCode
			};
		}
		UriBuilder uriBuilder = new UriBuilder();
		UriBuilder uriBuilder2 = uriBuilder;
		uriBuilder2.Host = serverType switch
		{
			TargetServerType.APIAuth => ServerAddrs[ServerType.Auth].host, 
			TargetServerType.APIGame => ServerAddrs[ServerType.Game].host, 
			_ => throw new Exception("invalid server Type on uribuilder while setup addr"), 
		};
		UriBuilder uriBuilder3 = uriBuilder;
		uriBuilder3.Port = serverType switch
		{
			TargetServerType.APIAuth => ServerAddrs[ServerType.Auth].port, 
			TargetServerType.APIGame => ServerAddrs[ServerType.Game].port, 
			_ => throw new Exception("invalid server Type on uribuilder while setup port"), 
		};
		uriBuilder.Scheme = (m_UseHttps ? "https" : "http");
		UriBuilder uriBuilder4 = uriBuilder;
		if (urnInfo.Path != null)
		{
			uriBuilder4.Path = Hub.s.urimapper.GetFormattedUri(urnInfo.Path, msg);
		}
		if (urnInfo.Query != null)
		{
			uriBuilder4.Query = Hub.s.urimapper.GetFormattedUri(urnInfo.Query, msg);
		}
		HttpRequestMessage request = new HttpRequestMessage(urnInfo.GetHttpMethod(), uriBuilder4.Uri);
		if (isAuthenticated)
		{
			request.Headers.Add("player_uid", "0");
		}
		if (request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
		{
			string content = JsonConvert.SerializeObject(msg);
			request.Content = new StringContent(content, Encoding.UTF8, "application/json");
		}
		APIResponse curlResponse = null;
		IResMsg resMsg;
		try
		{
			curlResponse = await SendAPI(request, timeout);
		}
		catch (UnityWebRequestException ex)
		{
			if (ex.Error == "Request timeout")
			{
				Logger.RError($"IMsg Send timeout. URL : {request.RequestUri}");
				resMsg = urnInfo.GenResponse(string.Empty);
				resMsg.errorCode = MsgErrorCode.ApiErrorExceptionOccurred;
				return resMsg as T;
			}
		}
		long responseCode = curlResponse.ResponseCode;
		if (responseCode < 500)
		{
			if (responseCode < 400)
			{
				if (responseCode != 200)
				{
					throw new Exception($"Invalid responseCode. Code : {curlResponse.ResponseCode}");
				}
				resMsg = urnInfo.GenResponse(curlResponse.Body);
			}
			else
			{
				resMsg = urnInfo.GenResponse(string.Empty);
				resMsg.errorCode = MsgErrorCode.ApiErrorExceptionOccurred;
			}
		}
		else
		{
			resMsg = urnInfo.GenResponse(string.Empty);
			resMsg.errorCode = MsgErrorCode.ApiErrorExceptionOccurred;
		}
		return resMsg as T;
	}

	private async UniTask<APIResponse> SendAPI(HttpRequestMessage request, int timeout)
	{
		UnityWebRequest www = new UnityWebRequest(request.RequestUri.ToString(), request.Method.ToString())
		{
			useHttpContinue = false,
			timeout = timeout,
			certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey()
		};
		foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
		{
			www.SetRequestHeader(header.Key, string.Join(",", header.Value));
		}
		if (request.Content != null && request.Content.Headers != null)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> header2 in request.Content.Headers)
			{
				www.SetRequestHeader(header2.Key, string.Join(",", header2.Value));
			}
		}
		www.downloadHandler = new DownloadHandlerBuffer();
		if (request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
		{
			www.uploadHandler = new UploadHandlerRaw(await request.Content.ReadAsByteArrayAsync());
		}
		try
		{
			await www.SendWebRequest();
		}
		catch (UnityWebRequestException ex)
		{
			Logger.RError($"APIRequestHandler.SendAPI() Exception : {ex} / URL : {request.RequestUri}");
			Debug.LogException(ex);
			if (ex.Error == "Request timeout")
			{
				throw ex;
			}
		}
		return new APIResponse(www.responseCode, www.downloadHandler.text, www.error);
	}

	public void SetAvailableSendRequest(bool canSend)
	{
		_canSendRequest = canSend;
		_lastSendTryTick = Hub.s.timeutil.GetCurrentTickMilliSec();
	}
}
