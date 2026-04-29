using System;

public class Succeeder : CRDecorator
{
	public Succeeder(IComposite behavior)
		: base(behavior)
	{
	}

	public override BehaviorResult Behave(IBehaviorTreeState state, BehaviorResult nestedResult = BehaviorResult.NONE)
	{
		BehaviorResult behaviorResult;
		if (nestedResult != BehaviorResult.NONE)
		{
			behaviorResult = nestedResult;
		}
		else
		{
			Push(state);
			behaviorResult = Composite.Behave(state);
		}
		switch (behaviorResult)
		{
		case BehaviorResult.RUNNING:
			return BehaviorResult.RUNNING;
		case BehaviorResult.SUCCESS:
		case BehaviorResult.FAILURE:
			Pop(state);
			return BehaviorResult.SUCCESS;
		default:
			throw new Exception("Invalid BehaviorResult");
		}
	}

	public override IComposite Clone()
	{
		return new Succeeder(Composite.Clone());
	}
}
