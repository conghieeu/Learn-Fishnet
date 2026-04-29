using System;

public class RepeatUntilSucceed : CRDecorator
{
	public RepeatUntilSucceed(IComposite behavior)
		: base(behavior)
	{
	}

	public override BehaviorResult Behave(IBehaviorTreeState state, BehaviorResult nestedResult = BehaviorResult.NONE)
	{
		if (!(state is BehaviorTreeState behaviorTreeState))
		{
			throw new Exception("Invalid IBehaviorTreeState");
		}
		BehaviorResult behaviorResult;
		if (nestedResult != BehaviorResult.NONE)
		{
			behaviorResult = nestedResult;
		}
		else
		{
			if (behaviorTreeState.GetDecoratorActivatedCount(DecoratorID) == -1)
			{
				Push(state);
				behaviorTreeState.RegisterDecoratorActivatedCount(DecoratorID);
			}
			behaviorResult = Composite.Behave(state);
		}
		switch (behaviorResult)
		{
		case BehaviorResult.RUNNING:
			return BehaviorResult.RUNNING;
		case BehaviorResult.SUCCESS:
			Pop(state);
			behaviorTreeState.RemoveDecoratorActivatedCount(DecoratorID);
			return BehaviorResult.SUCCESS;
		case BehaviorResult.FAILURE:
			behaviorTreeState.IncreaseDecoratorActivatedCount(DecoratorID);
			return BehaviorResult.RUNNING;
		default:
			throw new Exception("Invalid BehaviorResult");
		}
	}

	public override IComposite Clone()
	{
		return new RepeatUntilSucceed(Composite.Clone());
	}
}
