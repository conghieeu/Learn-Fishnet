using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class RUDPClientSession : ISession, IDisposable
{
	private readonly OnConnect _onConnect;

	private readonly OnClose _onClose;

	private readonly RUDPClientConnection _client;

	public OnRecv _onRecv;

	public IPEndPoint? RemoteEndPoint;

	public int ID { get; private set; }

	public IContext? Context { get; private set; }

	public RUDPClientSession(OnRecv onRecv, OnConnect onConnect, OnClose onClose)
	{
		_onRecv = onRecv;
		_onConnect = onConnect;
		_onClose = onClose;
		ID = 9999999;
		_client = new RUDPClientConnection(OnSend, OnReceive, OnConnect, OnClose, OnError);
	}

	private void OnConnect()
	{
		_onConnect();
	}

	public void Connect(IConnection connection)
	{
		if (!(connection is IPAddrConnection iPAddrConnection))
		{
			Logger.RError("Invalid connection type");
			return;
		}
		RemoteEndPoint = new IPEndPoint(IPAddress.Parse(iPAddrConnection.IPAddr), iPAddrConnection.Port);
		_client.Connect(RemoteEndPoint);
	}

	public void Update()
	{
		_client.Update();
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
		return _client.Send(msgs);
	}

	public bool SendLoopback(IMsg msg)
	{
		return true;
	}

	public bool IsRunning()
	{
		return _client.IsRunning();
	}

	public void BeginClose(DisconnectReason reason)
	{
		_client.Close(reason, immediately: false);
	}

	public void Close(DisconnectReason reason)
	{
		_client.Close(reason, immediately: true);
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
		_client.Dispose();
	}

	public IPEndPoint? GetRemoteEndPoint()
	{
		return RemoteEndPoint;
	}
}
