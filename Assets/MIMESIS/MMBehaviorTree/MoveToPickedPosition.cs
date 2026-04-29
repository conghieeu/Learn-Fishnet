using System;
using ReluProtocol.Enum;

namespace MMBehaviorTree
{
	public class MoveToPickedPosition : BehaviorAction
	{
		private ActorMoveType _moveType;

		public MoveToPickedPosition(string[] param)
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
			return new MoveToPickedPosition(new string[1] { _moveType.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.Self.CanAction(VActorActionType.Move) != MsgErrorCode.Success)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.TargetPosition == null)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.BTMovementController.PathMoveStart(behaviorTreeState.TargetPosition.toVector3(), _moveType, calcAngle: true, behaviorTreeState.FixLookAtTarget) == PathMoveResult.Fail)
			{
				behaviorTreeState.SetTargetPosition(null);
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
