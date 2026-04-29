using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluReplay.Data
{
	[MemoryPackable(GenerateType.VersionTolerant)]
	public class ReplayHeader : IReplayHeader, IMemoryPackable<ReplayHeader>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ReplayHeaderFormatter : MemoryPackFormatter<ReplayHeader>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplayHeader value)
			{
				ReplayHeader.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ReplayHeader value)
			{
				ReplayHeader.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackOrder(0)]
		public REPLAY_HEADER_VERSION Version { get; set; }

		[MemoryPackOrder(1)]
		public ReplayHeaderMeta MetaInfos { get; set; }

		[MemoryPackOrder(2)]
		public Dictionary<string, object> ExtraData { get; set; } = new Dictionary<string, object>();

		public void SetDungeonMasterID(int dungeonMasterID)
		{
			MetaInfos.MapInfo.NextDungeonMasterID = dungeonMasterID;
		}

		public int GetDungeonMasterID()
		{
			return MetaInfos.MapInfo.NextDungeonMasterID;
		}

		public void SetDungeonRandSeed(int dungeonRandSeed)
		{
			MetaInfos.MapInfo.RandDungeonSeed = dungeonRandSeed;
		}

		public int GetDungeonRandSeed()
		{
			return MetaInfos.MapInfo.RandDungeonSeed;
		}

		public void SetReplayRecordStartTime(long startTime)
		{
			MetaInfos.RecordStartTime = startTime;
		}

		public long GetReplayRecordStartTime()
		{
			return MetaInfos.RecordStartTime;
		}

		public long GetReplayRecordEndTime()
		{
			return MetaInfos.RecordEndTime;
		}

		public List<int> GetPlayerActorIDs()
		{
			return MetaInfos.MapInfo.PlayerActorIDs;
		}

		public List<string> GetPlayerActorNames()
		{
			return MetaInfos.MapInfo.PlayerActorNames;
		}

		public List<int> GetMapInfos()
		{
			return MetaInfos.MapInfo.MapInfos;
		}

		public ReplaySaveInfo GetSaveInfo()
		{
			return MetaInfos.SaveInfo;
		}

		static ReplayHeader()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ReplayHeader>())
			{
				MemoryPackFormatterProvider.Register(new ReplayHeaderFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReplayHeader[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ReplayHeader>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<REPLAY_HEADER_VERSION>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<REPLAY_HEADER_VERSION>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<string, object>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<string, object>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplayHeader? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			ReusableLinkedArrayBufferWriter writer2 = ReusableLinkedArrayBufferWriterPool.Rent();
			try
			{
				Span<int> span = stackalloc int[3];
				MemoryPackWriter<ReusableLinkedArrayBufferWriter> memoryPackWriter = new MemoryPackWriter<ReusableLinkedArrayBufferWriter>(ref writer2, writer.OptionalState);
				memoryPackWriter.WriteUnmanaged<REPLAY_HEADER_VERSION>(value.Version);
				span[0] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WritePackable<ReplayHeaderMeta>(value.MetaInfos);
				span[1] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WriteValue<Dictionary<string, object>>(value.ExtraData);
				span[2] = memoryPackWriter.WrittenCount;
				memoryPackWriter.Flush();
				writer.WriteObjectHeader(3);
				for (int i = 0; i < 3; i++)
				{
					int x = ((i != 0) ? (span[i] - span[i - 1]) : span[i]);
					writer.WriteVarInt(x);
				}
				writer2.WriteToAndReset(ref writer);
			}
			finally
			{
				ReusableLinkedArrayBufferWriterPool.Return(writer2);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref ReplayHeader? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Span<int> span = stackalloc int[(int)memberCount];
			for (int i = 0; i < memberCount; i++)
			{
				span[i] = reader.ReadVarIntInt32();
			}
			int num = 3;
			REPLAY_HEADER_VERSION value2;
			ReplayHeaderMeta value3;
			Dictionary<string, object> value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.Version;
					value3 = value.MetaInfos;
					value4 = value.ExtraData;
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<REPLAY_HEADER_VERSION>(out value2);
					}
					if (span[1] != 0)
					{
						reader.ReadPackable(ref value3);
					}
					if (span[2] != 0)
					{
						reader.ReadValue(ref value4);
					}
					goto IL_0161;
				}
				if (span[0] == 0)
				{
					value2 = REPLAY_HEADER_VERSION.Legacy;
				}
				else
				{
					reader.ReadUnmanaged<REPLAY_HEADER_VERSION>(out value2);
				}
				value3 = ((span[1] != 0) ? reader.ReadPackable<ReplayHeaderMeta>() : null);
				value4 = ((span[2] != 0) ? reader.ReadValue<Dictionary<string, object>>() : null);
			}
			else
			{
				if (value == null)
				{
					value2 = REPLAY_HEADER_VERSION.Legacy;
					value3 = null;
					value4 = null;
				}
				else
				{
					value2 = value.Version;
					value3 = value.MetaInfos;
					value4 = value.ExtraData;
				}
				if (memberCount != 0)
				{
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<REPLAY_HEADER_VERSION>(out value2);
					}
					if (memberCount != 1)
					{
						if (span[1] != 0)
						{
							reader.ReadPackable(ref value3);
						}
						if (memberCount != 2)
						{
							if (span[2] != 0)
							{
								reader.ReadValue(ref value4);
							}
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_0161;
				}
			}
			value = new ReplayHeader
			{
				Version = value2,
				MetaInfos = value3,
				ExtraData = value4
			};
			goto IL_0199;
			IL_0161:
			value.Version = value2;
			value.MetaInfos = value3;
			value.ExtraData = value4;
			goto IL_0199;
			IL_0199:
			if (memberCount != num)
			{
				for (int j = num; j < memberCount; j++)
				{
					reader.Advance(span[j]);
				}
			}
		}
	}
}
