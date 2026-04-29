using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class DeathMatchPlayerResult : IMemoryPackable<DeathMatchPlayerResult>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class DeathMatchPlayerResultFormatter : MemoryPackFormatter<DeathMatchPlayerResult>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DeathMatchPlayerResult value)
			{
				DeathMatchPlayerResult.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref DeathMatchPlayerResult value)
			{
				DeathMatchPlayerResult.Deserialize(ref reader, ref value);
			}
		}

		public int actorID { get; set; }

		public int killCount { get; set; }

		public (ActorType actorType, int actorID, int masterID) reasonofDead { get; set; }

		public long lastKillTime { get; set; }

		static DeathMatchPlayerResult()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<DeathMatchPlayerResult>())
			{
				MemoryPackFormatterProvider.Register(new DeathMatchPlayerResultFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<DeathMatchPlayerResult[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<DeathMatchPlayerResult>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<(ActorType, int, int)>())
			{
				MemoryPackFormatterProvider.Register(new ValueTupleFormatter<ActorType, int, int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ActorType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ActorType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DeathMatchPlayerResult? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<int, int, (ActorType, int, int), long>((byte)4, in ILSpyHelper_AsRefReadOnly(value.actorID), in ILSpyHelper_AsRefReadOnly(value.killCount), in ILSpyHelper_AsRefReadOnly(value.reasonofDead), in ILSpyHelper_AsRefReadOnly(value.lastKillTime));
			}
			static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
			{
				//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
				return ref temp;
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref DeathMatchPlayerResult? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			int value3;
			(ActorType, int, int) value4;
			long value5;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.actorID;
					value3 = value.killCount;
					value4 = value.reasonofDead;
					value5 = value.lastKillTime;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<(ActorType, int, int)>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					goto IL_00f3;
				}
				reader.ReadUnmanaged<int, int, (ActorType, int, int), long>(out value2, out value3, out value4, out value5);
			}
			else
			{
				if (memberCount > 4)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DeathMatchPlayerResult), 4, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = default((ActorType, int, int));
					value5 = 0L;
				}
				else
				{
					value2 = value.actorID;
					value3 = value.killCount;
					value4 = value.reasonofDead;
					value5 = value.lastKillTime;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<(ActorType, int, int)>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<long>(out value5);
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_00f3;
				}
			}
			value = new DeathMatchPlayerResult
			{
				actorID = value2,
				killCount = value3,
				reasonofDead = value4,
				lastKillTime = value5
			};
			return;
			IL_00f3:
			value.actorID = value2;
			value.killCount = value3;
			value.reasonofDead = value4;
			value.lastKillTime = value5;
		}
	}
}
