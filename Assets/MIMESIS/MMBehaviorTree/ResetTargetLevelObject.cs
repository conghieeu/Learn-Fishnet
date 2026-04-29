namespace MMBehaviorTree
{
	public class ResetTargetLevelObject : BehaviorAction
	{
		public ResetTargetLevelObject(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ResetTargetLevelObject(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).SetTargetLevelObject(null);
			return BehaviorResult.SUCCESS;
		}
	}
}
