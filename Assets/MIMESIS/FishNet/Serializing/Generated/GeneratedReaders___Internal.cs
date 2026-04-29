using System.Runtime.InteropServices;
using Mimic.Voice.SpeechSystem;
using UnityEngine;

namespace FishNet.Serializing.Generated
{
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	public static class GeneratedReaders___Internal
	{
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeOnce()
		{
			GenericReader<IncomingEvent>.SetRead(IncomingEventCustomSerializer.ReadIncomingEvent);
			GenericReader<SpeechEvent>.SetRead(SpeechEventCustomSerializer.ReadSpeechEvent);
			GenericReader<SpeechEvent_IncomingType>.SetRead(GRead___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_IncomingTypeFishNet_002ESerializing_002EGenerateds);
			GenericReader<SpeechEvent[]>.SetRead(GRead___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_005B_005DFishNet_002ESerializing_002EGenerateds);
			GenericReader<long[]>.SetRead(GRead___System_002EInt64_005B_005DFishNet_002ESerializing_002EGenerateds);
		}

		public static SpeechEvent_IncomingType GRead___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_IncomingTypeFishNet_002ESerializing_002EGenerateds(Reader reader)
		{
			return (SpeechEvent_IncomingType)reader.ReadInt32();
		}

		public static SpeechEvent[] GRead___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_005B_005DFishNet_002ESerializing_002EGenerateds(Reader reader)
		{
			return reader.ReadArrayAllocated<SpeechEvent>();
		}

		public static long[] GRead___System_002EInt64_005B_005DFishNet_002ESerializing_002EGenerateds(Reader reader)
		{
			return reader.ReadArrayAllocated<long>();
		}
	}
}
