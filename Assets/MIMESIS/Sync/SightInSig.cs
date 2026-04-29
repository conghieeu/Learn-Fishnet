using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class SightInSig : IMsg, IMemoryPackable<SightInSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SightInSigFormatter : MemoryPackFormatter<SightInSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SightInSig value)
		{
			SightInSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SightInSig value)
		{
			SightInSig.Deserialize(ref reader, ref value);
		}
	}

	public SightReason sightReason;

	public List<PlayerInfo> playerInfos { get; set; } = new List<PlayerInfo>();

	public List<OtherCreatureInfo> monsterInfos { get; set; } = new List<OtherCreatureInfo>();

	public List<LootingObjectInfo> lootingObjectInfos { get; set; } = new List<LootingObjectInfo>();

	public List<FieldSkillObjectInfo> fieldSkillObjectInfos { get; set; } = new List<FieldSkillObjectInfo>();

	public List<ProjectileObjectInfo> projectileObjectInfos { get; set; } = new List<ProjectileObjectInfo>();

	public SightInSig()
		: base(MsgType.C2S_SightInSig)
	{
		base.reliable = true;
	}

	static SightInSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SightInSig>())
		{
			MemoryPackFormatterProvider.Register(new SightInSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SightInSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SightInSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SightReason>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SightReason>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<PlayerInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<PlayerInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<OtherCreatureInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<OtherCreatureInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<LootingObjectInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<LootingObjectInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<FieldSkillObjectInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<FieldSkillObjectInfo>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<ProjectileObjectInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<ProjectileObjectInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SightInSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, SightReason>(8, value.msgType, value.hashCode, in value.sightReason);
		ListFormatter.SerializePackable(ref writer, value.playerInfos);
		ListFormatter.SerializePackable(ref writer, value.monsterInfos);
		ListFormatter.SerializePackable(ref writer, value.lootingObjectInfos);
		ListFormatter.SerializePackable(ref writer, value.fieldSkillObjectInfos);
		ListFormatter.SerializePackable(ref writer, value.projectileObjectInfos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SightInSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		SightReason value4;
		List<PlayerInfo> value5;
		List<OtherCreatureInfo> value6;
		List<LootingObjectInfo> value7;
		List<FieldSkillObjectInfo> value8;
		List<ProjectileObjectInfo> value9;
		if (memberCount == 8)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.sightReason;
				value5 = value.playerInfos;
				value6 = value.monsterInfos;
				value7 = value.lootingObjectInfos;
				value8 = value.fieldSkillObjectInfos;
				value9 = value.projectileObjectInfos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<SightReason>(out value4);
				ListFormatter.DeserializePackable(ref reader, ref value5);
				ListFormatter.DeserializePackable(ref reader, ref value6);
				ListFormatter.DeserializePackable(ref reader, ref value7);
				ListFormatter.DeserializePackable(ref reader, ref value8);
				ListFormatter.DeserializePackable(ref reader, ref value9);
				goto IL_01b9;
			}
			reader.ReadUnmanaged<MsgType, int, SightReason>(out value2, out value3, out value4);
			value5 = ListFormatter.DeserializePackable<PlayerInfo>(ref reader);
			value6 = ListFormatter.DeserializePackable<OtherCreatureInfo>(ref reader);
			value7 = ListFormatter.DeserializePackable<LootingObjectInfo>(ref reader);
			value8 = ListFormatter.DeserializePackable<FieldSkillObjectInfo>(ref reader);
			value9 = ListFormatter.DeserializePackable<ProjectileObjectInfo>(ref reader);
		}
		else
		{
			if (memberCount > 8)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SightInSig), 8, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = SightReason.None;
				value5 = null;
				value6 = null;
				value7 = null;
				value8 = null;
				value9 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.sightReason;
				value5 = value.playerInfos;
				value6 = value.monsterInfos;
				value7 = value.lootingObjectInfos;
				value8 = value.fieldSkillObjectInfos;
				value9 = value.projectileObjectInfos;
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
										if (memberCount != 7)
										{
											ListFormatter.DeserializePackable(ref reader, ref value9);
											_ = 8;
										}
									}
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_01b9;
			}
		}
		value = new SightInSig
		{
			msgType = value2,
			hashCode = value3,
			sightReason = value4,
			playerInfos = value5,
			monsterInfos = value6,
			lootingObjectInfos = value7,
			fieldSkillObjectInfos = value8,
			projectileObjectInfos = value9
		};
		return;
		IL_01b9:
		value.msgType = value2;
		value.hashCode = value3;
		value.sightReason = value4;
		value.playerInfos = value5;
		value.monsterInfos = value6;
		value.lootingObjectInfos = value7;
		value.fieldSkillObjectInfos = value8;
		value.projectileObjectInfos = value9;
	}
}
