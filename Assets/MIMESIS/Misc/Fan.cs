using DarkTonic.MasterAudio;
using UnityEngine;

public class Fan : SocketAttachable, IToggleableItem, IGaugeableItem
{
	private static readonly int TurnOn = Animator.StringToHash("TurnOn");

	[SerializeField]
	private string turnOnAudioClipName = "fan_on";

	[SerializeField]
	private string turnOffAudioClipName = "fan_off";

	[SerializeField]
	private string fanLoopAudioClipName = "fan_loop";

	private bool isAttached;

	private PlaySoundResult fanLoopSoundResult;

	private float _soundCheckTimer;

	private const float SOUND_CHECK_INTERVAL = 0.1f;

	public override void OnAttachToSocket()
	{
		isAttached = true;
	}

	public override void OnDetachFromSocket()
	{
		isAttached = false;
		TurnFan(on: false);
	}

	private void OnEnable()
	{
		if (isAttached)
		{
			TurnFan(base.item.IsTurnOn);
		}
	}

	private void OnDisable()
	{
		if (isAttached && fanLoopSoundResult != null)
		{
			fanLoopSoundResult.ActingVariation?.Stop();
			fanLoopSoundResult = null;
		}
	}

	private void Update()
	{
		if (!isAttached || !base.item.IsTurnOn)
		{
			return;
		}
		_soundCheckTimer += Time.deltaTime;
		if (_soundCheckTimer >= 0.1f)
		{
			_soundCheckTimer = 0f;
			if (fanLoopSoundResult == null || fanLoopSoundResult.ActingVariation == null || !fanLoopSoundResult.ActingVariation.IsPlaying)
			{
				fanLoopSoundResult = Hub.s.audioman.PlaySfxTransform(fanLoopAudioClipName, base.transform);
			}
		}
	}

	public void OnToggled(long itemID, bool toggleOn)
	{
		TurnFan(toggleOn);
		PlayFanSFX(toggleOn);
	}

	public void OnGaugeChanged(long itemID, int remainGauge)
	{
		if (remainGauge <= 0)
		{
			TurnFan(on: false);
			PlayFanSFX(on: false);
		}
	}

	private void TurnFan(bool on)
	{
		if (!(base.owner == null))
		{
			animator?.SetBool(TurnOn, on);
			if (on)
			{
				fanLoopSoundResult = Hub.s.audioman.PlaySfxTransform(fanLoopAudioClipName, base.transform);
			}
			else if (fanLoopSoundResult != null)
			{
				fanLoopSoundResult.ActingVariation?.Stop();
				fanLoopSoundResult = null;
			}
		}
	}

	private void PlayFanSFX(bool on)
	{
		if (on)
		{
			Hub.s.audioman.PlaySfx(turnOnAudioClipName, base.transform);
		}
		else
		{
			Hub.s.audioman.PlaySfx(turnOffAudioClipName, base.transform);
		}
	}
}
