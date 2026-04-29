using System;
using System.Globalization;
using ReluProtocol;

namespace MMBehaviorTree
{
	public class CheckDistanceToTarget : Conditional
	{
		private AIRangeType _rangeType;

		private double _range;

		private BTParamCompareType compareType;

		public CheckDistanceToTarget(IComposite composite, string[] param)
			: base(composite, param)
		{
			if (param.Length != 3)
			{
				throw new ArgumentException("CheckDistanceToTarget needs a range to check");
			}
			if (!Enum.TryParse<AIRangeType>(param[0], ignoreCase: true, out _rangeType))
			{
				throw new ArgumentException("Invalid range type " + param[0]);
			}
			if (!double.TryParse(param[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _range))
			{
				throw new ArgumentException("Invalid range " + param[1]);
			}
			if (!Enum.TryParse<BTParamCompareType>(param[2], ignoreCase: true, out compareType))
			{
				throw new ArgumentException("Invalid compare type " + param[2]);
			}
		}

		public override IComposite Clone()
		{
			return new CheckDistanceToTarget(this, new string[3]
			{
				_rangeType.ToString(),
				_range.ToString(),
				compareType.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.Target == null)
			{
				return BehaviorResult.FAILURE;
			}
			PosWithRot position = behaviorTreeState.Target.Position;
			switch (_rangeType)
			{
			case AIRangeType.Absolute:
			{
				double valueSrc = VWorldUtil.Distance(position, behaviorTreeState.Self.Position);
				if (BTStateUtil.CompareDouble(compareType, valueSrc, _range))
				{
					return BehaviorResult.SUCCESS;
				}
				break;
			}
			case AIRangeType.ByNavMesh:
			{
				float navDistance = Hub.s.vworld.GetNavDistance(behaviorTreeState.Self.PositionVector, behaviorTreeState.Target.PositionVector);
				if (BTStateUtil.CompareDouble(compareType, navDistance, _range))
				{
					return BehaviorResult.SUCCESS;
				}
				break;
			}
			}
			return BehaviorResult.FAILURE;
		}
	}
}
