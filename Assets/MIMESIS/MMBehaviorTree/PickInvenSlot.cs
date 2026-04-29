using System;
using System.Collections.Generic;

namespace MMBehaviorTree
{
	public class PickInvenSlot : BehaviorAction
	{
		private List<BTInvenPickRule> _pickRules = new List<BTInvenPickRule>();

		public PickInvenSlot(string[] param)
			: base(param)
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
			return new PickInvenSlot(new string[1] { string.Join("|", _pickRules) });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			InventoryController bTInventoryController = (state as BehaviorTreeState).BTInventoryController;
			int targetSlotIndex = bTInventoryController.GetTargetSlotIndex(_pickRules);
			if (targetSlotIndex <= 0)
			{
				return BehaviorResult.FAILURE;
			}
			bTInventoryController.HandleChangeActiveInvenSlot(targetSlotIndex);
			return BehaviorResult.SUCCESS;
		}
	}
}
