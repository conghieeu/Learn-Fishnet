using Bifrost.ConstEnum;

public sealed class UpdateImmutableStatsAbnormalArgs : VActorEventArgs
{
	public readonly long SyncID;

	public readonly StatType StatType;

	public readonly StatModifyType ModifyType;

	public readonly int Index;

	public readonly long Value;

	public UpdateImmutableStatsAbnormalArgs(long syncID, StatType statType, StatModifyType modifyType, long value, int index)
		: base(VActorEventType.UpdateImmutableStatAbnormal)
	{
		SyncID = syncID;
		StatType = statType;
		ModifyType = modifyType;
		Value = value;
		Index = index;
	}
}
