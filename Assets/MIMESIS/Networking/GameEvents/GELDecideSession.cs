using System;
using System.Collections.Generic;

public class GELDecideSession : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public List<GELShopData> ReplacedShopItems = new List<GELShopData>();

	public List<int> ScrapItems = new List<int>();

	public DateTime Timestamp;

	public GELDecideSession()
		: base(GELType.DecideSession)
	{
	}
}
