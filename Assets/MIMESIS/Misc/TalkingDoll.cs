using System;
using UnityEngine;

public class TalkingDoll : SocketAttachable, ICycleRandomableItem
{
	[SerializeField]
	private AudioSource audioSource;

	private System.Random syncRandom;

	private int syncRandomSeed;

	public override void OnAttachToSocket()
	{
		ResetRandom();
	}

	public override void OnDetachFromSocket()
	{
		ResetRandom();
	}

	public void OnCycleRandomEvent(int randomSeed)
	{
		if (!(audioSource == null))
		{
			System.Random random = GetRandom(randomSeed);
			AudioClip speechAudioClipFromSpeechEventArchiveRandom = Hub.s.voiceman.GetSpeechAudioClipFromSpeechEventArchiveRandom(random);
			if (speechAudioClipFromSpeechEventArchiveRandom != null)
			{
				SetEnableVoiceAudioEchoFilter(Hub.s.cameraman.IsSpectatorMode);
				audioSource.clip = speechAudioClipFromSpeechEventArchiveRandom;
				audioSource.Play();
			}
		}
	}

	private void SetEnableVoiceAudioEchoFilter(bool enable)
	{
		if (audioSource != null && audioSource.TryGetComponent<AudioEchoFilter>(out var component))
		{
			component.enabled = enable;
		}
	}

	private System.Random GetRandom(int randomSeed)
	{
		if (syncRandom == null || syncRandomSeed != randomSeed)
		{
			syncRandom = new System.Random(randomSeed);
			syncRandomSeed = randomSeed;
		}
		return syncRandom;
	}

	private void ResetRandom()
	{
		if (syncRandom != null)
		{
			syncRandom = null;
		}
	}
}
