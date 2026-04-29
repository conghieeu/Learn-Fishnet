using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ActorDyingSig : IActorMsg, IMemoryPackable<ActorDyingSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ActorDyingSigFormatter : MemoryPackFormatter<ActorDyingSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ActorDyingSig value)
		{
			ActorDyingSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ActorDyingSig value)
		{
			ActorDyingSig.Deserialize(ref reader, ref value);
		}
	}

	public int attackerActorID { get; set; }

	public ReasonOfDeath reasonOfDeath { get; set; }

	public int linkedMasterID { get; set; }

	public ActorDyingSig()
		: base(MsgType.C2S_ActorDyingSig)
	{
		base.reliable = true;
	}

	static ActorDyingSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ActorDyingSig>())
		{
			MemoryPackFormatterProvider.Register(new ActorDyingSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ActorDyingSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ActorDyingSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ReasonOfDeath>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ReasonOfDeath>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ActorDyingSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, ReasonOfDeath, int>(6, value.msgType, value.hashCode, value.actorID, value.attackerActorID, value.reasonOfDeath, value.linkedMasterID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ActorDyingSig? value)
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
		ReasonOfDeath value6;
		int value7;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.attackerActorID;
				value6 = value.reasonOfDeath;
				value7 = value.linkedMasterID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<ReasonOfDeath>(out value6);
				reader.ReadUnmanaged<int>(out value7);
				goto IL_0145;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, ReasonOfDeath, int>(out value2, out value3, out value4, out value5, out value6, out value7);
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ActorDyingSig), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = ReasonOfDeath.None;
				value7 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.attackerActorID;
				value6 = value.reasonOfDeath;
				value7 = value.linkedMasterID;
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
								reader.ReadUnmanaged<ReasonOfDeath>(out value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<int>(out value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0145;
			}
		}
		value = new ActorDyingSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			attackerActorID = value5,
			reasonOfDeath = value6,
			linkedMasterID = value7
		};
		return;
		IL_0145:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.attackerActorID = value5;
		value.reasonOfDeath = value6;
		value.linkedMasterID = value7;
	}
}
