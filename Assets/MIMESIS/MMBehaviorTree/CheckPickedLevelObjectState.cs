using System;
using System.Globalization;

namespace MMBehaviorTree
{
	public class CheckPickedLevelObjectState : Conditional
	{
		private readonly int _state;

		private BTParamCompareType _compareType;

		public CheckPickedLevelObjectState(IComposite children, string[] param)
			: base(children, param)
		{
			if (param.Length != 2)
			{
				throw new ArgumentException("Invalid number of parameters for CheckPickedLevelObjectState. Expected 2. Usage: CheckPickedLevelObjectState <State> <CompareType>");
			}
			if (!int.TryParse(param[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out _state))
			{
				throw new ArgumentException("Invalid state parameter: " + param[0]);
			}
			if (!Enum.TryParse<BTParamCompareType>(param[1], ignoreCase: true, out _compareType))
			{
				throw new ArgumentException("Invalid CompareType parameter: " + param[1]);
			}
		}

		public override IComposite Clone()
		{
			return new CheckPickedLevelObjectState(this, new string[2]
			{
				_state.ToString(),
				_compareType.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.TargetLevelObject == null)
			{
				return BehaviorResult.FAILURE;
			}
			if (!(behaviorTreeState.TargetLevelObject is StateLevelObjectInfo stateLevelObjectInfo))
			{
				return BehaviorResult.FAILURE;
			}
			if (!BTStateUtil.CompareInt(_compareType, _state, stateLevelObjectInfo.CurrentState))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
