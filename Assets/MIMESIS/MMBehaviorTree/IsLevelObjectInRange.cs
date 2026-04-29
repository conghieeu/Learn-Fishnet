using System;
using System.Globalization;

namespace MMBehaviorTree
{
	public class IsLevelObjectInRange : Conditional
	{
		private readonly LevelObjectClientType _staticLevelObjectType;

		private readonly double _range;

		private readonly bool _checkHeight;

		public IsLevelObjectInRange(IComposite children, string[] param)
			: base(children, param)
		{
			if (param.Length != 3)
			{
				throw new ArgumentException("IsLevelObjectInRange needs a range to check");
			}
			if (!Enum.TryParse<LevelObjectClientType>(param[0], ignoreCase: true, out _staticLevelObjectType))
			{
				throw new ArgumentException("IsLevelObjectInRange needs a StaticLevelObjectType value");
			}
			if (!double.TryParse(param[1], NumberStyles.Any, CultureInfo.InvariantCulture, out _range))
			{
				throw new ArgumentException("IsLevelObjectInRange needs a double value");
			}
			if (!bool.TryParse(param[2], out _checkHeight))
			{
				throw new ArgumentException("IsLevelObjectInRange needs a boolean value for height check");
			}
		}

		public override IComposite Clone()
		{
			return new IsLevelObjectInRange(this, new string[3]
			{
				_staticLevelObjectType.ToString(),
				_range.ToString(),
				_checkHeight.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.Self.VRoom.FindLevelObjectByType(behaviorTreeState.Self.PositionVector, _range, _staticLevelObjectType, _checkHeight) == null)
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
