using DarkTonic.MasterAudio;
using UnityEngine;

public class PinWheelSocketAttachable : SocketAttachable
{
	[Header("PinWheel Audio Settings")]
	[SerializeField]
	[Tooltip("바람개비 소리의 SoundGroup ID")]
	private string pinWheelSfxId = "PinWheel";

	[SerializeField]
	[Tooltip("소리가 시작되는 최소 속도")]
	private float minSpeedThreshold = 0.1f;

	[SerializeField]
	[Tooltip("최대 볼륨에 도달하는 속도")]
	private float maxSpeedForVolume = 2f;

	[SerializeField]
	[Tooltip("최소 볼륨 (0-1)")]
	private float minVolume = 0.1f;

	[SerializeField]
	[Tooltip("최대 볼륨 (0-1)")]
	private float maxVolume = 1f;

	[SerializeField]
	[Tooltip("볼륨 변화 속도 (부드러운 전환을 위해)")]
	private float volumeChangeSpeed = 2f;

	private PlaySoundResult? _pinWheelSfxResult;

	private float _currentVolume;

	private float _lastSpeed;

	private bool _isPlaying;

	private bool _isAttached;

	private void Update()
	{
		if (_isAttached && !(Hub.s == null) && !(Hub.s.gameSettingManager == null) && Hub.s.gameSettingManager.masterVolume != 0f && _pinWheelSfxResult != null && _pinWheelSfxResult.ActingVariation != null)
		{
			SoundGroupVariation actingVariation = _pinWheelSfxResult.ActingVariation;
			if (Mathf.Abs(actingVariation.VarAudio.volume - _currentVolume) > 0.01f)
			{
				actingVariation.VarAudio.volume = Mathf.Lerp(actingVariation.VarAudio.volume, _currentVolume, volumeChangeSpeed * Time.deltaTime);
			}
		}
	}

	public override void OnAttachToSocket()
	{
		_isAttached = true;
	}

	public override void OnDetachFromSocket()
	{
		_isAttached = false;
		StopWindmillSound();
	}

	public override void OnPuppetMove(float forward, float strafe, float speed)
	{
		base.OnPuppetMove(forward, strafe, speed);
		_lastSpeed = speed;
		if (speed >= minSpeedThreshold)
		{
			if (!_isPlaying)
			{
				StartWindmillSound();
			}
			UpdateWindmillSound(speed);
		}
		else if (_isPlaying)
		{
			StopWindmillSound();
		}
	}

	private void StartWindmillSound()
	{
		if (!(Hub.s == null) && !(Hub.s.gameSettingManager == null) && Hub.s.gameSettingManager.masterVolume != 0f && !string.IsNullOrEmpty(pinWheelSfxId))
		{
			_pinWheelSfxResult = Hub.s.audioman.PlaySfxTransform(pinWheelSfxId, base.transform);
			if (_pinWheelSfxResult != null && _pinWheelSfxResult.ActingVariation != null)
			{
				_pinWheelSfxResult.ActingVariation.VarAudio.volume = minVolume;
				_pinWheelSfxResult.ActingVariation.VarAudio.loop = true;
				_isPlaying = true;
			}
		}
	}

	private void UpdateWindmillSound(float speed)
	{
		if (_pinWheelSfxResult != null && !(_pinWheelSfxResult.ActingVariation == null))
		{
			float t = Mathf.Clamp01(speed / maxSpeedForVolume);
			_currentVolume = Mathf.Lerp(minVolume, maxVolume, t);
		}
	}

	private void StopWindmillSound()
	{
		if (_pinWheelSfxResult != null && _pinWheelSfxResult.ActingVariation != null)
		{
			_pinWheelSfxResult.ActingVariation.Stop();
			_pinWheelSfxResult = null;
		}
		_isPlaying = false;
		_currentVolume = 0f;
	}

	private void OnDestroy()
	{
		StopWindmillSound();
	}

	private void OnDisable()
	{
		StopWindmillSound();
	}
}
