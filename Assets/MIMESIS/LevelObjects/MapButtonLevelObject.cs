using System;
using Mimic.Actors;
using UnityEngine;

public class MapButtonLevelObject : SwitchLevelObject
{
	public enum EButtonType
	{
		Arrow_Left = 0,
		Arrow_Right = 1,
		Arrow_Up = 2,
		Arrow_Down = 3
	}

	[SerializeField]
	private EButtonType buttonType;

	[SerializeField]
	private string textInCrossHair;

	public Action<EButtonType> triggerAction;

	private void Start()
	{
		base.crossHairType = CrosshairType.Switch;
	}

	public override CrosshairType GetCrossHairType(ProtoActor protoActor)
	{
		if (Hub.s.pdata.main as InTramWaitingScene != null && Hub.s.pdata.DayCount != 4)
		{
			return base.crossHairType;
		}
		return CrosshairType.None;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (Hub.s.pdata.main as InTramWaitingScene != null && Hub.s.pdata.DayCount != 4)
		{
			return true;
		}
		return false;
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
		if (buttonType == EButtonType.Arrow_Left || buttonType == EButtonType.Arrow_Right)
		{
			triggerAction(buttonType);
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		string result = "";
		InTramWaitingScene inTramWaitingScene = Hub.s.pdata.main as InTramWaitingScene;
		if (inTramWaitingScene != null && Hub.s.pdata.DayCount != 4)
		{
			result = Hub.GetL10NText(textInCrossHair);
			int key = inTramWaitingScene.GetDunGeonCandidateIDs()[(buttonType != EButtonType.Arrow_Left) ? 1 : 0];
			string key2 = "";
			if (Hub.s.dataman.ExcelDataManager.DungeonInfoDict.TryGetValue(key, out DungeonMasterInfo value))
			{
				key2 = value.mapName;
			}
			result = result.Replace("[mapname:]", Hub.GetL10NText(key2));
		}
		return result;
	}
}
