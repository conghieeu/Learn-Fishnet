using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class UseLevelObjectReq : IMsg, IMemoryPackable<UseLevelObjectReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class UseLevelObjectReqFormatter : MemoryPackFormatter<UseLevelObjectReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseLevelObjectReq value)
		{
			UseLevelObjectReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref UseLevelObjectReq value)
		{
			UseLevelObjectReq.Deserialize(ref reader, ref value);
		}
	}

	public int levelObjectID { get; set; }

	public int state { get; set; }

	public bool occupy { get; set; }

	public UseLevelObjectReq()
		: base(MsgType.C2S_UseLevelObjectReq)
	{
		base.reliable = true;
	}

	static UseLevelObjectReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<UseLevelObjectReq>())
		{
			MemoryPackFormatterProvider.Register(new UseLevelObjectReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<UseLevelObjectReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<UseLevelObjectReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseLevelObjectReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, int, bool>(5, value.msgType, value.hashCode, value.levelObjectID, value.state, value.occupy);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref UseLevelObjectReq? value)
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
		bool value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.levelObjectID;
				value5 = value.state;
				value6 = value.occupy;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<bool>(out value6);
				goto IL_0117;
			}
			reader.ReadUnmanaged<MsgType, int, int, int, bool>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(UseLevelObjectReq), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = 0;
				value6 = false;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.levelObjectID;
				value5 = value.state;
				value6 = value.occupy;
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
								reader.ReadUnmanaged<bool>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0117;
			}
		}
		value = new UseLevelObjectReq
		{
			msgType = value2,
			hashCode = value3,
			levelObjectID = value4,
			state = value5,
			occupy = value6
		};
		return;
		IL_0117:
		value.msgType = value2;
		value.hashCode = value3;
		value.levelObjectID = value4;
		value.state = value5;
		value.occupy = value6;
	}
}
