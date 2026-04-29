using System;
using ReluProtocol.Enum;
using Steamworks;

public class SDRListenSocket : IDisposable
{
	private OnAcceptSDR _onAccept;

	private OnListenError _onListenError;

	private OnDisconnectSDR _onDisconnect;

	private int _port;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	private HSteamListenSocket _listenSocket = HSteamListenSocket.Invalid;

	private Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChangedCallback;

	public SDRListenSocket(int port, OnAcceptSDR onAccept, OnDisconnectSDR onDisconnect, OnListenError onListenError)
	{
		_port = port;
		_onAccept = onAccept;
		_onListenError = onListenError;
		_onDisconnect = onDisconnect;
	}

	public bool Start()
	{
		SteamNetworkingConfigValue_t[] steamNetworkingConfig = SDRSocket.GetSteamNetworkingConfig();
		_listenSocket = SteamNetworkingSockets.CreateListenSocketP2P(_port, 0, steamNetworkingConfig);
		if (_listenSocket == HSteamListenSocket.Invalid)
		{
			_onListenError(new Exception("failed to create listen socket"));
			return false;
		}
		connectionStatusChangedCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionStatusChanged);
		return true;
	}

	private void OnConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t callback)
	{
		if (callback.m_info.m_hListenSocket != _listenSocket)
		{
			return;
		}
		switch (callback.m_info.m_eState)
		{
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
			_onAccept(callback.m_hConn);
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
			SDRSocket.LogCloseReason("SDRListenSocket", callback);
			SDRSocket.LogConnectionRealTimeStatus("SDRListenSocket", callback.m_hConn);
			_onDisconnect(callback.m_hConn, DisconnectReason.ByClient);
			SteamNetworkingSockets.CloseConnection(callback.m_hConn, 0, "Disconnected", bEnableLinger: false);
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
			SDRSocket.LogCloseReason("SDRListenSocket", callback);
			SDRSocket.LogConnectionRealTimeStatus("SDRListenSocket", callback.m_hConn);
			_onDisconnect(callback.m_hConn, DisconnectReason.ByServer);
			SteamNetworkingSockets.CloseConnection(callback.m_hConn, 0, "Disconnected", bEnableLinger: false);
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None:
			if (callback.m_eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
			{
				_onDisconnect(callback.m_hConn, DisconnectReason.ByServer);
			}
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_FindingRoute:
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
			break;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && _disposed.On())
		{
			SteamNetworkingSockets.CloseListenSocket(_listenSocket);
			if (connectionStatusChangedCallback != null)
			{
				connectionStatusChangedCallback.Unregister();
				connectionStatusChangedCallback = null;
			}
		}
	}
}
