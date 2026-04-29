using Bifrost.ConstEnum;

public class GameActionChangeTramParts : GameAction
{
	public GameActionChangeTramParts()
		: base(DefAction.CHANGE_TRAM_PARTS)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionChangeTramParts();
	}

	public override IGameAction Clone()
	{
		return new GameActionChangeTramParts();
	}

	public override bool Correct(IGameAction action)
	{
		return this != null;
	}
}
