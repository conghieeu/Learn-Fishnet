using DG.Tweening;
using UnityEngine;

public class UIScreenEffectBase : UIPrefabScript
{
	[SerializeField]
	public DOTweenAnimation ScreenEffectAnimation;

	[Header("HACKS")]
	[SerializeField]
	[Tooltip("true인 경우 모든 자식 ParticleSystem의 duration 최대값 만큼 파괴를 지연시키고, ParticleSystem이 loop인 경우 loop도 꺼준다.")]
	public bool useDelayedDestroyForChildParticles;

	protected bool isPlaying;

	public void PlayScreenEffect(float durationSec)
	{
		if (ScreenEffectAnimation != null)
		{
			ScreenEffectAnimation.tween.SetLoops((int)(durationSec / ScreenEffectAnimation.duration) + 1);
			ScreenEffectAnimation.loopType = LoopType.Yoyo;
			ScreenEffectAnimation.DOPlay();
			isPlaying = true;
		}
	}

	public void PlayLoopingScreenEffect()
	{
		if (ScreenEffectAnimation != null)
		{
			ScreenEffectAnimation.tween.SetLoops(-1);
			ScreenEffectAnimation.loopType = LoopType.Yoyo;
			ScreenEffectAnimation.DOPlay();
			isPlaying = true;
		}
	}

	public void StopScreenEffect()
	{
		if (ScreenEffectAnimation != null)
		{
			ScreenEffectAnimation.DOKill();
		}
		isPlaying = false;
		if (useDelayedDestroyForChildParticles)
		{
			float maxDurationForChildParticles = GetMaxDurationForChildParticles();
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				ParticleSystem.MainModule main = particleSystem.main;
				if (main.loop)
				{
					main.loop = false;
					particleSystem.Pause();
					particleSystem.Play();
				}
			}
			Object.Destroy(base.gameObject, maxDurationForChildParticles);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private float GetMaxDurationForChildParticles()
	{
		float num = 0f;
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			float duration = componentsInChildren[i].main.duration;
			if (duration > num)
			{
				num = duration;
			}
		}
		return num;
	}
}
