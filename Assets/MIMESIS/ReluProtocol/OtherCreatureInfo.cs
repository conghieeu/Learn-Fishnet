using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class OtherCreatureInfo : ActorBaseInfo, IMemoryPackable<OtherCreatureInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class OtherCreatureInfoFormatter : MemoryPackFormatter<OtherCreatureInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref OtherCreatureInfo value)
			{
				OtherCreatureInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref OtherCreatureInfo value)
			{
				OtherCreatureInfo.Deserialize(ref reader, ref value);
			}
		}

		public VCreatureLifeCycle actorLifeCycle { get; set; }

		public StatCollection statInfoCollection { get; set; } = new StatCollection();

		public List<int> factions { get; set; } = new List<int>();

		public ItemInfo onHandItem { get; set; } = new ItemInfo();

		public Dictionary<int, int> attachingActorIDs { get; set; } = new Dictionary<int, int>();

		public int attachedActorID { get; set; }

		public List<int> activatedAuraMasterIDs { get; set; } = new List<int>();

		public Dictionary<int, ItemInfo> inventories { get; set; } = new Dictionary<int, ItemInfo>();

		static OtherCreatureInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<OtherCreatureInfo>())
			{
				MemoryPackFormatterProvider.Register(new OtherCreatureInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<OtherCreatureInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<OtherCreatureInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ActorType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ActorType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReasonOfSpawn>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ReasonOfSpawn>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<VCreatureLifeCycle>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<VCreatureLifeCycle>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, int>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, ItemInfo>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, ItemInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref OtherCreatureInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<ActorType, int, int>(15, value.actorType, value.actorID, value.masterID);
			writer.WriteString(value.actorName);
			writer.WritePackable<PosWithRot>(value.position);
			writer.WriteUnmanaged<long, ReasonOfSpawn, VCreatureLifeCycle>(value.UID, value.reasonOfSpawn, value.actorLifeCycle);
			writer.WritePackable<StatCollection>(value.statInfoCollection);
			writer.WriteValue<List<int>>(value.factions);
			writer.WritePackable<ItemInfo>(value.onHandItem);
			writer.WriteValue<Dictionary<int, int>>(value.attachingActorIDs);
			writer.WriteUnmanaged<int>(value.attachedActorID);
			writer.WriteValue<List<int>>(value.activatedAuraMasterIDs);
			writer.WriteValue<Dictionary<int, ItemInfo>>(value.inventories);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref OtherCreatureInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			ActorType value2;
			int value3;
			int value4;
			PosWithRot value5;
			long value6;
			ReasonOfSpawn value7;
			VCreatureLifeCycle value8;
			StatCollection value9;
			List<int> value10;
			ItemInfo value11;
			Dictionary<int, int> value12;
			int value13;
			List<int> value14;
			Dictionary<int, ItemInfo> value15;
			string text;
			if (memberCount == 15)
			{
				if (value != null)
				{
					value2 = value.actorType;
					value3 = value.actorID;
					value4 = value.masterID;
					text = value.actorName;
					value5 = value.position;
					value6 = value.UID;
					value7 = value.reasonOfSpawn;
					value8 = value.actorLifeCycle;
					value9 = value.statInfoCollection;
					value10 = value.factions;
					value11 = value.onHandItem;
					value12 = value.attachingActorIDs;
					value13 = value.attachedActorID;
					value14 = value.activatedAuraMasterIDs;
					value15 = value.inventories;
					reader.ReadUnmanaged<ActorType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					text = reader.ReadString();
					reader.ReadPackable(ref value5);
					reader.ReadUnmanaged<long>(out value6);
					reader.ReadUnmanaged<ReasonOfSpawn>(out value7);
					reader.ReadUnmanaged<VCreatureLifeCycle>(out value8);
					reader.ReadPackable(ref value9);
					reader.ReadValue(ref value10);
					reader.ReadPackable(ref value11);
					reader.ReadValue(ref value12);
					reader.ReadUnmanaged<int>(out value13);
					reader.ReadValue(ref value14);
					reader.ReadValue(ref value15);
					goto IL_0324;
				}
				reader.ReadUnmanaged<ActorType, int, int>(out value2, out value3, out value4);
				text = reader.ReadString();
				value5 = reader.ReadPackable<PosWithRot>();
				reader.ReadUnmanaged<long, ReasonOfSpawn, VCreatureLifeCycle>(out value6, out value7, out value8);
				value9 = reader.ReadPackable<StatCollection>();
				value10 = reader.ReadValue<List<int>>();
				value11 = reader.ReadPackable<ItemInfo>();
				value12 = reader.ReadValue<Dictionary<int, int>>();
				reader.ReadUnmanaged<int>(out value13);
				value14 = reader.ReadValue<List<int>>();
				value15 = reader.ReadValue<Dictionary<int, ItemInfo>>();
			}
			else
			{
				if (memberCount > 15)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(OtherCreatureInfo), 15, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = ActorType.None;
					value3 = 0;
					value4 = 0;
					text = null;
					value5 = null;
					value6 = 0L;
					value7 = ReasonOfSpawn.None;
					value8 = VCreatureLifeCycle.Ready;
					value9 = null;
					value10 = null;
					value11 = null;
					value12 = null;
					value13 = 0;
					value14 = null;
					value15 = null;
				}
				else
				{
					value2 = value.actorType;
					value3 = value.actorID;
					value4 = value.masterID;
					text = value.actorName;
					value5 = value.position;
					value6 = value.UID;
					value7 = value.reasonOfSpawn;
					value8 = value.actorLifeCycle;
					value9 = value.statInfoCollection;
					value10 = value.factions;
					value11 = value.onHandItem;
					value12 = value.attachingActorIDs;
					value13 = value.attachedActorID;
					value14 = value.activatedAuraMasterIDs;
					value15 = value.inventories;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<ActorType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								text = reader.ReadString();
								if (memberCount != 4)
								{
									reader.ReadPackable(ref value5);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<long>(out value6);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<ReasonOfSpawn>(out value7);
											if (memberCount != 7)
											{
												reader.ReadUnmanaged<VCreatureLifeCycle>(out value8);
												if (memberCount != 8)
												{
													reader.ReadPackable(ref value9);
													if (memberCount != 9)
													{
														reader.ReadValue(ref value10);
														if (memberCount != 10)
														{
															reader.ReadPackable(ref value11);
															if (memberCount != 11)
															{
																reader.ReadValue(ref value12);
																if (memberCount != 12)
																{
																	reader.ReadUnmanaged<int>(out value13);
																	if (memberCount != 13)
																	{
																		reader.ReadValue(ref value14);
																		if (memberCount != 14)
																		{
																			reader.ReadValue(ref value15);
																			_ = 15;
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
					goto IL_0324;
				}
			}
			value = new OtherCreatureInfo
			{
				actorType = value2,
				actorID = value3,
				masterID = value4,
				actorName = text,
				position = value5,
				UID = value6,
				reasonOfSpawn = value7,
				actorLifeCycle = value8,
				statInfoCollection = value9,
				factions = value10,
				onHandItem = value11,
				attachingActorIDs = value12,
				attachedActorID = value13,
				activatedAuraMasterIDs = value14,
				inventories = value15
			};
			return;
			IL_0324:
			value.actorType = value2;
			value.actorID = value3;
			value.masterID = value4;
			value.actorName = text;
			value.position = value5;
			value.UID = value6;
			value.reasonOfSpawn = value7;
			value.actorLifeCycle = value8;
			value.statInfoCollection = value9;
			value.factions = value10;
			value.onHandItem = value11;
			value.attachingActorIDs = value12;
			value.attachedActorID = value13;
			value.activatedAuraMasterIDs = value14;
			value.inventories = value15;
		}
	}
}
