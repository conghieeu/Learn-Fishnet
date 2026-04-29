using ReluProtocol;
using ReluProtocol.Enum;

namespace MMBehaviorTree
{
	public class RunDLMovementAgent : BehaviorAction
	{
		public RunDLMovementAgent(string[] param)
			: base(param)
		{
		}

		public override IComposite Clone()
		{
			return new RunDLMovementAgent(new string[0]);
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			AIController bTAIController = behaviorTreeState.BTAIController;
			bTAIController.ToggleDLAgent(flag: true);
			if (!bTAIController.GetTargetPosWithDLAgent(out PosWithRot resultPos, out ActorMoveType moveType, out bool calcAngle))
			{
				return BehaviorResult.FAILURE;
			}
			behaviorTreeState.BTMovementController.TurnAngle(resultPos.yaw, resultPos.pitch);
			if (!behaviorTreeState.Self.VRoom.FindNearestPoly(resultPos.toVector3(), out var nearestPos, 1f))
			{
				return BehaviorResult.FAILURE;
			}
			behaviorTreeState.BTMovementController.PathMoveStart(nearestPos, moveType, calcAngle);
			return BehaviorResult.SUCCESS;
		}
	}
}
