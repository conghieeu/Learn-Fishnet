using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class MaintenenceRoomInfo : IMemoryPackable<MaintenenceRoomInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class MaintenenceRoomInfoFormatter : MemoryPackFormatter<MaintenenceRoomInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MaintenenceRoomInfo value)
			{
				MaintenenceRoomInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref MaintenenceRoomInfo value)
			{
				MaintenenceRoomInfo.Deserialize(ref reader, ref value);
			}
		}

		public long ID { get; set; }

		public int CycleCountForClient { get; set; } = 1;

		public int dayCountForClient { get; set; } = 1;

		public bool repairedForClient { get; set; } = true;

		public List<int> tramUpgradeListForClient { get; set; } = new List<int>();

		static MaintenenceRoomInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<MaintenenceRoomInfo>())
			{
				MemoryPackFormatterProvider.Register(new MaintenenceRoomInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MaintenenceRoomInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<MaintenenceRoomInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<List<int>>())
			{
				MemoryPackFormatterProvider.Register(new ListFormatter<int>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref MaintenenceRoomInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<long, int, int, bool>(5, value.ID, value.CycleCountForClient, value.dayCountForClient, value.repairedForClient);
			writer.WriteValue<List<int>>(value.tramUpgradeListForClient);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref MaintenenceRoomInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			long value2;
			int value3;
			int value4;
			bool value5;
			List<int> value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.ID;
					value3 = value.CycleCountForClient;
					value4 = value.dayCountForClient;
					value5 = value.repairedForClient;
					value6 = value.tramUpgradeListForClient;
					reader.ReadUnmanaged<long>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<bool>(out value5);
					reader.ReadValue(ref value6);
					goto IL_011e;
				}
				reader.ReadUnmanaged<long, int, int, bool>(out value2, out value3, out value4, out value5);
				value6 = reader.ReadValue<List<int>>();
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(MaintenenceRoomInfo), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0L;
					value3 = 0;
					value4 = 0;
					value5 = false;
					value6 = null;
				}
				else
				{
					value2 = value.ID;
					value3 = value.CycleCountForClient;
					value4 = value.dayCountForClient;
					value5 = value.repairedForClient;
					value6 = value.tramUpgradeListForClient;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<long>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<bool>(out value5);
								if (memberCount != 4)
								{
									reader.ReadValue(ref value6);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_011e;
				}
			}
			value = new MaintenenceRoomInfo
			{
				ID = value2,
				CycleCountForClient = value3,
				dayCountForClient = value4,
				repairedForClient = value5,
				tramUpgradeListForClient = value6
			};
			return;
			IL_011e:
			value.ID = value2;
			value.CycleCountForClient = value3;
			value.dayCountForClient = value4;
			value.repairedForClient = value5;
			value.tramUpgradeListForClient = value6;
		}
	}
}
