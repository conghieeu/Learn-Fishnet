using Bifrost.ConstEnum;

public class GameActionSpawnFieldSkillMapMarker : GameAction
{
	public string MapMarkerName { get; private set; }

	public GameActionSpawnFieldSkillMapMarker(string mapMarkerName)
		: base(DefAction.SPAWN_FIELD_SKILL_MAPMARKER)
	{
		MapMarkerName = mapMarkerName;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionSpawnFieldSkillMapMarker(MapMarkerName);
	}

	public override IGameAction Clone()
	{
		return new GameActionSpawnFieldSkillMapMarker(MapMarkerName);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionSpawnFieldSkillMapMarker gameActionSpawnFieldSkillMapMarker)
		{
			return gameActionSpawnFieldSkillMapMarker.MapMarkerName == MapMarkerName;
		}
		return false;
	}
}
