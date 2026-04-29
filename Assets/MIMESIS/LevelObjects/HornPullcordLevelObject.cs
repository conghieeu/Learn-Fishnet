using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Mimic.Actors;
using MoreMountains.Feedbacks;
using UnityEngine;

public class HornPullcordLevelObject : SimpleSwitchLevelObject, ITramUpgradeLevelObject
{
	[Serializable]
	private class HighlightInfo
	{
		public Animator animator;

		public string animatorStateName = "First";
	}

	[Header("Horn Speakers")]
	[SerializeField]
	[Tooltip("마멋 스피커 연출을 위한 MMF_Player 객체들")]
	private MMF_Player[] speakerFeedbacks;

	[Header("L10N Texts")]
	[SerializeField]
	[Tooltip("사용 가능한 경우의 텍스트")]
	private string usableStateTextID = "STRING_TRAMUPGRADE_TRAMHORN_USEABLE";

	[SerializeField]
	[Tooltip("자신이나 다른 누군가 사용 중인 경우의 텍스트")]
	private string usingStateTextID = "STRING_TRAMUPGRADE_TRAMHORN_USING";

	[SerializeField]
	[Tooltip("업그레이드 연출이 진행 중인 경우의 텍스트")]
	private string upgradingStateTextID = "STRING_TRAMUPGRADE_UPGRADING";

	[Header("Master Audio")]
	[SerializeField]
	private string hornMasterAudioKey = string.Empty;

	[SerializeField]
	private Transform? hornAudioTransform;

	[Header("Upgrade Effects")]
	[SerializeField]
	[Tooltip("업그레이드 효과가 나오고 나서 지정된 시간동안 상호작용이 불가능하게 막는다.")]
	private float upgradeEffectDuration = 4f;

	[SerializeField]
	[Tooltip("업그레이드 효과가 나오기 전에 미리 당김줄이 접혀 들어가 있도록 유지하는 애니메이션")]
	private string foldedAnimState = "Folded";

	[SerializeField]
	private List<HighlightInfo> highlightInfos;

	private PlaySoundResult? hornPlaySoundResult;

	private bool isUpgradeActive;

	private bool isTriggered;

	private float interactableTime;

	[SerializeField]
	private int tramUpgradeID = -1;

	public override LevelObjectClientType LevelObjectType => LevelObjectClientType.HornPullcord;

	public bool IsUpgradeActive => isUpgradeActive;

	public int TramUpgradeID => tramUpgradeID;

	public void OnEnable()
	{
		isUpgradeActive = true;
	}

	protected override void OnSwitchInitialized()
	{
	}

	protected override void OnSwitchStateChanged(bool isOn)
	{
		if (isOn)
		{
			PullCord();
		}
		else
		{
			ReleaseCord();
		}
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (IsTriggerable(protoActor, 1))
		{
			Trigger(protoActor, 1);
			isTriggered = true;
			return true;
		}
		return false;
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		if (isTriggered)
		{
			Trigger(protoActor, 0);
			isTriggered = false;
			return true;
		}
		return false;
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (base.IsTriggerable(protoActor, newState))
		{
			return Time.time >= interactableTime;
		}
		return false;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Time.time < interactableTime)
		{
			return Hub.GetL10NText(upgradingStateTextID);
		}
		if (!base.IsOn)
		{
			return Hub.GetL10NText(usableStateTextID);
		}
		return Hub.GetL10NText(usingStateTextID);
	}

	public void PullCord()
	{
		if (animator != null)
		{
			animator.SetBool("IsOn", value: true);
		}
		if (speakerFeedbacks != null)
		{
			MMF_Player[] array = speakerFeedbacks;
			foreach (MMF_Player mMF_Player in array)
			{
				if (mMF_Player != null)
				{
					mMF_Player.PlayFeedbacks();
				}
			}
		}
		FadeOutHornSound();
		PlayHornSound();
	}

	public void ReleaseCord()
	{
		if (animator != null)
		{
			animator.SetBool("IsOn", value: false);
		}
		if (speakerFeedbacks != null)
		{
			MMF_Player[] array = speakerFeedbacks;
			foreach (MMF_Player mMF_Player in array)
			{
				if (mMF_Player != null)
				{
					mMF_Player.StopFeedbacks();
				}
			}
		}
		FadeOutHornSound();
	}

	private void PlayHornSound()
	{
		if (!string.IsNullOrWhiteSpace(hornMasterAudioKey) && Hub.s != null && Hub.s.audioman != null)
		{
			Transform parent = ((hornAudioTransform != null) ? hornAudioTransform : base.transform);
			hornPlaySoundResult = Hub.s.audioman.PlaySfxTransform(hornMasterAudioKey, parent);
		}
	}

	private void FadeOutHornSound()
	{
		if (hornPlaySoundResult != null && hornPlaySoundResult.ActingVariation != null)
		{
			hornPlaySoundResult.ActingVariation.FadeOutNowAndStop();
		}
	}

	public void PrepareUpgradeEffect()
	{
		if (animator != null)
		{
			animator.Play(foldedAnimState);
		}
	}

	public void PlayUpgradeEffect()
	{
		foreach (HighlightInfo highlightInfo in highlightInfos)
		{
			if (highlightInfo.animator != null)
			{
				highlightInfo.animator.Play(highlightInfo.animatorStateName);
			}
		}
		interactableTime = Time.time + upgradeEffectDuration;
	}
}
