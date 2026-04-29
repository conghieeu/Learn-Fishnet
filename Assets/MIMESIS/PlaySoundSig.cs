using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class PlaySoundSig : IMsg, IMemoryPackable<PlaySoundSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class PlaySoundSigFormatter : MemoryPackFormatter<PlaySoundSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlaySoundSig value)
		{
			PlaySoundSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref PlaySoundSig value)
		{
			PlaySoundSig.Deserialize(ref reader, ref value);
		}
	}

	public string soundClipID { get; set; }

	public Vector3 pos { get; set; }

	public PlaySoundSig()
		: base(MsgType.C2S_PlaySoundSig)
	{
	}

	static PlaySoundSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<PlaySoundSig>())
		{
			MemoryPackFormatterProvider.Register(new PlaySoundSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<PlaySoundSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<PlaySoundSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlaySoundSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(4, value.msgType, value.hashCode);
		writer.WriteString(value.soundClipID);
		writer.WriteUnmanaged<Vector3>(value.pos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref PlaySoundSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		Vector3 value4;
		string text;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				text = value.soundClipID;
				value4 = value.pos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				text = reader.ReadString();
				reader.ReadUnmanaged<Vector3>(out value4);
				goto IL_00fa;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			text = reader.ReadString();
			reader.ReadUnmanaged<Vector3>(out value4);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PlaySoundSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				text = null;
				value4 = default(Vector3);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				text = value.soundClipID;
				value4 = value.pos;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						text = reader.ReadString();
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<Vector3>(out value4);
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00fa;
			}
		}
		value = new PlaySoundSig
		{
			msgType = value2,
			hashCode = value3,
			soundClipID = text,
			pos = value4
		};
		return;
		IL_00fa:
		value.msgType = value2;
		value.hashCode = value3;
		value.soundClipID = text;
		value.pos = value4;
	}
}
