using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Mimic.Actors;
using UnityEngine;

public class PartyButtonLevelObject : SimpleSwitchLevelObject, ITramUpgradeLevelObject
{
	[Serializable]
	private class HighlightInfo
	{
		public Animator animator;

		public string animatorStateName = "First";
	}

	[SerializeField]
	private string partyMasterAudioKey = string.Empty;

	[SerializeField]
	private Transform? partyAudioTransform;

	[SerializeField]
	public Animator[] partyAnimators;

	[SerializeField]
	private List<HighlightInfo> highlightInfos;

	private PlaySoundResult? partyPlaySoundResult;

	private bool isUpgradeActive;

	[SerializeField]
	private int tramUpgradeID = -1;

	public override LevelObjectClientType LevelObjectType => LevelObjectClientType.PartyButton;

	public bool IsUpgradeActive => isUpgradeActive;

	public int TramUpgradeID => tramUpgradeID;

	public void OnEnable()
	{
		isUpgradeActive = true;
	}

	public void PrepareUpgradeEffect()
	{
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
	}

	protected override void OnSwitchInitialized()
	{
	}

	protected override void OnSwitchStateChanged(bool isOn)
	{
		SetPartyState(isOn);
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		int newState = ((!base.IsOn) ? 1 : 0);
		if (IsTriggerable(protoActor, newState))
		{
			Trigger(protoActor, newState);
			return true;
		}
		return false;
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		return true;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (base.IsOn)
		{
			return Hub.GetL10NText("STRING_TRAMUPGRADE_DISCOBALL_BUTTON_OFF");
		}
		return Hub.GetL10NText("STRING_TRAMUPGRADE_DISCOBALL_BUTTON_ON");
	}

	public void SetPartyState(bool isOn)
	{
		StopPartySound();
		if (isOn)
		{
			PlayPartySound();
		}
		SetPartyAnimations(isOn);
	}

	private void PlayPartySound()
	{
		if (!string.IsNullOrWhiteSpace(partyMasterAudioKey) && Hub.s != null && Hub.s.audioman != null)
		{
			Transform parent = ((partyAudioTransform != null) ? partyAudioTransform : base.transform);
			partyPlaySoundResult = Hub.s.audioman.PlaySfxTransform(partyMasterAudioKey, parent);
		}
	}

	private void StopPartySound()
	{
		if (partyPlaySoundResult != null)
		{
			if (partyPlaySoundResult.ActingVariation != null)
			{
				partyPlaySoundResult.ActingVariation.Stop();
			}
			partyPlaySoundResult = null;
		}
	}

	private void SetPartyAnimations(bool isOn)
	{
		if (base.animator != null)
		{
			base.animator.SetBool("IsOn", isOn);
		}
		if (partyAnimators == null)
		{
			return;
		}
		Animator[] array = partyAnimators;
		foreach (Animator animator in array)
		{
			if (animator != null)
			{
				animator.SetBool("IsOn", isOn);
			}
		}
	}
}
