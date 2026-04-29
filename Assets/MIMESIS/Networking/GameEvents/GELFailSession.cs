using System;

public class GELFailSession : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public int RemainCurrency;

	public DateTime Timestamp;

	public GELFailSession()
		: base(GELType.FailSession)
	{
	}
}
