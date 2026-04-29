using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class DebugDLAgentInfoSig : IActorMsg, IMemoryPackable<DebugDLAgentInfoSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class DebugDLAgentInfoSigFormatter : MemoryPackFormatter<DebugDLAgentInfoSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugDLAgentInfoSig value)
		{
			DebugDLAgentInfoSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DebugDLAgentInfoSig value)
		{
			DebugDLAgentInfoSig.Deserialize(ref reader, ref value);
		}
	}

	public int changedInfoType { get; set; }

	public int changedInfo { get; set; }

	public DebugDLAgentInfoSig()
		: base(MsgType.C2S_DebugDLAgentInfoSig)
	{
	}

	static DebugDLAgentInfoSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DebugDLAgentInfoSig>())
		{
			MemoryPackFormatterProvider.Register(new DebugDLAgentInfoSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DebugDLAgentInfoSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<DebugDLAgentInfoSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugDLAgentInfoSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, int>(5, value.msgType, value.hashCode, value.actorID, value.changedInfoType, value.changedInfo);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DebugDLAgentInfoSig? value)
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
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.changedInfoType;
				value6 = value.changedInfo;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				goto IL_0117;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, int>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DebugDLAgentInfoSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.changedInfoType;
				value6 = value.changedInfo;
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
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0117;
			}
		}
		value = new DebugDLAgentInfoSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			changedInfoType = value5,
			changedInfo = value6
		};
		return;
		IL_0117:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.changedInfoType = value5;
		value.changedInfo = value6;
	}
}
