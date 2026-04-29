using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class MoveStartSig : IActorMsg, IMemoryPackable<MoveStartSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class MoveStartSigFormatter : MemoryPackFormatter<MoveStartSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStartSig value)
		{
			MoveStartSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref MoveStartSig value)
		{
			MoveStartSig.Deserialize(ref reader, ref value);
		}
	}

	public int targetID { get; set; }

	public ActorMoveType actorMoveType { get; set; }

	public PosWithRot basePositionPrev { get; set; } = new PosWithRot();

	public PosWithRot basePositionCurr { get; set; } = new PosWithRot();

	public PosWithRot basePositionFuture { get; set; } = new PosWithRot();

	public long futureTime { get; set; }

	public float pitch { get; set; }

	public MoveStartSig()
		: base(MsgType.C2S_MoveStartSig)
	{
	}

	static MoveStartSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStartSig>())
		{
			MemoryPackFormatterProvider.Register(new MoveStartSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStartSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<MoveStartSig>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStartSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, ActorMoveType>(10, value.msgType, value.hashCode, value.actorID, value.targetID, value.actorMoveType);
		writer.WritePackable<PosWithRot>(value.basePositionPrev);
		writer.WritePackable<PosWithRot>(value.basePositionCurr);
		writer.WritePackable<PosWithRot>(value.basePositionFuture);
		writer.WriteUnmanaged<long, float>(value.futureTime, value.pitch);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref MoveStartSig? value)
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
		ActorMoveType value6;
		PosWithRot value7;
		PosWithRot value8;
		PosWithRot value9;
		long value10;
		float value11;
		if (memberCount == 10)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.targetID;
				value6 = value.actorMoveType;
				value7 = value.basePositionPrev;
				value8 = value.basePositionCurr;
				value9 = value.basePositionFuture;
				value10 = value.futureTime;
				value11 = value.pitch;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<ActorMoveType>(out value6);
				reader.ReadPackable(ref value7);
				reader.ReadPackable(ref value8);
				reader.ReadPackable(ref value9);
				reader.ReadUnmanaged<long>(out value10);
				reader.ReadUnmanaged<float>(out value11);
				goto IL_0213;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, ActorMoveType>(out value2, out value3, out value4, out value5, out value6);
			value7 = reader.ReadPackable<PosWithRot>();
			value8 = reader.ReadPackable<PosWithRot>();
			value9 = reader.ReadPackable<PosWithRot>();
			reader.ReadUnmanaged<long, float>(out value10, out value11);
		}
		else
		{
			if (memberCount > 10)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(MoveStartSig), 10, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = ActorMoveType.None;
				value7 = null;
				value8 = null;
				value9 = null;
				value10 = 0L;
				value11 = 0f;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.targetID;
				value6 = value.actorMoveType;
				value7 = value.basePositionPrev;
				value8 = value.basePositionCurr;
				value9 = value.basePositionFuture;
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
							reader.ReadUnmanaged<int>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<ActorMoveType>(out value6);
								if (memberCount != 5)
								{
									reader.ReadPackable(ref value7);
									if (memberCount != 6)
									{
										reader.ReadPackable(ref value8);
										if (memberCount != 7)
										{
											reader.ReadPackable(ref value9);
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
		value = new MoveStartSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			targetID = value5,
			actorMoveType = value6,
			basePositionPrev = value7,
			basePositionCurr = value8,
			basePositionFuture = value9,
			futureTime = value10,
			pitch = value11
		};
		return;
		IL_0213:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.targetID = value5;
		value.actorMoveType = value6;
		value.basePositionPrev = value7;
		value.basePositionCurr = value8;
		value.basePositionFuture = value9;
		value.futureTime = value10;
		value.pitch = value11;
	}
}
