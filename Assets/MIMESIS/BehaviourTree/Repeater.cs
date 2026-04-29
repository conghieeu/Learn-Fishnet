using System;

public class Repeater : CRDecorator
{
	public readonly int RepeaterCount;

	public Repeater(int count, IComposite composite)
		: base(composite)
	{
		RepeaterCount = count;
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
			behaviorTreeState.IncreaseDecoratorActivatedCount(DecoratorID);
			if (behaviorTreeState.GetDecoratorActivatedCount(DecoratorID) >= RepeaterCount)
			{
				Pop(state);
				behaviorTreeState.RemoveDecoratorActivatedCount(DecoratorID);
				return BehaviorResult.SUCCESS;
			}
			return BehaviorResult.RUNNING;
		case BehaviorResult.FAILURE:
			Pop(state);
			behaviorTreeState.RemoveDecoratorActivatedCount(DecoratorID);
			return BehaviorResult.FAILURE;
		default:
			throw new Exception("Invalid BehaviorResult");
		}
	}

	public override IComposite Clone()
	{
		return new Repeater(RepeaterCount, Composite.Clone());
	}
}
