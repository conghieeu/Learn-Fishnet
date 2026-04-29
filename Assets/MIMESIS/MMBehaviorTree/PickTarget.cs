using System;

namespace MMBehaviorTree
{
	public class PickTarget : BehaviorAction
	{
		private BTTargetPickRule _pickRule;

		private AIRangeType _rangeType;

		private bool _checkHeight;

		private bool _reset;

		private int _factionTier;

		public PickTarget(string[] param)
			: base(param)
		{
			if (param.Length != 5)
			{
				throw new ArgumentException("PickTarget needs a rule to pick target");
			}
			if (!Enum.TryParse<BTTargetPickRule>(param[0], out _pickRule))
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
			if (!bool.TryParse(param[3], out _reset))
			{
				throw new ArgumentException("Invalid reset " + param[1]);
			}
			if (!int.TryParse(param[4], out _factionTier))
			{
				throw new ArgumentException("Invalid faction tier " + param[4]);
			}
		}

		public override IComposite Clone()
		{
			return new PickTarget(new string[5]
			{
				_pickRule.ToString(),
				_rangeType.ToString(),
				_checkHeight.ToString(),
				_reset.ToString(),
				_factionTier.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.Target != null && !_reset)
			{
				return BehaviorResult.SUCCESS;
			}
			if (!behaviorTreeState.BTAIController.PickTarget(_pickRule, _rangeType, _checkHeight, _reset, _factionTier))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
