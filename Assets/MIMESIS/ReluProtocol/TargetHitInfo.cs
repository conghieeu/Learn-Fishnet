using System.Buffers;
using Bifrost.ConstEnum;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class TargetHitInfo : IMemoryPackable<TargetHitInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class TargetHitInfoFormatter : MemoryPackFormatter<TargetHitInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TargetHitInfo value)
			{
				TargetHitInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref TargetHitInfo value)
			{
				TargetHitInfo.Deserialize(ref reader, ref value);
			}
		}

		public int targetID { get; set; }

		public CCType actionAbnormalHitType { get; set; }

		public long pushTime { get; set; }

		public long hitDelay { get; set; }

		public long damage { get; set; }

		public ImmuneType immuneType { get; set; }

		public PosWithRot basePosition { get; set; } = new PosWithRot();

		public PosWithRot hitPosition { get; set; } = new PosWithRot();

		static TargetHitInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<TargetHitInfo>())
			{
				MemoryPackFormatterProvider.Register(new TargetHitInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<TargetHitInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<TargetHitInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CCType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<CCType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ImmuneType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ImmuneType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TargetHitInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<int, CCType, long, long, long, ImmuneType>(8, value.targetID, value.actionAbnormalHitType, value.pushTime, value.hitDelay, value.damage, value.immuneType);
			writer.WritePackable<PosWithRot>(value.basePosition);
			writer.WritePackable<PosWithRot>(value.hitPosition);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref TargetHitInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			CCType value3;
			long value4;
			long value5;
			long value6;
			ImmuneType value7;
			PosWithRot value8;
			PosWithRot value9;
			if (memberCount == 8)
			{
				if (value != null)
				{
					value2 = value.targetID;
					value3 = value.actionAbnormalHitType;
					value4 = value.pushTime;
					value5 = value.hitDelay;
					value6 = value.damage;
					value7 = value.immuneType;
					value8 = value.basePosition;
					value9 = value.hitPosition;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<CCType>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanaged<long>(out value6);
					reader.ReadUnmanaged<ImmuneType>(out value7);
					reader.ReadPackable(ref value8);
					reader.ReadPackable(ref value9);
					goto IL_01aa;
				}
				reader.ReadUnmanaged<int, CCType, long, long, long, ImmuneType>(out value2, out value3, out value4, out value5, out value6, out value7);
				value8 = reader.ReadPackable<PosWithRot>();
				value9 = reader.ReadPackable<PosWithRot>();
			}
			else
			{
				if (memberCount > 8)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(TargetHitInfo), 8, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = CCType.None;
					value4 = 0L;
					value5 = 0L;
					value6 = 0L;
					value7 = ImmuneType.None;
					value8 = null;
					value9 = null;
				}
				else
				{
					value2 = value.targetID;
					value3 = value.actionAbnormalHitType;
					value4 = value.pushTime;
					value5 = value.hitDelay;
					value6 = value.damage;
					value7 = value.immuneType;
					value8 = value.basePosition;
					value9 = value.hitPosition;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<CCType>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<long>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<long>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<long>(out value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<ImmuneType>(out value7);
										if (memberCount != 6)
										{
											reader.ReadPackable(ref value8);
											if (memberCount != 7)
											{
												reader.ReadPackable(ref value9);
												_ = 8;
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
					goto IL_01aa;
				}
			}
			value = new TargetHitInfo
			{
				targetID = value2,
				actionAbnormalHitType = value3,
				pushTime = value4,
				hitDelay = value5,
				damage = value6,
				immuneType = value7,
				basePosition = value8,
				hitPosition = value9
			};
			return;
			IL_01aa:
			value.targetID = value2;
			value.actionAbnormalHitType = value3;
			value.pushTime = value4;
			value.hitDelay = value5;
			value.damage = value6;
			value.immuneType = value7;
			value.basePosition = value8;
			value.hitPosition = value9;
		}
	}
}
