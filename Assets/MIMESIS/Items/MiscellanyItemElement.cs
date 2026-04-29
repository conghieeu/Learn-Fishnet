using Bifrost.ConstEnum;
using ReluProtocol;

public sealed class MiscellanyItemElement : ItemElement
{
	public MiscellanyItemElement(int itemMasterID, long itemID, bool isFake, int price = 0, InventoryController? parent = null)
		: base(ItemType.Miscellany, itemMasterID, itemID, isFake, price, parent)
	{
	}

	public override ItemElement Clone()
	{
		return new MiscellanyItemElement(ItemMasterID, ItemID, IsFake, Price, Parent);
	}

	public override ItemInfo toItemInfo()
	{
		return new ItemInfo
		{
			itemType = ItemType,
			itemID = ItemID,
			itemMasterID = ItemMasterID,
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
			Price = Price
		};
	}

	public override string ToString()
	{
		return base.ToString();
	}
}
