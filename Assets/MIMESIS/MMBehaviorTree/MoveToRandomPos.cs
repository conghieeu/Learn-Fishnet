using System;
using System.Globalization;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

namespace MMBehaviorTree
{
	public class MoveToRandomPos : BehaviorBlockingAction
	{
		private double _minRange;

		private double _maxRange;

		private bool _resetTarget;

		private bool _overwrite;

		private ActorMoveType _moveType;

		public MoveToRandomPos(string[] param)
			: base(param)
		{
			if (param.Length != 5)
			{
				throw new Exception("MoveToRandom requires 2 parameters");
			}
			if (!double.TryParse(param[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _minRange))
			{
				throw new Exception("can't parse min range");
			}
			if (!double.TryParse(param[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _maxRange))
			{
				throw new Exception("can't parse max range");
			}
			if (!bool.TryParse(param[2], out _resetTarget))
			{
				throw new Exception("can't parse reset target");
			}
			if (!bool.TryParse(param[3], out _overwrite))
			{
				throw new Exception("can't parse overwrite");
			}
			if (!Enum.TryParse<ActorMoveType>(param[4], ignoreCase: true, out _moveType))
			{
				throw new Exception("MoveToPosition requires 3 parameters");
			}
		}

		public override IComposite Clone()
		{
			return new MoveToRandomPos(new string[5]
			{
				_minRange.ToString(),
				_maxRange.ToString(),
				_resetTarget.ToString(),
				_overwrite.ToString(),
				_moveType.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.Self.CanAction(VActorActionType.Move) != MsgErrorCode.Success)
			{
				return BehaviorResult.FAILURE;
			}
			if (behaviorTreeState.BTMovementController.IsMoving)
			{
				if (behaviorTreeState.TargetPosition != null && !_overwrite)
				{
					return BehaviorResult.SUCCESS;
				}
				if (behaviorTreeState.Target != null && _resetTarget)
				{
					behaviorTreeState.ResetTargetActor();
				}
			}
			Vector3? reachableRandomPosWithLimit = Misc.GetReachableRandomPosWithLimit(behaviorTreeState.Self.PositionVector, _minRange, _maxRange);
			if (!reachableRandomPosWithLimit.HasValue)
			{
				return BehaviorResult.FAILURE;
			}
			PosWithRot posWithRot = new PosWithRot();
			posWithRot.x = reachableRandomPosWithLimit.Value.x;
			posWithRot.y = reachableRandomPosWithLimit.Value.y;
			posWithRot.z = reachableRandomPosWithLimit.Value.z;
			posWithRot.yaw = 0f;
			behaviorTreeState.SetTargetPosition(posWithRot);
			if (behaviorTreeState.BTMovementController.PathMoveStart(reachableRandomPosWithLimit.Value, _moveType, calcAngle: true, behaviorTreeState.FixLookAtTarget) == PathMoveResult.Fail)
			{
				behaviorTreeState.SetTargetPosition(null);
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}

		public override bool IsEnd(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.TargetPosition == null)
			{
				return true;
			}
			switch (behaviorTreeState.BTMovementController.PathMoveStart(behaviorTreeState.TargetPosition.toVector3(), _moveType, calcAngle: true, behaviorTreeState.FixLookAtTarget))
			{
			case PathMoveResult.Fail:
				behaviorTreeState.SetTargetPosition(null);
				return true;
			case PathMoveResult.AlreadyArrived:
				behaviorTreeState.SetTargetPosition(null);
				return true;
			case PathMoveResult.Duplicate:
				return false;
			default:
				if ((double)Misc.Distance(behaviorTreeState.Self.PositionVector, behaviorTreeState.TargetPosition.toVector3()) < 0.5)
				{
					return true;
				}
				return false;
			}
		}

		public override BehaviorResult OnEnd(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).SetTargetPosition(null);
			return base.OnEnd(state);
		}
	}
}
