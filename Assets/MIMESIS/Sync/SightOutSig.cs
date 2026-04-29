using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class SightOutSig : IMsg, IMemoryPackable<SightOutSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SightOutSigFormatter : MemoryPackFormatter<SightOutSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SightOutSig value)
		{
			SightOutSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SightOutSig value)
		{
			SightOutSig.Deserialize(ref reader, ref value);
		}
	}

	public SightReason sightReason;

	public List<int> actorIDs = new List<int>();

	public SightOutSig()
		: base(MsgType.C2S_SightOutSig)
	{
		base.reliable = true;
	}

	static SightOutSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SightOutSig>())
		{
			MemoryPackFormatterProvider.Register(new SightOutSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SightOutSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SightOutSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SightReason>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SightReason>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<int>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SightOutSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, SightReason>(4, value.msgType, value.hashCode, in value.sightReason);
		writer.WriteValue(in value.actorIDs);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SightOutSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		SightReason value4;
		List<int> value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.sightReason;
				value5 = value.actorIDs;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<SightReason>(out value4);
				reader.ReadValue(ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, SightReason>(out value2, out value3, out value4);
			value5 = reader.ReadValue<List<int>>();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SightOutSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = SightReason.None;
				value5 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.sightReason;
				value5 = value.actorIDs;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<SightReason>(out value4);
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
		value = new SightOutSig
		{
			msgType = value2,
			hashCode = value3,
			sightReason = value4,
			actorIDs = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.sightReason = value4;
		value.actorIDs = value5;
	}
}
