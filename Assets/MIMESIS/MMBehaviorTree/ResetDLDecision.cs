namespace MMBehaviorTree
{
	public class ResetDLDecision : BehaviorAction
	{
		public ResetDLDecision(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ResetDLDecision(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.ResetDLDecision();
			return BehaviorResult.SUCCESS;
		}
	}
}
