using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Bifrost;
using Newtonsoft.Json;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class UDPSocket : ISocket
{
	private readonly ConcurrentQueue<(IPEndPoint, IMsg)> m_SendQueue = new ConcurrentQueue<(IPEndPoint, IMsg)>();

	private AsyncIOReceiveContext m_ReceiveBufferContext;

	private Dictionary<IPEndPoint, AsyncIOSendContext> m_SendBufferContexts = new Dictionary<IPEndPoint, AsyncIOSendContext>();

	private OnUDPSendTo m_OnSend;

	private OnUDPRecvFrom m_OnRecv;

	protected OnSocketClose m_OnClose;

	protected SocketAsyncEventArgs m_SendArgs = new SocketAsyncEventArgs();

	protected SocketAsyncEventArgs m_RecvArgs = new SocketAsyncEventArgs();

	protected AtomicFlag m_Sending = new AtomicFlag(value: false);

	private INetworkBufferPool m_NetworkBufferPool;

	public UDPSocket(int port, INetworkBufferPool networkBufferPool, STSocketOption option, OnUDPRecvFrom onRecv, OnUDPSendTo onSend, OnSocketClose onClose, OnSocketError onSocketError)
		: base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, option, onSocketError)
	{
		m_NetworkBufferPool = networkBufferPool;
		m_OnSend = (OnUDPSendTo)Delegate.Combine(m_OnSend, onSend);
		m_OnRecv = (OnUDPRecvFrom)Delegate.Combine(m_OnRecv, onRecv);
		m_OnClose = (OnSocketClose)Delegate.Combine(m_OnClose, onClose);
		m_ReceiveBufferContext = new AsyncIOReceiveContext(networkBufferPool);
		m_SendArgs.Completed += SendCallback;
		m_RecvArgs.Completed += ReceiveCallback;
		m_Socket.Bind(new IPEndPoint(IPAddress.Any, port));
	}

	public bool IsRunning()
	{
		if (!m_Disposed.IsOn)
		{
			return m_Socket.Connected;
		}
		return false;
	}

	public void StartReceive()
	{
		BeginReceive();
	}

	public void ReceiveCallback(object? sender, SocketAsyncEventArgs e)
	{
		int bytesTransferred = e.BytesTransferred;
		IPEndPoint iPEndPoint = e.RemoteEndPoint as IPEndPoint;
		try
		{
			if (bytesTransferred != 0)
			{
				DisconnectReason disconnectReason;
				if (e.SocketError != SocketError.Success || bytesTransferred < 0)
				{
					m_OnSocketError(null, e.SocketError);
					RebindSocket();
				}
				else if (!OnReceive(bytesTransferred, iPEndPoint, out disconnectReason))
				{
					Logger.RError($"OnReceive failed, endpoint : {iPEndPoint}, disconnectReason : {disconnectReason}");
					m_OnSocketError(null, e.SocketError);
				}
				else
				{
					BeginReceive();
				}
			}
		}
		catch (Exception ex)
		{
			m_OnSocketError(ex);
		}
	}

	private void RebindSocket()
	{
		if (m_Socket != null)
		{
			m_Socket.Dispose();
		}
		m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		m_Socket.Bind(new IPEndPoint(IPAddress.Any, 0));
	}

	public bool OnReceive(int bytesToRead, IPEndPoint endpoint, out DisconnectReason disconnectReason)
	{
		disconnectReason = DisconnectReason.None;
		if (!m_ReceiveBufferContext.AddEndPosition(bytesToRead))
		{
			Logger.RError($"(Context) Invalid Message Size - {bytesToRead} / {m_ReceiveBufferContext.RemainReadSize}");
			disconnectReason = DisconnectReason.ConnectionError;
			return false;
		}
		long num = m_ReceiveBufferContext.EndPosition;
		while (0 < num && 4 <= num)
		{
			int intValue = 0;
			Serializer.Load(m_ReceiveBufferContext.GetSegment(), m_ReceiveBufferContext.CurrentPosition, ref intValue);
			if (intValue > m_ReceiveBufferContext.Size)
			{
				Logger.RError($"(Context) Invalid Message Size - {intValue}. Exceeding the maximum packet size. {m_ReceiveBufferContext.Size}");
				disconnectReason = DisconnectReason.ConnectionError;
				return false;
			}
			if (num < intValue)
			{
				break;
			}
			if (intValue < 4)
			{
				Logger.RError($"(Context) Invalid Message Size - {intValue}");
				break;
			}
			m_ReceiveBufferContext.Seek(4);
			int intValue2 = 0;
			Serializer.Load(m_ReceiveBufferContext.GetSegment(), m_ReceiveBufferContext.CurrentPosition, ref intValue2);
			m_ReceiveBufferContext.Seek(4);
			int num2 = intValue - 8;
			var (flag, data) = m_ReceiveBufferContext.Read(num2);
			if (!flag || data.Array == null)
			{
				Logger.RError($"(Context) Invalid Message Size - {intValue} / {m_ReceiveBufferContext.RemainReadSize}");
				disconnectReason = DisconnectReason.Undefined;
				return false;
			}
			m_ReceiveBufferContext.Seek(num2);
			m_OnRecv(intValue, intValue2, in data, endpoint);
			num -= intValue;
		}
		m_ReceiveBufferContext.RemoveFrontBuffer();
		return true;
	}

	public SendResult SendTo(IPEndPoint endpoint, IMsg[] msgs)
	{
		if (msgs.Length == 0)
		{
			return SendResult.InvalidSize;
		}
		if (!IsRunning())
		{
			return SendResult.NotConnect;
		}
		for (int i = 0; i < msgs.Length; i++)
		{
			m_SendQueue.Enqueue((endpoint, msgs[i]));
		}
		BeginSend();
		return SendResult.Success;
	}

	public void SendCallback(object? sender, SocketAsyncEventArgs arg)
	{
		int bytesTransferred = arg.BytesTransferred;
		IPEndPoint key = arg.RemoteEndPoint as IPEndPoint;
		try
		{
			if (arg.SocketError != SocketError.Success)
			{
				return;
			}
			if (bytesTransferred <= 0)
			{
				RebindSocket();
				return;
			}
			if (!m_SendBufferContexts.TryGetValue(key, out AsyncIOSendContext value))
			{
				Logger.RError("SendCallback Error! sendBufferContext is null!user : (Context!.GetUserUID(), " + $"byteWritten : {bytesTransferred}");
				return;
			}
			if (bytesTransferred > value.RemainSendSize)
			{
				throw new Exception($"Exceeding the remaining transmission amount. byteWritten : {bytesTransferred} / sendingSize : {value.EndPosition}");
			}
			if (bytesTransferred < value.RemainSendSize)
			{
				Logger.RError("SendCallback Error! operation complete without transferring all bytes!" + $"byteWritten : {bytesTransferred} / sendingSize : {value.EndPosition}");
				value.AddCurrentPosition(bytesTransferred);
				return;
			}
			value.Reset();
			m_Sending.Off();
			if (!m_SendQueue.IsEmpty)
			{
				BeginSend();
			}
		}
		catch (Exception e)
		{
			m_Sending.Off();
			Logger.RError(e);
		}
	}

	public void ShutdownTargetConnection(IPEndPoint endpoint, DisconnectReason reason)
	{
		try
		{
			if (!m_Disposed.IsOn && m_SendBufferContexts.TryGetValue(endpoint, out AsyncIOSendContext value))
			{
				value.Dispose();
				m_SendBufferContexts.Remove(endpoint);
			}
		}
		catch (Exception ex)
		{
			Logger.RError("Disconnect Error! message : " + ex.Message);
		}
		finally
		{
			m_OnClose?.Invoke(reason);
		}
	}

	protected void BeginReceive()
	{
		try
		{
			if (!m_Disposed.IsOn)
			{
				m_RecvArgs.SetBuffer(m_ReceiveBufferContext.GetBuffer(), m_ReceiveBufferContext.EndPosition, m_ReceiveBufferContext.RemainWriteSize);
				if (!m_Socket.ReceiveAsync(m_RecvArgs))
				{
					ReceiveCallback(null, m_RecvArgs);
				}
			}
		}
		catch (Exception ex)
		{
			m_OnSocketError(ex);
			Shutdown();
		}
	}

	private void BeginSend()
	{
		try
		{
			if (m_Disposed.IsOn || m_Sending.IsOn || m_SendQueue.IsEmpty)
			{
				return;
			}
			m_Sending.On();
			(bool, AsyncIOSendContext) tuple = FillSendBuffer();
			if (!tuple.Item1)
			{
				m_Sending.Off();
				return;
			}
			AsyncIOSendContext item = tuple.Item2;
			m_SendArgs.SetBuffer(item.GetBuffer(), 0, item.EndPosition);
			if (!m_Socket.SendAsync(m_SendArgs))
			{
				SendCallback(item, m_SendArgs);
			}
		}
		catch (Exception e)
		{
			m_Sending.Off();
			Logger.RError(e);
		}
	}

	public void Shutdown()
	{
		Dispose();
	}

	private (bool, AsyncIOSendContext?) FillSendBuffer()
	{
		int num = 0;
		if (m_SendQueue.TryPeek(out (IPEndPoint, IMsg) result))
		{
			var (iPEndPoint, msg) = result;
			if (!m_SendBufferContexts.TryGetValue(iPEndPoint, out AsyncIOSendContext value))
			{
				value = new AsyncIOSendContext(m_NetworkBufferPool);
				m_SendBufferContexts.Add(iPEndPoint, value);
			}
			byte[] array;
			if (msg.parsingType == ParsingType.Json)
			{
				string s = JsonConvert.SerializeObject(msg);
				array = Encoding.UTF8.GetBytes(s);
			}
			else
			{
				array = Hub.s.msggenerator.Serialize(msg);
			}
			num = array.Length + 8;
			if (value.EndPosition + num > 1048576)
			{
				Logger.RError($"SendBuffer is full. msgType : {msg.msgType} / size : {num} / remainSize : {value.RemainWriteSize}");
				return (false, null);
			}
			m_SendQueue.TryDequeue(out (IPEndPoint, IMsg) _);
			if (!value.Write(BitConverter.GetBytes(num)) || !value.Write(BitConverter.GetBytes((int)msg.msgType)) || !value.Write(array, 0, array.Length))
			{
				throw new Exception($"Failed to write to send buffer. msgType : {msg.msgType} / size : {num} / remainSize : {value.RemainWriteSize}");
			}
			m_OnSend(num, msg.msgType.ToString(), iPEndPoint);
			return (true, value);
		}
		return (false, null);
	}

	public override void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && m_Disposed.On())
		{
			m_SendBufferContexts.Values.ToList().ForEach(delegate(AsyncIOSendContext x)
			{
				x.Dispose();
			});
			m_SendBufferContexts.Clear();
			m_ReceiveBufferContext.Dispose();
			m_RecvArgs.Dispose();
			m_SendArgs.Dispose();
		}
	}

	public override SendResult Send(IMsg[] msgs)
	{
		throw new NotImplementedException();
	}
}
