using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class APIEnterTramLogRes : IResMsg, IMemoryPackable<APIEnterTramLogRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class APIEnterTramLogResFormatter : MemoryPackFormatter<APIEnterTramLogRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref APIEnterTramLogRes value)
		{
			APIEnterTramLogRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref APIEnterTramLogRes value)
		{
			APIEnterTramLogRes.Deserialize(ref reader, ref value);
		}
	}

	[MemoryPackConstructor]
	public APIEnterTramLogRes(int hashCode)
		: base(MsgType.C2A_EnterTramLogRes, hashCode)
	{
	}

	public APIEnterTramLogRes()
		: this(0)
	{
	}

	static APIEnterTramLogRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<APIEnterTramLogRes>())
		{
			MemoryPackFormatterProvider.Register(new APIEnterTramLogResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<APIEnterTramLogRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<APIEnterTramLogRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref APIEnterTramLogRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(3, value.msgType, value.hashCode, value.errorCode);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref APIEnterTramLogRes? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		MsgErrorCode value4;
		if (memberCount == 3)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
			}
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(APIEnterTramLogRes), 3, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
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
						_ = 3;
					}
				}
			}
			_ = value;
		}
		value = new APIEnterTramLogRes(value3)
		{
			msgType = value2,
			errorCode = value4
		};
	}
}
