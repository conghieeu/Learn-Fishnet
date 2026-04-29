using System.Net;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public interface ISession
{
	int ID { get; }

	IContext? Context { get; }

	SendResult SendLink(params IMsg[] msgs);

	SendResult SendError<TResponse>(MsgErrorCode errorCode, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where TResponse : IResMsg, new();

	bool SendLoopback(IMsg msg);

	void Connect(IConnection connection);

	void SetContext(IContext ctx);

	void BeginClose(DisconnectReason reason);

	bool IsRunning();

	IPEndPoint? GetRemoteEndPoint();
}
