using System;
using System.Net;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class UDPSession : ISession, IDisposable
{
	private IPEndPoint _remoteEndPoint;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	private UDPSocket _udpSocket;

	public int ID { get; private set; }

	public IContext? Context { get; private set; }

	public UDPSession(int sessionID, IPEndPoint remoteEndPoint, UDPSocket udpSocket)
	{
		ID = sessionID;
		_remoteEndPoint = remoteEndPoint;
		_udpSocket = udpSocket;
	}

	public void BeginClose(DisconnectReason reason)
	{
	}

	public HostInfo GetHostInfo()
	{
		return new HostInfo
		{
			Host = _remoteEndPoint.Address.ToString(),
			Port = _remoteEndPoint.Port
		};
	}

	public void Close(DisconnectReason reason)
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			_ = _disposed.IsOn;
		}
	}

	public IPEndPoint GetRemoteEndPoint()
	{
		return _remoteEndPoint;
	}

	public bool IsRunning()
	{
		return !_disposed.IsOn;
	}

	public SendResult SendError<TResponse>(MsgErrorCode errorCode, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where TResponse : IResMsg, new()
	{
		Logger.RError($"SendError, errorCode: {errorCode} / file: {file} / line: {line}");
		TResponse val = new TResponse
		{
			errorCode = errorCode
		};
		return SendLink(val);
	}

	public SendResult SendLink(params IMsg[] msgs)
	{
		return _udpSocket.SendTo(_remoteEndPoint, msgs);
	}

	public bool SendLoopback(IMsg msg)
	{
		throw new NotImplementedException();
	}

	public void SetContext(IContext ctx)
	{
		if (Context != null)
		{
			Logger.RError($"Context is already set, old: {Context}, new: {ctx}");
		}
		Context = ctx;
	}

	public void Connect(IConnection connection)
	{
		Logger.RError("UDPSession does not support Connect");
	}
}
