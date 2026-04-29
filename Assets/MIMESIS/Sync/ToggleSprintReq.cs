using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ToggleSprintReq : IMsg, IMemoryPackable<ToggleSprintReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ToggleSprintReqFormatter : MemoryPackFormatter<ToggleSprintReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ToggleSprintReq value)
		{
			ToggleSprintReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ToggleSprintReq value)
		{
			ToggleSprintReq.Deserialize(ref reader, ref value);
		}
	}

	public bool isSprint { get; set; }

	public ToggleSprintReq()
		: base(MsgType.C2S_ToggleSprintReq)
	{
	}

	static ToggleSprintReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ToggleSprintReq>())
		{
			MemoryPackFormatterProvider.Register(new ToggleSprintReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ToggleSprintReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ToggleSprintReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ToggleSprintReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, bool>(3, value.msgType, value.hashCode, value.isSprint);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ToggleSprintReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		bool value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.isSprint;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<bool>(out value4);
				goto IL_00be;
			}
			reader.ReadUnmanaged<MsgType, int, bool>(out value2, out value3, out value4);
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ToggleSprintReq), 3, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = false;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.isSprint;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<bool>(out value4);
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00be;
			}
		}
		value = new ToggleSprintReq
		{
			msgType = value2,
			hashCode = value3,
			isSprint = value4
		};
		return;
		IL_00be:
		value.msgType = value2;
		value.hashCode = value3;
		value.isSprint = value4;
	}
}
