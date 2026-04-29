using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class AbnormalStatsInfo : IMemoryPackable<AbnormalStatsInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class AbnormalStatsInfoFormatter : MemoryPackFormatter<AbnormalStatsInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalStatsInfo value)
			{
				AbnormalStatsInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref AbnormalStatsInfo value)
			{
				AbnormalStatsInfo.Deserialize(ref reader, ref value);
			}
		}

		public AbnormalDataSyncType changeType { get; set; }

		public int abnormalMasterID { get; set; }

		public long abnormalSyncID { get; set; }

		public long syncID { get; set; }

		public int index { get; set; }

		public long remainTime { get; set; }

		public long duration { get; set; }

		static AbnormalStatsInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalStatsInfo>())
			{
				MemoryPackFormatterProvider.Register(new AbnormalStatsInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalStatsInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<AbnormalStatsInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalDataSyncType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<AbnormalDataSyncType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalStatsInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<AbnormalDataSyncType, int, long, long, int, long, long>(7, value.changeType, value.abnormalMasterID, value.abnormalSyncID, value.syncID, value.index, value.remainTime, value.duration);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref AbnormalStatsInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			AbnormalDataSyncType value2;
			int value3;
			long value4;
			long value5;
			int value6;
			long value7;
			long value8;
			if (memberCount == 7)
			{
				if (value != null)
				{
					value2 = value.changeType;
					value3 = value.abnormalMasterID;
					value4 = value.abnormalSyncID;
					value5 = value.syncID;
					value6 = value.index;
					value7 = value.remainTime;
					value8 = value.duration;
					reader.ReadUnmanaged<AbnormalDataSyncType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanaged<int>(out value6);
					reader.ReadUnmanaged<long>(out value7);
					reader.ReadUnmanaged<long>(out value8);
					goto IL_0174;
				}
				reader.ReadUnmanaged<AbnormalDataSyncType, int, long, long, int, long, long>(out value2, out value3, out value4, out value5, out value6, out value7, out value8);
			}
			else
			{
				if (memberCount > 7)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AbnormalStatsInfo), 7, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = AbnormalDataSyncType.Add;
					value3 = 0;
					value4 = 0L;
					value5 = 0L;
					value6 = 0;
					value7 = 0L;
					value8 = 0L;
				}
				else
				{
					value2 = value.changeType;
					value3 = value.abnormalMasterID;
					value4 = value.abnormalSyncID;
					value5 = value.syncID;
					value6 = value.index;
					value7 = value.remainTime;
					value8 = value.duration;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<AbnormalDataSyncType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<long>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<long>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<int>(out value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<long>(out value7);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<long>(out value8);
											_ = 7;
										}
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0174;
				}
			}
			value = new AbnormalStatsInfo
			{
				changeType = value2,
				abnormalMasterID = value3,
				abnormalSyncID = value4,
				syncID = value5,
				index = value6,
				remainTime = value7,
				duration = value8
			};
			return;
			IL_0174:
			value.changeType = value2;
			value.abnormalMasterID = value3;
			value.abnormalSyncID = value4;
			value.syncID = value5;
			value.index = value6;
			value.remainTime = value7;
			value.duration = value8;
		}
	}
}
