using System.Collections.Generic;
using FishNet.Serializing;
using FishNet.Serializing.Generated;

namespace Mimic.Voice.SpeechSystem
{
	public static class SpeechEventCustomSerializer
	{
		public static void WriteSpeechEvent(this Writer writer, SpeechEvent speechEvent)
		{
			writer.WriteInt64(speechEvent.Id);
			writer.WriteString(speechEvent.PlayerName);
			writer.WriteSingle(speechEvent.RecordedTime);
			writer.WriteInt32(speechEvent.Channels);
			writer.WriteInt32(speechEvent.SampleRate);
			GeneratedWriters___Internal.GWrite___System_002EByte_005B_005DFishNet_002ESerializing_002EGenerated(writer, speechEvent.CompressedAudioData);
			writer.WriteInt32(speechEvent.OriginalAudioDataLength);
			writer.WriteSingle(speechEvent.AverageAmplitude);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_AdjacentPlayerCountFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.AdjacentPlayerCount);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_AreaFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.Area);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_GameTimeFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.GameTime);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_FacingPlayerCountFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.FacingPlayerCount);
			GeneratedWriters___Internal.GWrite___System_002ECollections_002EGeneric_002EList_00601_003CSystem_002EInt32_003EFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.ScrapObjects);
			GeneratedWriters___Internal.GWrite___System_002ECollections_002EGeneric_002EList_00601_003CSystem_002EInt32_003EFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.Monsters);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_TeleporterFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.Teleporter);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_IndoorEnteredFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.IndoorEntered);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_ChargerFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.Charger);
			GeneratedWriters___Internal.GWrite___Mimic_002EVoice_002ESpeechSystem_002ESpeechType_CrowShopFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.CrowShop);
			GeneratedWriters___Internal.GWrite___System_002ECollections_002EGeneric_002EList_00601_003CMimic_002EVoice_002ESpeechSystem_002EIncomingEvent_003EFishNet_002ESerializing_002EGenerated(writer, speechEvent.GameData.IncomingEvent);
			writer.WriteSingle(speechEvent.LastPlayedTime);
		}

		public static SpeechEvent ReadSpeechEvent(this Reader reader)
		{
			long id = reader.ReadInt64();
			string playerName = reader.ReadString();
			reader.ReadSingle();
			int channels = reader.ReadInt32();
			int sampleRate = reader.ReadInt32();
			byte[] compressedAudioData = reader.ReadArrayAllocated<byte>();
			int originalAudioDataLength = reader.ReadInt32();
			float averageAmplitude = reader.ReadSingle();
			int adjacentPlayerCount = reader.ReadInt32();
			int area = reader.ReadInt32();
			int gameTime = reader.ReadInt32();
			int facingPlayerCount = reader.ReadInt32();
			List<int> collection = new List<int>();
			reader.ReadList(ref collection);
			List<int> collection2 = new List<int>();
			reader.ReadList(ref collection2);
			int teleporter = reader.ReadInt32();
			int indoorEntered = reader.ReadInt32();
			int charger = reader.ReadInt32();
			int crowShop = reader.ReadInt32();
			List<IncomingEvent> collection3 = new List<IncomingEvent>();
			reader.ReadList(ref collection3);
			reader.ReadSingle();
			return new SpeechEvent(gameData: new SpeechEventAdditionalGameData((SpeechType_AdjacentPlayerCount)adjacentPlayerCount, (SpeechType_Area)area, (SpeechType_GameTime)gameTime, (SpeechType_FacingPlayerCount)facingPlayerCount, collection, collection2, (SpeechType_Teleporter)teleporter, (SpeechType_IndoorEntered)indoorEntered, (SpeechType_Charger)charger, (SpeechType_CrowShop)crowShop, collection3), id: id, playerName: playerName, recordedTime: Hub.s.timeutil.GetCurrentTickSec(), channels: channels, sampleRate: sampleRate, compressedAudioData: compressedAudioData, originalAudioDataLength: originalAudioDataLength, averageAmplitude: averageAmplitude, lastPlayedTime: Hub.s.timeutil.GetCurrentTickSec());
		}
	}
}
