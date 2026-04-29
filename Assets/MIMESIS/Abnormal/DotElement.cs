using System;
using Bifrost.ConstEnum;
using ReluProtocol;

public class DotElement : IAbnormalElement
{
	private OnActorEventDelegate? _tickEvent;

	public readonly AbnormalStatsCategory DOTCategory;

	private int _currentMultiIndex;

	public int TriggerCount { get; private set; }

	public long ElapsedTimeAfterTrigger { get; private set; }

	public DOTFixedValue? DotFixedValue { get; private set; }

	public DotElement(AbnormalStatsCategory category, OnActorEventDelegate tickEvent)
		: base(AbnormalCategory.Stats)
	{
		DOTCategory = category;
		_tickEvent = (OnActorEventDelegate)Delegate.Combine(_tickEvent, tickEvent);
		TriggerCount = 0;
		ElapsedTimeAfterTrigger = 0L;
	}

	public override bool Initialize(long syncID, long abnormalObjectID, int casterObjectID, AbnormalInfo info, int index, int initialStack, long duration, PosWithRot? pos = null)
	{
		if (!base.Initialize(syncID, abnormalObjectID, casterObjectID, info, index, initialStack, duration, pos))
		{
			return false;
		}
		DotFixedValue = new DOTFixedValue(info, index);
		Apply(0L);
		return true;
	}

	public override bool SetValue(AbnormalCommonInputArgs args)
	{
		if (!base.SetValue(args))
		{
			return false;
		}
		if (!(args is AbnormalDOTInputArgs args2))
		{
			return false;
		}
		DotFixedValue = new DOTFixedValue(DOTCategory, args2);
		return true;
	}

	public void Reset()
	{
		Reset(0L);
		TriggerCount = 0;
		ElapsedTimeAfterTrigger = 0L;
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
		ElapsedTimeAfterTrigger += delta;
		if (ElapsedTimeAfterTrigger <= DotFixedValue.Interval && _currentMultiIndex != 0)
		{
			return;
		}
		long value = 0L;
		if (DotFixedValue.ModifyType == StatModifyType.MultiCustomPercent || DotFixedValue.ModifyType == StatModifyType.MultiCustomStatic)
		{
			if (_currentMultiIndex >= DotFixedValue.MultiValues.Count)
			{
				return;
			}
			if (!DotFixedValue.MultiValues.TryGetValue(_currentMultiIndex, out value))
			{
				Logger.RWarn($"MultiValues does not contain index: {_currentMultiIndex}");
				return;
			}
		}
		else
		{
			value = DotFixedValue.Value;
		}
		e = ((DotFixedValue.Category != AbnormalStatsCategory.MutableStat) ? ((VActorEventArgs)new AddImmutableStatsAbnormalArgs(base.FixedValue.SyncID, DotFixedValue.ImmutableStatType, DotFixedValue.ModifyType, value, _currentMultiIndex)) : ((VActorEventArgs)new ApplyMutableStatsAbnormalArgs(base.CasterObjectID, base.FixedValue.SyncID, DotFixedValue.MutableStatType, DotFixedValue.ModifyType, value)));
		ElapsedTimeAfterTrigger -= Math.Min(ElapsedTimeAfterTrigger, DotFixedValue.Interval);
		_currentMultiIndex++;
		if (e != null)
		{
			_tickEvent?.Invoke(e);
		}
	}

	public override void ExtendDuration(int durationMsec)
	{
		base.ExtendDuration(durationMsec);
		if (DotFixedValue.Category == AbnormalStatsCategory.MutableStat)
		{
			_tickEvent?.Invoke(new RemoveImmutableStatsAbnormalArgs(base.FixedValue.SyncID));
		}
	}

	public override void Dispose()
	{
		if (DotFixedValue.Category != AbnormalStatsCategory.MutableStat)
		{
			_tickEvent?.Invoke(new RemoveImmutableStatsAbnormalArgs(base.FixedValue.SyncID));
		}
	}
}
