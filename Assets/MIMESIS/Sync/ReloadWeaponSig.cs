using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ReloadWeaponSig : IActorMsg, IMemoryPackable<ReloadWeaponSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ReloadWeaponSigFormatter : MemoryPackFormatter<ReloadWeaponSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReloadWeaponSig value)
		{
			ReloadWeaponSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ReloadWeaponSig value)
		{
			ReloadWeaponSig.Deserialize(ref reader, ref value);
		}
	}

	public ItemInfo currentHandEquipmentInfo { get; set; } = new ItemInfo();

	public Dictionary<int, ItemInfo> inventoryInfos { get; set; } = new Dictionary<int, ItemInfo>();

	public int currentInvenSlotIndex { get; set; }

	public ReloadWeaponSig()
		: base(MsgType.C2S_ReloadWeaponSig)
	{
		base.reliable = true;
	}

	static ReloadWeaponSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ReloadWeaponSig>())
		{
			MemoryPackFormatterProvider.Register(new ReloadWeaponSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ReloadWeaponSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ReloadWeaponSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, ItemInfo>>())
		{
			MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, ItemInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReloadWeaponSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(6, value.msgType, value.hashCode, value.actorID);
		writer.WritePackable<ItemInfo>(value.currentHandEquipmentInfo);
		writer.WriteValue<Dictionary<int, ItemInfo>>(value.inventoryInfos);
		writer.WriteUnmanaged<int>(value.currentInvenSlotIndex);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ReloadWeaponSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		ItemInfo value5;
		Dictionary<int, ItemInfo> value6;
		int value7;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.currentHandEquipmentInfo;
				value6 = value.inventoryInfos;
				value7 = value.currentInvenSlotIndex;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadPackable(ref value5);
				reader.ReadValue(ref value6);
				reader.ReadUnmanaged<int>(out value7);
				goto IL_0157;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadPackable<ItemInfo>();
			value6 = reader.ReadValue<Dictionary<int, ItemInfo>>();
			reader.ReadUnmanaged<int>(out value7);
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ReloadWeaponSig), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = null;
				value6 = null;
				value7 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.currentHandEquipmentInfo;
				value6 = value.inventoryInfos;
				value7 = value.currentInvenSlotIndex;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<int>(out value4);
						if (memberCount != 3)
						{
							reader.ReadPackable(ref value5);
							if (memberCount != 4)
							{
								reader.ReadValue(ref value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<int>(out value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0157;
			}
		}
		value = new ReloadWeaponSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			currentHandEquipmentInfo = value5,
			inventoryInfos = value6,
			currentInvenSlotIndex = value7
		};
		return;
		IL_0157:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.currentHandEquipmentInfo = value5;
		value.inventoryInfos = value6;
		value.currentInvenSlotIndex = value7;
	}
}
