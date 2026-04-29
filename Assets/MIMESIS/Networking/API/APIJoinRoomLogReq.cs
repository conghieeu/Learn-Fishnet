using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class APIJoinRoomLogReq : IMsg, IMemoryPackable<APIJoinRoomLogReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class APIJoinRoomLogReqFormatter : MemoryPackFormatter<APIJoinRoomLogReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref APIJoinRoomLogReq value)
		{
			APIJoinRoomLogReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref APIJoinRoomLogReq value)
		{
			APIJoinRoomLogReq.Deserialize(ref reader, ref value);
		}
	}

	public IntegrationProviderType provider { get; set; }

	public string guid { get; set; } = string.Empty;

	public string sessionID { get; set; } = string.Empty;

	public string roomSessionID { get; set; } = string.Empty;

	public APIJoinRoomLogReq()
		: base(MsgType.C2A_JoinRoomLogReq)
	{
	}

	static APIJoinRoomLogReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<APIJoinRoomLogReq>())
		{
			MemoryPackFormatterProvider.Register(new APIJoinRoomLogReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<APIJoinRoomLogReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<APIJoinRoomLogReq>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<IntegrationProviderType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<IntegrationProviderType>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref APIJoinRoomLogReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, IntegrationProviderType>(6, value.msgType, value.hashCode, value.provider);
		writer.WriteString(value.guid);
		writer.WriteString(value.sessionID);
		writer.WriteString(value.roomSessionID);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref APIJoinRoomLogReq? value)
	{
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
			return;
		}
		MsgType value2;
		int value3;
		IntegrationProviderType value4;
		string text;
		string text2;
		string text3;
		if (memberCount == 6)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.provider;
				text = value.guid;
				text2 = value.sessionID;
				text3 = value.roomSessionID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<IntegrationProviderType>(out value4);
				text = reader.ReadString();
				text2 = reader.ReadString();
				text3 = reader.ReadString();
				goto IL_0157;
			}
			reader.ReadUnmanaged<MsgType, int, IntegrationProviderType>(out value2, out value3, out value4);
			text = reader.ReadString();
			text2 = reader.ReadString();
			text3 = reader.ReadString();
		}
		else
		{
			if (memberCount > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(APIJoinRoomLogReq), 6, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = IntegrationProviderType.GUEST;
				text = null;
				text2 = null;
				text3 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.provider;
				text = value.guid;
				text2 = value.sessionID;
				text3 = value.roomSessionID;
			}
			if (memberCount != 0)
			{
				reader.ReadUnmanaged<MsgType>(out value2);
				if (memberCount != 1)
				{
					reader.ReadUnmanaged<int>(out value3);
					if (memberCount != 2)
					{
						reader.ReadUnmanaged<IntegrationProviderType>(out value4);
						if (memberCount != 3)
						{
							text = reader.ReadString();
							if (memberCount != 4)
							{
								text2 = reader.ReadString();
								if (memberCount != 5)
								{
									text3 = reader.ReadString();
									_ = 6;
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0157;
			}
		}
		value = new APIJoinRoomLogReq
		{
			msgType = value2,
			hashCode = value3,
			provider = value4,
			guid = text,
			sessionID = text2,
			roomSessionID = text3
		};
		return;
		IL_0157:
		value.msgType = value2;
		value.hashCode = value3;
		value.provider = value4;
		value.guid = text;
		value.sessionID = text2;
		value.roomSessionID = text3;
	}
}
