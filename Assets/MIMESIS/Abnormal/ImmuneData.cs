using Bifrost.ConstEnum;

public class ImmuneData
{
	public readonly ImmuneType ImmuneType;

	public readonly CCType CCType;

	public readonly MutableStatType MutableStatType;

	public readonly StatType ImmutableStatType;

	public ImmuneData(ImmuneType type)
	{
		ImmuneType = type;
	}

	public ImmuneData(CCType type)
	{
		ImmuneType = ImmuneType.TargetCC;
		CCType = type;
	}

	public ImmuneData(MutableStatType type)
	{
		ImmuneType = ImmuneType.TargetMutableStat;
		MutableStatType = type;
	}

	public ImmuneData(StatType type)
	{
		ImmuneType = ImmuneType.TargetImmutableStat;
		ImmutableStatType = type;
	}
}
