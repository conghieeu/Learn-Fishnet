using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

[MemoryPackable(GenerateType.Object)]
public class UseSkillReq : IMsg, IMemoryPackable<UseSkillReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class UseSkillReqFormatter : MemoryPackFormatter<UseSkillReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseSkillReq value)
		{
			UseSkillReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref UseSkillReq value)
		{
			UseSkillReq.Deserialize(ref reader, ref value);
		}
	}

	public int actorID { get; set; }

	public int skillMasterID { get; set; }

	public int targetID { get; set; }

	public PosWithRot startBasePosition { get; set; } = new PosWithRot();

	public PosWithRot endBasePosition { get; set; } = new PosWithRot();

	public List<Vector3> targetPos { get; set; } = new List<Vector3>();

	public UseSkillReq()
		: base(MsgType.C2S_UseSkillReq)
	{
		base.reliable = true;
	}

	static UseSkillReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<UseSkillReq>())
		{
			MemoryPackFormatterProvider.Register(new UseSkillReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<UseSkillReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<UseSkillReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<Vector3>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<Vector3>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseSkillReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, int>(8, value.msgType, value.hashCode, value.actorID, value.skillMasterID, value.targetID);
		writer.WritePackable<PosWithRot>(value.startBasePosition);
		writer.WritePackable<PosWithRot>(value.endBasePosition);
		writer.WriteValue<List<Vector3>>(value.targetPos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref UseSkillReq? value)
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
		PosWithRot value7;
		PosWithRot value8;
		List<Vector3> value9;
		if (memberCount == 8)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.skillMasterID;
				value6 = value.targetID;
				value7 = value.startBasePosition;
				value8 = value.endBasePosition;
				value9 = value.targetPos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				reader.ReadPackable(ref value7);
				reader.ReadPackable(ref value8);
				reader.ReadValue(ref value9);
				goto IL_01ad;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, int>(out value2, out value3, out value4, out value5, out value6);
			value7 = reader.ReadPackable<PosWithRot>();
			value8 = reader.ReadPackable<PosWithRot>();
			value9 = reader.ReadValue<List<Vector3>>();
		}
		else
		{
			if (memberCount > 8)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(UseSkillReq), 8, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = 0;
				value7 = null;
				value8 = null;
				value9 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.skillMasterID;
				value6 = value.targetID;
				value7 = value.startBasePosition;
				value8 = value.endBasePosition;
				value9 = value.targetPos;
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
									reader.ReadPackable(ref value7);
									if (memberCount != 6)
									{
										reader.ReadPackable(ref value8);
										if (memberCount != 7)
										{
											reader.ReadValue(ref value9);
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
				goto IL_01ad;
			}
		}
		value = new UseSkillReq
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			skillMasterID = value5,
			targetID = value6,
			startBasePosition = value7,
			endBasePosition = value8,
			targetPos = value9
		};
		return;
		IL_01ad:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.skillMasterID = value5;
		value.targetID = value6;
		value.startBasePosition = value7;
		value.endBasePosition = value8;
		value.targetPos = value9;
	}
}
