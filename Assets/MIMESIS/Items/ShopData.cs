using System.Collections.Generic;
using Bifrost.Cooked;
using Bifrost.ShopGroup;
using Mimic.Actors;

public class ShopData
{
	public class ShopItem
	{
		private int itemMasterId;

		private int originPrice;

		private int dcPrice;

		private string name;

		public ShopItem(int itemMasterId, int originPrice, int dcPrice, string name)
		{
			this.itemMasterId = itemMasterId;
			this.originPrice = originPrice;
			this.dcPrice = dcPrice;
			this.name = name;
		}

		public int GetOriginPrice()
		{
			return originPrice;
		}

		public int GetDCPrice()
		{
			return dcPrice;
		}

		public string GetName()
		{
			return name;
		}
	}

	private Dictionary<int, ShopItem> ShopItems;

	public int shopGroupID { get; private set; }

	private ShopItem CreateShopItem(int itemMasterID, int dcPrice)
	{
		ItemMasterInfo itemMasterInfo = ProtoActor.Inventory.GetItemMasterInfo(itemMasterID);
		if (Hub.s.dataman.ExcelDataManager.ShopGroupDict.TryGetValue(shopGroupID, out ShopGroup_MasterData value))
		{
			if (value.item1_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item1_price, dcPrice, itemMasterInfo.Name);
			}
			if (value.item2_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item2_price, dcPrice, itemMasterInfo.Name);
			}
			if (value.item3_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item3_price, dcPrice, itemMasterInfo.Name);
			}
			if (value.item4_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item4_price, dcPrice, itemMasterInfo.Name);
			}
			if (value.item5_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item5_price, dcPrice, itemMasterInfo.Name);
			}
			if (value.item6_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item6_price, dcPrice, itemMasterInfo.Name);
			}
			if (value.item7_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item7_price, dcPrice, itemMasterInfo.Name);
			}
			if (value.item8_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item8_price, dcPrice, itemMasterInfo.Name);
			}
			if (value.item9_masterid == itemMasterID)
			{
				return new ShopItem(itemMasterID, value.item9_price, dcPrice, itemMasterInfo.Name);
			}
		}
		return null;
	}

	public void BuildShopData(int shopGroupID, Dictionary<int, int> dcPriceList)
	{
		this.shopGroupID = shopGroupID;
		ShopItems = new Dictionary<int, ShopItem>();
		foreach (KeyValuePair<int, int> dcPrice in dcPriceList)
		{
			ShopItem shopItem = CreateShopItem(dcPrice.Key, dcPrice.Value);
			if (shopItem != null)
			{
				ShopItems.Add(dcPrice.Key, shopItem);
			}
		}
	}

	public int GetOriginPrice(int itemMasterId)
	{
		if (ShopItems.TryGetValue(itemMasterId, out var value))
		{
			return value.GetOriginPrice();
		}
		return 0;
	}

	public int GetDCPrice(int itemMasterId)
	{
		if (ShopItems.TryGetValue(itemMasterId, out var value))
		{
			return value.GetDCPrice();
		}
		return 0;
	}

	public string GetName(int itemMasterId)
	{
		if (ShopItems.TryGetValue(itemMasterId, out var value))
		{
			return value.GetName();
		}
		return "";
	}
}
