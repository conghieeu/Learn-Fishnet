using Mimic.Actors;
using ReluReplay.Shared;
using UnityEngine;

public class TramLeverLevelObject : LeverLevelObject
{
	[SerializeField]
	protected string textInCrossHairTramStop;

	[SerializeField]
	protected string textInCrossHairTramGotoMaintenance;

	protected override bool CanPullLever(string leverAction)
	{
		if (leverAction == "MAP_COMPLETE" && Hub.s.pdata.main as MaintenanceScene != null)
		{
			return Hub.s.pdata.main.IsAllPlayerInTram();
		}
		return base.CanPullLever(leverAction);
	}

	public override bool PullLever(string leverAction)
	{
		if (leverAction == "MAP_COMPLETE")
		{
			GameMainBase main = Hub.s.pdata.main;
			if (main is MaintenanceScene maintenanceScene)
			{
				maintenanceScene.TriggerHostStartSession();
				return true;
			}
			if (main is GamePlayScene gamePlayScene)
			{
				gamePlayScene.OnMapComplete();
				return true;
			}
		}
		return base.PullLever(leverAction);
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int CurrentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, occupiedActorID, prevState, CurrentState);
		GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
		if (!(gamePlayScene != null) || !HasStateActionTransition(prevState, CurrentState, out StateActionInfo stateActionInfo) || !(stateActionInfo.action == "MAP_COMPLETE") || ReplaySharedData.IsReplayPlayMode)
		{
			return;
		}
		if (gamePlayScene.GetMyAvatar().ActorID != actorId)
		{
			ProtoActor myAvatar = gamePlayScene.GetMyAvatar();
			if (!(myAvatar != null))
			{
				return;
			}
			string key = "UI_TOAST_TRAM_STARTED_WASTED";
			if (myAvatar.dead)
			{
				key = "UI_TOAST_TRAM_STARTED";
			}
			else
			{
				foreach (var item2 in Hub.s.dynamicDataMan.GetInTramVolume())
				{
					if (item2.Item1.usageType == MapTrigger.eUsageType.ClientOnly_InsideTramVolume)
					{
						Bounds item = item2.Item2;
						if (item.Contains(myAvatar.gameObject.transform.position))
						{
							key = "UI_TOAST_TRAM_STARTED";
						}
					}
				}
			}
			string l10NText = Hub.GetL10NText(key);
			ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(actorId);
			if (actorByActorID != null)
			{
				l10NText = l10NText.Replace("[usernickname:]", actorByActorID.netSyncActorData.actorName);
				StartCoroutine(ShowSimpleToast(l10NText, (float)Hub.s.dataman.ExcelDataManager.Consts.C_UI_PopupDurationDungeonStartLever * 0.001f));
			}
		}
		else
		{
			string l10NText2 = Hub.GetL10NText("UI_TOAST_TRAM_STARTED_SELF");
			StartCoroutine(ShowSimpleToast(l10NText2, (float)Hub.s.dataman.ExcelDataManager.Consts.C_UI_PopupDurationDungeonStartLever * 0.001f));
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			Logger.RError("TramLeverLevelObject::GetSimpleText : Hub or Main is null");
			return "";
		}
		LeverState nextState = GetNextState(base.LeverState);
		if (HasStateActionTransition((int)base.LeverState, (int)nextState, out StateActionInfo stateActionInfo))
		{
			string action = stateActionInfo.action;
			if (Hub.s.pdata.main is MaintenanceScene && action == "MAP_COMPLETE" && !Hub.s.pdata.main.IsAllPlayerInTram())
			{
				return Hub.GetL10NText("TRAM_NOT_ENOUGH_PASSENGERS");
			}
		}
		if (Hub.s.pdata.GameState == Hub.PersistentData.eGameState.PrepareDoneWithPublicLobby)
		{
			return Hub.GetL10NText("STRING_PUBLIC_TRAM_OFF");
		}
		return Hub.GetL10NText(textInCrossHair);
	}

	public override string GetAddtionalSimpleText(ProtoActor protoActor)
	{
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			Logger.RError("TramLeverLevelObject::GetAddtionalSimpleText : Hub or Main is null");
			return "";
		}
		LeverState nextState = GetNextState(base.LeverState);
		if (HasStateActionTransition((int)base.LeverState, (int)nextState, out StateActionInfo stateActionInfo))
		{
			string action = stateActionInfo.action;
			if (Hub.s.pdata.main is MaintenanceScene && action == "MAP_COMPLETE" && Hub.s.pdata.main.CurrentCurrency > 0)
			{
				return Hub.GetL10NText("UI_TRAM_LEVER_LEAVE_WARNING") + "\n" + Hub.GetL10NText("STRING_CROSSHAIR_CURRENT_FUNDS", Hub.s.pdata.main.CurrentCurrency);
			}
		}
		return "";
	}
}
