using System;
using System.Collections.Generic;
using LiteNetLib;
using ReluProtocol.Enum;

public class RUDPServer : IDisposable
{
	private RUDPConnectionProvider _connectionProvider;

	private OnSessionCreate _onSessionCreate;

	private OnSessionClose _onSessionClose;

	private OnSessionError _onSessionError;

	public SessionManager _sessionManager;

	public ServerDispatchManager _dispatchManager;

	private readonly INetworkBufferPool m_NetworkBufferPool;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	public readonly string IpAddress = string.Empty;

	public readonly int Port;

	private Dictionary<NetPeer, RUDPAcceptSession> _acceptSessions = new Dictionary<NetPeer, RUDPAcceptSession>();

	private List<NetPeer> _reservedRemoveSessions = new List<NetPeer>();

	private long _holePunchLastCheckTick;

	private string _holePunchToken = string.Empty;

	private long _holePunchLastUpdateTick;

	private bool _OnRegistProgress;

	public RUDPServer(SessionManager sessionManager, ServerDispatchManager dispatcherManager, int port, OnSessionCreate onSessionCreate, OnSessionClose onSessionClose, OnSessionError onSessionError, OnHolePunchUpdateDelegate onUpdateDelegate, INetworkBufferPool? networkBufferPool = null)
	{
		_connectionProvider = new RUDPConnectionProvider(port, OnAccept, OnReceive, OnDisconnect, OnListenError);
		_onSessionCreate = onSessionCreate;
		_onSessionClose = onSessionClose;
		_onSessionError = onSessionError;
		_dispatchManager = dispatcherManager;
		_sessionManager = sessionManager;
		if (networkBufferPool == null)
		{
			networkBufferPool = new NetworkBufferAllocator(4194304, 1048576);
		}
		m_NetworkBufferPool = networkBufferPool;
	}

	private void OnAccept(NetPeer netPeer)
	{
		RUDPAcceptSession rUDPAcceptSession = new RUDPAcceptSession(_sessionManager.GetNewSessionID(), _dispatchManager, netPeer, _onSessionClose, _onSessionError);
		_acceptSessions.Add(netPeer, rUDPAcceptSession);
		_onSessionCreate(rUDPAcceptSession);
	}

	public void OnReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
	{
		if (!_acceptSessions.TryGetValue(peer, out RUDPAcceptSession value))
		{
			Logger.RError("Session not found");
		}
		else
		{
			value.OnReceive(peer, reader, channel, deliveryMethod);
		}
	}

	public void Update()
	{
		_connectionProvider.Update();
		foreach (RUDPAcceptSession value in _acceptSessions.Values)
		{
			value.Update();
			if (value.PendingClose)
			{
				_reservedRemoveSessions.Add(value.GetRemoteEndPoint() as NetPeer);
			}
		}
		if (_reservedRemoveSessions.Count <= 0)
		{
			return;
		}
		foreach (NetPeer reservedRemoveSession in _reservedRemoveSessions)
		{
			_acceptSessions.Remove(reservedRemoveSession);
		}
		_reservedRemoveSessions.Clear();
	}

	public void OnDisconnect(NetPeer peer)
	{
		if (!_acceptSessions.TryGetValue(peer, out RUDPAcceptSession value))
		{
			Logger.RError("Session not found");
		}
		else
		{
			value.BeginClose(ReluProtocol.Enum.DisconnectReason.ByClient);
		}
	}

	public void Start()
	{
		_connectionProvider.Start();
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
		_connectionProvider.Dispose();
		foreach (RUDPAcceptSession value in _acceptSessions.Values)
		{
			value.Dispose();
		}
		_acceptSessions.Clear();
	}

	public void SetHolePunchTarget(string addr, int port, string token)
	{
		_connectionProvider.SetHolePunchTarget(addr, port);
		_holePunchToken = token;
	}
}
