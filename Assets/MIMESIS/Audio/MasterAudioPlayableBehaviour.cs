using System;
using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class MasterAudioPlayableBehaviour : PlayableBehaviour
{
	public string soundGroupName;

	public float volume = 1f;

	public bool enableFade = true;

	public bool playOnce;

	public Transform playSfxTransform;

	public bool use2DSound;

	public bool hasPlayed;

	public PlaySoundResult soundResult;

	public float currentWeight;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (hasPlayed || string.IsNullOrEmpty(soundGroupName) || Hub.s == null || Hub.s.audioman == null)
		{
			return;
		}
		if (use2DSound)
		{
			soundResult = Hub.s.audioman.PlaySfxResult(soundGroupName);
		}
		else
		{
			if (!(playSfxTransform != null))
			{
				return;
			}
			soundResult = Hub.s.audioman.PlaySfxTransform(soundGroupName, playSfxTransform);
		}
		if (soundResult?.ActingVariation != null)
		{
			float volumePercentage = ((enableFade && !playOnce) ? 0f : volume);
			soundResult.ActingVariation.AdjustVolume(volumePercentage);
		}
		hasPlayed = true;
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (!use2DSound && playSfxTransform == null && playerData is GameObject gameObject)
		{
			playSfxTransform = gameObject.transform;
			if (!hasPlayed && !string.IsNullOrEmpty(soundGroupName) && Hub.s?.audioman != null)
			{
				soundResult = Hub.s.audioman.PlaySfxTransform(soundGroupName, playSfxTransform);
				if (soundResult?.ActingVariation != null)
				{
					float volumePercentage = ((enableFade && !playOnce) ? 0f : volume);
					soundResult.ActingVariation.AdjustVolume(volumePercentage);
				}
				hasPlayed = true;
			}
		}
		if (!playOnce && hasPlayed && soundResult?.ActingVariation != null && enableFade)
		{
			float weight = info.weight;
			if (Mathf.Abs(weight - currentWeight) > 0.001f)
			{
				currentWeight = weight;
				float volumePercentage2 = volume * currentWeight;
				soundResult.ActingVariation.AdjustVolume(volumePercentage2);
			}
		}
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if (!playOnce && hasPlayed && soundResult != null)
		{
			if (soundResult.ActingVariation != null)
			{
				soundResult.ActingVariation.Stop();
			}
			hasPlayed = false;
			soundResult = null;
			currentWeight = 0f;
		}
	}

	public override void OnGraphStop(Playable playable)
	{
		if (!playOnce && hasPlayed && soundResult != null)
		{
			if (soundResult.ActingVariation != null)
			{
				soundResult.ActingVariation.Stop();
			}
			hasPlayed = false;
			soundResult = null;
			currentWeight = 0f;
		}
	}
}
