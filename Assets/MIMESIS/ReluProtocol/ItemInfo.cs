using System.Buffers;
using Bifrost.ConstEnum;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class ItemInfo : IMemoryPackable<ItemInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ItemInfoFormatter : MemoryPackFormatter<ItemInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ItemInfo value)
			{
				ItemInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ItemInfo value)
			{
				ItemInfo.Deserialize(ref reader, ref value);
			}
		}

		public ItemType itemType { get; set; }

		public long itemID { get; set; }

		public int itemMasterID { get; set; }

		public int stackCount { get; set; }

		public int durability { get; set; }

		public int remainGauge { get; set; }

		public bool isTurnOn { get; set; }

		public bool isFake { get; set; }

		public int price { get; set; }

		public ItemInfo Clone()
		{
			return new ItemInfo
			{
				itemType = itemType,
				itemID = itemID,
				itemMasterID = itemMasterID,
				stackCount = stackCount,
				durability = durability,
				remainGauge = remainGauge,
				isTurnOn = isTurnOn,
				isFake = isFake,
				price = price
			};
		}

		public override string ToString()
		{
			if (itemMasterID == 0)
			{
				return "(Empty)";
			}
			return itemType switch
			{
				ItemType.Consumable => $"({itemType}: MasterID={itemMasterID}, ID={itemID}, Stack={stackCount}, Fake={isFake}, Price={price})", 
				ItemType.Equipment => $"({itemType}: MasterID={itemMasterID}, ID={itemID}, Dur={durability}, Guage={remainGauge}, On={isTurnOn}, Fake={isFake}, Price={price})", 
				ItemType.Miscellany => $"({itemType}: MasterID={itemMasterID}, ID={itemID}, Stack={stackCount}, Fake={isFake}, Price={price})", 
				_ => $"({itemType}: MasterID={itemMasterID}, ID={itemID}, Stack={stackCount}, Dur={durability}, Guage={remainGauge}, On={isTurnOn}, Fake={isFake}, Price={price})", 
			};
		}

		static ItemInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ItemInfo>())
			{
				MemoryPackFormatterProvider.Register(new ItemInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ItemInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ItemInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ItemType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ItemType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ItemInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<ItemType, long, int, int, int, int, bool, bool, int>(9, value.itemType, value.itemID, value.itemMasterID, value.stackCount, value.durability, value.remainGauge, value.isTurnOn, value.isFake, value.price);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref ItemInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			ItemType value2;
			long value3;
			int value4;
			int value5;
			int value6;
			int value7;
			bool value8;
			bool value9;
			int value10;
			if (memberCount == 9)
			{
				if (value != null)
				{
					value2 = value.itemType;
					value3 = value.itemID;
					value4 = value.itemMasterID;
					value5 = value.stackCount;
					value6 = value.durability;
					value7 = value.remainGauge;
					value8 = value.isTurnOn;
					value9 = value.isFake;
					value10 = value.price;
					reader.ReadUnmanaged<ItemType>(out value2);
					reader.ReadUnmanaged<long>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					reader.ReadUnmanaged<int>(out value6);
					reader.ReadUnmanaged<int>(out value7);
					reader.ReadUnmanaged<bool>(out value8);
					reader.ReadUnmanaged<bool>(out value9);
					reader.ReadUnmanaged<int>(out value10);
					goto IL_01cb;
				}
				reader.ReadUnmanaged<ItemType, long, int, int, int, int, bool, bool, int>(out value2, out value3, out value4, out value5, out value6, out value7, out value8, out value9, out value10);
			}
			else
			{
				if (memberCount > 9)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ItemInfo), 9, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = ItemType.Consumable;
					value3 = 0L;
					value4 = 0;
					value5 = 0;
					value6 = 0;
					value7 = 0;
					value8 = false;
					value9 = false;
					value10 = 0;
				}
				else
				{
					value2 = value.itemType;
					value3 = value.itemID;
					value4 = value.itemMasterID;
					value5 = value.stackCount;
					value6 = value.durability;
					value7 = value.remainGauge;
					value8 = value.isTurnOn;
					value9 = value.isFake;
					value10 = value.price;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<ItemType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<long>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<int>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<int>(out value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<int>(out value7);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<bool>(out value8);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<bool>(out value9);
												if (memberCount != 8)
												{
													reader.ReadUnmanaged<int>(out value10);
													_ = 9;
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
					goto IL_01cb;
				}
			}
			value = new ItemInfo
			{
				itemType = value2,
				itemID = value3,
				itemMasterID = value4,
				stackCount = value5,
				durability = value6,
				remainGauge = value7,
				isTurnOn = value8,
				isFake = value9,
				price = value10
			};
			return;
			IL_01cb:
			value.itemType = value2;
			value.itemID = value3;
			value.itemMasterID = value4;
			value.stackCount = value5;
			value.durability = value6;
			value.remainGauge = value7;
			value.isTurnOn = value8;
			value.isFake = value9;
			value.price = value10;
		}
	}
}
