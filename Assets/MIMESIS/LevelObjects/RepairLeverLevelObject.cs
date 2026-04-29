using Mimic.Actors;
using UnityEngine;

public class RepairLeverLevelObject : LeverLevelObject
{
	[SerializeField]
	private string textInCrossHairFirstMaintenanceRepair;

	[SerializeField]
	private string textInCrossHairRepairCompleted;

	[SerializeField]
	private string textInCrossHairNotEnoughMoneyRepair;

	protected override bool CanPullLever(string leverAction)
	{
		MaintenanceScene maintenanceScene = Hub.s.pdata.main as MaintenanceScene;
		if (maintenanceScene != null)
		{
			if (!maintenanceScene.PrepareFirstCycle && !maintenanceScene.isRepaired)
			{
				return maintenanceScene.CurrentCurrency >= maintenanceScene.TargetCurrency;
			}
			return false;
		}
		return base.CanPullLever(leverAction);
	}

	public override bool PullLever(string leverAction)
	{
		MaintenanceScene maintenanceScene = Hub.s.pdata.main as MaintenanceScene;
		if (maintenanceScene != null)
		{
			maintenanceScene.TryRepairTram();
			return true;
		}
		return base.PullLever(leverAction);
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			Logger.RError("RepairLeverLevelObject::GetSimpleText : Hub or Main is null");
			return "";
		}
		if (Hub.s.pdata.main is MaintenanceScene maintenanceScene)
		{
			LeverState nextState = GetNextState(base.LeverState);
			if (HasStateActionTransition((int)base.LeverState, (int)nextState, out StateActionInfo stateActionInfo))
			{
				if (maintenanceScene.PrepareFirstCycle)
				{
					if (string.IsNullOrEmpty(textInCrossHairFirstMaintenanceRepair))
					{
						return Hub.GetL10NText(textInCrossHair);
					}
					return Hub.GetL10NText(textInCrossHairFirstMaintenanceRepair);
				}
				if (maintenanceScene.isRepairing)
				{
					return Hub.GetL10NText("TRAM_UNDER_REPAIR");
				}
				if (maintenanceScene.isRepaired)
				{
					return Hub.GetL10NText(textInCrossHairRepairCompleted);
				}
				if (!CanPullLever(stateActionInfo.action))
				{
					return Hub.GetL10NText(textInCrossHairNotEnoughMoneyRepair);
				}
			}
		}
		return base.GetSimpleText(protoActor);
	}
}
