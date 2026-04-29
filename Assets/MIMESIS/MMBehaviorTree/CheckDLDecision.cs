using System;
using DLAgent;

namespace MMBehaviorTree
{
	public class CheckDLDecision : Conditional
	{
		private DLDecisionType _decisionType;

		private bool _resetDecision;

		public CheckDLDecision(IComposite composite, string[] param)
			: base(composite, param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("CheckDLDecision requires 1 parameters");
			}
			if (!Enum.TryParse<DLDecisionType>(param[0], ignoreCase: true, out _decisionType))
			{
				throw new ArgumentException("Invalid decision type " + param[0]);
			}
		}

		public override IComposite Clone()
		{
			return new CheckDLDecision(this, new string[1] { _decisionType.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if (!(state as BehaviorTreeState).BTAIController.GetDLAgentDecisionResult(out DLAgentDecisionOutput output))
			{
				return BehaviorResult.FAILURE;
			}
			if (output == null)
			{
				return BehaviorResult.FAILURE;
			}
			if (output.Decision != _decisionType)
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
