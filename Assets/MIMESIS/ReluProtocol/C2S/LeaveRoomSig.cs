using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class LeaveRoomSig : IMsg, IMemoryPackable<LeaveRoomSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class LeaveRoomSigFormatter : MemoryPackFormatter<LeaveRoomSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LeaveRoomSig value)
			{
				LeaveRoomSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref LeaveRoomSig value)
			{
				LeaveRoomSig.Deserialize(ref reader, ref value);
			}
		}

		public int actorID;

		public LeaveRoomSig()
			: base(MsgType.C2S_LeaveRoomSig)
		{
			base.reliable = true;
		}

		static LeaveRoomSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<LeaveRoomSig>())
			{
				MemoryPackFormatterProvider.Register(new LeaveRoomSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<LeaveRoomSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<LeaveRoomSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LeaveRoomSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(3, value.msgType, value.hashCode, in value.actorID);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref LeaveRoomSig? value)
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
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.actorID;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					goto IL_00be;
				}
				reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(LeaveRoomSig), 3, memberCount);
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
				if (value != null)
				{
					goto IL_00be;
				}
			}
			value = new LeaveRoomSig
			{
				msgType = value2,
				hashCode = value3,
				actorID = value4
			};
			return;
			IL_00be:
			value.msgType = value2;
			value.hashCode = value3;
			value.actorID = value4;
		}
	}
}
