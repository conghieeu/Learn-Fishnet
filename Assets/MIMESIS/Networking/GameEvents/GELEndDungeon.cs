using System;
using System.Collections.Generic;
using ReluProtocol.Enum;

public class GELEndDungeon : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public int StageCount;

	public int DungeonMasterID;

	public List<(int actorID, ReasonOfDeath reason, long contaVal, long hp)> playerStates = new List<(int, ReasonOfDeath, long, long)>();

	public List<int> collectedScraps = new List<int>();

	public DateTime Timestamp;

	public GELEndDungeon()
		: base(GELType.EndDungeon)
	{
	}
}
