using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class HeartBeatRes : IResMsg, IMemoryPackable<HeartBeatRes>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class HeartBeatResFormatter : MemoryPackFormatter<HeartBeatRes>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref HeartBeatRes value)
		{
			HeartBeatRes.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref HeartBeatRes value)
		{
			HeartBeatRes.Deserialize(ref reader, ref value);
		}
	}

	public long clientSendTime { get; set; }

	public int seqID { get; set; }

	[MemoryPackConstructor]
	public HeartBeatRes(int hashCode)
		: base(MsgType.C2G_HeartBeatRes, hashCode)
	{
		base.reliable = true;
	}

	public HeartBeatRes()
		: this(0)
	{
	}

	static HeartBeatRes()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<HeartBeatRes>())
		{
			MemoryPackFormatterProvider.Register(new HeartBeatResFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HeartBeatRes[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<HeartBeatRes>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgErrorCode>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgErrorCode>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref HeartBeatRes? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode, long, int>(5, value.msgType, value.hashCode, value.errorCode, value.clientSendTime, value.seqID);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref HeartBeatRes? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		MsgErrorCode value4;
		long value5;
		int value6;
		if (memberCount == 5)
		{
			if (value == null)
			{
				reader.ReadUnmanaged<MsgType, int, MsgErrorCode, long, int>(out value2, out value3, out value4, out value5, out value6);
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.clientSendTime;
				value6 = value.seqID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<MsgErrorCode>(out value4);
				reader.ReadUnmanaged<long>(out value5);
				reader.ReadUnmanaged<int>(out value6);
			}
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(HeartBeatRes), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = MsgErrorCode.Success;
				value5 = 0L;
				value6 = 0;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.errorCode;
				value5 = value.clientSendTime;
				value6 = value.seqID;
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
							reader.ReadUnmanaged<long>(out value5);
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
		value = new HeartBeatRes(value3)
		{
			msgType = value2,
			errorCode = value4,
			clientSendTime = value5,
			seqID = value6
		};
	}
}
