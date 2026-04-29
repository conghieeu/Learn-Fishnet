using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class AnnounceMutableStatSig : IActorMsg, IMemoryPackable<AnnounceMutableStatSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class AnnounceMutableStatSigFormatter : MemoryPackFormatter<AnnounceMutableStatSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AnnounceMutableStatSig value)
		{
			AnnounceMutableStatSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref AnnounceMutableStatSig value)
		{
			AnnounceMutableStatSig.Deserialize(ref reader, ref value);
		}
	}

	public MutableStatType type { get; set; }

	public long remainValue { get; set; }

	public long maxValue { get; set; }

	public AnnounceMutableStatSig()
		: base(MsgType.C2S_AnnounceMutableStatSig)
	{
	}

	static AnnounceMutableStatSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<AnnounceMutableStatSig>())
		{
			MemoryPackFormatterProvider.Register(new AnnounceMutableStatSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<AnnounceMutableStatSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<AnnounceMutableStatSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MutableStatType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MutableStatType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AnnounceMutableStatSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, MutableStatType, long, long>(6, value.msgType, value.hashCode, value.actorID, value.type, value.remainValue, value.maxValue);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref AnnounceMutableStatSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		MutableStatType value5;
		long value6;
		long value7;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.type;
				value6 = value.remainValue;
				value7 = value.maxValue;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<MutableStatType>(out value5);
				reader.ReadUnmanaged<long>(out value6);
				reader.ReadUnmanaged<long>(out value7);
				goto IL_0147;
			}
			reader.ReadUnmanaged<MsgType, int, int, MutableStatType, long, long>(out value2, out value3, out value4, out value5, out value6, out value7);
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AnnounceMutableStatSig), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = MutableStatType.Invalid;
				value6 = 0L;
				value7 = 0L;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.type;
				value6 = value.remainValue;
				value7 = value.maxValue;
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
							reader.ReadUnmanaged<MutableStatType>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<long>(out value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<long>(out value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0147;
			}
		}
		value = new AnnounceMutableStatSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			type = value5,
			remainValue = value6,
			maxValue = value7
		};
		return;
		IL_0147:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.type = value5;
		value.remainValue = value6;
		value.maxValue = value7;
	}
}
