namespace MMBehaviorTree
{
	public class UseTeleportMarker : BehaviorAction
	{
		public UseTeleportMarker(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new UseTeleportMarker(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.PickedTeleportPoint == null)
			{
				return BehaviorResult.FAILURE;
			}
			if (!behaviorTreeState.Self.VRoom.UseTeleportStartPoint(behaviorTreeState.Self, behaviorTreeState.PickedTeleportPoint))
			{
				return BehaviorResult.FAILURE;
			}
			behaviorTreeState.SetTeleportPoint(null);
			behaviorTreeState.SetTargetPosition(null);
			return BehaviorResult.SUCCESS;
		}
	}
}
