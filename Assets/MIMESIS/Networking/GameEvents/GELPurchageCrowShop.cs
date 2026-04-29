using System;

public class GELPurchageCrowShop : IGameEventLog
{
	public long RoomSessionID;

	public int SessionCycleCount;

	public int StageCount;

	public int ShopMasterID;

	public int inputItemMasterID;

	public int outputItemMasterID;

	public DateTime Timestamp;

	public GELPurchageCrowShop()
		: base(GELType.PurchageCrowShop)
	{
	}
}
