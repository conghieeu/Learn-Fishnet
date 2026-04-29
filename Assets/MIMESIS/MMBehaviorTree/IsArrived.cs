using System;
using System.Globalization;
using ReluProtocol;

namespace MMBehaviorTree
{
	public class IsArrived : Conditional
	{
		private double _range;

		private AITargetType _aiTargetType;

		public LevelObjectClientType _levelObjectType;

		public IsArrived(IComposite composite, string[] param)
			: base(composite, param)
		{
			if (param.Length != 3)
			{
				throw new ArgumentException("IsArrived needs a range to check");
			}
			if (!double.TryParse(param[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _range))
			{
				throw new ArgumentException("Invalid range " + param[0]);
			}
			if (!Enum.TryParse<AITargetType>(param[1], ignoreCase: true, out _aiTargetType))
			{
				throw new ArgumentException("Invalid AITargetType " + param[1]);
			}
			if (_aiTargetType == AITargetType.LevelObject && !Enum.TryParse<LevelObjectClientType>(param[2], ignoreCase: true, out _levelObjectType))
			{
				throw new ArgumentException("Invalid LevelObjectType " + param[2]);
			}
		}

		public override IComposite Clone()
		{
			return new IsArrived(this, new string[3]
			{
				_range.ToString(),
				_aiTargetType.ToString(),
				_levelObjectType.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.TargetPosition == null && behaviorTreeState.Target == null)
			{
				return BehaviorResult.FAILURE;
			}
			PosWithRot posA;
			switch (_aiTargetType)
			{
			case AITargetType.Actor:
				if (behaviorTreeState.Target == null)
				{
					return BehaviorResult.FAILURE;
				}
				posA = behaviorTreeState.Target.Position;
				break;
			case AITargetType.Position:
				if (behaviorTreeState.TargetPosition == null)
				{
					return BehaviorResult.FAILURE;
				}
				posA = behaviorTreeState.TargetPosition;
				break;
			case AITargetType.LevelObject:
				if (behaviorTreeState.TargetLevelObject == null)
				{
					return BehaviorResult.FAILURE;
				}
				if (behaviorTreeState.TargetLevelObject.DataOrigin.LevelObjectType != _levelObjectType)
				{
					return BehaviorResult.FAILURE;
				}
				posA = behaviorTreeState.TargetLevelObject.Pos.toPosWithRot(0f);
				break;
			default:
				return BehaviorResult.FAILURE;
			}
			if (!(VWorldUtil.Distance(posA, behaviorTreeState.Self.Position) < _range))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
