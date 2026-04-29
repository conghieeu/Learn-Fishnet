using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class SaveGameDataReq : IMsg, IMemoryPackable<SaveGameDataReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class SaveGameDataReqFormatter : MemoryPackFormatter<SaveGameDataReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SaveGameDataReq value)
		{
			SaveGameDataReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref SaveGameDataReq value)
		{
			SaveGameDataReq.Deserialize(ref reader, ref value);
		}
	}

	public int SlotID { get; set; }

	public List<string> PlayerNames { get; set; }

	public SaveGameDataReq()
		: base(MsgType.C2S_SaveGameDataReq)
	{
		base.reliable = true;
	}

	static SaveGameDataReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<SaveGameDataReq>())
		{
			MemoryPackFormatterProvider.Register(new SaveGameDataReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<SaveGameDataReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<SaveGameDataReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<string>>())
		{
			MemoryPackFormatterProvider.Register(new ListFormatter<string>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SaveGameDataReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.SlotID);
		writer.WriteValue<List<string>>(value.PlayerNames);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref SaveGameDataReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		List<string> value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.SlotID;
				value5 = value.PlayerNames;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadValue(ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadValue<List<string>>();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SaveGameDataReq), 4, memberCount);
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
				value4 = value.SlotID;
				value5 = value.PlayerNames;
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
							reader.ReadValue(ref value5);
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
		value = new SaveGameDataReq
		{
			msgType = value2,
			hashCode = value3,
			SlotID = value4,
			PlayerNames = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.SlotID = value4;
		value.PlayerNames = value5;
	}
}
