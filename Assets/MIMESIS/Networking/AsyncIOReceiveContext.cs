using System;
using System.Runtime.InteropServices;

public class AsyncIOReceiveContext : IAsyncIOContext, IDisposable
{
	private readonly ArraySegment<byte> m_Buffer;

	private readonly INetworkBufferPool m_NetworkBufferPool;

	public readonly int Size;

	public int CurrentPosition { get; private set; }

	public int EndPosition { get; private set; }

	public int RemainReadSize => EndPosition - CurrentPosition;

	public int RemainWriteSize => Size - EndPosition;

	public AsyncIOReceiveContext(INetworkBufferPool networkBufferPool)
	{
		m_NetworkBufferPool = networkBufferPool;
		m_Buffer = networkBufferPool.RentBuffer(BufferType.Recv);
		Size = m_Buffer.Count;
	}

	~AsyncIOReceiveContext()
	{
		Dispose(disposing: false);
	}

	public byte[] GetBuffer()
	{
		return m_Buffer.Array;
	}

	public ArraySegment<byte> GetSegment()
	{
		return m_Buffer;
	}

	public long Seek(int offset)
	{
		CurrentPosition += offset;
		return CurrentPosition;
	}

	public bool AddEndPosition(int position)
	{
		if (position + EndPosition > Size)
		{
			return false;
		}
		EndPosition += position;
		return true;
	}

	public (bool, ArraySegment<byte>) Read(int count)
	{
		if (count > RemainReadSize)
		{
			return (false, default(ArraySegment<byte>));
		}
		return (true, m_Buffer.Slice(CurrentPosition, count));
	}

	public void RemoveFrontBuffer()
	{
		if (RemainReadSize > 0)
		{
			Array.Copy(m_Buffer.Array, CurrentPosition, m_Buffer.Array, 0, RemainReadSize);
			EndPosition = RemainReadSize;
			CurrentPosition = 0;
		}
		else
		{
			CurrentPosition = 0;
			EndPosition = 0;
		}
	}

	public override void Reset()
	{
		CurrentPosition = 0;
		EndPosition = 0;
	}

	protected override void Dispose(bool disposing)
	{
		if (!m_Disposed && disposing)
		{
			m_NetworkBufferPool.ReturnBuffer(BufferType.Recv, in m_Buffer);
		}
	}

	public void WriteBuffer(IntPtr ptr, int size)
	{
		if (RemainWriteSize < size)
		{
			throw new Exception("WriteBuffer: RemainWriteSize < size");
		}
		Marshal.Copy(ptr, m_Buffer.Array, EndPosition, size);
		EndPosition += size;
	}
}
