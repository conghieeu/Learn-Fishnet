using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluReplay.Data
{
	[MemoryPackable(GenerateType.VersionTolerant)]
	public class ReplayHeaderMeta : IMemoryPackable<ReplayHeaderMeta>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ReplayHeaderMetaFormatter : MemoryPackFormatter<ReplayHeaderMeta>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplayHeaderMeta value)
			{
				ReplayHeaderMeta.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ReplayHeaderMeta value)
			{
				ReplayHeaderMeta.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackOrder(0)]
		public ReplayMepInfo MapInfo { get; set; }

		[MemoryPackOrder(1)]
		public long RecordStartTime { get; set; }

		[MemoryPackOrder(2)]
		public long RecordEndTime { get; set; }

		[MemoryPackOrder(3)]
		public ReplaySaveInfo SaveInfo { get; set; }

		static ReplayHeaderMeta()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ReplayHeaderMeta>())
			{
				MemoryPackFormatterProvider.Register(new ReplayHeaderMetaFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReplayHeaderMeta[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ReplayHeaderMeta>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplayHeaderMeta? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			ReusableLinkedArrayBufferWriter writer2 = ReusableLinkedArrayBufferWriterPool.Rent();
			try
			{
				Span<int> span = stackalloc int[4];
				MemoryPackWriter<ReusableLinkedArrayBufferWriter> memoryPackWriter = new MemoryPackWriter<ReusableLinkedArrayBufferWriter>(ref writer2, writer.OptionalState);
				memoryPackWriter.WritePackable<ReplayMepInfo>(value.MapInfo);
				span[0] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WriteUnmanaged<long>(value.RecordStartTime);
				span[1] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WriteUnmanaged<long>(value.RecordEndTime);
				span[2] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WritePackable<ReplaySaveInfo>(value.SaveInfo);
				span[3] = memoryPackWriter.WrittenCount;
				memoryPackWriter.Flush();
				writer.WriteObjectHeader(4);
				for (int i = 0; i < 4; i++)
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
		public static void Deserialize(ref MemoryPackReader reader, ref ReplayHeaderMeta? value)
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
			int num = 4;
			ReplayMepInfo value2;
			long value3;
			long value4;
			ReplaySaveInfo value5;
			if (memberCount == 4)
			{
				if (value != null)
				{
					value2 = value.MapInfo;
					value3 = value.RecordStartTime;
					value4 = value.RecordEndTime;
					value5 = value.SaveInfo;
					if (span[0] != 0)
					{
						reader.ReadPackable(ref value2);
					}
					if (span[1] != 0)
					{
						reader.ReadUnmanaged<long>(out value3);
					}
					if (span[2] != 0)
					{
						reader.ReadUnmanaged<long>(out value4);
					}
					if (span[3] != 0)
					{
						reader.ReadPackable(ref value5);
					}
					goto IL_01c2;
				}
				value2 = ((span[0] != 0) ? reader.ReadPackable<ReplayMepInfo>() : null);
				if (span[1] == 0)
				{
					value3 = 0L;
				}
				else
				{
					reader.ReadUnmanaged<long>(out value3);
				}
				if (span[2] == 0)
				{
					value4 = 0L;
				}
				else
				{
					reader.ReadUnmanaged<long>(out value4);
				}
				value5 = ((span[3] != 0) ? reader.ReadPackable<ReplaySaveInfo>() : null);
			}
			else
			{
				if (value == null)
				{
					value2 = null;
					value3 = 0L;
					value4 = 0L;
					value5 = null;
				}
				else
				{
					value2 = value.MapInfo;
					value3 = value.RecordStartTime;
					value4 = value.RecordEndTime;
					value5 = value.SaveInfo;
				}
				if (memberCount != 0)
				{
					if (span[0] != 0)
					{
						reader.ReadPackable(ref value2);
					}
					if (memberCount != 1)
					{
						if (span[1] != 0)
						{
							reader.ReadUnmanaged<long>(out value3);
						}
						if (memberCount != 2)
						{
							if (span[2] != 0)
							{
								reader.ReadUnmanaged<long>(out value4);
							}
							if (memberCount != 3)
							{
								if (span[3] != 0)
								{
									reader.ReadPackable(ref value5);
								}
								_ = 4;
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_01c2;
				}
			}
			value = new ReplayHeaderMeta
			{
				MapInfo = value2,
				RecordStartTime = value3,
				RecordEndTime = value4,
				SaveInfo = value5
			};
			goto IL_020b;
			IL_020b:
			if (memberCount != num)
			{
				for (int j = num; j < memberCount; j++)
				{
					reader.Advance(span[j]);
				}
			}
			return;
			IL_01c2:
			value.MapInfo = value2;
			value.RecordStartTime = value3;
			value.RecordEndTime = value4;
			value.SaveInfo = value5;
			goto IL_020b;
		}
	}
}
