using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ChangeActiveInvenSlotRes : IResMsg, IMemoryPackable<ChangeActiveInvenSlotRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ChangeActiveInvenSlotResFormatter : MemoryPackFormatter<ChangeActiveInvenSlotRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeActiveInvenSlotRes value)
		{
			ChangeActiveInvenSlotRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ChangeActiveInvenSlotRes value)
		{
			ChangeActiveInvenSlotRes.Deserialize(ref reader, ref value);
		}
	}

	public ItemInfo currentHandEquipmentInfo { get; set; } = new ItemInfo();

	public int currentInvenSlotIndex { get; set; }

	[MemoryPackConstructor]
	public ChangeActiveInvenSlotRes(int hashCode)
		: base(MsgType.C2S_ChangeActiveInvenSlotRes, hashCode)
	{
		base.reliable = true;
	}

	public ChangeActiveInvenSlotRes()
		: this(0)
	{
	}

	static ChangeActiveInvenSlotRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeActiveInvenSlotRes>())
		{
			MemoryPackFormatterProvider.Register(new ChangeActiveInvenSlotResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeActiveInvenSlotRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ChangeActiveInvenSlotRes>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgErrorCode>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgErrorCode>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeActiveInvenSlotRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(5, value.msgType, value.hashCode, value.errorCode);
		writer.WritePackable<ItemInfo>(value.currentHandEquipmentInfo);
		writer.WriteUnmanaged<int>(value.currentInvenSlotIndex);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ChangeActiveInvenSlotRes? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		MsgErrorCode value4;
		ItemInfo value5;
		int value6;
		if (memberCount == 5)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
				value5 = reader.ReadPackable<ItemInfo>();
				reader.ReadUnmanaged<int>(out value6);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.currentHandEquipmentInfo;
				value6 = value.currentInvenSlotIndex;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadPackable(ref value5);
				reader.ReadUnmanaged<int>(out value6);
			}
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ChangeActiveInvenSlotRes), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = null;
				value6 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.currentHandEquipmentInfo;
				value6 = value.currentInvenSlotIndex;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<MsgErrorCode>(out value4);
						if (memberCount != 3)
						{
							reader.ReadPackable(ref value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<int>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			_ = value;
		}
		value = new ChangeActiveInvenSlotRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			currentHandEquipmentInfo = value5,
			currentInvenSlotIndex = value6
		};
	}
}
