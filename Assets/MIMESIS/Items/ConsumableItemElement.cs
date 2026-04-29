using System;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;

public sealed class ConsumableItemElement : ItemElement
{
	public readonly ConsumeItemType Type;

	public readonly BulletType BulletType;

	public int RemainCount { get; private set; }

	public ConsumableItemElement(int itemMasterID, long itemID, bool isFake, int count = 0, int price = 0, InventoryController? parent = null)
		: base(ItemType.Consumable, itemMasterID, itemID, isFake, price, parent)
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
		if (itemInfo == null || !(itemInfo is ItemConsumableInfo itemConsumableInfo))
		{
			throw new Exception($"ConsumableItemElement: itemInfo is not ItemConsumableInfo. itemMasterID: {itemMasterID}");
		}
		Type = itemConsumableInfo.ConsumeType;
		BulletType = itemConsumableInfo.BulletType;
		RemainCount = ((count == 0) ? itemConsumableInfo.DefaultProvideCount : count);
	}

	public override ItemInfo toItemInfo()
	{
		return new ItemInfo
		{
			itemType = ItemType,
			itemID = ItemID,
			itemMasterID = ItemMasterID,
			stackCount = RemainCount,
			isFake = IsFake,
			price = Price
		};
	}

	public override MMSaveGameDataItemElement ToMMSaveGameDataItemElement()
	{
		return new MMSaveGameDataItemElement
		{
			ItemType = ItemType,
			ItemMasterID = ItemMasterID,
			RemainCount = RemainCount,
			Price = Price
		};
	}

	public void DecreaseCount(int count)
	{
		RemainCount -= count;
	}

	public override string ToString()
	{
		return $"{base.ToString()}, Type: {Type}, BulletType: {BulletType}, RemainCount: {RemainCount}";
	}

	public override ItemElement Clone()
	{
		return new ConsumableItemElement(ItemMasterID, ItemID, IsFake, RemainCount, Price, Parent);
	}
}
