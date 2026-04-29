using System.Net;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class BulkSession : ISession
{
	public int ID => 0;

	public IContext? Context { get; private set; }

	public static BulkSession Instance { get; } = new BulkSession();

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
		return true;
	}

	public SendResult SendError<TResponse>(MsgErrorCode errorCode, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where TResponse : IResMsg, new()
	{
		return SendResult.Success;
	}

	public SendResult SendLink(params IMsg[] msgs)
	{
		return SendResult.Success;
	}

	public bool SendLoopback(IMsg msg)
	{
		return true;
	}

	public void SetContext(IContext ctx)
	{
		Context = ctx;
	}

	public void Connect(IConnection connection)
	{
	}
}
