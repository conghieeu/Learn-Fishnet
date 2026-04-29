using System;
using UnityEngine;

public class Clock : SocketAttachable, ITimeSyncable
{
	[Header("Hands")]
	[SerializeField]
	private Transform? hourHandPivot;

	[SerializeField]
	private Transform? minuteHandPivot;

	private void OnEnable()
	{
		if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main != null)
		{
			RotateHands(Hub.s.pdata.main.CurrentTime);
			Hub.s.pdata.main.AddTimeSyncable(this);
		}
	}

	private void OnDisable()
	{
		if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main != null)
		{
			Hub.s.pdata.main.RemoveTimeSyncable(this);
		}
	}

	public void OnTimeSync(TimeSpan now)
	{
		RotateHands(now);
	}

	private void RotateHands(TimeSpan now)
	{
		if (hourHandPivot != null)
		{
			hourHandPivot.localRotation = Quaternion.Euler(0f, 0f, 360 * (now.Hours % 12) / 12);
		}
		if (minuteHandPivot != null)
		{
			minuteHandPivot.localRotation = Quaternion.Euler(0f, 0f, 360 * now.Minutes / 60);
		}
	}
}
