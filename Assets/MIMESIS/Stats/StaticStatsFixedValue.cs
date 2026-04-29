using Bifrost.ConstEnum;

public class StaticStatsFixedValue
{
	public readonly AbnormalStatsCategory Category = AbnormalStatsCategory.MutableStat;

	public readonly StatType ImmutableStatType;

	public readonly MutableStatType MutableStatType;

	public readonly StatModifyType ModifyType;

	public readonly long Value;

	public bool Initialized { get; private set; }

	public StaticStatsFixedValue(AbnormalInfo info, int index, AbnormalFixedValue fixedValue)
	{
		if (info.ElementList.Count >= index && info.ElementList[index] is AbnormalStatsElementInfo abnormalStatsElementInfo)
		{
			Category = abnormalStatsElementInfo.StatsCategory;
			if (Category == AbnormalStatsCategory.ImmutableStat)
			{
				ImmutableStatType = abnormalStatsElementInfo.ImmutableStatType;
			}
			else
			{
				MutableStatType = abnormalStatsElementInfo.MutableStatType;
			}
			ModifyType = abnormalStatsElementInfo.ModifyType;
			Value = abnormalStatsElementInfo.Value;
			Initialized = true;
		}
	}

	public StaticStatsFixedValue(AbnormalStatsCategory category, AbnormalStatsInputArgs args)
	{
		MutableStatType = args.MutableStatType;
		ImmutableStatType = args.StatType;
		Value = args.Value;
		ModifyType = args.ModifyType;
		if (MutableStatType != MutableStatType.Invalid)
		{
			Category = AbnormalStatsCategory.MutableStat;
		}
		else
		{
			Category = AbnormalStatsCategory.ImmutableStat;
		}
		Initialized = true;
	}
}
