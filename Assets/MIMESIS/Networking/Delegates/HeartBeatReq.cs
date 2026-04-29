using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class HeartBeatReq : IMsg, IMemoryPackable<HeartBeatReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class HeartBeatReqFormatter : MemoryPackFormatter<HeartBeatReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref HeartBeatReq value)
		{
			HeartBeatReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref HeartBeatReq value)
		{
			HeartBeatReq.Deserialize(ref reader, ref value);
		}
	}

	public long clientSendTime { get; set; }

	public int seqID { get; set; }

	public NetworkGrade networkGrade { get; set; }

	public HeartBeatReq()
		: base(MsgType.C2G_HeartBeatReq)
	{
		base.reliable = true;
	}

	static HeartBeatReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<HeartBeatReq>())
		{
			MemoryPackFormatterProvider.Register(new HeartBeatReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HeartBeatReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<HeartBeatReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<NetworkGrade>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<NetworkGrade>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref HeartBeatReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
		}
		else
		{
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, long, int, NetworkGrade>(5, value.msgType, value.hashCode, value.clientSendTime, value.seqID, value.networkGrade);
		}
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref HeartBeatReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		long value4;
		int value5;
		NetworkGrade value6;
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.clientSendTime;
				value5 = value.seqID;
				value6 = value.networkGrade;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<long>(out value4);
				reader.ReadUnmanaged<int>(out value5);
				reader.ReadUnmanaged<NetworkGrade>(out value6);
				goto IL_0118;
			}
			reader.ReadUnmanaged<MsgType, int, long, int, NetworkGrade>(out value2, out value3, out value4, out value5, out value6);
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(HeartBeatReq), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = 0L;
				value5 = 0;
				value6 = NetworkGrade.Broken;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.clientSendTime;
				value5 = value.seqID;
				value6 = value.networkGrade;
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
							reader.ReadUnmanaged<int>(out value5);
							if (memberCount != 4)
							{
								reader.ReadUnmanaged<NetworkGrade>(out value6);
								_ = 5;
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0118;
			}
		}
		value = new HeartBeatReq
		{
			msgType = value2,
			hashCode = value3,
			clientSendTime = value4,
			seqID = value5,
			networkGrade = value6
		};
		return;
		IL_0118:
		value.msgType = value2;
		value.hashCode = value3;
		value.clientSendTime = value4;
		value.seqID = value5;
		value.networkGrade = value6;
	}
}
