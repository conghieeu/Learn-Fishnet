namespace MMBehaviorTree
{
	public class AnyActorAttaching : Conditional
	{
		public AnyActorAttaching(IComposite composite, string[] param)
			: base(composite, param)
		{
		}

		public override IComposite Clone()
		{
			return new AnyActorAttaching(this, new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (!behaviorTreeState.Self.AttachControlUnit.IsAttaching() && !behaviorTreeState.Self.AttachControlUnit.IsAttached())
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
