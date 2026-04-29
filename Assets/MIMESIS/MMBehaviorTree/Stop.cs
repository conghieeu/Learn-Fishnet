namespace MMBehaviorTree
{
	public class Stop : BehaviorAction
	{
		public Stop(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new Stop(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTMovementController.StopMove();
			return BehaviorResult.SUCCESS;
		}
	}
}
