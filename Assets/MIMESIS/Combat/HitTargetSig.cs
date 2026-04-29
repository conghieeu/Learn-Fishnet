using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class HitTargetSig : IActorMsg, IMemoryPackable<HitTargetSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class HitTargetSigFormatter : MemoryPackFormatter<HitTargetSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref HitTargetSig value)
		{
			HitTargetSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref HitTargetSig value)
		{
			HitTargetSig.Deserialize(ref reader, ref value);
		}
	}

	public long skillSyncID { get; set; }

	public int skillSequenceMasterID { get; set; }

	public List<TargetHitInfo> targetHitInfos { get; set; } = new List<TargetHitInfo>();

	public HitTargetSig()
		: base(MsgType.C2S_HitTargetSig)
	{
		base.reliable = true;
	}

	static HitTargetSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<HitTargetSig>())
		{
			MemoryPackFormatterProvider.Register(new HitTargetSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HitTargetSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<HitTargetSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<TargetHitInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<TargetHitInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref HitTargetSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, long, int>(6, value.msgType, value.hashCode, value.actorID, value.skillSyncID, value.skillSequenceMasterID);
		ListFormatter.SerializePackable(ref writer, value.targetHitInfos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref HitTargetSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		long value5;
		int value6;
		List<TargetHitInfo> value7;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.skillSyncID;
				value6 = value.skillSequenceMasterID;
				value7 = value.targetHitInfos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<long>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				ListFormatter.DeserializePackable(ref reader, ref value7);
				goto IL_014c;
			}
			reader.ReadUnmanaged<MsgType, int, int, long, int>(out value2, out value3, out value4, out value5, out value6);
			value7 = ListFormatter.DeserializePackable<TargetHitInfo>(ref reader);
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(HitTargetSig), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0L;
				value6 = 0;
				value7 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.skillSyncID;
				value6 = value.skillSequenceMasterID;
				value7 = value.targetHitInfos;
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
							reader.ReadUnmanaged<long>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<int>(out value6);
								if (memberCount != 5)
								{
									ListFormatter.DeserializePackable(ref reader, ref value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_014c;
			}
		}
		value = new HitTargetSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			skillSyncID = value5,
			skillSequenceMasterID = value6,
			targetHitInfos = value7
		};
		return;
		IL_014c:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.skillSyncID = value5;
		value.skillSequenceMasterID = value6;
		value.targetHitInfos = value7;
	}
}
