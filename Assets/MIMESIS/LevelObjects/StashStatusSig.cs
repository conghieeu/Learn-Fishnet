using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class StashStatusSig : IMsg, IMemoryPackable<StashStatusSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class StashStatusSigFormatter : MemoryPackFormatter<StashStatusSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StashStatusSig value)
		{
			StashStatusSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref StashStatusSig value)
		{
			StashStatusSig.Deserialize(ref reader, ref value);
		}
	}

	public Dictionary<int, ItemInfo> stashedItems { get; set; } = new Dictionary<int, ItemInfo>();

	public StashStatusSig()
		: base(MsgType.C2S_StashStatusSig)
	{
		base.reliable = true;
	}

	static StashStatusSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<StashStatusSig>())
		{
			MemoryPackFormatterProvider.Register(new StashStatusSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<StashStatusSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<StashStatusSig>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StashStatusSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int>(3, value.msgType, value.hashCode);
		writer.WriteValue<Dictionary<int, ItemInfo>>(value.stashedItems);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref StashStatusSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		Dictionary<int, ItemInfo> value4;
		if (memberCount == 3)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.stashedItems;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadValue(ref value4);
				goto IL_00c3;
			}
			reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
			value4 = reader.ReadValue<Dictionary<int, ItemInfo>>();
		}
		else
		{
			if (memberCount > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(StashStatusSig), 3, memberCount);
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
				value4 = value.stashedItems;
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
		value = new StashStatusSig
		{
			msgType = value2,
			hashCode = value3,
			stashedItems = value4
		};
		return;
		IL_00c3:
		value.msgType = value2;
		value.hashCode = value3;
		value.stashedItems = value4;
	}
}
