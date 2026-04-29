using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class StartSessionReq : IMsg, IMemoryPackable<StartSessionReq>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class StartSessionReqFormatter : MemoryPackFormatter<StartSessionReq>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartSessionReq value)
			{
				StartSessionReq.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref StartSessionReq value)
			{
				StartSessionReq.Deserialize(ref reader, ref value);
			}
		}

		public StartSessionReq()
			: base(MsgType.C2S_StartSessionReq)
		{
			base.reliable = true;
		}

		static StartSessionReq()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<StartSessionReq>())
			{
				MemoryPackFormatterProvider.Register(new StartSessionReqFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<StartSessionReq[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<StartSessionReq>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartSessionReq? value) where TBufferWriter : class, IBufferWriter<byte>
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
		public static void Deserialize(ref MemoryPackReader reader, ref StartSessionReq? value)
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
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(StartSessionReq), 2, memberCount);
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
			value = new StartSessionReq
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
