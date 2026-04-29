using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class StartScrapMotionSig : IActorMsg, IMemoryPackable<StartScrapMotionSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class StartScrapMotionSigFormatter : MemoryPackFormatter<StartScrapMotionSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartScrapMotionSig value)
		{
			StartScrapMotionSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref StartScrapMotionSig value)
		{
			StartScrapMotionSig.Deserialize(ref reader, ref value);
		}
	}

	public ItemInfo onHandItem { get; set; } = new ItemInfo();

	public PosWithRot basePosition { get; set; } = new PosWithRot();

	public StartScrapMotionSig()
		: base(MsgType.C2S_StartScrapMotionSig)
	{
		base.reliable = true;
	}

	static StartScrapMotionSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<StartScrapMotionSig>())
		{
			MemoryPackFormatterProvider.Register(new StartScrapMotionSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<StartScrapMotionSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<StartScrapMotionSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StartScrapMotionSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int>(5, value.msgType, value.hashCode, value.actorID);
		writer.WritePackable<ItemInfo>(value.onHandItem);
		writer.WritePackable<PosWithRot>(value.basePosition);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref StartScrapMotionSig? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		int value4;
		ItemInfo value5;
		PosWithRot value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.onHandItem;
				value6 = value.basePosition;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<int>(out value4);
				reader.ReadPackable(ref value5);
				reader.ReadPackable(ref value6);
				goto IL_0123;
			}
			reader.ReadUnmanaged<MsgType, int, int>(out value2, out value3, out value4);
			value5 = reader.ReadPackable<ItemInfo>();
			value6 = reader.ReadPackable<PosWithRot>();
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(StartScrapMotionSig), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0;
				value5 = null;
				value6 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.actorID;
				value5 = value.onHandItem;
				value6 = value.basePosition;
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
							if (memberCount != 4)
							{
								reader.ReadPackable(ref value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0123;
			}
		}
		value = new StartScrapMotionSig
		{
			msgType = value2,
			hashCode = value3,
			actorID = value4,
			onHandItem = value5,
			basePosition = value6
		};
		return;
		IL_0123:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.onHandItem = value5;
		value.basePosition = value6;
	}
}
