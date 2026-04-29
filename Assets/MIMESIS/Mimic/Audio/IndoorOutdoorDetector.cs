using System.Collections;
using Mimic.Animation;
using UnityEngine;
using UnityEngine.Audio;

namespace Mimic.Audio
{
	public class IndoorOutdoorDetector : MonoBehaviour
	{
		[Header("Detection Settings")]
		[SerializeField]
		[Tooltip("머리에서 위로 향하는 LineTrace 거리")]
		private float detectionDistance = 10f;

		[SerializeField]
		[Tooltip("LineTrace에 사용할 레이어 마스크")]
		private LayerMask detectionLayerMask = -1;

		[Header("Raycast Optimization")]
		[SerializeField]
		[Tooltip("Raycast 캐시 지속 시간 (초)")]
		private float raycastCacheDuration = 0.5f;

		[SerializeField]
		[Tooltip("Update 호출 간격 (초) - 0이면 매 프레임")]
		private float updateInterval = 0.1f;

		[Header("Audio Parameters")]
		[SerializeField]
		[Tooltip("실내일 때 적용할 오디오 파라미터 설정")]
		private AudioEnvironmentSettings indoorSettings = new AudioEnvironmentSettings();

		[SerializeField]
		[Tooltip("실외일 때 적용할 오디오 파라미터 설정")]
		private AudioEnvironmentSettings outdoorSettings = new AudioEnvironmentSettings();

		[Header("Interpolation Settings")]
		[SerializeField]
		[Tooltip("오디오 파라미터 값이 변경될 때 보간 시간 (초)")]
		private float interpolationDuration = 2f;

		private bool _isIndoor;

		private bool _lastDetectionResult;

		private PuppetScript? _puppet;

		private Animator? animator;

		private float lastFootstep;

		private string animatorParameterName = "Footstep";

		private float animatorParameterThreshold = 0.01f;

		private int animatorParameterHash;

		private float lastUpdateTime;

		private bool raycastCacheValid;

		private bool cachedRaycastResult;

		private float lastRaycastTime;

		private Vector3 lastHeadPosition = Vector3.zero;

		private readonly float positionChangeThreshold = 0.5f;

		private Coroutine? currentInterpolationCoroutine;

		private AudioEnvironmentSettings? currentTargetSettings;

		private AudioEnvironmentSettings? currentStartSettings;

		public bool IsIndoor => _isIndoor;

		private void Start()
		{
			_puppet = GetComponent<PuppetScript>();
			animator = GetComponent<Animator>();
			animatorParameterHash = Animator.StringToHash(animatorParameterName);
		}

		private void OnDisable()
		{
			StopCurrentInterpolation();
		}

		private void OnDestroy()
		{
			StopCurrentInterpolation();
		}

		private void Update()
		{
			if (animator == null)
			{
				return;
			}
			float time = Time.time;
			if (updateInterval > 0f && time - lastUpdateTime < updateInterval)
			{
				return;
			}
			lastUpdateTime = time;
			float num = animator.GetFloat(animatorParameterHash);
			if (Mathf.Abs(num) > animatorParameterThreshold)
			{
				if (num > 0f && lastFootstep < 0f)
				{
					OnFootStep();
				}
				else if (num < 0f && lastFootstep > 0f)
				{
					OnFootStep();
				}
				lastFootstep = num;
			}
		}

		public bool DetectIndoorOutdoor()
		{
			if (_puppet == null)
			{
				return false;
			}
			Vector3 playerHeadPosition = GetPlayerHeadPosition();
			float time = Time.time;
			bool flag = Vector3.Distance(playerHeadPosition, lastHeadPosition) > positionChangeThreshold;
			bool flag2 = time - lastRaycastTime > raycastCacheDuration;
			if (raycastCacheValid && !flag && !flag2)
			{
				return cachedRaycastResult;
			}
			Vector3 up = Vector3.up;
			RaycastHit hitInfo;
			bool flag3 = (cachedRaycastResult = (Physics.Raycast(playerHeadPosition, up, out hitInfo, detectionDistance, detectionLayerMask) ? true : false));
			raycastCacheValid = true;
			lastRaycastTime = time;
			lastHeadPosition = playerHeadPosition;
			_isIndoor = flag3;
			return flag3;
		}

		public void OnFootStep()
		{
			if (!(Hub.s == null) && !(Hub.s.pdata.main == null))
			{
				bool flag = DetermineIndoorStatus();
				if (flag != _lastDetectionResult)
				{
					ApplyAudioSettings(flag);
					_lastDetectionResult = flag;
				}
			}
		}

