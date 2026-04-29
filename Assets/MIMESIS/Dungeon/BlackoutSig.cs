using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class BlackoutSig : IMsg, IMemoryPackable<BlackoutSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class BlackoutSigFormatter : MemoryPackFormatter<BlackoutSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BlackoutSig value)
		{
			BlackoutSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref BlackoutSig value)
		{
			BlackoutSig.Deserialize(ref reader, ref value);
		}
	}

	public bool isBlackout { get; set; }

	public ReasonOfBlackout reasonOfBlackout { get; set; }

	public int ownerActorID { get; set; }

	public BlackoutSig()
		: base(MsgType.C2S_BlackoutSig)
	{
	}

	static BlackoutSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<BlackoutSig>())
		{
			MemoryPackFormatterProvider.Register(new BlackoutSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<BlackoutSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<BlackoutSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ReasonOfBlackout>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ReasonOfBlackout>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref BlackoutSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, bool, ReasonOfBlackout, int>(5, value.msgType, value.hashCode, value.isBlackout, value.reasonOfBlackout, value.ownerActorID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref BlackoutSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		bool value4;
		ReasonOfBlackout value5;
		int value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.isBlackout;
				value5 = value.reasonOfBlackout;
				value6 = value.ownerActorID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<bool>(out value4);
				reader.ReadUnmanaged<ReasonOfBlackout>(out value5);
				reader.ReadUnmanaged<int>(out value6);
				goto IL_0117;
			}
			reader.ReadUnmanaged<MsgType, int, bool, ReasonOfBlackout, int>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(BlackoutSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = false;
				value5 = ReasonOfBlackout.None;
				value6 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.isBlackout;
				value5 = value.reasonOfBlackout;
				value6 = value.ownerActorID;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<bool>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<ReasonOfBlackout>(out value5);
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
		value = new BlackoutSig
		{
			msgType = value2,
			hashCode = value3,
			isBlackout = value4,
			reasonOfBlackout = value5,
			ownerActorID = value6
		};
		return;
		IL_0117:
		value.msgType = value2;
		value.hashCode = value3;
		value.isBlackout = value4;
		value.reasonOfBlackout = value5;
		value.ownerActorID = value6;
	}
}
