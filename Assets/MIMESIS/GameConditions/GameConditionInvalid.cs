using Bifrost.ConstEnum;

public class GameConditionInvalid : BaseGameCondition
{
	public GameConditionInvalid()
		: base(DefCondition.Invalid)
	{
	}

	public override bool Correct(IGameCondition info)
	{
		return info is GameConditionInvalid;
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
		info = new GameConditionInvalid();
		info.SetIndex(base.ConditionIndex);
	}

	public override IGameCondition Clone()
	{
		GameConditionInvalid gameConditionInvalid = new GameConditionInvalid();
		gameConditionInvalid.SetIndex(base.ConditionIndex);
		return gameConditionInvalid;
	}

	public override void ApplyCurrentCondition(int conditionValue)
	{
	}

	public override void ForceComplete()
	{
	}

	public override GameConditionParamType GetLinkedParamType()
	{
		return GameConditionParamType.None;
	}
}