		private bool DetermineIndoorStatus()
		{
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (gamePlayScene != null && gamePlayScene.IsAvatarIndoor)
			{
				return true;
			}
			return DetectIndoorOutdoor();
		}

		public void ApplyAudioSettings(bool isIndoor)
		{
			if (Hub.s?.audioman == null)
			{
				return;
			}
			AudioEnvironmentSettings exposedParameters = (isIndoor ? indoorSettings : outdoorSettings);
			if (interpolationDuration <= 0f)
			{
				Hub.s.audioman.SetExposedParameters(exposedParameters);
				return;
			}
			if (currentInterpolationCoroutine != null)
			{
				StopCoroutine(currentInterpolationCoroutine);
			}
			currentStartSettings = GetCurrentAudioSettings();
			currentTargetSettings = exposedParameters;
			currentInterpolationCoroutine = StartCoroutine(InterpolateAudioSettings());
		}

		private IEnumerator InterpolateAudioSettings()
		{
			if (currentStartSettings == null || currentTargetSettings == null)
			{
				currentInterpolationCoroutine = null;
				yield break;
			}
			if (interpolationDuration <= 0f)
			{
				if (Hub.s?.audioman != null)
				{
					Hub.s.audioman.SetExposedParameters(currentTargetSettings);
				}
				currentInterpolationCoroutine = null;
				yield break;
			}
			float elapsedTime = 0f;
			while (elapsedTime < interpolationDuration)
			{
				float unscaledDeltaTime = Time.unscaledDeltaTime;
				float t = elapsedTime / interpolationDuration;
				float t2 = Mathf.SmoothStep(0f, 1f, t);
				AudioEnvironmentSettings exposedParameters = LerpAudioSettings(currentStartSettings, currentTargetSettings, t2);
				if (Hub.s?.audioman != null)
				{
					Hub.s.audioman.SetExposedParameters(exposedParameters);
					elapsedTime += unscaledDeltaTime;
					yield return null;
					continue;
				}
				currentInterpolationCoroutine = null;
				yield break;
			}
			if (Hub.s?.audioman != null && currentTargetSettings != null)
			{
				Hub.s.audioman.SetExposedParameters(currentTargetSettings);
			}
			currentInterpolationCoroutine = null;
		}

		private Vector3 GetPlayerHeadPosition()
		{
			if (_puppet == null)
			{
				return Vector3.zero;
			}
			return _puppet.transform.position + Vector3.up * 1.6f;
		}

