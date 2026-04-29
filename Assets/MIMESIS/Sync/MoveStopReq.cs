using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class MoveStopReq : IMsg, IMemoryPackable<MoveStopReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class MoveStopReqFormatter : MemoryPackFormatter<MoveStopReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStopReq value)
		{
			MoveStopReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref MoveStopReq value)
		{
			MoveStopReq.Deserialize(ref reader, ref value);
		}
	}

	public ActorMoveType actorMoveType { get; set; }

	public PosWithRot prevPos { get; set; }

	public PosWithRot currentPos { get; set; }

	public MoveStopReq()
		: base(MsgType.C2S_MoveStopReq)
	{
	}

	static MoveStopReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStopReq>())
		{
			MemoryPackFormatterProvider.Register(new MoveStopReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MoveStopReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<MoveStopReq>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MoveStopReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, ActorMoveType>(5, value.msgType, value.hashCode, value.actorMoveType);
		writer.WritePackable<PosWithRot>(value.prevPos);
		writer.WritePackable<PosWithRot>(value.currentPos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref MoveStopReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		ActorMoveType value4;
		PosWithRot value5;
		PosWithRot value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorMoveType;
				value5 = value.prevPos;
				value6 = value.currentPos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<ActorMoveType>(out value4);
				reader.ReadPackable(ref value5);
				reader.ReadPackable(ref value6);
				goto IL_0123;
			}
			reader.ReadUnmanaged<MsgType, int, ActorMoveType>(out value2, out value3, out value4);
			value5 = reader.ReadPackable<PosWithRot>();
			value6 = reader.ReadPackable<PosWithRot>();
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(MoveStopReq), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = ActorMoveType.None;
				value5 = null;
				value6 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorMoveType;
				value5 = value.prevPos;
				value6 = value.currentPos;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<ActorMoveType>(out value4);
						if (memberCount != 3)
						{
							reader.ReadPackable(ref value5);
							if (memberCount != 4)
							{
								reader.ReadPackable(ref value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0123;
			}
		}
		value = new MoveStopReq
		{
			msgType = value2,
			hashCode = value3,
			actorMoveType = value4,
			prevPos = value5,
			currentPos = value6
		};
		return;
		IL_0123:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorMoveType = value4;
		value.prevPos = value5;
		value.currentPos = value6;
	}
}
