using Bifrost.ConstEnum;

public class GameActionRandomTeleport : GameAction
{
	public GameActionRandomTeleport()
		: base(DefAction.RANDOM_TELEPORT)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionRandomTeleport();
	}

	public override IGameAction Clone()
	{
		return new GameActionRandomTeleport();
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionRandomTeleport)
		{
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{base.ActionType}";
	}
}
