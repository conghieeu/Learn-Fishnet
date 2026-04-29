public interface IGameCondition
{
	bool Correct(IGameCondition info);

	bool IsComplete();

	bool IsFailed();

	bool Progress(int accumulateCount);

	int GetCurrentCount();

	void Clone(ref IGameCondition? info);

	IGameCondition Clone();

	void ApplyCurrentCondition(int conditionValue);

	void ForceComplete();

	void SetIndex(int index);

	new string ToString();

	GameConditionParamType GetLinkedParamType();
}
