namespace MMBehaviorTree
{
	public class UsePickedInvenItem : BehaviorAction
	{
		public UsePickedInvenItem(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new UsePickedInvenItem(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if (!(state as BehaviorTreeState).BTInventoryController.UseItemByAI())
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
