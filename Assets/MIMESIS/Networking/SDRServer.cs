using System;
using System.Collections.Generic;
using ReluProtocol.Enum;
using Steamworks;
using UnityEngine;

public class SDRServer : IDisposable
{
	private SDRListenSocket _listenSocket;

	private OnSessionCreate _onSessionCreate;

	private OnSessionClose _onSessionClose;

	private OnSessionClose _onSessionCloseDummy;

	private OnSessionError _onSessionError;

	public SessionManager _sessionManager;

	public ServerDispatchManager _dispatchManager;

	private readonly INetworkBufferPool m_NetworkBufferPool;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	public readonly string IpAddress = string.Empty;

	public readonly int Port;

	private HSteamNetPollGroup pollGroup = HSteamNetPollGroup.Invalid;

	private Dictionary<HSteamNetConnection, SDRAcceptSession> Sessions = new Dictionary<HSteamNetConnection, SDRAcceptSession>();

	public SDRServer(SessionManager sessionManager, ServerDispatchManager dispatcherManager, int port, OnSessionCreate onSessionCreate, OnSessionClose onSessionClose, OnSessionError onSessionError)
	{
		_listenSocket = new SDRListenSocket(port, OnAccept, OnDisconnect, OnListenError);
		_onSessionCreate = onSessionCreate;
		_onSessionClose = onSessionClose;
		_onSessionError = onSessionError;
		_dispatchManager = dispatcherManager;
		_sessionManager = sessionManager;
		m_NetworkBufferPool = new NetworkBufferAllocator(4194304, 1048576);
	}

	private void OnAccept(HSteamNetConnection connection)
	{
		if (SteamNetworkingSockets.AcceptConnection(connection) != EResult.k_EResultOK)
		{
			SteamNetworkingSockets.CloseConnection(connection, 0, "Failed to accept connection", bEnableLinger: false);
		}
		else if (!SteamNetworkingSockets.SetConnectionPollGroup(connection, pollGroup))
		{
			Logger.RError("failed to set poll group");
			SteamNetworkingSockets.CloseConnection(connection, 0, "Failed to set poll group", bEnableLinger: false);
		}
		else
		{
			SDRAcceptSession sDRAcceptSession = new SDRAcceptSession(_sessionManager.GetNewSessionID(), m_NetworkBufferPool, _dispatchManager, connection, _onSessionCloseDummy, _onSessionError);
			Sessions.Add(connection, sDRAcceptSession);
			_onSessionCreate(sDRAcceptSession);
		}
	}

	public void Update()
	{
		foreach (SDRAcceptSession value in Sessions.Values)
		{
			value.Update();
		}
	}

	private void OnDisconnect(HSteamNetConnection connection, DisconnectReason disconnectReason)
	{
		if (Sessions.ContainsKey(connection))
		{
			if (Sessions.TryGetValue(connection, out SDRAcceptSession value) && value.Context != null)
			{
				_onSessionClose(value.Context, disconnectReason);
			}
			Sessions.Remove(connection);
		}
	}

	public void Start()
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("Steamworks 초기화 실패");
			return;
		}
		SteamNetworkingUtils.InitRelayNetworkAccess();
		if (_listenSocket.Start())
		{
			pollGroup = SteamNetworkingSockets.CreatePollGroup();
		}
	}

	private void OnListenError(Exception ex)
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposing || !_disposed.On())
		{
			return;
		}
		_listenSocket.Dispose();
		Hub.s.steamInviteDispatcher.LeaveLobby();
		foreach (SDRAcceptSession value in Sessions.Values)
		{
			value.Dispose();
		}
		Sessions.Clear();
		if (pollGroup != HSteamNetPollGroup.Invalid)
		{
			SteamNetworkingSockets.DestroyPollGroup(pollGroup);
		}
	}
}
