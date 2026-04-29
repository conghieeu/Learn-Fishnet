using System;

public class GELPurchaseItem : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public int ItemMasterID;

	public int Price;

	public DateTime Timestamp;

	public GELPurchaseItem()
		: base(GELType.PurchaseItem)
	{
	}
}
