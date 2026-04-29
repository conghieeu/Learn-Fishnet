using System;

namespace MMBehaviorTree
{
	public class AddFaction : BehaviorAction
	{
		private int _factionID;

		public AddFaction(string[] param)
			: base(param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("SetVoiceRule needs a rule to set voice");
			}
			if (!int.TryParse(param[0], out _factionID))
			{
				throw new ArgumentException("Invalid faction ID " + param[0]);
			}
		}

		public override IComposite Clone()
		{
			return new AddFaction(new string[1] { _factionID.ToString() });
		}

		public override BehaviorResult Execute(IBehaviorTreeState state)
		{
			BehaviorTreeState behaviorTreeState = state as BehaviorTreeState;
			if (!behaviorTreeState.Self.AddFaction(_factionID))
			{
				return BehaviorResult.FAILURE;
			}
			behaviorTreeState.Self.VRoom.IterateAllMonster(delegate(VActor monster)
			{
				monster.AIControlUnit.CollectAggroObjectByFaction();
			});
			return BehaviorResult.SUCCESS;
		}
	}
}
