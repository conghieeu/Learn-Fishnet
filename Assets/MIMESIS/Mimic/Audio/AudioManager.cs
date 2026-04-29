using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.Audio;

namespace Mimic.Audio
{
	public class AudioManager : MonoBehaviour
	{
		[Header("Audio Mixer")]
		[SerializeField]
		[Tooltip("오디오 믹서 참조 (Exposed Parameters 제어용)")]
		private AudioMixer? audioMixer;

		public AudioMixer? AudioMixer => audioMixer;

		public void PlaySfx(string? sfxId)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string variationName))
			{
				string sType = groupName;
				string variationName2 = variationName;
				MasterAudio.PlaySoundAndForget(sType, 1f, null, 0f, variationName2);
			}
		}

		public void PlaySfx(string? sfxId, Transform parent)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string variationName))
			{
				string sType = groupName;
				string variationName2 = variationName;
				MasterAudio.PlaySound3DFollowTransformAndForget(sType, parent, 1f, null, 0f, variationName2);
			}
		}

		public PlaySoundResult? PlaySfxTransform(string? sfxId, Transform parent)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string variationName))
			{
				string sType = groupName;
				string variationName2 = variationName;
				return MasterAudio.PlaySound3DFollowTransform(sType, parent, 1f, null, 0f, variationName2);
			}
			return null;
		}

		public PlaySoundResult? PlaySfxResult(string? sfxId)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string variationName))
			{
				string sType = groupName;
				string variationName2 = variationName;
				return MasterAudio.PlaySound(sType, 1f, null, 0f, variationName2);
			}
			return null;
		}

		public PlaySoundResult? PlaySfxAtPosition(string? sfxId, Vector3 position)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string variationName))
			{
				string sType = groupName;
				string variationName2 = variationName;
				return MasterAudio.PlaySound3DAtVector3(sType, position, 1f, null, 0f, variationName2);
			}
			return null;
		}

		public void PlaySfx(string? sfxId, Vector3 position)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string variationName))
			{
				string sType = groupName;
				string variationName2 = variationName;
				MasterAudio.PlaySound3DAtVector3AndForget(sType, position, 1f, null, 0f, variationName2);
			}
		}

		public void StopSfx(string? sfxId)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string variationName))
			{
				if (string.IsNullOrWhiteSpace(variationName))
				{
					Logger.RWarn("Variation name will be ignored. Use group name only for stopping SFX.", sendToLogServer: false, useConsoleOut: true, "audio");
				}
				MasterAudio.StopAllOfSound(groupName);
			}
		}

		public PlaySoundResult? PlaySfxAtTransform(string? sfxId, Transform parent)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string variationName))
			{
				string sType = groupName;
				string variationName2 = variationName;
				return MasterAudio.PlaySound3DAtTransform(sType, parent, 1f, null, 0f, variationName2);
			}
			return null;
		}

		public void StopSfxTransform(Transform parent)
		{
			MasterAudio.StopAllSoundsOfTransform(parent);
		}

		public void StopSfxTransform(string? sfxId, Transform parent)
		{
			if (TryParseSfxId(sfxId, out string groupName, out string _))
			{
				MasterAudio.StopSoundGroupOfTransform(parent, groupName);
			}
		}

		public void MuteBus(string busName)
		{
			MasterAudio.MuteBus(busName);
		}

		public void UnmuteBus(string busName)
		{
			MasterAudio.UnmuteBus(busName);
		}

		public void SetMasterVolume(float value)
		{
			MasterAudio.MasterVolumeLevel = value;
		}

		public void SetExposedParameter(string parameterName, float value)
		{
			if (string.IsNullOrWhiteSpace(parameterName))
			{
				Logger.RWarn("Parameter name is null or empty.", sendToLogServer: false, useConsoleOut: true, "audio");
			}
			else if (audioMixer == null)
			{
				Logger.RWarn("AudioMixer is not assigned. Cannot set exposed parameter.", sendToLogServer: false, useConsoleOut: true, "audio");
			}
			else if (!audioMixer.SetFloat(parameterName, value))
			{
				Logger.RWarn("Failed to set exposed parameter '" + parameterName + "'. Parameter may not exist or is not exposed in AudioMixer.", sendToLogServer: false, useConsoleOut: true, "audio");
			}
		}

		public void SetExposedParameters(AudioEnvironmentSettings settings)
		{
			SetExposedParameter(settings.pcActionGunTailIndoorParamName, settings.pcActionGunTailIndoorAttenuation);
			SetExposedParameter(settings.pcActionGunTailOutdoorParamName, settings.pcActionGunTailOutdoorAttenuation);
			SetExposedParameter(settings.ambienceIndoorParamName, settings.ambienceIndoorAttenuation);
			SetExposedParameter(settings.ambienceOutdoorParamName, settings.ambienceOutdoorAttenuation);
			SetExposedParameter(settings.reverbIndoorParamName, settings.reverbIndoorAttenuation);
			SetExposedParameter(settings.itemExplosiveTailIndoorParamName, settings.itemExplosiveTailIndoorAttenuation);
			SetExposedParameter(settings.itemExplosiveTailOutdoorParamName, settings.itemExplosiveTailOutdoorAttenuation);
			SetExposedParameter(settings.musicIndoorParamName, settings.musicIndoorAttenuation);
			SetExposedParameter(settings.musicOutdoorParamName, settings.musicOutdoorAttenuation);
		}

		private static bool TryParseSfxId(string sfxId, out string? groupName, out string? variationName)
		{
			groupName = null;
			variationName = null;
			if (string.IsNullOrWhiteSpace(sfxId))
			{
				Logger.RWarn("SFX ID is null or empty: " + sfxId, sendToLogServer: false, useConsoleOut: true, "audio");
				return false;
			}
			string[] array = sfxId.Split('.');
			if (array.Length == 2)
			{
				groupName = array[0];
				variationName = array[1];
				return true;
			}
			if (array.Length == 1)
			{
				groupName = array[0];
				variationName = null;
				return true;
			}
			Logger.RWarn("Invalid SFX ID format: " + sfxId + ". Expected format: 'SoundGroupName.SoundVariationName' or 'SoundGroupName'.", sendToLogServer: false, useConsoleOut: true, "audio");
			return false;
		}
	}
}
