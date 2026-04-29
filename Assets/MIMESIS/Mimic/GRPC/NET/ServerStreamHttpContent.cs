using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mimic.GRPC.NET
{
	public class ServerStreamHttpContent : HttpContent
	{
		private readonly Stream _stream;

		public ServerStreamHttpContent(Stream memStream)
		{
			_stream = memStream;
		}

		protected override Task<Stream> CreateContentReadStreamAsync()
		{
			return Task.FromResult(_stream);
		}

		protected override void Dispose(bool disposing)
		{
		}

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			throw new NotSupportedException();
		}

		protected override bool TryComputeLength(out long length)
		{
			throw new NotSupportedException();
		}
	}
}
