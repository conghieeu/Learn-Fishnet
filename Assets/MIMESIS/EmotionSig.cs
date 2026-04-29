using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class EmotionSig : IActorMsg, IMemoryPackable<EmotionSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class EmotionSigFormatter : MemoryPackFormatter<EmotionSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EmotionSig value)
		{
			EmotionSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref EmotionSig value)
		{
			EmotionSig.Deserialize(ref reader, ref value);
		}
	}

	public int emotionMasterID { get; set; }

	public PosWithRot basePosition { get; set; } = new PosWithRot();

	public EmotionSig()
		: base(MsgType.C2S_EmotionSig)
	{
		base.reliable = true;
	}

	static EmotionSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<EmotionSig>())
		{
			MemoryPackFormatterProvider.Register(new EmotionSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<EmotionSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<EmotionSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EmotionSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int>(5, value.msgType, value.hashCode, value.actorID, value.emotionMasterID);
		writer.WritePackable<PosWithRot>(value.basePosition);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref EmotionSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		int value5;
		PosWithRot value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.emotionMasterID;
				value6 = value.basePosition;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadPackable(ref value6);
				goto IL_011d;
			}
			reader.ReadUnmanaged<MsgType, int, int, int>(out value2, out value3, out value4, out value5);
			value6 = reader.ReadPackable<PosWithRot>();
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EmotionSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.emotionMasterID;
				value6 = value.basePosition;
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
							reader.ReadUnmanaged<int>(out value5);
							if (memberCount != 4)
							{
								reader.ReadPackable(ref value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_011d;
			}
		}
		value = new EmotionSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			emotionMasterID = value5,
			basePosition = value6
		};
		return;
		IL_011d:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.emotionMasterID = value5;
		value.basePosition = value6;
	}
}
