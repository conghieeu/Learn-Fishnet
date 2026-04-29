using Bifrost.ConstEnum;

public class GameActionChangeMutableStat : GameAction
{
	public MutableStatType StatType { get; private set; }

	public long Value { get; private set; }

	public GameActionChangeMutableStat(MutableStatType statType, long value)
		: base(DefAction.CHANGE_MUTABLE_STAT)
	{
		StatType = statType;
		Value = value;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionChangeMutableStat(StatType, Value);
	}

	public override IGameAction Clone()
	{
		return new GameActionChangeMutableStat(StatType, Value);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionChangeMutableStat gameActionChangeMutableStat)
		{
			if (gameActionChangeMutableStat.StatType == StatType)
			{
				return gameActionChangeMutableStat.Value == Value;
			}
			return false;
		}
		return false;
	}
}
