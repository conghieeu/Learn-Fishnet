using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class MoveStartReq : IMsg, IMemoryPackable<MoveStartReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class MoveStartReqFormatter : MemoryPackFormatter<MoveStartReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStartReq value)
		{
			MoveStartReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref MoveStartReq value)
		{
			MoveStartReq.Deserialize(ref reader, ref value);
		}
	}

	public int transID { get; set; }

	public ActorMoveType actorMoveType { get; set; }

	public PosWithRot basePositionPrev { get; set; }

	public PosWithRot basePositionCurr { get; set; }

	public PosWithRot basePositionFuture { get; set; }

	public int targetActorID { get; set; }

	public long futureTime { get; set; }

	public float pitch { get; set; }

	public MoveStartReq()
		: base(MsgType.C2S_MoveStartReq)
	{
	}

	static MoveStartReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStartReq>())
		{
			MemoryPackFormatterProvider.Register(new MoveStartReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStartReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<MoveStartReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ActorMoveType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ActorMoveType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStartReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, ActorMoveType>(10, value.msgType, value.hashCode, value.transID, value.actorMoveType);
		writer.WritePackable<PosWithRot>(value.basePositionPrev);
		writer.WritePackable<PosWithRot>(value.basePositionCurr);
		writer.WritePackable<PosWithRot>(value.basePositionFuture);
		writer.WriteUnmanaged<int, long, float>(value.targetActorID, value.futureTime, value.pitch);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref MoveStartReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		ActorMoveType value5;
		PosWithRot value6;
		PosWithRot value7;
		PosWithRot value8;
		int value9;
		long value10;
		float value11;
		if (memberCount == 10)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.transID;
				value5 = value.actorMoveType;
				value6 = value.basePositionPrev;
				value7 = value.basePositionCurr;
				value8 = value.basePositionFuture;
				value9 = value.targetActorID;
				value10 = value.futureTime;
				value11 = value.pitch;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<ActorMoveType>(out value5);
				reader.ReadPackable(ref value6);
				reader.ReadPackable(ref value7);
				reader.ReadPackable(ref value8);
				reader.ReadUnmanaged<int>(out value9);
				reader.ReadUnmanaged<long>(out value10);
				reader.ReadUnmanaged<float>(out value11);
				goto IL_0213;
			}
			reader.ReadUnmanaged<MsgType, int, int, ActorMoveType>(out value2, out value3, out value4, out value5);
			value6 = reader.ReadPackable<PosWithRot>();
			value7 = reader.ReadPackable<PosWithRot>();
			value8 = reader.ReadPackable<PosWithRot>();
			reader.ReadUnmanaged<int, long, float>(out value9, out value10, out value11);
		}
		else
		{
			if (memberCount > 10)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(MoveStartReq), 10, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = ActorMoveType.None;
				value6 = null;
				value7 = null;
				value8 = null;
				value9 = 0;
				value10 = 0L;
				value11 = 0f;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.transID;
				value5 = value.actorMoveType;
				value6 = value.basePositionPrev;
				value7 = value.basePositionCurr;
				value8 = value.basePositionFuture;
				value9 = value.targetActorID;
				value10 = value.futureTime;
				value11 = value.pitch;
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
							reader.ReadUnmanaged<ActorMoveType>(out value5);
							if (memberCount != 4)
							{
								reader.ReadPackable(ref value6);
								if (memberCount != 5)
								{
									reader.ReadPackable(ref value7);
									if (memberCount != 6)
									{
										reader.ReadPackable(ref value8);
										if (memberCount != 7)
										{
											reader.ReadUnmanaged<int>(out value9);
											if (memberCount != 8)
											{
												reader.ReadUnmanaged<long>(out value10);
												if (memberCount != 9)
												{
													reader.ReadUnmanaged<float>(out value11);
													_ = 10;
												}
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
				goto IL_0213;
			}
		}
		value = new MoveStartReq
		{
			msgType = value2,
			hashCode = value3,
			transID = value4,
			actorMoveType = value5,
			basePositionPrev = value6,
			basePositionCurr = value7,
			basePositionFuture = value8,
			targetActorID = value9,
			futureTime = value10,
			pitch = value11
		};
		return;
		IL_0213:
		value.msgType = value2;
		value.hashCode = value3;
		value.transID = value4;
		value.actorMoveType = value5;
		value.basePositionPrev = value6;
		value.basePositionCurr = value7;
		value.basePositionFuture = value8;
		value.targetActorID = value9;
		value.futureTime = value10;
		value.pitch = value11;
	}
}
