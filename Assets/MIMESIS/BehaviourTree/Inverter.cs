using System;

public class Inverter : CRDecorator
{
	public Inverter(IComposite behavior)
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
		case BehaviorResult.SUCCESS:
			Pop(state);
			return BehaviorResult.FAILURE;
		case BehaviorResult.FAILURE:
			Pop(state);
			return BehaviorResult.SUCCESS;
		case BehaviorResult.RUNNING:
			return BehaviorResult.RUNNING;
		default:
			throw new Exception("Invalid BehaviorResult");
		}
	}

	public override IComposite Clone()
	{
		return new Inverter(Composite.Clone());
	}
}
