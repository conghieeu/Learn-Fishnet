using System;

namespace MMBehaviorTree
{
	public class CopyInventory : BehaviorAction
	{
		private BTTargetPickRule _pickRule;

		public CopyInventory(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("PickTarget needs a rule to pick target");
			}
			string text = param[0];
			if (!Enum.TryParse<BTTargetPickRule>(text, out _pickRule))
			{
				throw new ArgumentException("Invalid rule " + text);
			}
		}

		public override IComposite Clone()
		{
			return new CopyInventory(new string[1] { _pickRule.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if (!(state as BehaviorTreeState).BTAIController.CopyInventory(_pickRule))
			{
				return BehaviorResult.FAILURE;
			}
			return BehaviorResult.SUCCESS;
		}
	}
}
