using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class PutIntoToiletReq : IMsg, IMemoryPackable<PutIntoToiletReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class PutIntoToiletReqFormatter : MemoryPackFormatter<PutIntoToiletReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PutIntoToiletReq value)
		{
			PutIntoToiletReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref PutIntoToiletReq value)
		{
			PutIntoToiletReq.Deserialize(ref reader, ref value);
		}
	}

	public PutIntoToiletReq()
		: base(MsgType.C2S_PutIntoToiletReq)
	{
		base.reliable = true;
	}

	static PutIntoToiletReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<PutIntoToiletReq>())
		{
			MemoryPackFormatterProvider.Register(new PutIntoToiletReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<PutIntoToiletReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<PutIntoToiletReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PutIntoToiletReq? value) where TBufferWriter : class, IBufferWriter<byte>
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
	public static void Deserialize(ref MemoryPackReader reader, ref PutIntoToiletReq? value)
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
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PutIntoToiletReq), 2, memberCount);
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
		value = new PutIntoToiletReq
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
