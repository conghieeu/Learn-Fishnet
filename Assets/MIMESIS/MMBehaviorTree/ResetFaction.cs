namespace MMBehaviorTree
{
	public class ResetFaction : BehaviorAction
	{
		public ResetFaction(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new ResetFaction(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState obj = state as BehaviorTreeState;
			obj.Self.ResetFaction();
			obj.Self.VRoom.IterateAllMonster(delegate(VActor monster)
			{
				monster.AIControlUnit.UpdateAggroTable();
			});
			return BehaviorResult.SUCCESS;
		}
	}
}
