using System.Net;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class InvalidSession : ISession
{
	public int ID => -1;

	public IContext? Context => null;

	public SendResult SendLink(params IMsg[] msgs)
	{
		return SendResult.Undefined;
	}

	public SendResult SendError<TResponse>(MsgErrorCode errorCode, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where TResponse : IResMsg, new()
	{
		return SendResult.Undefined;
	}

	public bool SendLoopback(IMsg msg)
	{
		return false;
	}

	public void SetContext(IContext ctx)
	{
	}

	public void BeginClose(DisconnectReason reason)
	{
	}

	public void Close(DisconnectReason reason)
	{
	}

	public IPEndPoint? GetRemoteEndPoint()
	{
		return null;
	}

	public bool IsRunning()
	{
		return false;
	}

	public void Connect(IConnection connection)
	{
	}
}
