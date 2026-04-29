using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	[MemoryPackable(GenerateType.Object)]
	public class SkillCooltimeInfo : CooltimeInfo, IMemoryPackable<SkillCooltimeInfo>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class SkillCooltimeInfoFormatter : MemoryPackFormatter<SkillCooltimeInfo>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SkillCooltimeInfo value)
			{
				SkillCooltimeInfo.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref SkillCooltimeInfo value)
			{
				SkillCooltimeInfo.Deserialize(ref reader, ref value);
			}
		}

		public int skillMasterID { get; set; }

		static SkillCooltimeInfo()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<SkillCooltimeInfo>())
			{
				MemoryPackFormatterProvider.Register(new SkillCooltimeInfoFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SkillCooltimeInfo[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<SkillCooltimeInfo>());
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
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref SkillCooltimeInfo? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<CooltimeChangeType, CooltimeType, float, long, bool, int>(6, value.changeType, value.cooltimeType, value.remainTime, value.endTime, value.global, value.skillMasterID);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref SkillCooltimeInfo? value)
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
			int value7;
			if (memberCount == 6)
			{
				if (value != null)
				{
					value2 = value.changeType;
					value3 = value.cooltimeType;
					value4 = value.remainTime;
					value5 = value.endTime;
					value6 = value.global;
					value7 = value.skillMasterID;
					reader.ReadUnmanaged<CooltimeChangeType>(out value2);
					reader.ReadUnmanaged<CooltimeType>(out value3);
					reader.ReadUnmanaged<float>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanaged<bool>(out value6);
					reader.ReadUnmanaged<int>(out value7);
					goto IL_014a;
				}
				reader.ReadUnmanaged<CooltimeChangeType, CooltimeType, float, long, bool, int>(out value2, out value3, out value4, out value5, out value6, out value7);
			}
			else
			{
				if (memberCount > 6)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(SkillCooltimeInfo), 6, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = CooltimeChangeType.Add;
					value3 = CooltimeType.Skill;
					value4 = 0f;
					value5 = 0L;
					value6 = false;
					value7 = 0;
				}
				else
				{
					value2 = value.changeType;
					value3 = value.cooltimeType;
					value4 = value.remainTime;
					value5 = value.endTime;
					value6 = value.global;
					value7 = value.skillMasterID;
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
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<int>(out value7);
										_ = 6;
									}
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_014a;
				}
			}
			value = new SkillCooltimeInfo
			{
				changeType = value2,
				cooltimeType = value3,
				remainTime = value4,
				endTime = value5,
				global = value6,
				skillMasterID = value7
			};
			return;
			IL_014a:
			value.changeType = value2;
			value.cooltimeType = value3;
			value.remainTime = value4;
			value.endTime = value5;
			value.global = value6;
			value.skillMasterID = value7;
		}
	}
}
