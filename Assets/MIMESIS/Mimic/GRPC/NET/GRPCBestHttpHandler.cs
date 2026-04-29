using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Best.HTTP;
using Best.HTTP.Request.Settings;
using Best.HTTP.Response;
using Best.HTTP.Shared.PlatformSupport.Memory;

namespace Mimic.GRPC.NET
{
	public class GRPCBestHttpHandler : HttpClientHandler
	{
		private static readonly string ContentType = "Content-Type";

		private const int ASYNC_READ_WAIT_MS = 5;

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage grpcRequest, CancellationToken cancellationToken)
		{
			if (grpcRequest.Method != HttpMethod.Post)
			{
				throw new NotSupportedException("gRPC only supports POST method.");
			}
			HTTPRequest request = new HTTPRequest(grpcRequest.RequestUri, HTTPMethods.Post);
			request.RetrySettings.MaxRetries = 0;
			foreach (KeyValuePair<string, IEnumerable<string>> header in grpcRequest.Headers)
			{
				foreach (string item in header.Value)
				{
					request.AddHeader(header.Key, item);
				}
			}
			request.AddHeader(ContentType, "application/grpc");
			PushPullStream outgoingDataStream = new PushPullStream
			{
				NonBlockingRead = true
			};
			request.UploadSettings.UploadStream = outgoingDataStream;
			grpcRequest.Content.CopyToAsync(outgoingDataStream);
			request.UploadSettings.OnHeadersSent += delegate
			{
				grpcRequest.Content.ReadAsStreamAsync().ContinueWith(delegate
				{
					outgoingDataStream.Close();
				}, cancellationToken);
			};
			TaskCompletionSource<HttpResponseMessage> grpcResponseTask = new TaskCompletionSource<HttpResponseMessage>();
			PushPullStream incomingDataStream = new PushPullStream();
			HttpResponseMessage grpcResponseMessage = new HttpResponseMessage
			{
				RequestMessage = grpcRequest,
				Content = new ServerStreamHttpContent(incomingDataStream)
			};
			bool isHeader = true;
			DownloadSettings downloadSettings = request.DownloadSettings;
			downloadSettings.OnHeadersReceived = (OnHeadersReceivedDelegate)Delegate.Combine(downloadSettings.OnHeadersReceived, (OnHeadersReceivedDelegate)delegate(HTTPRequest req, HTTPResponse res, Dictionary<string, List<string>> headers)
			{
				bool flag = isHeader && headers.Keys.Contains("grpc-status");
				foreach (KeyValuePair<string, List<string>> header2 in headers)
				{
					grpcResponseMessage.Content.Headers.TryAddWithoutValidation(header2.Key, header2.Value);
					if (isHeader || flag)
					{
						grpcResponseMessage.Headers.TryAddWithoutValidation(header2.Key, header2.Value);
					}
					else
					{
						grpcResponseMessage.TrailingHeaders.TryAddWithoutValidation(header2.Key, header2.Value);
					}
				}
				if (isHeader)
				{
					grpcResponseMessage.ReasonPhrase = res.Message;
					grpcResponseMessage.StatusCode = (HttpStatusCode)res.StatusCode;
					grpcResponseMessage.Version = new Version(res.HTTPVersion.Major, res.HTTPVersion.Minor);
				}
				if (flag)
				{
					incomingDataStream.Close();
				}
				if (!grpcResponseTask.Task.IsCompleted)
				{
					grpcResponseTask.SetResult(grpcResponseMessage);
				}
				isHeader = false;
			});
			DownloadSettings downloadSettings2 = request.DownloadSettings;
			downloadSettings2.OnDownloadStarted = (OnDownloadStartedDelegate)Delegate.Combine(downloadSettings2.OnDownloadStarted, (OnDownloadStartedDelegate)delegate(HTTPRequest _, HTTPResponse _, DownloadContentStream stream)
			{
				try
				{
					try
					{
						BufferSegment segment;
						while (stream.TryTake(out segment))
						{
							AutoReleaseBuffer autoReleaseBuffer = segment.AsAutoRelease();
							try
							{
								incomingDataStream.Write(autoReleaseBuffer.Data, autoReleaseBuffer.Offset, autoReleaseBuffer.Count);
							}
							finally
							{
								((IDisposable)autoReleaseBuffer/*cast due to .constrained prefix*/).Dispose();
							}
						}
						if (stream.IsCompleted)
						{
							incomingDataStream.Close();
							stream.Dispose();
							return;
						}
					}
					catch (Exception exception)
					{
						incomingDataStream.CloseWithException(exception);
						stream.Dispose();
						return;
					}
					stream.IsDetached = true;
					Task.Run(async delegate
					{
						try
						{
							while (!stream.IsCompleted)
							{
								if (stream.TryTake(out var segment2))
								{
									AutoReleaseBuffer autoReleaseBuffer2 = segment2.AsAutoRelease();
									try
									{
										incomingDataStream.Write(autoReleaseBuffer2.Data, autoReleaseBuffer2.Offset, autoReleaseBuffer2.Count);
									}
									finally
									{
										((IDisposable)autoReleaseBuffer2/*cast due to .constrained prefix*/).Dispose();
									}
								}
								else
								{
									await Task.Delay(5, cancellationToken);
								}
							}
						}
						catch (Exception exception3)
						{
							incomingDataStream.CloseWithException(exception3);
						}
						finally
						{
							stream.Dispose();
							incomingDataStream.Close();
						}
					}, cancellationToken);
				}
				catch (Exception exception2)
				{
					incomingDataStream.CloseWithException(exception2);
				}
			});
			CancellationTokenRegistration cancellationTokenRegistration = cancellationToken.Register(delegate
			{
				request.Abort();
				incomingDataStream.Close();
			});
			HTTPRequest hTTPRequest = request;
			hTTPRequest.Callback = (OnRequestFinishedDelegate)Delegate.Combine(hTTPRequest.Callback, (OnRequestFinishedDelegate)delegate(HTTPRequest req, HTTPResponse res)
			{
				if (req.State != HTTPRequestStates.Finished)
				{
					Exception exception = req.Exception ?? new Exception($"Unknown error while processing grpc req/res (state={req.State}).");
					if (req.State == HTTPRequestStates.Aborted)
					{
						exception = new Exception("gRPC call aborted by client.");
					}
					if (!grpcResponseTask.Task.IsCompleted)
					{
						grpcResponseTask.SetException(exception);
					}
					else
					{
						incomingDataStream.CloseWithException(exception);
					}
				}
				cancellationTokenRegistration.Dispose();
			});
			request.Send();
			return grpcResponseTask.Task;
		}
	}
}
