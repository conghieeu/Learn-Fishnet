using System;
using System.Globalization;

namespace MMBehaviorTree
{
	public class Ping : BehaviorAction
	{
		private float _distance;

		public Ping(string[] param)
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
			return new Ping(new string[1] { _distance.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			foreach (VPlayer item in behaviorTreeState.Self.VRoom.GetPlayersInRange(behaviorTreeState.Self.PositionVector, 0f, _distance, checkHeight: true))
			{
				behaviorTreeState.Self.VRoom.IncreaseMimicEncounterCount(item);
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
