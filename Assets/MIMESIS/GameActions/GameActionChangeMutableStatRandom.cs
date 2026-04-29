using Bifrost.ConstEnum;

public class GameActionChangeMutableStatRandom : GameAction
{
	public MutableStatType StatType { get; private set; }

	public long MinValue { get; private set; }

	public long MaxValue { get; private set; }

	public GameActionChangeMutableStatRandom(MutableStatType statType, long minValue, long maxValue)
		: base(DefAction.CHANGE_MUTABLE_STAT_RANDOM)
	{
		StatType = statType;
		MinValue = minValue;
		MaxValue = maxValue;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionChangeMutableStatRandom(StatType, MinValue, MaxValue);
	}

	public override IGameAction Clone()
	{
		return new GameActionChangeMutableStatRandom(StatType, MinValue, MaxValue);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionChangeMutableStatRandom gameActionChangeMutableStatRandom)
		{
			if (gameActionChangeMutableStatRandom.StatType == StatType && gameActionChangeMutableStatRandom.MinValue == MinValue)
			{
				return gameActionChangeMutableStatRandom.MaxValue == MaxValue;
			}
			return false;
		}
		return false;
	}
}
