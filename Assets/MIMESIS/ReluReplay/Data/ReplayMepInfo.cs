using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluReplay.Data
{
	[MemoryPackable(GenerateType.VersionTolerant)]
	public class ReplayMepInfo : IMemoryPackable<ReplayMepInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ReplayMepInfoFormatter : MemoryPackFormatter<ReplayMepInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplayMepInfo value)
			{
				ReplayMepInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ReplayMepInfo value)
			{
				ReplayMepInfo.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackOrder(0)]
		public int NextDungeonMasterID { get; set; }

		[MemoryPackOrder(1)]
		public int RandDungeonSeed { get; set; }

		[MemoryPackOrder(2)]
		public List<int> PlayerActorIDs { get; set; } = new List<int>();

		[MemoryPackOrder(3)]
		public List<string> PlayerActorNames { get; set; } = new List<string>();

		[MemoryPackOrder(4)]
		public List<int> MapInfos { get; set; } = new List<int>();

		static ReplayMepInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ReplayMepInfo>())
			{
				MemoryPackFormatterProvider.Register(new ReplayMepInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReplayMepInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ReplayMepInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<string>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<string>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplayMepInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			ReusableLinkedArrayBufferWriter writer2 = ReusableLinkedArrayBufferWriterPool.Rent();
			try
			{
				Span<int> span = stackalloc int[5];
				MemoryPackWriter<ReusableLinkedArrayBufferWriter> memoryPackWriter = new MemoryPackWriter<ReusableLinkedArrayBufferWriter>(ref writer2, writer.OptionalState);
				memoryPackWriter.WriteUnmanaged<int>(value.NextDungeonMasterID);
				span[0] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WriteUnmanaged<int>(value.RandDungeonSeed);
				span[1] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WriteValue<List<int>>(value.PlayerActorIDs);
				span[2] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WriteValue<List<string>>(value.PlayerActorNames);
				span[3] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WriteValue<List<int>>(value.MapInfos);
				span[4] = memoryPackWriter.WrittenCount;
				memoryPackWriter.Flush();
				writer.WriteObjectHeader(5);
				for (int i = 0; i < 5; i++)
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
		public static void Deserialize(ref MemoryPackReader reader, ref ReplayMepInfo? value)
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
			int num = 5;
			int value2;
			int value3;
			List<int> value4;
			List<string> value5;
			List<int> value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.NextDungeonMasterID;
					value3 = value.RandDungeonSeed;
					value4 = value.PlayerActorIDs;
					value5 = value.PlayerActorNames;
					value6 = value.MapInfos;
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<int>(out value2);
					}
					if (span[1] != 0)
					{
						reader.ReadUnmanaged<int>(out value3);
					}
					if (span[2] != 0)
					{
						reader.ReadValue(ref value4);
					}
					if (span[3] != 0)
					{
						reader.ReadValue(ref value5);
					}
					if (span[4] != 0)
					{
						reader.ReadValue(ref value6);
					}
					goto IL_0216;
				}
				if (span[0] == 0)
				{
					value2 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value2);
				}
				if (span[1] == 0)
				{
					value3 = 0;
				}
				else
				{
					reader.ReadUnmanaged<int>(out value3);
				}
				value4 = ((span[2] != 0) ? reader.ReadValue<List<int>>() : null);
				value5 = ((span[3] != 0) ? reader.ReadValue<List<string>>() : null);
				value6 = ((span[4] != 0) ? reader.ReadValue<List<int>>() : null);
			}
			else
			{
				if (value == null)
				{
					value2 = 0;
					value3 = 0;
					value4 = null;
					value5 = null;
					value6 = null;
				}
				else
				{
					value2 = value.NextDungeonMasterID;
					value3 = value.RandDungeonSeed;
					value4 = value.PlayerActorIDs;
					value5 = value.PlayerActorNames;
					value6 = value.MapInfos;
				}
				if (memberCount != 0)
				{
					if (span[0] != 0)
					{
						reader.ReadUnmanaged<int>(out value2);
					}
					if (memberCount != 1)
					{
						if (span[1] != 0)
						{
							reader.ReadUnmanaged<int>(out value3);
						}
						if (memberCount != 2)
						{
							if (span[2] != 0)
							{
								reader.ReadValue(ref value4);
							}
							if (memberCount != 3)
							{
								if (span[3] != 0)
								{
									reader.ReadValue(ref value5);
								}
								if (memberCount != 4)
								{
									if (span[4] != 0)
									{
										reader.ReadValue(ref value6);
									}
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0216;
				}
			}
			value = new ReplayMepInfo
			{
				NextDungeonMasterID = value2,
				RandDungeonSeed = value3,
				PlayerActorIDs = value4,
				PlayerActorNames = value5,
				MapInfos = value6
			};
			goto IL_0270;
			IL_0270:
			if (memberCount != num)
			{
				for (int j = num; j < memberCount; j++)
				{
					reader.Advance(span[j]);
				}
			}
			return;
			IL_0216:
			value.NextDungeonMasterID = value2;
			value.RandDungeonSeed = value3;
			value.PlayerActorIDs = value4;
			value.PlayerActorNames = value5;
			value.MapInfos = value6;
			goto IL_0270;
		}
	}
}
