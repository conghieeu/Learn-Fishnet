using System;

namespace MMBehaviorTree
{
	public class CheckAIState : Conditional
	{
		private readonly AIState _aiState;

		public CheckAIState(IComposite children, string[] param)
			: base(children, param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("CheckTargetPickedTime needs a range to check");
			}
			if (!Enum.TryParse<AIState>(param[0], out _aiState))
			{
				throw new ArgumentException("CheckAIState cannot parse " + param[0] + " to AIState");
			}
		}

		public override IComposite Clone()
		{
			return new CheckAIState(this, new string[1] { _aiState.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if ((state as BehaviorTreeState).BTAIController.State != _aiState)
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
