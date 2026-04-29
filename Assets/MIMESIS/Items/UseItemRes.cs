using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class UseItemRes : IResMsg, IMemoryPackable<UseItemRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class UseItemResFormatter : MemoryPackFormatter<UseItemRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseItemRes value)
		{
			UseItemRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref UseItemRes value)
		{
			UseItemRes.Deserialize(ref reader, ref value);
		}
	}

	[MemoryPackConstructor]
	public UseItemRes(int hashCode)
		: base(MsgType.C2S_UseItemRes, hashCode)
	{
		base.reliable = true;
	}

	public UseItemRes()
		: this(0)
	{
	}

	static UseItemRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<UseItemRes>())
		{
			MemoryPackFormatterProvider.Register(new UseItemResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<UseItemRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<UseItemRes>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseItemRes? value) where TBufferWriter : class, IBufferWriter<byte>
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
	public static void Deserialize(ref MemoryPackReader reader, ref UseItemRes? value)
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
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(UseItemRes), 3, memberCount);
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
		value = new UseItemRes(value3)
		{
			msgType = value2,
			errorCode = value4
		};
	}
}
