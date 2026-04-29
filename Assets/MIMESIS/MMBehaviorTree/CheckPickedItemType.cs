using System;
using System.Collections.Generic;

namespace MMBehaviorTree
{
	public class CheckPickedItemType : Conditional
	{
		private List<BTInvenPickRule> _pickRules = new List<BTInvenPickRule>();

		public CheckPickedItemType(IComposite children, string[] param)
			: base(children, param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("PickTarget needs a rule to pick target");
			}
			string text = param[0];
			string[] array = text.Split('|');
			for (int i = 0; i < array.Length; i++)
			{
				if (!Enum.TryParse<BTInvenPickRule>(array[i], ignoreCase: true, out var result))
				{
					throw new ArgumentException("Invalid rule " + text);
				}
				_pickRules.Add(result);
			}
		}

		public override IComposite Clone()
		{
			return new CheckPickedItemType(this, new string[1] { string.Join("|", _pickRules) });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			if ((state as BehaviorTreeState).BTInventoryController.CheckCurrentSlotInPickRules(_pickRules))
			{
				return BehaviorResult.SUCCESS;
			}
			return BehaviorResult.FAILURE;
		}
	}
}
