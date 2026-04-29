using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ReturnMembertoMaintenenceRoomSig : IMsg, IMemoryPackable<ReturnMembertoMaintenenceRoomSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ReturnMembertoMaintenenceRoomSigFormatter : MemoryPackFormatter<ReturnMembertoMaintenenceRoomSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReturnMembertoMaintenenceRoomSig value)
		{
			ReturnMembertoMaintenenceRoomSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ReturnMembertoMaintenenceRoomSig value)
		{
			ReturnMembertoMaintenenceRoomSig.Deserialize(ref reader, ref value);
		}
	}

	public Dictionary<int, int> itemPrices { get; set; } = new Dictionary<int, int>();

	public int targetCurrency { get; set; }

	public ReturnMembertoMaintenenceRoomSig()
		: base(MsgType.C2S_ReturnMembertoMaintenenceRoomSig)
	{
		base.reliable = true;
	}

	static ReturnMembertoMaintenenceRoomSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ReturnMembertoMaintenenceRoomSig>())
		{
			MemoryPackFormatterProvider.Register(new ReturnMembertoMaintenenceRoomSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ReturnMembertoMaintenenceRoomSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ReturnMembertoMaintenenceRoomSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, int>>())
		{
			MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, int>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReturnMembertoMaintenenceRoomSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(4, value.msgType, value.hashCode);
		writer.WriteValue<Dictionary<int, int>>(value.itemPrices);
		writer.WriteUnmanaged<int>(value.targetCurrency);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ReturnMembertoMaintenenceRoomSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		Dictionary<int, int> value4;
		int value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.itemPrices;
				value5 = value.targetCurrency;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadValue(ref value4);
				reader.ReadUnmanaged<int>(out value5);
				goto IL_00f4;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			value4 = reader.ReadValue<Dictionary<int, int>>();
			reader.ReadUnmanaged<int>(out value5);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ReturnMembertoMaintenenceRoomSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = null;
				value5 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.itemPrices;
				value5 = value.targetCurrency;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadValue(ref value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<int>(out value5);
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00f4;
			}
		}
		value = new ReturnMembertoMaintenenceRoomSig
		{
			msgType = value2,
			hashCode = value3,
			itemPrices = value4,
			targetCurrency = value5
		};
		return;
		IL_00f4:
		value.msgType = value2;
		value.hashCode = value3;
		value.itemPrices = value4;
		value.targetCurrency = value5;
	}
}
