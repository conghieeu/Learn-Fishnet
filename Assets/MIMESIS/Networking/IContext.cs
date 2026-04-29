using System;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public interface IContext : IDisposable
{
	int ServerID { get; }

	int GetSessionID();

	void BeginClose(DisconnectReason reason);

	SendResult Send(IMsg msg);

	void Update();

	SendResult SendError<TResponse>(MsgErrorCode errorCode, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where TResponse : IResMsg, new();
}
