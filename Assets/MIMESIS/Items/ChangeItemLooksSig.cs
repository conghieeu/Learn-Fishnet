using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ChangeItemLooksSig : IActorMsg, IMemoryPackable<ChangeItemLooksSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ChangeItemLooksSigFormatter : MemoryPackFormatter<ChangeItemLooksSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeItemLooksSig value)
		{
			ChangeItemLooksSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ChangeItemLooksSig value)
		{
			ChangeItemLooksSig.Deserialize(ref reader, ref value);
		}
	}

	public int slotIndex { get; set; }

	public ItemInfo onHandItem { get; set; } = new ItemInfo();

	public long activateTime { get; set; }

	public ChangeItemLooksSig()
		: base(MsgType.C2S_ChangeItemLooksSig)
	{
		base.reliable = true;
	}

	static ChangeItemLooksSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeItemLooksSig>())
		{
			MemoryPackFormatterProvider.Register(new ChangeItemLooksSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ChangeItemLooksSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ChangeItemLooksSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ChangeItemLooksSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int>(6, value.msgType, value.hashCode, value.actorID, value.slotIndex);
		writer.WritePackable<ItemInfo>(value.onHandItem);
		writer.WriteUnmanaged<long>(value.activateTime);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ChangeItemLooksSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		int value5;
		ItemInfo value6;
		long value7;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.slotIndex;
				value6 = value.onHandItem;
				value7 = value.activateTime;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadPackable(ref value6);
				reader.ReadUnmanaged<long>(out value7);
				goto IL_0152;
			}
			reader.ReadUnmanaged<MsgType, int, int, int>(out value2, out value3, out value4, out value5);
			value6 = reader.ReadPackable<ItemInfo>();
			reader.ReadUnmanaged<long>(out value7);
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ChangeItemLooksSig), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = null;
				value7 = 0L;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.slotIndex;
				value6 = value.onHandItem;
				value7 = value.activateTime;
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
							reader.ReadUnmanaged<int>(out value5);
							if (memberCount != 4)
							{
								reader.ReadPackable(ref value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<long>(out value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0152;
			}
		}
		value = new ChangeItemLooksSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			slotIndex = value5,
			onHandItem = value6,
			activateTime = value7
		};
		return;
		IL_0152:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.slotIndex = value5;
		value.onHandItem = value6;
		value.activateTime = value7;
	}
}
