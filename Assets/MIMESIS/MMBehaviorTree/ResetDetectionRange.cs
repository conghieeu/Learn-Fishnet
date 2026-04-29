namespace MMBehaviorTree
{
	public class ResetDetectionRange : BehaviorAction
	{
		public ResetDetectionRange(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ResetDetectionRange(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.SetSightRange(0f);
			return BehaviorResult.SUCCESS;
		}
	}
}
