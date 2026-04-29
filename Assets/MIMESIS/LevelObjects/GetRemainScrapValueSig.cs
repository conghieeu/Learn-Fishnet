using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class GetRemainScrapValueSig : IMsg, IMemoryPackable<GetRemainScrapValueSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class GetRemainScrapValueSigFormatter : MemoryPackFormatter<GetRemainScrapValueSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GetRemainScrapValueSig value)
		{
			GetRemainScrapValueSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref GetRemainScrapValueSig value)
		{
			GetRemainScrapValueSig.Deserialize(ref reader, ref value);
		}
	}

	public int remainValue { get; set; }

	public GetRemainScrapValueSig()
		: base(MsgType.C2S_GetRemainScrapValueSig)
	{
		base.reliable = true;
	}

	static GetRemainScrapValueSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<GetRemainScrapValueSig>())
		{
			MemoryPackFormatterProvider.Register(new GetRemainScrapValueSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<GetRemainScrapValueSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<GetRemainScrapValueSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GetRemainScrapValueSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(3, value.msgType, value.hashCode, value.remainValue);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref GetRemainScrapValueSig? value)
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
				value4 = value.remainValue;
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
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(GetRemainScrapValueSig), 3, memberCount);
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
				value4 = value.remainValue;
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
		value = new GetRemainScrapValueSig
		{
			msgType = value2,
			hashCode = value3,
			remainValue = value4
		};
		return;
		IL_00be:
		value.msgType = value2;
		value.hashCode = value3;
		value.remainValue = value4;
	}
}
