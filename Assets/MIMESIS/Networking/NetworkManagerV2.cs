using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using ReluReplay.Shared;
using ReluServerBase.Threading;
using UnityEngine;

public class NetworkManagerV2 : MonoBehaviour
{
	private int _seqID;

	private NetworkGrade _prevGrade;

	private ISession? m_SessionDirect;

	private CommandExecutor m_Executor;

	private ConcurrentQueue<IMsg> m_SendQueue = new ConcurrentQueue<IMsg>();

	private EventTimer m_Timer = new EventTimer();

	private NetworkManagerState m_State;

	private DisconnectReason m_LastDisconnectReason;

	private bool m_ReconnectRequested;

	private AtomicFlag m_Sending = new AtomicFlag(value: false);

	private float m_CurrentTime;

	private float m_LoginTryTime;

	private int m_ReconnectCount;

	private Dictionary<ServerType, List<ServerAddrInfo>> m_ServerAddrs = new Dictionary<ServerType, List<ServerAddrInfo>>();

	private bool connectErrorAccur;

	private ulong m_UserUID;

	private int m_GameServerAddrIndex;

	private string threadingToken = string.Empty;

	private ConcurrentQueue<(bool callbackHandled, IMsg)> m_msgQueue = new ConcurrentQueue<(bool, IMsg)>();

	private Dictionary<MsgType, ConcurrentDictionary<int, IRequestHandler>> _timeoutHandlers = new Dictionary<MsgType, ConcurrentDictionary<int, IRequestHandler>>();

	private int lastHeartBeatSeqId;

	public NetworkManagerState State => m_State;

