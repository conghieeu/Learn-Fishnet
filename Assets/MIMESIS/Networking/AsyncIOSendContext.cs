using System;
using System.Runtime.InteropServices;

public class AsyncIOSendContext : IAsyncIOContext
{
	private readonly ArraySegment<byte> m_Buffer;

	private readonly INetworkBufferPool m_NetworkBufferPool;

	public readonly int Size;

	private int m_CurrentPosition;

	public int EndPosition { get; private set; }

	public int RemainSendSize => EndPosition - m_CurrentPosition;

	public int RemainWriteSize => Size - EndPosition;

	public AsyncIOSendContext(INetworkBufferPool networkBufferPool)
	{
		m_CurrentPosition = 0;
		m_NetworkBufferPool = networkBufferPool;
		m_Buffer = networkBufferPool.RentBuffer(BufferType.Send);
		Size = m_Buffer.Count;
	}

	public byte[] GetBuffer()
	{
		return m_Buffer.Array;
	}

	public override void Reset()
	{
		m_CurrentPosition = 0;
		EndPosition = 0;
	}

	public void AddCurrentPosition(int position)
	{
		m_CurrentPosition += position;
	}

	public bool Write(byte[] data)
	{
		if (data.Length > RemainWriteSize)
		{
			return false;
		}
		Array.Copy(data, 0, m_Buffer.Array, EndPosition, data.Length);
		EndPosition += data.Length;
		return true;
	}

	public IntPtr GetAllBuffers()
	{
		IntPtr intPtr = Marshal.AllocHGlobal(RemainSendSize);
		Marshal.Copy(m_Buffer.Array, m_CurrentPosition, intPtr, RemainSendSize);
		return intPtr;
	}

	public bool Write(byte[] data, int offset, int count)
	{
		if (count > RemainWriteSize)
		{
			return false;
		}
		Array.Copy(data, offset, m_Buffer.Array, EndPosition, count);
		EndPosition += count;
		return true;
	}

	protected override void Dispose(bool disposing)
	{
		if (!m_Disposed && disposing)
		{
			m_NetworkBufferPool.ReturnBuffer(BufferType.Send, in m_Buffer);
		}
	}
}
