using Bifrost.ConstEnum;

public class GameConditionCheckScrapWeight : BaseGameCondition
{
	public int Weight { get; private set; }

	public GameConditionCheckScrapWeight(int weight)
		: base(DefCondition.CHECK_SCRAP_WEIGHT)
	{
		Weight = weight;
	}

	public override bool Correct(IGameCondition info)
	{
		if (info is GameConditionCheckScrapWeight gameConditionCheckScrapWeight)
		{
			return gameConditionCheckScrapWeight.Weight == Weight;
		}
		return false;
	}

	public override bool IsComplete()
	{
		return false;
	}

	public override bool IsFailed()
	{
		return false;
	}

	public override bool Progress(int accumulateCount)
	{
		return true;
	}

	public override int GetCurrentCount()
	{
		return 0;
	}

	public override void Clone(ref IGameCondition? info)
	{
		info = new GameConditionCheckScrapWeight(Weight);
		info.SetIndex(base.ConditionIndex);
	}

	public override IGameCondition Clone()
	{
		GameConditionCheckScrapWeight gameConditionCheckScrapWeight = new GameConditionCheckScrapWeight(Weight);
		gameConditionCheckScrapWeight.SetIndex(base.ConditionIndex);
		return gameConditionCheckScrapWeight;
	}

	public override void ApplyCurrentCondition(int conditionValue)
	{
	}

	public override void ForceComplete()
	{
	}

	public override string ToString()
	{
		return $"{base.ObjType}:{Weight}";
	}

	public override GameConditionParamType GetLinkedParamType()
	{
		return GameConditionParamType.Actors;
	}
}
