using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;

public class RUDPConnectionProvider : IDisposable, INatPunchListener
{
	private EventBasedNetListener _listener;

	private EventBasedNatPunchListener _natPunchListener;

	private NetManager _manager;

	private OnAcceptRUDP _onAccept;

	private OnListenError _onListenError;

	private OnDisconnect _onDisconnect;

	private OnReceive _onReceive;

	private int _port;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	private string _holePunchInternalAddr = string.Empty;

	private int _holePunchInternalPort;

	private string _holePunchExternalAddr = string.Empty;

	private int _holePunchExternalPort;

	private string _holePunchTargetAddr = string.Empty;

	private int _holePunchTargetPort;

	public RUDPConnectionProvider(int port, OnAcceptRUDP onAccept, OnReceive onReceive, OnDisconnect onDisconnect, OnListenError onListenError)
	{
		_port = port;
		_listener = new EventBasedNetListener();
		_natPunchListener = new EventBasedNatPunchListener();
		_onAccept = onAccept;
		_onListenError = onListenError;
		_onDisconnect = onDisconnect;
		_onReceive = onReceive;
		_manager = new NetManager(_listener)
		{
			ReuseAddress = true,
			IPv6Enabled = false,
			AutoRecycle = true,
			DontRoute = true,
			NatPunchEnabled = true
		};
	}

	public void Start()
	{
		_listener.ConnectionRequestEvent += delegate(ConnectionRequest request)
		{
			if (_manager.ConnectedPeersCount >= 16)
			{
				request.Reject();
				_onListenError(new Exception("Max connection reached"));
			}
			else
			{
				request.AcceptIfKey("RELURELU");
			}
		};
		_listener.PeerConnectedEvent += delegate(NetPeer peer)
		{
			_onAccept(peer);
		};
		_listener.NetworkErrorEvent += delegate(IPEndPoint endPoint, SocketError socketError)
		{
			Logger.RError($"NetworkErrorEvent: {socketError}");
			_onListenError(new Exception($"NetworkErrorEvent: {socketError}"));
		};
		_listener.NetworkReceiveEvent += delegate(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
		{
			_onReceive(peer, reader, channel, deliveryMethod);
		};
		_listener.PeerDisconnectedEvent += delegate(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			_onDisconnect(peer);
		};
		_natPunchListener.NatIntroductionRequest += delegate
		{
		};
		_natPunchListener.NatIntroductionSuccess += delegate(IPEndPoint addr, NatAddressType addrType, string token)
		{
			if (addrType == NatAddressType.External)
			{
				_holePunchExternalAddr = addr.Address.ToString();
				_holePunchExternalPort = addr.Port;
			}
			else
			{
				_holePunchInternalAddr = addr.Address.ToString();
				_holePunchInternalPort = addr.Port;
			}
		};
		_manager.NatPunchModule.Init(_natPunchListener);
		if (!_manager.Start("0.0.0.0", "0:0:0:0:0:0:0:0", _port))
		{
			_onListenError(new Exception("Failed to start"));
		}
	}

	public void Update()
	{
		_manager.NatPunchModule.PollEvents();
		_manager.PollEvents();
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
		if (_listener != null)
		{
			_listener.ConnectionRequestEvent -= delegate(ConnectionRequest request)
			{
				request.AcceptIfKey("RELURELU");
			};
			_listener.PeerConnectedEvent -= delegate(NetPeer peer)
			{
				_onAccept(peer);
			};
			_listener = null;
		}
		_manager.Stop();
	}

	public bool ExistHolePunchTarget()
	{
		if (_holePunchTargetAddr != string.Empty)
		{
			return _holePunchTargetPort != 0;
		}
		return false;
	}

	public void SetHolePunchTarget(string addr, int port)
	{
		_holePunchTargetAddr = addr;
		_holePunchTargetPort = port;
	}

	public void CollectHolepunchAddress(string token)
	{
		if (_holePunchTargetAddr == string.Empty || _holePunchTargetPort == 0)
		{
			Logger.RError("Hole punch target is not set");
		}
		else if (token.Length > 256)
		{
			Logger.RError($"Hole punch token size over. token.Length({token.Length}) > NatPunchModule.MaxTokenLength({256}). ");
		}
		else
		{
			_manager.NatPunchModule.SendNatIntroduceRequest(_holePunchTargetAddr, _holePunchTargetPort, token);
		}
	}

	public void OnNatIntroductionRequest(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, string token)
	{
	}

	public void OnNatIntroductionSuccess(IPEndPoint targetEndPoint, NatAddressType type, string token)
	{
	}

	public (string, int, string, int) GetHolePunchInfo()
	{
		return (_holePunchExternalAddr, _holePunchExternalPort, _holePunchInternalAddr, _holePunchInternalPort);
	}
}
