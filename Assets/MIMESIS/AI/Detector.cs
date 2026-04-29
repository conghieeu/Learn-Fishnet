using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Mimic.Actors;
using UnityEngine;

public class Detector : SocketAttachable
{
	[Header("Detection Settings")]
	[SerializeField]
	public float detectionRange = 30f;

	[Header("Alarm Settings")]
	[SerializeField]
	private float alarmDuration = 3.5f;

	[SerializeField]
	private string alarmSoundKey = "detector_alarm";

	[SerializeField]
	private List<int> detectingMonsterMasterIds = new List<int>();

	private float lastAlarmTime;

	private bool alarmEnabled;

	private bool isCurrentlyDetecting;

	private float detectionLostTime;

	private PlaySoundResult currentAlarmSound;

	private void OnDisable()
	{
		StopAlarm();
		alarmEnabled = false;
		isCurrentlyDetecting = false;
		detectionLostTime = 0f;
	}

	private void OnDestroy()
	{
		StopAlarm();
	}

	public override void OnAttachToSocket()
	{
		alarmEnabled = true;
	}

	public override void OnDetachFromSocket()
	{
		StopAlarm();
		alarmEnabled = false;
		isCurrentlyDetecting = false;
		detectionLostTime = 0f;
	}

	private void Update()
	{
		if (alarmEnabled)
		{
			bool flag = IsDetected();
			if (flag && !isCurrentlyDetecting)
			{
				StartDetection();
			}
			else if (!flag && isCurrentlyDetecting && detectionLostTime == 0f)
			{
				detectionLostTime = Time.time;
			}
			else if (flag && isCurrentlyDetecting && detectionLostTime > 0f)
			{
				detectionLostTime = 0f;
			}
			if (!flag && isCurrentlyDetecting && detectionLostTime > 0f && Time.time > detectionLostTime + alarmDuration)
			{
				StopDetection();
			}
			if (isCurrentlyDetecting && Time.time > lastAlarmTime + alarmDuration)
			{
				TriggerAlarm();
			}
		}
	}

	private bool IsDetected()
	{
		if (Hub.s?.pdata?.main == null)
		{
			return false;
		}
		Hub s = Hub.s;
		if ((object)s != null && s.pdata?.main.IsGameLogicRunning == false)
		{
			return false;
		}
		foreach (int detectingMonsterMasterId in detectingMonsterMasterIds)
		{
			List<ProtoActor> monstersInRange = Hub.s.pdata.main.GetMonstersInRange(detectingMonsterMasterId, base.transform.position, detectionRange);
			if (monstersInRange != null && monstersInRange.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	private void StartDetection()
	{
		isCurrentlyDetecting = true;
		lastAlarmTime = 0f;
	}

	private void StopDetection()
	{
		isCurrentlyDetecting = false;
		detectionLostTime = 0f;
		StopAlarm();
	}

	private void TriggerAlarm()
	{
		OnDetect();
		PlayAlarm();
		lastAlarmTime = Time.time;
	}

	private void PlayAlarm()
	{
		StopAlarm();
		if (!string.IsNullOrEmpty(alarmSoundKey) && Hub.s?.audioman != null)
		{
			currentAlarmSound = Hub.s.audioman.PlaySfxTransform(alarmSoundKey, base.transform);
		}
	}

	private void StopAlarm()
	{
		if (currentAlarmSound?.ActingVariation != null)
		{
			currentAlarmSound.ActingVariation.Stop();
			currentAlarmSound = null;
		}
	}

	private bool IsAlarmCurrentlyPlaying()
	{
		if (currentAlarmSound?.ActingVariation != null)
		{
			return currentAlarmSound.ActingVariation.IsPlaying;
		}
		return false;
	}
}
