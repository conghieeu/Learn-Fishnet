using UnityEngine;

public class BehaviorDelayedAction : IComposite
{
	public readonly IComposite Action;

	public readonly long DelayTime;

	public BehaviorDelayedAction(IComposite action, long delayTime)
	{
		Action = action;
		DelayTime = delayTime;
	}

	public override BehaviorResult Behave(IBehaviorTreeState state, BehaviorResult nestedResult = BehaviorResult.NONE)
	{
		if (nestedResult != BehaviorResult.NONE)
		{
			Debug.LogWarning("NestedResult is not NONE in BehaviorDelayedAction. " + Action.GetType().Name);
			return BehaviorResult.FAILURE;
		}
		if (!state.ReserveDealyedAction(this, DelayTime))
		{
			return BehaviorResult.FAILURE;
		}
		return BehaviorResult.SUCCESS;
	}

	public override IComposite Clone()
	{
		return new BehaviorDelayedAction(Action.Clone(), DelayTime);
	}
}
