using System.Collections.Generic;
using Bifrost.ConstEnum;

public sealed class AddImmutableStatsAbnormalArgs : VActorEventArgs
{
	public readonly long SyncID;

	public readonly StatType StatType;

	public readonly StatModifyType ModifyType;

	public readonly long Value;

	public readonly int Index;

	public readonly List<long> MultiValues = new List<long>();

	public AddImmutableStatsAbnormalArgs(long syncID, StatType statType, StatModifyType modifyType, long value, int index)
		: base(VActorEventType.AddImmutableStatAbnormal)
	{
		SyncID = syncID;
		StatType = statType;
		ModifyType = modifyType;
		Value = value;
		Index = index;
	}
}
