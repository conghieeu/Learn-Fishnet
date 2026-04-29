using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlarmClock : MonoBehaviour
{
	[Serializable]
	private class AlarmTriggerTime
	{
		public int hour;

		public int minute;

		public string audioClipKey = string.Empty;
	}

	[Header("Hands")]
	[SerializeField]
	private Transform? hourHandPivot;

	[SerializeField]
	private Transform? minuteHandPivot;

	[SerializeField]
	private List<AlarmTriggerTime> alarmTriggerTimes = new List<AlarmTriggerTime>();

	private List<bool> alarmTriggeredFlags = new List<bool>();

	private void Awake()
	{
		ClearAlarmTriggeredFlags();
	}

	public void UpdateTime(TimeSpan now)
	{
		RotateHands(now);
		if (TryGetAlarmTriggerTime(now, out var alarmTriggerIndex))
		{
			TriggerAlarm(alarmTriggerIndex);
			SetAlarmTriggeredFlag(alarmTriggerIndex);
		}
	}

	public void ClearAlarmTriggeredFlags()
	{
		alarmTriggeredFlags = alarmTriggerTimes.Select((AlarmTriggerTime _) => false).ToList();
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

	private void SetAlarmTriggeredFlag(int alarmTriggerIndex)
	{
		if (alarmTriggerIndex >= 0 && alarmTriggerIndex < alarmTriggeredFlags.Count)
		{
			alarmTriggeredFlags[alarmTriggerIndex] = true;
		}
	}

	private bool TryGetAlarmTriggerTime(TimeSpan now, out int alarmTriggerIndex)
	{
		alarmTriggerIndex = alarmTriggerTimes.FindIndex((AlarmTriggerTime t) => t.hour == now.Hours && t.minute == now.Minutes);
		return alarmTriggerIndex > -1;
	}

	private void TriggerAlarm(int alarmTriggerIndex)
	{
		if (alarmTriggerIndex >= 0 && alarmTriggerIndex < alarmTriggerTimes.Count)
		{
			AlarmTriggerTime alarmTriggerTime = alarmTriggerTimes[alarmTriggerIndex];
			if (alarmTriggerTime != null)
			{
				Hub.s.audioman.PlaySfx(alarmTriggerTime.audioClipKey);
			}
		}
	}
}
