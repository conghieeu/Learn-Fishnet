using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class IActorMsg : IMsg, IMemoryPackable<IActorMsg>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class IActorMsgFormatter : MemoryPackFormatter<IActorMsg>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref IActorMsg value)
			{
				IActorMsg.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref IActorMsg value)
			{
				IActorMsg.Deserialize(ref reader, ref value);
			}
		}

		public int actorID { get; set; }

		[MemoryPackConstructor]
		public IActorMsg(MsgType msgType)
			: base(msgType)
		{
		}

		static IActorMsg()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<IActorMsg>())
			{
				MemoryPackFormatterProvider.Register(new IActorMsgFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<IActorMsg[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<IActorMsg>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref IActorMsg? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(3, value.msgType, value.hashCode, value.actorID);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref IActorMsg? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			int value4;
			if (memberCount == 3)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.actorID;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
				}
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(IActorMsg), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = 0;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.actorID;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							_ = 3;
						}
					}
				}
				_ = value;
			}
			value = new IActorMsg(value2)
			{
				hashCode = value3,
				actorID = value4
			};
		}
	}
}
