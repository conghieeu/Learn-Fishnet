using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class TramStatusSig : IMsg, IMemoryPackable<TramStatusSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class TramStatusSigFormatter : MemoryPackFormatter<TramStatusSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TramStatusSig value)
		{
			TramStatusSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref TramStatusSig value)
		{
			TramStatusSig.Deserialize(ref reader, ref value);
		}
	}

	public int triggeredActorID { get; set; }

	public bool restored { get; set; }

	public TramStatus status { get; set; }

	public TramStatusSig()
		: base(MsgType.C2S_TramStatusSig)
	{
		base.reliable = true;
	}

	static TramStatusSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<TramStatusSig>())
		{
			MemoryPackFormatterProvider.Register(new TramStatusSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<TramStatusSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<TramStatusSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<TramStatus>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<TramStatus>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TramStatusSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, bool, TramStatus>(5, value.msgType, value.hashCode, value.triggeredActorID, value.restored, value.status);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref TramStatusSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		bool value5;
		TramStatus value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.triggeredActorID;
				value5 = value.restored;
				value6 = value.status;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<bool>(out value5);
				reader.ReadUnmanaged<TramStatus>(out value6);
				goto IL_0117;
			}
			reader.ReadUnmanaged<MsgType, int, int, bool, TramStatus>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(TramStatusSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = false;
				value6 = TramStatus.NotStarted;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.triggeredActorID;
				value5 = value.restored;
				value6 = value.status;
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
							reader.ReadUnmanaged<bool>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<TramStatus>(out value6);
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
		value = new TramStatusSig
		{
			msgType = value2,
			hashCode = value3,
			triggeredActorID = value4,
			restored = value5,
			status = value6
		};
		return;
		IL_0117:
		value.msgType = value2;
		value.hashCode = value3;
		value.triggeredActorID = value4;
		value.restored = value5;
		value.status = value6;
	}
}
