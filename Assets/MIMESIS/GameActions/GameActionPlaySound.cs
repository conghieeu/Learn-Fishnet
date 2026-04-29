using Bifrost.ConstEnum;

public class GameActionPlaySound : GameAction
{
	public string SoundClipKey { get; private set; }

	public GameActionPlaySound(string soundName)
		: base(DefAction.PLAY_SOUND)
	{
		SoundClipKey = soundName;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionPlaySound(SoundClipKey);
	}

	public override IGameAction Clone()
	{
		return new GameActionPlaySound(SoundClipKey);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionPlaySound gameActionPlaySound)
		{
			return gameActionPlaySound.SoundClipKey == SoundClipKey;
		}
		return false;
	}
}
