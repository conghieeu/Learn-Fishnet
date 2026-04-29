using Bifrost.ConstEnum;

public class GameConditionCheckTramRepair : BaseGameCondition
{
	public bool IsTramRepaired { get; private set; }

	public GameConditionCheckTramRepair(bool isTramRepaired)
		: base(DefCondition.CHECK_TRAM_REPAIR)
	{
		IsTramRepaired = isTramRepaired;
	}

	public override bool Correct(IGameCondition info)
	{
		if (info is GameConditionCheckTramRepair gameConditionCheckTramRepair)
		{
			return gameConditionCheckTramRepair.IsTramRepaired == IsTramRepaired;
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
		info = new GameConditionCheckTramRepair(IsTramRepaired);
		info.SetIndex(base.ConditionIndex);
	}

	public override IGameCondition Clone()
	{
		GameConditionCheckTramRepair gameConditionCheckTramRepair = new GameConditionCheckTramRepair(IsTramRepaired);
		gameConditionCheckTramRepair.SetIndex(base.ConditionIndex);
		return gameConditionCheckTramRepair;
	}

	public override void ApplyCurrentCondition(int conditionValue)
	{
	}

	public override void ForceComplete()
	{
	}

	public override string ToString()
	{
		return $"{base.ObjType}:{IsTramRepaired}";
	}

	public override GameConditionParamType GetLinkedParamType()
	{
		return GameConditionParamType.Actors;
	}
}
