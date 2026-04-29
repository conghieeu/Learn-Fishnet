using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class DebugMimicVoiceInfoSig : IActorMsg, IMemoryPackable<DebugMimicVoiceInfoSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class DebugMimicVoiceInfoSigFormatter : MemoryPackFormatter<DebugMimicVoiceInfoSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugMimicVoiceInfoSig value)
		{
			DebugMimicVoiceInfoSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DebugMimicVoiceInfoSig value)
		{
			DebugMimicVoiceInfoSig.Deserialize(ref reader, ref value);
		}
	}

	public string VoicePickReason { get; set; }

	public DebugMimicVoiceInfoSig()
		: base(MsgType.C2S_DebugMimicVoiceInfoSig)
	{
	}

	static DebugMimicVoiceInfoSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DebugMimicVoiceInfoSig>())
		{
			MemoryPackFormatterProvider.Register(new DebugMimicVoiceInfoSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DebugMimicVoiceInfoSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<DebugMimicVoiceInfoSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugMimicVoiceInfoSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.actorID);
		writer.WriteString(value.VoicePickReason);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DebugMimicVoiceInfoSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		string voicePickReason;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				voicePickReason = value.VoicePickReason;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				voicePickReason = reader.ReadString();
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			voicePickReason = reader.ReadString();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DebugMimicVoiceInfoSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				voicePickReason = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				voicePickReason = value.VoicePickReason;
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
							voicePickReason = reader.ReadString();
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
		value = new DebugMimicVoiceInfoSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			VoicePickReason = voicePickReason
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.VoicePickReason = voicePickReason;
	}
}
