using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

namespace ReluReplay.Data
{
	[MemoryPackable(GenerateType.Object)]
	public class LegacyReplayHeader : IReplayHeader, IMemoryPackable<LegacyReplayHeader>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class LegacyReplayHeaderFormatter : MemoryPackFormatter<LegacyReplayHeader>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LegacyReplayHeader value)
			{
				LegacyReplayHeader.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref LegacyReplayHeader value)
			{
				LegacyReplayHeader.Deserialize(ref reader, ref value);
			}
		}

		public int nextDungeonMasterID { get; set; }

		public int recordRandDungeonSeed { get; set; }

		public long recordStartTime { get; set; }

		public void SetDungeonMasterID(int dungeonMasterID)
		{
			nextDungeonMasterID = dungeonMasterID;
		}

		public int GetDungeonMasterID()
		{
			return nextDungeonMasterID;
		}

		public void SetDungeonRandSeed(int dungeonRandSeed)
		{
			recordRandDungeonSeed = dungeonRandSeed;
		}

		public int GetDungeonRandSeed()
		{
			return recordRandDungeonSeed;
		}

		public void SetReplayRecordStartTime(long startTime)
		{
			recordStartTime = startTime;
		}

		public long GetReplayRecordStartTime()
		{
			return recordStartTime;
		}

		public long GetReplayRecordEndTime()
		{
			return -1L;
		}

		public List<int> GetPlayerActorIDs()
		{
			return null;
		}

		public List<string> GetPlayerActorNames()
		{
			return null;
		}

		public List<int> GetMapInfos()
		{
			return null;
		}

		public ReplaySaveInfo GetSaveInfo()
		{
			Debug.LogError("[LegacyReplayHeader] GetSaveInfo() is not supported in legacy version");
			return null;
		}

		static LegacyReplayHeader()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<LegacyReplayHeader>())
			{
				MemoryPackFormatterProvider.Register(new LegacyReplayHeaderFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<LegacyReplayHeader[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<LegacyReplayHeader>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref LegacyReplayHeader? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<int, int, long>(3, value.nextDungeonMasterID, value.recordRandDungeonSeed, value.recordStartTime);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref LegacyReplayHeader? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			int value3;
			long value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.nextDungeonMasterID;
					value3 = value.recordRandDungeonSeed;
					value4 = value.recordStartTime;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					goto IL_00bf;
				}
				reader.ReadUnmanaged<int, int, long>(out value2, out value3, out value4);
			}
			else
			{
				if (memberCount > 3)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(LegacyReplayHeader), 3, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = 0L;
				}
				else
				{
					value2 = value.nextDungeonMasterID;
					value3 = value.recordRandDungeonSeed;
					value4 = value.recordStartTime;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<long>(out value4);
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_00bf;
				}
			}
			value = new LegacyReplayHeader
			{
				nextDungeonMasterID = value2,
				recordRandDungeonSeed = value3,
				recordStartTime = value4
			};
			return;
			IL_00bf:
			value.nextDungeonMasterID = value2;
			value.recordRandDungeonSeed = value3;
			value.recordStartTime = value4;
		}
	}
}
