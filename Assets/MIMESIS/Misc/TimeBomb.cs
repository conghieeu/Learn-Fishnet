using DarkTonic.MasterAudio;
using ReluProtocol.Enum;
using UnityEngine;

public class TimeBomb : SocketAttachable, IGaugeableItem
{
	[SerializeField]
	private string BombBeepFastAudioClipName = "bomb_beep_fast";

	[SerializeField]
	private string BombBeepSlowAudioClipName = "bomb_beep_slow";

	private string? _currentBeepAudioClipName;

	[SerializeField]
	[Tooltip("Beep 소리가 변경되는 폭발까지 남은 시간 (초)")]
	private int _remainingTimeToExplode = 5;

	private PlaySoundResult? _beepSlowSfxResult;

	private PlaySoundResult? _beepFastSfxResult;

	private bool _isAttached;

	private float _soundCheckTimer;

	private const float SOUND_CHECK_INTERVAL = 0.1f;

	public override void OnAttachToSocket()
	{
		if (base.item.IsFake || base.item.Durability <= 0)
		{
			PlayBeepAudioClip(BombBeepFastAudioClipName);
		}
		else if (!_isAttached || !IsAudioPlaying())
		{
			_isAttached = true;
			_currentBeepAudioClipName = BombBeepSlowAudioClipName;
			if (!IsAudioPlaying())
			{
				PlayBeepAudioClip(_currentBeepAudioClipName);
			}
		}
	}

	private void OnEnable()
	{
		if (_isAttached && !string.IsNullOrEmpty(_currentBeepAudioClipName))
		{
			PlayBeepAudioClip(_currentBeepAudioClipName);
		}
	}

	private void OnDisable()
	{
		if (_isAttached)
		{
			StopAudio();
		}
	}

	private void Update()
	{
		if (!_isAttached)
		{
			return;
		}
		_soundCheckTimer += Time.deltaTime;
		if (_soundCheckTimer >= 0.1f)
		{
			_soundCheckTimer = 0f;
			if (!IsAudioPlaying() && !string.IsNullOrEmpty(_currentBeepAudioClipName))
			{
				PlayBeepAudioClip(_currentBeepAudioClipName);
			}
		}
	}

	public override void OnDetachFromSocket()
	{
		_isAttached = false;
		StopAudio();
	}

	private void OnDestroy()
	{
		StopAudio();
	}

	private void UpdateRemainTime(int remainTime)
	{
		if (remainTime > 0)
		{
			string beepAudioClipName = GetBeepAudioClipName(remainTime);
			if (_currentBeepAudioClipName != beepAudioClipName)
			{
				_currentBeepAudioClipName = beepAudioClipName;
				StopAudio();
				PlayBeepAudioClip(_currentBeepAudioClipName);
			}
		}
	}

	private string GetBeepAudioClipName(int remainTime)
	{
		if (remainTime > _remainingTimeToExplode && base.item.Durability > 0)
		{
			return BombBeepSlowAudioClipName;
		}
		return BombBeepFastAudioClipName;
	}

	public bool IsFastBeeping()
	{
		return _currentBeepAudioClipName == BombBeepFastAudioClipName;
	}

	private bool IsAudioPlaying()
	{
		if (_beepSlowSfxResult == null || !(_beepSlowSfxResult.ActingVariation != null) || !_beepSlowSfxResult.ActingVariation.IsPlaying)
		{
			if (_beepFastSfxResult != null && _beepFastSfxResult.ActingVariation != null)
			{
				return _beepFastSfxResult.ActingVariation.IsPlaying;
			}
			return false;
		}
		return true;
	}

	private void StopAudio()
	{
		if (_beepSlowSfxResult != null)
		{
			if (_beepSlowSfxResult.ActingVariation != null)
			{
				_beepSlowSfxResult.ActingVariation.Stop();
			}
			_beepSlowSfxResult = null;
		}
		if (_beepFastSfxResult != null)
		{
			if (_beepFastSfxResult.ActingVariation != null)
			{
				_beepFastSfxResult.ActingVariation.Stop();
			}
			_beepFastSfxResult = null;
		}
	}

	private void PlayBeepAudioClip(string? clipId)
	{
		if (!string.IsNullOrEmpty(clipId))
		{
			if (clipId == BombBeepSlowAudioClipName)
			{
				_beepSlowSfxResult = Hub.s.audioman.PlaySfxTransform(clipId, base.owner.SfxRoot);
			}
			else
			{
				_beepFastSfxResult = Hub.s.audioman.PlaySfxTransform(clipId, base.owner.SfxRoot);
			}
		}
	}

	public void OnGaugeChanged(long itemID, int remainGauge)
	{
		if (!(base.owner != null) || base.owner.ActorType == ActorType.Player)
		{
			UpdateRemainTime(remainGauge);
		}
	}
}
