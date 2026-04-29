namespace MMBehaviorTree
{
	public class ClearBlackBoard : BehaviorAction
	{
		public ClearBlackBoard(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ClearBlackBoard(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.BlackBoard.Clear();
			return BehaviorResult.SUCCESS;
		}
	}
}
