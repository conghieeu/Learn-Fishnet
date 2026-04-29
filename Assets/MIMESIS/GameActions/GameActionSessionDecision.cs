using Bifrost.ConstEnum;

public class GameActionSessionDecision : GameAction
{
	public GameActionSessionDecision()
		: base(DefAction.SESSION_DECISION)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionSessionDecision();
	}

	public override IGameAction Clone()
	{
		return new GameActionSessionDecision();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionSessionDecision;
	}
}
