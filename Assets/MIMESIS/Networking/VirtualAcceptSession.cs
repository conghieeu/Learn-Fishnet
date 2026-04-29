using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class VirtualAcceptSession : ISession
{
	private VirtualClientSession? _clientSession;

	private ServerDispatchManager _dispatchManagerCtx;

	private Queue<IMsg> _recvQueue = new Queue<IMsg>();

	public int ID => 999999999;

	public IContext? Context { get; private set; }

	public VirtualAcceptSession(ServerDispatchManager dispatcherManager)
	{
		_dispatchManagerCtx = dispatcherManager;
	}

	public void SetClientSession(VirtualClientSession? session)
	{
		_clientSession = session;
	}

	public void BeginClose(DisconnectReason reason)
	{
		_clientSession?.OnClose(reason);
	}

	public void Close(DisconnectReason reason)
	{
		_clientSession?.OnClose(reason);
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
		if (_clientSession == null)
		{
			return SendResult.NotConnect;
		}
		_clientSession.OnReceive(new TResponse
		{
			errorCode = errorCode
		});
		return SendResult.Success;
	}

	public SendResult SendLink(params IMsg[] msgs)
	{
		if (_clientSession == null)
		{
			return SendResult.NotConnect;
		}
		for (int i = 0; i < msgs.Length; i++)
		{
			_clientSession.OnReceive(msgs[i]);
		}
		return SendResult.Success;
	}

	public void OnReceive(IMsg msg)
	{
		if (Context != null)
		{
			_recvQueue.Enqueue(msg);
		}
	}

	public void OnUpdate()
	{
		IMsg result;
		while (_recvQueue.TryDequeue(out result))
		{
			_dispatchManagerCtx.Dispatch(this, result);
		}
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
		Logger.RError("VirtualAcceptSession does not support Connect");
	}
}
