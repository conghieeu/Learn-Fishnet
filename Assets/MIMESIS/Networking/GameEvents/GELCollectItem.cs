using System;

public class GELCollectItem : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public int StageCount;

	public int ItemMasterID;

	public DateTime Timestamp;

	public GELCollectItem()
		: base(GELType.CollectItem)
	{
	}
}
