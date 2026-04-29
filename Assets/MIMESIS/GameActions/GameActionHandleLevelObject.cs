using Bifrost.ConstEnum;

public class GameActionHandleLevelObject : GameAction
{
	public string LevelObjectName { get; private set; }

	public int LevelObjectState { get; private set; }

	public bool Occupy { get; private set; }

	public bool ResetRemainCount { get; private set; }

	public GameActionHandleLevelObject(string levelObjectName, int state, bool occupy, bool resetRemainCount)
		: base(DefAction.HANDLE_LEVELOBJECT)
	{
		LevelObjectName = levelObjectName;
		LevelObjectState = state;
		Occupy = occupy;
		ResetRemainCount = resetRemainCount;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionHandleLevelObject(LevelObjectName, LevelObjectState, Occupy, ResetRemainCount);
	}

	public override IGameAction Clone()
	{
		return new GameActionHandleLevelObject(LevelObjectName, LevelObjectState, Occupy, ResetRemainCount);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionHandleLevelObject gameActionHandleLevelObject)
		{
			if (gameActionHandleLevelObject.LevelObjectName == LevelObjectName && gameActionHandleLevelObject.State == base.State && gameActionHandleLevelObject.Occupy == Occupy)
			{
				return gameActionHandleLevelObject.ResetRemainCount == ResetRemainCount;
			}
			return false;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{base.ActionType}:{LevelObjectName}:{base.State}:{Occupy}:{ResetRemainCount}";
	}
}
