using Bifrost.ConstEnum;

public class GameActionInvalid : GameAction
{
	public GameActionInvalid()
		: base(DefAction.Invalid)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionInvalid();
	}

	public override IGameAction Clone()
	{
		return new GameActionInvalid();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionInvalid;
	}
}
