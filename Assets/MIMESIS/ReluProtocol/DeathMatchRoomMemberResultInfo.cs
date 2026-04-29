using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class DeathMatchRoomMemberResultInfo : IMemoryPackable<DeathMatchRoomMemberResultInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class DeathMatchRoomMemberResultInfoFormatter : MemoryPackFormatter<DeathMatchRoomMemberResultInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DeathMatchRoomMemberResultInfo value)
			{
				DeathMatchRoomMemberResultInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref DeathMatchRoomMemberResultInfo value)
			{
				DeathMatchRoomMemberResultInfo.Deserialize(ref reader, ref value);
			}
		}

		public int actorID { get; set; }

		public int killCount { get; set; }

		public ReasonOfDeath ReasonOfDeath { get; set; }

		public int attackerActorID { get; set; }

		static DeathMatchRoomMemberResultInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<DeathMatchRoomMemberResultInfo>())
			{
				MemoryPackFormatterProvider.Register(new DeathMatchRoomMemberResultInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<DeathMatchRoomMemberResultInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<DeathMatchRoomMemberResultInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReasonOfDeath>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ReasonOfDeath>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DeathMatchRoomMemberResultInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<int, int, ReasonOfDeath, int>(4, value.actorID, value.killCount, value.ReasonOfDeath, value.attackerActorID);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref DeathMatchRoomMemberResultInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			int value3;
			ReasonOfDeath value4;
			int value5;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.actorID;
					value3 = value.killCount;
					value4 = value.ReasonOfDeath;
					value5 = value.attackerActorID;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<ReasonOfDeath>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					goto IL_00e9;
				}
				reader.ReadUnmanaged<int, int, ReasonOfDeath, int>(out value2, out value3, out value4, out value5);
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DeathMatchRoomMemberResultInfo), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = ReasonOfDeath.None;
					value5 = 0;
				}
				else
				{
					value2 = value.actorID;
					value3 = value.killCount;
					value4 = value.ReasonOfDeath;
					value5 = value.attackerActorID;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<ReasonOfDeath>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<int>(out value5);
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_00e9;
				}
			}
			value = new DeathMatchRoomMemberResultInfo
			{
				actorID = value2,
				killCount = value3,
				ReasonOfDeath = value4,
				attackerActorID = value5
			};
			return;
			IL_00e9:
			value.actorID = value2;
			value.killCount = value3;
			value.ReasonOfDeath = value4;
			value.attackerActorID = value5;
		}
	}
}
