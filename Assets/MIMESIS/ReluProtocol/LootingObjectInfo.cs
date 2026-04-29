using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class LootingObjectInfo : ActorBaseInfo, IMemoryPackable<LootingObjectInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class LootingObjectInfoFormatter : MemoryPackFormatter<LootingObjectInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LootingObjectInfo value)
			{
				LootingObjectInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref LootingObjectInfo value)
			{
				LootingObjectInfo.Deserialize(ref reader, ref value);
			}
		}

		public ItemInfo linkedItemInfo { get; set; } = new ItemInfo();

		static LootingObjectInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<LootingObjectInfo>())
			{
				MemoryPackFormatterProvider.Register(new LootingObjectInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<LootingObjectInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<LootingObjectInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ActorType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ActorType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReasonOfSpawn>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ReasonOfSpawn>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LootingObjectInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<ActorType, int, int>(8, value.actorType, value.actorID, value.masterID);
			writer.WriteString(value.actorName);
			writer.WritePackable<PosWithRot>(value.position);
			writer.WriteUnmanaged<long, ReasonOfSpawn>(value.UID, value.reasonOfSpawn);
			writer.WritePackable<ItemInfo>(value.linkedItemInfo);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref LootingObjectInfo? value)
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
			ItemInfo value8;
			string text;
			if (memberCount == 8)
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
					value8 = value.linkedItemInfo;
					reader.ReadUnmanaged<ActorType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					text = reader.ReadString();
					reader.ReadPackable(ref value5);
					reader.ReadUnmanaged<long>(out value6);
					reader.ReadUnmanaged<ReasonOfSpawn>(out value7);
					reader.ReadPackable(ref value8);
					goto IL_01b4;
				}
				reader.ReadUnmanaged<ActorType, int, int>(out value2, out value3, out value4);
				text = reader.ReadString();
				value5 = reader.ReadPackable<PosWithRot>();
				reader.ReadUnmanaged<long, ReasonOfSpawn>(out value6, out value7);
				value8 = reader.ReadPackable<ItemInfo>();
			}
			else
			{
				if (memberCount > 8)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(LootingObjectInfo), 8, memberCount);
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
					value8 = null;
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
					value8 = value.linkedItemInfo;
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
												reader.ReadPackable(ref value8);
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
					goto IL_01b4;
				}
			}
			value = new LootingObjectInfo
			{
				actorType = value2,
				actorID = value3,
				masterID = value4,
				actorName = text,
				position = value5,
				UID = value6,
				reasonOfSpawn = value7,
				linkedItemInfo = value8
			};
			return;
			IL_01b4:
			value.actorType = value2;
			value.actorID = value3;
			value.masterID = value4;
			value.actorName = text;
			value.position = value5;
			value.UID = value6;
			value.reasonOfSpawn = value7;
			value.linkedItemInfo = value8;
		}
	}
}
