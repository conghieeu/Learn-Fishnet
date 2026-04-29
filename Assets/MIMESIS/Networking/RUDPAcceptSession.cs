using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using LiteNetLib;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class RUDPAcceptSession : ISession
{
	private OnSessionClose m_OnSessionClose;

	private OnSessionError m_OnSessionError;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	public int ID { get; }

	public ServerDispatchManager DispatchManagerCtx { get; private set; }

	public RUDPAcceptConnection _connection { get; private set; }

	public IContext? Context { get; private set; }

	public bool PendingClose => _connection.PendingClose;

	public RUDPAcceptSession(int sessionID, ServerDispatchManager dispatcherManager, NetPeer netPeer, OnSessionClose onSessionClose, OnSessionError onSessionError)
	{
		_connection = new RUDPAcceptConnection(OnSend, OnReceive, OnClose, OnSocketError);
		_connection.SetNetPeer(netPeer);
		ID = sessionID;
		DispatchManagerCtx = dispatcherManager;
		m_OnSessionClose = (OnSessionClose)Delegate.Combine(m_OnSessionClose, onSessionClose);
		m_OnSessionError = (OnSessionError)Delegate.Combine(m_OnSessionError, onSessionError);
	}

	private void OnSocketError(Exception? ex, SocketError e)
	{
		if (ex != null)
		{
			m_OnSessionError(Context, ex);
		}
	}

	private void OnClose(ReluProtocol.Enum.DisconnectReason reason)
	{
		m_OnSessionClose(Context, reason);
	}

	private void OnSend(int byteSent, string pktMsgName)
	{
		DispatchManagerCtx.OnPreSendEventHandler?.Invoke(byteSent, pktMsgName);
	}

	private void OnReceive(int length, int pktType, in ArraySegment<byte> data)
	{
		if (!DispatchManagerCtx.Dispatch(this, length, pktType, in data))
		{
			BeginClose(ReluProtocol.Enum.DisconnectReason.PacketError);
		}
	}

	public void OnReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
	{
		_connection.ReceiveCallback(peer, reader, channel, deliveryMethod);
	}

	public void BeginClose(ReluProtocol.Enum.DisconnectReason reason)
	{
		_connection.Close(reason, immediately: false);
	}

	public void Close(ReluProtocol.Enum.DisconnectReason reason)
	{
		_connection.Close(reason, immediately: true);
	}

	public bool IsRunning()
	{
		return _connection.IsRunning();
	}

	public SendResult SendLink(params IMsg[] msgs)
	{
		if (IsRunning())
		{
			return _connection.Send(msgs);
		}
		return SendResult.NotConnect;
	}

	public bool SendLoopback(IMsg msg)
	{
		return DispatchManagerCtx.LoopbackLink(this, msg);
	}

	public void SetContext(IContext context)
	{
		Context = context;
	}

	public SendResult SendError<TResponse>(MsgErrorCode errorCode, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where TResponse : IResMsg, new()
	{
		Logger.RError($"SendError, errorCode: {errorCode} file: {file} line: {line}");
		TResponse val = new TResponse
		{
			errorCode = errorCode
		};
		return SendLink(val);
	}

	public IPEndPoint? GetRemoteEndPoint()
	{
		return _connection.GetRemoteEndPoint();
	}

	public void Connect(IConnection connection)
	{
		Logger.RError("RUDPAcceptSession does not support Connect");
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
			_connection.Dispose();
		}
	}

	public void Update()
	{
		_connection.Update();
	}
}
