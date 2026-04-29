using System.Collections.Generic;

namespace ReluReplay.Data
{
	public struct ReplayMetaData
	{
		public int NextDungeonMasterID;

		public int RandDungeonSeed;

		public long RecordStartTime;

		public long RecordEndTime;

		public int TotalPlayPacketCount;

		public int TotalVoicePacketCount;

		public int TotalPlayerCount;

		public int TotalMonsterCount;

		public int TotalMimicCount;

		public string FilePath;

		public List<PlayerMetaInfo> PlayerInfoList;

		public int MapInfoDanger;

		public int MapInfoToxicity;

		public int MapInfoReward;

		public List<int> TramUpgradeIDs;
	}
}
