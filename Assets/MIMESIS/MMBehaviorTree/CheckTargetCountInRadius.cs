using System;
using System.Collections.Generic;
using System.Globalization;

namespace MMBehaviorTree
{
	public class CheckTargetCountInRadius : Conditional
	{
		private float _range;

		private string _comparator;

		private int _count;

		private bool _checkHeight;

		private int _factionTier;

		public CheckTargetCountInRadius(IComposite composite, string[] param)
			: base(composite, param)
		{
			if (param.Length != 5)
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
			if (!int.TryParse(param[4], out _factionTier))
			{
				throw new ArgumentException("Invalid faction tier " + param[4]);
			}
		}

		public override IComposite Clone()
		{
			return new CheckTargetCountInRadius(this, new string[5]
			{
				_range.ToString(),
				_comparator,
				_count.ToString(),
				_checkHeight.ToString(),
				_factionTier.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			List<VCreature> aggroActors = (state as BehaviorTreeState).BTAIController.GetAggroActors(_checkHeight, _range, _factionTier);
			if (!Enum.TryParse<BTParamCompareType>(_comparator, out var result))
			{
				Logger.RError("Invalid compare type " + _comparator);
				return BehaviorResult.FAILURE;
			}
			if (!BTStateUtil.CompareInt(result, aggroActors.Count, _count))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
