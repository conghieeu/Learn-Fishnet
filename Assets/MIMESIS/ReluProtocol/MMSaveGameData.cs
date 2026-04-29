using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[Serializable]
	[MemoryPackable(GenerateType.VersionTolerant)]
	public class MMSaveGameData : IMemoryPackable<MMSaveGameData>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class MMSaveGameDataFormatter : MemoryPackFormatter<MMSaveGameData>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MMSaveGameData value)
			{
				MMSaveGameData.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref MMSaveGameData value)
			{
				MMSaveGameData.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackOrder(0)]
		public int Version { get; set; }

		[MemoryPackOrder(1)]
		public int SlotID { get; set; }

		[MemoryPackOrder(2)]
		public DateTime RegDateTime { get; set; }

		[MemoryPackOrder(3)]
		public int CycleCount { get; set; }

		[MemoryPackOrder(4)]
		public int DayCount { get; set; }

		[MemoryPackOrder(5)]
		public bool TramRepaired { get; set; }

		[MemoryPackOrder(6)]
		public int Currency { get; set; }

		[MemoryPackOrder(7)]
		public List<string> PlayerNames { get; set; }

		[MemoryPackOrder(8)]
		public List<MMSaveGameDataLootingObjectInfo> LootingObjectInfos { get; set; }

		[MemoryPackOrder(9)]
		public List<MMSaveGameDataItemElement> PlayerItemElements { get; set; }

		[MemoryPackOrder(10)]
		public int BoostedItemMasterID { get; set; }

		[MemoryPackOrder(11)]
		public float BoostedItemRate { get; set; }

		[MemoryPackOrder(12)]
		public Dictionary<int, int> PriceForItems { get; set; }

		[MemoryPackOrder(13)]
		public Dictionary<int, MMSaveGameDataItemElement> Stashes { get; set; }

		[MemoryPackOrder(14)]
		public List<int> TramUpgradeList { get; set; }

		[MemoryPackOrder(15)]
		public List<int> TramUpgradeCandidateList { get; set; }

		public static string GetSaveFileName(int slotID)
		{
			return $"MMGameData{slotID:D002}.sav";
		}

		public static bool CheckSaveSlotID(int slotID, bool includeAutoSlot = true)
		{
			if (slotID == -1)
			{
				return false;
			}
			if ((includeAutoSlot && slotID == 0) || (slotID >= 1 && slotID <= 3))
			{
				return true;
			}
			return false;
		}

		static MMSaveGameData()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<MMSaveGameData>())
			{
				MemoryPackFormatterProvider.Register(new MMSaveGameDataFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MMSaveGameData[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<MMSaveGameData>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<string>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<string>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<MMSaveGameDataLootingObjectInfo>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<MMSaveGameDataLootingObjectInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<MMSaveGameDataItemElement>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<MMSaveGameDataItemElement>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, int>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, MMSaveGameDataItemElement>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, MMSaveGameDataItemElement>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MMSaveGameData? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			ReusableLinkedArrayBufferWriter writer2 = ReusableLinkedArrayBufferWriterPool.Rent();
			try
			{
				Span<int> span = stackalloc int[16];
				MemoryPackWriter<ReusableLinkedArrayBufferWriter> writer3 = new MemoryPackWriter<ReusableLinkedArrayBufferWriter>(ref writer2, writer.OptionalState);
				writer3.WriteUnmanaged<int>(value.Version);
				span[0] = writer3.WrittenCount;
				writer3.WriteUnmanaged<int>(value.SlotID);
				span[1] = writer3.WrittenCount;
				writer3.WriteUnmanaged<DateTime>(value.RegDateTime);
				span[2] = writer3.WrittenCount;
				writer3.WriteUnmanaged<int>(value.CycleCount);
				span[3] = writer3.WrittenCount;
				writer3.WriteUnmanaged<int>(value.DayCount);
				span[4] = writer3.WrittenCount;
				writer3.WriteUnmanaged<bool>(value.TramRepaired);
				span[5] = writer3.WrittenCount;
				writer3.WriteUnmanaged<int>(value.Currency);
				span[6] = writer3.WrittenCount;
				writer3.WriteValue<List<string>>(value.PlayerNames);
				span[7] = writer3.WrittenCount;
				ListFormatter.SerializePackable(ref writer3, value.LootingObjectInfos);
				span[8] = writer3.WrittenCount;
				ListFormatter.SerializePackable(ref writer3, value.PlayerItemElements);
				span[9] = writer3.WrittenCount;
				writer3.WriteUnmanaged<int>(value.BoostedItemMasterID);
				span[10] = writer3.WrittenCount;
				writer3.WriteUnmanaged<float>(value.BoostedItemRate);
				span[11] = writer3.WrittenCount;
				writer3.WriteValue<Dictionary<int, int>>(value.PriceForItems);
				span[12] = writer3.WrittenCount;
				writer3.WriteValue<Dictionary<int, MMSaveGameDataItemElement>>(value.Stashes);
				span[13] = writer3.WrittenCount;
				writer3.WriteValue<List<int>>(value.TramUpgradeList);
				span[14] = writer3.WrittenCount;
				writer3.WriteValue<List<int>>(value.TramUpgradeCandidateList);
				span[15] = writer3.WrittenCount;
				writer3.Flush();
				writer.WriteObjectHeader(16);
				for (int i = 0; i < 16; i++)
				{
					int x = ((i != 0) ? (span[i] - span[i - 1]) : span[i]);
					writer.WriteVarInt(x);
				}
				writer2.WriteToAndReset(ref writer);
			}
			finally
			{
				ReusableLinkedArrayBufferWriterPool.Return(writer2);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref MMSaveGameData? value)
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
			int num = 16;
			int value2;
			int value3;
			DateTime value4;
			int value5;
			int value6;
			bool value7;
			int value8;
			List<string> value9;
			List<MMSaveGameDataLootingObjectInfo> value10;
			List<MMSaveGameDataItemElement> value11;
			int value12;
			float value13;
			Dictionary<int, int> value14;
			Dictionary<int, MMSaveGameDataItemElement> value15;
			List<int> value16;
			List<int> value17;
			if (memberCount == 16)
			{
				if (value != null)
				{
					value2 = value.Version;
					value3 = value.SlotID;
					value4 = value.RegDateTime;
					value5 = value.CycleCount;
					value6 = value.DayCount;
					value7 = value.TramRepaired;
					value8 = value.Currency;
					value9 = value.PlayerNames;
					value10 = value.LootingObjectInfos;
					value11 = value.PlayerItemElements;
					value12 = value.BoostedItemMasterID;
					value13 = value.BoostedItemRate;
					value14 = value.PriceForItems;
					value15 = value.Stashes;
					value16 = value.TramUpgradeList;
					value17 = value.TramUpgradeCandidateList;
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<int>(out value2);
					}
					if (span[1] != 0)
					{
						reader.ReadUnmanaged<int>(out value3);
					}
					if (span[2] != 0)
					{
						reader.ReadUnmanaged<DateTime>(out value4);
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
						reader.ReadUnmanaged<bool>(out value7);
					}
					if (span[6] != 0)
					{
						reader.ReadUnmanaged<int>(out value8);
					}
					if (span[7] != 0)
					{
						reader.ReadValue(ref value9);
					}
					if (span[8] != 0)
					{
						ListFormatter.DeserializePackable(ref reader, ref value10);
					}
					if (span[9] != 0)
					{
						ListFormatter.DeserializePackable(ref reader, ref value11);
					}
					if (span[10] != 0)
					{
						reader.ReadUnmanaged<int>(out value12);
					}
					if (span[11] != 0)
					{
						reader.ReadUnmanaged<float>(out value13);
					}
					if (span[12] != 0)
					{
						reader.ReadValue(ref value14);
					}
					if (span[13] != 0)
					{
						reader.ReadValue(ref value15);
					}
					if (span[14] != 0)
					{
						reader.ReadValue(ref value16);
					}
					if (span[15] != 0)
					{
						reader.ReadValue(ref value17);
					}
					goto IL_062e;
				}
				if (span[0] == 0)
				{
					value2 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value2);
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
					value4 = default(DateTime);
				}
				else
				{
					reader.ReadUnmanaged<DateTime>(out value4);
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
					value7 = false;
				}
				else
				{
					reader.ReadUnmanaged<bool>(out value7);
				}
				if (span[6] == 0)
				{
					value8 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value8);
				}
				value9 = ((span[7] != 0) ? reader.ReadValue<List<string>>() : null);
				value10 = ((span[8] != 0) ? ListFormatter.DeserializePackable<MMSaveGameDataLootingObjectInfo>(ref reader) : null);
				value11 = ((span[9] != 0) ? ListFormatter.DeserializePackable<MMSaveGameDataItemElement>(ref reader) : null);
				if (span[10] == 0)
				{
					value12 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value12);
				}
				if (span[11] == 0)
				{
					value13 = 0f;
				}
				else
				{
					reader.ReadUnmanaged<float>(out value13);
				}
				value14 = ((span[12] != 0) ? reader.ReadValue<Dictionary<int, int>>() : null);
				value15 = ((span[13] != 0) ? reader.ReadValue<Dictionary<int, MMSaveGameDataItemElement>>() : null);
				value16 = ((span[14] != 0) ? reader.ReadValue<List<int>>() : null);
				value17 = ((span[15] != 0) ? reader.ReadValue<List<int>>() : null);
			}
			else
			{
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = default(DateTime);
					value5 = 0;
					value6 = 0;
					value7 = false;
					value8 = 0;
					value9 = null;
					value10 = null;
					value11 = null;
					value12 = 0;
					value13 = 0f;
					value14 = null;
					value15 = null;
					value16 = null;
					value17 = null;
				}
				else
				{
					value2 = value.Version;
					value3 = value.SlotID;
					value4 = value.RegDateTime;
					value5 = value.CycleCount;
					value6 = value.DayCount;
					value7 = value.TramRepaired;
					value8 = value.Currency;
					value9 = value.PlayerNames;
					value10 = value.LootingObjectInfos;
					value11 = value.PlayerItemElements;
					value12 = value.BoostedItemMasterID;
					value13 = value.BoostedItemRate;
					value14 = value.PriceForItems;
					value15 = value.Stashes;
					value16 = value.TramUpgradeList;
					value17 = value.TramUpgradeCandidateList;
				}
				if (memberCount != 0)
				{
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<int>(out value2);
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
								reader.ReadUnmanaged<DateTime>(out value4);
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
											reader.ReadUnmanaged<bool>(out value7);
										}
										if (memberCount != 6)
										{
											if (span[6] != 0)
											{
												reader.ReadUnmanaged<int>(out value8);
											}
											if (memberCount != 7)
											{
												if (span[7] != 0)
												{
													reader.ReadValue(ref value9);
												}
												if (memberCount != 8)
												{
													if (span[8] != 0)
													{
														ListFormatter.DeserializePackable(ref reader, ref value10);
													}
													if (memberCount != 9)
													{
														if (span[9] != 0)
														{
															ListFormatter.DeserializePackable(ref reader, ref value11);
														}
														if (memberCount != 10)
														{
															if (span[10] != 0)
															{
																reader.ReadUnmanaged<int>(out value12);
															}
															if (memberCount != 11)
															{
																if (span[11] != 0)
																{
																	reader.ReadUnmanaged<float>(out value13);
																}
																if (memberCount != 12)
																{
																	if (span[12] != 0)
																	{
																		reader.ReadValue(ref value14);
																	}
																	if (memberCount != 13)
																	{
																		if (span[13] != 0)
																		{
																			reader.ReadValue(ref value15);
																		}
																		if (memberCount != 14)
																		{
																			if (span[14] != 0)
																			{
																				reader.ReadValue(ref value16);
																			}
																			if (memberCount != 15)
																			{
																				if (span[15] != 0)
																				{
																					reader.ReadValue(ref value17);
																				}
																				_ = 16;
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
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_062e;
				}
			}
			value = new MMSaveGameData
			{
				Version = value2,
				SlotID = value3,
				RegDateTime = value4,
				CycleCount = value5,
				DayCount = value6,
				TramRepaired = value7,
				Currency = value8,
				PlayerNames = value9,
				LootingObjectInfos = value10,
				PlayerItemElements = value11,
				BoostedItemMasterID = value12,
				BoostedItemRate = value13,
				PriceForItems = value14,
				Stashes = value15,
				TramUpgradeList = value16,
				TramUpgradeCandidateList = value17
			};
			goto IL_0746;
			IL_0746:
			if (memberCount != num)
			{
				for (int j = num; j < memberCount; j++)
				{
					reader.Advance(span[j]);
				}
			}
			return;
			IL_062e:
			value.Version = value2;
			value.SlotID = value3;
			value.RegDateTime = value4;
			value.CycleCount = value5;
			value.DayCount = value6;
			value.TramRepaired = value7;
			value.Currency = value8;
			value.PlayerNames = value9;
			value.LootingObjectInfos = value10;
			value.PlayerItemElements = value11;
			value.BoostedItemMasterID = value12;
			value.BoostedItemRate = value13;
			value.PriceForItems = value14;
			value.Stashes = value15;
			value.TramUpgradeList = value16;
			value.TramUpgradeCandidateList = value17;
			goto IL_0746;
		}
	}
}
