using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class RoomStatusSig : IMsg, IMemoryPackable<RoomStatusSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class RoomStatusSigFormatter : MemoryPackFormatter<RoomStatusSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RoomStatusSig value)
		{
			RoomStatusSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref RoomStatusSig value)
		{
			RoomStatusSig.Deserialize(ref reader, ref value);
		}
	}

	public int currency { get; set; }

	public List<PlayerStatusInfo> playerStatusInfos { get; set; } = new List<PlayerStatusInfo>();

	public RoomStatusSig()
		: base(MsgType.C2S_RoomStatusSig)
	{
	}

	static RoomStatusSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<RoomStatusSig>())
		{
			MemoryPackFormatterProvider.Register(new RoomStatusSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<RoomStatusSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<RoomStatusSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<PlayerStatusInfo>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<PlayerStatusInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref RoomStatusSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.currency);
		ListFormatter.SerializePackable(ref writer, value.playerStatusInfos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref RoomStatusSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		List<PlayerStatusInfo> value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.currency;
				value5 = value.playerStatusInfos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				ListFormatter.DeserializePackable(ref reader, ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = ListFormatter.DeserializePackable<PlayerStatusInfo>(ref reader);
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(RoomStatusSig), 4, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.currency;
				value5 = value.playerStatusInfos;
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
							ListFormatter.DeserializePackable(ref reader, ref value5);
							_ = 4;
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_00ef;
			}
		}
		value = new RoomStatusSig
		{
			msgType = value2,
			hashCode = value3,
			currency = value4,
			playerStatusInfos = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.currency = value4;
		value.playerStatusInfos = value5;
	}
}
