using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Grpc.Core;

namespace Relugames.Replay
{
	public static class ReplayTransfer
	{
		[GeneratedCode("grpc_csharp_plugin", null)]
		private static class __Helper_MessageCache<T>
		{
			public static readonly bool IsBufferMessage = typeof(IBufferMessage).GetTypeInfo().IsAssignableFrom(typeof(T));
		}

		[BindServiceMethod(typeof(ReplayTransfer), "BindService")]
		public abstract class ReplayTransferBase
		{
			[GeneratedCode("grpc_csharp_plugin", null)]
			public virtual Task<UploadReplayDataResponse> UploadReplayData(IAsyncStreamReader<UploadReplayDataRequest> requestStream, ServerCallContext context)
			{
				throw new RpcException(new Status(StatusCode.Unimplemented, ""));
			}
		}

		public class ReplayTransferClient : ClientBase<ReplayTransferClient>
		{
			[GeneratedCode("grpc_csharp_plugin", null)]
			public ReplayTransferClient(ChannelBase channel)
				: base(channel)
			{
			}

			[GeneratedCode("grpc_csharp_plugin", null)]
			public ReplayTransferClient(CallInvoker callInvoker)
				: base(callInvoker)
			{
			}

			[GeneratedCode("grpc_csharp_plugin", null)]
			protected ReplayTransferClient()
			{
			}

			[GeneratedCode("grpc_csharp_plugin", null)]
			protected ReplayTransferClient(ClientBaseConfiguration configuration)
				: base(configuration)
			{
			}

			[GeneratedCode("grpc_csharp_plugin", null)]
			public virtual AsyncClientStreamingCall<UploadReplayDataRequest, UploadReplayDataResponse> UploadReplayData(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
			{
				return UploadReplayData(new CallOptions(headers, deadline, cancellationToken));
			}

			[GeneratedCode("grpc_csharp_plugin", null)]
			public virtual AsyncClientStreamingCall<UploadReplayDataRequest, UploadReplayDataResponse> UploadReplayData(CallOptions options)
			{
				return base.CallInvoker.AsyncClientStreamingCall(__Method_UploadReplayData, null, options);
			}

			[GeneratedCode("grpc_csharp_plugin", null)]
			protected override ReplayTransferClient NewInstance(ClientBaseConfiguration configuration)
			{
				return new ReplayTransferClient(configuration);
			}
		}

		private static readonly string __ServiceName = "ReplayTransfer";

		[GeneratedCode("grpc_csharp_plugin", null)]
		private static readonly Marshaller<UploadReplayDataRequest> __Marshaller_UploadReplayDataRequest = Marshallers.Create(__Helper_SerializeMessage, (DeserializationContext context) => __Helper_DeserializeMessage(context, UploadReplayDataRequest.Parser));

		[GeneratedCode("grpc_csharp_plugin", null)]
		private static readonly Marshaller<UploadReplayDataResponse> __Marshaller_UploadReplayDataResponse = Marshallers.Create(__Helper_SerializeMessage, (DeserializationContext context) => __Helper_DeserializeMessage(context, UploadReplayDataResponse.Parser));

		[GeneratedCode("grpc_csharp_plugin", null)]
		private static readonly Method<UploadReplayDataRequest, UploadReplayDataResponse> __Method_UploadReplayData = new Method<UploadReplayDataRequest, UploadReplayDataResponse>(MethodType.ClientStreaming, __ServiceName, "UploadReplayData", __Marshaller_UploadReplayDataRequest, __Marshaller_UploadReplayDataResponse);

		public static ServiceDescriptor Descriptor => ReplayTransferReflection.Descriptor.Services[0];

		[GeneratedCode("grpc_csharp_plugin", null)]
		private static void __Helper_SerializeMessage(IMessage message, SerializationContext context)
		{
			if (message is IBufferMessage)
			{
				context.SetPayloadLength(message.CalculateSize());
				message.WriteTo(context.GetBufferWriter());
				context.Complete();
			}
			else
			{
				context.Complete(message.ToByteArray());
			}
		}

		[GeneratedCode("grpc_csharp_plugin", null)]
		private static T __Helper_DeserializeMessage<T>(DeserializationContext context, MessageParser<T> parser) where T : IMessage<T>
		{
			if (__Helper_MessageCache<T>.IsBufferMessage)
			{
				return parser.ParseFrom(context.PayloadAsReadOnlySequence());
			}
			return parser.ParseFrom(context.PayloadAsNewBuffer());
		}

		[GeneratedCode("grpc_csharp_plugin", null)]
		public static ServerServiceDefinition BindService(ReplayTransferBase serviceImpl)
		{
			return ServerServiceDefinition.CreateBuilder().AddMethod(__Method_UploadReplayData, serviceImpl.UploadReplayData).Build();
		}

		[GeneratedCode("grpc_csharp_plugin", null)]
		public static void BindService(ServiceBinderBase serviceBinder, ReplayTransferBase serviceImpl)
		{
			serviceBinder.AddMethod(__Method_UploadReplayData, (serviceImpl == null) ? null : new ClientStreamingServerMethod<UploadReplayDataRequest, UploadReplayDataResponse>(serviceImpl.UploadReplayData));
		}
	}
}
