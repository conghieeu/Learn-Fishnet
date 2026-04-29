using System.Buffers;
using Bifrost.ConstEnum;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class AbnormalCCInfo : IMemoryPackable<AbnormalCCInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class AbnormalCCInfoFormatter : MemoryPackFormatter<AbnormalCCInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalCCInfo value)
			{
				AbnormalCCInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref AbnormalCCInfo value)
			{
				AbnormalCCInfo.Deserialize(ref reader, ref value);
			}
		}

		public AbnormalDataSyncType changeType { get; set; }

		public int abnormalMasterID { get; set; }

		public long syncID { get; set; }

		public long abnormalSyncID { get; set; }

		public CCType ccType { get; set; }

		public long remainTime { get; set; }

		public long duration { get; set; }

		public long pushTime { get; set; }

		public PosWithRot basePosition { get; set; } = new PosWithRot();

		public PosWithRot hitPosition { get; set; } = new PosWithRot();

		static AbnormalCCInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalCCInfo>())
			{
				MemoryPackFormatterProvider.Register(new AbnormalCCInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalCCInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<AbnormalCCInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AbnormalDataSyncType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<AbnormalDataSyncType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CCType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<CCType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalCCInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<AbnormalDataSyncType, int, long, long, CCType, long, long, long>(10, value.changeType, value.abnormalMasterID, value.syncID, value.abnormalSyncID, value.ccType, value.remainTime, value.duration, value.pushTime);
			writer.WritePackable<PosWithRot>(value.basePosition);
			writer.WritePackable<PosWithRot>(value.hitPosition);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref AbnormalCCInfo? value)
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
			CCType value6;
			long value7;
			long value8;
			long value9;
			PosWithRot value10;
			PosWithRot value11;
			if (memberCount == 10)
			{
				if (value != null)
				{
					value2 = value.changeType;
					value3 = value.abnormalMasterID;
					value4 = value.syncID;
					value5 = value.abnormalSyncID;
					value6 = value.ccType;
					value7 = value.remainTime;
					value8 = value.duration;
					value9 = value.pushTime;
					value10 = value.basePosition;
					value11 = value.hitPosition;
					reader.ReadUnmanaged<AbnormalDataSyncType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanaged<CCType>(out value6);
					reader.ReadUnmanaged<long>(out value7);
					reader.ReadUnmanaged<long>(out value8);
					reader.ReadUnmanaged<long>(out value9);
					reader.ReadPackable(ref value10);
					reader.ReadPackable(ref value11);
					goto IL_0207;
				}
				reader.ReadUnmanaged<AbnormalDataSyncType, int, long, long, CCType, long, long, long>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9);
				value10 = reader.ReadPackable<PosWithRot>();
				value11 = reader.ReadPackable<PosWithRot>();
			}
			else
			{
				if (memberCount > 10)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AbnormalCCInfo), 10, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = AbnormalDataSyncType.Add;
					value3 = 0;
					value4 = 0L;
					value5 = 0L;
					value6 = CCType.None;
					value7 = 0L;
					value8 = 0L;
					value9 = 0L;
					value10 = null;
					value11 = null;
				}
				else
				{
					value2 = value.changeType;
					value3 = value.abnormalMasterID;
					value4 = value.syncID;
					value5 = value.abnormalSyncID;
					value6 = value.ccType;
					value7 = value.remainTime;
					value8 = value.duration;
					value9 = value.pushTime;
					value10 = value.basePosition;
					value11 = value.hitPosition;
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
									reader.ReadUnmanaged<CCType>(out value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<long>(out value7);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<long>(out value8);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<long>(out value9);
												if (memberCount != 8)
												{
													reader.ReadPackable(ref value10);
													if (memberCount != 9)
													{
														reader.ReadPackable(ref value11);
														_ = 10;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0207;
				}
			}
			value = new AbnormalCCInfo
			{
				changeType = value2,
				abnormalMasterID = value3,
				syncID = value4,
				abnormalSyncID = value5,
				ccType = value6,
				remainTime = value7,
				duration = value8,
				pushTime = value9,
				basePosition = value10,
				hitPosition = value11
			};
			return;
			IL_0207:
			value.changeType = value2;
			value.abnormalMasterID = value3;
			value.syncID = value4;
			value.abnormalSyncID = value5;
			value.ccType = value6;
			value.remainTime = value7;
			value.duration = value8;
			value.pushTime = value9;
			value.basePosition = value10;
			value.hitPosition = value11;
		}
	}
}
