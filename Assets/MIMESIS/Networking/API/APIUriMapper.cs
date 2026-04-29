using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public class APIUriMapper : MonoBehaviour
{
	public enum MethodType
	{
		GET = 0,
		POST = 3,
		PUT = 1,
		DELETE = 2
	}

	public delegate T GenerateDelegate<T>(string value) where T : IResMsg, new();

	public abstract class IUriInfo
	{
		public Dictionary<string, string> headerInfo = new Dictionary<string, string>();

		public string Path { get; private set; }

		public string Query { get; private set; }

		public bool SystemAPI { get; private set; }

		public MethodType Method { get; private set; }

		public bool Retryable { get; private set; }

		public bool AllowOnBlockedChannel { get; private set; }

		public IUriInfo(string path, MethodType method, string query = "", bool systemAPI = false, bool allowOnBlockedChannel = false, bool retryable = false)
		{
			Path = path;
			Query = query;
			SystemAPI = systemAPI;
			Method = method;
			AllowOnBlockedChannel = allowOnBlockedChannel;
			Retryable = retryable;
		}

		public HttpMethod GetHttpMethod()
		{
			return Method switch
			{
				MethodType.GET => HttpMethod.Get, 
				MethodType.POST => HttpMethod.Post, 
				MethodType.PUT => HttpMethod.Put, 
				MethodType.DELETE => HttpMethod.Delete, 
				_ => HttpMethod.Get, 
			};
		}

		public abstract IResMsg GenResponse(string body);
	}

	public class UriInfo<T> : IUriInfo where T : IResMsg, new()
	{
		public GenerateDelegate<T> GenFunc;

		public UriInfo(string path, MethodType method, string query = "", bool systemAPI = false, bool allowOnBlockedChannel = false, bool retryable = false)
			: base(path, method, query, systemAPI, allowOnBlockedChannel, retryable)
		{
			GenFunc = Hub.s.urimapper.GenerateResponseFromString<T>;
		}

		public override IResMsg GenResponse(string body)
		{
			return GenFunc(body);
		}
	}

	private Dictionary<MsgType, IUriInfo>? m_UriDictionary;

	public const string HEADER_API_PROTOCOL_ERROR = "API-PROTOCOL-ERROR-CODE";

	public T GenerateResponseFromString<T>(string value) where T : IResMsg, new()
	{
		try
		{
			if (value == string.Empty)
			{
				return new T();
			}
			T val = JsonConvert.DeserializeObject<T>(value);
			if (val == null)
			{
				return new T();
			}
			return val;
		}
		catch (Exception e)
		{
			Logger.RError(e);
			return new T();
		}
	}

	private void OnDestroy()
	{
		m_UriDictionary?.Clear();
	}

	public void Initialize()
	{
		if (m_UriDictionary == null)
		{
			m_UriDictionary = new Dictionary<MsgType, IUriInfo>
			{
				{
					new APIEnterLobbyLogReq().msgType,
					new UriInfo<APIEnterLobbyLogRes>("/log/enterlobby", MethodType.PUT)
				},
				{
					new APINewRoomLogReq().msgType,
					new UriInfo<APINewRoomLogRes>("/log/newroom", MethodType.PUT)
				},
				{
					new APIJoinRoomLogReq().msgType,
					new UriInfo<APIJoinRoomLogRes>("/log/joinroom", MethodType.PUT)
				},
				{
					new APIEnterTramLogReq().msgType,
					new UriInfo<APIEnterTramLogRes>("/log/entertram", MethodType.PUT)
				},
				{
					new APIDisconnectLogReq().msgType,
					new UriInfo<APIDisconnectLogRes>("/log/disconnect", MethodType.PUT)
				},
				{
					new APIOpenPublicTramLogReq().msgType,
					new UriInfo<APIOpenPublicTramLogRes>("/log/openpublictram", MethodType.PUT)
				},
				{
					new APIGameEventLogReq().msgType,
					new UriInfo<APIGameEventLogRes>("/log/gel", MethodType.POST)
				}
			};
			CheckInvalidUri();
		}
	}

	public bool IsAllowOnBlockedChannel(IMsg msg)
	{
		if (m_UriDictionary == null)
		{
			return false;
		}
		if (m_UriDictionary.TryGetValue(msg.msgType, out IUriInfo value))
		{
			return value.AllowOnBlockedChannel;
		}
		return false;
	}

	public IUriInfo? GetUriInfo(IMsg msg)
	{
		if (m_UriDictionary == null)
		{
			Logger.RError("APIUriMapper not initialized!");
			return null;
		}
		if (m_UriDictionary.TryGetValue(msg.msgType, out IUriInfo value))
		{
			return value;
		}
		Logger.RError($"Can't find URI info for MsgType: {msg.msgType}, m_UriDictionary count: {m_UriDictionary.Count}");
		return null;
	}

	public bool HasUriInfo(IMsg msg)
	{
		return m_UriDictionary.Where<KeyValuePair<MsgType, IUriInfo>>((KeyValuePair<MsgType, IUriInfo> x) => x.Key == msg.msgType).Any();
	}

	public string GetFormattedUri(string originalUri, IMsg msg)
	{
		string text = originalUri;
		foreach (Match item in new Regex("{(?<param>[\\w]*)}").Matches(originalUri))
		{
			string value = item.Groups["param"].Value;
			string newValue = msg.GetType().GetProperty(value)?.GetValue(msg)?.ToString() ?? string.Empty;
			text = text.Replace(item.Value, newValue);
		}
		return text;
	}

	public void CheckInvalidUri()
	{
		if (m_UriDictionary == null)
		{
			return;
		}
		foreach (KeyValuePair<MsgType, IUriInfo> item in m_UriDictionary)
		{
			MsgType key = item.Key;
			Regex regex = new Regex("{(?<param>[\\w]*)}");
			MatchCollection matchCollection = regex.Matches(item.Value.Path);
			MatchCollection matchCollection2 = regex.Matches(item.Value.Query);
			foreach (Match item2 in matchCollection)
			{
				string value = item2.Groups["param"].Value;
				_ = key.GetType().GetProperty(value) == null;
			}
			foreach (Match item3 in matchCollection2)
			{
				string value2 = item3.Groups["param"].Value;
				_ = key.GetType().GetProperty(value2) == null;
			}
		}
	}

	public Dictionary<string, string>? GetFormattedHeader(IUriInfo uriInfo, IMsg msg, Type msgType)
	{
		if (uriInfo.headerInfo == null || uriInfo.headerInfo.Count == 0)
		{
			return null;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> item in uriInfo.headerInfo)
		{
			string value = msgType.GetProperty(item.Value)?.GetValue(msg)?.ToString() ?? string.Empty;
			dictionary.Add(item.Key, value);
		}
		return dictionary;
	}
}
