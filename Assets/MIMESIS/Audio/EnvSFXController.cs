using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class EnvSFXController : MonoBehaviour
{
	public enum eEnvSFXClips
	{
		TRAM_LOOP = 0,
		TRAM_STOP = 1
	}

	[SerializeField]
	private AudioClip[] envSFXClips;

	private AudioSource[] audioSources;

	private AudioSource lastPlayingAudioSource;

	[SerializeField]
	private float globalVolume = 1f;

	[SerializeField]
	private AudioMixerGroup mixerGroup;

	private void Awake()
	{
		for (int i = 0; i < envSFXClips.Length; i++)
		{
			AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.loop = true;
			audioSource.spatialBlend = 0f;
			audioSource.playOnAwake = false;
			audioSource.volume = globalVolume;
			audioSource.outputAudioMixerGroup = mixerGroup;
		}
		audioSources = GetComponents<AudioSource>();
		for (int j = 0; j < envSFXClips.Length; j++)
		{
			audioSources[j].clip = envSFXClips[j];
		}
		lastPlayingAudioSource = audioSources[0];
	}

	private IEnumerator ChangeEnvSFXCorutine(eEnvSFXClips index, float fadeTime)
	{
		float startVolume = lastPlayingAudioSource.volume;
		float targetVolume = 0f;
		float elapsedTime = 0f;
		if (lastPlayingAudioSource.isPlaying)
		{
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / fadeTime;
				lastPlayingAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
				yield return null;
			}
			lastPlayingAudioSource.Pause();
			lastPlayingAudioSource.Stop();
		}
		audioSources[(int)index].Play();
		startVolume = 0f;
		targetVolume = globalVolume;
		elapsedTime = 0f;
		while (elapsedTime < fadeTime)
		{
			elapsedTime += Time.deltaTime;
			float t2 = elapsedTime / fadeTime;
			audioSources[(int)index].volume = Mathf.Lerp(startVolume, targetVolume, t2);
			yield return null;
		}
		audioSources[(int)index].volume = targetVolume;
		lastPlayingAudioSource = audioSources[(int)index];
	}

	public void ChangeEnvSFX(eEnvSFXClips index, float fadeTime = 1f)
	{
		if (!(lastPlayingAudioSource == audioSources[(int)index]) || !lastPlayingAudioSource.isPlaying)
		{
			StartCoroutine(ChangeEnvSFXCorutine(index, fadeTime));
		}
	}

	public void AddNoLoopSFX(eEnvSFXClips index)
	{
		audioSources[(int)index].loop = false;
		audioSources[(int)index].volume = globalVolume;
		audioSources[(int)index].Play();
	}

	public void AddLoopSFX(eEnvSFXClips index)
	{
		audioSources[(int)index].loop = true;
		audioSources[(int)index].volume = globalVolume;
		audioSources[(int)index].Play();
	}
}
