using System;

namespace MMBehaviorTree
{
	public class CheckTargetPickedTime : Conditional
	{
		private readonly long _checkTime;

		private AITargetType _aiTargetType;

		private readonly BTParamCompareType _compareType;

		public CheckTargetPickedTime(IComposite children, string[] param)
			: base(children, param)
		{
			if (param.Length != 3)
			{
				throw new ArgumentException("CheckTargetPickedTime needs a range to check");
			}
			if (!long.TryParse(param[0], out _checkTime))
			{
				throw new ArgumentException("CheckBTActivateTime needs a long value");
			}
			if (!Enum.TryParse<AITargetType>(param[1], ignoreCase: true, out _aiTargetType))
			{
				throw new ArgumentException("Invalid AITargetType " + param[1]);
			}
			if (!Enum.TryParse<BTParamCompareType>(param[2], ignoreCase: true, out _compareType))
			{
				throw new ArgumentException("Invalid compare type " + param[2]);
			}
		}

		public override IComposite Clone()
		{
			return new CheckTargetPickedTime(this, new string[2]
			{
				_checkTime.ToString(),
				_aiTargetType.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			Hub.s.timeutil.GetCurrentTickMilliSec();
			long num = 0L;
			switch (_aiTargetType)
			{
			case AITargetType.Actor:
				if (behaviorTreeState.Target == null)
				{
					return BehaviorResult.FAILURE;
				}
				num = behaviorTreeState.TargetPickedTime;
				break;
			case AITargetType.Position:
				if (behaviorTreeState.TargetPosition == null)
				{
					return BehaviorResult.FAILURE;
				}
				num = behaviorTreeState.TargetPositionPickedTime;
				break;
			case AITargetType.LevelObject:
				if (behaviorTreeState.TargetLevelObject == null)
				{
					return BehaviorResult.FAILURE;
				}
				num = behaviorTreeState.TargetLevelObjectPickedTime;
				break;
			default:
				return BehaviorResult.FAILURE;
			}
			if (!BTStateUtil.CompareLong(_compareType, num, _checkTime))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
