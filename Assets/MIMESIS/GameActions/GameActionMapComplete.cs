using Bifrost.ConstEnum;

public class GameActionMapComplete : GameAction
{
	public GameActionMapComplete()
		: base(DefAction.MAP_COMPLETE)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionMapComplete();
	}

	public override IGameAction Clone()
	{
		return new GameActionMapComplete();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionMapComplete;
	}
}
