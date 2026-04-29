using System;
using System.Collections.Generic;

public class GELInitMainteneceRoom : IGameEventLog
{
	public long RoomSessionID;

	public List<GELShopData> ShopItems = new List<GELShopData>();

	public DateTime Timestamp;

	public GELInitMainteneceRoom()
		: base(GELType.InitMaintenenceRoom)
	{
	}
}
