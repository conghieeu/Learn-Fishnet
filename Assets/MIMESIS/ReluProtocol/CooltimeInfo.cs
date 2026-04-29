using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class CooltimeInfo : IMemoryPackable<CooltimeInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class CooltimeInfoFormatter : MemoryPackFormatter<CooltimeInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CooltimeInfo value)
			{
				CooltimeInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref CooltimeInfo value)
			{
				CooltimeInfo.Deserialize(ref reader, ref value);
			}
		}

		public CooltimeChangeType changeType { get; set; }

		public CooltimeType cooltimeType { get; set; }

		public float remainTime { get; set; }

		public long endTime { get; set; }

		public bool global { get; set; }

		static CooltimeInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<CooltimeInfo>())
			{
				MemoryPackFormatterProvider.Register(new CooltimeInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CooltimeInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<CooltimeInfo>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CooltimeChangeType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<CooltimeChangeType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<CooltimeType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<CooltimeType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref CooltimeInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<CooltimeChangeType, CooltimeType, float, long, bool>(5, value.changeType, value.cooltimeType, value.remainTime, value.endTime, value.global);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref CooltimeInfo? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			CooltimeChangeType value2;
			CooltimeType value3;
			float value4;
			long value5;
			bool value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.changeType;
					value3 = value.cooltimeType;
					value4 = value.remainTime;
					value5 = value.endTime;
					value6 = value.global;
					reader.ReadUnmanaged<CooltimeChangeType>(out value2);
					reader.ReadUnmanaged<CooltimeType>(out value3);
					reader.ReadUnmanaged<float>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanaged<bool>(out value6);
					goto IL_011c;
				}
				reader.ReadUnmanaged<CooltimeChangeType, CooltimeType, float, long, bool>(out value2, out value3, out value4, out value5, out value6);
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(CooltimeInfo), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = CooltimeChangeType.Add;
					value3 = CooltimeType.Skill;
					value4 = 0f;
					value5 = 0L;
					value6 = false;
				}
				else
				{
					value2 = value.changeType;
					value3 = value.cooltimeType;
					value4 = value.remainTime;
					value5 = value.endTime;
					value6 = value.global;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<CooltimeChangeType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<CooltimeType>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<float>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<long>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<bool>(out value6);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_011c;
				}
			}
			value = new CooltimeInfo
			{
				changeType = value2,
				cooltimeType = value3,
				remainTime = value4,
				endTime = value5,
				global = value6
			};
			return;
			IL_011c:
			value.changeType = value2;
			value.cooltimeType = value3;
			value.remainTime = value4;
			value.endTime = value5;
			value.global = value6;
		}
	}
}
