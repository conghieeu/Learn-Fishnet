using System.Buffers;
using System.Collections.Generic;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class StatCollection : IMemoryPackable<StatCollection>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class StatCollectionFormatter : MemoryPackFormatter<StatCollection>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StatCollection value)
			{
				StatCollection.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref StatCollection value)
			{
				StatCollection.Deserialize(ref reader, ref value);
			}
		}

		public Dictionary<MutableStatType, long> mutableStats { get; set; } = new Dictionary<MutableStatType, long>();

		public Dictionary<StatType, long> ImmutableStats { get; set; } = new Dictionary<StatType, long>();

		static StatCollection()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<StatCollection>())
			{
				MemoryPackFormatterProvider.Register(new StatCollectionFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<StatCollection[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<StatCollection>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<MutableStatType, long>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<MutableStatType, long>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MutableStatType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MutableStatType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<StatType, long>>())
			{
				MemoryPackFormatterProvider.Register(new DictionaryFormatter<StatType, long>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<StatType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<StatType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref StatCollection? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteObjectHeader(2);
			writer.WriteValue<Dictionary<MutableStatType, long>>(value.mutableStats);
			writer.WriteValue<Dictionary<StatType, long>>(value.ImmutableStats);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref StatCollection? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			Dictionary<MutableStatType, long> value2;
			Dictionary<StatType, long> value3;
			if (memberCount == 2)
			{
				if (value != null)
				{
					value2 = value.mutableStats;
					value3 = value.ImmutableStats;
					reader.ReadValue(ref value2);
					reader.ReadValue(ref value3);
					goto IL_009a;
				}
				value2 = reader.ReadValue<Dictionary<MutableStatType, long>>();
				value3 = reader.ReadValue<Dictionary<StatType, long>>();
			}
			else
			{
				if (memberCount > 2)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(StatCollection), 2, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = null;
					value3 = null;
				}
				else
				{
					value2 = value.mutableStats;
					value3 = value.ImmutableStats;
				}
				if (memberCount != 0)
				{
					reader.ReadValue(ref value2);
					if (memberCount != 1)
					{
						reader.ReadValue(ref value3);
						_ = 2;
					}
				}
				if (value != null)
				{
					goto IL_009a;
				}
			}
			value = new StatCollection
			{
				mutableStats = value2,
				ImmutableStats = value3
			};
			return;
			IL_009a:
			value.mutableStats = value2;
			value.ImmutableStats = value3;
		}
	}
}
