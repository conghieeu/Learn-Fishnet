namespace MMBehaviorTree
{
	public class SetHandEmpty : BehaviorAction
	{
		public SetHandEmpty(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new SetHandEmpty(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTInventoryController.SetHandEmptyByAI();
			return BehaviorResult.SUCCESS;
		}
	}
}
