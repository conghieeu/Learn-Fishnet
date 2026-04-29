using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using ReluReplay.Shared;
using TMPro;
using UnityEngine;

internal class MomentarySwitchLevelObject : StaticLevelObject
{
	[SerializeField]
	private MomentarySwitchState initialState;

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<MomentarySwitchState>> stateActions = new List<StateAction<MomentarySwitchState>>();

	[SerializeField]
	private string MomentarySwitchOffText = "[E] Use Switch";

	[SerializeField]
	private string MomentarySwitchOnText = "Pressing...";

	private CancellationTokenSource ctsForServerRequest;

	[SerializeField]
	private TextMeshPro debugText;

	private int reserveOccupiedActorID;

	private bool interactionEndBeforeSwitchOn;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.MomentarySwitch;

	public MomentarySwitchState MomentarySwitchState => (MomentarySwitchState)base.State;

	public override bool ForServer => true;

	private void Awake()
	{
		base.State = (int)initialState;
		base.InitialState = base.State;
		LoadStateActionsToMap(stateActions);
		ChangeClientState(base.State);
		UpdateDebugText();
		debugText?.gameObject.SetActive(value: false);
	}

	public void Start()
	{
		base.crossHairType = CrosshairType.SecretPassageNormal;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	public override bool NeedToShowCrossHair(ProtoActor protoActor)
	{
		return base.gameObject.activeSelf;
	}

	public override CrosshairType GetCrossHairType(ProtoActor protoActor)
	{
		return base.crossHairType;
	}

	protected bool TryChangeMomentarySwitchState(ProtoActor protoActor, MomentarySwitchState newMomentarySwitchState)
	{
		if (IsTriggerable(protoActor, (int)newMomentarySwitchState))
		{
			Trigger(protoActor, (int)newMomentarySwitchState);
			return true;
		}
		return false;
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (MomentarySwitchState == MomentarySwitchState.RemoteOn)
		{
			return false;
		}
		if ((base.State != newState || reserveOccupiedActorID == protoActor.ActorID) && Time.time - lastTransitionTime >= minTransitionInterval && (currentTransition == null || reserveOccupiedActorID == protoActor.ActorID))
		{
			if (base.OccupiedActorID != 0 && base.OccupiedActorID != protoActor.ActorID)
			{
				return reserveOccupiedActorID == protoActor.ActorID;
			}
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
		bool flag = ((base.State == 0 && newState == 1) ? true : false);
		int prevState = base.State;
		if (flag)
		{
			reserveOccupiedActorID = protoActor.ActorID;
		}
		Logger.RLog($"MomentarySwitch ChangeLevelObjectRequest in TriggerAction: {MomentarySwitchState}, prevState: {prevState}, newState: {newState}, occupy: {flag}, reserveOccupiedActorID: {reserveOccupiedActorID}, protoActor.ActorID: {protoActor.ActorID}");
		ChangeLevelObjectState(levelObjectID, newState, flag, ctsForServerRequest.Token, delegate(int toState, UseLevelObjectRes? res)
		{
			if (res.errorCode == MsgErrorCode.Success)
			{
				if (HasStateActionTransition(prevState, toState, out StateActionInfo stateActionInfo) && animator != null)
				{
					animator.CrossFade(stateActionInfo.animatorStateName, 0.3f);
				}
			}
			else
			{
				ctsForTriggerAction?.Cancel();
				ctsForTriggerAction?.Dispose();
				ctsForTriggerAction = null;
			}
		});
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		switch (MomentarySwitchState)
		{
		case MomentarySwitchState.Off:
			return TryChangeMomentarySwitchState(protoActor, MomentarySwitchState.On);
		case MomentarySwitchState.On:
			reserveOccupiedActorID = 0;
			return TryChangeMomentarySwitchState(protoActor, MomentarySwitchState.Off);
		default:
			return false;
		}
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		if (protoActor.AmIAvatar())
		{
			if (MomentarySwitchState == MomentarySwitchState.On && base.OccupiedActorID == protoActor.ActorID)
			{
				TryChangeMomentarySwitchState(protoActor, MomentarySwitchState.Off);
				reserveOccupiedActorID = 0;
			}
			if (MomentarySwitchState == MomentarySwitchState.Off && reserveOccupiedActorID == protoActor.ActorID)
			{
				interactionEndBeforeSwitchOn = true;
				Logger.RLog($"TryInteractEnd: {MomentarySwitchState}, reserveOccupiedActorID: {reserveOccupiedActorID}, protoActor.ActorID: {protoActor.ActorID}");
			}
			currentTransition = null;
		}
		UpdateDebugText();
		return true;
	}

	private void UpdateDebugText()
	{
		if (debugText != null)
		{
			debugText.text = $"TestMomentarySwitch\n( {levelObjectName} )\ncrossHairType: {base.crossHairType}";
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		return MomentarySwitchState switch
		{
			MomentarySwitchState.Off => Hub.GetL10NText(MomentarySwitchOffText), 
			MomentarySwitchState.On => Hub.GetL10NText(MomentarySwitchOnText), 
			MomentarySwitchState.RemoteOn => Hub.GetL10NText(MomentarySwitchOnText), 
			_ => "", 
		};
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int currentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, occupiedActorID, prevState, currentState);
		Logger.RLog($"MomentarySwitch OnChangeLevelObjectStateSig: {(MomentarySwitchState)prevState} -> {(MomentarySwitchState)currentState}, actorId: {actorId}, OccupiedActorID: {occupiedActorID}");
		int? num = Hub.s?.pdata?.main?.GetMyAvatar()?.ActorID;
		if (!num.HasValue || actorId != num.Value || prevState == 2 || currentState == 2)
		{
			PlayTriggerSound(prevState, currentState);
			AnimateObject(prevState, currentState);
		}
		switch ((MomentarySwitchState)currentState)
		{
		case MomentarySwitchState.On:
		case MomentarySwitchState.RemoteOn:
			base.crossHairType = CrosshairType.SecretPassageOpening;
			break;
		case MomentarySwitchState.Off:
			base.crossHairType = CrosshairType.SecretPassageNormal;
			break;
		}
		if (interactionEndBeforeSwitchOn && currentState == 1 && reserveOccupiedActorID == base.OccupiedActorID)
		{
			ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(base.OccupiedActorID);
			TryChangeMomentarySwitchState(actorByActorID, MomentarySwitchState.Off);
			interactionEndBeforeSwitchOn = false;
		}
		if (currentState == 0)
		{
			reserveOccupiedActorID = 0;
		}
		UpdateDebugText();
		if (currentState == 1 && Hub.s?.pdata != null && Hub.s?.voiceman != null)
		{
			Hub.s.voiceman.TrySendToServerVoiceEmotion(actorId, ReplaySharedData.E_EVENT.USE_SWITCH);
		}
	}
}
