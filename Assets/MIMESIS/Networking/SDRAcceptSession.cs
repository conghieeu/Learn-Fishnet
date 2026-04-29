using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using Steamworks;

public class SDRAcceptSession : ISession, IDisposable
{
	private OnSessionClose m_OnSessionClose;

	private OnSessionError m_OnSessionError;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	public int ID { get; }

	public ServerDispatchManager DispatchManagerCtx { get; private set; }

	public SDRSocket _connection { get; private set; }

	public IContext? Context { get; private set; }

	public SDRAcceptSession(int sessionID, INetworkBufferPool networkBufferPool, ServerDispatchManager dispatcherManager, HSteamNetConnection connection, OnSessionClose onSessionClose, OnSessionError onSessionError)
	{
		_connection = new SDRSocket(networkBufferPool, OnSend, OnReceive, OnConnect, OnClose, OnSocketError);
		_connection.SetConnection(connection);
		ID = sessionID;
		DispatchManagerCtx = dispatcherManager;
		m_OnSessionClose = (OnSessionClose)Delegate.Combine(m_OnSessionClose, onSessionClose);
		m_OnSessionError = (OnSessionError)Delegate.Combine(m_OnSessionError, onSessionError);
	}

	private void OnConnect()
	{
	}

	private void OnSocketError(Exception? ex, SocketError e)
	{
		if (ex != null)
		{
			m_OnSessionError(Context, ex);
		}
	}

	private void OnClose(DisconnectReason reason)
	{
		m_OnSessionClose?.Invoke(Context, reason);
		_connection.Dispose();
	}

	private void OnSend(int byteSent, string pktMsgName)
	{
		DispatchManagerCtx.OnPreSendEventHandler?.Invoke(byteSent, pktMsgName);
	}

	private void OnReceive(int length, int pktType, in ArraySegment<byte> data)
	{
		if (!DispatchManagerCtx.Dispatch(this, length, pktType, in data))
		{
			BeginClose(DisconnectReason.PacketError);
		}
	}

	public void BeginClose(DisconnectReason reason)
	{
		_connection.Close(reason, immediately: false);
	}

	public void Close(DisconnectReason reason)
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
		Logger.RError($"SendError, Msg : {typeof(TResponse).Name}, errorCode: {errorCode}");
		TResponse val = new TResponse
		{
			errorCode = errorCode
		};
		return SendLink(val);
	}

	public void Update()
	{
		_connection.Update();
	}

	public void Connect(IConnection connection)
	{
		Logger.RError("Connect is not supported in SDRAcceptSession");
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

	public IPEndPoint? GetRemoteEndPoint()
	{
		return null;
	}
}
