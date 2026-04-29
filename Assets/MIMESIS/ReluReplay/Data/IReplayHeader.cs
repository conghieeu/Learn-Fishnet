using System.Collections.Generic;

namespace ReluReplay.Data
{
	public interface IReplayHeader
	{
		void SetDungeonMasterID(int dungeonMasterID);

		int GetDungeonMasterID();

		void SetDungeonRandSeed(int dungeonRandSeed);

		int GetDungeonRandSeed();

		void SetReplayRecordStartTime(long startTime);

		long GetReplayRecordStartTime();

		long GetReplayRecordEndTime();

		List<int> GetPlayerActorIDs();

		List<string> GetPlayerActorNames();

		List<int> GetMapInfos();

		ReplaySaveInfo GetSaveInfo();
	}
}
