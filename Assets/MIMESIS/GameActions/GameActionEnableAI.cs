using Bifrost.ConstEnum;

public class GameActionEnableAI : GameAction
{
	public GameActionEnableAI()
		: base(DefAction.ENABLE_AI)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionEnableAI();
	}

	public override IGameAction Clone()
	{
		return new GameActionEnableAI();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionEnableAI;
	}
}
