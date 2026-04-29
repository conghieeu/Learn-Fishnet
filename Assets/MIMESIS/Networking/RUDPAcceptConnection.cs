using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Bifrost;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class RUDPAcceptConnection
{
	protected readonly ConcurrentQueue<IMsg> m_ReliableSendQueue = new ConcurrentQueue<IMsg>();

	protected readonly ConcurrentQueue<IMsg> m_UnreliableSendQueue = new ConcurrentQueue<IMsg>();

	protected NetPeer? _netPeer;

	protected NetDataWriter? _netWriter;

	protected OnSendPacket _onSend;

	protected OnRecvPacket _onRecv;

	protected OnRUDPClose _onClose;

	protected OnRUDPError _onError;

	protected AtomicFlag _closePending = new AtomicFlag(value: false);

	protected AtomicFlag m_Disposed = new AtomicFlag(value: false);

	public bool PendingClose => m_Disposed.IsOn;

	public RUDPAcceptConnection(OnSendPacket onSend, OnRecvPacket onRecv, OnRUDPClose onClose, OnRUDPError onError)
	{
		_onSend = onSend;
		_onRecv = onRecv;
		_onClose = onClose;
		_onError = onError;
		_netWriter = new NetDataWriter(autoResize: false, 4194304);
	}

	public void SetNetPeer(NetPeer peer)
	{
		_netPeer = peer;
	}

	public bool IsRunning()
	{
		if (!m_Disposed.IsOn)
		{
			return !_closePending.IsOn;
		}
		return false;
	}

	public void Close(ReluProtocol.Enum.DisconnectReason reason, bool immediately)
	{
		if (!m_Disposed.IsOn)
		{
			if (immediately || (m_UnreliableSendQueue.IsEmpty && m_ReliableSendQueue.IsEmpty))
			{
				CloseInternal(reason);
			}
			else
			{
				_closePending.On();
			}
		}
	}

	public void ReceiveCallback(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
	{
		try
		{
			if (_netPeer != peer)
			{
				Logger.RError("Invalid Peer - " + peer.ToString());
				CloseInternal(ReluProtocol.Enum.DisconnectReason.ConnectionError);
				return;
			}
			byte[] remainingBytes = reader.GetRemainingBytes();
			ArraySegment<byte> data = new ArraySegment<byte>(remainingBytes);
			int count = data.Count;
			if (count > 0)
			{
				if (!OnReceive(count, data, out var disconnectReason))
				{
					CloseInternal(disconnectReason);
				}
			}
			else
			{
				Logger.RError($"Invalid Message Size - {count}");
				CloseInternal(ReluProtocol.Enum.DisconnectReason.ConnectionError);
			}
		}
		catch (Exception ex)
		{
			_onError(ex);
			CloseInternal(ReluProtocol.Enum.DisconnectReason.Undefined);
		}
	}

	public bool OnReceive(int pktSize, ArraySegment<byte> data, out ReluProtocol.Enum.DisconnectReason disconnectReason)
	{
		disconnectReason = ReluProtocol.Enum.DisconnectReason.None;
		if (4 > pktSize)
		{
			Logger.RError($"(Context) Invalid Message Size - {pktSize}");
			disconnectReason = ReluProtocol.Enum.DisconnectReason.ConnectionError;
			return false;
		}
		int intValue;
		for (int i = 0; i < pktSize; i += intValue)
		{
			int num = pktSize - i;
			if (num < 4)
			{
				Logger.RError($"(Context) Insufficient data for packet length - remaining: {num}");
				disconnectReason = ReluProtocol.Enum.DisconnectReason.ConnectionError;
				return false;
			}
			intValue = 0;
			Serializer.Load(data, i, ref intValue);
			if (intValue < 4)
			{
				Logger.RError($"(Context) Invalid packet size - {intValue}");
				disconnectReason = ReluProtocol.Enum.DisconnectReason.ConnectionError;
				return false;
			}
			if (num < intValue)
			{
				Logger.RError($"(Context) Incomplete packet - expected: {intValue}, remaining: {num}");
				disconnectReason = ReluProtocol.Enum.DisconnectReason.ConnectionError;
				return false;
			}
			int intValue2 = 0;
			Serializer.Load(data, i + 4, ref intValue2);
			int count = intValue - 8;
			ArraySegment<byte> data2 = new ArraySegment<byte>(data.Array, data.Offset + i + 4 + 4, count);
			try
			{
				_onRecv(intValue, intValue2, in data2);
			}
			catch (Exception ex)
			{
				Logger.RError($"Error processing packet type {intValue2}: {ex.Message}");
				disconnectReason = ReluProtocol.Enum.DisconnectReason.Undefined;
				return false;
			}
		}
		return true;
	}

	public SendResult Send(IMsg[] msgs)
	{
		if (msgs.Length == 0)
		{
			return SendResult.InvalidSize;
		}
		if (!IsRunning())
		{
			CloseInternal(ReluProtocol.Enum.DisconnectReason.None);
			return SendResult.NotConnect;
		}
		for (int i = 0; i < msgs.Length; i++)
		{
			if (msgs[i].reliable)
			{
				m_ReliableSendQueue.Enqueue(msgs[i]);
			}
			else
			{
				m_UnreliableSendQueue.Enqueue(msgs[i]);
			}
		}
		return SendResult.Success;
	}

	public void Dispose()
	{
		Dispose(isDisposing: true);
		GC.SuppressFinalize(this);
	}

	protected void CloseInternal(ReluProtocol.Enum.DisconnectReason reason)
	{
		try
		{
			if (m_Disposed.On())
			{
				_netPeer?.Disconnect();
			}
		}
		catch (Exception ex)
		{
			Logger.RError("Disconnect Error! message : " + ex.Message);
		}
		finally
		{
			_onClose?.Invoke(reason);
		}
	}

	protected virtual void Dispose(bool isDisposing)
	{
		if (isDisposing && m_Disposed.On())
		{
			m_ReliableSendQueue.Clear();
			m_UnreliableSendQueue.Clear();
			_netPeer?.Disconnect();
			_netWriter = null;
		}
	}

	protected void Flush()
	{
		try
		{
			if (m_Disposed.IsOn || (m_UnreliableSendQueue.IsEmpty && m_ReliableSendQueue.IsEmpty) || _netPeer == null || _netWriter == null)
			{
				return;
			}
			while (!m_UnreliableSendQueue.IsEmpty && !m_Disposed.IsOn)
			{
				FillSendBuffer(reliable: false);
				if (_netWriter.Length == 0)
				{
					break;
				}
				_netPeer.Send(_netWriter, (_netWriter.Length > 1200) ? DeliveryMethod.ReliableOrdered : DeliveryMethod.ReliableSequenced);
			}
			while (!m_ReliableSendQueue.IsEmpty && !m_Disposed.IsOn)
			{
				FillSendBuffer(reliable: true);
				if (_netWriter.Length != 0)
				{
					_netPeer.Send(_netWriter, DeliveryMethod.ReliableOrdered);
					continue;
				}
				break;
			}
		}
		catch (Exception e)
		{
			Logger.RError(e);
			CloseInternal(ReluProtocol.Enum.DisconnectReason.Undefined);
		}
	}

	protected void FillSendBuffer(bool reliable)
	{
		if (_netWriter == null)
		{
			Logger.RError("NetWriter is null");
			return;
		}
		if (_netPeer == null)
		{
			Logger.RError("NetPeer is null");
			return;
		}
		int maxSinglePacketSize = _netPeer.GetMaxSinglePacketSize(reliable ? DeliveryMethod.ReliableOrdered : DeliveryMethod.ReliableSequenced);
		int num = 0;
		_netWriter.Reset();
		ConcurrentQueue<IMsg> concurrentQueue = (reliable ? m_ReliableSendQueue : m_UnreliableSendQueue);
		IMsg result;
		while (concurrentQueue.TryPeek(out result))
		{
			byte[] array = SerializeMessage(result);
			int num2 = array.Length + 8;
			IMsg result2;
			if (num == 0)
			{
				concurrentQueue.TryDequeue(out result2);
				byte[] data = BuildPacket(num2, (int)result.msgType, array);
				_netWriter.Put(data);
				num += num2;
				if (num2 >= maxSinglePacketSize)
				{
					break;
				}
			}
			else
			{
				if (num + num2 > maxSinglePacketSize)
				{
					break;
				}
				concurrentQueue.TryDequeue(out result2);
				byte[] data2 = BuildPacket(num2, (int)result.msgType, array);
				_netWriter.Put(data2);
				num += num2;
			}
		}
	}

	private byte[] SerializeMessage(IMsg msg)
	{
		if (msg.parsingType == ParsingType.Json)
		{
			string s = JsonConvert.SerializeObject(msg);
			return Encoding.UTF8.GetBytes(s);
		}
		return Hub.s.msggenerator.Serialize(msg);
	}

	private byte[] BuildPacket(int msgSize, int msgType, byte[] buffer)
	{
		byte[] array = new byte[msgSize];
		byte[] bytes = BitConverter.GetBytes(msgSize);
		byte[] bytes2 = BitConverter.GetBytes(msgType);
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		Buffer.BlockCopy(bytes2, 0, array, bytes.Length, bytes2.Length);
		Buffer.BlockCopy(buffer, 0, array, bytes.Length + bytes2.Length, buffer.Length);
		return array;
	}

	public IPEndPoint? GetRemoteEndPoint()
	{
		return _netPeer ?? throw new Exception("NetPeer is null");
	}

	public virtual void Update()
	{
		if (!m_Disposed.IsOn)
		{
			if (_closePending.IsOn && m_UnreliableSendQueue.IsEmpty && m_ReliableSendQueue.IsEmpty)
			{
				CloseInternal(ReluProtocol.Enum.DisconnectReason.None);
			}
			else
			{
				Flush();
			}
		}
	}
}