	public DisconnectReason LastDisconnectReason => m_LastDisconnectReason;

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] NetworkManagerV2.Awake ->");
		m_Executor = CommandExecutor.CreateCommandExecutor("Client", 0L);
		Logger.RLog("[AwakeLogs] NetworkManagerV2.Awake <-");
	}

	public bool TryDequeueMsg(out (bool, IMsg) msgPair)
	{
		return m_msgQueue.TryDequeue(out msgPair);
	}

	public void PurgeMsg()
	{
		_ = m_msgQueue.Count;
		_ = 0;
		m_msgQueue.Clear();
	}

	public void OnRecvPacket(IMsg msg)
	{
		if (HandleGlobalPacket(msg))
		{
			return;
		}
		bool item = false;
		if (_timeoutHandlers.TryGetValue(msg.msgType, out ConcurrentDictionary<int, IRequestHandler> value) && value.TryGetValue(msg.hashCode, out var handler))
		{
			value.TryRemove(msg.hashCode, out var _);
			item = true;
			m_Executor.Invoke(delegate
			{
				handler.ExecuteResponseHandler(msg);
			});
		}
		m_msgQueue.Enqueue((item, msg));
	}

	private bool HandleGlobalPacket(IMsg msg)
	{
		if (msg.msgType == MsgType.C2S_AdminCommandRes)
		{
			if (msg is AdminCommandRes adminCommandRes)
			{
				_ = adminCommandRes.errorCode;
			}
			return true;
		}
		return false;
	}

	public void Initialize()
	{
		m_State = NetworkManagerState.NotConnected;
		m_CurrentTime = Time.realtimeSinceStartup;
		connectErrorAccur = false;
	}

	public void SetUserUID(ulong uid)
	{
		m_UserUID = uid;
		m_GameServerAddrIndex = (int)((m_ServerAddrs[ServerType.Game].Count > 1) ? (m_UserUID % (ulong)m_ServerAddrs[ServerType.Game].Count) : 0);
	}

	public bool AddServInfo(ServerType serverType, string host, int port, bool steamRelay = false)
	{
		if (!m_ServerAddrs.ContainsKey(serverType))
		{
			m_ServerAddrs.Add(serverType, new List<ServerAddrInfo>());
		}
		m_ServerAddrs[serverType].Add(new ServerAddrInfo(host, port, steamRelay));
		return true;
	}

	public void ClearServInfo(ServerType serverType)
	{
		if (m_ServerAddrs.ContainsKey(serverType))
		{
			m_ServerAddrs[serverType].Clear();
		}
	}

	private void ClearServerAddrTryCount(ServerType serverType = ServerType.Game)
	{
		m_ServerAddrs[serverType].ForEach(delegate(ServerAddrInfo x)
		{
			x.TryCount = 0;
		});
	}

	public bool Connect(ServerType type = ServerType.Game, int reconnectCount = 0)
	{
		if (m_State != NetworkManagerState.Disconnected && m_State != NetworkManagerState.NotConnected && m_ReconnectRequested)
		{
			return false;
		}
		if (m_SessionDirect != null)
		{
			return false;
		}
		if (!m_ServerAddrs.ContainsKey(type))
		{
			return false;
		}
		if (reconnectCount == 0)
		{
			ClearServerAddrTryCount(type);
		}
		else
		{
			m_GameServerAddrIndex = SimpleRandUtil.Next(m_ServerAddrs[type].Count);
		}
		ServerAddrInfo serverAddrInfo = m_ServerAddrs[type][m_GameServerAddrIndex];
		if (Hub.s.pdata.ClientMode == NetworkClientMode.Host)
		{
			m_SessionDirect = new VirtualClientSession(OnRecvPacket, OnConnect, OnClose);
		}
		else if (serverAddrInfo.SteamRelay)
		{
			m_SessionDirect = new SDRClientSession(OnRecvPacket, OnConnect, OnClose);
		}
		else
		{
			m_SessionDirect = new RUDPClientSession(OnRecvPacket, OnConnect, OnClose);
		}
		m_State = NetworkManagerState.Connecting;
		ServerAddrInfo serverAddrInfo2 = m_ServerAddrs[type][m_GameServerAddrIndex];
		serverAddrInfo2.TryCount++;
		if (serverAddrInfo.SteamRelay)
		{
			m_SessionDirect.Connect(new SteamIDConnection
			{
				SteamID = ulong.Parse(serverAddrInfo2.Host),
				Port = serverAddrInfo2.Port
			});
		}
		else
		{
			m_SessionDirect.Connect(new IPAddrConnection
			{
				IPAddr = serverAddrInfo2.Host,
				Port = serverAddrInfo2.Port
			});
		}
		return true;
	}

	public void Disconnect(DisconnectReason reason)
	{
		m_SessionDirect?.BeginClose(reason);
	}

	public void OnConnect()
	{
		m_State = NetworkManagerState.Connected;
		if (m_ReconnectRequested)
		{
			m_ReconnectRequested = false;
		}
		m_ReconnectCount = 0;
		ClearServerAddrTryCount();
	}

	public void OnDestroy()
	{
		if (m_Timer != null)
		{
			m_Timer.Dispose();
		}
		if (m_ServerAddrs != null)
		{
			m_ServerAddrs.Clear();
		}
		Disconnect(DisconnectReason.ByClient);
	}

	private void OnCloseInternal(DisconnectReason reason)
	{
		m_SessionDirect = null;
		m_State = NetworkManagerState.Disconnected;
		m_LastDisconnectReason = reason;
		if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main != null)
		{
			Hub.s.pdata.main.RLog("OnCloseInternal");
			Hub.s.pdata.main.OnNetDisconnected(reason);
		}
	}

	public void OnClose(DisconnectReason reason)
	{
		OnCloseInternal(reason);
	}

	private IEnumerator WaitAndRetry()
	{
		while (m_ReconnectCount < 5)
		{
			yield return new WaitForSeconds(10f);
			if (m_State != NetworkManagerState.Connected && m_State != NetworkManagerState.Connecting && ++m_ReconnectCount < 5)
			{
				Connect(ServerType.Game, m_ReconnectCount);
				continue;
			}
			connectErrorAccur = true;
			break;
		}
	}

	public void OnError(Exception ex, SocketError e)
	{
		Logger.RError((ex?.ToString() ?? string.Empty) + " // " + e);
		OnCloseInternal(DisconnectReason.ByServer);
	}

	private void Update()
	{
		if (ReplaySharedData.IsReplayPlayMode)
		{
			return;
		}
		m_Executor.Execute();
		m_Timer.Update();
		if (m_SessionDirect == null)
		{
			return;
		}
		if (m_SessionDirect is VirtualClientSession virtualClientSession)
		{
			virtualClientSession.Update();
		}
		else if (m_SessionDirect is RUDPClientSession rUDPClientSession)
		{
			rUDPClientSession.Update();
		}
		else if (m_SessionDirect is SDRClientSession sDRClientSession)
		{
			sDRClientSession.Update();
		}
		switch (m_State)
		{
		case NetworkManagerState.NotConnected:
			if (connectErrorAccur)
			{
				connectErrorAccur = false;
			}
			break;
		case NetworkManagerState.Connected:
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - m_CurrentTime > 10f)
			{
				m_CurrentTime = realtimeSinceStartup;
				SendHeartBeat();
			}
			if (Math.Abs((long)_seqID - (long)lastHeartBeatSeqId) > 10)
			{
				Logger.RError("HeartBeat Lost");
				OnClose(DisconnectReason.ConnectionError);
			}
			break;
		}
		case NetworkManagerState.Connecting:
		case NetworkManagerState.Disconnected:
			break;
		}
	}

	private void SendHeartBeat()
	{
		Send(new HeartBeatReq
		{
			clientSendTime = Hub.s.timeutil.GetCurrentTickMilliSec(),
			seqID = _seqID++,
			networkGrade = _prevGrade
		});
	}

	private void OnEnable()
	{
	}

	public bool SendNoCallback(IMsg msg)
	{
		if (Send(msg) != SendResult.Success)
		{
			Logger.RError($"Failed to send message: {msg.msgType}");
			return false;
		}
		return true;
	}

	public void SendWithCallback<T>(IMsg msg, OnClientDispatchEventHandler<T> respHandler, CancellationToken cancellationToken, int timeout = 60000, bool disconnectOnTimeout = false) where T : IResMsg, new()
	{
		SendAsyncInternal(msg, respHandler, cancellationToken, timeout, disconnectOnTimeout);
	}

	private void SendAsyncInternal<T>(IMsg msg, OnClientDispatchEventHandler<T> respHandler, CancellationToken cancellationToken, int timeout = 60000, bool disconnectOnTimeout = false) where T : IResMsg, new()
	{
		T val = new T();
		string timerName = $"{val.msgType}|{msg.hashCode}";
		if (m_Timer.Exist(timerName))
		{
			T errorAns = new T
			{
				errorCode = MsgErrorCode.ClientErrorDuplicatedTimer
			};
			Logger.RError($"Duplicated timer for {val.msgType} with hash {msg.hashCode}");
			if (respHandler == null)
			{
				return;
			}
			m_Executor.Invoke(delegate
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					respHandler(errorAns);
				}
			});
			return;
		}
		if (Send(msg) != SendResult.Success)
		{
			T errorAns2 = new T
			{
				errorCode = MsgErrorCode.ClientErrorSendFailed
			};
			Logger.RError($"Failed to send message: {msg.msgType} with hash {msg.hashCode}");
			if (respHandler == null)
			{
				return;
			}
			m_Executor.Invoke(delegate
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					respHandler(errorAns2);
				}
			});
			return;
		}
		int timerID = m_Timer.CreateTimerEvent(delegate
		{
			if (_timeoutHandlers.TryGetValue(msg.msgType, out ConcurrentDictionary<int, IRequestHandler> value2) && value2.TryGetValue(msg.hashCode, out var _))
			{
				value2.TryRemove(msg.hashCode, out var _);
			}
			Logger.RError($"Timeout occurred for message: {msg.msgType} with hash {msg.hashCode}");
			T errorAns3 = new T
			{
				errorCode = MsgErrorCode.ClientErrorSendTimeout
			};
			m_Executor.Invoke(delegate
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					respHandler(errorAns3);
				}
				if (disconnectOnTimeout)
				{
					Logger.RError($"Timeout occurred for message: {msg.msgType} with hash {msg.hashCode}, disconnecting...");
					m_SessionDirect?.BeginClose(DisconnectReason.ByClient);
				}
			});
		}, timeout, repeat: false, timerName);
		OnClientDispatchEventHandler<T> responseHandler = delegate(T msg2)
		{
			m_Timer.RemoveTimerEvent(timerName);
			if (!cancellationToken.IsCancellationRequested)
			{
				respHandler(msg2);
			}
		};
		if (!_timeoutHandlers.TryGetValue(val.msgType, out ConcurrentDictionary<int, IRequestHandler> value))
		{
			value = new ConcurrentDictionary<int, IRequestHandler>();
			value.TryAdd(msg.hashCode, new RequestHandler<T>(responseHandler, timerID));
			_timeoutHandlers.Add(val.msgType, value);
		}
		else if (!value.TryAdd(msg.hashCode, new RequestHandler<T>(responseHandler, timerID)))
		{
			Logger.RError($"Failed to add timeout handler for {msg.msgType} with hash {msg.hashCode}");
			m_Timer.RemoveTimerEvent(timerName);
		}
	}

	private SendResult Send(IMsg msg)
	{
		return m_SessionDirect?.SendLink(msg) ?? SendResult.Undefined;
	}

	public void SetLastHeartBeatSeqId(int seqId)
	{
		lastHeartBeatSeqId = seqId;
	}

	public void SetNetworkGrade(int rtt)
	{
		if (rtt < 60)
		{
			if (rtt < 30)
			{
				_prevGrade = NetworkGrade.Fine;
			}
			else
			{
				_prevGrade = NetworkGrade.Medium;
			}
		}
		else if (rtt < 100)
		{
			_prevGrade = NetworkGrade.Slow;
		}
		else
		{
			_prevGrade = NetworkGrade.Terrible;
		}
	}
}
