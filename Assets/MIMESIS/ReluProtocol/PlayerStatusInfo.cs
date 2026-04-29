using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class PlayerStatusInfo : IMemoryPackable<PlayerStatusInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class PlayerStatusInfoFormatter : MemoryPackFormatter<PlayerStatusInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlayerStatusInfo value)
			{
				PlayerStatusInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref PlayerStatusInfo value)
			{
				PlayerStatusInfo.Deserialize(ref reader, ref value);
			}
		}

		public int actorID { get; set; }

		public VCreatureLifeCycle actorLifeCycle { get; set; }

		public PosWithRot position { get; set; } = new PosWithRot();

		public Dictionary<int, ItemInfo> inventories { get; set; } = new Dictionary<int, ItemInfo>();

		public int currentInventorySlot { get; set; }

		static PlayerStatusInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerStatusInfo>())
			{
				MemoryPackFormatterProvider.Register(new PlayerStatusInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<PlayerStatusInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<PlayerStatusInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<VCreatureLifeCycle>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<VCreatureLifeCycle>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<int, ItemInfo>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<int, ItemInfo>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref PlayerStatusInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<int, VCreatureLifeCycle>(5, value.actorID, value.actorLifeCycle);
			writer.WritePackable<PosWithRot>(value.position);
			writer.WriteValue<Dictionary<int, ItemInfo>>(value.inventories);
			writer.WriteUnmanaged<int>(value.currentInventorySlot);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref PlayerStatusInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			int value2;
			VCreatureLifeCycle value3;
			PosWithRot value4;
			Dictionary<int, ItemInfo> value5;
			int value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.actorID;
					value3 = value.actorLifeCycle;
					value4 = value.position;
					value5 = value.inventories;
					value6 = value.currentInventorySlot;
					reader.ReadUnmanaged<int>(out value2);
					reader.ReadUnmanaged<VCreatureLifeCycle>(out value3);
					reader.ReadPackable(ref value4);
					reader.ReadValue(ref value5);
					reader.ReadUnmanaged<int>(out value6);
					goto IL_012b;
				}
				reader.ReadUnmanaged<int, VCreatureLifeCycle>(out value2, out value3);
				value4 = reader.ReadPackable<PosWithRot>();
				value5 = reader.ReadValue<Dictionary<int, ItemInfo>>();
				reader.ReadUnmanaged<int>(out value6);
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(PlayerStatusInfo), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = 0;
					value3 = VCreatureLifeCycle.Ready;
					value4 = null;
					value5 = null;
					value6 = 0;
				}
				else
				{
					value2 = value.actorID;
					value3 = value.actorLifeCycle;
					value4 = value.position;
					value5 = value.inventories;
					value6 = value.currentInventorySlot;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<int>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<VCreatureLifeCycle>(out value3);
						if (memberCount != 2)
						{
							reader.ReadPackable(ref value4);
							if (memberCount != 3)
							{
								reader.ReadValue(ref value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<int>(out value6);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_012b;
				}
			}
			value = new PlayerStatusInfo
			{
				actorID = value2,
				actorLifeCycle = value3,
				position = value4,
				inventories = value5,
				currentInventorySlot = value6
			};
			return;
			IL_012b:
			value.actorID = value2;
			value.actorLifeCycle = value3;
			value.position = value4;
			value.inventories = value5;
			value.currentInventorySlot = value6;
		}
	}
}
