namespace MMBehaviorTree
{
	public class ResetDLTimeToAttackTimer : BehaviorAction
	{
		public ResetDLTimeToAttackTimer(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ResetDLTimeToAttackTimer(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.ResetDLTimeToAttackTimer();
			return BehaviorResult.SUCCESS;
		}
	}
}
