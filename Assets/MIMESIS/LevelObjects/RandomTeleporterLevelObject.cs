using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class RandomTeleporterLevelObject : StaticLevelObject
{
	[SerializeField]
	private RandomTeleporterState initialState;

	[SerializeField]
	private bool destinationIsToInDoor;

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<RandomTeleporterState>> stateActions = new List<StateAction<RandomTeleporterState>>();

	private CancellationTokenSource? ctsForServerRequest;

	[SerializeField]
	private string textInCrossHair;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.RandomTeleporter;

	public bool DestinationIsToInDoor => destinationIsToInDoor;

	public override bool ForServer => true;

	private void Awake()
	{
		base.State = (int)initialState;
		base.InitialState = base.State;
		LoadStateActionsToMap(stateActions);
		ChangeClientState(base.State);
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
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	public override bool NeedToShowCrossHair(ProtoActor protoActor)
	{
		return base.State == 0;
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (base.State != 0)
		{
			return false;
		}
		Trigger(protoActor, 1);
		return true;
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
		ChangeLevelObjectState(levelObjectID, newState, occupy: true, CancellationToken.None, delegate(int num, UseLevelObjectRes? res)
		{
			if (res != null)
			{
				if (res.errorCode == MsgErrorCode.Success)
				{
					if (Hub.s.pdata.main as GamePlayScene != null)
					{
						currentTransition = null;
					}
				}
				else
				{
					Logger.RWarn($"RandomTeleport failed: {res.errorCode}");
				}
			}
		});
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int OccupiedActorID, int prevState, int currentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, OccupiedActorID, prevState, currentState);
		Logger.RLog($"RandomTeleporter OnChangeLevelObjectStateSig: {prevState} -> {currentState}, actorId: {actorId}, OccupiedActorID: {OccupiedActorID}");
		int? num = Hub.s?.pdata?.main?.GetMyAvatar()?.ActorID;
		if (!num.HasValue || actorId != num.Value || prevState != 0)
		{
			PlayTriggerSound(prevState, currentState);
			AnimateObject(prevState, currentState);
		}
		if (base.State == 2)
		{
			ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(actorId);
			if (actorByActorID != null)
			{
				actorByActorID.HideActor();
			}
		}
		else if (base.State == 0)
		{
			ProtoActor actorByActorID2 = Hub.s.pdata.main.GetActorByActorID(actorId);
			if (actorByActorID2 != null)
			{
				actorByActorID2.ShowActor();
			}
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		return Hub.GetL10NText(textInCrossHair);
	}
}
