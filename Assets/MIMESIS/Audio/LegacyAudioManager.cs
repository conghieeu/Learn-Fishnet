using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LegacyAudioManager : MonoBehaviour
{
	public enum eAudioMixState
	{
		Normal = 0,
		Ringing = 1
	}

	[SerializeField]
	[Tooltip("오디오를 생성할 때 사용할 프리팹입니다.")]
	private AudioSource? voiceAudioSourcePrefab;

	[SerializeField]
	private AudioSource? SFXAudioSourcePrefab;

	[SerializeField]
	private AudioSource? ringingAudioSourcePrefab;

	private Dictionary<string, float> timestamp = new Dictionary<string, float>();

	private Dictionary<string, MMAudioClipTable.Row> cache = new Dictionary<string, MMAudioClipTable.Row>();

	public AudioMixer mixer;

	private void Start()
	{
		Logger.RLog("[AwakeLogs] LegacyAudioManager.Start ->");
		if (mixer == null)
		{
			Logger.RError("AudioMixer is not set");
		}
		else
		{
			SetMasterVolume(PlayerPrefs.GetFloat("masterVolume", Hub.s.gameConfig.gameSetting.defaultMasterVolume));
		}
		Logger.RLog("[AwakeLogs] LegacyAudioManager.Start <-");
	}

	public bool Play(string clipId, AudioSource? source)
	{
		if (!TryFindRow(clipId, out MMAudioClipTable.Row row))
		{
			return false;
		}
		if (!TryCheckCooltime(row, source))
		{
			return false;
		}
		row.PrepareToPlay(source);
		source.Play();
		return true;
	}

	public bool PlaySFXOneShotAtPoint(string clipId, Vector3 position)
	{
		if (SFXAudioSourcePrefab == null)
		{
			Logger.RError("Audio source prefab is not set");
			return false;
		}
		if (!TryFindRow(clipId, out MMAudioClipTable.Row row))
		{
			return false;
		}
		AudioSource audioSource = Object.Instantiate(SFXAudioSourcePrefab, position, Quaternion.identity);
		row.PrepareToPlay(audioSource);
		ApplyLifeTime(audioSource);
		audioSource.Play();
		return true;
	}

	public bool PlayVoiceOneShotAtPoint(AudioClip clip, Vector3 position, bool useEchoEffect = false)
	{
		if (voiceAudioSourcePrefab == null)
		{
			Logger.RError("Audio source prefab is not set");
			return false;
		}
		AudioSource audioSource = Object.Instantiate(voiceAudioSourcePrefab, position, Quaternion.identity);
		audioSource.clip = clip;
		audioSource.loop = false;
		audioSource.volume = 1f;
		if (useEchoEffect)
		{
			AudioEchoFilter audioEchoFilter = audioSource.gameObject.AddComponent<AudioEchoFilter>();
			if (audioEchoFilter != null)
			{
				audioEchoFilter.enabled = true;
			}
		}
		ApplyLifeTime(audioSource);
		audioSource.Play();
		return true;
	}

	public AudioSource? PlaySFXOneShotOnParent(string clipId, Transform parent)
	{
		if (SFXAudioSourcePrefab == null)
		{
			Logger.RError("Audio source prefab is not set");
			return null;
		}
		if (!TryFindRow(clipId, out MMAudioClipTable.Row row))
		{
			return null;
		}
		AudioSource audioSource = Object.Instantiate(SFXAudioSourcePrefab, parent);
		row.PrepareToPlay(audioSource);
		ApplyLifeTime(audioSource);
		audioSource.Play();
		return audioSource;
	}

	public AudioSource? PlayRingingOnParent(string clipId, Transform parent)
	{
		if (ringingAudioSourcePrefab == null)
		{
			Logger.RError("Ringing audio source prefab is not set");
			return null;
		}
		if (!TryFindRow(clipId, out MMAudioClipTable.Row row))
		{
			return null;
		}
		AudioSource audioSource = Object.Instantiate(ringingAudioSourcePrefab, parent);
		row.PrepareToPlay(audioSource);
		audioSource.loop = true;
		audioSource.Play();
		return audioSource;
	}

	public AudioSource? PlaySFXLoopOnParent(string clipId, Transform parent)
	{
		if (SFXAudioSourcePrefab == null)
		{
			Logger.RError("Audio source prefab is not set");
			return null;
		}
		if (!TryFindRow(clipId, out MMAudioClipTable.Row row))
		{
			return null;
		}
		AudioSource audioSource = Object.Instantiate(SFXAudioSourcePrefab, parent);
		row.PrepareToPlay(audioSource);
		audioSource.Play();
		return audioSource;
	}

	private bool TryFindRow(string clipId, out MMAudioClipTable.Row? row)
	{
		if (!cache.TryGetValue(clipId, out row))
		{
			row = Hub.s.tableman.audioClip.FindRow2(clipId);
			if (row != null)
			{
				cache.Add(clipId, row);
			}
		}
		if (row == null)
		{
			Logger.RWarn("AudioClip not found: " + clipId, sendToLogServer: false, useConsoleOut: true, "legacyaudio");
			return false;
		}
		return true;
	}

	private bool TryCheckCooltime(MMAudioClipTable.Row row, AudioSource source)
	{
		string key = source.name + row.id;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (timestamp.TryGetValue(key, out var value) && realtimeSinceStartup - value < row.cooltime)
		{
			return false;
		}
		timestamp[key] = realtimeSinceStartup;
		return true;
	}

	private static void ApplyLifeTime(AudioSource source)
	{
		float length = source.clip.length;
		length *= ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale);
		Object.Destroy(source.gameObject, length);
	}

	public void SetMasterVolume(float value)
	{
		float value2 = ((!(value > 0.0001f)) ? (-80f) : (Mathf.Log10(value) * 20f));
		mixer.SetFloat("ExcludeVoice.Attenuation", value2);
	}

	public void SetVoiceVolume(float value)
	{
		float value2 = ((!(value > 0.0001f)) ? (-80f) : (Mathf.Log10(value) * 20f));
		mixer.SetFloat("Voice.Attenuation", value2);
	}
}
