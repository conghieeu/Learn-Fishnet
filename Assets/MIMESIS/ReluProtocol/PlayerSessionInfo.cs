using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class PlayerSessionInfo : IMemoryPackable<PlayerSessionInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class PlayerSessionInfoFormatter : MemoryPackFormatter<PlayerSessionInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlayerSessionInfo value)
			{
				PlayerSessionInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PlayerSessionInfo value)
			{
				PlayerSessionInfo.Deserialize(ref reader, ref value);
			}
		}

		public bool isHost { get; set; }

		public long playerUID { get; set; }

		public ulong steamID { get; set; }

		public string guid { get; set; } = string.Empty;

		static PlayerSessionInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerSessionInfo>())
			{
				MemoryPackFormatterProvider.Register(new PlayerSessionInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerSessionInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PlayerSessionInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlayerSessionInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<bool, long, ulong>(4, value.isHost, value.playerUID, value.steamID);
			writer.WriteString(value.guid);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PlayerSessionInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			bool value2;
			long value3;
			ulong value4;
			string text;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.isHost;
					value3 = value.playerUID;
					value4 = value.steamID;
					text = value.guid;
					reader.ReadUnmanaged<bool>(out value2);
					reader.ReadUnmanaged<long>(out value3);
					reader.ReadUnmanaged<ulong>(out value4);
					text = reader.ReadString();
					goto IL_00f1;
				}
				reader.ReadUnmanaged<bool, long, ulong>(out value2, out value3, out value4);
				text = reader.ReadString();
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PlayerSessionInfo), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = false;
					value3 = 0L;
					value4 = 0uL;
					text = null;
				}
				else
				{
					value2 = value.isHost;
					value3 = value.playerUID;
					value4 = value.steamID;
					text = value.guid;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<bool>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<long>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<ulong>(out value4);
							if (memberCount != 3)
							{
								text = reader.ReadString();
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_00f1;
				}
			}
			value = new PlayerSessionInfo
			{
				isHost = value2,
				playerUID = value3,
				steamID = value4,
				guid = text
			};
			return;
			IL_00f1:
			value.isHost = value2;
			value.playerUID = value3;
			value.steamID = value4;
			value.guid = text;
		}
	}
}
