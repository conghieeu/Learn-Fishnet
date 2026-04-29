using DarkTonic.MasterAudio;
using UnityEngine;

public class MusicBox : SocketAttachable
{
	private static readonly int TurnOnParameter = Animator.StringToHash("TurnOn");

	private static readonly int RotateSpeed = Animator.StringToHash("RotateSpeed");

	[SerializeField]
	private string musicBoxSfxId = "MusicBoxSound";

	[SerializeField]
	private float rotateSpeedFactor = 1f;

	private bool isAttached;

	private PlaySoundResult _musicBoxSfxResult;

	private float _soundCheckTimer;

	private const float SOUND_CHECK_INTERVAL = 0.1f;

	public override void OnAttachToSocket()
	{
		isAttached = true;
		PlayMusicBox(on: true);
	}

	public override void OnDetachFromSocket()
	{
		isAttached = false;
		PlayMusicBox(on: false);
	}

	private void OnEnable()
	{
		if (isAttached)
		{
			PlayMusicBox(on: true);
		}
	}

	private void OnDisable()
	{
		if (isAttached)
		{
			PlayMusicBox(on: false);
		}
	}

	private void Update()
	{
		if (!isAttached)
		{
			return;
		}
		_soundCheckTimer += Time.deltaTime;
		if (_soundCheckTimer >= 0.1f)
		{
			_soundCheckTimer = 0f;
			if (_musicBoxSfxResult == null || _musicBoxSfxResult.ActingVariation == null || !_musicBoxSfxResult.ActingVariation.IsPlaying)
			{
				_musicBoxSfxResult = Hub.s.audioman.PlaySfxTransform(musicBoxSfxId, base.transform);
			}
		}
	}

	private void PlayMusicBox(bool on)
	{
		if (on)
		{
			animator?.SetBool(TurnOnParameter, value: true);
			animator?.SetFloat(RotateSpeed, rotateSpeedFactor);
			_musicBoxSfxResult = Hub.s.audioman.PlaySfxTransform(musicBoxSfxId, base.transform);
			return;
		}
		animator?.SetBool(TurnOnParameter, value: false);
		animator?.SetFloat(RotateSpeed, 0f);
		if (_musicBoxSfxResult != null && _musicBoxSfxResult.ActingVariation != null)
		{
			_musicBoxSfxResult.ActingVariation.Stop();
		}
	}

	private void OnDestroy()
	{
		if (_musicBoxSfxResult != null && _musicBoxSfxResult.ActingVariation != null)
		{
			_musicBoxSfxResult.ActingVariation.Stop();
		}
	}
}
