using System;
using System.Globalization;

namespace MMBehaviorTree
{
	public class SetDetectionRange : BehaviorAction
	{
		private float _distance;

		public SetDetectionRange(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("PickTarget needs a rule to pick target");
			}
			if (!float.TryParse(param[0], NumberStyles.Any, CultureInfo.InvariantCulture, out _distance))
			{
				throw new ArgumentException("Invalid distance " + param[0]);
			}
		}

		public override IComposite Clone()
		{
			return new SetDetectionRange(new string[1] { _distance.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.SetSightRange(_distance);
			return BehaviorResult.SUCCESS;
		}
	}
}
