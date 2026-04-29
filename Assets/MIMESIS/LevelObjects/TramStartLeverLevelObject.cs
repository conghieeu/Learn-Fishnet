using System.Collections.Generic;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class TramStartLeverLevelObject : StaticLevelObject
{
	[SerializeField]
	private string NeedAllPlayerText = "CanStartTogether";

	[SerializeField]
	private string NeedToRepairText = "CanStartWithoutRepair";

	[SerializeField]
	private string OffText = "TrainStart";

	[SerializeField]
	private string EngineStartedText = "StartingEngine";

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<TramStartLeverState>> stateActions = new List<StateAction<TramStartLeverState>>();

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Lever;

	public TramStartLeverState TramStartLeverState => (TramStartLeverState)base.State;

	public override bool ForServer => true;

	public void Awake()
	{
		base.State = 0;
		base.InitialState = base.State;
		LoadStateActionsToMap(stateActions);
		ChangeClientState(base.State);
	}

	private void Start()
	{
		base.crossHairType = CrosshairType.Switch;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	public override bool NeedToShowCrossHair(ProtoActor protoActor)
	{
		if (Hub.s.pdata.main is MaintenanceScene { IsGameOver: not false } && Hub.s.pdata.GameState == Hub.PersistentData.eGameState.GoDeathMatch)
		{
			return false;
		}
		return base.gameObject.activeSelf;
	}

	public override float GetTransitionDuration()
	{
		if (currentTransition != null)
		{
			MaintenanceScene maintenanceScene = Hub.s.pdata.main as MaintenanceScene;
			if ((bool)maintenanceScene && TramStartLeverState == TramStartLeverState.Off)
			{
				if (!maintenanceScene.isRepaired)
				{
					return (float)Hub.s.dataman.ExcelDataManager.Consts.C_WaitTimeDecisionGameover * 0.001f;
				}
				return (float)Hub.s.dataman.ExcelDataManager.Consts.C_WaitTimeDecisionGamesuccess * 0.001f;
			}
			return currentTransition.transitionDurtaion;
		}
		return 0f;
	}

	private bool IsTriggeredByOther(ProtoActor protoActor)
	{
		MaintenanceScene maintenanceScene = Hub.s.pdata.main as MaintenanceScene;
		if ((bool)maintenanceScene)
		{
			if (maintenanceScene.ActorIdPullingTramStartLever != 0)
			{
				return maintenanceScene.ActorIdPullingTramStartLever != protoActor.ActorID;
			}
			return false;
		}
		return false;
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		bool flag = base.IsTriggerable(protoActor, newState);
		if (Hub.s.pdata.main is MaintenanceScene maintenanceScene)
		{
			if (IsTriggeredByOther(protoActor))
			{
				return false;
			}
			if (!Hub.s.pdata.main.IsAllPlayerInTram())
			{
				return false;
			}
			if (maintenanceScene.IsGameOver)
			{
				return false;
			}
		}
		if (flag)
		{
			return Hub.s.pdata.main.IsAllPlayerInTram();
		}
		return false;
	}

	protected override void Trigger(ProtoActor protoActor, int newState)
	{
		if ((bool)(Hub.s.pdata.main as MaintenanceScene) && !IsTriggeredByOther(protoActor) && Hub.s.pdata.main.IsAllPlayerInTram())
		{
			base.Trigger(protoActor, newState);
		}
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		TramStartLeverState newState = ((TramStartLeverState == TramStartLeverState.Off) ? TramStartLeverState.Depart : TramStartLeverState.Off);
		if (IsTriggerable(protoActor, (int)newState))
		{
			Trigger(protoActor, (int)newState);
		}
		return true;
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
		ChangeLevelObjectState(levelObjectID, newState, occupy: false, ctsForTriggerAction.Token, delegate(int num, UseLevelObjectRes? res)
		{
			if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RWarn($"TriggerAction : ChangeLevelObjectState Failed - {res.errorCode}");
				base.State = 0;
			}
			else
			{
				Logger.RLog($"TriggerAction : ChangeLevelObjectState Success - {num}", sendToLogServer: false, useConsoleOut: true, "levelobject");
				base.State = 1;
			}
		});
	}

	private void TryCancelStartTrain(ProtoActor protoActor)
	{
		base.State = 0;
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		base.TryInteractEnd(protoActor);
		TryCancelStartTrain(protoActor);
		return true;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		MaintenanceScene maintenanceScene = Hub.s.pdata.main as MaintenanceScene;
		if ((bool)maintenanceScene)
		{
			if (Hub.s.pdata.GameState == Hub.PersistentData.eGameState.PrepareDoneWithPublicLobby)
			{
				return Hub.GetL10NText("STRING_PUBLIC_TRAM_OFF");
			}
			if (currentTransition != null)
			{
				return Hub.GetL10NText(EngineStartedText);
			}
			switch ((TramStartLeverState)base.State)
			{
			case TramStartLeverState.Off:
				if (!Hub.s.pdata.main.IsAllPlayerInTram())
				{
					return Hub.GetL10NText(NeedAllPlayerText);
				}
				if (!maintenanceScene.isRepaired)
				{
					return Hub.GetL10NText(NeedToRepairText);
				}
				return Hub.GetL10NText(OffText);
			case TramStartLeverState.Depart:
				return "";
			}
		}
		return "";
	}

	public override string GetAddtionalSimpleText(ProtoActor protoActor)
	{
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			Logger.RError("LeverLevelObject::GetSimpleText : Hub or Main is null");
			return "";
		}
		if (Hub.s.pdata.main is MaintenanceScene maintenanceScene && Hub.s.pdata.main.IsAllPlayerInTram() && maintenanceScene.isRepaired && Hub.s.pdata.main.CurrentCurrency > 0 && Hub.s.pdata.GameState != Hub.PersistentData.eGameState.PrepareDoneWithPublicLobby)
		{
			return Hub.GetL10NText("UI_TRAM_LEVER_LEAVE_WARNING") + "\n" + Hub.GetL10NText("STRING_CROSSHAIR_CURRENT_FUNDS", Hub.s.pdata.main.CurrentCurrency);
		}
		return "";
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int currentState)
	{
		switch ((TramStartLeverState)currentState)
		{
		case TramStartLeverState.Off:
			base.crossHairType = CrosshairType.Switch;
			break;
		case TramStartLeverState.Depart:
			base.crossHairType = CrosshairType.None;
			break;
		}
		base.State = currentState;
	}
}
