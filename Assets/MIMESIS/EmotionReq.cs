using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class EmotionReq : IMsg, IMemoryPackable<EmotionReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class EmotionReqFormatter : MemoryPackFormatter<EmotionReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EmotionReq value)
		{
			EmotionReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref EmotionReq value)
		{
			EmotionReq.Deserialize(ref reader, ref value);
		}
	}

	public int emotionMasterID { get; set; }

	public PosWithRot basePosition { get; set; } = new PosWithRot();

	public EmotionReq()
		: base(MsgType.C2S_EmotionReq)
	{
	}

	static EmotionReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<EmotionReq>())
		{
			MemoryPackFormatterProvider.Register(new EmotionReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<EmotionReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<EmotionReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EmotionReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.emotionMasterID);
		writer.WritePackable<PosWithRot>(value.basePosition);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref EmotionReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		PosWithRot value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.emotionMasterID;
				value5 = value.basePosition;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadPackable(ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadPackable<PosWithRot>();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EmotionReq), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.emotionMasterID;
				value5 = value.basePosition;
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
						if (memberCount != 3)
						{
							reader.ReadPackable(ref value5);
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00ef;
			}
		}
		value = new EmotionReq
		{
			msgType = value2,
			hashCode = value3,
			emotionMasterID = value4,
			basePosition = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.emotionMasterID = value4;
		value.basePosition = value5;
	}
}
