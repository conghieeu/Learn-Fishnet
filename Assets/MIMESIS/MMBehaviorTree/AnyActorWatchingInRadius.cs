using System;
using System.Collections.Generic;
using System.Globalization;

namespace MMBehaviorTree
{
	public class AnyActorWatchingInRadius : Conditional
	{
		private float _radius;

		private float _angle;

		public AnyActorWatchingInRadius(IComposite composite, string[] param)
			: base(composite, param)
		{
			if (param.Length != 2)
			{
				throw new ArgumentException("CheckTargetCountInRadius needs a range to check");
			}
			if (!float.TryParse(param[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _radius))
			{
				throw new ArgumentException("Invalid range " + param[0]);
			}
			if (!float.TryParse(param[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _angle))
			{
				throw new ArgumentException("Invalid angle " + param[1]);
			}
		}

		public override IComposite Clone()
		{
			return new AnyActorWatchingInRadius(this, new string[2]
			{
				_radius.ToString(CultureInfo.InvariantCulture),
				_angle.ToString(CultureInfo.InvariantCulture)
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			List<VPlayer> playersInRange = behaviorTreeState.Self.VRoom.GetPlayersInRange(behaviorTreeState.Self.PositionVector, 0f, _radius, checkHeight: true);
			if (playersInRange.Count == 0)
			{
				return BehaviorResult.FAILURE;
			}
			foreach (VPlayer item in playersInRange)
			{
				if (VWorldUtil.CheckVisible(behaviorTreeState.Self.PositionVector, item.PositionVector, behaviorTreeState.Self.Height, item.Height, behaviorTreeState.Self.HitCollisionRadius, item.HitCollisionRadius) && VWorldUtil.CheckTargetLookingAtMe(item.PositionVector, behaviorTreeState.Self.PositionVector, item.Position.yaw, _angle))
				{
					return BehaviorResult.SUCCESS;
				}
			}
			return BehaviorResult.FAILURE;
		}
	}
}
