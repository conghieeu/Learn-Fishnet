using System;
using ReluProtocol.Enum;

namespace MMBehaviorTree
{
	public class MoveToTarget : BehaviorAction
	{
		private ActorMoveType _moveType;

		public MoveToTarget(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new Exception("MoveToPosition requires 3 parameters");
			}
			if (!Enum.TryParse<ActorMoveType>(param[0], ignoreCase: true, out _moveType))
			{
				throw new Exception("MoveToPosition requires 3 parameters");
			}
		}

		public override IComposite Clone()
		{
			return new MoveToTarget(new string[1] { _moveType.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.Target == null)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.Self.CanAction(VActorActionType.Move) != MsgErrorCode.Success)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.BTMovementController.PathMoveStart(behaviorTreeState.Target.PositionVector, _moveType, calcAngle: true, behaviorTreeState.FixLookAtTarget) == PathMoveResult.Fail)
			{
				behaviorTreeState.ResetTargetActor();
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
