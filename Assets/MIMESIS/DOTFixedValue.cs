using System.Collections.Generic;
using Bifrost.ConstEnum;

public class DOTFixedValue
{
	public readonly long Interval;

	public readonly StatType ImmutableStatType;

	public readonly MutableStatType MutableStatType;

	public readonly StatModifyType ModifyType;

	public readonly AbnormalStatsCategory Category = AbnormalStatsCategory.MutableStat;

	public Dictionary<int, long> MultiValues = new Dictionary<int, long>();

	public long Value { get; set; }

	public bool Initialized { get; private set; }

	public DOTFixedValue(AbnormalInfo info, int index, int stackValue = 0)
	{
		if (info.ElementList.Count < index)
		{
			Logger.RWarn($"element list count is less than index. index: {index} / {info.ElementList.Count}");
			return;
		}
		if (!(info.ElementList[index] is AbnormalStatsElementInfo abnormalStatsElementInfo))
		{
			Logger.RWarn($"element info is not AbnormalStatsElementInfo. index: {index}");
			return;
		}
		Interval = abnormalStatsElementInfo.Interval;
		ImmutableStatType = abnormalStatsElementInfo.ImmutableStatType;
		MutableStatType = abnormalStatsElementInfo.MutableStatType;
		Category = abnormalStatsElementInfo.StatsCategory;
		ModifyType = abnormalStatsElementInfo.ModifyType;
		if (ModifyType == StatModifyType.Static || ModifyType == StatModifyType.Percent)
		{
			Value = abnormalStatsElementInfo.Value;
		}
		else if (ModifyType == StatModifyType.MultiCustomStatic || ModifyType == StatModifyType.MultiCustomPercent)
		{
			int num = 0;
			foreach (long multiCustomValue in abnormalStatsElementInfo.MultiCustomValues)
			{
				MultiValues.Add(num++, multiCustomValue);
			}
		}
		Initialized = true;
	}

	public DOTFixedValue(AbnormalStatsCategory category, AbnormalDOTInputArgs args, int stackCount = 0)
	{
		Category = category;
		ImmutableStatType = args.StatType;
		MutableStatType = args.MutableStatType;
		ModifyType = args.ModifyType;
		Interval = args.Interval;
		if (ModifyType == StatModifyType.Static || ModifyType == StatModifyType.Percent)
		{
			Value = args.Value;
		}
		else if (ModifyType == StatModifyType.MultiCustomStatic || ModifyType == StatModifyType.MultiCustomPercent)
		{
			foreach (KeyValuePair<int, int> multiValue in args.MultiValues)
			{
				MultiValues.Add(multiValue.Key, multiValue.Value);
			}
		}
		Initialized = true;
	}
}
