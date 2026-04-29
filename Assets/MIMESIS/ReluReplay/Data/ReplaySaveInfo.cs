using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluReplay.Data
{
	[MemoryPackable(GenerateType.VersionTolerant)]
	public class ReplaySaveInfo : IMemoryPackable<ReplaySaveInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ReplaySaveInfoFormatter : MemoryPackFormatter<ReplaySaveInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplaySaveInfo value)
			{
				ReplaySaveInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ReplaySaveInfo value)
			{
				ReplaySaveInfo.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackOrder(0)]
		public List<int> TramUpgradeIDs { get; set; } = new List<int>();

		[MemoryPackConstructor]
		public ReplaySaveInfo()
		{
		}

		public ReplaySaveInfo(ReplaySaveInfo inSaveInfo)
		{
			if (inSaveInfo?.TramUpgradeIDs != null)
			{
				TramUpgradeIDs = new List<int>(inSaveInfo.TramUpgradeIDs);
			}
			else
			{
				TramUpgradeIDs = new List<int>();
			}
		}

		static ReplaySaveInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ReplaySaveInfo>())
			{
				MemoryPackFormatterProvider.Register(new ReplaySaveInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReplaySaveInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ReplaySaveInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplaySaveInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			ReusableLinkedArrayBufferWriter writer2 = ReusableLinkedArrayBufferWriterPool.Rent();
			try
			{
				Span<int> span = stackalloc int[1];
				MemoryPackWriter<ReusableLinkedArrayBufferWriter> memoryPackWriter = new MemoryPackWriter<ReusableLinkedArrayBufferWriter>(ref writer2, writer.OptionalState);
				memoryPackWriter.WriteValue<List<int>>(value.TramUpgradeIDs);
				span[0] = memoryPackWriter.WrittenCount;
				memoryPackWriter.Flush();
				writer.WriteObjectHeader(1);
				for (int i = 0; i < 1; i++)
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
		public static void Deserialize(ref MemoryPackReader reader, ref ReplaySaveInfo? value)
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
			int num = 1;
			List<int> value2;
			if (memberCount == 1)
			{
				if (value != null)
				{
					value2 = value.TramUpgradeIDs;
					if (span[0] != 0)
					{
						reader.ReadValue(ref value2);
					}
					goto IL_00ad;
				}
				value2 = ((span[0] != 0) ? reader.ReadValue<List<int>>() : null);
			}
			else
			{
				value2 = ((value != null) ? value.TramUpgradeIDs : null);
				if (memberCount != 0)
				{
					if (span[0] != 0)
					{
						reader.ReadValue(ref value2);
					}
					_ = 1;
				}
				if (value != null)
				{
					goto IL_00ad;
				}
			}
			value = new ReplaySaveInfo
			{
				TramUpgradeIDs = value2
			};
			goto IL_00c5;
			IL_00c5:
			if (memberCount != num)
			{
				for (int j = num; j < memberCount; j++)
				{
					reader.Advance(span[j]);
				}
			}
			return;
			IL_00ad:
			value.TramUpgradeIDs = value2;
			goto IL_00c5;
		}
	}
}
