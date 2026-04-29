namespace MMBehaviorTree
{
	public class ResetTargetActor : BehaviorAction
	{
		public ResetTargetActor(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ResetTargetActor(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).ResetTargetActor();
			return BehaviorResult.SUCCESS;
		}
	}
}
