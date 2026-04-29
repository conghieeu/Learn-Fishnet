using System;
using System.Collections.Generic;

public class SkillSequenceHitLog : IDisposable
{
	public int TotalHitCount;

	public Dictionary<int, TargetHitLog> TargetHitLog = new Dictionary<int, TargetHitLog>();

	public readonly float HitInterval;

	private int _hitCountLimit;

	public SkillSequenceHitLog(int hitCount, float hitInterval)
	{
		TotalHitCount = 0;
		HitInterval = hitInterval;
		_hitCountLimit = hitCount;
	}

	public bool CheckHitCountLimit()
	{
		if (_hitCountLimit > 0)
		{
			return TotalHitCount < _hitCountLimit;
		}
		return true;
	}

	public bool CheckHitTerm(int objectID, long currentTick)
	{
		if (!CheckHitCountLimit())
		{
			return false;
		}
		if (!TargetHitLog.TryGetValue(objectID, out var value))
		{
			value = new TargetHitLog(1, currentTick);
			TargetHitLog.Add(objectID, value);
			TotalHitCount++;
			return true;
		}
		if (HitInterval == -1f)
		{
			return true;
		}
		if (HitInterval == 0f)
		{
			return false;
		}
		long num = currentTick - value.LastHitTimestamp;
		if (HitInterval > (float)num)
		{
			return false;
		}
		value.HitCount++;
		value.LastHitTimestamp = currentTick;
		TotalHitCount++;
		return true;
	}

	public void Dispose()
	{
		TargetHitLog.Clear();
	}
}
