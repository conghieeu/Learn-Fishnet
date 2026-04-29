using System;

public abstract class IAsyncIOContext : IDisposable
{
	protected bool m_Disposed;

	public IAsyncIOContext()
	{
		m_Disposed = false;
	}

	public void Dispose()
	{
		if (!m_Disposed)
		{
			Dispose(isDisposing: true);
		}
	}

	public abstract void Reset();

	protected abstract void Dispose(bool isDisposing);
}
