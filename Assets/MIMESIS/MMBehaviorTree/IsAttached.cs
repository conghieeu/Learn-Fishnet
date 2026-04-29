namespace MMBehaviorTree
{
	public class IsAttached : Conditional
	{
		public IsAttached(IComposite composite, string[] param)
			: base(composite, param)
		{
		}

		public override IComposite Clone()
		{
			return new IsAttached(this, new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if (!(state as BehaviorTreeState).Self.AttachControlUnit.IsAttached())
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
