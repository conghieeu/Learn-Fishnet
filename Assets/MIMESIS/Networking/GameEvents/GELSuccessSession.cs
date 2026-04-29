using System;
using System.Collections.Generic;

public class GELSuccessSession : IGameEventLog
{
	public long RoomSessionID;

	public List<int> remainScraps = new List<int>();

	public int SessionCycleCount;

	public int RemainCurrency;

	public DateTime Timestamp;

	public GELSuccessSession()
		: base(GELType.SuccessSession)
	{
	}
}
