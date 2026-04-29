using DarkTonic.MasterAudio;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	[SerializeField]
	protected string sfxId = "";

	private PlaySoundResult? _sfxResult;

	private void Start()
	{
		if (Hub.s != null && Hub.s.audioman != null)
		{
			_sfxResult = Hub.s.audioman.PlaySfxTransform(sfxId, base.transform);
		}
	}

	private void OnDestroy()
	{
		if (Hub.s != null && Hub.s.audioman != null && _sfxResult != null && _sfxResult.ActingVariation != null && _sfxResult.ActingVariation.IsPlaying)
		{
			_sfxResult.ActingVariation.Stop();
		}
	}
}
