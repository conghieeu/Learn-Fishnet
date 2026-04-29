using Bifrost.ConstEnum;

public abstract class BaseGameCondition : IGameCondition
{
	public int ConditionIndex { get; protected set; }

	public DefCondition ObjType { get; protected set; }

	public BaseGameCondition(DefCondition type)
	{
		ObjType = type;
	}

	public abstract bool Correct(IGameCondition info);

	public abstract bool IsComplete();

	public abstract bool IsFailed();

	public abstract bool Progress(int accumulateCount);

	public bool IsCorrectTarget(BaseGameCondition info)
	{
		if (IsComplete())
		{
			return false;
		}
		if (info.ObjType == ObjType)
		{
			return Correct(info);
		}
		return false;
	}

	public abstract int GetCurrentCount();

	public abstract void Clone(ref IGameCondition? info);

	public abstract IGameCondition Clone();

	public abstract void ApplyCurrentCondition(int conditionValue);

	public abstract void ForceComplete();

	public bool VaildObjType(DefCondition type)
	{
		return ObjType == type;
	}

	public void SetIndex(int index)
	{
		ConditionIndex = index;
	}

	public abstract GameConditionParamType GetLinkedParamType();
}
