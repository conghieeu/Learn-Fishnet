using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class MoveStopSig : IActorMsg, IMemoryPackable<MoveStopSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class MoveStopSigFormatter : MemoryPackFormatter<MoveStopSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStopSig value)
		{
			MoveStopSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref MoveStopSig value)
		{
			MoveStopSig.Deserialize(ref reader, ref value);
		}
	}

	public ActorMoveType actorMoveType { get; set; }

	public int targetID { get; set; }

	public PosWithRot currentPos { get; set; }

	public MoveStopSig()
		: base(MsgType.C2S_MoveStopSig)
	{
	}

	static MoveStopSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStopSig>())
		{
			MemoryPackFormatterProvider.Register(new MoveStopSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStopSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<MoveStopSig>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStopSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, ActorMoveType, int>(6, value.msgType, value.hashCode, value.actorID, value.actorMoveType, value.targetID);
		writer.WritePackable<PosWithRot>(value.currentPos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref MoveStopSig? value)
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
		int value6;
		PosWithRot value7;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.actorMoveType;
				value6 = value.targetID;
				value7 = value.currentPos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<ActorMoveType>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				reader.ReadPackable(ref value7);
				goto IL_014b;
			}
			reader.ReadUnmanaged<MsgType, int, int, ActorMoveType, int>(out value2, out value3, out value4, out value5, out value6);
			value7 = reader.ReadPackable<PosWithRot>();
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(MoveStopSig), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = ActorMoveType.None;
				value6 = 0;
				value7 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.actorMoveType;
				value6 = value.targetID;
				value7 = value.currentPos;
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
								reader.ReadUnmanaged<int>(out value6);
								if (memberCount != 5)
								{
									reader.ReadPackable(ref value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_014b;
			}
		}
		value = new MoveStopSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			actorMoveType = value5,
			targetID = value6,
			currentPos = value7
		};
		return;
		IL_014b:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.actorMoveType = value5;
		value.targetID = value6;
		value.currentPos = value7;
	}
}