		private AudioEnvironmentSettings GetCurrentAudioSettings()
		{
			AudioEnvironmentSettings audioEnvironmentSettings = indoorSettings;
			AudioEnvironmentSettings audioEnvironmentSettings2 = new AudioEnvironmentSettings
			{
				pcActionGunTailIndoorParamName = audioEnvironmentSettings.pcActionGunTailIndoorParamName,
				pcActionGunTailOutdoorParamName = audioEnvironmentSettings.pcActionGunTailOutdoorParamName,
				itemExplosiveTailIndoorParamName = audioEnvironmentSettings.itemExplosiveTailIndoorParamName,
				itemExplosiveTailOutdoorParamName = audioEnvironmentSettings.itemExplosiveTailOutdoorParamName,
				ambienceIndoorParamName = audioEnvironmentSettings.ambienceIndoorParamName,
				ambienceOutdoorParamName = audioEnvironmentSettings.ambienceOutdoorParamName,
				reverbIndoorParamName = audioEnvironmentSettings.reverbIndoorParamName,
				musicIndoorParamName = audioEnvironmentSettings.musicIndoorParamName,
				musicOutdoorParamName = audioEnvironmentSettings.musicOutdoorParamName
			};
			if (Hub.s?.audioman?.AudioMixer == null)
			{
				return audioEnvironmentSettings2;
			}
			AudioMixer audioMixer = Hub.s.audioman.AudioMixer;
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.pcActionGunTailIndoorParamName, ref audioEnvironmentSettings2.pcActionGunTailIndoorAttenuation);
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.pcActionGunTailOutdoorParamName, ref audioEnvironmentSettings2.pcActionGunTailOutdoorAttenuation);
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.itemExplosiveTailIndoorParamName, ref audioEnvironmentSettings2.itemExplosiveTailIndoorAttenuation);
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.itemExplosiveTailOutdoorParamName, ref audioEnvironmentSettings2.itemExplosiveTailOutdoorAttenuation);
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.ambienceIndoorParamName, ref audioEnvironmentSettings2.ambienceIndoorAttenuation);
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.ambienceOutdoorParamName, ref audioEnvironmentSettings2.ambienceOutdoorAttenuation);
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.reverbIndoorParamName, ref audioEnvironmentSettings2.reverbIndoorAttenuation);
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.musicIndoorParamName, ref audioEnvironmentSettings2.musicIndoorAttenuation);
			TryGetFloatFromMixer(audioMixer, audioEnvironmentSettings2.musicOutdoorParamName, ref audioEnvironmentSettings2.musicOutdoorAttenuation);
			return audioEnvironmentSettings2;
		}

		private bool TryGetFloatFromMixer(AudioMixer mixer, string parameterName, ref float value)
		{
			if (string.IsNullOrWhiteSpace(parameterName))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("[IndoorOutdoorDetector] Parameter name is null or empty.");
				}
				return false;
			}
			float value2;
			bool num = mixer.GetFloat(parameterName, out value2);
			if (num)
			{
				value = value2;
				return num;
			}
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("[IndoorOutdoorDetector] Failed to get parameter '" + parameterName + "' from AudioMixer. Using default value.");
			}
			return num;
		}

		private AudioEnvironmentSettings LerpAudioSettings(AudioEnvironmentSettings from, AudioEnvironmentSettings to, float t)
		{
			if (from == null || to == null)
			{
				return to ?? from ?? new AudioEnvironmentSettings();
			}
			AudioEnvironmentSettings audioEnvironmentSettings = new AudioEnvironmentSettings();
			t = Mathf.Clamp01(t);
			audioEnvironmentSettings.pcActionGunTailIndoorParamName = to.pcActionGunTailIndoorParamName;
			audioEnvironmentSettings.pcActionGunTailOutdoorParamName = to.pcActionGunTailOutdoorParamName;
			audioEnvironmentSettings.itemExplosiveTailIndoorParamName = to.itemExplosiveTailIndoorParamName;
			audioEnvironmentSettings.itemExplosiveTailOutdoorParamName = to.itemExplosiveTailOutdoorParamName;
			audioEnvironmentSettings.ambienceIndoorParamName = to.ambienceIndoorParamName;
			audioEnvironmentSettings.ambienceOutdoorParamName = to.ambienceOutdoorParamName;
			audioEnvironmentSettings.reverbIndoorParamName = to.reverbIndoorParamName;
			audioEnvironmentSettings.musicIndoorParamName = to.musicIndoorParamName;
			audioEnvironmentSettings.musicOutdoorParamName = to.musicOutdoorParamName;
			audioEnvironmentSettings.pcActionGunTailIndoorAttenuation = Mathf.Lerp(from.pcActionGunTailIndoorAttenuation, to.pcActionGunTailIndoorAttenuation, t);
			audioEnvironmentSettings.pcActionGunTailOutdoorAttenuation = Mathf.Lerp(from.pcActionGunTailOutdoorAttenuation, to.pcActionGunTailOutdoorAttenuation, t);
			audioEnvironmentSettings.itemExplosiveTailIndoorAttenuation = Mathf.Lerp(from.itemExplosiveTailIndoorAttenuation, to.itemExplosiveTailIndoorAttenuation, t);
			audioEnvironmentSettings.itemExplosiveTailOutdoorAttenuation = Mathf.Lerp(from.itemExplosiveTailOutdoorAttenuation, to.itemExplosiveTailOutdoorAttenuation, t);
			audioEnvironmentSettings.ambienceIndoorAttenuation = Mathf.Lerp(from.ambienceIndoorAttenuation, to.ambienceIndoorAttenuation, t);
			audioEnvironmentSettings.ambienceOutdoorAttenuation = Mathf.Lerp(from.ambienceOutdoorAttenuation, to.ambienceOutdoorAttenuation, t);
			audioEnvironmentSettings.reverbIndoorAttenuation = Mathf.Lerp(from.reverbIndoorAttenuation, to.reverbIndoorAttenuation, t);
			audioEnvironmentSettings.musicIndoorAttenuation = Mathf.Lerp(from.musicIndoorAttenuation, to.musicIndoorAttenuation, t);
			audioEnvironmentSettings.musicOutdoorAttenuation = Mathf.Lerp(from.musicOutdoorAttenuation, to.musicOutdoorAttenuation, t);
			return audioEnvironmentSettings;
		}

		public void StopCurrentInterpolation()
		{
			if (currentInterpolationCoroutine != null)
			{
				StopCoroutine(currentInterpolationCoroutine);
				currentInterpolationCoroutine = null;
			}
		}
	}
}
