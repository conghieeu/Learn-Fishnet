using System;
using System.Collections.Immutable;
using Bifrost.AbnormalData;
using Bifrost.ConstEnum;

public class AbnormalStatsElementInfo : IAbnormalElementInfo
{
	public readonly AbnormalStatsCategory StatsCategory;

	public readonly StatModifyType ModifyType;

	public readonly StatType ImmutableStatType;

	public readonly MutableStatType MutableStatType;

	public readonly AbnormalApplyPeriodType PeriodType;

	public readonly long Value;

	public ImmutableList<long> MultiCustomValues = ImmutableList<long>.Empty;

	public readonly long Interval;

	public AbnormalStatsElementInfo(AbnormalData_element element)
		: base(element)
	{
		if (!Enum.TryParse<AbnormalStatsCategory>(element.type, ignoreCase: true, out StatsCategory))
		{
			throw new Exception("Invalid AbnormalStatsCategory: " + element.type);
		}
		PeriodType = (AbnormalApplyPeriodType)element.apply_period_type;
		switch (StatsCategory)
		{
		case AbnormalStatsCategory.MutableStat:
			MutableStatType = StatUtil.GetMutableStatTypeFromString(element.sub_type);
			break;
		case AbnormalStatsCategory.ImmutableStat:
			ImmutableStatType = StatUtil.GetStatTypeFromString(element.sub_type);
			break;
		default:
			throw new Exception("Invalid AbnormalStatsCategory");
		}
		ModifyType = (StatModifyType)element.modify_type;
		if (ModifyType == StatModifyType.Static || ModifyType == StatModifyType.Percent)
		{
			Value = element.modify_value;
		}
		else if (ModifyType == StatModifyType.MultiCustomStatic || ModifyType == StatModifyType.MultiCustomPercent)
		{
			ImmutableList<long>.Builder builder = ImmutableList.CreateBuilder<long>();
			foreach (int item in element.dot_val_per_tick)
			{
				builder.Add(item);
			}
			MultiCustomValues = builder.ToImmutable();
		}
		Interval = element.interval;
	}
}
