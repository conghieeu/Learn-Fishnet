using System;
using System.Collections.Generic;
using System.Threading;
using Mimic;
using Mimic.Actors;
using ReluProtocol.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class DoorLevelObject : StaticLevelObject
{
	public DoorState initialState;

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<DoorState>> stateActions = new List<StateAction<DoorState>>();

	[SerializeField]
	private TextMeshPro debugText;

	[SerializeField]
	private List<DoorStateText> stateTexts;

	[SerializeField]
	private string openStateText;

	[SerializeField]
	private string closeStateText;

	[SerializeField]
	[Tooltip("켜면, 길막힘. 캐비넷 문등에 사용")]
	private bool skipNavMeshProcessing;

	private CancellationTokenSource? ctsForServerRequest;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Door;

	public DoorState doorState => (DoorState)base.State;

	public bool SkipNavMeshProcessing => skipNavMeshProcessing;

	public override bool ForServer => true;

	private void Awake()
	{
		base.State = (int)initialState;
		base.InitialState = base.State;
		LoadStateActionsToMap(stateActions);
		ChangeClientState(base.State);
		OnUpdateDoorState();
	}

	private void Start()
	{
		base.crossHairType = CrosshairType.Door;
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

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_door", allowScaling: true, iconColor);
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		switch (doorState)
		{
		case DoorState.Locked:
			return TryChangeDoorState(protoActor, DoorState.Opened);
		case DoorState.Closed:
		case DoorState.Opened:
			try
			{
				return TryChangeDoorState(protoActor, doorState switch
				{
					DoorState.Closed => DoorState.Opened, 
					DoorState.Opened => DoorState.Closed, 
					_ => throw new NotImplementedException(), 
				});
			}
			catch (NotImplementedException)
			{
				return false;
			}
		default:
			return false;
		}
	}

	protected bool TryChangeDoorState(ProtoActor protoActor, DoorState newDoorState)
	{
		if (IsTriggerable(protoActor, (int)newDoorState))
		{
			Trigger(protoActor, (int)newDoorState);
			return true;
		}
		return false;
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		bool result = base.IsTriggerable(protoActor, newState);
		if (HasStateActionTransition(base.State, newState, out StateActionInfo stateActionInfo) && stateActionInfo.triggerableByClient && !string.IsNullOrEmpty(stateActionInfo.condition))
		{
			string condition = stateActionInfo.condition;
			string[] conditionToken = condition.Split("/");
			if (conditionToken[0] == "CHECK_ITEM")
			{
				result = protoActor.GetInventoryItems().FindAll((InventoryItem? item) => item != null && item.ItemMasterID == int.Parse(conditionToken[1])).Count >= int.Parse(conditionToken[2]);
			}
		}
		return result;
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
			if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError($"DoorLevelObject TriggerAction failed: {res.errorCode}");
				ctsForTriggerAction?.Cancel();
				ctsForTriggerAction?.Dispose();
				ctsForTriggerAction = null;
			}
		});
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		return Hub.GetL10NText(stateTexts.Find((DoorStateText e) => e.state == doorState)?.text) ?? $"DoorState: {doorState}";
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int currentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, occupiedActorID, prevState, currentState);
		int? num = Hub.s?.pdata?.main?.GetMyAvatar()?.ActorID;
		if (!num.HasValue || actorId != num.Value)
		{
			PlayTriggerSound(prevState, currentState);
			AnimateObject(prevState, currentState);
		}
		OnUpdateDoorState();
	}

	private void OnUpdateDoorState()
	{
		if (debugText != null)
		{
			debugText.text = $"TestDoor\nInitialState: {initialState}\nCurrentState: {doorState}";
		}
		if (doorState != DoorState.Locked)
		{
			NavMeshObstacle component = GetComponent<NavMeshObstacle>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
	}
}
