using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class JumpReq : IMsg, IMemoryPackable<JumpReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class JumpReqFormatter : MemoryPackFormatter<JumpReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref JumpReq value)
		{
			JumpReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref JumpReq value)
		{
			JumpReq.Deserialize(ref reader, ref value);
		}
	}

	public JumpReq()
		: base(MsgType.C2S_JumpReq)
	{
	}

	static JumpReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<JumpReq>())
		{
			MemoryPackFormatterProvider.Register(new JumpReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<JumpReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<JumpReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref JumpReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int>(2, value.msgType, value.hashCode);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref JumpReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		if (memberCount == 2)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				goto IL_0096;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
		}
		else
		{
			if (memberCount > 2)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(JumpReq), 2, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					_ = 2;
				}
			}
			if (value != null)
			{
				goto IL_0096;
			}
		}
		value = new JumpReq
		{
			msgType = value2,
			hashCode = value3
		};
		return;
		IL_0096:
		value.msgType = value2;
		value.hashCode = value3;
	}
}
