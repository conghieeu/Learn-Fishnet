using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ChangeActiveInvenSlotReq : IMsg, IMemoryPackable<ChangeActiveInvenSlotReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ChangeActiveInvenSlotReqFormatter : MemoryPackFormatter<ChangeActiveInvenSlotReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeActiveInvenSlotReq value)
		{
			ChangeActiveInvenSlotReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ChangeActiveInvenSlotReq value)
		{
			ChangeActiveInvenSlotReq.Deserialize(ref reader, ref value);
		}
	}

	public int slotIndex { get; set; }

	public ChangeActiveInvenSlotReq()
		: base(MsgType.C2S_ChangeActiveInvenSlotReq)
	{
		base.reliable = true;
	}

	static ChangeActiveInvenSlotReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeActiveInvenSlotReq>())
		{
			MemoryPackFormatterProvider.Register(new ChangeActiveInvenSlotReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeActiveInvenSlotReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ChangeActiveInvenSlotReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeActiveInvenSlotReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(3, value.msgType, value.hashCode, value.slotIndex);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ChangeActiveInvenSlotReq? value)
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
				value4 = value.slotIndex;
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
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ChangeActiveInvenSlotReq), 3, memberCount);
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
				value4 = value.slotIndex;
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
		value = new ChangeActiveInvenSlotReq
		{
			msgType = value2,
			hashCode = value3,
			slotIndex = value4
		};
		return;
		IL_00be:
		value.msgType = value2;
		value.hashCode = value3;
		value.slotIndex = value4;
	}
}
