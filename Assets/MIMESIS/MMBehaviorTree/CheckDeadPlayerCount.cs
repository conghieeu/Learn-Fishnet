using System;

namespace MMBehaviorTree
{
	public class CheckDeadPlayerCount : Conditional
	{
		private readonly int _count;

		private readonly BTParamCompareType _compareType;

		public CheckDeadPlayerCount(IComposite children, string[] param)
			: base(children, param)
		{
			if (param.Length != 2)
			{
				throw new ArgumentException("CheckTargetPickedTime needs a range to check");
			}
			if (!int.TryParse(param[0], out _count))
			{
				throw new ArgumentException("CheckBTActivateTime needs a long value");
			}
			if (!Enum.TryParse<BTParamCompareType>(param[1], ignoreCase: true, out _compareType))
			{
				throw new ArgumentException("Invalid compare type " + param[1]);
			}
		}

		public override IComposite Clone()
		{
			return new CheckDeadPlayerCount(this, new string[2]
			{
				_count.ToString(),
				_compareType.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			int deadPlayerCount = (state as BehaviorTreeState).Self.VRoom.GetDeadPlayerCount();
			if (!BTStateUtil.CompareInt(_compareType, deadPlayerCount, _count))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
