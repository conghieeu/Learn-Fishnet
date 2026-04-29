using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ActorChangePhaseNoti : IActorMsg, IMemoryPackable<ActorChangePhaseNoti>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ActorChangePhaseNotiFormatter : MemoryPackFormatter<ActorChangePhaseNoti>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ActorChangePhaseNoti value)
		{
			ActorChangePhaseNoti.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ActorChangePhaseNoti value)
		{
			ActorChangePhaseNoti.Deserialize(ref reader, ref value);
		}
	}

	public int PhaseID { get; set; }

	public ActorChangePhaseNoti()
		: base(MsgType.C2S_ActorChangePhaseNoti)
	{
	}

	static ActorChangePhaseNoti()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ActorChangePhaseNoti>())
		{
			MemoryPackFormatterProvider.Register(new ActorChangePhaseNotiFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ActorChangePhaseNoti[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ActorChangePhaseNoti>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ActorChangePhaseNoti? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int>(4, value.msgType, value.hashCode, value.actorID, value.PhaseID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ActorChangePhaseNoti? value)
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
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.PhaseID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				goto IL_00e9;
			}
			reader.ReadUnmanaged<MsgType, int, int, int>(out value2, out value3, out value4, out value5);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ActorChangePhaseNoti), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.PhaseID;
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
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00e9;
			}
		}
		value = new ActorChangePhaseNoti
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			PhaseID = value5
		};
		return;
		IL_00e9:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.PhaseID = value5;
	}
}
