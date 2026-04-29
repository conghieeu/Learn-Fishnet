using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class CancelSkillSig : IActorMsg, IMemoryPackable<CancelSkillSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class CancelSkillSigFormatter : MemoryPackFormatter<CancelSkillSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CancelSkillSig value)
		{
			CancelSkillSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref CancelSkillSig value)
		{
			CancelSkillSig.Deserialize(ref reader, ref value);
		}
	}

	public long syncID { get; set; }

	public CancelSkillSig()
		: base(MsgType.C2S_CancelSkillSig)
	{
		base.reliable = true;
	}

	static CancelSkillSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<CancelSkillSig>())
		{
			MemoryPackFormatterProvider.Register(new CancelSkillSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<CancelSkillSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<CancelSkillSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CancelSkillSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, long>(4, value.msgType, value.hashCode, value.actorID, value.syncID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref CancelSkillSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		long value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.syncID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<long>(out value5);
				goto IL_00ea;
			}
			reader.ReadUnmanaged<MsgType, int, int, long>(out value2, out value3, out value4, out value5);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CancelSkillSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0L;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.syncID;
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
							reader.ReadUnmanaged<long>(out value5);
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00ea;
			}
		}
		value = new CancelSkillSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			syncID = value5
		};
		return;
		IL_00ea:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.syncID = value5;
	}
}
