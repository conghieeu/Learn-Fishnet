using System;

public sealed class NetworkBufferAllocator : INetworkBufferPool
{
	private readonly int m_RecvBufferSize;

	private readonly int m_SendBufferSize;

	public NetworkBufferAllocator(int recvBufferSize, int sendBufferSize)
	{
		m_RecvBufferSize = recvBufferSize;
		m_SendBufferSize = sendBufferSize;
	}

	public ArraySegment<byte> RentBuffer(BufferType bufferType)
	{
		return bufferType switch
		{
			BufferType.Recv => new ArraySegment<byte>(new byte[m_RecvBufferSize]), 
			BufferType.Send => new ArraySegment<byte>(new byte[m_SendBufferSize]), 
			_ => throw new NotSupportedException($"RentBuffer failed, not supported bufferType: {bufferType}"), 
		};
	}

	public void ReturnBuffer(BufferType bufferType, in ArraySegment<byte> memory)
	{
	}

	void INetworkBufferPool.ReturnBuffer(BufferType bufferType, in ArraySegment<byte> memory)
	{
		ReturnBuffer(bufferType, in memory);
	}
}
