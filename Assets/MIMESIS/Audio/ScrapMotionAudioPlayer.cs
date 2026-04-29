using DarkTonic.MasterAudio;
using Mimic.Audio;
using UnityEngine;

public class ScrapMotionAudioPlayer : SocketAttachable, IMotionableItem
{
	[Header("Master Audio")]
	[SerializeField]
	[Tooltip("스크랩 모션의 재생부터 종료까지 재생할 SoundGroup ID")]
	private string scrapMotionSfxId = string.Empty;

	private PlaySoundResult? _scrapMotionSfxResult;

	private bool _isMotionStarted;

	private float _soundCheckTimer;

	private const float SOUND_CHECK_INTERVAL = 0.1f;

	public void OnMotionStarted()
	{
		if (TryGetAudioManager(out AudioManager audioman))
		{
			_scrapMotionSfxResult = audioman.PlaySfxTransform(scrapMotionSfxId, base.transform);
			_isMotionStarted = true;
		}
	}

	public void OnMotionStopped()
	{
		_isMotionStarted = false;
		_soundCheckTimer = 0f;
		if (TryGetAudioManager(out AudioManager _) && _scrapMotionSfxResult != null)
		{
			if (_scrapMotionSfxResult.ActingVariation != null)
			{
				_scrapMotionSfxResult.ActingVariation.Stop();
			}
			_scrapMotionSfxResult = null;
		}
	}

	public override void OnDetachFromSocket()
	{
		OnMotionStopped();
	}

	private void OnEnable()
	{
		if (_isMotionStarted)
		{
			OnMotionStarted();
		}
	}

	private void Update()
	{
		if (!_isMotionStarted)
		{
			return;
		}
		_soundCheckTimer += Time.deltaTime;
		if (_soundCheckTimer >= 0.1f)
		{
			_soundCheckTimer = 0f;
			if ((_scrapMotionSfxResult == null || _scrapMotionSfxResult.ActingVariation == null || !_scrapMotionSfxResult.ActingVariation.IsPlaying) && TryGetAudioManager(out AudioManager audioman))
			{
				_scrapMotionSfxResult = audioman.PlaySfxTransform(scrapMotionSfxId, base.transform);
			}
		}
	}

	private static bool TryGetAudioManager(out AudioManager? audioman)
	{
		audioman = ((Hub.s != null) ? Hub.s.audioman : null);
		return audioman != null;
	}
}
