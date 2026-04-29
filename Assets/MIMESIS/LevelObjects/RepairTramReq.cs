using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class RepairTramReq : IMsg, IMemoryPackable<RepairTramReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class RepairTramReqFormatter : MemoryPackFormatter<RepairTramReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RepairTramReq value)
		{
			RepairTramReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref RepairTramReq value)
		{
			RepairTramReq.Deserialize(ref reader, ref value);
		}
	}

	public RepairTramReq()
		: base(MsgType.C2S_RepairTramReq)
	{
		base.reliable = true;
	}

	static RepairTramReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<RepairTramReq>())
		{
			MemoryPackFormatterProvider.Register(new RepairTramReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<RepairTramReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<RepairTramReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RepairTramReq? value) where TBufferWriter : class, IBufferWriter<byte>
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
	public static void Deserialize(ref MemoryPackReader reader, ref RepairTramReq? value)
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
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RepairTramReq), 2, memberCount);
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
		value = new RepairTramReq
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
