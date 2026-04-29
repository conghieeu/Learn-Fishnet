using System;
using System.Collections;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class InTramWaitingStartLeverLevelObject : TramLeverLevelObject
{
	private Coroutine corOnPullLever;

	private bool isPullingCanceled;

	private int clientCheckedOccupiedActorID;

	private void OnEnable()
	{
		OnTransitionStarted = (Action<int, int, string>)Delegate.Combine(OnTransitionStarted, new Action<int, int, string>(OnTransitionStartedCallback));
		OnTransitionCanceled = (Action<int, int, string>)Delegate.Combine(OnTransitionCanceled, new Action<int, int, string>(OnTransitionCanceledCallback));
	}

	private void OnDisable()
	{
		if (corOnPullLever != null)
		{
			StopCoroutine(corOnPullLever);
			corOnPullLever = null;
		}
		OnTransitionStarted = (Action<int, int, string>)Delegate.Remove(OnTransitionStarted, new Action<int, int, string>(OnTransitionStartedCallback));
		OnTransitionCanceled = (Action<int, int, string>)Delegate.Remove(OnTransitionCanceled, new Action<int, int, string>(OnTransitionCanceledCallback));
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

	private void OnTransitionStartedCallback(int prevState, int newState, string action)
	{
		if (prevState == 0 && newState == 1)
		{
			InTramWaitingScene inTramWaitingScene = Hub.s.pdata.main as InTramWaitingScene;
			if (inTramWaitingScene != null)
			{
				inTramWaitingScene.SetPullingTramStartLever(pulling: true);
			}
		}
	}

	private void OnTransitionCanceledCallback(int prevState, int newState, string action)
	{
		if (newState == 0)
		{
			InTramWaitingScene inTramWaitingScene = Hub.s.pdata.main as InTramWaitingScene;
			if (inTramWaitingScene != null)
			{
				inTramWaitingScene.SetPullingTramStartLever(pulling: false);
			}
		}
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (base.State != newState && Time.time - lastTransitionTime >= minTransitionInterval)
		{
			return currentTransition == null;
		}
		return false;
	}

	public override bool PullLever(string leverAction)
	{
		if (!(Hub.s.pdata.main is InTramWaitingScene inTramWaitingScene))
		{
			return false;
		}
		inTramWaitingScene.TriggerHostStartGame();
		return true;
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
		PlayTriggerSound(prevState, CurrentState);
		AnimateObject(prevState, CurrentState);
		switch (CurrentState)
		{
		case 1:
			if (clientCheckedOccupiedActorID != 0)
			{
				isPullingCanceled = false;
				corOnPullLever = StartCoroutine(CorOnPullLever(stateActionInfo.transitionDurtaion, CurrentState, stateActionInfo.action));
			}
			OnTransitionStarted?.Invoke(prevState, CurrentState, stateActionInfo.action);
			break;
		case 0:
			if (clientCheckedOccupiedActorID != 0)
			{
				clientCheckedOccupiedActorID = 0;
				isPullingCanceled = true;
			}
			OnTransitionCanceled?.Invoke(prevState, CurrentState, stateActionInfo.action);
			break;
		}
	}

	private IEnumerator CorOnPullLever(float transitionDuration, int CurrentState, string leverAction)
	{
		while (transitionDuration > 0f)
		{
			transitionDuration -= Time.deltaTime;
			if (isPullingCanceled)
			{
				break;
			}
			yield return null;
		}
		if (CurrentState == 1 && !isPullingCanceled)
		{
			PullLever(leverAction);
		}
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		if (protoActor.AmIAvatar() && clientCheckedOccupiedActorID != 0 && clientCheckedOccupiedActorID == protoActor.ActorID)
		{
			ChangeLevelObjectState(levelObjectID, 0, occupy: false, ctsForServerRequest.Token, delegate(int newState, UseLevelObjectRes? res)
			{
				if (res == null || res.errorCode != MsgErrorCode.Success)
				{
					Logger.RError("InTramWaitingStartLeverLevelObject::TryInteractEnd : ChangeLevelObjectState Failed");
				}
			});
		}
		return true;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			Logger.RError("InTramWaitingStartLeverLevelObject::GetSimpleText : Hub or Main is null");
			return "";
		}
		InTramWaitingScene inTramWaitingScene = Hub.s.pdata.main as InTramWaitingScene;
		if (inTramWaitingScene == null)
		{
			return base.GetSimpleText(protoActor);
		}
		LeverState nextState = GetNextState(base.LeverState);
		if (HasStateActionTransition((int)base.LeverState, (int)nextState, out StateActionInfo stateActionInfo) && CanPullLever(stateActionInfo.action))
		{
			if (Hub.s.pdata.DayCount == 4)
			{
				return Hub.GetL10NText(textInCrossHairTramGotoMaintenance);
			}
			int dungeonIndex = inTramWaitingScene.GetDungeonIndex();
			if (dungeonIndex < 0)
			{
				Logger.RError("InTramWaitingStartLeverLevelObject::GetSimpleText : dungeonIdx is out of range");
				return "";
			}
			string l10NText = Hub.GetL10NText(textInCrossHairTramStop);
			int key = inTramWaitingScene.GetDunGeonCandidateIDs()[dungeonIndex];
			string key2 = "";
			if (Hub.s.dataman.ExcelDataManager.DungeonInfoDict.TryGetValue(key, out DungeonMasterInfo value))
			{
				key2 = value.mapName;
			}
			return l10NText.Replace("[mapname:]", Hub.GetL10NText(key2));
		}
		return base.GetSimpleText(protoActor);
	}
}
