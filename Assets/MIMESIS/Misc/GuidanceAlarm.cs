using System.Collections;
using DarkTonic.MasterAudio;
using UnityEngine;

public class GuidanceAlarm : MonoBehaviour
{
	[Header("Alarm")]
	[SerializeField]
	private string damagedTramAlarmKey = "alarm-warning";

	[SerializeField]
	private float damagedTramAlarmSec = 10f;

	private PlaySoundResult? _sfxResult;

	public void PlayDamagedTramAlarm()
	{
		StartCoroutine(CorPlayDamagedTramAlarm());
	}

	private IEnumerator CorPlayDamagedTramAlarm()
	{
		_sfxResult = Hub.s.audioman.PlaySfxTransform(damagedTramAlarmKey, base.transform);
		yield return new WaitForSeconds(damagedTramAlarmSec);
		if (_sfxResult != null && _sfxResult.ActingVariation != null)
		{
			_sfxResult.ActingVariation.Stop();
		}
	}
}
