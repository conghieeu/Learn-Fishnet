using Bifrost.ConstEnum;

public class GameActionDecreaseItem : GameAction
{
	public int ItemMasterID { get; private set; }

	public int Count { get; private set; }

	public GameActionDecreaseItem(int itemMasterID, int count)
		: base(DefAction.DECREASE_ITEM_COUNT)
	{
		ItemMasterID = itemMasterID;
		Count = count;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionDecreaseItem(ItemMasterID, Count);
	}

	public override IGameAction Clone()
	{
		return new GameActionDecreaseItem(ItemMasterID, Count);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionDecreaseItem gameActionDecreaseItem)
		{
			if (gameActionDecreaseItem.ItemMasterID == ItemMasterID)
			{
				return gameActionDecreaseItem.Count == Count;
			}
			return false;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{base.ActionType}:{ItemMasterID}:{Count}";
	}
}
