using Bifrost.ConstEnum;

public class GameActionRespawnArchivedObject : GameAction
{
	public GameActionRespawnArchivedObject()
		: base(DefAction.RESPAWN_ARCHIVED_OBJECT)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionRespawnArchivedObject();
	}

	public override IGameAction Clone()
	{
		return new GameActionRespawnArchivedObject();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionRespawnArchivedObject;
	}
}
