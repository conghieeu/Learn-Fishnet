using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class ShutterSwitchObject : StaticLevelObject
{
	private enum TransitionMode
	{
		Off = 0,
		Once = 1,
		Toggle = 2
	}

	[SerializeField]
	private LeverState initialState;

	[SerializeField]
	private TransitionMode transitionMode = TransitionMode.Toggle;

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<LeverState>> stateActions = new List<StateAction<LeverState>>();

	private CancellationTokenSource? ctsForServerRequest;

	[SerializeField]
	private string TextSwitchReleased = "";

	[SerializeField]
	private string TextSwitchPushed = "";

	[SerializeField]
	private GameObject hiddenObstacle;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.ShutterSwitch;

	public LeverState LeverState => (LeverState)base.State;

	public override bool ForServer => true;

	protected void Awake()
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

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (base.IsTriggerable(protoActor, newState) && HasStateActionTransition(base.State, newState, out StateActionInfo stateActionInfo) && stateActionInfo.triggerableByClient)
		{
			return true;
		}
		return false;
	}

	private LeverState GetNextState(LeverState currentLeverState)
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
		ChangeLevelObjectState(levelObjectID, newState, occupy: false, ctsForServerRequest.Token, delegate(int num, UseLevelObjectRes? res)
		{
			if (res == null)
			{
				Logger.RError("LeverLevelObject::TriggerAction response is null");
			}
			else if (res.errorCode == MsgErrorCode.Success)
			{
				currentTransition = null;
				HasStateActionTransition(res.fromState, res.toState, out StateActionInfo _);
			}
		});
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int CurrentState)
	{
		int? num = Hub.s?.pdata?.main?.GetMyAvatar()?.ActorID;
		if (!num.HasValue || actorId != num.Value || prevState == 1)
		{
			PlayTriggerSound(prevState, CurrentState);
			AnimateObject(prevState, CurrentState);
		}
		if (prevState == 1 && CurrentState == 0)
		{
			StartCoroutine(CorSetActiveHiddenObstacle(active: false, 1f));
		}
		else if (prevState == 0 && CurrentState == 1)
		{
			StartCoroutine(CorSetActiveHiddenObstacle(active: true, 3.5f));
		}
		base.State = CurrentState;
		base.OccupiedActorID = occupiedActorID;
		currentTransition = null;
	}

	private IEnumerator CorSetActiveHiddenObstacle(bool active, float delay)
	{
		yield return new WaitForSeconds(delay);
		hiddenObstacle.SetActive(active);
		yield return null;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (LeverState == LeverState.On)
		{
			return Hub.GetL10NText(TextSwitchPushed);
		}
		return Hub.GetL10NText(TextSwitchReleased);
	}
}
