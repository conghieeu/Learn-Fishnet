namespace MMBehaviorTree
{
	public class ResetTargetPosition : BehaviorAction
	{
		public ResetTargetPosition(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ResetTargetPosition(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).SetTargetPosition(null);
			return BehaviorResult.SUCCESS;
		}
	}
}
