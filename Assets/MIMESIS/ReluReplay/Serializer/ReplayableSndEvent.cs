using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using Mimic.Voice.SpeechSystem;

namespace ReluReplay.Serializer
{
	[MemoryPackable(GenerateType.Object)]
	public class ReplayableSndEvent : IMemoryPackable<ReplayableSndEvent>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class ReplayableSndEventFormatter : MemoryPackFormatter<ReplayableSndEvent>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplayableSndEvent value)
			{
				ReplayableSndEvent.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref ReplayableSndEvent value)
			{
				ReplayableSndEvent.Deserialize(ref reader, ref value);
			}
		}

		public SndEventType SndEventType { get; set; }

		public int ActorID { get; set; }

		public long PlayerUID { get; set; }

		public long Time { get; set; }

		public byte[] MetaData { get; set; }

		public byte[] AudioData { get; set; }

		public ReplayableSndEvent(SndEventType sndEventType, int actorID, long playerUID, long time, byte[] metaData, byte[] audioData)
		{
			SndEventType = sndEventType;
			ActorID = actorID;
			PlayerUID = playerUID;
			Time = time;
			MetaData = metaData;
			AudioData = audioData;
		}

		public static byte[] GetDataFromSndEvent(SpeechEvent sndEvent)
		{
			return MemoryPackSerializer.Serialize(sndEvent.GetType(), sndEvent);
		}

		public static byte[] GetAudioDataFromSndEvent(SpeechEvent sndEvent)
		{
			return null;
		}

		public SpeechEvent GetSndEvent()
		{
			return (SpeechEvent)MemoryPackSerializer.Deserialize(typeof(SpeechEvent), MetaData);
		}

		static ReplayableSndEvent()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<ReplayableSndEvent>())
			{
				MemoryPackFormatterProvider.Register(new ReplayableSndEventFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<ReplayableSndEvent[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<ReplayableSndEvent>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SndEventType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SndEventType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<byte[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<byte>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref ReplayableSndEvent? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<SndEventType, int, long, long>(6, value.SndEventType, value.ActorID, value.PlayerUID, value.Time);
			writer.WriteUnmanagedArray(value.MetaData);
			writer.WriteUnmanagedArray(value.AudioData);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref ReplayableSndEvent? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			SndEventType value2;
			int value3;
			long value4;
			long value5;
			byte[] value6;
			byte[] value7;
			if (memberCount == 6)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<SndEventType, int, long, long>(out value2, out value3, out value4, out value5);
					value6 = reader.ReadUnmanagedArray<byte>();
					value7 = reader.ReadUnmanagedArray<byte>();
				}
				else
				{
					value2 = value.SndEventType;
					value3 = value.ActorID;
					value4 = value.PlayerUID;
					value5 = value.Time;
					value6 = value.MetaData;
					value7 = value.AudioData;
					reader.ReadUnmanaged<SndEventType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<long>(out value4);
					reader.ReadUnmanaged<long>(out value5);
					reader.ReadUnmanagedArray(ref value6);
					reader.ReadUnmanagedArray(ref value7);
				}
			}
			else
			{
				if (memberCount > 6)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(ReplayableSndEvent), 6, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = SndEventType.NONE;
					value3 = 0;
					value4 = 0L;
					value5 = 0L;
					value6 = null;
					value7 = null;
				}
				else
				{
					value2 = value.SndEventType;
					value3 = value.ActorID;
					value4 = value.PlayerUID;
					value5 = value.Time;
					value6 = value.MetaData;
					value7 = value.AudioData;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<SndEventType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<long>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<long>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanagedArray(ref value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanagedArray(ref value7);
										_ = 6;
									}
								}
							}
						}
					}
				}
				_ = value;
			}
			value = new ReplayableSndEvent(value2, value3, value4, value5, value6, value7);
		}
	}
}
