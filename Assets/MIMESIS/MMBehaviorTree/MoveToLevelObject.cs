using System;
using ReluProtocol.Enum;
using UnityEngine;

namespace MMBehaviorTree
{
	public class MoveToLevelObject : BehaviorAction
	{
		private ActorMoveType _moveType;

		public MoveToLevelObject(string[] param)
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
			return new MoveToLevelObject(new string[1] { _moveType.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.TargetLevelObject == null)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.Self.CanAction(VActorActionType.Move) != MsgErrorCode.Success)
			{
				return BehaviorResult.FAILURE;
			}
			Vector3 pos = behaviorTreeState.TargetLevelObject.Pos;
			if (!behaviorTreeState.Self.VRoom.FindNearestPoly(pos, out var nearestPos))
			{
				Logger.RError($"Failed to find nearest poly for {pos} in PickLevelObject");
				behaviorTreeState.SetTargetLevelObject(null);
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.BTMovementController.PathMoveStart(nearestPos, _moveType, calcAngle: true, behaviorTreeState.FixLookAtTarget) == PathMoveResult.Fail)
			{
				Logger.RError($"Failed to move to level object {behaviorTreeState.TargetLevelObject.ID} at {pos}");
				behaviorTreeState.SetTargetLevelObject(null);
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
