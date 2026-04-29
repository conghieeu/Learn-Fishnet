using DarkTonic.MasterAudio;
using UnityEngine;

namespace Mimic.Actors
{
	public class Flashlight : SocketAttachable, IToggleableItem, IGaugeableItem
	{
		[SerializeField]
		[Tooltip("손에 들었을 때 사용할 광원")]
		private Light? flashlight;

		[SerializeField]
		[Tooltip("켤 때 재생할 오디오 클립")]
		private string turnOnAudioClipName = "flash_light_on";

		[SerializeField]
		[Tooltip("끌 때 재생할 오디오 클립")]
		private string turnOffAudioClipName = "flash_light_off";

		[SerializeField]
		[Tooltip("효과음 재생을 위한 MasterAudio")]
		private string fxAudioClipName = "";

		private PlaySoundResult? _fxAudioResult;

		public void OnToggled(long itemID, bool toggleOn)
		{
			TurnLight(toggleOn);
		}

		public void OnGaugeChanged(long itemID, int remainGauge)
		{
			if (remainGauge <= 0)
			{
				TurnLight(on: false);
			}
		}

		private void TurnLight(bool on)
		{
			if (!(flashlight == null) && flashlight.gameObject.activeSelf != on)
			{
				flashlight.gameObject.SetActive(on);
				if (on)
				{
					base.owner.OverrideMountedLight(flashlight);
				}
				if (!base.owner.IsMountedLightOn())
				{
					Hub.s.audioman.PlaySfx(on ? turnOnAudioClipName : turnOffAudioClipName, base.gameObject.transform);
				}
			}
		}

		public override void OnDetachFromSocket()
		{
			base.OnDetachFromSocket();
			if (_fxAudioResult != null)
			{
				_fxAudioResult?.ActingVariation?.Stop();
				_fxAudioResult = null;
			}
		}

		public override void OnAttachToSocket()
		{
			base.OnAttachToSocket();
			if (!string.IsNullOrEmpty(fxAudioClipName))
			{
				_fxAudioResult = Hub.s.audioman.PlaySfxTransform(fxAudioClipName, base.transform);
			}
		}
	}
}
