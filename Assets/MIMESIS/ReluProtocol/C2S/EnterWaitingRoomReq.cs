using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class EnterWaitingRoomReq : IMsg, IMemoryPackable<EnterWaitingRoomReq>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EnterWaitingRoomReqFormatter : MemoryPackFormatter<EnterWaitingRoomReq>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EnterWaitingRoomReq value)
			{
				EnterWaitingRoomReq.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EnterWaitingRoomReq value)
			{
				EnterWaitingRoomReq.Deserialize(ref reader, ref value);
			}
		}

		public long playerUID { get; set; }

		public EnterWaitingRoomReq()
			: base(MsgType.C2S_EnterWaitingRoomReq)
		{
			base.reliable = true;
		}

		static EnterWaitingRoomReq()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EnterWaitingRoomReq>())
			{
				MemoryPackFormatterProvider.Register(new EnterWaitingRoomReqFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EnterWaitingRoomReq[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EnterWaitingRoomReq>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EnterWaitingRoomReq? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int, long>(3, value.msgType, value.hashCode, value.playerUID);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EnterWaitingRoomReq? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			long value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.playerUID;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					goto IL_00bf;
				}
				reader.ReadUnmanaged<MsgType, int, long>(out value2, out value3, out value4);
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EnterWaitingRoomReq), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = 0L;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.playerUID;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<long>(out value4);
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00bf;
				}
			}
			value = new EnterWaitingRoomReq
			{
				msgType = value2,
				hashCode = value3,
				playerUID = value4
			};
			return;
			IL_00bf:
			value.msgType = value2;
			value.hashCode = value3;
			value.playerUID = value4;
		}
	}
}
