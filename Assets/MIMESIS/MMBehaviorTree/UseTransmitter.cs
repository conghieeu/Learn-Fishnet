namespace MMBehaviorTree
{
	public class UseTransmitter : BehaviorAction
	{
		public UseTransmitter(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new UseTransmitter(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (!Hub.s.voiceman.TrySpawnMimicTransmitterVoice(behaviorTreeState.Self.ObjectID))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
