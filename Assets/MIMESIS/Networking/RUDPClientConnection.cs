using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using ReluProtocol.Enum;

public class RUDPClientConnection : RUDPAcceptConnection
{
	private OnRUDPConnect _onRUDPConnect;

	private EventBasedNetListener _listener;

	private NetManager _manager;

	public RUDPClientConnection(OnSendPacket onSend, OnRecvPacket onRecv, OnRUDPConnect onConnect, OnRUDPClose onClose, OnRUDPError onError)
		: base(onSend, onRecv, onClose, onError)
	{
		_onRUDPConnect = onConnect;
		_listener = new EventBasedNetListener();
		_manager = new NetManager(_listener)
		{
			AutoRecycle = true,
			ChannelsCount = 1,
			BroadcastReceiveEnabled = true
		};
		if (!_manager.Start())
		{
			throw new Exception("Failed to start RUDPClientConnection");
		}
	}

	public bool Connect(IPEndPoint ipEndPoint)
	{
		try
		{
			_manager.Connect(ipEndPoint, "RELURELU");
			_listener.PeerConnectedEvent += delegate(NetPeer peer)
			{
				_onRUDPConnect();
				_netPeer = peer;
			};
			_listener.PeerDisconnectedEvent += delegate(NetPeer peer, DisconnectInfo disconnectInfo)
			{
				ReluProtocol.Enum.DisconnectReason reason = ReluProtocol.Enum.DisconnectReason.None;
				switch (disconnectInfo.Reason)
				{
				case LiteNetLib.DisconnectReason.RemoteConnectionClose:
					reason = ReluProtocol.Enum.DisconnectReason.ConnectionError;
					break;
				}
				CloseInternal(reason);
			};
			_listener.NetworkReceiveEvent += base.ReceiveCallback;
			_listener.NetworkErrorEvent += delegate(IPEndPoint endPoint, SocketError socketError)
			{
				Logger.RError($"Network error: {socketError}");
				_onError(null, socketError);
			};
			return true;
		}
		catch (Exception arg)
		{
			Logger.RError($"Connect failed for {ipEndPoint}\n message: {arg}");
			return false;
		}
	}

	public override void Update()
	{
		base.Update();
		_manager.PollEvents();
	}

	protected override void Dispose(bool isDisposing)
	{
		base.Dispose(isDisposing);
		if (isDisposing)
		{
			_manager.Stop();
		}
	}
}
