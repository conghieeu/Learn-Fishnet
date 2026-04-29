using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class AnnounceImmutableStatSig : IActorMsg, IMemoryPackable<AnnounceImmutableStatSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class AnnounceImmutableStatSigFormatter : MemoryPackFormatter<AnnounceImmutableStatSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AnnounceImmutableStatSig value)
		{
			AnnounceImmutableStatSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref AnnounceImmutableStatSig value)
		{
			AnnounceImmutableStatSig.Deserialize(ref reader, ref value);
		}
	}

	public Dictionary<StatType, long> ImmutableStats { get; set; } = new Dictionary<StatType, long>();

	public AnnounceImmutableStatSig()
		: base(MsgType.C2S_AnnounceImmutableStatSig)
	{
	}

	static AnnounceImmutableStatSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<AnnounceImmutableStatSig>())
		{
			MemoryPackFormatterProvider.Register(new AnnounceImmutableStatSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<AnnounceImmutableStatSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<AnnounceImmutableStatSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<StatType, long>>())
		{
			MemoryPackFormatterProvider.Register(new DictionaryFormatter<StatType, long>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<StatType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<StatType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AnnounceImmutableStatSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.actorID);
		writer.WriteValue<Dictionary<StatType, long>>(value.ImmutableStats);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref AnnounceImmutableStatSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		Dictionary<StatType, long> value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.ImmutableStats;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadValue(ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadValue<Dictionary<StatType, long>>();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AnnounceImmutableStatSig), 4, memberCount);
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
				value4 = value.actorID;
				value5 = value.ImmutableStats;
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
							reader.ReadValue(ref value5);
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
		value = new AnnounceImmutableStatSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			ImmutableStats = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.ImmutableStats = value5;
	}
}
