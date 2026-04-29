using System;

public class Once : CRDecorator
{
	public Once(IComposite behavior)
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
			int decoratorActivatedCount = behaviorTreeState.GetDecoratorActivatedCount(DecoratorID);
			if (decoratorActivatedCount == -1)
			{
				Push(state);
				behaviorTreeState.RegisterDecoratorActivatedCount(DecoratorID);
			}
			else if (decoratorActivatedCount >= 0)
			{
				return BehaviorResult.FAILURE;
			}
			behaviorResult = Composite.Behave(state);
		}
		switch (behaviorResult)
		{
		case BehaviorResult.RUNNING:
			return BehaviorResult.RUNNING;
		case BehaviorResult.SUCCESS:
			Pop(state);
			behaviorTreeState.IncreaseDecoratorActivatedCount(DecoratorID);
			return BehaviorResult.SUCCESS;
		case BehaviorResult.FAILURE:
			Pop(state);
			return BehaviorResult.FAILURE;
		default:
			throw new Exception("Invalid BehaviorResult");
		}
	}

	public override IComposite Clone()
	{
		return new Once(Composite.Clone());
	}
}
