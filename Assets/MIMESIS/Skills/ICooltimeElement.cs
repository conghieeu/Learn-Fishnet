using ReluProtocol.Enum;

public abstract class ICooltimeElement
{
	public readonly long SyncID;

	public long RemainDuration { get; protected set; }

	public bool DeferToDelete { get; protected set; }

	public bool Global { get; protected set; }

	public long EndTimestamp { get; protected set; }

	public bool Sync { get; protected set; }

	public ICooltimeElement(long syncID, long duration, bool global, bool sync)
	{
		SyncID = syncID;
		RemainDuration = duration;
		Global = global;
		Sync = sync;
		EndTimestamp = Hub.s.timeutil.GetTimeStampInMilliSec() + RemainDuration;
		DeferToDelete = false;
	}

	public virtual void Update(long delta)
	{
		RemainDuration -= delta;
		if (RemainDuration <= 0)
		{
			RemainDuration = 0L;
			DeferToDelete = true;
		}
	}

	public void SetDelete()
	{
		RemainDuration = 0L;
		DeferToDelete = true;
	}

	public abstract void FillCooltimeSig(ref CooltimeSig sig, CooltimeChangeType type);
}
