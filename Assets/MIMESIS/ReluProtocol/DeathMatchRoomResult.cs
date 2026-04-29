using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class DeathMatchRoomResult : IMemoryPackable<DeathMatchRoomResult>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class DeathMatchRoomResultFormatter : MemoryPackFormatter<DeathMatchRoomResult>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DeathMatchRoomResult value)
			{
				DeathMatchRoomResult.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref DeathMatchRoomResult value)
			{
				DeathMatchRoomResult.Deserialize(ref reader, ref value);
			}
		}

		public Dictionary<int, DeathMatchRoomMemberResultInfo> playerStatus { get; set; } = new Dictionary<int, DeathMatchRoomMemberResultInfo>();

		static DeathMatchRoomResult()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<DeathMatchRoomResult>())
			{
				MemoryPackFormatterProvider.Register(new DeathMatchRoomResultFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<DeathMatchRoomResult[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<DeathMatchRoomResult>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, DeathMatchRoomMemberResultInfo>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, DeathMatchRoomMemberResultInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DeathMatchRoomResult? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(1);
			writer.WriteValue<Dictionary<int, DeathMatchRoomMemberResultInfo>>(value.playerStatus);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref DeathMatchRoomResult? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Dictionary<int, DeathMatchRoomMemberResultInfo> value2;
			if (memberCount == 1)
			{
				if (value != null)
				{
					value2 = value.playerStatus;
					reader.ReadValue(ref value2);
					goto IL_006a;
				}
				value2 = reader.ReadValue<Dictionary<int, DeathMatchRoomMemberResultInfo>>();
			}
			else
			{
				if (memberCount > 1)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DeathMatchRoomResult), 1, memberCount);
					return;
				}
				value2 = ((value != null) ? value.playerStatus : null);
				if (memberCount != 0)
				{
					reader.ReadValue(ref value2);
					_ = 1;
				}
				if (value != null)
				{
					goto IL_006a;
				}
			}
			value = new DeathMatchRoomResult
			{
				playerStatus = value2
			};
			return;
			IL_006a:
			value.playerStatus = value2;
		}
	}
}
