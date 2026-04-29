using System;
using System.Globalization;

namespace MMBehaviorTree
{
	public class CheckPlayerCountInRadius : Conditional
	{
		private float _range;

		private string _comparator;

		private int _count;

		private bool _checkHeight;

		public CheckPlayerCountInRadius(IComposite composite, string[] param)
			: base(composite, param)
		{
			if (param.Length != 4)
			{
				throw new ArgumentException("CheckTargetCountInRadius needs a range to check");
			}
			if (!float.TryParse(param[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _range))
			{
				throw new ArgumentException("Invalid range " + param[0]);
			}
			_comparator = param[1];
			if (!int.TryParse(param[2], out _count))
			{
				throw new ArgumentException("Invalid count " + param[2]);
			}
			if (!bool.TryParse(param[3], out _checkHeight))
			{
				throw new ArgumentException("Invalid check height " + param[3]);
			}
		}

		public override IComposite Clone()
		{
			return new CheckPlayerCountInRadius(this, new string[4]
			{
				_range.ToString(),
				_comparator,
				_count.ToString(),
				_checkHeight.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			int aggroPlayerCount = (state as BehaviorTreeState).BTAIController.GetAggroPlayerCount(_checkHeight, _range);
			if (!Enum.TryParse<BTParamCompareType>(_comparator, out var result))
			{
				Logger.RError("Invalid compare type " + _comparator);
				return BehaviorResult.FAILURE;
			}
			if (!BTStateUtil.CompareInt(result, aggroPlayerCount, _count))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
