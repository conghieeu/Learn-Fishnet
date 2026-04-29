using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ChangeViewPointSig : IActorMsg, IMemoryPackable<ChangeViewPointSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ChangeViewPointSigFormatter : MemoryPackFormatter<ChangeViewPointSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeViewPointSig value)
		{
			ChangeViewPointSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ChangeViewPointSig value)
		{
			ChangeViewPointSig.Deserialize(ref reader, ref value);
		}
	}

	public float pitch { get; set; }

	public float angle { get; set; }

	public ChangeViewPointSig()
		: base(MsgType.C2S_ChangeViewPointSig)
	{
	}

	static ChangeViewPointSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeViewPointSig>())
		{
			MemoryPackFormatterProvider.Register(new ChangeViewPointSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeViewPointSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ChangeViewPointSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeViewPointSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, float, float>(5, value.msgType, value.hashCode, value.actorID, value.pitch, value.angle);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ChangeViewPointSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		float value5;
		float value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.pitch;
				value6 = value.angle;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<float>(out value5);
				reader.ReadUnmanaged<float>(out value6);
				goto IL_011f;
			}
			reader.ReadUnmanaged<MsgType, int, int, float, float>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ChangeViewPointSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0f;
				value6 = 0f;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.pitch;
				value6 = value.angle;
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
							reader.ReadUnmanaged<float>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<float>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_011f;
			}
		}
		value = new ChangeViewPointSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			pitch = value5,
			angle = value6
		};
		return;
		IL_011f:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.pitch = value5;
		value.angle = value6;
	}
}
