using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ChargeItemRes : IResMsg, IMemoryPackable<ChargeItemRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ChargeItemResFormatter : MemoryPackFormatter<ChargeItemRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChargeItemRes value)
		{
			ChargeItemRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ChargeItemRes value)
		{
			ChargeItemRes.Deserialize(ref reader, ref value);
		}
	}

	public ItemInfo currentHandEquipmentInfo { get; set; } = new ItemInfo();

	[MemoryPackConstructor]
	public ChargeItemRes(int hashCode)
		: base(MsgType.C2S_ChargeItemRes, hashCode)
	{
		base.reliable = true;
	}

	public ChargeItemRes()
		: this(0)
	{
	}

	static ChargeItemRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ChargeItemRes>())
		{
			MemoryPackFormatterProvider.Register(new ChargeItemResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ChargeItemRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ChargeItemRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChargeItemRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(4, value.msgType, value.hashCode, value.errorCode);
		writer.WritePackable<ItemInfo>(value.currentHandEquipmentInfo);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ChargeItemRes? value)
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
		if (memberCount == 4)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
				value5 = reader.ReadPackable<ItemInfo>();
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.currentHandEquipmentInfo;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadPackable(ref value5);
			}
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ChargeItemRes), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.currentHandEquipmentInfo;
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
							_ = 4;
						}
					}
				}
			}
			_ = value;
		}
		value = new ChargeItemRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			currentHandEquipmentInfo = value5
		};
	}
}
