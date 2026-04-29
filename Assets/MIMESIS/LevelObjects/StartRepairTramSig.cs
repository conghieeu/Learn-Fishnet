using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class StartRepairTramSig : IMsg, IMemoryPackable<StartRepairTramSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class StartRepairTramSigFormatter : MemoryPackFormatter<StartRepairTramSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartRepairTramSig value)
		{
			StartRepairTramSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref StartRepairTramSig value)
		{
			StartRepairTramSig.Deserialize(ref reader, ref value);
		}
	}

	public int remainCurrency { get; set; }

	public StartRepairTramSig()
		: base(MsgType.C2S_StartRepairTramSig)
	{
		base.reliable = true;
	}

	static StartRepairTramSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<StartRepairTramSig>())
		{
			MemoryPackFormatterProvider.Register(new StartRepairTramSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<StartRepairTramSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<StartRepairTramSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartRepairTramSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(3, value.msgType, value.hashCode, value.remainCurrency);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref StartRepairTramSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.remainCurrency;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				goto IL_00be;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(StartRepairTramSig), 3, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.remainCurrency;
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
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00be;
			}
		}
		value = new StartRepairTramSig
		{
			msgType = value2,
			hashCode = value3,
			remainCurrency = value4
		};
		return;
		IL_00be:
		value.msgType = value2;
		value.hashCode = value3;
		value.remainCurrency = value4;
	}
}
