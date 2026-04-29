public class AbnormalFixedValue
{
	public readonly long SyncID;

	public readonly long AbnormalObjectID;

	public readonly bool Dispelable;

	public readonly long WaitTime;

	public AbnormalFixedValue(long syncID, long abnormalObjectID, AbnormalInfo info, int index)
	{
		AbnormalObjectID = abnormalObjectID;
		SyncID = syncID;
		Dispelable = info.Dispelable;
		if (info.ElementList.ContainsKey(index))
		{
			WaitTime = info.ElementList[index].ActivateDelay;
		}
	}

	public AbnormalFixedValue(AbnormalCommonInputArgs args)
	{
		AbnormalObjectID = args.AbnormalObjectID;
		SyncID = args.SyncID;
		Dispelable = args.Dispelable;
	}
}
