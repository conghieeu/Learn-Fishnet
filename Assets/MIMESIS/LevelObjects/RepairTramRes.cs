using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class RepairTramRes : IResMsg, IMemoryPackable<RepairTramRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class RepairTramResFormatter : MemoryPackFormatter<RepairTramRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RepairTramRes value)
		{
			RepairTramRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref RepairTramRes value)
		{
			RepairTramRes.Deserialize(ref reader, ref value);
		}
	}

	[MemoryPackConstructor]
	public RepairTramRes(int hashCode)
		: base(MsgType.C2S_RepairTramRes, hashCode)
	{
		base.reliable = true;
	}

	public RepairTramRes()
		: this(0)
	{
	}

	static RepairTramRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<RepairTramRes>())
		{
			MemoryPackFormatterProvider.Register(new RepairTramResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<RepairTramRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<RepairTramRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RepairTramRes? value) where TBufferWriter : class, IBufferWriter<byte>
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
	public static void Deserialize(ref MemoryPackReader reader, ref RepairTramRes? value)
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
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RepairTramRes), 3, memberCount);
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
		value = new RepairTramRes(value3)
		{
			msgType = value2,
			errorCode = value4
		};
	}
}
