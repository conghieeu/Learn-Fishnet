using System;
using System.Collections.Generic;

public class GELInitDungeon : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public int StageCount;

	public int DungeonMasterID;

	public List<int> MonsterSpawns = new List<int>();

	public List<int> ScrapSpawns = new List<int>();

	public List<int> Weathers = new List<int>();

	public DateTime Timestamp;

	public GELInitDungeon()
		: base(GELType.InitDungeon)
	{
	}
}
