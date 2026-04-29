using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Bifrost.ConstEnum;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[Serializable]
	[MemoryPackable(GenerateType.VersionTolerant)]
	public class MMSaveGameDataItemElement : IMemoryPackable<MMSaveGameDataItemElement>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class MMSaveGameDataItemElementFormatter : MemoryPackFormatter<MMSaveGameDataItemElement>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MMSaveGameDataItemElement value)
			{
				MMSaveGameDataItemElement.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref MMSaveGameDataItemElement value)
			{
				MMSaveGameDataItemElement.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackOrder(0)]
		public ItemType ItemType { get; set; }

		[MemoryPackOrder(1)]
		public int ItemMasterID { get; set; }

		[MemoryPackOrder(2)]
		public int RemainCount { get; set; }

		[MemoryPackOrder(3)]
		public int RemainDurability { get; set; }

		[MemoryPackOrder(4)]
		public int RemainAmount { get; set; }

		[MemoryPackOrder(5)]
		public int Price { get; set; }

		public ItemElement? ToItemElement(IVroom vroom)
		{
			return vroom?.GetNewItemElement(ItemMasterID, isFake: false, RemainCount, RemainDurability, RemainAmount, Price);
		}

		static MMSaveGameDataItemElement()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<MMSaveGameDataItemElement>())
			{
				MemoryPackFormatterProvider.Register(new MMSaveGameDataItemElementFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MMSaveGameDataItemElement[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<MMSaveGameDataItemElement>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ItemType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ItemType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MMSaveGameDataItemElement? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(6);
			writer.WriteVarInt(Unsafe.SizeOf<ItemType>());
			writer.WriteVarInt(Unsafe.SizeOf<int>());
			writer.WriteVarInt(Unsafe.SizeOf<int>());
			writer.WriteVarInt(Unsafe.SizeOf<int>());
			writer.WriteVarInt(Unsafe.SizeOf<int>());
			writer.WriteVarInt(Unsafe.SizeOf<int>());
			writer.WriteUnmanaged<ItemType, int, int, int, int, int>(value.ItemType, value.ItemMasterID, value.RemainCount, value.RemainDurability, value.RemainAmount, value.Price);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref MMSaveGameDataItemElement? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Span<int> span = stackalloc int[(int)memberCount];
			for (int i = 0; i < memberCount; i++)
			{
				span[i] = reader.ReadVarIntInt32();
			}
			int num = 6;
			ItemType value2;
			int value3;
			int value4;
			int value5;
			int value6;
			int value7;
			if (memberCount == 6)
			{
				if (value != null)
				{
					value2 = value.ItemType;
					value3 = value.ItemMasterID;
					value4 = value.RemainCount;
					value5 = value.RemainDurability;
					value6 = value.RemainAmount;
					value7 = value.Price;
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<ItemType>(out value2);
					}
					if (span[1] != 0)
					{
						reader.ReadUnmanaged<int>(out value3);
					}
					if (span[2] != 0)
					{
						reader.ReadUnmanaged<int>(out value4);
					}
					if (span[3] != 0)
					{
						reader.ReadUnmanaged<int>(out value5);
					}
					if (span[4] != 0)
					{
						reader.ReadUnmanaged<int>(out value6);
					}
					if (span[5] != 0)
					{
						reader.ReadUnmanaged<int>(out value7);
					}
					goto IL_0273;
				}
				if (span[0] == 0)
				{
					value2 = ItemType.Consumable;
				}
				else
				{
					reader.ReadUnmanaged<ItemType>(out value2);
				}
				if (span[1] == 0)
				{
					value3 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value3);
				}
				if (span[2] == 0)
				{
					value4 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value4);
				}
				if (span[3] == 0)
				{
					value5 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value5);
				}
				if (span[4] == 0)
				{
					value6 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value6);
				}
				if (span[5] == 0)
				{
					value7 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value7);
				}
			}
			else
			{
				if (value == null)
				{
					value2 = ItemType.Consumable;
					value3 = 0;
					value4 = 0;
					value5 = 0;
					value6 = 0;
					value7 = 0;
				}
				else
				{
					value2 = value.ItemType;
					value3 = value.ItemMasterID;
					value4 = value.RemainCount;
					value5 = value.RemainDurability;
					value6 = value.RemainAmount;
					value7 = value.Price;
				}
				if (memberCount != 0)
				{
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<ItemType>(out value2);
					}
					if (memberCount != 1)
					{
						if (span[1] != 0)
						{
							reader.ReadUnmanaged<int>(out value3);
						}
						if (memberCount != 2)
						{
							if (span[2] != 0)
							{
								reader.ReadUnmanaged<int>(out value4);
							}
							if (memberCount != 3)
							{
								if (span[3] != 0)
								{
									reader.ReadUnmanaged<int>(out value5);
								}
								if (memberCount != 4)
								{
									if (span[4] != 0)
									{
										reader.ReadUnmanaged<int>(out value6);
									}
									if (memberCount != 5)
									{
										if (span[5] != 0)
										{
											reader.ReadUnmanaged<int>(out value7);
										}
										_ = 6;
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0273;
				}
			}
			value = new MMSaveGameDataItemElement
			{
				ItemType = value2,
				ItemMasterID = value3,
				RemainCount = value4,
				RemainDurability = value5,
				RemainAmount = value6,
				Price = value7
			};
			goto IL_02de;
			IL_0273:
			value.ItemType = value2;
			value.ItemMasterID = value3;
			value.RemainCount = value4;
			value.RemainDurability = value5;
			value.RemainAmount = value6;
			value.Price = value7;
			goto IL_02de;
			IL_02de:
			if (memberCount != num)
			{
				for (int j = num; j < memberCount; j++)
				{
					reader.Advance(span[j]);
				}
			}
		}
	}
}
