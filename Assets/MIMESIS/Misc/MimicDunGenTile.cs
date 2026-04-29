using System.Collections.Generic;
using UnityEngine;

public class MimicDunGenTile : MonoBehaviour
{
	[InspectorReadOnly]
	public int debug_tileID;

	public int tileID { get; private set; }

	public void Awake()
	{
		tileID = 0;
		if (!(Hub.s == null) && Hub.s.pdata != null)
		{
			int num = (tileID = Hub.s.pdata.GenerateDunGenTileID());
			debug_tileID = num;
			base.name = $"({tileID}) {base.name}";
		}
	}

	private void Start()
	{
		MapTrigger[] componentsInChildren = GetComponentsInChildren<MapTrigger>();
		foreach (MapTrigger mapTrigger in componentsInChildren)
		{
			if (mapTrigger.usageType == MapTrigger.eUsageType.Server_TriggerVolume)
			{
				mapTrigger.serverData_actionString = mapTrigger.serverData_actionString.Replace("{tileid}", tileID.ToString());
			}
		}
		TrapLevelObject[] componentsInChildren2 = GetComponentsInChildren<TrapLevelObject>();
		foreach (TrapLevelObject trapLevelObject in componentsInChildren2)
		{
			trapLevelObject.levelObjectName = trapLevelObject.levelObjectName.Replace("{tileid}", tileID.ToString());
			foreach (KeyValuePair<int, Dictionary<int, LevelObject.StateActionInfo>> item in trapLevelObject.StateActionsMap)
			{
				Dictionary<int, LevelObject.StateActionInfo> dictionary = trapLevelObject.StateActionsMap[item.Key];
				foreach (KeyValuePair<int, LevelObject.StateActionInfo> item2 in dictionary)
				{
					LevelObject.StateActionInfo stateActionInfo = dictionary[item2.Key];
					stateActionInfo.action = stateActionInfo.action.Replace("{tileid}", tileID.ToString());
				}
			}
		}
		ShutterSwitchObject[] componentsInChildren3 = GetComponentsInChildren<ShutterSwitchObject>();
		foreach (ShutterSwitchObject shutterSwitchObject in componentsInChildren3)
		{
			shutterSwitchObject.levelObjectName = shutterSwitchObject.levelObjectName.Replace("{tileid}", tileID.ToString());
			foreach (KeyValuePair<int, Dictionary<int, LevelObject.StateActionInfo>> item3 in shutterSwitchObject.StateActionsMap)
			{
				Dictionary<int, LevelObject.StateActionInfo> dictionary2 = shutterSwitchObject.StateActionsMap[item3.Key];
				foreach (KeyValuePair<int, LevelObject.StateActionInfo> item4 in dictionary2)
				{
					LevelObject.StateActionInfo stateActionInfo2 = dictionary2[item4.Key];
					stateActionInfo2.action = stateActionInfo2.action.Replace("{tileid}", tileID.ToString());
				}
			}
		}
		DoorLevelObject[] componentsInChildren4 = GetComponentsInChildren<DoorLevelObject>();
		foreach (DoorLevelObject doorLevelObject in componentsInChildren4)
		{
			doorLevelObject.levelObjectName = doorLevelObject.levelObjectName.Replace("{tileid}", tileID.ToString());
			foreach (KeyValuePair<int, Dictionary<int, LevelObject.StateActionInfo>> item5 in doorLevelObject.StateActionsMap)
			{
				Dictionary<int, LevelObject.StateActionInfo> dictionary3 = doorLevelObject.StateActionsMap[item5.Key];
				foreach (KeyValuePair<int, LevelObject.StateActionInfo> item6 in dictionary3)
				{
					LevelObject.StateActionInfo stateActionInfo3 = dictionary3[item6.Key];
					stateActionInfo3.action = stateActionInfo3.action.Replace("{tileid}", tileID.ToString());
				}
			}
		}
	}
}
