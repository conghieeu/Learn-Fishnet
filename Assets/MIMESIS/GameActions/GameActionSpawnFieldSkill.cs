using Bifrost.ConstEnum;

public class GameActionSpawnFieldSkill : GameAction
{
	public int FieldSkillMasterID { get; private set; }

	public GameActionSpawnFieldSkill(int fieldSkillMasterID)
		: base(DefAction.SPAWN_FIELD_SKILL)
	{
		FieldSkillMasterID = fieldSkillMasterID;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionSpawnFieldSkill(FieldSkillMasterID);
	}

	public override IGameAction Clone()
	{
		return new GameActionSpawnFieldSkill(FieldSkillMasterID);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionSpawnFieldSkill gameActionSpawnFieldSkill)
		{
			return gameActionSpawnFieldSkill.FieldSkillMasterID == FieldSkillMasterID;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{base.ActionType}:{FieldSkillMasterID}";
	}

	public override string GetActionName()
	{
		return $"{base.ActionType}:{FieldSkillMasterID}";
	}
}
