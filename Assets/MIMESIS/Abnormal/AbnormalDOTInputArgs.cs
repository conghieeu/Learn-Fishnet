using System.Collections.Generic;
using Bifrost.ConstEnum;

public class AbnormalDOTInputArgs : AbnormalCommonInputArgs
{
	public AbnormalApplyPeriodType ApplyType;

	public MutableStatType MutableStatType;

	public StatType StatType;

	public StatModifyType ModifyType;

	public long Interval;

	public long Value;

	public Dictionary<int, int> MultiValues = new Dictionary<int, int>();
}
