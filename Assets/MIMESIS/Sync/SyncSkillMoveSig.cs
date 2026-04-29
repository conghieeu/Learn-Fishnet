using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class SyncSkillMoveSig : IActorMsg, IMemoryPackable<SyncSkillMoveSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SyncSkillMoveSigFormatter : MemoryPackFormatter<SyncSkillMoveSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SyncSkillMoveSig value)
		{
			SyncSkillMoveSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SyncSkillMoveSig value)
		{
			SyncSkillMoveSig.Deserialize(ref reader, ref value);
		}
	}

	public int targetActorID { get; set; }

	public long skillSyncID { get; set; }

	public int skillMasterID { get; set; }

	public PosWithRot targetPosition { get; set; } = new PosWithRot();

	public float moveSpeed { get; set; }

	public SyncSkillMoveSig()
		: base(MsgType.C2S_SyncSkillMoveSig)
	{
	}

	static SyncSkillMoveSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SyncSkillMoveSig>())
		{
			MemoryPackFormatterProvider.Register(new SyncSkillMoveSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SyncSkillMoveSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SyncSkillMoveSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SyncSkillMoveSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, long, int>(8, value.msgType, value.hashCode, value.actorID, value.targetActorID, value.skillSyncID, value.skillMasterID);
		writer.WritePackable<PosWithRot>(value.targetPosition);
		writer.WriteUnmanaged<float>(value.moveSpeed);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SyncSkillMoveSig? value)
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
		int value7;
		PosWithRot value8;
		float value9;
		if (memberCount == 8)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.targetActorID;
				value6 = value.skillSyncID;
				value7 = value.skillMasterID;
				value8 = value.targetPosition;
				value9 = value.moveSpeed;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<long>(out value6);
				reader.ReadUnmanaged<int>(out value7);
				reader.ReadPackable(ref value8);
				reader.ReadUnmanaged<float>(out value9);
				goto IL_01ac;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, long, int>(out value2, out value3, out value4, out value5, out value6, out value7);
			value8 = reader.ReadPackable<PosWithRot>();
			reader.ReadUnmanaged<float>(out value9);
		}
		else
		{
			if (memberCount > 8)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SyncSkillMoveSig), 8, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = 0L;
				value7 = 0;
				value8 = null;
				value9 = 0f;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.targetActorID;
				value6 = value.skillSyncID;
				value7 = value.skillMasterID;
				value8 = value.targetPosition;
				value9 = value.moveSpeed;
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
									reader.ReadUnmanaged<int>(out value7);
									if (memberCount != 6)
									{
										reader.ReadPackable(ref value8);
										if (memberCount != 7)
										{
											reader.ReadUnmanaged<float>(out value9);
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
				goto IL_01ac;
			}
		}
		value = new SyncSkillMoveSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			targetActorID = value5,
			skillSyncID = value6,
			skillMasterID = value7,
			targetPosition = value8,
			moveSpeed = value9
		};
		return;
		IL_01ac:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.targetActorID = value5;
		value.skillSyncID = value6;
		value.skillMasterID = value7;
		value.targetPosition = value8;
		value.moveSpeed = value9;
	}
}
