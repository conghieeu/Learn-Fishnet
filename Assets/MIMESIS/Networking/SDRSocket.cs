using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Bifrost;
using Newtonsoft.Json;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using Steamworks;

public class SDRSocket
{
	private OnSDRConnect _onConnect;

	private readonly ConcurrentQueue<IMsg> m_ReliableSendQueue = new ConcurrentQueue<IMsg>();

	private readonly ConcurrentQueue<IMsg> m_UnreliableSendQueue = new ConcurrentQueue<IMsg>();

	private AsyncIOReceiveContext m_ReceiveBufferContext;

	private AsyncIOSendContext m_SendBufferContext;

	private OnSendPacket _onSend;

	private OnRecvPacket _onRecv;

	private OnSDRClose _onClose;

	private OnSDRError _onError;

	private AtomicFlag m_ClosePending = new AtomicFlag(value: false);

	private AtomicFlag m_Disposed = new AtomicFlag(value: false);

	protected AtomicFlag m_Sending = new AtomicFlag(value: false);

	private HSteamNetConnection _connection = HSteamNetConnection.Invalid;

	private IntPtr[] messages = new IntPtr[16];

	private Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChangedCallback;

	private bool _isConnectedToServer;

	private long _lastTimeoutTick;

	private int _timeoutCount;

	public SDRSocket(INetworkBufferPool networkBufferPool, OnSendPacket onSend, OnRecvPacket onRecv, OnSDRConnect onConnect, OnSDRClose onClose, OnSDRError onError)
	{
		_onSend = onSend;
		_onRecv = onRecv;
		_onClose = onClose;
		_onError = onError;
		_onConnect = onConnect;
		m_ReceiveBufferContext = new AsyncIOReceiveContext(networkBufferPool);
		m_SendBufferContext = new AsyncIOSendContext(networkBufferPool);
	}

	public void SetConnection(HSteamNetConnection connection)
	{
		if (_connection != HSteamNetConnection.Invalid)
		{
			Logger.RError("Connection already set");
			return;
		}
		_connection = connection;
		_isConnectedToServer = true;
	}

	public void Update()
	{
		if (_connection == HSteamNetConnection.Invalid || !SteamNetworkingSockets.GetConnectionInfo(_connection, out var pInfo) || pInfo.m_eState != ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
		{
			return;
		}
		Flush();
		int num = SteamNetworkingSockets.ReceiveMessagesOnConnection(_connection, messages, 16);
		if (num == 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			_ = messages[i];
			SteamNetworkingMessage_t steamNetworkingMessage_t = SteamNetworkingMessage_t.FromIntPtr(messages[i]);
			if (steamNetworkingMessage_t.m_pData == IntPtr.Zero || steamNetworkingMessage_t.m_cbSize <= 0)
			{
				Logger.RError("Invalid message");
			}
			else
			{
				m_ReceiveBufferContext.WriteBuffer(steamNetworkingMessage_t.m_pData, steamNetworkingMessage_t.m_cbSize);
			}
		}
		OnReceive();
		if (_timeoutCount > 0 && Hub.s.timeutil.GetCurrentTickMilliSec() - _lastTimeoutTick > 60000)
		{
			_timeoutCount = 0;
			_lastTimeoutTick = 0L;
		}
	}

	public bool Connect(SteamNetworkingIdentity identity, int port)
	{
		try
		{
			SteamNetworkingConfigValue_t[] steamNetworkingConfig = GetSteamNetworkingConfig();
			_connection = SteamNetworkingSockets.ConnectP2P(ref identity, port, 0, steamNetworkingConfig);
			if (_connection == HSteamNetConnection.Invalid)
			{
				Logger.RError("Connection failed");
				return false;
			}
			connectionStatusChangedCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnClientConnectionStatusChanged);
			return true;
		}
		catch (Exception ex)
		{
			Logger.RError("Connection failed, " + ex.Message);
			return false;
		}
	}

