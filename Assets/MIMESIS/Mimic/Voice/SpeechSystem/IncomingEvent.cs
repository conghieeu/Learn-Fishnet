using System;
using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

namespace Mimic.Voice.SpeechSystem
{
	[Serializable]
	[MemoryPackable(GenerateType.Object)]
	public struct IncomingEvent : IMemoryPackable<IncomingEvent>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class IncomingEventFormatter : MemoryPackFormatter<IncomingEvent>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref IncomingEvent value)
			{
				IncomingEvent.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref IncomingEvent value)
			{
				IncomingEvent.Deserialize(ref reader, ref value);
			}
		}

		public float EventExpireTime;

		public SpeechEvent_IncomingType EventType;

		public int EventID;

		static IncomingEvent()
		{
			RegisterFormatter();
		}

		[Preserve]
		public static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<IncomingEvent>())
			{
				MemoryPackFormatterProvider.Register(new IncomingEventFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<IncomingEvent[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<IncomingEvent>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<SpeechEvent_IncomingType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<SpeechEvent_IncomingType>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref IncomingEvent value) where TBufferWriter : class, IBufferWriter<byte>
		{
			writer.WriteUnmanaged(in value);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref IncomingEvent value)
		{
			reader.ReadUnmanaged<IncomingEvent>(out value);
		}
	}
}
