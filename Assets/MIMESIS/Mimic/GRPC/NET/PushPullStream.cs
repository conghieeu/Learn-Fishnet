using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Best.HTTP;
using Best.HTTP.Request.Upload;

namespace Mimic.GRPC.NET
{
	public class PushPullStream : UploadStreamBase
	{
		private const long MAX_BUFFFER_SIZE = 5242880L;

		public bool NonBlockingRead;

		private readonly Queue<byte> _buffer = new Queue<byte>();

		private readonly string _name;

		private bool _flushed;

		private bool _closed;

		private Exception _exception;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => true;

		public override long Length => -2L;

		public override long Position
		{
			get
			{
				return 0L;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (count == 0)
			{
				return 0;
			}
			int i = 0;
			lock (_buffer)
			{
				while (!ReadAvailable())
				{
					Monitor.Wait(_buffer);
				}
				for (; i < count; i++)
				{
					if (_buffer.Count <= 0)
					{
						break;
					}
					buffer[offset + i] = _buffer.Dequeue();
				}
				Monitor.Pulse(_buffer);
			}
			if (i == 0 && !_closed)
			{
				return -1;
			}
			return i;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			lock (_buffer)
			{
				while ((long)_buffer.Count >= 5242880L)
				{
					Monitor.Wait(_buffer);
				}
				for (int i = offset; i < offset + count; i++)
				{
					_buffer.Enqueue(buffer[i]);
				}
				_flushed = false;
				Monitor.Pulse(_buffer);
			}
		}

		public override void Flush()
		{
			lock (_buffer)
			{
				_flushed = true;
				Monitor.Pulse(_buffer);
			}
			base.Signaler?.SignalThread();
		}

		public override void Close()
		{
			CloseWithException(null);
		}

		public override void BeforeSendHeaders(HTTPRequest request)
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public void CloseWithException(Exception exception)
		{
			_closed = true;
			_exception = exception;
			Flush();
		}

		private bool ReadAvailable()
		{
			if (_exception != null)
			{
				throw _exception;
			}
			if (_buffer.Count < 0 && !_flushed && !_closed)
			{
				return NonBlockingRead;
			}
			return true;
		}
	}
}
