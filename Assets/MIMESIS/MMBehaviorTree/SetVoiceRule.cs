using System;

namespace MMBehaviorTree
{
	public class SetVoiceRule : BehaviorAction
	{
		private BTVoiceRule _btVoiceRule;

		public SetVoiceRule(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("SetVoiceRule needs a rule to set voice");
			}
			if (!Enum.TryParse<BTVoiceRule>(param[0], ignoreCase: true, out _btVoiceRule))
			{
				throw new ArgumentException("Invalid voice rule " + param[0]);
			}
		}

		public override IComposite Clone()
		{
			return new SetVoiceRule(new string[1] { _btVoiceRule.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTAIController.SetVoiceRule(_btVoiceRule);
			return BehaviorResult.SUCCESS;
		}
	}
}
