using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class GrapLootingObjectRes : IResMsg, IMemoryPackable<GrapLootingObjectRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class GrapLootingObjectResFormatter : MemoryPackFormatter<GrapLootingObjectRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GrapLootingObjectRes value)
		{
			GrapLootingObjectRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref GrapLootingObjectRes value)
		{
			GrapLootingObjectRes.Deserialize(ref reader, ref value);
		}
	}

	public Dictionary<int, ItemInfo> inventoryInfos { get; set; } = new Dictionary<int, ItemInfo>();

	public int currentInvenSlotIndex { get; set; }

	[MemoryPackConstructor]
	public GrapLootingObjectRes(int hashCode)
		: base(MsgType.C2S_GrapLootingObjectRes, hashCode)
	{
		base.reliable = true;
	}

	public GrapLootingObjectRes()
		: this(0)
	{
	}

	static GrapLootingObjectRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<GrapLootingObjectRes>())
		{
			MemoryPackFormatterProvider.Register(new GrapLootingObjectResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<GrapLootingObjectRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<GrapLootingObjectRes>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgErrorCode>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgErrorCode>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, ItemInfo>>())
		{
			MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, ItemInfo>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GrapLootingObjectRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(5, value.msgType, value.hashCode, value.errorCode);
		writer.WriteValue<Dictionary<int, ItemInfo>>(value.inventoryInfos);
		writer.WriteUnmanaged<int>(value.currentInvenSlotIndex);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref GrapLootingObjectRes? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		MsgErrorCode value4;
		Dictionary<int, ItemInfo> value5;
		int value6;
		if (memberCount == 5)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
				value5 = reader.ReadValue<Dictionary<int, ItemInfo>>();
				reader.ReadUnmanaged<int>(out value6);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.inventoryInfos;
				value6 = value.currentInvenSlotIndex;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadValue(ref value5);
				reader.ReadUnmanaged<int>(out value6);
			}
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(GrapLootingObjectRes), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = null;
				value6 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.inventoryInfos;
				value6 = value.currentInvenSlotIndex;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<MsgErrorCode>(out value4);
						if (memberCount != 3)
						{
							reader.ReadValue(ref value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<int>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			_ = value;
		}
		value = new GrapLootingObjectRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			inventoryInfos = value5,
			currentInvenSlotIndex = value6
		};
	}
}
