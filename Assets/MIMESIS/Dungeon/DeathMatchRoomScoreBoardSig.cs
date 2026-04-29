using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class DeathMatchRoomScoreBoardSig : IMsg, IMemoryPackable<DeathMatchRoomScoreBoardSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class DeathMatchRoomScoreBoardSigFormatter : MemoryPackFormatter<DeathMatchRoomScoreBoardSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DeathMatchRoomScoreBoardSig value)
		{
			DeathMatchRoomScoreBoardSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DeathMatchRoomScoreBoardSig value)
		{
			DeathMatchRoomScoreBoardSig.Deserialize(ref reader, ref value);
		}
	}

	public Dictionary<int, DeathMatchPlayerResult> deathMatchScordBoards { get; set; } = new Dictionary<int, DeathMatchPlayerResult>();

	public DeathMatchRoomScoreBoardSig()
		: base(MsgType.C2S_DeathMatchRoomScoreBoardSig)
	{
		base.reliable = true;
	}

	static DeathMatchRoomScoreBoardSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DeathMatchRoomScoreBoardSig>())
		{
			MemoryPackFormatterProvider.Register(new DeathMatchRoomScoreBoardSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DeathMatchRoomScoreBoardSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<DeathMatchRoomScoreBoardSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, DeathMatchPlayerResult>>())
		{
			MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, DeathMatchPlayerResult>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DeathMatchRoomScoreBoardSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
		writer.WriteValue<Dictionary<int, DeathMatchPlayerResult>>(value.deathMatchScordBoards);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DeathMatchRoomScoreBoardSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		Dictionary<int, DeathMatchPlayerResult> value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.deathMatchScordBoards;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadValue(ref value4);
				goto IL_00c3;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			value4 = reader.ReadValue<Dictionary<int, DeathMatchPlayerResult>>();
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DeathMatchRoomScoreBoardSig), 3, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.deathMatchScordBoards;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadValue(ref value4);
						_ = 3;
					}
				}
			}
			if (value != null)
			{
				goto IL_00c3;
			}
		}
		value = new DeathMatchRoomScoreBoardSig
		{
			msgType = value2,
			hashCode = value3,
			deathMatchScordBoards = value4
		};
		return;
		IL_00c3:
		value.msgType = value2;
		value.hashCode = value3;
		value.deathMatchScordBoards = value4;
	}
}
