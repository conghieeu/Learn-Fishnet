using Bifrost.ConstEnum;

public class GameActionPlayCutscene : GameAction
{
	public string CutsceneName { get; private set; }

	public bool NeedToBroadcast { get; private set; } = true;

	public GameActionPlayCutscene(string cutsceneName, bool needToBroadCast)
		: base(DefAction.PLAY_CUTSCENE)
	{
		CutsceneName = cutsceneName;
		NeedToBroadcast = needToBroadCast;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionPlayCutscene(CutsceneName, NeedToBroadcast);
	}

	public override IGameAction Clone()
	{
		return new GameActionPlayCutscene(CutsceneName, NeedToBroadcast);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionPlayCutscene gameActionPlayCutscene)
		{
			return gameActionPlayCutscene.CutsceneName == CutsceneName;
		}
		return false;
	}
}
