using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class JoinServerReq : IMsg, IMemoryPackable<JoinServerReq>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class JoinServerReqFormatter : MemoryPackFormatter<JoinServerReq>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref JoinServerReq value)
			{
				JoinServerReq.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref JoinServerReq value)
			{
				JoinServerReq.Deserialize(ref reader, ref value);
			}
		}

		public string guid { get; set; } = string.Empty;

		public ulong steamID { get; set; }

		public string nickName { get; set; } = string.Empty;

		public string voiceUID { get; set; } = string.Empty;

		public bool isHost { get; set; }

		public string clientVersion { get; set; } = string.Empty;

		public JoinServerReq()
			: base(MsgType.C2S_JoinServerReq)
		{
			base.reliable = true;
		}

		static JoinServerReq()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<JoinServerReq>())
			{
				MemoryPackFormatterProvider.Register(new JoinServerReqFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<JoinServerReq[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<JoinServerReq>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref JoinServerReq? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int>(8, value.msgType, value.hashCode);
			writer.WriteString(value.guid);
			writer.WriteUnmanaged<ulong>(value.steamID);
			writer.WriteString(value.nickName);
			writer.WriteString(value.voiceUID);
			writer.WriteUnmanaged<bool>(value.isHost);
			writer.WriteString(value.clientVersion);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref JoinServerReq? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			ulong value4;
			bool value5;
			string text;
			string text2;
			string text3;
			string text4;
			if (memberCount == 8)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					text = value.guid;
					value4 = value.steamID;
					text2 = value.nickName;
					text3 = value.voiceUID;
					value5 = value.isHost;
					text4 = value.clientVersion;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					text = reader.ReadString();
					reader.ReadUnmanaged<ulong>(out value4);
					text2 = reader.ReadString();
					text3 = reader.ReadString();
					reader.ReadUnmanaged<bool>(out value5);
					text4 = reader.ReadString();
					goto IL_01bd;
				}
				reader.ReadUnmanaged<MsgType, int>(out value2, out value3);
				text = reader.ReadString();
				reader.ReadUnmanaged<ulong>(out value4);
				text2 = reader.ReadString();
				text3 = reader.ReadString();
				reader.ReadUnmanaged<bool>(out value5);
				text4 = reader.ReadString();
			}
			else
			{
				if (memberCount > 8)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(JoinServerReq), 8, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					text = null;
					value4 = 0uL;
					text2 = null;
					text3 = null;
					value5 = false;
					text4 = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					text = value.guid;
					value4 = value.steamID;
					text2 = value.nickName;
					text3 = value.voiceUID;
					value5 = value.isHost;
					text4 = value.clientVersion;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							text = reader.ReadString();
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<ulong>(out value4);
								if (memberCount != 4)
								{
									text2 = reader.ReadString();
									if (memberCount != 5)
									{
										text3 = reader.ReadString();
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<bool>(out value5);
											if (memberCount != 7)
											{
												text4 = reader.ReadString();
												_ = 8;
											}
										}
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_01bd;
				}
			}
			value = new JoinServerReq
			{
				msgType = value2,
				hashCode = value3,
				guid = text,
				steamID = value4,
				nickName = text2,
				voiceUID = text3,
				isHost = value5,
				clientVersion = text4
			};
			return;
			IL_01bd:
			value.msgType = value2;
			value.hashCode = value3;
			value.guid = text;
			value.steamID = value4;
			value.nickName = text2;
			value.voiceUID = text3;
			value.isHost = value5;
			value.clientVersion = text4;
		}
	}
}
