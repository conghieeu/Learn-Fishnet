namespace MMBehaviorTree
{
	public class ResetUseScrapScanner : BehaviorAction
	{
		public ResetUseScrapScanner(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ResetUseScrapScanner(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.ResetUseScrapScanner();
			return BehaviorResult.SUCCESS;
		}
	}
}
