using Bifrost.ConstEnum;

public class GameActionWait : GameAction
{
	public readonly long Duration;

	private long _startTime;

	public GameActionWait(long duration)
		: base(DefAction.WAIT)
	{
		Duration = duration;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionWait(Duration);
	}

	public override IGameAction Clone()
	{
		return new GameActionWait(Duration);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionWait gameActionWait)
		{
			return gameActionWait.Duration == Duration;
		}
		return false;
	}

	public override bool Progress()
	{
		if (base.State != GameActionState.Ready)
		{
			return false;
		}
		_startTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		base.State = GameActionState.Progress;
		RegisterCompleteDelegate(() => Hub.s.timeutil.GetCurrentTickMilliSec() - _startTime >= Duration);
		return true;
	}

	public override string ToString()
	{
		return $"{base.ActionType}:{Duration}";
	}
}
