using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class UseSkillSig : IActorMsg, IMemoryPackable<UseSkillSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class UseSkillSigFormatter : MemoryPackFormatter<UseSkillSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseSkillSig value)
		{
			UseSkillSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref UseSkillSig value)
		{
			UseSkillSig.Deserialize(ref reader, ref value);
		}
	}

	public List<PosWithRot> targetPos = new List<PosWithRot>();

	public long skillSyncID { get; set; }

	public int skillMasterID { get; set; }

	public int targetID { get; set; }

	public PosWithRot startBasePosition { get; set; } = new PosWithRot();

	public PosWithRot endBasePosition { get; set; } = new PosWithRot();

	public UseSkillSig()
		: base(MsgType.C2S_UseSkillSig)
	{
		base.reliable = true;
	}

	static UseSkillSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<UseSkillSig>())
		{
			MemoryPackFormatterProvider.Register(new UseSkillSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<UseSkillSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<UseSkillSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<PosWithRot>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<PosWithRot>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseSkillSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, long, int, int>(9, value.msgType, value.hashCode, value.actorID, value.skillSyncID, value.skillMasterID, value.targetID);
		writer.WritePackable<PosWithRot>(value.startBasePosition);
		writer.WritePackable<PosWithRot>(value.endBasePosition);
		ListFormatter.SerializePackable(ref writer, value.targetPos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref UseSkillSig? value)
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
		int value7;
		PosWithRot value8;
		PosWithRot value9;
		List<PosWithRot> value10;
		if (memberCount == 9)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.skillSyncID;
				value6 = value.skillMasterID;
				value7 = value.targetID;
				value8 = value.startBasePosition;
				value9 = value.endBasePosition;
				value10 = value.targetPos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<long>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				reader.ReadUnmanaged<int>(out value7);
				reader.ReadPackable(ref value8);
				reader.ReadPackable(ref value9);
				ListFormatter.DeserializePackable(ref reader, ref value10);
				goto IL_01dd;
			}
			reader.ReadUnmanaged<MsgType, int, int, long, int, int>(out value2, out value3, out value4, out value5, out value6, out value7);
			value8 = reader.ReadPackable<PosWithRot>();
			value9 = reader.ReadPackable<PosWithRot>();
			value10 = ListFormatter.DeserializePackable<PosWithRot>(ref reader);
		}
		else
		{
			if (memberCount > 9)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(UseSkillSig), 9, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0L;
				value6 = 0;
				value7 = 0;
				value8 = null;
				value9 = null;
				value10 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.skillSyncID;
				value6 = value.skillMasterID;
				value7 = value.targetID;
				value8 = value.startBasePosition;
				value9 = value.endBasePosition;
				value10 = value.targetPos;
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
									reader.ReadUnmanaged<int>(out value7);
									if (memberCount != 6)
									{
										reader.ReadPackable(ref value8);
										if (memberCount != 7)
										{
											reader.ReadPackable(ref value9);
											if (memberCount != 8)
											{
												ListFormatter.DeserializePackable(ref reader, ref value10);
												_ = 9;
											}
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
				goto IL_01dd;
			}
		}
		value = new UseSkillSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			skillSyncID = value5,
			skillMasterID = value6,
			targetID = value7,
			startBasePosition = value8,
			endBasePosition = value9,
			targetPos = value10
		};
		return;
		IL_01dd:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.skillSyncID = value5;
		value.skillMasterID = value6;
		value.targetID = value7;
		value.startBasePosition = value8;
		value.endBasePosition = value9;
		value.targetPos = value10;
	}
}
