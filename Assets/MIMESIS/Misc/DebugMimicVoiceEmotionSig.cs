using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class DebugMimicVoiceEmotionSig : IMsg, IMemoryPackable<DebugMimicVoiceEmotionSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class DebugMimicVoiceEmotionSigFormatter : MemoryPackFormatter<DebugMimicVoiceEmotionSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugMimicVoiceEmotionSig value)
		{
			DebugMimicVoiceEmotionSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DebugMimicVoiceEmotionSig value)
		{
			DebugMimicVoiceEmotionSig.Deserialize(ref reader, ref value);
		}
	}

	public int ActorID { get; set; }

	public string ActorName { get; set; }

	public byte EmotionData { get; set; }

	public DebugMimicVoiceEmotionSig()
		: base(MsgType.C2S_DebugMimicVoiceEmotionSig)
	{
	}

	static DebugMimicVoiceEmotionSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DebugMimicVoiceEmotionSig>())
		{
			MemoryPackFormatterProvider.Register(new DebugMimicVoiceEmotionSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DebugMimicVoiceEmotionSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<DebugMimicVoiceEmotionSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugMimicVoiceEmotionSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(5, value.msgType, value.hashCode, value.ActorID);
		writer.WriteString(value.ActorName);
		writer.WriteUnmanaged<byte>(value.EmotionData);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DebugMimicVoiceEmotionSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		byte value5;
		string actorName;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.ActorID;
				actorName = value.ActorName;
				value5 = value.EmotionData;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				actorName = reader.ReadString();
				reader.ReadUnmanaged<byte>(out value5);
				goto IL_0123;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			actorName = reader.ReadString();
			reader.ReadUnmanaged<byte>(out value5);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DebugMimicVoiceEmotionSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				actorName = null;
				value5 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.ActorID;
				actorName = value.ActorName;
				value5 = value.EmotionData;
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
							actorName = reader.ReadString();
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<byte>(out value5);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0123;
			}
		}
		value = new DebugMimicVoiceEmotionSig
		{
			msgType = value2,
			hashCode = value3,
			ActorID = value4,
			ActorName = actorName,
			EmotionData = value5
		};
		return;
		IL_0123:
		value.msgType = value2;
		value.hashCode = value3;
		value.ActorID = value4;
		value.ActorName = actorName;
		value.EmotionData = value5;
	}
}
