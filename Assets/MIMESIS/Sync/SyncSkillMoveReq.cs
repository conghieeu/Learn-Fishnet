using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class SyncSkillMoveReq : IMsg, IMemoryPackable<SyncSkillMoveReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SyncSkillMoveReqFormatter : MemoryPackFormatter<SyncSkillMoveReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SyncSkillMoveReq value)
		{
			SyncSkillMoveReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SyncSkillMoveReq value)
		{
			SyncSkillMoveReq.Deserialize(ref reader, ref value);
		}
	}

	public int targetActorID { get; set; }

	public int skillMasterID { get; set; }

	public long skillSyncID { get; set; }

	public PosWithRot targetPosition { get; set; } = new PosWithRot();

	public float moveSpeed { get; set; }

	public SyncSkillMoveReq()
		: base(MsgType.C2S_SyncSkillMoveReq)
	{
	}

	static SyncSkillMoveReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SyncSkillMoveReq>())
		{
			MemoryPackFormatterProvider.Register(new SyncSkillMoveReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SyncSkillMoveReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SyncSkillMoveReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SyncSkillMoveReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, long>(7, value.msgType, value.hashCode, value.targetActorID, value.skillMasterID, value.skillSyncID);
		writer.WritePackable<PosWithRot>(value.targetPosition);
		writer.WriteUnmanaged<float>(value.moveSpeed);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SyncSkillMoveReq? value)
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
		long value6;
		PosWithRot value7;
		float value8;
		if (memberCount == 7)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.targetActorID;
				value5 = value.skillMasterID;
				value6 = value.skillSyncID;
				value7 = value.targetPosition;
				value8 = value.moveSpeed;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<long>(out value6);
				reader.ReadPackable(ref value7);
				reader.ReadUnmanaged<float>(out value8);
				goto IL_0181;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, long>(out value2, out value3, out value4, out value5, out value6);
			value7 = reader.ReadPackable<PosWithRot>();
			reader.ReadUnmanaged<float>(out value8);
		}
		else
		{
			if (memberCount > 7)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SyncSkillMoveReq), 7, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = 0L;
				value7 = null;
				value8 = 0f;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.targetActorID;
				value5 = value.skillMasterID;
				value6 = value.skillSyncID;
				value7 = value.targetPosition;
				value8 = value.moveSpeed;
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
								reader.ReadUnmanaged<long>(out value6);
								if (memberCount != 5)
								{
									reader.ReadPackable(ref value7);
									if (memberCount != 6)
									{
										reader.ReadUnmanaged<float>(out value8);
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
				goto IL_0181;
			}
		}
		value = new SyncSkillMoveReq
		{
			msgType = value2,
			hashCode = value3,
			targetActorID = value4,
			skillMasterID = value5,
			skillSyncID = value6,
			targetPosition = value7,
			moveSpeed = value8
		};
		return;
		IL_0181:
		value.msgType = value2;
		value.hashCode = value3;
		value.targetActorID = value4;
		value.skillMasterID = value5;
		value.skillSyncID = value6;
		value.targetPosition = value7;
		value.moveSpeed = value8;
	}
}
