using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class TimeSyncSig : IMsg, IMemoryPackable<TimeSyncSig>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class TimeSyncSigFormatter : MemoryPackFormatter<TimeSyncSig>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TimeSyncSig value)
			{
				TimeSyncSig.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref TimeSyncSig value)
			{
				TimeSyncSig.Deserialize(ref reader, ref value);
			}
		}

		public TimeSpan currentTime { get; set; }

		public int currentWeatherMasterID { get; set; }

		public int forecastWeatherMasterID { get; set; }

		public TimeSyncSig()
			: base(MsgType.C2S_TimeSyncSig)
		{
		}

		static TimeSyncSig()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<TimeSyncSig>())
			{
				MemoryPackFormatterProvider.Register(new TimeSyncSigFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<TimeSyncSig[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<TimeSyncSig>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref TimeSyncSig? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
			}
			else
			{
				writer.WriteUnmanagedWithObjectHeader<MsgType, int, TimeSpan, int, int>(5, value.msgType, value.hashCode, value.currentTime, value.currentWeatherMasterID, value.forecastWeatherMasterID);
			}
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref TimeSyncSig? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			TimeSpan value4;
			int value5;
			int value6;
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.currentTime;
					value5 = value.currentWeatherMasterID;
					value6 = value.forecastWeatherMasterID;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<TimeSpan>(out value4);
					reader.ReadUnmanaged<int>(out value5);
					reader.ReadUnmanaged<int>(out value6);
					goto IL_011d;
				}
				reader.ReadUnmanaged<MsgType, int, TimeSpan, int, int>(out value2, out value3, out value4, out value5, out value6);
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(TimeSyncSig), 5, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = default(TimeSpan);
					value5 = 0;
					value6 = 0;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.currentTime;
					value5 = value.currentWeatherMasterID;
					value6 = value.forecastWeatherMasterID;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<TimeSpan>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<int>(out value5);
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
					goto IL_011d;
				}
			}
			value = new TimeSyncSig
			{
				msgType = value2,
				hashCode = value3,
				currentTime = value4,
				currentWeatherMasterID = value5,
				forecastWeatherMasterID = value6
			};
			return;
			IL_011d:
			value.msgType = value2;
			value.hashCode = value3;
			value.currentTime = value4;
			value.currentWeatherMasterID = value5;
			value.forecastWeatherMasterID = value6;
		}
	}
}
