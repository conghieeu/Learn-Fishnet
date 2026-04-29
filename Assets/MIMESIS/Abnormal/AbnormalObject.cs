using System;
using System.Collections.Generic;
using Bifrost.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class AbnormalObject
{
	public readonly int CasterObjectID;

	public readonly long AbnormalSyncID;

	public readonly long StartTime;

	public long EndTime;

	public readonly AbnormalInfo AbnormalMasterInfo;

	public Dictionary<long, AbnormalCategory> AbnormalSyncIDs;

	public long Duration { get; private set; }

	public bool DeferToDelete { get; private set; }

	public bool Dispeled { get; private set; }

	public bool Changed { get; private set; }

	public AbnormalObject(int casterObjectID, long objectSyncID, AbnormalInfo info)
	{
		CasterObjectID = casterObjectID;
		AbnormalSyncID = objectSyncID;
		AbnormalMasterInfo = info;
		StartTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		Duration = 0L;
		EndTime = 0L;
		AbnormalSyncIDs = new Dictionary<long, AbnormalCategory>();
		DeferToDelete = false;
		Dispeled = false;
		Changed = false;
	}

	public bool AddElement(long syncID, IAbnormalElement element)
	{
		if (AbnormalSyncIDs.ContainsKey(syncID))
		{
			return false;
		}
		Duration = Math.Max(Duration, element.Duration);
		EndTime = ((0 < Duration) ? (StartTime + Duration) : 0);
		AbnormalSyncIDs.Add(syncID, element.Category);
		return true;
	}

	public bool RemoveElement(long syncID)
	{
		if (!AbnormalSyncIDs.ContainsKey(syncID))
		{
			return false;
		}
		AbnormalSyncIDs.Remove(syncID);
		return true;
	}

	public long GetRemainTime()
	{
		if (EndTime == 0L)
		{
			return 0L;
		}
		return Math.Max(0L, EndTime - Hub.s.timeutil.GetCurrentTickMilliSec());
	}

	public void Dispel()
	{
		Dispeled = true;
	}

	public void Update()
	{
		if ((Dispeled || Hub.s.timeutil.GetCurrentTickMilliSec() > EndTime) && AbnormalSyncIDs.Count == 0)
		{
			DeferToDelete = true;
		}
	}

	public void ExtendDuration(int durationMsec)
	{
		if (durationMsec > 0)
		{
			long num = Hub.s.timeutil.GetCurrentTickMilliSec() + durationMsec;
			if (EndTime <= num)
			{
				Duration = durationMsec;
				EndTime = num;
				Changed = true;
			}
		}
	}

	public AbnormalObjectInfo Serialize(AbnormalDataSyncType syncType = AbnormalDataSyncType.Add)
	{
		return new AbnormalObjectInfo
		{
			syncType = syncType,
			abnormalSyncID = AbnormalSyncID,
			abnormalMasterID = AbnormalMasterInfo.MasterID,
			remainTime = GetRemainTime(),
			duration = Duration
		};
	}

	public void ClearChanged()
	{
		Changed = false;
	}
}
