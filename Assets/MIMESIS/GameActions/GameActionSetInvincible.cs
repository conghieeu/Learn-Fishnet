using Bifrost.ConstEnum;

public class GameActionSetInvincible : GameAction
{
	public long Period { get; private set; }

	public GameActionSetInvincible(long period)
		: base(DefAction.SETINVINCIBLE)
	{
		Period = period;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionSetInvincible(Period);
	}

	public override IGameAction Clone()
	{
		return new GameActionSetInvincible(Period);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionSetInvincible gameActionSetInvincible)
		{
			return gameActionSetInvincible.Period == Period;
		}
		return false;
	}
}
