using Bifrost.ConstEnum;

public class GameActionRemoveFieldSkillNearby : GameAction
{
	public float Radius { get; private set; }

	public GameActionRemoveFieldSkillNearby(float radius)
		: base(DefAction.REMOVE_FIELD_SKILL_NEARBY)
	{
		Radius = radius;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionRemoveFieldSkillNearby(Radius);
	}

	public override IGameAction Clone()
	{
		return new GameActionRemoveFieldSkillNearby(Radius);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionRemoveFieldSkillNearby gameActionRemoveFieldSkillNearby)
		{
			return gameActionRemoveFieldSkillNearby.Radius == Radius;
		}
		return false;
	}
}
