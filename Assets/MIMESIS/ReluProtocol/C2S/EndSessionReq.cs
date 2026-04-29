using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class EndSessionReq : IMsg, IMemoryPackable<EndSessionReq>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EndSessionReqFormatter : MemoryPackFormatter<EndSessionReq>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EndSessionReq value)
			{
				EndSessionReq.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EndSessionReq value)
			{
				EndSessionReq.Deserialize(ref reader, ref value);
			}
		}

		public EndSessionReq()
			: base(MsgType.C2S_EndSessionReq)
		{
			base.reliable = true;
		}

		static EndSessionReq()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EndSessionReq>())
			{
				MemoryPackFormatterProvider.Register(new EndSessionReqFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EndSessionReq[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EndSessionReq>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EndSessionReq? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int>(2, value.msgType, value.hashCode);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EndSessionReq? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			if (memberCount == 2)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					goto IL_0096;
				}
				reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			}
			else
			{
				if (memberCount > 2)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EndSessionReq), 2, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						_ = 2;
					}
				}
				if (value != null)
				{
					goto IL_0096;
				}
			}
			value = new EndSessionReq
			{
				msgType = value2,
				hashCode = value3
			};
			return;
			IL_0096:
			value.msgType = value2;
			value.hashCode = value3;
		}
	}
}
