using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public abstract class SimpleSwitchLevelObject : SwitchLevelObject
{
	[SerializeField]
	[Tooltip("플레이어의 조작시 점유되면 true, 그렇지 않으면 false")]
	private bool canBeOccupied;

	public override LevelObjectClientType LevelObjectType => LevelObjectClientType.Switch;

	public bool IsOn => base.State != 0;

	public override bool ForServer => true;

	protected virtual void Start()
	{
		base.crossHairType = CrosshairType.Switch;
		OnSwitchInitialized();
	}

	protected abstract void OnSwitchInitialized();

	protected abstract void OnSwitchStateChanged(bool isOn);

	public override bool TryInteract(ProtoActor protoActor)
	{
		int newState = ((base.State == 0) ? 1 : 0);
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

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (!base.IsTriggerable(protoActor, newState))
		{
			return false;
		}
		if (canBeOccupied)
		{
			bool num = base.OccupiedActorID != 0;
			bool flag = base.OccupiedActorID == protoActor.ActorID;
			if (num && !flag)
			{
				return false;
			}
		}
		return true;
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
		bool occupy = ((canBeOccupied && base.State == 0 && newState == 1) ? true : false);
		ChangeLevelObjectState(levelObjectID, newState, occupy, CancellationToken.None, OnUseLevelObjectRes);
	}

	private void OnUseLevelObjectRes(int newState, UseLevelObjectRes res)
	{
		if (!(this == null) && (res == null || res.errorCode != MsgErrorCode.Success))
		{
			Logger.RError($"Failed to use '{base.gameObject.name}': error={res?.errorCode}, newState={newState}");
		}
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int currentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, occupiedActorID, prevState, currentState);
		OnSwitchStateChanged(currentState != 0);
	}
}
