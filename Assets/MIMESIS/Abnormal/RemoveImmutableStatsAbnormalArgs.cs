public sealed class RemoveImmutableStatsAbnormalArgs : VActorEventArgs
{
	public readonly long SyncID;

	public RemoveImmutableStatsAbnormalArgs(long syncID)
		: base(VActorEventType.RemoveImmutableStatAbnormal)
	{
		SyncID = syncID;
	}
}
