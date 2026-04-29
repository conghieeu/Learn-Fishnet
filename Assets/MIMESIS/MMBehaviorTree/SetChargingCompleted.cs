namespace MMBehaviorTree
{
	public class SetChargingCompleted : BehaviorAction
	{
		public SetChargingCompleted(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new SetChargingCompleted(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.SetChargingCompleted();
			return BehaviorResult.SUCCESS;
		}
	}
}
