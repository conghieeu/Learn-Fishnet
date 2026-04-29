using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using TMPro;
using UnityEngine;

public class TransmitterLevelObject : StaticLevelObject
{
	[SerializeField]
	private TransmitterState initialState;

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<TransmitterState>> stateActions = new List<StateAction<TransmitterState>>();

	[SerializeField]
	private string TransmitterUsableText = "[E] Use Transmitter";

	[SerializeField]
	private string TransmitterUsingText = "Transmitting...";

	private CancellationTokenSource ctsForServerRequest;

	[SerializeField]
	private TextMeshPro debugText;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Transmitter;

	public TransmitterState TransmitterState => (TransmitterState)base.State;

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
		base.crossHairType = CrosshairType.TransmitterUsable;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_transmitter", allowScaling: true, iconColor);
	}

	public override bool NeedToShowCrossHair(ProtoActor protoActor)
	{
		return base.gameObject.activeSelf;
	}

	public override CrosshairType GetCrossHairType(ProtoActor protoActor)
	{
		return base.crossHairType;
	}

	protected bool TryChangeTransmitterState(ProtoActor protoActor, TransmitterState newTransmitterState)
	{
		if (IsTriggerable(protoActor, (int)newTransmitterState))
		{
			Trigger(protoActor, (int)newTransmitterState);
			return true;
		}
		return false;
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (base.IsTriggerable(protoActor, newState))
		{
			if (base.OccupiedActorID != 0)
			{
				return base.OccupiedActorID == protoActor.ActorID;
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
		bool occupy = ((base.State == 0 && newState == 1) ? true : false);
		ChangeLevelObjectState(levelObjectID, newState, occupy, ctsForServerRequest.Token, delegate(int num, UseLevelObjectRes? res)
		{
			if (res.errorCode != MsgErrorCode.Success)
			{
				ctsForTriggerAction?.Cancel();
			}
		});
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (protoActor != null)
		{
			protoActor.SetInputMoveDisableReason(ProtoActor.EInputMoveDisableReason.OccupyingLevelObject);
			protoActor.StartPauseTransformSync();
		}
		return TransmitterState switch
		{
			TransmitterState.Off => TryChangeTransmitterState(protoActor, TransmitterState.On), 
			TransmitterState.On => TryChangeTransmitterState(protoActor, TransmitterState.Off), 
			_ => false, 
		};
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		if (protoActor == null)
		{
			return false;
		}
		protoActor.ClearInputMoveDisableReason(ProtoActor.EInputMoveDisableReason.OccupyingLevelObject);
		protoActor.StopPauseTransformSync();
		if (protoActor.AmIAvatar())
		{
			if (TransmitterState == TransmitterState.On && base.OccupiedActorID == protoActor.ActorID)
			{
				TryChangeTransmitterState(protoActor, TransmitterState.Off);
				base.crossHairType = CrosshairType.TransmitterUsable;
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
			debugText.text = $"TestTransmitter\n( {levelObjectName} )\ncrossHairType: {base.crossHairType}";
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		return base.crossHairType switch
		{
			CrosshairType.TransmitterUsable => Hub.GetL10NText(TransmitterUsableText), 
			CrosshairType.TransmitterUsing => Hub.GetL10NText(TransmitterUsingText), 
			_ => "", 
		};
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int currentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, occupiedActorID, prevState, currentState);
		Logger.RLog($"Transmitter OnChangeLevelObjectStateSig: {actorId}, {occupiedActorID}, {prevState}, {currentState}");
		int? num = Hub.s?.pdata?.main?.GetMyAvatar()?.ActorID;
		if (!num.HasValue || actorId != num.Value)
		{
			PlayTriggerSound(prevState, currentState);
			AnimateObject(prevState, currentState);
		}
		if (currentState == 1)
		{
			base.crossHairType = CrosshairType.TransmitterUsing;
			if (!Hub.s.replayManager.IsReplayPlayMode && occupiedActorID == Hub.s.pdata.main.GetMyAvatar().ActorID)
			{
				Hub.s.voiceman.SetTransmitterChannelSend(canSpeak: true);
			}
		}
		else
		{
			base.crossHairType = CrosshairType.TransmitterUsable;
			if (!Hub.s.replayManager.IsReplayPlayMode)
			{
				Hub.s.voiceman.SetTransmitterChannelSend(canSpeak: false);
			}
		}
		ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(actorId);
		if (actorByActorID != null)
		{
			bool isEnabled = currentState == 1;
			actorByActorID.SetVoiceEffect(ProtoActor.VoiceEffecter.VoiceEffectType.Transmitter, isEnabled);
		}
		UpdateDebugText();
	}
}
