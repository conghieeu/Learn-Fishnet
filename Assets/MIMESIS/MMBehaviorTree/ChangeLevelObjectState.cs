using System;
using ReluProtocol.Enum;

namespace MMBehaviorTree
{
	public class ChangeLevelObjectState : BehaviorAction
	{
		private int _fromState;

		private int _toState;

		public ChangeLevelObjectState(string[] param)
			: base(param)
		{
			if (param.Length != 2)
			{
				throw new ArgumentException("PickTarget needs a rule to pick target");
			}
			if (!int.TryParse(param[0], out _fromState))
			{
				throw new ArgumentException("Invalid fromState " + param[0]);
			}
			if (!int.TryParse(param[1], out _toState))
			{
				throw new ArgumentException("Invalid toState " + param[1]);
			}
		}

		public override IComposite Clone()
		{
			return new ChangeLevelObjectState(new string[2]
			{
				_fromState.ToString(),
				_toState.ToString()
			});
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (behaviorTreeState.TargetLevelObject == null)
			{
				return BehaviorResult.FAILURE;
			}
			if (!behaviorTreeState.Self.VRoom.CheckLevelObjectStateChange(behaviorTreeState.TargetLevelObject.ID, _fromState, _toState))
			{
				return BehaviorResult.FAILURE;
			}
			bool occupy = false;
			if (behaviorTreeState.TargetLevelObject is OccupiedLevelObjectInfo { CurrentState: 0 })
			{
				occupy = true;
			}
			if (behaviorTreeState.Self.VRoom.HandleLevelObject(behaviorTreeState.Self.ObjectID, behaviorTreeState.TargetLevelObject.ID, _toState, occupy, out var _) != MsgErrorCode.Success)
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
