using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class CancelScrapMotionSig : IActorMsg, IMemoryPackable<CancelScrapMotionSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class CancelScrapMotionSigFormatter : MemoryPackFormatter<CancelScrapMotionSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CancelScrapMotionSig value)
		{
			CancelScrapMotionSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref CancelScrapMotionSig value)
		{
			CancelScrapMotionSig.Deserialize(ref reader, ref value);
		}
	}

	public CancelScrapMotionSig()
		: base(MsgType.C2S_CancelScrapMotionSig)
	{
		base.reliable = true;
	}

	static CancelScrapMotionSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<CancelScrapMotionSig>())
		{
			MemoryPackFormatterProvider.Register(new CancelScrapMotionSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<CancelScrapMotionSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<CancelScrapMotionSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CancelScrapMotionSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(3, value.msgType, value.hashCode, value.actorID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref CancelScrapMotionSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				goto IL_00be;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CancelScrapMotionSig), 3, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
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
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00be;
			}
		}
		value = new CancelScrapMotionSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4
		};
		return;
		IL_00be:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
	}
}
