using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class APIOpenPublicTramLogReq : IMsg, IMemoryPackable<APIOpenPublicTramLogReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class APIOpenPublicTramLogReqFormatter : MemoryPackFormatter<APIOpenPublicTramLogReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref APIOpenPublicTramLogReq value)
		{
			APIOpenPublicTramLogReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref APIOpenPublicTramLogReq value)
		{
			APIOpenPublicTramLogReq.Deserialize(ref reader, ref value);
		}
	}

	public IntegrationProviderType provider { get; set; }

	public string guid { get; set; } = string.Empty;

	public string sessionID { get; set; } = string.Empty;

	public string roomSessionID { get; set; } = string.Empty;

	public string appVersion { get; set; } = string.Empty;

	public APIOpenPublicTramLogReq()
		: base(MsgType.C2A_OpenPublicTramLogReq)
	{
	}

	static APIOpenPublicTramLogReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<APIOpenPublicTramLogReq>())
		{
			MemoryPackFormatterProvider.Register(new APIOpenPublicTramLogReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<APIOpenPublicTramLogReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<APIOpenPublicTramLogReq>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref APIOpenPublicTramLogReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, IntegrationProviderType>(7, value.msgType, value.hashCode, value.provider);
		writer.WriteString(value.guid);
		writer.WriteString(value.sessionID);
		writer.WriteString(value.roomSessionID);
		writer.WriteString(value.appVersion);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref APIOpenPublicTramLogReq? value)
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
		string text4;
		if (memberCount == 7)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.provider;
				text = value.guid;
				text2 = value.sessionID;
				text3 = value.roomSessionID;
				text4 = value.appVersion;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<IntegrationProviderType>(out value4);
				text = reader.ReadString();
				text2 = reader.ReadString();
				text3 = reader.ReadString();
				text4 = reader.ReadString();
				goto IL_0188;
			}
			reader.ReadUnmanaged<MsgType, int, IntegrationProviderType>(out value2, out value3, out value4);
			text = reader.ReadString();
			text2 = reader.ReadString();
			text3 = reader.ReadString();
			text4 = reader.ReadString();
		}
		else
		{
			if (memberCount > 7)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(APIOpenPublicTramLogReq), 7, memberCount);
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
				text4 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.provider;
				text = value.guid;
				text2 = value.sessionID;
				text3 = value.roomSessionID;
				text4 = value.appVersion;
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
									if (memberCount != 6)
									{
										text4 = reader.ReadString();
										_ = 7;
									}
								}
							}
						}
					}
				}
			}
			if (value != null)
			{
				goto IL_0188;
			}
		}
		value = new APIOpenPublicTramLogReq
		{
			msgType = value2,
			hashCode = value3,
			provider = value4,
			guid = text,
			sessionID = text2,
			roomSessionID = text3,
			appVersion = text4
		};
		return;
		IL_0188:
		value.msgType = value2;
		value.hashCode = value3;
		value.provider = value4;
		value.guid = text;
		value.sessionID = text2;
		value.roomSessionID = text3;
		value.appVersion = text4;
	}
}
