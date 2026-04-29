using System;

namespace MMBehaviorTree
{
	public class CheckHP : Conditional
	{
		private readonly int _percent;

		private readonly BTParamCompareType _compareType;

		public CheckHP(IComposite children, string[] param)
			: base(children, param)
		{
			if (param.Length != 2)
			{
				throw new ArgumentException("CheckTargetPickedTime needs a range to check");
			}
			if (!int.TryParse(param[0], out _percent))
			{
				throw new ArgumentException("CheckTargetPickedTime percent must be between 0 and 100");
			}
			if (!Enum.TryParse<BTParamCompareType>(param[1], ignoreCase: true, out _compareType))
			{
				throw new ArgumentException("CheckTargetPickedTime aiTargetType must be a valid AITargetType");
			}
		}

		public override IComposite Clone()
		{
			return new CheckHP(this, new string[2]
			{
				_percent.ToString(),
				_compareType.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState obj = state as BehaviorTreeState;
			long currentHP = obj.Self.StatControlUnit.GetCurrentHP();
			long specificStatValue = obj.Self.StatControlUnit.GetSpecificStatValue(StatType.HP);
			int valueSrc = (int)(currentHP * 100 / specificStatValue);
			if (!BTStateUtil.CompareInt(_compareType, valueSrc, _percent))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
