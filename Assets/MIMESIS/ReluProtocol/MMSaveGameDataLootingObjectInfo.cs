using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[Serializable]
	[MemoryPackable(GenerateType.VersionTolerant)]
	public class MMSaveGameDataLootingObjectInfo : IMemoryPackable<MMSaveGameDataLootingObjectInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class MMSaveGameDataLootingObjectInfoFormatter : MemoryPackFormatter<MMSaveGameDataLootingObjectInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MMSaveGameDataLootingObjectInfo value)
			{
				MMSaveGameDataLootingObjectInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref MMSaveGameDataLootingObjectInfo value)
			{
				MMSaveGameDataLootingObjectInfo.Deserialize(ref reader, ref value);
			}
		}

		[MemoryPackOrder(0)]
		public MMSaveGameDataItemElement ItemElement { get; set; }

		[MemoryPackOrder(1)]
		public MMSaveGameDataPosWithRot PosWithRot { get; set; }

		[MemoryPackOrder(2)]
		public ReasonOfSpawn ReasonOfSpawn { get; set; }

		static MMSaveGameDataLootingObjectInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<MMSaveGameDataLootingObjectInfo>())
			{
				MemoryPackFormatterProvider.Register(new MMSaveGameDataLootingObjectInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MMSaveGameDataLootingObjectInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<MMSaveGameDataLootingObjectInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReasonOfSpawn>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<ReasonOfSpawn>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MMSaveGameDataLootingObjectInfo? value) where TBufferWriter : class, IBufferWriter<byte>
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
				memoryPackWriter.WritePackable<MMSaveGameDataItemElement>(value.ItemElement);
				span[0] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WritePackable<MMSaveGameDataPosWithRot>(value.PosWithRot);
				span[1] = memoryPackWriter.WrittenCount;
				memoryPackWriter.WriteUnmanaged<ReasonOfSpawn>(value.ReasonOfSpawn);
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
		public static void Deserialize(ref MemoryPackReader reader, ref MMSaveGameDataLootingObjectInfo? value)
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
			MMSaveGameDataItemElement value2;
			MMSaveGameDataPosWithRot value3;
			ReasonOfSpawn value4;
			if (memberCount == 3)
			{
				if (value != null)
				{
					value2 = value.ItemElement;
					value3 = value.PosWithRot;
					value4 = value.ReasonOfSpawn;
					if (span[0] != 0)
					{
						reader.ReadPackable(ref value2);
					}
					if (span[1] != 0)
					{
						reader.ReadPackable(ref value3);
					}
					if (span[2] != 0)
					{
						reader.ReadUnmanaged<ReasonOfSpawn>(out value4);
					}
					goto IL_0160;
				}
				value2 = ((span[0] != 0) ? reader.ReadPackable<MMSaveGameDataItemElement>() : null);
				value3 = ((span[1] != 0) ? reader.ReadPackable<MMSaveGameDataPosWithRot>() : null);
				if (span[2] == 0)
				{
					value4 = ReasonOfSpawn.None;
				}
				else
				{
					reader.ReadUnmanaged<ReasonOfSpawn>(out value4);
				}
			}
			else
			{
				if (value == null)
				{
					value2 = null;
					value3 = null;
					value4 = ReasonOfSpawn.None;
				}
				else
				{
					value2 = value.ItemElement;
					value3 = value.PosWithRot;
					value4 = value.ReasonOfSpawn;
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
							reader.ReadPackable(ref value3);
						}
						if (memberCount != 2)
						{
							if (span[2] != 0)
							{
								reader.ReadUnmanaged<ReasonOfSpawn>(out value4);
							}
							_ = 3;
						}
					}
				}
				if (value != null)
				{
					goto IL_0160;
				}
			}
			value = new MMSaveGameDataLootingObjectInfo
			{
				ItemElement = value2,
				PosWithRot = value3,
				ReasonOfSpawn = value4
			};
			goto IL_0198;
			IL_0160:
			value.ItemElement = value2;
			value.PosWithRot = value3;
			value.ReasonOfSpawn = value4;
			goto IL_0198;
			IL_0198:
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
