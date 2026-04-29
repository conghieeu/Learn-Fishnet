using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class AttachActorSig : IActorMsg, IMemoryPackable<AttachActorSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class AttachActorSigFormatter : MemoryPackFormatter<AttachActorSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AttachActorSig value)
		{
			AttachActorSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref AttachActorSig value)
		{
			AttachActorSig.Deserialize(ref reader, ref value);
		}
	}

	public int targetID { get; set; }

	public int socketIndex { get; set; }

	public AttachState state { get; set; }

	public int grabMasterID { get; set; }

	public AttachActorSig()
		: base(MsgType.C2S_AttachActorSig)
	{
		base.reliable = true;
	}

	static AttachActorSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<AttachActorSig>())
		{
			MemoryPackFormatterProvider.Register(new AttachActorSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<AttachActorSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<AttachActorSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<AttachState>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<AttachState>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref AttachActorSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, int, AttachState, int>(7, value.msgType, value.hashCode, value.actorID, value.targetID, value.socketIndex, value.state, value.grabMasterID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref AttachActorSig? value)
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
		AttachState value7;
		int value8;
		if (memberCount == 7)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.targetID;
				value6 = value.socketIndex;
				value7 = value.state;
				value8 = value.grabMasterID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				reader.ReadUnmanaged<AttachState>(out value7);
				reader.ReadUnmanaged<int>(out value8);
				goto IL_0170;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, int, AttachState, int>(out value2, out value3, out value4, out value5, out value6, out value7, out value8);
		}
		else
		{
			if (memberCount > 7)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(AttachActorSig), 7, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = 0;
				value7 = AttachState.Attached;
				value8 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.targetID;
				value6 = value.socketIndex;
				value7 = value.state;
				value8 = value.grabMasterID;
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
									reader.ReadUnmanaged<AttachState>(out value7);
									if (memberCount != 6)
									{
										reader.ReadUnmanaged<int>(out value8);
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
				goto IL_0170;
			}
		}
		value = new AttachActorSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			targetID = value5,
			socketIndex = value6,
			state = value7,
			grabMasterID = value8
		};
		return;
		IL_0170:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.targetID = value5;
		value.socketIndex = value6;
		value.state = value7;
		value.grabMasterID = value8;
	}
}
