using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class EndScrapMotionReq : IMsg, IMemoryPackable<EndScrapMotionReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class EndScrapMotionReqFormatter : MemoryPackFormatter<EndScrapMotionReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EndScrapMotionReq value)
		{
			EndScrapMotionReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref EndScrapMotionReq value)
		{
			EndScrapMotionReq.Deserialize(ref reader, ref value);
		}
	}

	public PosWithRot basePosition { get; set; } = new PosWithRot();

	public EndScrapMotionReq()
		: base(MsgType.C2S_EndScrapMotionReq)
	{
		base.reliable = true;
	}

	static EndScrapMotionReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<EndScrapMotionReq>())
		{
			MemoryPackFormatterProvider.Register(new EndScrapMotionReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<EndScrapMotionReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<EndScrapMotionReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EndScrapMotionReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
		writer.WritePackable<PosWithRot>(value.basePosition);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref EndScrapMotionReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		PosWithRot value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.basePosition;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadPackable(ref value4);
				goto IL_00c3;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			value4 = reader.ReadPackable<PosWithRot>();
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EndScrapMotionReq), 3, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.basePosition;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadPackable(ref value4);
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00c3;
			}
		}
		value = new EndScrapMotionReq
		{
			msgType = value2,
			hashCode = value3,
			basePosition = value4
		};
		return;
		IL_00c3:
		value.msgType = value2;
		value.hashCode = value3;
		value.basePosition = value4;
	}
}
