using System.Collections.Generic;
using Bifrost.ConstEnum;

public class GameActionSpawnFieldSkillRandom : GameAction
{
	public List<int> FieldSkillMasterIDList { get; private set; } = new List<int>();

	public GameActionSpawnFieldSkillRandom(List<int> fieldSkillMasterIDList)
		: base(DefAction.SPAWN_FIELD_SKILL_RANDOM)
	{
		FieldSkillMasterIDList.Clear();
		FieldSkillMasterIDList.AddRange(fieldSkillMasterIDList);
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionSpawnFieldSkillRandom(FieldSkillMasterIDList);
	}

	public override IGameAction Clone()
	{
		return new GameActionSpawnFieldSkillRandom(FieldSkillMasterIDList);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionSpawnFieldSkillRandom gameActionSpawnFieldSkillRandom)
		{
			if (FieldSkillMasterIDList.Count != gameActionSpawnFieldSkillRandom.FieldSkillMasterIDList.Count)
			{
				return false;
			}
			for (int i = 0; i < FieldSkillMasterIDList.Count; i++)
			{
				if (FieldSkillMasterIDList[i] != gameActionSpawnFieldSkillRandom.FieldSkillMasterIDList[i])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return string.Format("{0}:{1}", base.ActionType, string.Join(",", FieldSkillMasterIDList));
	}
}
