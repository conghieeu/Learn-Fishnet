using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class LevelObjectInfo : IMemoryPackable<LevelObjectInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class LevelObjectInfoFormatter : MemoryPackFormatter<LevelObjectInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LevelObjectInfo value)
			{
				LevelObjectInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref LevelObjectInfo value)
			{
				LevelObjectInfo.Deserialize(ref reader, ref value);
			}
		}

		public int levelObjectID { get; set; }

		public int prevState { get; set; }

		public int CurrentState { get; set; }

		public int OccupiedActorID { get; set; }

		public PosWithRot position { get; set; }

		static LevelObjectInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<LevelObjectInfo>())
			{
				MemoryPackFormatterProvider.Register(new LevelObjectInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<LevelObjectInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<LevelObjectInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LevelObjectInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<int, int, int, int>(5, value.levelObjectID, value.prevState, value.CurrentState, value.OccupiedActorID);
			writer.WritePackable<PosWithRot>(value.position);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref LevelObjectInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			int value3;
			int value4;
			int value5;
			PosWithRot value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.levelObjectID;
					value3 = value.prevState;
					value4 = value.CurrentState;
					value5 = value.OccupiedActorID;
					value6 = value.position;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					reader.ReadPackable(ref value6);
					goto IL_011d;
				}
				reader.ReadUnmanaged<int, int, int, int>(out value2, out value3, out value4, out value5);
				value6 = reader.ReadPackable<PosWithRot>();
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(LevelObjectInfo), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = 0;
					value5 = 0;
					value6 = null;
				}
				else
				{
					value2 = value.levelObjectID;
					value3 = value.prevState;
					value4 = value.CurrentState;
					value5 = value.OccupiedActorID;
					value6 = value.position;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<int>(out value5);
								if (memberCount != 4)
								{
									reader.ReadPackable(ref value6);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_011d;
				}
			}
			value = new LevelObjectInfo
			{
				levelObjectID = value2,
				prevState = value3,
				CurrentState = value4,
				OccupiedActorID = value5,
				position = value6
			};
			return;
			IL_011d:
			value.levelObjectID = value2;
			value.prevState = value3;
			value.CurrentState = value4;
			value.OccupiedActorID = value5;
			value.position = value6;
		}
	}
}
