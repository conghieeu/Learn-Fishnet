using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class VirtualClientSession : VirtualSession, ISession
{
	public VirtualAcceptSession? _serverSession;

	public OnRecv _onRecv;

	private Queue<IMsg> _recvQueue = new Queue<IMsg>();

	private OnConnect _onConnect;

	private OnClose _onClose;

	public int ID => 999999999;

	public IContext? Context => null;

	public VirtualClientSession(OnRecv onRecv, OnConnect onConnect, OnClose onClose)
	{
		_onRecv = onRecv;
		_onConnect = onConnect;
		_onClose = onClose;
	}

	public void SetServerSession(VirtualAcceptSession serverSession)
	{
		_serverSession = serverSession;
	}

	public void BeginClose(DisconnectReason reason)
	{
		Disconnect();
		_serverSession?.SetClientSession(null);
		_serverSession = null;
		_recvQueue.Clear();
		OnClose(reason);
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
		if (_serverSession == null)
		{
			Logger.RError("serverSession is null on VirtualClientSession - SendError");
			return SendResult.NotConnect;
		}
		_serverSession.OnReceive(new TResponse
		{
			errorCode = errorCode
		});
		return SendResult.Success;
	}

	public SendResult SendLink(params IMsg[] msgs)
	{
		if (_serverSession == null)
		{
			return SendResult.NotConnect;
		}
		for (int i = 0; i < msgs.Length; i++)
		{
			_serverSession.OnReceive(msgs[i]);
		}
		return SendResult.Success;
	}

	public void OnReceive(IMsg msg)
	{
		_recvQueue.Enqueue(msg);
	}

	public void Update()
	{
		IMsg result;
		while (_recvQueue.TryDequeue(out result))
		{
			_onRecv(result);
		}
	}

	public bool SendLoopback(IMsg msg)
	{
		_recvQueue.Enqueue(msg);
		return true;
	}

	public void SetContext(IContext ctx)
	{
	}

	public void OnClose(DisconnectReason reason)
	{
		_onClose(reason);
	}

	public void Disconnect()
	{
		if (!(Hub.s == null) && Hub.s.vworld != null && !Hub.s.vworld.DisconnectVirtualSession(this))
		{
			Logger.RError("serverSession is null on VirtualClientSession - Connect");
		}
	}

	public void Connect(IConnection connection)
	{
		if (Hub.s == null || Hub.s.vworld == null)
		{
			return;
		}
		if (!Hub.s.vworld.Running)
		{
			Logger.RError("VWorld is not running");
			return;
		}
		_serverSession = Hub.s.vworld.ConnectVirtualSession(this);
		if (_serverSession == null)
		{
			Logger.RError("serverSession is null on VirtualClientSession - Connect");
		}
		_onConnect();
	}
}
