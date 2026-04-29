using System;

public interface INetworkBufferPool
{
	ArraySegment<byte> RentBuffer(BufferType bufferType);

	void ReturnBuffer(BufferType bufferType, in ArraySegment<byte> memory);
}
