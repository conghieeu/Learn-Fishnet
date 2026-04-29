using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class CompleteMakingRoomSig : IMsg, IMemoryPackable<CompleteMakingRoomSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CompleteMakingRoomSigFormatter : MemoryPackFormatter<CompleteMakingRoomSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CompleteMakingRoomSig value)
			{
				CompleteMakingRoomSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CompleteMakingRoomSig value)
			{
				CompleteMakingRoomSig.Deserialize(ref reader, ref value);
			}
		}

		public RoomInfo nextRoomInfo { get; set; } = new RoomInfo();

		public CompleteMakingRoomSig()
			: base(MsgType.C2S_CompleteMakingRoomSig)
		{
			base.reliable = true;
		}

		static CompleteMakingRoomSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CompleteMakingRoomSig>())
			{
				MemoryPackFormatterProvider.Register(new CompleteMakingRoomSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CompleteMakingRoomSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CompleteMakingRoomSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CompleteMakingRoomSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
			writer.WritePackable<RoomInfo>(value.nextRoomInfo);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CompleteMakingRoomSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			RoomInfo value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.nextRoomInfo;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadPackable(ref value4);
					goto IL_00c3;
				}
				reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
				value4 = reader.ReadPackable<RoomInfo>();
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CompleteMakingRoomSig), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.nextRoomInfo;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadPackable(ref value4);
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00c3;
				}
			}
			value = new CompleteMakingRoomSig
			{
				msgType = value2,
				hashCode = value3,
				nextRoomInfo = value4
			};
			return;
			IL_00c3:
			value.msgType = value2;
			value.hashCode = value3;
			value.nextRoomInfo = value4;
		}
	}
}
