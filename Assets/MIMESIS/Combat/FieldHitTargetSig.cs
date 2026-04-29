using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class FieldHitTargetSig : IMsg, IMemoryPackable<FieldHitTargetSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class FieldHitTargetSigFormatter : MemoryPackFormatter<FieldHitTargetSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FieldHitTargetSig value)
		{
			FieldHitTargetSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref FieldHitTargetSig value)
		{
			FieldHitTargetSig.Deserialize(ref reader, ref value);
		}
	}

	public int fieldSkillObjectID { get; set; }

	public int fieldSkillMasterID { get; set; }

	public int fieldSkillIndex { get; set; }

	public int skillSequenceMasterID { get; set; }

	public List<TargetHitInfo> targetHitInfos { get; set; } = new List<TargetHitInfo>();

	public FieldHitTargetSig()
		: base(MsgType.C2S_FieldHitTargetSig)
	{
		base.reliable = true;
	}

	static FieldHitTargetSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<FieldHitTargetSig>())
		{
			MemoryPackFormatterProvider.Register(new FieldHitTargetSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<FieldHitTargetSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<FieldHitTargetSig>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref FieldHitTargetSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, int, int>(7, value.msgType, value.hashCode, value.fieldSkillObjectID, value.fieldSkillMasterID, value.fieldSkillIndex, value.skillSequenceMasterID);
		ListFormatter.SerializePackable(ref writer, value.targetHitInfos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref FieldHitTargetSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		int value5;
		int value6;
		int value7;
		List<TargetHitInfo> value8;
		if (memberCount == 7)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.fieldSkillObjectID;
				value5 = value.fieldSkillMasterID;
				value6 = value.fieldSkillIndex;
				value7 = value.skillSequenceMasterID;
				value8 = value.targetHitInfos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				reader.ReadUnmanaged<int>(out value7);
				ListFormatter.DeserializePackable(ref reader, ref value8);
				goto IL_0176;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, int, int>(out value2, out value3, out value4, out value5, out value6, out value7);
			value8 = ListFormatter.DeserializePackable<TargetHitInfo>(ref reader);
		}
		else
		{
			if (memberCount > 7)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(FieldHitTargetSig), 7, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = 0;
				value7 = 0;
				value8 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.fieldSkillObjectID;
				value5 = value.fieldSkillMasterID;
				value6 = value.fieldSkillIndex;
				value7 = value.skillSequenceMasterID;
				value8 = value.targetHitInfos;
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
							reader.ReadUnmanaged<int>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<int>(out value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<int>(out value7);
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
				goto IL_0176;
			}
		}
		value = new FieldHitTargetSig
		{
			msgType = value2,
			hashCode = value3,
			fieldSkillObjectID = value4,
			fieldSkillMasterID = value5,
			fieldSkillIndex = value6,
			skillSequenceMasterID = value7,
			targetHitInfos = value8
		};
		return;
		IL_0176:
		value.msgType = value2;
		value.hashCode = value3;
		value.fieldSkillObjectID = value4;
		value.fieldSkillMasterID = value5;
		value.fieldSkillIndex = value6;
		value.skillSequenceMasterID = value7;
		value.targetHitInfos = value8;
	}
}
