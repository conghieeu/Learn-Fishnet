public class MutableStatInfo
{
	public readonly StatType StatType;

	public readonly MutableStatType MutableStatType;

	public readonly bool IsNoAffectMutableStat;

	public static MutableStatInfo InvalidMutableStat = new MutableStatInfo(StatType.Invalid, MutableStatType.Invalid);

	public MutableStatInfo(StatType statType, MutableStatType mutableStatType)
	{
		StatType = statType;
		MutableStatType = mutableStatType;
	}
}
