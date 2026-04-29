using FishNet.Serializing;
using FishNet.Serializing.Generated;

namespace Mimic.Voice.SpeechSystem
{
	public static class IncomingEventCustomSerializer
	{
		public static void WriteIncomingEvent(this Writer writer, IncomingEvent incomingEvent)
		{
			writer.WriteSingle(incomingEvent.EventExpireTime);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_IncomingTypeFishNet_002ESerializing_002EGenerated(writer, incomingEvent.EventType);
			writer.WriteInt32(incomingEvent.EventID);
		}

		public static IncomingEvent ReadIncomingEvent(this Reader reader)
		{
			return new IncomingEvent
			{
				EventExpireTime = reader.ReadSingle(),
				EventType = GeneratedReaders___Internal.GRead___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_IncomingTypeFishNet_002ESerializing_002EGenerateds(reader),
				EventID = reader.ReadInt32()
			};
		}
	}
}
