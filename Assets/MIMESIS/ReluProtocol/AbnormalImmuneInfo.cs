using System.Buffers;
using Bifrost.ConstEnum;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class AbnormalImmuneInfo : IMemoryPackable<AbnormalImmuneInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class AbnormalImmuneInfoFormatter : MemoryPackFormatter<AbnormalImmuneInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalImmuneInfo value)
			{
				AbnormalImmuneInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref AbnormalImmuneInfo value)
			{
				AbnormalImmuneInfo.Deserialize(ref reader, ref value);
			}
		}

		public AbnormalDataSyncType changeType { get; set; }

		public int abnormalMasterID { get; set; }

		public long abnormalSyncID { get; set; }

		public long syncID { get; set; }

		public ImmuneType immuneType { get; set; }

		public float remainTime { get; set; }

		public long duration { get; set; }

		static AbnormalImmuneInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalImmuneInfo>())
			{
				MemoryPackFormatterProvider.Register(new AbnormalImmuneInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalImmuneInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<AbnormalImmuneInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalDataSyncType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<AbnormalDataSyncType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ImmuneType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ImmuneType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalImmuneInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<AbnormalDataSyncType, int, long, long, ImmuneType, float, long>(7, value.changeType, value.abnormalMasterID, value.abnormalSyncID, value.syncID, value.immuneType, value.remainTime, value.duration);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref AbnormalImmuneInfo? value)
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
			ImmuneType value6;
			float value7;
			long value8;
			if (memberCount == 7)
			{
				if (value != null)
				{
					value2 = value.changeType;
					value3 = value.abnormalMasterID;
					value4 = value.abnormalSyncID;
					value5 = value.syncID;
					value6 = value.immuneType;
					value7 = value.remainTime;
					value8 = value.duration;
					reader.ReadUnmanaged<AbnormalDataSyncType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanaged<ImmuneType>(out value6);
					reader.ReadUnmanaged<float>(out value7);
					reader.ReadUnmanaged<long>(out value8);
					goto IL_0177;
				}
				reader.ReadUnmanaged<AbnormalDataSyncType, int, long, long, ImmuneType, float, long>(out value2, out value3, out value4, out value5, out value6, out value7, out value8);
			}
			else
			{
				if (memberCount > 7)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AbnormalImmuneInfo), 7, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = AbnormalDataSyncType.Add;
					value3 = 0;
					value4 = 0L;
					value5 = 0L;
					value6 = ImmuneType.None;
					value7 = 0f;
					value8 = 0L;
				}
				else
				{
					value2 = value.changeType;
					value3 = value.abnormalMasterID;
					value4 = value.abnormalSyncID;
					value5 = value.syncID;
					value6 = value.immuneType;
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
									reader.ReadUnmanaged<ImmuneType>(out value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<float>(out value7);
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
					goto IL_0177;
				}
			}
			value = new AbnormalImmuneInfo
			{
				changeType = value2,
				abnormalMasterID = value3,
				abnormalSyncID = value4,
				syncID = value5,
				immuneType = value6,
				remainTime = value7,
				duration = value8
			};
			return;
			IL_0177:
			value.changeType = value2;
			value.abnormalMasterID = value3;
			value.abnormalSyncID = value4;
			value.syncID = value5;
			value.immuneType = value6;
			value.remainTime = value7;
			value.duration = value8;
		}
	}
}
