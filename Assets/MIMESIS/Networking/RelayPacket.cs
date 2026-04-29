using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class RelayPacket : IMsg, IMemoryPackable<RelayPacket>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class RelayPacketFormatter : MemoryPackFormatter<RelayPacket>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RelayPacket value)
		{
			RelayPacket.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref RelayPacket value)
		{
			RelayPacket.Deserialize(ref reader, ref value);
		}
	}

	public string relayBodyJson { get; set; }

	public RelayPacket()
		: base(MsgType.C2S_RelayPacket)
	{
	}

	static RelayPacket()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<RelayPacket>())
		{
			MemoryPackFormatterProvider.Register(new RelayPacketFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<RelayPacket[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<RelayPacket>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RelayPacket? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
		writer.WriteString(value.relayBodyJson);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref RelayPacket? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		string text;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				text = value.relayBodyJson;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				text = reader.ReadString();
				goto IL_00c1;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			text = reader.ReadString();
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RelayPacket), 3, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				text = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				text = value.relayBodyJson;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						text = reader.ReadString();
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00c1;
			}
		}
		value = new RelayPacket
		{
			msgType = value2,
			hashCode = value3,
			relayBodyJson = text
		};
		return;
		IL_00c1:
		value.msgType = value2;
		value.hashCode = value3;
		value.relayBodyJson = text;
	}
}
