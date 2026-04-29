using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class RoomInfo : IMemoryPackable<RoomInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class RoomInfoFormatter : MemoryPackFormatter<RoomInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RoomInfo value)
			{
				RoomInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref RoomInfo value)
			{
				RoomInfo.Deserialize(ref reader, ref value);
			}
		}

		public VRoomType roomType { get; set; }

		public long roomUID { get; set; }

		public int roomMasterID { get; set; }

		static RoomInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<RoomInfo>())
			{
				MemoryPackFormatterProvider.Register(new RoomInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<RoomInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<RoomInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<VRoomType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<VRoomType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RoomInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<VRoomType, long, int>(3, value.roomType, value.roomUID, value.roomMasterID);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref RoomInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			VRoomType value2;
			long value3;
			int value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.roomType;
					value3 = value.roomUID;
					value4 = value.roomMasterID;
					reader.ReadUnmanaged<VRoomType>(out value2);
					reader.ReadUnmanaged<long>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					goto IL_00bf;
				}
				reader.ReadUnmanaged<VRoomType, long, int>(out value2, out value3, out value4);
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RoomInfo), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = VRoomType.Invalid;
					value3 = 0L;
					value4 = 0;
				}
				else
				{
					value2 = value.roomType;
					value3 = value.roomUID;
					value4 = value.roomMasterID;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<VRoomType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<long>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00bf;
				}
			}
			value = new RoomInfo
			{
				roomType = value2,
				roomUID = value3,
				roomMasterID = value4
			};
			return;
			IL_00bf:
			value.roomType = value2;
			value.roomUID = value3;
			value.roomMasterID = value4;
		}
	}
}
