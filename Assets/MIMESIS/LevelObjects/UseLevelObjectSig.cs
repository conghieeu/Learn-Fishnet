using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class UseLevelObjectSig : IMsg, IMemoryPackable<UseLevelObjectSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class UseLevelObjectSigFormatter : MemoryPackFormatter<UseLevelObjectSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseLevelObjectSig value)
		{
			UseLevelObjectSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref UseLevelObjectSig value)
		{
			UseLevelObjectSig.Deserialize(ref reader, ref value);
		}
	}

	public int actorID { get; set; }

	public LevelObjectInfo changedLevelObject { get; set; } = new LevelObjectInfo();

	public UseLevelObjectSig()
		: base(MsgType.C2S_UseLevelObjectSig)
	{
		base.reliable = true;
	}

	static UseLevelObjectSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<UseLevelObjectSig>())
		{
			MemoryPackFormatterProvider.Register(new UseLevelObjectSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<UseLevelObjectSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<UseLevelObjectSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref UseLevelObjectSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(4, value.msgType, value.hashCode, value.actorID);
		writer.WritePackable<LevelObjectInfo>(value.changedLevelObject);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref UseLevelObjectSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		LevelObjectInfo value5;
		if (memberCount == 4)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.changedLevelObject;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadPackable(ref value5);
				goto IL_00ef;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadPackable<LevelObjectInfo>();
		}
		else
		{
			if (memberCount > 4)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(UseLevelObjectSig), 4, memberCount);
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
				value5 = value.changedLevelObject;
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
							reader.ReadPackable(ref value5);
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
		value = new UseLevelObjectSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			changedLevelObject = value5
		};
		return;
		IL_00ef:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.changedLevelObject = value5;
	}
}
