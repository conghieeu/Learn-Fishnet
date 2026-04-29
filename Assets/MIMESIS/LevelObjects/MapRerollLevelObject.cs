using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class MapRerollLevelObject : LeverLevelObject, ITramUpgradeLevelObject
{
	[Serializable]
	private class HighlightInfo
	{
		public Animator animator;

		public string animatorStateName = "First";
	}

	private bool isRerolling;

	private bool isTriggered;

	[SerializeField]
	private string textInCrossHairRerolling = "TRAM_MAP_REROLLING";

	[SerializeField]
	private string textInCrossHairAlreadyRerolled = "TRAM_MAP_REROLLED";

	[SerializeField]
	private string textInCrossHairCanReroll = "CAN_REROLL_TRAM_MAP";

	[SerializeField]
	private string textInCrossHairPullingTramStartLever = "STRING_TRAMUPGRADE_TRAMBOOSTER_CANNOT_USE_BY_STOPLEVER";

	[SerializeField]
	private float mapRerollTramEffectDelaySec = 0.5f;

	[SerializeField]
	private GameObject mapRerollTramEffect;

	[SerializeField]
	private string mapRerollSound = "reroll_tram_sound";

	[SerializeField]
	private string mapRerollScreenSound = "reroll_tram_screen_sound";

	[SerializeField]
	private float mapRerollScreenEffectDelaySec = 1f;

	[SerializeField]
	private Animator mapRerollScreenEffectAnimator;

	[SerializeField]
	private string mapRerollScreenEffectAnimatorStateName = "tram_Reset_Lever_ScreenEffect";

	[SerializeField]
	private string prepareAnim;

	[SerializeField]
	private List<HighlightInfo> highlightInfos;

	private bool isUpgradeActive;

	private Coroutine corOnPullLever;

	private bool isPullingCanceled;

	private int clientCheckedOccupiedActorID;

	[SerializeField]
	private int tramUpgradeID = -1;

	public override LevelObjectClientType LevelObjectType => LevelObjectClientType.MapReroll;

	public bool IsUpgradeActive => isUpgradeActive;

	public int TramUpgradeID => tramUpgradeID;

	public void OnEnable()
	{
		isUpgradeActive = true;
	}

	public void PrepareUpgradeEffect()
	{
		if (animator != null)
		{
			animator.Play(prepareAnim);
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
	}

	public void NotifyTramStartLeverPullingStarted()
	{
		if (isRerolling)
		{
			ctsForTriggerAction?.Cancel();
			ctsForTriggerAction?.Dispose();
			ctsForTriggerAction = null;
			isRerolling = false;
			isTriggered = false;
		}
	}

	public override float GetCrossHairAnimDuration()
	{
		return 2f;
	}

	public override CrosshairType GetCrossHairType(ProtoActor protoActor)
	{
		if (base.State == 1)
		{
			return CrosshairType.AnimatedSample;
		}
		return base.GetCrossHairType(protoActor);
	}

	protected override void Trigger(ProtoActor protoActor, int newState)
	{
		if (ctsForServerRequest != null)
		{
			if (!ctsForServerRequest.IsCancellationRequested)
			{
				ctsForServerRequest.Cancel();
			}
			ctsForServerRequest.Dispose();
		}
		ctsForServerRequest = new CancellationTokenSource();
		_ = base.State;
		clientCheckedOccupiedActorID = protoActor.ActorID;
		ChangeLevelObjectState(levelObjectID, newState, occupy: false, ctsForServerRequest.Token, delegate(int num, UseLevelObjectRes? res)
		{
			if (res == null)
			{
				Logger.RError("LeverLevelObject::TriggerAction response is null");
			}
		});
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int CurrentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, occupiedActorID, prevState, CurrentState);
		if (!HasStateActionTransition(prevState, CurrentState, out StateActionInfo stateActionInfo))
		{
			return;
		}
		switch (CurrentState)
		{
		case 1:
			if (clientCheckedOccupiedActorID != 0)
			{
				isRerolling = true;
				isPullingCanceled = false;
				corOnPullLever = StartCoroutine(CorOnPullLever(clientCheckedOccupiedActorID, stateActionInfo.transitionDurtaion, prevState, CurrentState, stateActionInfo.action));
			}
			break;
		case 0:
			if (clientCheckedOccupiedActorID != 0)
			{
				clientCheckedOccupiedActorID = 0;
				isPullingCanceled = true;
			}
			break;
		}
	}

	private IEnumerator CorOnPullLever(int actorID, float transitionDuration, int prevState, int currentState, string leverAction)
	{
		while (transitionDuration > 0f)
		{
			transitionDuration -= Time.deltaTime;
			if (isPullingCanceled)
			{
				isRerolling = false;
				isTriggered = false;
				yield break;
			}
			if (Hub.s.pdata.main is InTramWaitingScene inTramWaitingScene && inTramWaitingScene.IsPullingTramStartLever())
			{
				ReturnToOffState(actorID);
				isRerolling = false;
				isTriggered = false;
				yield break;
			}
			yield return null;
		}
		if (currentState == 1 && !isPullingCanceled)
		{
			PlayTriggerSound(prevState, currentState);
			AnimateObject(prevState, currentState);
			PullLever(leverAction);
		}
	}

	private void ReturnToOffState(int actorID)
	{
		if (clientCheckedOccupiedActorID == 0 || clientCheckedOccupiedActorID != actorID)
		{
			return;
		}
		ChangeLevelObjectState(levelObjectID, 0, occupy: false, ctsForServerRequest.Token, delegate(int newState, UseLevelObjectRes? res)
		{
			if (res == null || res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError("MapRerollLevelObject::ReturnToOffState : ChangeLevelObjectState Failed");
			}
		});
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		if (protoActor.AmIAvatar())
		{
			ReturnToOffState(protoActor.ActorID);
		}
		return true;
	}

	protected override bool CanPullLever(string leverAction)
	{
		bool flag = base.CanPullLever(leverAction);
		if (Hub.s.pdata.main is InTramWaitingScene inTramWaitingScene)
		{
			if (inTramWaitingScene.IsPullingTramStartLever())
			{
				return false;
			}
			if (flag && !isTriggered)
			{
				return !isRerolling;
			}
			return false;
		}
		return false;
	}

	public override bool PullLever(string leverAction)
	{
		InTramWaitingScene inTramWaitingScene = Hub.s.pdata.main as InTramWaitingScene;
		if (inTramWaitingScene != null)
		{
			isRerolling = true;
			inTramWaitingScene.RerollMap(OnRerollMapSuccess);
			return true;
		}
		return false;
	}

	private void OnRerollMapSuccess(bool success)
	{
		if (success)
		{
			isRerolling = false;
			isTriggered = true;
		}
		else
		{
			isRerolling = false;
		}
	}

	public void UpdateDungeonCandidates(int newDungeonMasterID1, int newDungeonMasterID2)
	{
		if (base.State == 1)
		{
			isRerolling = false;
			isTriggered = true;
			StartCoroutine(CorPlayMapRerollTramEffect());
			StartCoroutine(CorPlayMapRerollScreenEffect(newDungeonMasterID1, newDungeonMasterID2));
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			Logger.RError("MapRerollLevelObject::GetSimpleText : Hub or Main is null");
			return "";
		}
		if (Hub.s.pdata.main is InTramWaitingScene inTramWaitingScene)
		{
			if (!isTriggered && inTramWaitingScene.IsPullingTramStartLever())
			{
				return Hub.GetL10NText(textInCrossHairPullingTramStartLever);
			}
			if (currentTransition != null)
			{
				return Hub.GetL10NText(textInCrossHairRerolling);
			}
			if (isTriggered)
			{
				return Hub.GetL10NText(textInCrossHairAlreadyRerolled);
			}
			return Hub.GetL10NText(textInCrossHairCanReroll);
		}
		return base.GetSimpleText(protoActor);
	}

	private IEnumerator CorPlayMapRerollTramEffect()
	{
		yield return new WaitForSeconds(mapRerollTramEffectDelaySec);
		if (!(mapRerollTramEffect != null))
		{
			yield break;
		}
		mapRerollTramEffect.SetActive(value: true);
		ParticleSystem[] componentsInChildren = mapRerollTramEffect.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (particleSystem != null)
			{
				particleSystem.Play();
			}
		}
		if (Hub.s != null && Hub.s.audioman != null)
		{
			Hub.s.audioman.PlaySfx(mapRerollSound, mapRerollTramEffect.transform);
		}
	}

	private IEnumerator CorPlayMapRerollScreenEffect(int newDungeonMasterID1, int newDungeonMasterID2)
	{
		yield return new WaitForSeconds(mapRerollScreenEffectDelaySec);
		if (mapRerollScreenEffectAnimator != null)
		{
			mapRerollScreenEffectAnimator.Play(mapRerollScreenEffectAnimatorStateName);
			if (Hub.s != null && Hub.s.audioman != null)
			{
				Hub.s.audioman.PlaySfx(mapRerollScreenSound, mapRerollScreenEffectAnimator.transform);
			}
		}
		InTramWaitingScene inTramWaitingScene = Hub.s.pdata.main as InTramWaitingScene;
		if (inTramWaitingScene != null)
		{
			inTramWaitingScene.UpdateDungeonCandidates(new List<int> { newDungeonMasterID1, newDungeonMasterID2 });
		}
	}
}
