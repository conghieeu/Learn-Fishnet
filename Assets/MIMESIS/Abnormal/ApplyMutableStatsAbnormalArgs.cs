using Bifrost.ConstEnum;

public sealed class ApplyMutableStatsAbnormalArgs : VActorEventArgs
{
	public readonly int CasterActorID;

	public readonly long SyncID;

	public readonly MutableStatType StatType;

	public readonly StatModifyType ModifyType;

	public readonly long Value;

	public ApplyMutableStatsAbnormalArgs(int casterActorID, long syncID, MutableStatType statType, StatModifyType modifyType, long value)
		: base(VActorEventType.ApplyMutableStatAbnormal)
	{
		CasterActorID = casterActorID;
		SyncID = syncID;
		StatType = statType;
		ModifyType = modifyType;
		Value = value;
	}
}
