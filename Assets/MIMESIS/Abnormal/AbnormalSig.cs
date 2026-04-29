using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class AbnormalSig : IActorMsg, IMemoryPackable<AbnormalSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class AbnormalSigFormatter : MemoryPackFormatter<AbnormalSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalSig value)
		{
			AbnormalSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref AbnormalSig value)
		{
			AbnormalSig.Deserialize(ref reader, ref value);
		}
	}

	public List<AbnormalObjectInfo> abnormalIcons { get; set; } = new List<AbnormalObjectInfo>();

	public List<AbnormalCCInfo> ccList { get; set; } = new List<AbnormalCCInfo>();

	public List<AbnormalStatsInfo> statsList { get; set; } = new List<AbnormalStatsInfo>();

	public List<AbnormalImmuneInfo> immuneList { get; set; } = new List<AbnormalImmuneInfo>();

	public AbnormalSig()
		: base(MsgType.C2S_AbnormalSig)
	{
		base.reliable = true;
	}

	static AbnormalSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<AbnormalSig>())
		{
			MemoryPackFormatterProvider.Register(new AbnormalSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<AbnormalSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<AbnormalSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<AbnormalObjectInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<AbnormalObjectInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<AbnormalCCInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<AbnormalCCInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<AbnormalStatsInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<AbnormalStatsInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<AbnormalImmuneInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<AbnormalImmuneInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AbnormalSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(7, value.msgType, value.hashCode, value.actorID);
		ListFormatter.SerializePackable(ref writer, value.abnormalIcons);
		ListFormatter.SerializePackable(ref writer, value.ccList);
		ListFormatter.SerializePackable(ref writer, value.statsList);
		ListFormatter.SerializePackable(ref writer, value.immuneList);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref AbnormalSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		List<AbnormalObjectInfo> value5;
		List<AbnormalCCInfo> value6;
		List<AbnormalStatsInfo> value7;
		List<AbnormalImmuneInfo> value8;
		if (memberCount == 7)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.abnormalIcons;
				value6 = value.ccList;
				value7 = value.statsList;
				value8 = value.immuneList;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				ListFormatter.DeserializePackable(ref reader, ref value5);
				ListFormatter.DeserializePackable(ref reader, ref value6);
				ListFormatter.DeserializePackable(ref reader, ref value7);
				ListFormatter.DeserializePackable(ref reader, ref value8);
				goto IL_0188;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = ListFormatter.DeserializePackable<AbnormalObjectInfo>(ref reader);
			value6 = ListFormatter.DeserializePackable<AbnormalCCInfo>(ref reader);
			value7 = ListFormatter.DeserializePackable<AbnormalStatsInfo>(ref reader);
			value8 = ListFormatter.DeserializePackable<AbnormalImmuneInfo>(ref reader);
		}
		else
		{
			if (memberCount > 7)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AbnormalSig), 7, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = null;
				value6 = null;
				value7 = null;
				value8 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.abnormalIcons;
				value6 = value.ccList;
				value7 = value.statsList;
				value8 = value.immuneList;
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
							if (memberCount != 4)
							{
								ListFormatter.DeserializePackable(ref reader, ref value6);
								if (memberCount != 5)
								{
									ListFormatter.DeserializePackable(ref reader, ref value7);
									if (memberCount != 6)
									{
										ListFormatter.DeserializePackable(ref reader, ref value8);
										_ = 7;
									}
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0188;
			}
		}
		value = new AbnormalSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			abnormalIcons = value5,
			ccList = value6,
			statsList = value7,
			immuneList = value8
		};
		return;
		IL_0188:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.abnormalIcons = value5;
		value.ccList = value6;
		value.statsList = value7;
		value.immuneList = value8;
	}
}
