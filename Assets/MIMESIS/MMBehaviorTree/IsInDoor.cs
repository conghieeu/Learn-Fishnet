namespace MMBehaviorTree
{
	public class IsInDoor : Conditional
	{
		public IsInDoor(IComposite composite, string[] param)
			: base(composite, param)
		{
		}

		public override IComposite Clone()
		{
			return new IsInDoor(this, new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if (!(state as BehaviorTreeState).Self.IsIndoor)
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
