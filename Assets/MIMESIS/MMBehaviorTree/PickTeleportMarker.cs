using System;
using ReluProtocol;

namespace MMBehaviorTree
{
	public class PickTeleportMarker : BehaviorAction
	{
		private BTTargetPickRule _pickRule;

		private AIRangeType _rangeType;

		private bool _checkHeight;

		public PickTeleportMarker(string[] param)
			: base(param)
		{
			if (param.Length != 3)
			{
				throw new Exception("MoveToRandom requires 3 parameters");
			}
			if (!Enum.TryParse<BTTargetPickRule>(param[0], ignoreCase: true, out _pickRule))
			{
				throw new ArgumentException("Invalid rule " + param[0]);
			}
			if (!Enum.TryParse<AIRangeType>(param[1], ignoreCase: true, out _rangeType))
			{
				throw new ArgumentException("Invalid range type " + param[1]);
			}
			if (!bool.TryParse(param[2], out _checkHeight))
			{
				throw new ArgumentException("Invalid check height " + param[2]);
			}
		}

		public override IComposite Clone()
		{
			return new PickTeleportMarker(new string[3]
			{
				_pickRule.ToString(),
				_rangeType.ToString(),
				_checkHeight.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.PickedTeleportPoint != null)
			{
				return BehaviorResult.SUCCESS;
			}
			MapMarker_TeleportStartPoint teleportStartPoint = behaviorTreeState.Self.VRoom.GetTeleportStartPoint(behaviorTreeState.Self, _pickRule, _rangeType, _checkHeight);
			if (teleportStartPoint == null)
			{
				return BehaviorResult.FAILURE;
			}
			behaviorTreeState.SetTeleportPoint(teleportStartPoint);
			PosWithRot pos = teleportStartPoint.Pos;
			PosWithRot posWithRot = new PosWithRot();
			posWithRot.x = pos.x;
			posWithRot.y = pos.y;
			posWithRot.z = pos.z;
			posWithRot.yaw = 0f;
			behaviorTreeState.SetTargetPosition(posWithRot);
			return BehaviorResult.SUCCESS;
		}
	}
}
