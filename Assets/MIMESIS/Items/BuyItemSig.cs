using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class BuyItemSig : IMsg, IMemoryPackable<BuyItemSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class BuyItemSigFormatter : MemoryPackFormatter<BuyItemSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BuyItemSig value)
		{
			BuyItemSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref BuyItemSig value)
		{
			BuyItemSig.Deserialize(ref reader, ref value);
		}
	}

	public int itemMasterID { get; set; }

	public int machineIndex { get; set; }

	public BuyItemSig()
		: base(MsgType.C2S_BuyItemSig)
	{
		base.reliable = true;
	}

	static BuyItemSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<BuyItemSig>())
		{
			MemoryPackFormatterProvider.Register(new BuyItemSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<BuyItemSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<BuyItemSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BuyItemSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int>(4, value.msgType, value.hashCode, value.itemMasterID, value.machineIndex);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref BuyItemSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		int value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.itemMasterID;
				value5 = value.machineIndex;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				goto IL_00e9;
			}
			reader.ReadUnmanaged<MsgType, int, int, int>(out value2, out value3, out value4, out value5);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BuyItemSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.itemMasterID;
				value5 = value.machineIndex;
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
							reader.ReadUnmanaged<int>(out value5);
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00e9;
			}
		}
		value = new BuyItemSig
		{
			msgType = value2,
			hashCode = value3,
			itemMasterID = value4,
			machineIndex = value5
		};
		return;
		IL_00e9:
		value.msgType = value2;
		value.hashCode = value3;
		value.itemMasterID = value4;
		value.machineIndex = value5;
	}
}
