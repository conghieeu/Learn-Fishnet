using System;
using System.Globalization;

namespace MMBehaviorTree
{
	public class Wait : BehaviorBlockingAction
	{
		private long _startTick;

		private float _duration;

		public Wait(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("Wait action must have 1 parameter, the duration to wait");
			}
			if (!float.TryParse(param[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _duration))
			{
				throw new ArgumentException("Wait action parameter must be a float");
			}
		}

		public override IComposite Clone()
		{
			return new Wait(new string[1] { _duration.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			_startTick = Hub.s.timeutil.GetCurrentTickMilliSec();
			(state as BehaviorTreeState).BTMovementController.StopMove();
			return BehaviorResult.SUCCESS;
		}

		public override bool IsEnd(IBehaviorTreeState state)
		{
			return (float)Hub.s.timeutil.GetCurrentTickMilliSec() >= (float)_startTick + _duration * 1000f;
		}
	}
}
