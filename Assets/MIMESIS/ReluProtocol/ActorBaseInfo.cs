using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class ActorBaseInfo : IMemoryPackable<ActorBaseInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ActorBaseInfoFormatter : MemoryPackFormatter<ActorBaseInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ActorBaseInfo value)
			{
				ActorBaseInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ActorBaseInfo value)
			{
				ActorBaseInfo.Deserialize(ref reader, ref value);
			}
		}

		public ActorType actorType { get; set; }

		public int actorID { get; set; }

		public int masterID { get; set; }

		public string actorName { get; set; } = string.Empty;

		public PosWithRot position { get; set; } = new PosWithRot();

		public long UID { get; set; }

		public ReasonOfSpawn reasonOfSpawn { get; set; } = ReasonOfSpawn.Spawn;

		static ActorBaseInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ActorBaseInfo>())
			{
				MemoryPackFormatterProvider.Register(new ActorBaseInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ActorBaseInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ActorBaseInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ActorType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ActorType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReasonOfSpawn>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ReasonOfSpawn>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ActorBaseInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<ActorType, int, int>(7, value.actorType, value.actorID, value.masterID);
			writer.WriteString(value.actorName);
			writer.WritePackable<PosWithRot>(value.position);
			writer.WriteUnmanaged<long, ReasonOfSpawn>(value.UID, value.reasonOfSpawn);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref ActorBaseInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			ActorType value2;
			int value3;
			int value4;
			PosWithRot value5;
			long value6;
			ReasonOfSpawn value7;
			string text;
			if (memberCount == 7)
			{
				if (value != null)
				{
					value2 = value.actorType;
					value3 = value.actorID;
					value4 = value.masterID;
					text = value.actorName;
					value5 = value.position;
					value6 = value.UID;
					value7 = value.reasonOfSpawn;
					reader.ReadUnmanaged<ActorType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					text = reader.ReadString();
					reader.ReadPackable(ref value5);
					reader.ReadUnmanaged<long>(out value6);
					reader.ReadUnmanaged<ReasonOfSpawn>(out value7);
					goto IL_0183;
				}
				reader.ReadUnmanaged<ActorType, int, int>(out value2, out value3, out value4);
				text = reader.ReadString();
				value5 = reader.ReadPackable<PosWithRot>();
				reader.ReadUnmanaged<long, ReasonOfSpawn>(out value6, out value7);
			}
			else
			{
				if (memberCount > 7)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ActorBaseInfo), 7, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = ActorType.None;
					value3 = 0;
					value4 = 0;
					text = null;
					value5 = null;
					value6 = 0L;
					value7 = ReasonOfSpawn.None;
				}
				else
				{
					value2 = value.actorType;
					value3 = value.actorID;
					value4 = value.masterID;
					text = value.actorName;
					value5 = value.position;
					value6 = value.UID;
					value7 = value.reasonOfSpawn;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<ActorType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								text = reader.ReadString();
								if (memberCount != 4)
								{
									reader.ReadPackable(ref value5);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<long>(out value6);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<ReasonOfSpawn>(out value7);
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
					goto IL_0183;
				}
			}
			value = new ActorBaseInfo
			{
				actorType = value2,
				actorID = value3,
				masterID = value4,
				actorName = text,
				position = value5,
				UID = value6,
				reasonOfSpawn = value7
			};
			return;
			IL_0183:
			value.actorType = value2;
			value.actorID = value3;
			value.masterID = value4;
			value.actorName = text;
			value.position = value5;
			value.UID = value6;
			value.reasonOfSpawn = value7;
		}
	}
}
