public interface IBehaviorTreeState
{
	string TriggeredAction { get; }

	void SnapBTComposite(string actionName, string[]? actionParams, string actionType);

	bool CheckBTNodeCooltime(int nodeId);

	void LogBTCompositeTrack(string message);

	bool ReserveDealyedAction(BehaviorDelayedAction action, long delayTime);

	bool PushComposite(IComposite composite);

	bool PopComposite(IComposite composite);

	bool SaveChildIndex(GroupComposite composite, int index);

	int GetSavedChildIndex(GroupComposite composite);
}
