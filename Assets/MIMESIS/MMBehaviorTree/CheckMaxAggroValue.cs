using System;

namespace MMBehaviorTree
{
	public class CheckMaxAggroValue : Conditional
	{
		private string _comparator;

		private int _compareValue;

		public CheckMaxAggroValue(IComposite composite, string[] param)
			: base(composite, param)
		{
			if (param.Length != 2)
			{
				throw new ArgumentException("CheckMaxAgrroValue . needs to check params " + string.Join(", ", param));
			}
			_comparator = param[0];
			if (!int.TryParse(param[1], out _compareValue))
			{
				throw new ArgumentException("CheckMaxAgrroValue . invalid compareValue " + param[2]);
			}
		}

		public override IComposite Clone()
		{
			return new CheckMaxAggroValue(this, new string[2]
			{
				_comparator,
				_compareValue.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.Target == null)
			{
				return BehaviorResult.FAILURE;
			}
			int maxAggroValue = behaviorTreeState.BTAIController.GetMaxAggroValue();
			if (!Enum.TryParse<BTParamCompareType>(_comparator, out var result))
			{
				Logger.RError("Invalid compare type " + _comparator);
				return BehaviorResult.FAILURE;
			}
			if (!BTStateUtil.CompareInt(result, maxAggroValue, _compareValue))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
