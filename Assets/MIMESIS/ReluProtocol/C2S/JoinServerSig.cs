using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class JoinServerSig : IMsg, IMemoryPackable<JoinServerSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class JoinServerSigFormatter : MemoryPackFormatter<JoinServerSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref JoinServerSig value)
			{
				JoinServerSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref JoinServerSig value)
			{
				JoinServerSig.Deserialize(ref reader, ref value);
			}
		}

		public long playerUID { get; set; }

		public ulong steamID { get; set; }

		public string nickName { get; set; } = string.Empty;

		public string voiceUID { get; set; } = string.Empty;

		public JoinServerSig()
			: base(MsgType.C2S_JoinServerSig)
		{
			base.reliable = true;
		}

		static JoinServerSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<JoinServerSig>())
			{
				MemoryPackFormatterProvider.Register(new JoinServerSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<JoinServerSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<JoinServerSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref JoinServerSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, long, ulong>(6, value.msgType, value.hashCode, value.playerUID, value.steamID);
			writer.WriteString(value.nickName);
			writer.WriteString(value.voiceUID);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref JoinServerSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			long value4;
			ulong value5;
			string text;
			string text2;
			if (memberCount == 6)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.playerUID;
					value5 = value.steamID;
					text = value.nickName;
					text2 = value.voiceUID;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					reader.ReadUnmanaged<ulong>(out value5);
					text = reader.ReadString();
					text2 = reader.ReadString();
					goto IL_0153;
				}
				reader.ReadUnmanaged<MsgType, int, long, ulong>(out value2, out value3, out value4, out value5);
				text = reader.ReadString();
				text2 = reader.ReadString();
			}
			else
			{
				if (memberCount > 6)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(JoinServerSig), 6, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = 0L;
					value5 = 0uL;
					text = null;
					text2 = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.playerUID;
					value5 = value.steamID;
					text = value.nickName;
					text2 = value.voiceUID;
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
								reader.ReadUnmanaged<ulong>(out value5);
								if (memberCount != 4)
								{
									text = reader.ReadString();
									if (memberCount != 5)
									{
										text2 = reader.ReadString();
										_ = 6;
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0153;
				}
			}
			value = new JoinServerSig
			{
				msgType = value2,
				hashCode = value3,
				playerUID = value4,
				steamID = value5,
				nickName = text,
				voiceUID = text2
			};
			return;
			IL_0153:
			value.msgType = value2;
			value.hashCode = value3;
			value.playerUID = value4;
			value.steamID = value5;
			value.nickName = text;
			value.voiceUID = text2;
		}
	}
}
