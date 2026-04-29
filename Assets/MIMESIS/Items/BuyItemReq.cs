using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class BuyItemReq : IMsg, IMemoryPackable<BuyItemReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class BuyItemReqFormatter : MemoryPackFormatter<BuyItemReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BuyItemReq value)
		{
			BuyItemReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref BuyItemReq value)
		{
			BuyItemReq.Deserialize(ref reader, ref value);
		}
	}

	public int itemMasterID { get; set; }

	public int machineIndex { get; set; }

	public BuyItemReq()
		: base(MsgType.C2S_BuyItemReq)
	{
		base.reliable = true;
	}

	static BuyItemReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<BuyItemReq>())
		{
			MemoryPackFormatterProvider.Register(new BuyItemReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<BuyItemReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<BuyItemReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BuyItemReq? value) where TBufferWriter : class, IBufferWriter<byte>
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
	public static void Deserialize(ref MemoryPackReader reader, ref BuyItemReq? value)
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
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BuyItemReq), 4, memberCount);
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
		value = new BuyItemReq
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
