using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mimic.Voice.SpeechSystem;
using UnityEngine;

namespace FishNet.Serializing.Generated
{
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	public static class GeneratedWriters___Internal
	{
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeOnce()
		{
			GenericWriter<IncomingEvent>.SetWrite(IncomingEventCustomSerializer.WriteIncomingEvent);
			GenericWriter<SpeechEvent>.SetWrite(SpeechEventCustomSerializer.WriteSpeechEvent);
			GenericWriter<SpeechEvent_IncomingType>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_IncomingTypeFishNet_002ESerializing_002EGenerated);
			GenericWriter<byte[]>.SetWrite(GWrite___System_002EByte_005B_005DFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechType_AdjacentPlayerCount>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_AdjacentPlayerCountFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechType_Area>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_AreaFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechType_GameTime>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_GameTimeFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechType_FacingPlayerCount>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_FacingPlayerCountFishNet_002ESerializing_002EGenerated);
			GenericWriter<List<int>>.SetWrite(GWrite___System_002ECollections_002EGeneric_002EList_00601_003CSystem_002EInt32_003EFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechType_Teleporter>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_TeleporterFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechType_IndoorEntered>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_IndoorEnteredFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechType_Charger>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_ChargerFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechType_CrowShop>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_CrowShopFishNet_002ESerializing_002EGenerated);
			GenericWriter<List<IncomingEvent>>.SetWrite(GWrite___System_002ECollections_002EGeneric_002EList_00601_003CMimic_002EVoice_002ESpeechSystem_002EIncomingEvent_003EFishNet_002ESerializing_002EGenerated);
			GenericWriter<SpeechEvent[]>.SetWrite(GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_005B_005DFishNet_002ESerializing_002EGenerated);
			GenericWriter<long[]>.SetWrite(GWrite___System_002EInt64_005B_005DFishNet_002ESerializing_002EGenerated);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_IncomingTypeFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechEvent_IncomingType value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___System_002EByte_005B_005DFishNet_002ESerializing_002EGenerated(this Writer writer, byte[] value)
		{
			writer.WriteArray(value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_AdjacentPlayerCountFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechType_AdjacentPlayerCount value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_AreaFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechType_Area value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_GameTimeFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechType_GameTime value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_FacingPlayerCountFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechType_FacingPlayerCount value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___System_002ECollections_002EGeneric_002EList_00601_003CSystem_002EInt32_003EFishNet_002ESerializing_002EGenerated(this Writer writer, List<int> value)
		{
			writer.WriteList(value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_TeleporterFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechType_Teleporter value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_IndoorEnteredFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechType_IndoorEntered value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_ChargerFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechType_Charger value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_CrowShopFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechType_CrowShop value)
		{
			writer.WriteInt32((int)value);
		}

		public static void GWrite___System_002ECollections_002EGeneric_002EList_00601_003CMimic_002EVoice_002ESpeechSystem_002EIncomingEvent_003EFishNet_002ESerializing_002EGenerated(this Writer writer, List<IncomingEvent> value)
		{
			writer.WriteList(value);
		}

		public static void GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechEvent_005B_005DFishNet_002ESerializing_002EGenerated(this Writer writer, SpeechEvent[] value)
		{
			writer.WriteArray(value);
		}

		public static void GWrite___System_002EInt64_005B_005DFishNet_002ESerializing_002EGenerated(this Writer writer, long[] value)
		{
			writer.WriteArray(value);
		}
	}
}
