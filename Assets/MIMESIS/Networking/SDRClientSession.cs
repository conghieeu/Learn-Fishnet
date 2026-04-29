using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using Steamworks;

public class SDRClientSession : ISession, IDisposable
{
	private readonly OnConnect _onConnect;

	private readonly OnClose _onClose;

	private readonly SDRSocket _connection;

	public OnRecv _onRecv;

	public int ID { get; private set; }

	public IContext? Context { get; private set; }

	public SDRClientSession(OnRecv onRecv, OnConnect onConnect, OnClose onClose)
	{
		_onRecv = onRecv;
		_onConnect = onConnect;
		_onClose = onClose;
		ID = 9999999;
		_connection = new SDRSocket(new NetworkBufferAllocator(4194304, 1048576), OnSend, OnReceive, OnConnect, OnClose, OnError);
	}

	private void OnConnect()
	{
		_onConnect();
	}

	public void Connect(IConnection connection)
	{
		if (!(connection is SteamIDConnection steamIDConnection))
		{
			Logger.RError("Invalid connection type");
			return;
		}
		SteamNetworkingIdentity identity = default(SteamNetworkingIdentity);
		identity.SetSteamID64(steamIDConnection.SteamID);
		if (!_connection.Connect(identity, steamIDConnection.Port))
		{
			_onClose(DisconnectReason.ConnectionError);
		}
	}

	public void Update()
	{
		_connection.Update();
	}

	private void OnSend(int byteSent, string pktName)
	{
	}

	private void OnReceive(int size, int pktType, in ArraySegment<byte> data)
	{
		IMsg msg = Hub.s.msggenerator.Deserialize((MsgType)pktType, in data);
		if (msg == null)
		{
			Logger.RError($"Deserialize failed, pktType: {pktType}");
		}
		else
		{
			_onRecv(msg);
		}
	}

	private void OnClose(DisconnectReason reason)
	{
		_onClose(reason);
	}

	private void OnError(Exception? ex, SocketError e)
	{
	}

	public SendResult SendLink(params IMsg[] msgs)
	{
		if (!IsRunning())
		{
			return SendResult.NotConnect;
		}
		return _connection.Send(msgs);
	}

	public bool SendLoopback(IMsg msg)
	{
		return true;
	}

	public bool IsRunning()
	{
		return _connection.IsRunning();
	}

	public void BeginClose(DisconnectReason reason)
	{
		_connection.Close(reason, immediately: false);
	}

	public void Close(DisconnectReason reason)
	{
		_connection.Close(reason, immediately: true);
	}

	public void SetContext(IContext ctx)
	{
		Context = ctx;
	}

	public SendResult SendError<TResponse>(MsgErrorCode errorCode, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where TResponse : IResMsg, new()
	{
		Logger.RError($"SendError, errorCode: {errorCode} / {file} / {line}");
		TResponse val = new TResponse
		{
			errorCode = errorCode
		};
		return SendLink(val);
	}

	public void Dispose()
	{
		_connection.Dispose();
	}

	public IPEndPoint? GetRemoteEndPoint()
	{
		return null;
	}
}
