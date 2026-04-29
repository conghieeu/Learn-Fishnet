using System;

namespace MMBehaviorTree
{
	public class CheckBTActivateTime : Conditional
	{
		private readonly long _checkTime;

		public CheckBTActivateTime(IComposite children, string[] param)
			: base(children, param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("CheckBTActivateTime needs a range to check");
			}
			if (!long.TryParse(param[0], out _checkTime))
			{
				throw new ArgumentException("CheckBTActivateTime needs a long value");
			}
		}

		public override IComposite Clone()
		{
			return new CheckBTActivateTime(this, new string[1] { _checkTime.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if ((state as BehaviorTreeState).UpTime < _checkTime)
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
