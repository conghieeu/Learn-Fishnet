public struct AbnormalParamInfo
{
	public readonly int CasterObjectID;

	public readonly int AbnormalMasterID;

	public readonly int Duration;

	public readonly AbnormalReason Reason;

	public AbnormalParamInfo(int casterObjectID, int abnormalMasterID, int duration, AbnormalReason reason)
	{
		CasterObjectID = casterObjectID;
		AbnormalMasterID = abnormalMasterID;
		Duration = duration;
		Reason = reason;
	}
}
