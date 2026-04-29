using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class BuyItemRes : IResMsg, IMemoryPackable<BuyItemRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class BuyItemResFormatter : MemoryPackFormatter<BuyItemRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BuyItemRes value)
		{
			BuyItemRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref BuyItemRes value)
		{
			BuyItemRes.Deserialize(ref reader, ref value);
		}
	}

	public int remainCurrency { get; set; }

	[MemoryPackConstructor]
	public BuyItemRes(int hashCode)
		: base(MsgType.C2S_BuyItemRes, hashCode)
	{
		base.reliable = true;
	}

	public BuyItemRes()
		: this(0)
	{
	}

	static BuyItemRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<BuyItemRes>())
		{
			MemoryPackFormatterProvider.Register(new BuyItemResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<BuyItemRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<BuyItemRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BuyItemRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode, int>(4, value.msgType, value.hashCode, value.errorCode, value.remainCurrency);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref BuyItemRes? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		MsgErrorCode value4;
		int value5;
		if (memberCount == 4)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode, int>(out value2, out value3, out value4, out value5);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.remainCurrency;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadUnmanaged<int>(out value5);
			}
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BuyItemRes), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.remainCurrency;
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
							reader.ReadUnmanaged<int>(out value5);
							_ = 4;
						}
					}
				}
			}
			_ = value;
		}
		value = new BuyItemRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			remainCurrency = value5
		};
	}
}
