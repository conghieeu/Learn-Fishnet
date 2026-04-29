namespace MMBehaviorTree
{
	public class ExistTarget : Conditional
	{
		public ExistTarget(IComposite composite, string[] param)
			: base(composite, param)
		{
		}

		public override IComposite Clone()
		{
			return new ExistTarget(this, new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if ((state as BehaviorTreeState).Target != null)
			{
				return BehaviorResult.SUCCESS;
			}
			return BehaviorResult.FAILURE;
		}
	}
}
