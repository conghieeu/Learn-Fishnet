using System;

namespace MMBehaviorTree
{
	public class SetFakeItem : BehaviorAction
	{
		private int _targetItemMasterID;

		public SetFakeItem(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("PickTarget needs a rule to SetFakeItem");
			}
			if (!int.TryParse(param[0], out _targetItemMasterID))
			{
				throw new ArgumentException("Invalid item master ID " + param[0]);
			}
		}

		public override IComposite Clone()
		{
			return new SetFakeItem(new string[1] { _targetItemMasterID.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			(state as BehaviorTreeState).BTInventoryController.GenerateFakeItemForAI(_targetItemMasterID);
			return BehaviorResult.SUCCESS;
		}
	}
}
