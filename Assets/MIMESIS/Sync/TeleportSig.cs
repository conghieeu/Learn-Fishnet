using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class TeleportSig : IMsg, IMemoryPackable<TeleportSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class TeleportSigFormatter : MemoryPackFormatter<TeleportSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TeleportSig value)
		{
			TeleportSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref TeleportSig value)
		{
			TeleportSig.Deserialize(ref reader, ref value);
		}
	}

	public int actorID { get; set; }

	public PosWithRot pos { get; set; }

	public TeleportReason reason { get; set; }

	public TeleportSig()
		: base(MsgType.C2S_TeleportSig)
	{
		base.reliable = true;
	}

	static TeleportSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<TeleportSig>())
		{
			MemoryPackFormatterProvider.Register(new TeleportSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<TeleportSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<TeleportSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<TeleportReason>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<TeleportReason>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TeleportSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(5, value.msgType, value.hashCode, value.actorID);
		writer.WritePackable<PosWithRot>(value.pos);
		writer.WriteUnmanaged<TeleportReason>(value.reason);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref TeleportSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		PosWithRot value5;
		TeleportReason value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.pos;
				value6 = value.reason;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadPackable(ref value5);
				reader.ReadUnmanaged<TeleportReason>(out value6);
				goto IL_0123;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadPackable<PosWithRot>();
			reader.ReadUnmanaged<TeleportReason>(out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(TeleportSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = null;
				value6 = TeleportReason.None;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.pos;
				value6 = value.reason;
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
							reader.ReadPackable(ref value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<TeleportReason>(out value6);
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
		value = new TeleportSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			pos = value5,
			reason = value6
		};
		return;
		IL_0123:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.pos = value5;
		value.reason = value6;
	}
}
