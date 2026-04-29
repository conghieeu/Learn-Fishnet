namespace MMBehaviorTree
{
	public class ReleaseItem : BehaviorAction
	{
		public ReleaseItem(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ReleaseItem(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).Self.HandleReleaseItem(0);
			return BehaviorResult.SUCCESS;
		}
	}
}
