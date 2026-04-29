using System;
using System.Net.Sockets;
using ReluNetwork.ConstEnum;
using ReluProtocol;

public abstract class ISocket : IDisposable
{
	protected Socket m_Socket;

	protected OnSocketError m_OnSocketError;

	protected STSocketOption m_SocketOption;

	protected AtomicFlag m_Disposed = new AtomicFlag(value: false);

	public bool IsDisposed => m_Disposed.IsOn;

	protected ISocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, STSocketOption option, OnSocketError e)
	{
		m_Socket = new Socket(addressFamily, socketType, protocolType);
		m_OnSocketError = e;
		m_SocketOption = option;
	}

	protected ISocket(Socket socket, STSocketOption option, OnSocketError e)
	{
		m_Socket = socket;
		m_SocketOption = option;
		m_OnSocketError = e;
	}

	public virtual void ApplySocketOption()
	{
		if (m_SocketOption.NoDelay)
		{
			m_Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, optionValue: true);
		}
		if (m_SocketOption.DontLinger)
		{
			m_Socket.LingerState = new LingerOption(enable: true, 10);
		}
		if (m_SocketOption.ReuseAddr)
		{
			m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
		}
		if (m_SocketOption.KeepAlive)
		{
			m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, optionValue: true);
		}
		if (m_SocketOption.Broadcast)
		{
			m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, optionValue: true);
		}
	}

	public abstract void Dispose();

	protected abstract void Dispose(bool disposing);

	public abstract SendResult Send(IMsg[] msgs);
}
