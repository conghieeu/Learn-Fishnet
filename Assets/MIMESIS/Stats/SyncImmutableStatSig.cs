using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class SyncImmutableStatSig : IMsg, IMemoryPackable<SyncImmutableStatSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SyncImmutableStatSigFormatter : MemoryPackFormatter<SyncImmutableStatSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SyncImmutableStatSig value)
		{
			SyncImmutableStatSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SyncImmutableStatSig value)
		{
			SyncImmutableStatSig.Deserialize(ref reader, ref value);
		}
	}

	public Dictionary<StatType, long> ImmutableStats { get; set; } = new Dictionary<StatType, long>();

	public SyncImmutableStatSig()
		: base(MsgType.C2S_SyncImmutableStatSig)
	{
	}

	static SyncImmutableStatSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SyncImmutableStatSig>())
		{
			MemoryPackFormatterProvider.Register(new SyncImmutableStatSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SyncImmutableStatSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SyncImmutableStatSig>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SyncImmutableStatSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
		writer.WriteValue<Dictionary<StatType, long>>(value.ImmutableStats);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SyncImmutableStatSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		Dictionary<StatType, long> value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.ImmutableStats;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadValue(ref value4);
				goto IL_00c3;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			value4 = reader.ReadValue<Dictionary<StatType, long>>();
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SyncImmutableStatSig), 3, memberCount);
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
				value4 = value.ImmutableStats;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadValue(ref value4);
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00c3;
			}
		}
		value = new SyncImmutableStatSig
		{
			msgType = value2,
			hashCode = value3,
			ImmutableStats = value4
		};
		return;
		IL_00c3:
		value.msgType = value2;
		value.hashCode = value3;
		value.ImmutableStats = value4;
	}
}
