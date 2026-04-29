using DarkTonic.MasterAudio;
using UnityEngine;

public class SocketAudioAttachable : SocketAttachable
{
	[SerializeField]
	private string? sfxId = "";

	protected PlaySoundResult? _sfxResult;

	private bool _isAttached;

	private float _soundCheckTimer;

	private const float SOUND_CHECK_INTERVAL = 0.1f;

	public override void OnAttachToSocket()
	{
		_isAttached = true;
		OnAttachSound();
	}

	public override void OnDetachFromSocket()
	{
		_isAttached = false;
		_soundCheckTimer = 0f;
		OnDetachSound();
	}

	protected virtual void OnAttachSound()
	{
		PlaySound();
	}

	protected virtual void OnDetachSound()
	{
		StopSound();
	}

	protected void PlaySound()
	{
		_sfxResult = Hub.s.audioman.PlaySfxTransform(sfxId, base.transform);
	}

	protected void StopSound()
	{
		if (_sfxResult != null)
		{
			if (_sfxResult.ActingVariation != null)
			{
				_sfxResult.ActingVariation.Stop();
			}
			_sfxResult = null;
		}
	}

	protected virtual void OnEnable()
	{
		if (_isAttached)
		{
			PlaySound();
		}
	}

	protected virtual void Update()
	{
		if (!_isAttached)
		{
			return;
		}
		_soundCheckTimer += Time.deltaTime;
		if (_soundCheckTimer >= 0.1f)
		{
			_soundCheckTimer = 0f;
			if (_sfxResult == null || _sfxResult.ActingVariation == null || !_sfxResult.ActingVariation.IsPlaying)
			{
				PlaySound();
			}
		}
	}

	private void OnDestroy()
	{
		StopSound();
	}
}
