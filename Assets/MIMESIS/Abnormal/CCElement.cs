using System;
using Bifrost.ConstEnum;
using ReluProtocol;

public class CCElement : IAbnormalElement
{
	private OnActorEventDelegate? _tickEvent;

	public readonly CCType CCType;

	public long PushTime { get; private set; }

	public long DownTime { get; private set; }

	public PosWithRot TargetPos { get; private set; } = new PosWithRot();

	public PosWithRot StartPos { get; private set; } = new PosWithRot();

	public CCElement(CCType ccType, OnActorEventDelegate? tickEvent)
		: base(AbnormalCategory.CC)
	{
		if (tickEvent != null)
		{
			_tickEvent = (OnActorEventDelegate)Delegate.Combine(_tickEvent, tickEvent);
		}
		CCType = ccType;
		PushTime = 0L;
	}

	public override bool Initialize(long syncID, long abnormalObjectID, int casterObjectID, AbnormalInfo info, int index, int initialStack, long duration, PosWithRot? pos = null)
	{
		if (!base.Initialize(syncID, abnormalObjectID, casterObjectID, info, index, initialStack, duration, pos))
		{
			return false;
		}
		PushTime = 0L;
		pos?.CopyTo(TargetPos);
		Apply(0L);
		return true;
	}

	public override bool SetValue(AbnormalCommonInputArgs args)
	{
		if (!(args is AbnormalCCInputArgs abnormalCCInputArgs))
		{
			return false;
		}
		if (!base.SetValue(args))
		{
			return false;
		}
		PushTime = abnormalCCInputArgs.PushTime;
		if (abnormalCCInputArgs.TargetPos != null)
		{
			abnormalCCInputArgs.TargetPos.CopyTo(TargetPos);
		}
		if (abnormalCCInputArgs.CurrentPos != null)
		{
			abnormalCCInputArgs.CurrentPos.CopyTo(StartPos);
		}
		return true;
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
		if (IsActionAbnormal())
		{
			if (delta != 0L)
			{
				long num = Hub.s.timeutil.GetCurrentTickMilliSec() - base.StartTime;
				if (num < base.Duration)
				{
					UpdateCrowdControlArgs eventArgs = new UpdateCrowdControlArgs(TargetPos, StartPos, PushTime, num);
					_tickEvent?.Invoke(eventArgs);
				}
				else
				{
					base.Applied = true;
				}
			}
		}
		else
		{
			base.Applied = true;
		}
	}

	public override void Dispose()
	{
	}

	private bool IsActionAbnormal()
	{
		if (CCType != CCType.Airborne && CCType != CCType.Knockback && CCType != CCType.Knockdown)
		{
			return CCType == CCType.NormalPush;
		}
		return true;
	}
}
