using Bifrost.ConstEnum;

public interface IGameAction
{
	GameActionState State { get; }

	bool Correct(IGameAction action);

	bool Progress();

	void SetFailed();

	void SetComplete();

	void Clone(ref IGameAction action);

	bool IsComplete();

	IGameAction Clone();

	string GetActionName();

	GameActionParamType GetLinkedParamType();

	void RegisterCompleteDelegate(GameActionComplete deleFunc);

	bool HasCompleteChecker();
}
