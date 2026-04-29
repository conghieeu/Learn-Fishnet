using System;

public class Cooltime : CRDecorator
{
	public long CooltimeMS { get; set; }

	public Cooltime(IComposite behavior, long cooltimeMS)
		: base(behavior)
	{
		CooltimeMS = cooltimeMS;
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
			long decoratorActivatedTime = behaviorTreeState.GetDecoratorActivatedTime(DecoratorID);
			if (decoratorActivatedTime == -1)
			{
				Push(state);
				behaviorTreeState.RegisterDecoratorActivatedTime(DecoratorID);
			}
			else if (decoratorActivatedTime > 0)
			{
				if (Hub.s.timeutil.GetCurrentTickMilliSec() - decoratorActivatedTime < CooltimeMS)
				{
					return BehaviorResult.FAILURE;
				}
				Push(state);
				behaviorTreeState.RemoveDecoratorActivatedTime(DecoratorID);
			}
			behaviorResult = Composite.Behave(state);
		}
		switch (behaviorResult)
		{
		case BehaviorResult.RUNNING:
			return BehaviorResult.RUNNING;
		case BehaviorResult.SUCCESS:
			Pop(state);
			behaviorTreeState.AllocateDecoratorActivatedTime(DecoratorID);
			return BehaviorResult.SUCCESS;
		case BehaviorResult.FAILURE:
			Pop(state);
			behaviorTreeState.RemoveDecoratorActivatedTime(DecoratorID);
			return BehaviorResult.FAILURE;
		default:
			throw new Exception("Invalid BehaviorResult");
		}
	}

	public override IComposite Clone()
	{
		return new Cooltime(Composite.Clone(), CooltimeMS);
	}
}
