using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class APIEnterLobbyLogReq : IMsg, IMemoryPackable<APIEnterLobbyLogReq>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class APIEnterLobbyLogReqFormatter : MemoryPackFormatter<APIEnterLobbyLogReq>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref APIEnterLobbyLogReq value)
		{
			APIEnterLobbyLogReq.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref APIEnterLobbyLogReq value)
		{
			APIEnterLobbyLogReq.Deserialize(ref reader, ref value);
		}
	}

	public IntegrationProviderType provider { get; set; }

	public string guid { get; set; } = string.Empty;

	public string sessionID { get; set; } = string.Empty;

	public APIEnterLobbyLogReq()
		: base(MsgType.C2A_EnterLobbyLogReq)
	{
	}

	static APIEnterLobbyLogReq()
	{
		RegisterFormatter();
	}

	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<APIEnterLobbyLogReq>())
		{
			MemoryPackFormatterProvider.Register(new APIEnterLobbyLogReqFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<APIEnterLobbyLogReq[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<APIEnterLobbyLogReq>());
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
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref APIEnterLobbyLogReq? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, IntegrationProviderType>(5, value.msgType, value.hashCode, value.provider);
		writer.WriteString(value.guid);
		writer.WriteString(value.sessionID);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref APIEnterLobbyLogReq? value)
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
		if (memberCount == 5)
		{
			if (value != null)
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.provider;
				text = value.guid;
				text2 = value.sessionID;
				reader.ReadUnmanaged<MsgType>(out value2);
				reader.ReadUnmanaged<int>(out value3);
				reader.ReadUnmanaged<IntegrationProviderType>(out value4);
				text = reader.ReadString();
				text2 = reader.ReadString();
				goto IL_0123;
			}
			reader.ReadUnmanaged<MsgType, int, IntegrationProviderType>(out value2, out value3, out value4);
			text = reader.ReadString();
			text2 = reader.ReadString();
		}
		else
		{
			if (memberCount > 5)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(APIEnterLobbyLogReq), 5, memberCount);
				return;
			}
			if (value == null)
			{
				value2 = MsgType.Invalid;
				value3 = 0;
				value4 = IntegrationProviderType.GUEST;
				text = null;
				text2 = null;
			}
			else
			{
				value2 = value.msgType;
				value3 = value.hashCode;
				value4 = value.provider;
				text = value.guid;
				text2 = value.sessionID;
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
		value = new APIEnterLobbyLogReq
		{
			msgType = value2,
			hashCode = value3,
			provider = value4,
			guid = text,
			sessionID = text2
		};
		return;
		IL_0123:
		value.msgType = value2;
		value.hashCode = value3;
		value.provider = value4;
		value.guid = text;
		value.sessionID = text2;
	}
}
