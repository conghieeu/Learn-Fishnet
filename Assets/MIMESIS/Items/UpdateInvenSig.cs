using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class UpdateInvenSig : IActorMsg, IMemoryPackable<UpdateInvenSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class UpdateInvenSigFormatter : MemoryPackFormatter<UpdateInvenSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UpdateInvenSig value)
		{
			UpdateInvenSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref UpdateInvenSig value)
		{
			UpdateInvenSig.Deserialize(ref reader, ref value);
		}
	}

	public Dictionary<int, ItemInfo> inventoryInfos { get; set; } = new Dictionary<int, ItemInfo>();

	public UpdateInvenSig()
		: base(MsgType.C2S_UpdateInvenSig)
	{
		base.reliable = true;
	}

	static UpdateInvenSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<UpdateInvenSig>())
		{
			MemoryPackFormatterProvider.Register(new UpdateInvenSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<UpdateInvenSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<UpdateInvenSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, ItemInfo>>())
		{
			MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, ItemInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UpdateInvenSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.actorID);
		writer.WriteValue<Dictionary<int, ItemInfo>>(value.inventoryInfos);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref UpdateInvenSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		Dictionary<int, ItemInfo> value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.inventoryInfos;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadValue(ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadValue<Dictionary<int, ItemInfo>>();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(UpdateInvenSig), 4, memberCount);
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
				value4 = value.actorID;
				value5 = value.inventoryInfos;
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
		value = new UpdateInvenSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			inventoryInfos = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.inventoryInfos = value5;
	}
}
