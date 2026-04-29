public class CompositeRandom : CRDecorator
{
	public readonly long Probability;

	public CompositeRandom(long probability, IComposite behavior)
		: base(behavior)
	{
		Probability = probability;
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
			long num = SimpleRandUtil.Next(1, 10001);
			if (Probability < num)
			{
				return BehaviorResult.FAILURE;
			}
			Push(state);
			behaviorResult = Composite.Behave(state);
		}
		if (behaviorResult == BehaviorResult.FAILURE || behaviorResult == BehaviorResult.SUCCESS)
		{
			Pop(state);
		}
		return behaviorResult;
	}

	public override IComposite Clone()
	{
		return new CompositeRandom(Probability, Composite.Clone());
	}
}
