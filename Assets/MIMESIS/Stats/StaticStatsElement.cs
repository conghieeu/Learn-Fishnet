using Bifrost.ConstEnum;
using ReluProtocol;

public class StaticStatsElement : IAbnormalElement
{
	private OnActorEventDelegate _tickEvent;

	private readonly AbnormalStatsCategory _category;

	public StaticStatsFixedValue? StaticStatsFixedValue { get; private set; }

	public StaticStatsElement(AbnormalStatsCategory category, OnActorEventDelegate tickEvent)
		: base(AbnormalCategory.Stats)
	{
		_category = category;
		_tickEvent = tickEvent;
	}

	public override bool Initialize(long syncID, long abnormalObjectID, int casterObjectID, AbnormalInfo info, int index, int initialStack, long duration, PosWithRot? pos = null)
	{
		if (!base.Initialize(syncID, abnormalObjectID, casterObjectID, info, index, initialStack, duration, pos))
		{
			return false;
		}
		StaticStatsFixedValue = new StaticStatsFixedValue(info, index, base.FixedValue);
		if (!StaticStatsFixedValue.Initialized)
		{
			return false;
		}
		Apply(0L);
		return true;
	}

	public override bool SetValue(AbnormalCommonInputArgs args)
	{
		if (!base.SetValue(args))
		{
			return false;
		}
		if (!(args is AbnormalStatsInputArgs args2))
		{
			return false;
		}
		StaticStatsFixedValue = new StaticStatsFixedValue(_category, args2);
		return true;
	}

	public void Reset()
	{
		Reset(0L);
	}

	public override void Update(long delta)
	{
		Apply(delta);
	}

	public override void Apply(long delta)
	{
		if (!CanApply())
		{
			return;
		}
		VActorEventArgs e = null;
		if (!base.Applied)
		{
			if (StaticStatsFixedValue.Category == AbnormalStatsCategory.MutableStat)
			{
				e = new ApplyMutableStatsAbnormalArgs(base.CasterObjectID, base.FixedValue.SyncID, StaticStatsFixedValue.MutableStatType, StaticStatsFixedValue.ModifyType, StaticStatsFixedValue.Value);
				base.DeferToDelete = true;
			}
			else
			{
				e = new AddImmutableStatsAbnormalArgs(base.FixedValue.SyncID, StaticStatsFixedValue.ImmutableStatType, StaticStatsFixedValue.ModifyType, StaticStatsFixedValue.Value, 0);
			}
		}
		if (e != null)
		{
			_tickEvent?.Invoke(e);
		}
		base.Applied = true;
	}

	public override void Dispose()
	{
		RemoveImmutableStatsAbnormalArgs eventArgs = new RemoveImmutableStatsAbnormalArgs(base.FixedValue.SyncID);
		_tickEvent?.Invoke(eventArgs);
	}
}
