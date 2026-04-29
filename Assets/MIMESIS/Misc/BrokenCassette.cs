using System;
using DarkTonic.MasterAudio;
using UnityEngine;

public class BrokenCassette : SocketAttachable, ICycleRandomableItem
{
	[SerializeField]
	private string[] sfxIds;

	private PlaySoundResult? _sfxResult;

	private bool isAttached;

	private System.Random syncRandom;

	private int syncRandomSeed;

	public override void OnAttachToSocket()
	{
		isAttached = true;
		Reset();
	}

	public override void OnDetachFromSocket()
	{
		isAttached = false;
		Reset();
	}

	private void OnDisable()
	{
		StopAudio();
	}

	public void OnCycleRandomEvent(int randomSeed)
	{
		if (isAttached && sfxIds.Length != 0)
		{
			StopAudio();
			int audioClipIndex = GetRandom(randomSeed).Next(0, sfxIds.Length);
			PlayAudioClip(audioClipIndex);
		}
	}

	private void PlayAudioClip(int audioClipIndex)
	{
		_sfxResult = Hub.s.audioman.PlaySfxTransform(sfxIds[audioClipIndex], base.transform);
	}

	private void StopAudio()
	{
		if (_sfxResult != null)
		{
			if (_sfxResult.ActingVariation != null)
			{
				_sfxResult.ActingVariation.Stop();
			}
			_sfxResult = null;
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

	private void Reset()
	{
		ResetRandom();
		StopAudio();
	}
}
