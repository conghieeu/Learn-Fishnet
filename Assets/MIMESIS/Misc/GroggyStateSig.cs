using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class GroggyStateSig : IActorMsg, IMemoryPackable<GroggyStateSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class GroggyStateSigFormatter : MemoryPackFormatter<GroggyStateSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GroggyStateSig value)
		{
			GroggyStateSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref GroggyStateSig value)
		{
			GroggyStateSig.Deserialize(ref reader, ref value);
		}
	}

	public GroggyState state { get; set; }

	public GroggyStateSig()
		: base(MsgType.C2S_GroggyStateSig)
	{
	}

	static GroggyStateSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<GroggyStateSig>())
		{
			MemoryPackFormatterProvider.Register(new GroggyStateSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<GroggyStateSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<GroggyStateSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<GroggyState>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<GroggyState>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GroggyStateSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, GroggyState>(4, value.msgType, value.hashCode, value.actorID, value.state);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref GroggyStateSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		GroggyState value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.state;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<GroggyState>(out value5);
				goto IL_00e9;
			}
			reader.ReadUnmanaged<MsgType, int, int, GroggyState>(out value2, out value3, out value4, out value5);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(GroggyStateSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = GroggyState.Normal;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.state;
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
							reader.ReadUnmanaged<GroggyState>(out value5);
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
		value = new GroggyStateSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			state = value5
		};
		return;
		IL_00e9:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.state = value5;
	}
}