	private void OnClientConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t callback)
	{
		if (callback.m_hConn != _connection)
		{
			return;
		}
		switch (callback.m_info.m_eState)
		{
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
			_isConnectedToServer = true;
			_onConnect();
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
			LogCloseReason("SDRSocket", callback);
			LogConnectionRealTimeStatus("SDRSocket", callback.m_hConn);
			CloseInternal(DisconnectReason.ByServer);
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
			if (callback.m_info.m_eEndReason == 4001 && _timeoutCount < 3)
			{
				_lastTimeoutTick = Hub.s.timeutil.GetCurrentTickMilliSec();
				_timeoutCount++;
			}
			else
			{
				LogCloseReason("SDRSocket", callback);
				LogConnectionRealTimeStatus("SDRSocket", callback.m_hConn);
				CloseInternal(DisconnectReason.ByClient);
			}
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None:
			if (callback.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
			{
				_onClose(DisconnectReason.Undefined);
			}
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_FindingRoute:
			break;
		}
	}

	public void Dispose()
	{
		Dispose(isDisposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool isDisposing)
	{
		if (isDisposing && m_Disposed.On())
		{
			if (_connection != HSteamNetConnection.Invalid)
			{
				SteamNetworkingSockets.CloseConnection(_connection, 0, "Dispose", bEnableLinger: false);
				connectionStatusChangedCallback?.Unregister();
			}
			m_ReliableSendQueue.Clear();
			m_UnreliableSendQueue.Clear();
			m_SendBufferContext.Dispose();
			m_ReceiveBufferContext.Dispose();
		}
	}

	public bool OnReceive()
	{
		long num = m_ReceiveBufferContext.EndPosition;
		while (0 < num && 4 <= num)
		{
			int intValue = 0;
			Serializer.Load(m_ReceiveBufferContext.GetSegment(), m_ReceiveBufferContext.CurrentPosition, ref intValue);
			if (intValue > m_ReceiveBufferContext.Size)
			{
				Logger.RError($"Invalid Message Size - {intValue}. Exceeding the maximum packet size. {m_ReceiveBufferContext.Size}");
				_onError(null, SocketError.MessageSize);
				CloseInternal(DisconnectReason.ConnectionError);
				return false;
			}
			if (num < intValue)
			{
				break;
			}
			if (intValue < 4)
			{
				Logger.RError($"Invalid Message Size - {intValue}");
				break;
			}
			m_ReceiveBufferContext.Seek(4);
			int intValue2 = 0;
			Serializer.Load(m_ReceiveBufferContext.GetSegment(), m_ReceiveBufferContext.CurrentPosition, ref intValue2);
			m_ReceiveBufferContext.Seek(4);
			int num2 = intValue - 8;
			var (flag, data) = m_ReceiveBufferContext.Read(num2);
			if (!flag || data.Array == null)
			{
				Logger.RWarn($"Invalid Message Size - {intValue} / {m_ReceiveBufferContext.RemainReadSize}");
				_onError(null, SocketError.MessageSize);
				CloseInternal(DisconnectReason.Undefined);
				return false;
			}
			m_ReceiveBufferContext.Seek(num2);
			_onRecv(intValue, intValue2, in data);
			num -= intValue;
		}
		m_ReceiveBufferContext.RemoveFrontBuffer();
		return true;
	}

	public bool IsRunning()
	{
		if (_isConnectedToServer)
		{
			return _connection != HSteamNetConnection.Invalid;
		}
		return false;
	}

	public SendResult Send(IMsg[] msgs)
	{
		if (msgs.Length == 0)
		{
			return SendResult.InvalidSize;
		}
		if (!IsRunning())
		{
			CloseInternal(DisconnectReason.None);
			return SendResult.NotConnect;
		}
		for (int i = 0; i < msgs.Length; i++)
		{
			if (msgs[i].reliable)
			{
				m_ReliableSendQueue.Enqueue(msgs[i]);
			}
			else
			{
				m_UnreliableSendQueue.Enqueue(msgs[i]);
			}
		}
		return SendResult.Success;
	}

	public void Close(DisconnectReason reason, bool immediately)
	{
		if (!m_Disposed.IsOn)
		{
			if (immediately || (m_ReliableSendQueue.IsEmpty && m_UnreliableSendQueue.IsEmpty))
			{
				CloseInternal(reason);
			}
			else
			{
				m_ClosePending.On();
			}
		}
	}

	protected void CloseInternal(DisconnectReason reason)
	{
		try
		{
			if (!m_Disposed.IsOn && !(_connection == HSteamNetConnection.Invalid))
			{
				SteamNetworkingSockets.CloseConnection(_connection, 0, reason.ToString(), bEnableLinger: false);
				_connection = HSteamNetConnection.Invalid;
				connectionStatusChangedCallback?.Unregister();
				_isConnectedToServer = false;
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			_onClose(reason);
		}
	}

	protected void Flush()
	{
		try
		{
			if (m_Disposed.IsOn || (m_UnreliableSendQueue.IsEmpty && m_ReliableSendQueue.IsEmpty) || !m_Sending.On())
			{
				return;
			}
			long pOutMessageNumber;
			while (!m_UnreliableSendQueue.IsEmpty && !m_Disposed.IsOn)
			{
				FillSendBuffer(reliable: false);
				if (m_SendBufferContext.RemainSendSize == 0)
				{
					break;
				}
				IntPtr allBuffers = m_SendBufferContext.GetAllBuffers();
				EResult eResult = SteamNetworkingSockets.SendMessageToConnection(_connection, allBuffers, (uint)m_SendBufferContext.RemainSendSize, (m_SendBufferContext.RemainSendSize > 1200) ? 9 : 5, out pOutMessageNumber);
				if (eResult != EResult.k_EResultOK)
				{
					Logger.RError($"Failed to send message. Result: {eResult}, Size: {m_SendBufferContext.RemainSendSize}");
					m_Sending.Off();
					CloseInternal(DisconnectReason.ConnectionError);
					return;
				}
				FreeIntPtr(allBuffers);
				m_SendBufferContext.Reset();
			}
			while (!m_ReliableSendQueue.IsEmpty && !m_Disposed.IsOn)
			{
				FillSendBuffer(reliable: true);
				if (m_SendBufferContext.RemainSendSize == 0)
				{
					break;
				}
				IntPtr allBuffers2 = m_SendBufferContext.GetAllBuffers();
				EResult eResult2 = SteamNetworkingSockets.SendMessageToConnection(_connection, allBuffers2, (uint)m_SendBufferContext.RemainSendSize, 9, out pOutMessageNumber);
				if (eResult2 != EResult.k_EResultOK)
				{
					Logger.RError($"Failed to send message. Result: {eResult2}, Size: {m_SendBufferContext.RemainSendSize}");
					m_Sending.Off();
					CloseInternal(DisconnectReason.ConnectionError);
					return;
				}
				FreeIntPtr(allBuffers2);
				m_SendBufferContext.Reset();
			}
			SendCallback();
		}
		catch (Exception ex)
		{
			m_Sending.Off();
			Logger.RWarn(ex.Message);
			CloseInternal(DisconnectReason.Undefined);
		}
	}

	private void FillSendBuffer(bool reliable)
	{
		int num = 0;
		ConcurrentQueue<IMsg> concurrentQueue = (reliable ? m_ReliableSendQueue : m_UnreliableSendQueue);
		IMsg result;
		while (concurrentQueue.TryPeek(out result))
		{
			byte[] array = SerializeMessage(result);
			int num2 = array.Length + 8;
			if (m_SendBufferContext.EndPosition + num2 > 1048576)
			{
				break;
			}
			IMsg result2;
			if (num == 0)
			{
				concurrentQueue.TryDequeue(out result2);
				if (!m_SendBufferContext.Write(BitConverter.GetBytes(num2)) || !m_SendBufferContext.Write(BitConverter.GetBytes((int)result.msgType)) || !m_SendBufferContext.Write(array, 0, array.Length))
				{
					throw new Exception($"Failed to write to send buffer. msgType : {result.msgType} / size : {num2} / remainSize : {m_SendBufferContext.RemainWriteSize}");
				}
				_onSend(num2, result.msgType.ToString());
				num += num2;
				if (num2 >= 1200)
				{
					break;
				}
			}
			else
			{
				if (num + num2 > 1200)
				{
					break;
				}
				concurrentQueue.TryDequeue(out result2);
				if (!m_SendBufferContext.Write(BitConverter.GetBytes(num2)) || !m_SendBufferContext.Write(BitConverter.GetBytes((int)result.msgType)) || !m_SendBufferContext.Write(array, 0, array.Length))
				{
					throw new Exception($"Failed to write to send buffer. msgType : {result.msgType} / size : {num2} / remainSize : {m_SendBufferContext.RemainWriteSize}");
				}
				_onSend(num2, result.msgType.ToString());
				num += num2;
			}
		}
	}

	private byte[] SerializeMessage(IMsg msg)
	{
		if (msg.parsingType == ParsingType.Json)
		{
			string s = JsonConvert.SerializeObject(msg);
			return Encoding.UTF8.GetBytes(s);
		}
		return Hub.s.msggenerator.Serialize(msg);
	}

	public void SendCallback()
	{
		try
		{
			m_Sending.Off();
			if (m_ReliableSendQueue.IsEmpty && m_UnreliableSendQueue.IsEmpty && m_ClosePending.IsOn)
			{
				CloseInternal(DisconnectReason.ByServer);
			}
		}
		catch (Exception ex)
		{
			m_Sending.Off();
			Logger.RWarn(ex.Message);
			CloseInternal(DisconnectReason.Undefined);
		}
	}

	public static void FreeIntPtr(IntPtr ptr)
	{
		if (ptr != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(ptr);
		}
	}

	public static SteamNetworkingConfigValue_t[] GetSteamNetworkingConfig()
	{
		return new SteamNetworkingConfigValue_t[1]
		{
			new SteamNetworkingConfigValue_t
			{
				m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
				m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
				m_val = new SteamNetworkingConfigValue_t.OptionValue
				{
					m_int32 = 1048576
				}
			}
		};
	}

	public static void LogCloseReason(string originName, SteamNetConnectionStatusChangedCallback_t callback)
	{
		if (Enum.IsDefined(typeof(ESteamNetConnectionEnd), callback.m_info.m_eEndReason))
		{
			ESteamNetConnectionEnd eEndReason = (ESteamNetConnectionEnd)callback.m_info.m_eEndReason;
			Logger.RWarn($"[{originName}] {callback.m_info.m_eState} - End: {eEndReason}({callback.m_info.m_eEndReason}), {callback.m_info.m_szEndDebug}");
		}
		else
		{
			Logger.RWarn($"[{originName}] {callback.m_info.m_eState} - End: {callback.m_info.m_eEndReason}, {callback.m_info.m_szEndDebug}");
		}
	}

	public static void LogConnectionRealTimeStatus(string originName, HSteamNetConnection connection)
	{
		if (!(connection == HSteamNetConnection.Invalid))
		{
			SteamNetConnectionRealTimeStatus_t pStatus = default(SteamNetConnectionRealTimeStatus_t);
			SteamNetConnectionRealTimeLaneStatus_t pLanes = default(SteamNetConnectionRealTimeLaneStatus_t);
			EResult connectionRealTimeStatus = SteamNetworkingSockets.GetConnectionRealTimeStatus(connection, ref pStatus, 0, ref pLanes);
			if (connectionRealTimeStatus == EResult.k_EResultOK)
			{
				Logger.RWarn($"[{originName}] RTT: {pStatus.m_nPing} ms, LocalQ: {pStatus.m_flConnectionQualityLocal * 100f}%, RemoteQ: {pStatus.m_flConnectionQualityRemote * 100f}%, Send: {pStatus.m_flOutBytesPerSec} B/s, Recv: {pStatus.m_flInBytesPerSec} B/s");
			}
			else
			{
				Logger.RWarn($"[{originName}] get real status fail. {connectionRealTimeStatus}");
			}
		}
	}
}
