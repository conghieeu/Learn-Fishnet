using System;
using Bifrost.ConstEnum;
using ReluProtocol;

public abstract class IAbnormalElement
{
	public readonly AbnormalCategory Category;

	public int CasterObjectID { get; protected set; }

	public int AbnormalMasterID { get; protected set; }

	public int Index { get; protected set; }

	public AbnormalFixedValue FixedValue { get; protected set; }

	public long Duration { get; protected set; }

	public long StartTime { get; protected set; }

	public long RegisterTime { get; protected set; }

	public long EndTime { get; protected set; }

	public bool DeferToDelete { get; protected set; }

	public int StackCount { get; protected set; }

	public bool Applied { get; protected set; }

	public bool Initialized { get; protected set; }

	public bool Changed { get; protected set; }

	public Action? OnElementDispose { get; protected set; }

	public IAbnormalElement(AbnormalCategory category)
	{
		Category = category;
		Changed = false;
		OnElementDispose = null;
		CasterObjectID = 0;
		AbnormalMasterID = 0;
		Index = 0;
		Duration = 0L;
		RegisterTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		StartTime = 0L;
		EndTime = 0L;
		DeferToDelete = false;
		StackCount = 0;
		Applied = false;
		Initialized = false;
		Changed = false;
	}

	public virtual bool Initialize(long syncID, long abnormalObjectID, int casterObjectID, AbnormalInfo info, int index, int initialStack, long duration, PosWithRot? pos = null)
	{
		CasterObjectID = casterObjectID;
		AbnormalMasterID = info.MasterID;
		FixedValue = new AbnormalFixedValue(syncID, abnormalObjectID, info, index);
		Index = index;
		Duration = duration;
		Reset(0L);
		Initialized = true;
		return true;
	}

	public virtual bool SetValue(AbnormalCommonInputArgs args)
	{
		if (args.SyncID == 0L)
		{
			return false;
		}
		CasterObjectID = args.CasterObjectID;
		if (FixedValue == null)
		{
			FixedValue = new AbnormalFixedValue(args);
			Reset(args.Duration);
		}
		return true;
	}

	public void Reset(long duration = 0L)
	{
		if (duration > 0)
		{
			Duration = duration;
		}
		RegisterTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		StartTime = RegisterTime + FixedValue.WaitTime;
		EndTime = StartTime + Duration;
	}

	public virtual void ExtendDuration(int durationMsec)
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

	public abstract void Update(long delta);

	public bool Expired()
	{
		return EndTime < Hub.s.timeutil.GetCurrentTickMilliSec();
	}

	public void SetDelete()
	{
		DeferToDelete = true;
	}

	public void OverwriteEndTime(long endTime)
	{
		EndTime = endTime;
	}

	public long GetRemainTime()
	{
		return EndTime - Hub.s.timeutil.GetCurrentTickMilliSec();
	}

	public void ClearChanged()
	{
		Changed = false;
	}

	protected bool CanApply()
	{
		if (!Applied && !DeferToDelete)
		{
			return Hub.s.timeutil.GetCurrentTickMilliSec() >= StartTime;
		}
		return false;
	}

	public virtual void Apply(long delta)
	{
		if (CanApply())
		{
			Applied = true;
		}
	}

	public abstract void Dispose();
}
