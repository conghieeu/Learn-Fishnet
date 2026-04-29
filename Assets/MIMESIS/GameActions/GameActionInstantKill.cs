using Bifrost.ConstEnum;

public class GameActionInstantKill : GameAction
{
	public string LevelObjectName { get; private set; }

	public GameActionInstantKill(string levelObjectName)
		: base(DefAction.INSTANT_KILL)
	{
		LevelObjectName = levelObjectName;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionInstantKill(LevelObjectName);
	}

	public override IGameAction Clone()
	{
		return new GameActionInstantKill(LevelObjectName);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionInstantKill gameActionInstantKill)
		{
			return gameActionInstantKill.LevelObjectName == LevelObjectName;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{base.ActionType}:{LevelObjectName}";
	}
}
