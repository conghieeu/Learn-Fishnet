using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using ReluReplay.Shared;
using UnityEngine;

public class LeverLevelObject : StaticLevelObject
{
	protected enum TransitionMode
	{
		Off = 0,
		Once = 1,
		Toggle = 2,
		ResetInitAfterAction = 3
	}

	[SerializeField]
	private LeverState initialState;

	[SerializeField]
	protected TransitionMode transitionMode = TransitionMode.Toggle;

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<LeverState>> stateActions = new List<StateAction<LeverState>>();

	protected CancellationTokenSource? ctsForServerRequest;

	[SerializeField]
	protected string textInCrossHair;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Lever;

	public LeverState LeverState => (LeverState)base.State;

	public override bool ForServer => true;

	private void Awake()
	{
		base.State = (int)initialState;
		base.InitialState = (int)initialState;
		base.DisableReverseState = ((transitionMode == TransitionMode.Off || transitionMode == TransitionMode.Once) ? true : false);
		LoadStateActionsToMap(stateActions);
		ChangeClientState(base.State);
	}

	public void Start()
	{
		base.crossHairType = CrosshairType.Switch;
	}

	private void OnDestroy()
	{
		if (ctsForServerRequest != null)
		{
			if (!ctsForServerRequest.IsCancellationRequested)
			{
				ctsForServerRequest.Cancel();
			}
			ctsForServerRequest.Dispose();
			ctsForServerRequest = null;
		}
	}

	protected virtual LeverState GetNextState(LeverState currentLeverState)
	{
		switch (transitionMode)
		{
		case TransitionMode.Off:
			return LeverState.Off;
		case TransitionMode.Once:
			return LeverState.On;
		case TransitionMode.Toggle:
			try
			{
				return currentLeverState switch
				{
					LeverState.Off => LeverState.On, 
					LeverState.On => LeverState.Off, 
					_ => throw new NotImplementedException(), 
				};
			}
			catch
			{
				return LeverState.Off;
			}
		case TransitionMode.ResetInitAfterAction:
			try
			{
				if (currentLeverState == LeverState.Off)
				{
					return LeverState.On;
				}
				throw new NotImplementedException();
			}
			catch
			{
				return LeverState.Off;
			}
		default:
			return LeverState.Off;
		}
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		LeverState nextState = GetNextState(LeverState);
		switch (transitionMode)
		{
		case TransitionMode.Off:
			return false;
		case TransitionMode.Once:
			if (LeverState == LeverState.Off)
			{
				return TryChangeLeverState(protoActor, nextState);
			}
			return false;
		case TransitionMode.Toggle:
			return TryChangeLeverState(protoActor, nextState);
		case TransitionMode.ResetInitAfterAction:
			return TryChangeLeverState(protoActor, nextState);
		default:
			return false;
		}
	}

	protected bool TryChangeLeverState(ProtoActor protoActor, LeverState newLeverState)
	{
		if (IsTriggerable(protoActor, (int)newLeverState))
		{
			Trigger(protoActor, (int)newLeverState);
			return true;
		}
		return false;
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		bool flag = base.IsTriggerable(protoActor, newState);
		if (flag && HasStateActionTransition(base.State, newState, out StateActionInfo stateActionInfo) && stateActionInfo.triggerableByClient)
		{
			flag = CanPullLever(stateActionInfo.action) && flag;
		}
		return flag;
	}

	protected virtual bool CanPullLever(string leverAction)
	{
		return true;
	}

	protected void ResetState(CancellationTokenSource cts)
	{
		ChangeLevelObjectState(levelObjectID, (int)initialState, occupy: false, cts.Token, delegate(int newState, UseLevelObjectRes? res)
		{
			if (res.errorCode == MsgErrorCode.Success)
			{
				HasStateActionTransition(res.fromState, res.toState, out StateActionInfo _);
			}
			else
			{
				HasStateActionTransition((int)LeverState, newState, out StateActionInfo _);
			}
		});
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
		if (!ForServer)
		{
			return;
		}
		if (ctsForServerRequest != null)
		{
			if (!ctsForServerRequest.IsCancellationRequested)
			{
				ctsForServerRequest.Cancel();
			}
			ctsForServerRequest.Dispose();
		}
		ctsForServerRequest = new CancellationTokenSource();
		ChangeLevelObjectState(levelObjectID, newState, occupy: false, ctsForServerRequest.Token, delegate(int toState, UseLevelObjectRes? res)
		{
			StateActionInfo stateActionInfo2;
			if (res == null)
			{
				Logger.RError("LeverLevelObject::TriggerAction response is null");
			}
			else if (res.errorCode == MsgErrorCode.Success)
			{
				if (HasStateActionTransition(res.fromState, res.toState, out StateActionInfo stateActionInfo))
				{
					PullLever(stateActionInfo.action);
					if (transitionMode == TransitionMode.ResetInitAfterAction)
					{
						ResetState(ctsForServerRequest);
					}
				}
			}
			else if (HasStateActionTransition((int)LeverState, toState, out stateActionInfo2))
			{
				PullLever(stateActionInfo2.action);
				if (transitionMode == TransitionMode.ResetInitAfterAction)
				{
					ResetState(ctsForServerRequest);
				}
			}
		});
	}

	public virtual bool PullLever(string leverAction)
	{
		return false;
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int CurrentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, occupiedActorID, prevState, CurrentState);
		MaintenanceScene maintenanceScene = Hub.s.pdata.main as MaintenanceScene;
		if (maintenanceScene != null && !ReplaySharedData.IsReplayPlayMode && prevState == 0 && CurrentState == 1 && maintenanceScene.GetMyAvatar().ActorID != actorId)
		{
			AnimateObject(prevState, CurrentState);
			PlayTriggerSound(prevState, CurrentState);
		}
	}

	protected IEnumerator ShowSimpleToast(string text, float overrideDurationSec)
	{
		float seconds = Hub.s.tableman.uiprefabs.ShowTimerDialog("ToastSimple", overrideDurationSec, text);
		yield return new WaitForSeconds(seconds);
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			Logger.RError("LeverLevelObject::GetSimpleText : Hub or Main is null");
			return "";
		}
		return Hub.GetL10NText(textInCrossHair);
	}

	public override string GetAddtionalSimpleText(ProtoActor protoActor)
	{
		return "";
	}
}
