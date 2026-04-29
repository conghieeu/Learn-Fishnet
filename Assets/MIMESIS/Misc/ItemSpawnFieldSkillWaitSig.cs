using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class ItemSpawnFieldSkillWaitSig : IMsg, IMemoryPackable<ItemSpawnFieldSkillWaitSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class ItemSpawnFieldSkillWaitSigFormatter : MemoryPackFormatter<ItemSpawnFieldSkillWaitSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ItemSpawnFieldSkillWaitSig value)
		{
			ItemSpawnFieldSkillWaitSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref ItemSpawnFieldSkillWaitSig value)
		{
			ItemSpawnFieldSkillWaitSig.Deserialize(ref reader, ref value);
		}
	}

	public long itemID { get; set; }

	public long itemMasterID { get; set; }

	public long actorID { get; set; }

	public bool waitEvent { get; set; }

	public ItemSpawnFieldSkillWaitSig()
		: base(MsgType.C2S_ItemSpawnFieldSkillWaitSig)
	{
	}

	static ItemSpawnFieldSkillWaitSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<ItemSpawnFieldSkillWaitSig>())
		{
			MemoryPackFormatterProvider.Register(new ItemSpawnFieldSkillWaitSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<ItemSpawnFieldSkillWaitSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<ItemSpawnFieldSkillWaitSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ItemSpawnFieldSkillWaitSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, long, long, long, bool>(6, value.msgType, value.hashCode, value.itemID, value.itemMasterID, value.actorID, value.waitEvent);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref ItemSpawnFieldSkillWaitSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		long value4;
		long value5;
		long value6;
		bool value7;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.itemID;
				value5 = value.itemMasterID;
				value6 = value.actorID;
				value7 = value.waitEvent;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<long>(out value4);
				reader.ReadUnmanaged<long>(out value5);
				reader.ReadUnmanaged<long>(out value6);
				reader.ReadUnmanaged<bool>(out value7);
				goto IL_0148;
			}
			reader.ReadUnmanaged<MsgType, int, long, long, long, bool>(out value2, out value3, out value4, out value5, out value6, out value7);
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ItemSpawnFieldSkillWaitSig), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0L;
				value5 = 0L;
				value6 = 0L;
				value7 = false;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.itemID;
				value5 = value.itemMasterID;
				value6 = value.actorID;
				value7 = value.waitEvent;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<long>(out value4);
						if (memberCount != 3)
						{
							reader.ReadUnmanaged<long>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<long>(out value6);
								if (memberCount != 5)
								{
									reader.ReadUnmanaged<bool>(out value7);
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0148;
			}
		}
		value = new ItemSpawnFieldSkillWaitSig
		{
			msgType = value2,
			hashCode = value3,
			itemID = value4,
			itemMasterID = value5,
			actorID = value6,
			waitEvent = value7
		};
		return;
		IL_0148:
		value.msgType = value2;
		value.hashCode = value3;
		value.itemID = value4;
		value.itemMasterID = value5;
		value.actorID = value6;
		value.waitEvent = value7;
	}
}
