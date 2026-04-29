using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class CooltimeSig : IActorMsg, IMemoryPackable<CooltimeSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class CooltimeSigFormatter : MemoryPackFormatter<CooltimeSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CooltimeSig value)
		{
			CooltimeSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref CooltimeSig value)
		{
			CooltimeSig.Deserialize(ref reader, ref value);
		}
	}

	public List<SkillCooltimeInfo> skillCooltimeInfos { get; set; } = new List<SkillCooltimeInfo>();

	public CooltimeSig()
		: base(MsgType.C2S_CooltimeSig)
	{
	}

	static CooltimeSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<CooltimeSig>())
		{
			MemoryPackFormatterProvider.Register(new CooltimeSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<CooltimeSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<CooltimeSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<SkillCooltimeInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<SkillCooltimeInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CooltimeSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.actorID);
		ListFormatter.SerializePackable(ref writer, value.skillCooltimeInfos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref CooltimeSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		List<SkillCooltimeInfo> value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.skillCooltimeInfos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				ListFormatter.DeserializePackable(ref reader, ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = ListFormatter.DeserializePackable<SkillCooltimeInfo>(ref reader);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CooltimeSig), 4, memberCount);
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
				value5 = value.skillCooltimeInfos;
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
							ListFormatter.DeserializePackable(ref reader, ref value5);
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
		value = new CooltimeSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			skillCooltimeInfos = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.skillCooltimeInfos = value5;
	}
}
