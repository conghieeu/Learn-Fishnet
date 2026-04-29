using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReluProtocol.Enum;
using UnityEngine;

[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=5084410072")]
public class DynamicDataManager : MonoBehaviour
{
	public class TriggerEvent
	{
		public MapTrigger? trigger;

		public string hitterAddress = "N/A";

		public bool isEnter;
	}

	public class CutSceneData
	{
		public string name = string.Empty;

		public float duration;

		public bool lockInput;
	}

	private List<(MapTrigger, Bounds)> boxTriggers = new List<(MapTrigger, Bounds)>();

	private List<(MapTrigger, BoundingSphere)> sphereTriggers = new List<(MapTrigger, BoundingSphere)>();

	private List<TriggerEvent> triggerEvents = new List<TriggerEvent>();

	private Dictionary<int, MapMarker_CreatureSpawnPoint> _playerStartPoints = new Dictionary<int, MapMarker_CreatureSpawnPoint>();

	private Dictionary<int, MapMarker_CreatureSpawnPoint> _monsterSpawnPoints = new Dictionary<int, MapMarker_CreatureSpawnPoint>();

	private Dictionary<int, MapMarker_LootingObjectSpawnPoint> _lootingObjectPoints = new Dictionary<int, MapMarker_LootingObjectSpawnPoint>();

	private Dictionary<int, MapMarker_FieldSkillSpawnPoint> _fieldSkillSpawnPoints = new Dictionary<int, MapMarker_FieldSkillSpawnPoint>();

	private Dictionary<string, MapMarker_TeleportStartPoint> _teleportStartPoints = new Dictionary<string, MapMarker_TeleportStartPoint>();

	private Dictionary<string, MapMarker_TeleportEndPoint> _teleportEndPoints = new Dictionary<string, MapMarker_TeleportEndPoint>();

	private List<MapMarker_TeleportEndPoint> _randomTeleportEndPoints = new List<MapMarker_TeleportEndPoint>();

	private Dictionary<int, MapMarker_TargetPoint> _targetPoints = new Dictionary<int, MapMarker_TargetPoint>();

	private Dictionary<int, LevelObject> _levelObjects = new Dictionary<int, LevelObject>();

	private List<Camera> _cctvCameras = new List<Camera>();

	private List<CutSceneData> _cutscenes = new List<CutSceneData>();

	private List<BlackOut> _blackOutProps = new List<BlackOut>();

	private Dictionary<int, LevelObject> _tramUpgradeLevelObjects = new Dictionary<int, LevelObject>();

	public void Build(Transform rootNode, int randDungeonSeed = 0)
	{
		GameMainBase.SyncRandom syncRandom = new GameMainBase.SyncRandom(randDungeonSeed);
		boxTriggers.Clear();
		sphereTriggers.Clear();
		foreach (MapTrigger item2 in ReLUGameKitUtility.MakeListByHierarchyOrder<MapTrigger>(rootNode))
		{
			BoxCollider component = item2.GetComponent<BoxCollider>();
			if (component != null && item2.shapeType == MapTrigger.eShapeType.AABB)
			{
				boxTriggers.Add((item2, component.bounds));
			}
			SphereCollider component2 = item2.GetComponent<SphereCollider>();
			if (component2 != null && item2.shapeType == MapTrigger.eShapeType.Sphere)
			{
				BoundingSphere item = new BoundingSphere(component2.bounds.center, component2.bounds.extents.x);
				sphereTriggers.Add((item2, item));
			}
			if (item2.usageType != MapTrigger.eUsageType.Server_TriggerVolume || string.IsNullOrEmpty(item2.serverData_conditionString) || !item2.serverData_conditionString.Contains("{scrap_weight}"))
			{
				continue;
			}
			int weightGram = syncRandom.Next(Hub.s.dataman.ExcelDataManager.Consts.C_WeightLimitTrapWeightMin, Hub.s.dataman.ExcelDataManager.Consts.C_WeightLimitTrapWeightMax + 1) * 1000;
			item2.serverData_conditionString = item2.serverData_conditionString.Replace("{scrap_weight}", weightGram.ToString());
			Transform parent = item2.transform.parent;
			if (!(parent != null))
			{
				continue;
			}
			foreach (Transform item3 in parent)
			{
				if (!(item3 == base.transform) && item3.TryGetComponent<UIPrefab_Weight>(out var component3))
				{
					component3.OnSetWeight(weightGram);
				}
			}
		}
		_playerStartPoints.Clear();
		_monsterSpawnPoints.Clear();
		List<MapMarker_CreatureSpawnPoint> list = ReLUGameKitUtility.MakeListByHierarchyOrder<MapMarker_CreatureSpawnPoint>(rootNode);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (MapMarker_CreatureSpawnPoint item4 in list)
		{
			item4.SetID(++num3);
			if (item4.type == ActorType.Player)
			{
				_playerStartPoints.Add(num++, item4);
			}
			else
			{
				if (item4.type != ActorType.Monster)
				{
					continue;
				}
				if (item4.masterID != 0 && item4.FixedSpawnPointActiveRate != 10000)
				{
					if (item4.FixedSpawnPointActiveRate <= syncRandom.Next(0, 10001))
					{
						item4.gameObject.SetActive(value: false);
						continue;
					}
					item4.gameObject.SetActive(value: true);
				}
				_monsterSpawnPoints.Add(num2++, item4);
			}
		}
		_lootingObjectPoints.Clear();
		List<MapMarker_LootingObjectSpawnPoint> list2 = ReLUGameKitUtility.MakeListByHierarchyOrder<MapMarker_LootingObjectSpawnPoint>(rootNode);
		int num4 = 0;
		foreach (MapMarker_LootingObjectSpawnPoint item5 in list2)
		{
			item5.SetID(++num3);
			_lootingObjectPoints.Add(++num4, item5);
		}
		_fieldSkillSpawnPoints.Clear();
		List<MapMarker_FieldSkillSpawnPoint> list3 = ReLUGameKitUtility.MakeListByHierarchyOrder<MapMarker_FieldSkillSpawnPoint>(rootNode);
		int num5 = 0;
		foreach (MapMarker_FieldSkillSpawnPoint item6 in list3)
		{
			item6.SetID(++num3);
			_fieldSkillSpawnPoints.Add(++num5, item6);
		}
		_teleportEndPoints.Clear();
		_randomTeleportEndPoints.Clear();
		foreach (MapMarker_TeleportEndPoint item7 in ReLUGameKitUtility.MakeListByHierarchyOrder<MapMarker_TeleportEndPoint>(rootNode))
		{
			if (item7.TeleportType == TeleportType.EventActionRandom)
			{
				_randomTeleportEndPoints.Add(item7);
			}
			else if (_teleportEndPoints.ContainsKey(item7.CallSign))
			{
				Logger.RError("[DynamicDataManager] Duplicate TeleportEndPoint CallSign(" + item7.CallSign + ") found.");
			}
			else
			{
				_teleportEndPoints.Add(item7.CallSign, item7);
			}
		}
		_teleportStartPoints.Clear();
		foreach (MapMarker_TeleportStartPoint item8 in ReLUGameKitUtility.MakeListByHierarchyOrder<MapMarker_TeleportStartPoint>(rootNode))
		{
			if (item8.TeleportType == TeleportType.EventActionRandom && _randomTeleportEndPoints.Count <= 0)
			{
				Logger.RError($"[DynamicDataManager] RandomTeleportStartPoint({item8.CallSign}). NO RandomTeleportEndPoint({_randomTeleportEndPoints.Count}).");
			}
			else if (item8.TeleportType == TeleportType.EventActionNormal && !_teleportEndPoints.ContainsKey(item8.EndCallSign))
			{
				Logger.RError("[DynamicDataManager] TeleportStartPoint(" + item8.CallSign + ") NOT FOUND TeleportEndPoint(" + item8.EndCallSign + ").");
			}
			else if (_teleportStartPoints.ContainsKey(item8.CallSign))
			{
				Logger.RError("[DynamicDataManager] Duplicate TeleportStartPoint CallSign(" + item8.CallSign + ") found.");
			}
			else
			{
				_teleportStartPoints.Add(item8.CallSign, item8);
			}
		}
		_targetPoints.Clear();
		foreach (MapMarker_TargetPoint item9 in ReLUGameKitUtility.MakeListByHierarchyOrder<MapMarker_TargetPoint>(rootNode))
		{
			if ((object)item9 != null)
			{
				MapMarker_TargetPoint mapMarker_TargetPoint = item9;
				mapMarker_TargetPoint.SetID(++num3);
				_targetPoints.Add(mapMarker_TargetPoint.ID, mapMarker_TargetPoint);
			}
		}
		_levelObjects.Clear();
		_tramUpgradeLevelObjects.Clear();
		List<LevelObject> allInactiveIDAssignableLevelObjects = GetAllInactiveIDAssignableLevelObjects(rootNode);
		int num6 = 1;
		List<LevelObject> list4 = new List<LevelObject>();
		List<LevelObject> collection = ReLUGameKitUtility.MakeListByHierarchyOrder<LevelObject>(rootNode);
		list4.AddRange(collection);
		list4.AddRange(allInactiveIDAssignableLevelObjects);
		for (int i = 0; i < list4.Count; i++)
		{
			if (list4[i].GetAIHandlePos().HasValue)
			{
				list4[i].internalOnly_AdjustAIHandler();
			}
			if (list4[i] is TeleporterLevelObject teleporterLevelObject)
			{
				if (!_teleportStartPoints.ContainsKey(teleporterLevelObject.StartCallSign))
				{
					Logger.RError("[DynamicDataManager] TeleporterLevelObject(" + teleporterLevelObject.name + ") NOT FOUND TeleportStartPoint(" + teleporterLevelObject.StartCallSign + ").");
					continue;
				}
				MapMarker_TeleportStartPoint mapMarker_TeleportStartPoint = _teleportStartPoints[teleporterLevelObject.StartCallSign];
				if (!_teleportEndPoints.ContainsKey(mapMarker_TeleportStartPoint.EndCallSign))
				{
					Logger.RError("[DynamicDataManager] TeleporterLevelObject(" + teleporterLevelObject.name + ") NOT FOUND TeleportEndPoint(" + mapMarker_TeleportStartPoint.EndCallSign + ").");
					continue;
				}
				MapMarker_TeleportEndPoint mapMarker_TeleportEndPoint = _teleportEndPoints[mapMarker_TeleportStartPoint.EndCallSign];
				if (teleporterLevelObject.DestinationIsToInDoor != mapMarker_TeleportEndPoint.IsIndoor)
				{
					Logger.RError("[DynamicDataManager] TeleporterLevelObject(" + teleporterLevelObject.name + ") has different indoor value with TeleportEndPoint(" + mapMarker_TeleportStartPoint.EndCallSign + ").");
				}
			}
			list4[i].levelObjectID = num6;
			_levelObjects.Add(num6, list4[i]);
			num6++;
			if (list4[i] is ITramUpgradeLevelObject)
			{
				if (!_tramUpgradeLevelObjects.ContainsKey(list4[i].levelObjectID))
				{
					_tramUpgradeLevelObjects.Add(list4[i].levelObjectID, list4[i]);
				}
				else
				{
					Logger.RWarn("[DynamicDataManager] Duplicate TramUpgradeLevelObject name(" + list4[i].name + ") found.");
				}
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<int, LevelObject> levelObject in _levelObjects)
		{
			stringBuilder.AppendLine($"{{name={levelObject.Value.name}, ID={levelObject.Key}]}}");
		}
		List<CutScenePlayer> list5 = ReLUGameKitUtility.MakeListByHierarchyOrder<CutScenePlayer>(rootNode);
		for (int j = 0; j < list5.Count; j++)
		{
			foreach (CutScenePlayer.CutScene cutscene in list5[j].cutscenes)
			{
				CutSceneData cutSceneData = new CutSceneData();
				cutSceneData.name = cutscene.name;
				if (cutscene.direction != null)
				{
					cutSceneData.duration = (float)cutscene.direction.duration;
				}
				else if (cutscene.videoClip != null)
				{
					cutSceneData.duration = (float)cutscene.videoClip.length;
				}
				cutSceneData.lockInput = cutscene.lockInput;
				_cutscenes.Add(cutSceneData);
			}
		}
		_blackOutProps.Clear();
		List<BlackOut> list6 = ReLUGameKitUtility.MakeListByHierarchyOrder<BlackOut>(rootNode);
		for (int k = 0; k < list6.Count; k++)
		{
			_blackOutProps.Add(list6[k]);
		}
	}

	private List<LevelObject> GetAllInactiveIDAssignableLevelObjects(Transform rootNode)
	{
		List<LevelObject> list = new List<LevelObject>();
		foreach (LevelObject item in ReLUGameKitUtility.MakeListByHierarchyOrder<LevelObject>(rootNode, includeInactive: true))
		{
			if (item != null && item.assignIDWhenInactive && !item.gameObject.activeInHierarchy)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public void Clear()
	{
		boxTriggers.Clear();
		sphereTriggers.Clear();
		triggerEvents.Clear();
		_playerStartPoints.Clear();
		_monsterSpawnPoints.Clear();
		_cutscenes.Clear();
		_blackOutProps.Clear();
	}

	public List<CutSceneData> GetCutSceneInfos()
	{
		return _cutscenes;
	}

	public List<(MapTrigger, Bounds)> GetPlayerStartingVolume()
	{
		List<(MapTrigger, Bounds)> list = new List<(MapTrigger, Bounds)>();
		foreach (var (mapTrigger, item) in boxTriggers)
		{
			if (mapTrigger.usageType == MapTrigger.eUsageType.Server_StartingVolume)
			{
				list.Add((mapTrigger, item));
			}
		}
		return list;
	}

	public List<(MapTrigger, Bounds)> GetInTramVolume()
	{
		List<(MapTrigger, Bounds)> list = new List<(MapTrigger, Bounds)>();
		foreach (var (mapTrigger, item) in boxTriggers)
		{
			if (mapTrigger.usageType == MapTrigger.eUsageType.ClientOnly_InsideTramVolume)
			{
				list.Add((mapTrigger, item));
			}
		}
		return list;
	}

	public List<(MapTrigger, Bounds)> GetPlayerTriggerVolumes()
	{
		List<(MapTrigger, Bounds)> list = new List<(MapTrigger, Bounds)>();
		foreach (var (mapTrigger, item) in boxTriggers)
		{
			if (mapTrigger.usageType == MapTrigger.eUsageType.Server_TriggerVolume || mapTrigger.usageType == MapTrigger.eUsageType.Server_StartingVolume)
			{
				list.Add((mapTrigger, item));
			}
		}
		return list;
	}

	public int GetTramLevelObjectIDByUpgradeID(int upgradeID)
	{
		foreach (LevelObject value in _tramUpgradeLevelObjects.Values)
		{
			if (value is ITramUpgradeLevelObject tramUpgradeLevelObject && tramUpgradeLevelObject.TramUpgradeID == upgradeID)
			{
				return value.levelObjectID;
			}
		}
		return 0;
	}

	public Dictionary<int, LevelObject> GetAllTramUpgradeLevelObjects()
	{
		return _tramUpgradeLevelObjects;
	}

	public List<(MapTrigger, Bounds)> GetCollectingVolumes()
	{
		List<(MapTrigger, Bounds)> list = new List<(MapTrigger, Bounds)>();
		foreach (var (mapTrigger, item) in boxTriggers)
		{
			if (mapTrigger.usageType == MapTrigger.eUsageType.Server_CollectingVolume)
			{
				list.Add((mapTrigger, item));
			}
		}
		return list;
	}

	public List<(MapTrigger, Bounds)> GetVerticalTrackVolumes()
	{
		List<(MapTrigger, Bounds)> list = new List<(MapTrigger, Bounds)>();
		foreach (var (mapTrigger, item) in boxTriggers)
		{
			if (mapTrigger.usageType == MapTrigger.eUsageType.Server_VerticalTrackVolume)
			{
				list.Add((mapTrigger, item));
			}
		}
		return list;
	}

	public List<(MapTrigger, Bounds)> GetCanopyVolumes()
	{
		List<(MapTrigger, Bounds)> list = new List<(MapTrigger, Bounds)>();
		foreach (var (mapTrigger, item) in boxTriggers)
		{
			if (mapTrigger.usageType == MapTrigger.eUsageType.Server_CanopyVolume)
			{
				list.Add((mapTrigger, item));
			}
		}
		return list;
	}

	public Dictionary<int, MapMarker_CreatureSpawnPoint> GetAllPlayerStartPoints()
	{
		return new Dictionary<int, MapMarker_CreatureSpawnPoint>(_playerStartPoints);
	}

	public Dictionary<int, MapMarker_CreatureSpawnPoint> GetAllMonsterSpawnPoints()
	{
		return new Dictionary<int, MapMarker_CreatureSpawnPoint>(_monsterSpawnPoints);
	}

	public Dictionary<int, MapMarker_LootingObjectSpawnPoint> GetAllLootingObjectSpawnPoints()
	{
		return new Dictionary<int, MapMarker_LootingObjectSpawnPoint>(_lootingObjectPoints);
	}

	public MapMarker_LootingObjectSpawnPoint? GetReinforceLootingObjectSpawnPoint()
	{
		foreach (MapMarker_LootingObjectSpawnPoint value in _lootingObjectPoints.Values)
		{
			if (value.spawnType == SpawnType.ReinforceItem)
			{
				return value;
			}
		}
		return null;
	}

	public Dictionary<int, MapMarker_FieldSkillSpawnPoint> GetAllFieldSkillSpawnPoints()
	{
		return new Dictionary<int, MapMarker_FieldSkillSpawnPoint>(_fieldSkillSpawnPoints);
	}

	public Dictionary<string, MapMarker_TeleportStartPoint> GetAllTeleportStartPoints()
	{
		return new Dictionary<string, MapMarker_TeleportStartPoint>(_teleportStartPoints);
	}

	public MapMarker_TeleportStartPoint? GetTeleportStartPoint(string callSign)
	{
		if (_teleportStartPoints.TryGetValue(callSign, out MapMarker_TeleportStartPoint value))
		{
			return value;
		}
		return null;
	}

	public List<MapMarker_TeleportStartPoint> GetTeleportStartPointByType(TeleportType type)
	{
		List<MapMarker_TeleportStartPoint> list = new List<MapMarker_TeleportStartPoint>();
		foreach (MapMarker_TeleportStartPoint value in _teleportStartPoints.Values)
		{
			if (value.TeleportType == type)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public Dictionary<string, MapMarker_TeleportEndPoint> GetAllTeleportEndPoints()
	{
		return _teleportEndPoints;
	}

	public MapMarker_TeleportEndPoint? GetTeleportEndPoint(string callSign)
	{
		if (_teleportEndPoints.TryGetValue(callSign, out MapMarker_TeleportEndPoint value))
		{
			return value;
		}
		return null;
	}

	public List<MapMarker_TeleportEndPoint> GetAllRandomTeleportEndPoints()
	{
		return _randomTeleportEndPoints;
	}

	public Dictionary<int, MapMarker_TargetPoint> GetAllTargetPoints()
	{
		return _targetPoints;
	}

	public Dictionary<int, LevelObject> GetAllLevelObjects(bool excludeClientOnly)
	{
		if (excludeClientOnly)
		{
			return _levelObjects.Where<KeyValuePair<int, LevelObject>>((KeyValuePair<int, LevelObject> x) => x.Value.ForServer).ToDictionary((KeyValuePair<int, LevelObject> x) => x.Key, (KeyValuePair<int, LevelObject> x) => x.Value);
		}
		return _levelObjects.ToDictionary<KeyValuePair<int, LevelObject>, int, LevelObject>((KeyValuePair<int, LevelObject> x) => x.Key, (KeyValuePair<int, LevelObject> x) => x.Value);
	}

	public List<VendingMachineLevelObject?> GetVendingMachineLevelObjects()
	{
		return (from x in _levelObjects.Values
			where x.crossHairType == CrosshairType.VendingMachine
			select x as VendingMachineLevelObject).ToList();
	}

	public MapMarker_CreatureSpawnPoint? GetPlayerStartPoint(int index)
	{
		if (index < 0 || index >= _playerStartPoints.Count)
		{
			return null;
		}
		if (_playerStartPoints.TryGetValue(index, out MapMarker_CreatureSpawnPoint value))
		{
			return value;
		}
		return null;
	}

	public List<Camera> GetCCTVCameras()
	{
		return _cctvCameras;
	}

	public List<BlackOut> GetBlackOutProps()
	{
		return _blackOutProps;
	}

	public bool CheckAABB(Vector3 position, out MapTrigger? trigger)
	{
		foreach (var (mapTrigger, bounds) in boxTriggers)
		{
			if (bounds.Contains(position))
			{
				trigger = mapTrigger;
				return true;
			}
		}
		trigger = null;
		return false;
	}

	public bool CheckSphere(Vector3 position, out MapTrigger? trigger)
	{
		foreach (var (mapTrigger, boundingSphere) in sphereTriggers)
		{
			if (Vector3.Distance(boundingSphere.position, position) < boundingSphere.radius)
			{
				trigger = mapTrigger;
				return true;
			}
		}
		trigger = null;
		return false;
	}

	public bool CheckAny(Vector3 position, out MapTrigger? trigger)
	{
		if (CheckAABB(position, out trigger))
		{
			return true;
		}
		if (CheckSphere(position, out trigger))
		{
			return true;
		}
		return false;
	}

	public void ReportTriggerEvent(MapTrigger trigger, string hitterAddress, bool isEnter)
	{
		triggerEvents.Add(new TriggerEvent
		{
			trigger = trigger,
			hitterAddress = hitterAddress,
			isEnter = isEnter
		});
	}

	public void DecodeTriggerEvent()
	{
		foreach (TriggerEvent triggerEvent in triggerEvents)
		{
			_ = triggerEvent;
		}
	}
}
