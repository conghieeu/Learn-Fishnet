using DarkTonic.MasterAudio;
using Mimic.Actors;
using UnityEngine;

namespace Mimic.Audio
{
	public class SpatialAudioYAxisRolloffFilter : MonoBehaviour
	{
		[SerializeField]
		private SoundGroupVariation? variation;

		[SerializeField]
		private float updateInterval = 0.1f;

		private float timer;

		private float originalVariationVolume = 1f;

		private bool hasStoredOriginalVolume;

		private float currentTargetVolume = 1f;

		private float volumeSmoothTime = 0.1f;

		private float volumeVelocity;

		private AnimationCurve? rolloffCurve;

		private static AnimationCurve? GetRolloffCurve()
		{
			AnimationCurve result = null;
			if (Hub.s != null && Hub.s.tableman != null && Hub.s.tableman.audioClip != null)
			{
				result = Hub.s.tableman.audioClip.yAxisRolloffCurve;
			}
			return result;
		}

		private void OnEnable()
		{
			if (variation == null)
			{
				TryGetComponent<SoundGroupVariation>(out variation);
			}
			if (rolloffCurve == null)
			{
				rolloffCurve = GetRolloffCurve();
			}
			if (!hasStoredOriginalVolume && variation != null)
			{
				originalVariationVolume = variation.VarAudio.volume;
				hasStoredOriginalVolume = true;
			}
			currentTargetVolume = variation?.VarAudio.volume ?? originalVariationVolume;
			UpdateYAxisVolume(applyVolume: true);
		}

		private void OnDisable()
		{
			if (variation != null)
			{
				variation.VarAudio.volume = originalVariationVolume;
			}
		}

		private void Update()
		{
			if (!(variation == null) && variation.VarAudio.isPlaying)
			{
				timer += Time.deltaTime;
				if (timer >= updateInterval)
				{
					timer = 0f;
					UpdateYAxisVolume();
				}
				float volume = variation.VarAudio.volume;
				if (Mathf.Abs(volume - currentTargetVolume) > 0.001f)
				{
					float volume2 = Mathf.SmoothDamp(volume, currentTargetVolume, ref volumeVelocity, volumeSmoothTime);
					variation.VarAudio.volume = volume2;
				}
			}
		}

		private void UpdateYAxisVolume(bool applyVolume = false)
		{
			if (variation == null || !TryGetListenerPosition(out var listenerPosition))
			{
				return;
			}
			Transform transform = variation.ObjectToFollow ?? variation.ObjectToTriggerFrom;
			if (!(transform == null))
			{
				float time = Mathf.Abs(transform.position.y - listenerPosition.y);
				float num = rolloffCurve?.Evaluate(time) ?? 1f;
				currentTargetVolume = originalVariationVolume * num;
				if (applyVolume)
				{
					variation.VarAudio.volume = currentTargetVolume;
					volumeVelocity = 0f;
				}
			}
		}

		private bool TryGetListenerPosition(out Vector3 listenerPosition)
		{
			if (Hub.s?.pdata?.main != null && Hub.s.cameraman != null)
			{
				ProtoActor target;
				ProtoActor protoActor = ((!Hub.s.cameraman.TryGetCurrentSpectatorTarget(out target)) ? Hub.s.pdata.main.GetMyAvatar() : target);
				if (protoActor != null)
				{
					listenerPosition = protoActor.transform.position;
					return true;
				}
			}
			listenerPosition = default(Vector3);
			return false;
		}
	}
}
