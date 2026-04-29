using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class AbnormalObjectInfo : IMemoryPackable<AbnormalObjectInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class AbnormalObjectInfoFormatter : MemoryPackFormatter<AbnormalObjectInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalObjectInfo value)
			{
				AbnormalObjectInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref AbnormalObjectInfo value)
			{
				AbnormalObjectInfo.Deserialize(ref reader, ref value);
			}
		}

		public AbnormalDataSyncType syncType { get; set; }

		public long abnormalSyncID { get; set; }

		public int abnormalMasterID { get; set; }

		public long remainTime { get; set; }

		public long duration { get; set; }

		static AbnormalObjectInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalObjectInfo>())
			{
				MemoryPackFormatterProvider.Register(new AbnormalObjectInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalObjectInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<AbnormalObjectInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalDataSyncType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<AbnormalDataSyncType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalObjectInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<AbnormalDataSyncType, long, int, long, long>(5, value.syncType, value.abnormalSyncID, value.abnormalMasterID, value.remainTime, value.duration);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref AbnormalObjectInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			AbnormalDataSyncType value2;
			long value3;
			int value4;
			long value5;
			long value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.syncType;
					value3 = value.abnormalSyncID;
					value4 = value.abnormalMasterID;
					value5 = value.remainTime;
					value6 = value.duration;
					reader.ReadUnmanaged<AbnormalDataSyncType>(out value2);
					reader.ReadUnmanaged<long>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanaged<long>(out value6);
					goto IL_011a;
				}
				reader.ReadUnmanaged<AbnormalDataSyncType, long, int, long, long>(out value2, out value3, out value4, out value5, out value6);
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AbnormalObjectInfo), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = AbnormalDataSyncType.Add;
					value3 = 0L;
					value4 = 0;
					value5 = 0L;
					value6 = 0L;
				}
				else
				{
					value2 = value.syncType;
					value3 = value.abnormalSyncID;
					value4 = value.abnormalMasterID;
					value5 = value.remainTime;
					value6 = value.duration;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<AbnormalDataSyncType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<long>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<long>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<long>(out value6);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_011a;
				}
			}
			value = new AbnormalObjectInfo
			{
				syncType = value2,
				abnormalSyncID = value3,
				abnormalMasterID = value4,
				remainTime = value5,
				duration = value6
			};
			return;
			IL_011a:
			value.syncType = value2;
			value.abnormalSyncID = value3;
			value.abnormalMasterID = value4;
			value.remainTime = value5;
			value.duration = value6;
		}
	}
}
