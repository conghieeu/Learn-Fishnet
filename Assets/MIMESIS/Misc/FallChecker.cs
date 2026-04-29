using UnityEngine;

public struct FallChecker
{
	public long _lastFallTime;

	public long _FallCheckStopTime;

	public float TotalFallDistance { get; private set; }

	public void Reset()
	{
		_lastFallTime = 0L;
		TotalFallDistance = 0f;
	}

	private float CalculateFallDamage()
	{
		float num = Hub.s.dataman.ExcelDataManager.Consts.C_FallSafeDistance;
		float num2 = Hub.s.dataman.ExcelDataManager.Consts.C_FallHazardDistance;
		float num3 = 0f;
		if (TotalFallDistance <= num)
		{
			Reset();
			return 0f;
		}
		num3 = ((!(TotalFallDistance >= num2)) ? (TotalFallDistance / num2 * 100f) : 100f);
		Reset();
		return num3;
	}

	public float UpdateMove(Vector3 before, Vector3 after, bool stopped)
	{
		float num = before.y - after.y;
		if (num <= 0f || stopped)
		{
			if (_lastFallTime != 0L)
			{
				return CheckLanding(immediate: true);
			}
			Reset();
			return 0f;
		}
		_lastFallTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		TotalFallDistance += num;
		return 0f;
	}

	public void Pause()
	{
		_FallCheckStopTime = Hub.s.timeutil.GetCurrentTickMilliSec() + 2000;
	}

	public float CheckLanding(bool immediate = false)
	{
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (currentTickMilliSec < _FallCheckStopTime)
		{
			if (_lastFallTime != 0L)
			{
				Reset();
			}
			return 0f;
		}
		_FallCheckStopTime = 0L;
		if (_lastFallTime != 0L)
		{
			if (!immediate && currentTickMilliSec - _lastFallTime < 330)
			{
				return 0f;
			}
			return CalculateFallDamage();
		}
		return 0f;
	}
}
