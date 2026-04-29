using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class GameRoomResult : IMemoryPackable<GameRoomResult>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class GameRoomResultFormatter : MemoryPackFormatter<GameRoomResult>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GameRoomResult value)
			{
				GameRoomResult.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref GameRoomResult value)
			{
				GameRoomResult.Deserialize(ref reader, ref value);
			}
		}

		public bool success { get; set; }

		public Dictionary<int, (PlayerResultStatus resultStatus, AwardType awardType)> playerStatus { get; set; } = new Dictionary<int, (PlayerResultStatus, AwardType)>();

		public List<ItemInfo> removedItems { get; set; } = new List<ItemInfo>();

		static GameRoomResult()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<GameRoomResult>())
			{
				MemoryPackFormatterProvider.Register(new GameRoomResultFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<GameRoomResult[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<GameRoomResult>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, (PlayerResultStatus, AwardType)>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, (PlayerResultStatus, AwardType)>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<(PlayerResultStatus, AwardType)>())
			{
				MemoryPackFormatterProvider.Register(new ValueTupleFormatter<PlayerResultStatus, AwardType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerResultStatus>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<PlayerResultStatus>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<AwardType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<AwardType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<ItemInfo>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<ItemInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref GameRoomResult? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<bool>(3, value.success);
			writer.WriteValue<Dictionary<int, (PlayerResultStatus, AwardType)>>(value.playerStatus);
			ListFormatter.SerializePackable(ref writer, value.removedItems);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref GameRoomResult? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			bool value2;
			Dictionary<int, (PlayerResultStatus, AwardType)> value3;
			List<ItemInfo> value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.success;
					value3 = value.playerStatus;
					value4 = value.removedItems;
					reader.ReadUnmanaged<bool>(out value2);
					reader.ReadValue(ref value3);
					ListFormatter.DeserializePackable(ref reader, ref value4);
					goto IL_00c8;
				}
				reader.ReadUnmanaged<bool>(out value2);
				value3 = reader.ReadValue<Dictionary<int, (PlayerResultStatus, AwardType)>>();
				value4 = ListFormatter.DeserializePackable<ItemInfo>(ref reader);
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(GameRoomResult), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = false;
					value3 = null;
					value4 = null;
				}
				else
				{
					value2 = value.success;
					value3 = value.playerStatus;
					value4 = value.removedItems;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<bool>(out value2);
					if (memberCount != 1)
					{
						reader.ReadValue(ref value3);
						if (memberCount != 2)
						{
							ListFormatter.DeserializePackable(ref reader, ref value4);
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00c8;
				}
			}
			value = new GameRoomResult
			{
				success = value2,
				playerStatus = value3,
				removedItems = value4
			};
			return;
			IL_00c8:
			value.success = value2;
			value.playerStatus = value3;
			value.removedItems = value4;
		}
	}
}
