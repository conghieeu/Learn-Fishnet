namespace MMBehaviorTree
{
	public class EnableDLAgent : BehaviorAction
	{
		public EnableDLAgent(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new EnableDLAgent(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.ToggleDLAgent(flag: true);
			return BehaviorResult.SUCCESS;
		}
	}
}
