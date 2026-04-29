using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class AlarmBounds : MonoBehaviour
{
	[SerializeField]
	private AlarmTargetType targetType = AlarmTargetType.All;

	[SerializeField]
	private List<AlarmRadiusData> alarmRadiusDataList = new List<AlarmRadiusData>();

	private Coroutine checkCoroutine;

	private float lastPlayTime;

	private string currentAudioKey;

	private PlaySoundResult? _sfxResult;

	public AlarmTargetType TargetType => targetType;

	public List<AlarmRadiusData> AlarmRadiusDataList => alarmRadiusDataList;

	private void Start()
	{
		alarmRadiusDataList.Sort((AlarmRadiusData a, AlarmRadiusData b) => a.radius.CompareTo(b.radius));
		TrapLevelObject componentInChildren = GetComponentInChildren<TrapLevelObject>();
		if (componentInChildren != null && componentInChildren.TrapType == TrapType.Mine_Invisible)
		{
			componentInChildren.AddOnTrapStateChanged(OnTrapStateChanged);
		}
		StartCheckingBounds();
	}

	private void OnTrapStateChanged(TrapState state)
	{
		if (state == TrapState.Triggered)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		StopCheckingBounds();
	}

	public void StartCheckingBounds()
	{
		StopCheckingBounds();
		checkCoroutine = StartCoroutine(CheckBoundsAndPlayAudio());
	}

	public void StopCheckingBounds()
	{
		if (checkCoroutine != null)
		{
			StopCoroutine(checkCoroutine);
			checkCoroutine = null;
		}
	}

	private IEnumerator CheckBoundsAndPlayAudio()
	{
		while (true)
		{
			if (Hub.s == null || Hub.s.pdata == null || Hub.s.pdata.main == null)
			{
				yield return null;
				continue;
			}
			(string? audioKey, AlarmRadiusData? radiusData) audioKeyAndDataForActorsInRange = GetAudioKeyAndDataForActorsInRange();
			string item = audioKeyAndDataForActorsInRange.audioKey;
			AlarmRadiusData? item2 = audioKeyAndDataForActorsInRange.radiusData;
			float time = Time.time;
			if (item2.HasValue)
			{
				if (item != currentAudioKey || time >= lastPlayTime + item2.Value.playInterval)
				{
					_sfxResult = Hub.s.audioman.PlaySfxTransform(item, base.transform);
					currentAudioKey = item;
					lastPlayTime = time;
				}
			}
			else if (currentAudioKey != null)
			{
				if (_sfxResult != null && _sfxResult.ActingVariation != null)
				{
					_sfxResult = null;
				}
				currentAudioKey = null;
			}
			yield return null;
		}
	}

	private (string? audioKey, AlarmRadiusData? radiusData) GetAudioKeyAndDataForActorsInRange()
	{
		if (alarmRadiusDataList.Count == 0)
		{
			return (audioKey: null, radiusData: null);
		}
		Vector3 position = base.transform.position;
		float? num = null;
		AlarmRadiusData? alarmRadiusData = null;
		foreach (ProtoActor value in Hub.s.pdata.main.GetProtoActorMap().Values)
		{
			if (value == null || targetType switch
			{
				AlarmTargetType.Ally => (value.ActorType == ActorType.Player) ? 1 : 0, 
				AlarmTargetType.Enemy => (value.ActorType != ActorType.Player) ? 1 : 0, 
				AlarmTargetType.All => 1, 
				_ => 0, 
			} == 0)
			{
				continue;
			}
			float num2 = Vector3.Distance(position, value.transform.position);
			foreach (AlarmRadiusData alarmRadiusData2 in alarmRadiusDataList)
			{
				if (num2 <= alarmRadiusData2.radius)
				{
					if (!num.HasValue || alarmRadiusData2.radius < num.Value)
					{
						num = alarmRadiusData2.radius;
						alarmRadiusData = alarmRadiusData2;
					}
					break;
				}
			}
		}
		return (audioKey: alarmRadiusData?.audioKey, radiusData: alarmRadiusData);
	}

	public string? GetAudioKeyForActorsInRange()
	{
		if (alarmRadiusDataList.Count == 0)
		{
			return null;
		}
		Vector3 position = base.transform.position;
		float? num = null;
		string result = null;
		foreach (ProtoActor value in Hub.s.pdata.main.GetProtoActorMap().Values)
		{
			if (value == null || targetType switch
			{
				AlarmTargetType.Ally => (value.ActorType == ActorType.Player) ? 1 : 0, 
				AlarmTargetType.Enemy => (value.ActorType != ActorType.Player) ? 1 : 0, 
				AlarmTargetType.All => 1, 
				_ => 0, 
			} == 0)
			{
				continue;
			}
			float num2 = Vector3.Distance(position, value.transform.position);
			foreach (AlarmRadiusData alarmRadiusData in alarmRadiusDataList)
			{
				if (num2 <= alarmRadiusData.radius)
				{
					if (!num.HasValue || alarmRadiusData.radius < num.Value)
					{
						num = alarmRadiusData.radius;
						result = alarmRadiusData.audioKey;
					}
					break;
				}
			}
		}
		return result;
	}
}
