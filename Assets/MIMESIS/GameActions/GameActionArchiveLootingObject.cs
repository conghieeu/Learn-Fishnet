using Bifrost.ConstEnum;

public class GameActionArchiveLootingObject : GameAction
{
	public GameActionArchiveLootingObject()
		: base(DefAction.ARCHIVE_LOOTING_OBJECT)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionArchiveLootingObject();
	}

	public override IGameAction Clone()
	{
		return new GameActionArchiveLootingObject();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionArchiveLootingObject;
	}
}
