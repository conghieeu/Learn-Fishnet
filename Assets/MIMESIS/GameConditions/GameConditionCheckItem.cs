using Bifrost.ConstEnum;

public class GameConditionCheckItem : BaseGameCondition
{
	public int ItemMasterID { get; private set; }

	public int Count { get; private set; }

	public bool Accomplished { get; private set; }

	public GameConditionCheckItem(int itemMasterID, int count)
		: base(DefCondition.CHECK_ITEM)
	{
		ItemMasterID = itemMasterID;
		Count = count;
		Accomplished = false;
	}

	public override bool Correct(IGameCondition info)
	{
		if (info is GameConditionCheckItem gameConditionCheckItem)
		{
			if (gameConditionCheckItem.ItemMasterID == ItemMasterID)
			{
				return gameConditionCheckItem.Count >= Count;
			}
			return false;
		}
		return false;
	}

	public override bool IsComplete()
	{
		return Accomplished;
	}

	public override bool IsFailed()
	{
		return false;
	}

	public override bool Progress(int accumulateCount)
	{
		if (Accomplished)
		{
			return false;
		}
		Accomplished = true;
		return true;
	}

	public override int GetCurrentCount()
	{
		if (!Accomplished)
		{
			return 0;
		}
		return 1;
	}

	public override void Clone(ref IGameCondition? info)
	{
		info = new GameConditionCheckItem(ItemMasterID, Count);
		info.SetIndex(base.ConditionIndex);
	}

	public override IGameCondition Clone()
	{
		GameConditionCheckItem gameConditionCheckItem = new GameConditionCheckItem(ItemMasterID, Count);
		gameConditionCheckItem.SetIndex(base.ConditionIndex);
		return gameConditionCheckItem;
	}

	public override void ApplyCurrentCondition(int conditionValue)
	{
		if (conditionValue == 1)
		{
			Accomplished = true;
		}
	}

	public override void ForceComplete()
	{
		Accomplished = true;
	}

	public override string ToString()
	{
		return $"{base.ObjType}:{ItemMasterID}:{Count}";
	}

	public override GameConditionParamType GetLinkedParamType()
	{
		return GameConditionParamType.Actor;
	}
}
