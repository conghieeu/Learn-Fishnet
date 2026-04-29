using Mimic.Actors;
using UnityEngine;

namespace Mimic.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public sealed class VoiceSpatialAudioYAxisRolloffFilter : MonoBehaviour
	{
		private AudioSource? audioSource;

		private AnimationCurve? rolloffCurve;

		private volatile float cachedRolloffValue;

		private volatile float cachedSpatialBlend = 1f;

		private volatile bool hasValidRolloff;

		private void Awake()
		{
			if (!TryGetComponent<AudioSource>(out audioSource))
			{
				Logger.RError("AudioSource component not found on " + base.gameObject.name);
				base.enabled = false;
				return;
			}
			rolloffCurve = GetRolloffCurve();
			if (rolloffCurve == null || rolloffCurve.length == 0)
			{
				Logger.RWarn("Y-Axis rolloff curve not found or empty on " + base.gameObject.name + ". Disabling component.", sendToLogServer: false, useConsoleOut: true, "audio");
				base.enabled = false;
			}
		}

		private void Update()
		{
			if (!(audioSource == null) && rolloffCurve != null)
			{
				if (TryGetListenerPosition(out var listenerPosition))
				{
					float time = Mathf.Abs(base.transform.position.y - listenerPosition.y);
					cachedRolloffValue = rolloffCurve.Evaluate(time);
					hasValidRolloff = true;
				}
				else
				{
					cachedRolloffValue = 0f;
					hasValidRolloff = false;
				}
				cachedSpatialBlend = audioSource.spatialBlend;
			}
		}

		private bool TryGetListenerPosition(out Vector3 listenerPosition)
		{
			if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main != null)
			{
				ProtoActor target;
				ProtoActor protoActor = ((!Hub.s.cameraman.TryGetCurrentSpectatorTarget(out target)) ? Hub.s.pdata.main.GetMyAvatar() : target);
				if (protoActor != null && protoActor.transform != null)
				{
					listenerPosition = protoActor.transform.position;
					return true;
				}
			}
			listenerPosition = default(Vector3);
			return false;
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (!(audioSource == null) && hasValidRolloff && !(cachedSpatialBlend <= 0f))
			{
				float num = cachedSpatialBlend;
				float num2 = cachedRolloffValue;
				float num3 = 1f - num;
				float num4 = num * num2;
				for (int i = 0; i < data.Length; i++)
				{
					data[i] *= num3 + num4;
				}
			}
		}

		private static AnimationCurve? GetRolloffCurve()
		{
			try
			{
				AnimationCurve animationCurve = null;
				if (Hub.s != null && Hub.s.tableman != null && Hub.s.tableman.audioClip != null)
				{
					animationCurve = Hub.s.tableman.audioClip.yAxisRolloffCurve;
				}
				if (animationCurve == null || animationCurve.length == 0)
				{
					return null;
				}
				return animationCurve;
			}
			catch
			{
				return null;
			}
		}
	}
}
