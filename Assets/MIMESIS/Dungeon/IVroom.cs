using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluServerBase.Threading;
using UnityEngine;

public abstract class IVroom : IDisposable
{
	public readonly VRoomManager RoomManager;

	public readonly long RoomID;

	public readonly IVRoomProperty Property;

	public readonly int MasterID;

	protected VActorDict<int, VActor> _vActorDict;

	protected VActorDict<int, VPlayer> _vPlayerDict;

	protected AtomicFlag _disposed = new AtomicFlag(value: false);

	protected ISpaceGroup _spaceGroup;

	protected CommandExecutor _commandExecutor;

	protected AtomicFlag _stopped = new AtomicFlag(value: false);

	protected AtomicFlag _blockEventAction = new AtomicFlag(value: false);

	protected ConcurrentQueue<SessionContext> _enterWaitSessions = new ConcurrentQueue<SessionContext>();

	protected EventTimer _eventTimer = new EventTimer();

	protected int _objectIDGenerator;

	protected bool _terminated;

	protected long _resumeTime;

	protected bool _spawnHoldDebug;

	protected bool _turnOnDebug;

	protected long _debugIntervalMillisec;

	protected long _waitThresholdTime;

	protected long _periodicAllMemberPktSentTime;

	protected int _playerSpawnPointIndex;

	protected Dictionary<int, SpawnPointData> _playerStartSpawnPoints = new Dictionary<int, SpawnPointData>();

	protected Dictionary<int, MapMarker_TargetPoint> _targetPoints = new Dictionary<int, MapMarker_TargetPoint>();

	protected Dictionary<int, SpawnedActorData> _spawnedActorDatas = new Dictionary<int, SpawnedActorData>();

	protected Dictionary<int, ILevelObjectInfo> _levelObjects = new Dictionary<int, ILevelObjectInfo>();

	protected List<int> _closedTramUpgradeObject = new List<int>();

	protected List<TriggerVolumeData> _playerStartingVolumes = new List<TriggerVolumeData>();

	protected List<TriggerVolumeData> _playerTriggerVolumes = new List<TriggerVolumeData>();

	protected List<LootingObjectSpawnVolumeData> _lootingObjectSpawnVolumes = new List<LootingObjectSpawnVolumeData>();

	protected List<VerticalTrackVolumeData> _verticalTrackVolumeDatas = new List<VerticalTrackVolumeData>();

	protected List<CanopyVolumeData> _canopyVolumes = new List<CanopyVolumeData>();

	protected Dictionary<long, long> _playerContas = new Dictionary<long, long>();

	protected int _currentDay = 1;

	protected int _currentDayDefered;

	protected int _currentSessionCount = 1;

	protected long _skillSyncIDGenerator;

	protected Dictionary<string, long> _soundClipDict = new Dictionary<string, long>();

	protected ConcurrentDictionary<int, ActionQueuePack> _eventActionGroups = new ConcurrentDictionary<int, ActionQueuePack>();

	protected Queue<Command> _onEmptyEventActionQueue = new Queue<Command>();

	protected Dictionary<string, CutSceneInfo> _cutSceneInfos = new Dictionary<string, CutSceneInfo>();

	protected List<ItemElement> _disappearedItems = new List<ItemElement>();

	protected List<IGameEventLog> _logDumps = new List<IGameEventLog>();

	protected long _currentTick;

	protected long _lastSyncTime;

	protected Dictionary<long, List<int>> _playerReservedItems = new Dictionary<long, List<int>>();

	protected Dictionary<int, PlayReportData> _playReportDict = new Dictionary<int, PlayReportData>();

	protected bool _startNotified;

	protected HashSet<ulong> _levelLoadCompleteActorIDs = new HashSet<ulong>();

	protected OnCreateRoomDelegate? _onLazyInitDelegate;

	protected List<int> _tramUpgradeList = new List<int>();

	protected Dictionary<int, ItemElement> _stashes = new Dictionary<int, ItemElement>();

	public int Currency { get; protected set; }

	public bool TurnOnDebug => _turnOnDebug;

	public long DebugIntervalMillisec
	{
		get
		{
			if (_debugIntervalMillisec <= 0)
			{
				return 30L;
			}
			return _debugIntervalMillisec;
		}
		set
		{
			_debugIntervalMillisec = ((value >= 30) ? value : 30);
		}
	}

	public SkyAndWeatherSystem.eWeatherPreset CurrentWeatherPreset { get; protected set; }

	public long SessionID => Property.SessionID;

	public int SessionCycleCount => _currentSessionCount;

	public int CurrentDay => _currentDay;

	public int BoostedItemMasterID { get; protected set; }

	public float BoostedItemRate { get; protected set; }

	public bool EnableAggroLog { get; private set; }

	public bool EnableAIController { get; protected set; } = true;

	public Dictionary<int, ItemElement> Stashes => _stashes;

	public List<int> TramUpgradeList => _tramUpgradeList;

	public IVroom(VRoomManager roomManager, long roomID, IVRoomProperty property, OnCreateRoomDelegate? onCreateRoomDelegate = null)
	{
		RoomManager = roomManager;
		RoomID = roomID;
		Property = property;
		_vActorDict = new VActorDict<int, VActor>(10000);
		_vPlayerDict = new VActorDict<int, VPlayer>(10);
		_commandExecutor = CommandExecutor.CreateCommandExecutor("VRoom", RoomID);
		_vActorDict.OnActorListChange += OnActorListChanged;
		_spaceGroup = LoadSpace(property.Min, property.Max);
		_resumeTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		_waitThresholdTime = Hub.s.timeutil.GetInfiniteTickMilliSec();
		_lastSyncTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		_currentTick = Hub.s.timeutil.GetCurrentTickMilliSec();
		CurrentWeatherPreset = Property.vRoomType switch
		{
			VRoomType.Maintenance => Hub.s.dataman.ExcelDataManager.Consts.C_DefaultWeather_Maintenance, 
			VRoomType.Waiting => Hub.s.dataman.ExcelDataManager.Consts.C_DefaultWeather_InTramWaiting, 
			VRoomType.Game => Hub.s.dataman.ExcelDataManager.Consts.C_DefaultWeather_Dungeon, 
			VRoomType.DeathMatch => Hub.s.dataman.ExcelDataManager.Consts.C_DefaultWeather_DeathMatch, 
			_ => throw new NotImplementedException($"CurrentWeatherPreset {Property.vRoomType}"), 
		};
	}

	public void SetLazyInitDelegate(OnCreateRoomDelegate onCreateRoomDelegate)
	{
		_onLazyInitDelegate = onCreateRoomDelegate;
	}

	public void InitLevel()
	{
		_levelObjects.Clear();
		_playerStartingVolumes.Clear();
		_playerTriggerVolumes.Clear();
		_lootingObjectSpawnVolumes.Clear();
		_canopyVolumes.Clear();
		_targetPoints.Clear();
		foreach (LevelObject value in Hub.s.dynamicDataMan.GetAllLevelObjects(excludeClientOnly: true).Values)
		{
			ILevelObjectInfo levelObjectInfo = null;
			switch (value.LevelObjectType)
			{
			case LevelObjectClientType.Switch:
			case LevelObjectClientType.Door:
			case LevelObjectClientType.Lever:
			case LevelObjectClientType.Trap:
			case LevelObjectClientType.ShutterSwitch:
			case LevelObjectClientType.LockerDoor:
			case LevelObjectClientType.GameSaver:
			case LevelObjectClientType.MapReroll:
			case LevelObjectClientType.PartyButton:
			case LevelObjectClientType.ScrapScan:
			case LevelObjectClientType.StashHanger:
				levelObjectInfo = new StateLevelObjectInfo(value);
				break;
			case LevelObjectClientType.Transmitter:
			case LevelObjectClientType.MomentarySwitch:
			case LevelObjectClientType.RandomTeleporter:
			case LevelObjectClientType.HornPullcord:
				levelObjectInfo = new OccupiedLevelObjectInfo(value);
				break;
			case LevelObjectClientType.Teleporter:
			case LevelObjectClientType.Electronics_05:
				levelObjectInfo = new TouchLevelObjectInfo(value);
				break;
			}
			if (levelObjectInfo != null)
			{
				_levelObjects.Add(value.levelObjectID, levelObjectInfo);
			}
		}
		foreach (var playerTriggerVolume in Hub.s.dynamicDataMan.GetPlayerTriggerVolumes())
		{
			if (playerTriggerVolume.Item1.usageType == MapTrigger.eUsageType.Server_StartingVolume)
			{
				_playerStartingVolumes.Add(new TriggerVolumeData(playerTriggerVolume.Item1, playerTriggerVolume.Item2));
			}
			else if (SimpleRandUtil.Next(0, 10001) <= playerTriggerVolume.Item1.activateRate)
			{
				_playerTriggerVolumes.Add(new TriggerVolumeData(playerTriggerVolume.Item1, playerTriggerVolume.Item2));
			}
		}
		foreach (var collectingVolume in Hub.s.dynamicDataMan.GetCollectingVolumes())
		{
			_lootingObjectSpawnVolumes.Add(new LootingObjectSpawnVolumeData(collectingVolume.Item1, collectingVolume.Item2));
		}
		foreach (var verticalTrackVolume in Hub.s.dynamicDataMan.GetVerticalTrackVolumes())
		{
			_verticalTrackVolumeDatas.Add(new VerticalTrackVolumeData(verticalTrackVolume.Item1, verticalTrackVolume.Item2));
		}
		foreach (var canopyVolume in Hub.s.dynamicDataMan.GetCanopyVolumes())
		{
			_canopyVolumes.Add(new CanopyVolumeData(canopyVolume.Item1, canopyVolume.Item2));
		}
		foreach (KeyValuePair<int, MapMarker_TargetPoint> allTargetPoint in Hub.s.dynamicDataMan.GetAllTargetPoints())
		{
			_targetPoints.Add(allTargetPoint.Key, allTargetPoint.Value);
		}
	}

	public void InitCutScenes()
	{
		_cutSceneInfos.Clear();
		List<DynamicDataManager.CutSceneData> cutSceneInfos = Hub.s.dynamicDataMan.GetCutSceneInfos();
		if (cutSceneInfos == null)
		{
			return;
		}
		foreach (DynamicDataManager.CutSceneData item in cutSceneInfos)
		{
			if (_cutSceneInfos.ContainsKey(item.name))
			{
				Logger.RWarn("InitCutScenes: duplicate cutscene name " + item.name + ", ignore");
			}
			else
			{
				_cutSceneInfos.Add(item.name, new CutSceneInfo(item.name, Hub.s.timeutil.ChangeTimeSec2Milli(item.duration)));
			}
		}
	}

	public virtual void InitSpawn()
	{
		_playerStartSpawnPoints.Clear();
		foreach (KeyValuePair<int, MapMarker_CreatureSpawnPoint> allPlayerStartPoint in Hub.s.dynamicDataMan.GetAllPlayerStartPoints())
		{
			_playerStartSpawnPoints.Add(allPlayerStartPoint.Key, new SpawnPointData(allPlayerStartPoint.Value.ID, allPlayerStartPoint.Value.pos, allPlayerStartPoint.Value.IsIndoor, 0, allPlayerStartPoint.Value.isFirstSpawnPoint));
		}
		Dictionary<int, MapMarker_CreatureSpawnPoint> allMonsterSpawnPoints = Hub.s.dynamicDataMan.GetAllMonsterSpawnPoints();
		Dictionary<int, MapMarker_LootingObjectSpawnPoint> allLootingObjectSpawnPoints = Hub.s.dynamicDataMan.GetAllLootingObjectSpawnPoints();
		Dictionary<int, MapMarker_FieldSkillSpawnPoint> allFieldSkillSpawnPoints = Hub.s.dynamicDataMan.GetAllFieldSkillSpawnPoints();
		_spawnedActorDatas.Clear();
		foreach (KeyValuePair<int, MapMarker_CreatureSpawnPoint> item in allMonsterSpawnPoints)
		{
			if (!_spawnedActorDatas.ContainsKey(item.Value.ID) && item.Value.masterID > 0)
			{
				_spawnedActorDatas.Add(item.Value.ID, new FixedSpawnedActorData(item.Value));
			}
		}
		foreach (KeyValuePair<int, MapMarker_LootingObjectSpawnPoint> item2 in allLootingObjectSpawnPoints)
		{
			if (!_spawnedActorDatas.ContainsKey(item2.Value.ID) && item2.Value.masterID > 0)
			{
				_spawnedActorDatas.Add(item2.Value.ID, new FixedSpawnedActorData(item2.Value));
			}
		}
		foreach (KeyValuePair<int, MapMarker_FieldSkillSpawnPoint> item3 in allFieldSkillSpawnPoints)
		{
			if (!_spawnedActorDatas.ContainsKey(item3.Value.ID))
			{
				_spawnedActorDatas.Add(item3.Value.ID, new FixedSpawnedActorData(item3.Value));
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing || !_disposed.On())
		{
			return;
		}
		foreach (VActor value in _vActorDict.Values)
		{
			value.Dispose();
		}
		foreach (VPlayer value2 in _vPlayerDict.Values)
		{
			value2.Dispose();
		}
		_vActorDict.Clear();
		_vPlayerDict.Clear();
		_spaceGroup?.Dispose();
		_commandExecutor.Dispose();
		_eventTimer.Dispose();
	}

	public static ISpaceGroup LoadSpace(Vector3 min, Vector3 max)
	{
		return new VSpaceSingleGroup(new SRect(new SPoint((int)min.x, (int)min.y), new SPoint((int)max.x, (int)max.y)));
	}

	private void OnActorListChanged(VActor actor, ActorListChangeEventArgs args)
	{
		switch (args.EventType)
		{
		case ActorListEventType.Add:
			OnActorEnter(actor);
			break;
		case ActorListEventType.Remove:
			OnActorExit(actor);
			break;
		}
	}

	protected virtual void OnActorEnter(VActor actor)
	{
		if (actor.ActorType != ActorType.Player && _spaceGroup != null && !_spaceGroup.AddObject(actor, actor.PositionVector))
		{
			Logger.RError($"OnActorEnter: {actor.ObjectID} {actor.ActorType} {actor.Position} is not in map");
		}
	}

	public virtual void OnActorExit(VActor actor)
	{
		if (_spaceGroup == null)
		{
			return;
		}
		_spaceGroup.RemoveObject(actor);
		if (actor.SpawnPointIndex != 0)
		{
			if (!_spawnedActorDatas.TryGetValue(actor.SpawnPointIndex, out SpawnedActorData value))
			{
				return;
			}
			value.OnActorDead();
		}
		if (actor.ActorType != ActorType.Player)
		{
			actor.Dispose();
		}
	}

	public void IterateAllPlayer(Action<VPlayer> action)
	{
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			action(value);
		}
	}

	public void IterateAllActor(Action<VActor> action)
	{
		foreach (VActor value in _vActorDict.Values)
		{
			action(value);
		}
	}

	public void IterateAllMonsterInRange(Vector3 centerPos, float range, Action<VMonster> action)
	{
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is VMonster vMonster && Vector3.Distance(vMonster.PositionVector, centerPos) <= range)
			{
				action(vMonster);
			}
		}
	}

	public void IterateAllMonster(Action<VActor> action)
	{
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is VMonster obj)
			{
				action(obj);
			}
		}
	}

	public virtual void OnCreatureAlive(VActor creature)
	{
		_ = creature is VMonster;
	}

	public virtual void OnCreatureDeath(VActor actor)
	{
	}

	public virtual void OnCreatureDying(VActor creature, VActor? attacker)
	{
	}

	public virtual void OnActorEvent(VActorEventArgs args)
	{
		switch (args.EventType)
		{
		case VActorEventType.Dead:
			if (!(args is GameActorDeadEventArgs e))
			{
				Logger.RError("[OnActorEvent] args is not GameActorDeadEventArgs");
			}
			else if (e.Victim != null)
			{
				TryReleaseOccupiedLevelObject(e.Victim.ObjectID);
			}
			break;
		case VActorEventType.ApplyCrowdControl:
			if (!(args is ApplyCrowdControlArgs applyCrowdControlArgs))
			{
				Logger.RError("[OnActorEvent] args is not applyCrowdControlArgs");
			}
			else if (applyCrowdControlArgs.Victim != null)
			{
				TryReleaseOccupiedLevelObject(applyCrowdControlArgs.Victim.ObjectID);
			}
			break;
		}
	}

	public List<VPlayer> GetPlayersInRange(Vector3 pos, float minRange, float maxRange, bool checkHeight = false)
	{
		List<VPlayer> list = new List<VPlayer>();
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (value.IsAliveStatus() && (!checkHeight || !(Mathf.Abs(value.PositionVector.y - pos.y) > (float)Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold)))
			{
				float num = Vector3.Distance(value.PositionVector, pos);
				if (num >= minRange && num <= maxRange)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<VCreature> GetCreaturesInRange(Vector3 pos, float minRange, float maxRange, bool checkHeight = false, VActor? excludeActor = null, bool collectDeadActor = false)
	{
		List<VCreature> list = new List<VCreature>();
		foreach (VActor value in _vActorDict.Values)
		{
			if ((collectDeadActor || value.IsAliveStatus()) && (excludeActor == null || value != excludeActor) && value is VCreature vCreature && (!checkHeight || !(Mathf.Abs(vCreature.PositionVector.y - pos.y) > (float)Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold)))
			{
				float num = Vector3.Distance(vCreature.PositionVector, pos);
				if (num >= minRange - vCreature.HitCollisionRadius && num <= maxRange + vCreature.HitCollisionRadius)
				{
					list.Add(vCreature);
				}
			}
		}
		foreach (VPlayer value2 in _vPlayerDict.Values)
		{
			if (value2.IsAliveStatus() && (excludeActor == null || value2 != excludeActor) && (!checkHeight || !(Mathf.Abs(value2.PositionVector.y - pos.y) > (float)Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold)))
			{
				float num2 = Vector3.Distance(value2.PositionVector, pos);
				if (num2 >= minRange - value2.HitCollisionRadius && num2 <= maxRange + value2.HitCollisionRadius)
				{
					list.Add(value2);
				}
			}
		}
		return list;
	}

	public List<VActor> GetActorsInRange(Vector3 pos, float minRange, float maxRange)
	{
		List<VActor> list = new List<VActor>();
		foreach (VActor value in _vActorDict.Values)
		{
			if (value.IsAliveStatus())
			{
				float num = Vector3.Distance(value.PositionVector, pos);
				if (num >= minRange && num <= maxRange)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<(VActor actor, double distance)> GetPlayerActorsInRange(Vector3 pos, float minRange, float maxRange, bool ignoreHeight = false)
	{
		List<(VActor, double)> list = new List<(VActor, double)>();
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (value.IsAliveStatus())
			{
				double num = VWorldUtil.Distance(value.PositionVector, pos, ignoreHeight);
				if (num >= (double)minRange && num <= (double)maxRange)
				{
					list.Add((value, num));
				}
			}
		}
		return list;
	}

	public List<FieldSkillObject> GetFieldSkillObjectsInRange(Vector3 pos, float minRange, float maxRange)
	{
		List<FieldSkillObject> list = new List<FieldSkillObject>();
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is FieldSkillObject fieldSkillObject)
			{
				float num = Vector3.Distance(fieldSkillObject.PositionVector, pos);
				if (num >= minRange && num <= maxRange)
				{
					list.Add(fieldSkillObject);
				}
			}
		}
		return list;
	}

	public VLootingObject? FindLootingObjectByItemID(long itemID)
	{
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is VLootingObject vLootingObject && vLootingObject.GetItemElement().ItemID == itemID)
			{
				return vLootingObject;
			}
		}
		return null;
	}

	public VActor? FindActorByObjectID(int objectID)
	{
		VActor vActor = _vActorDict.FindActor(objectID);
		if (vActor != null)
		{
			return vActor;
		}
		vActor = _vPlayerDict.FindActor(objectID);
		if (vActor != null)
		{
			return vActor;
		}
		return null;
	}

	public VPlayer? FindPlayerByUID(long playerUID)
	{
		return _vPlayerDict.Values.Where((VPlayer x) => x.UID == playerUID).FirstOrDefault();
	}

	public VPlayer? FindPlayerByObjectID(int objectID)
	{
		return _vPlayerDict.FindActor(objectID);
	}

	public virtual void SendToOther(IMsg msg, VActor? excludeActor)
	{
		foreach (VActor value in _vActorDict.Values)
		{
			if (excludeActor == null || value != excludeActor)
			{
				value.SendToMe(msg);
			}
		}
	}

	public virtual void SendToAllPlayers(IMsg msg, VActor? excludeActor = null)
	{
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (excludeActor == null || value != excludeActor)
			{
				value.SendToMe(msg);
			}
		}
	}

	public virtual void SendToAll(IMsg msg, VActor? excludeActor = null)
	{
		foreach (VActor value in _vActorDict.Values)
		{
			if (excludeActor == null || value != excludeActor)
			{
				value.SendToMe(msg);
			}
		}
		foreach (VPlayer value2 in _vPlayerDict.Values)
		{
			if (excludeActor == null || value2 != excludeActor)
			{
				value2.SendToMe(msg);
			}
		}
	}

	public void SendToTarget(int objectID, IMsg msg)
	{
		_vActorDict.FindActor(objectID)?.SendToMe(msg);
	}

	public MsgErrorCode EnterRoom(SessionContext context)
	{
		if (_stopped.IsOn)
		{
			return MsgErrorCode.RoomBlocked;
		}
		long playerUID = context.GetPlayerUID();
		if (_vPlayerDict.Values.Where((VPlayer x) => x.UID == playerUID).Any())
		{
			Logger.RError($"EnterRoom failed, player already exist. playerUID: {playerUID}");
			return MsgErrorCode.DuplicatePlayer;
		}
		if (_enterWaitSessions.Any((SessionContext x) => x.GetPlayerUID() == playerUID))
		{
			Logger.RError($"EnterRoom failed, player already in enter wait queue. playerUID: {playerUID}");
			return MsgErrorCode.DuplicatePlayer;
		}
		_enterWaitSessions.Enqueue(context);
		return MsgErrorCode.Success;
	}

	public void Stop(bool tickStop = true)
	{
		if (tickStop)
		{
			_stopped.On();
		}
		_eventActionGroups.Clear();
		RemoveAllFieldSkills();
	}

	public void RemoveAllFieldSkills()
	{
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is FieldSkillObject)
			{
				PendRemoveActor(value.ObjectID);
			}
		}
	}

	public void BlockEventAction()
	{
		_blockEventAction.On();
	}

	public virtual void OnUpdate(long delta)
	{
		try
		{
			_currentTick += delta;
			_eventTimer.Update();
			_commandExecutor.Execute();
			PumpEventAction();
			ManageSoundPlayData();
			SyncRoomInfo(delta);
			ManagePlayCutScene();
			if (_terminated || _stopped.IsOn)
			{
				return;
			}
			ProcessEnterWaitQueue();
			ManageSpawnData();
			foreach (VPlayer value in _vPlayerDict.Values)
			{
				try
				{
					value.Update(delta);
				}
				catch (Exception arg)
				{
					Logger.RError($"OnUpdate Player Update Exception: PlayerID={value.ObjectID}, Exception={arg}");
				}
			}
			foreach (VActor value2 in _vActorDict.Values)
			{
				try
				{
					value2.Update(delta);
				}
				catch (Exception arg2)
				{
					Logger.RError($"OnUpdate Actor Update Exception: ActorID={value2.ObjectID}, Exception={arg2}");
				}
			}
			if (!_startNotified && _waitThresholdTime + 40000 >= Hub.s.timeutil.GetCurrentTickMilliSec())
			{
				int roomTypeMemberCount = Hub.s.vworld.GetRoomTypeMemberCount(Property.vRoomType);
				if (_levelLoadCompleteActorIDs.Count == roomTypeMemberCount)
				{
					OnAllMemberEntered();
				}
			}
			else if (_periodicAllMemberPktSentTime + 10000 < Hub.s.timeutil.GetCurrentTickMilliSec())
			{
				_periodicAllMemberPktSentTime = Hub.s.timeutil.GetCurrentTickMilliSec();
				OnAllMemberEntered();
			}
		}
		catch (Exception arg3)
		{
			Logger.RError($"OnUpdate Exception: {arg3}");
		}
	}

	public virtual void SyncRoomInfo(long delta)
	{
		if (_lastSyncTime + 5000 > _currentTick)
		{
			return;
		}
		_lastSyncTime = _currentTick;
		RoomStatusSig roomStatusSig = new RoomStatusSig
		{
			currency = Currency
		};
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			roomStatusSig.playerStatusInfos.Add(value.GetPlayerStatusInfo());
		}
		SendToAll(roomStatusSig);
	}

	public virtual void ManageSpawnData()
	{
		if (_spawnHoldDebug)
		{
			return;
		}
		foreach (SpawnedActorData value in _spawnedActorDatas.Values)
		{
			if (value.ActorID != 0)
			{
				continue;
			}
			if (value.MarkerType == MapMarkerType.Creature && value.SpawnType == SpawnType.OnStartMap)
			{
				if (value.CanSpawn() && value.MasterID != 0 && !SpawnMonster(value.MasterID, value, value.IsIndoor, value.AIName, value.BTName))
				{
					Logger.RError($"ManageSpawnData failed, SpawnMonster failed. masterID: {value.MasterID}");
				}
			}
			else
			{
				if (value.MarkerType != MapMarkerType.LootingObject || value.SpawnType != SpawnType.OnStartMap || !value.CanSpawn() || value.MasterID == 0)
				{
					continue;
				}
				ItemElement newItemElement = GetNewItemElement(value.MasterID, isFake: false, value.StackCount, value.Durability, value.DefaultGauge);
				if (newItemElement == null)
				{
					Logger.RError($"ManageSpawnData failed, targetItem is null. masterID: {value.MasterID}");
					continue;
				}
				int num = SpawnLootingObject(newItemElement, value.Pos, value.IsIndoor, ReasonOfSpawn.Spawn, value.Index);
				if (num == 0)
				{
					Logger.RError($"ManageSpawnData failed, SpawnLootingObject failed. masterID: {value.MasterID}");
				}
				else
				{
					value.SetActorID(num);
				}
			}
		}
	}

	public ItemElement? GetNewItemElement(int itemMasterID, bool isFake, int itemCount = 1, int durability = 0, int gauge = 0, int price = 0)
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
		if (itemInfo == null)
		{
			Logger.RError($"GetNewItemElement failed, GetItemInfo failed. itemMasterID: {itemMasterID}");
			return null;
		}
		switch (itemInfo.ItemType)
		{
		case ItemType.Consumable:
			return new ConsumableItemElement(itemInfo.MasterID, GetNewItemID(), isFake, itemCount, price);
		case ItemType.Equipment:
			if (!(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
			{
				Logger.RError($"TriggerSpawnByEvent failed, itemInfo is not ItemEquipmentInfo. masterID: {itemInfo.MasterID}");
				return null;
			}
			return new EquipmentItemElement(itemInfo.MasterID, GetNewItemID(), isFake, (durability == 0) ? SimpleRandUtil.Next(itemEquipmentInfo.MinDurability, itemEquipmentInfo.MaxDurability) : durability, (gauge == 0) ? itemEquipmentInfo.InitialGauge : gauge, price);
		case ItemType.Miscellany:
			return new MiscellanyItemElement(itemInfo.MasterID, GetNewItemID(), isFake, price);
		default:
			return null;
		}
	}

	public ItemElement CloneNewFakeItemElement(ItemElement origin)
	{
		switch (origin.ItemType)
		{
		case ItemType.Consumable:
		{
			ConsumableItemElement consumableItemElement = origin as ConsumableItemElement;
			return new ConsumableItemElement(consumableItemElement.ItemMasterID, GetNewItemID(), isFake: true, consumableItemElement.RemainCount);
		}
		case ItemType.Equipment:
		{
			EquipmentItemElement equipmentItemElement = origin as EquipmentItemElement;
			return new EquipmentItemElement(equipmentItemElement.ItemMasterID, GetNewItemID(), isFake: true, equipmentItemElement.RemainDurability, equipmentItemElement.RemainAmount);
		}
		case ItemType.Miscellany:
			return new MiscellanyItemElement((origin as MiscellanyItemElement).ItemMasterID, GetNewItemID(), isFake: true);
		default:
			throw new Exception($"CloneNewFakeItemElement failed, invalid ItemType. ItemType: {origin.ItemType}");
		}
	}

	public int SpawnLootingObject(ItemElement element, PosWithRot pos, bool isIndoor, ReasonOfSpawn reasonOfSpawn, int spawnPointIndex = 0)
	{
		VLootingObject vLootingObject = new VLootingObject(GetNewObjectID(), pos, isIndoor, element, this, reasonOfSpawn);
		if (!PendAddActor(vLootingObject))
		{
			Logger.RError($"ManageSpawnData failed, AddActor failed. masterID: {element.ItemMasterID}");
			vLootingObject.Dispose();
			return 0;
		}
		element.SetParent(null);
		vLootingObject.SetSpawnPointIndex(spawnPointIndex);
		return vLootingObject.ObjectID;
	}

	protected virtual MsgErrorCode CanEnterChannel(long playerUID)
	{
		if (_vPlayerDict.Values.Where((VPlayer x) => x.UID == playerUID).Any())
		{
			Logger.RError($"ProcessEnterWaitQueue failed, player already exist. playerUID: {playerUID}");
			return MsgErrorCode.DuplicatePlayer;
		}
		if (_vPlayerDict.Count >= Hub.s.dataman.ExcelDataManager.Consts.C_MaxPlayerCount)
		{
			Logger.RError($"ProcessEnterWaitQueue failed, player count is over limit. playerUID: {playerUID}");
			return MsgErrorCode.PlayerCountExceeded;
		}
		return MsgErrorCode.Success;
	}

	protected virtual SpawnPointData? GetPlayerStartPoint()
	{
		int key = _playerSpawnPointIndex++ % _playerStartSpawnPoints.Count;
		if (_playerStartSpawnPoints.TryGetValue(key, out SpawnPointData value))
		{
			return value;
		}
		return null;
	}

	private void ProcessEnterWaitQueue()
	{
		SessionContext result;
		while (_enterWaitSessions.TryDequeue(out result))
		{
			MsgErrorCode msgErrorCode = MsgErrorCode.Success;
			int enterPktHashCode = result.EnterPktHashCode;
			result.SetEnterPacketHashCode(0);
			try
			{
				long playerUID = result.GetPlayerUID();
				msgErrorCode = CanEnterChannel(playerUID);
				if (msgErrorCode != MsgErrorCode.Success)
				{
					continue;
				}
				SpawnPointData playerStartPoint = GetPlayerStartPoint();
				if (playerStartPoint == null)
				{
					Logger.RError($"ProcessEnterWaitQueue failed, GetPlayerStartPoint failed. sessionID: {result.GetSessionID()}");
					msgErrorCode = MsgErrorCode.PlayerStartPointNotFound;
					continue;
				}
				VPlayer vPlayer = result.CreatePlayer(GetNewObjectID(), this, playerStartPoint.Pos, playerStartPoint.IsInDoor);
				if (vPlayer == null)
				{
					Logger.RError($"ProcessEnterWaitQueue failed, CreatePlayer failed. sessionID: {result.GetSessionID()}");
					msgErrorCode = MsgErrorCode.CreatePlayerFailed;
				}
				else
				{
					msgErrorCode = AddPlayer(vPlayer, enterPktHashCode);
				}
			}
			finally
			{
				if (msgErrorCode != MsgErrorCode.Success)
				{
					OnEnterRoomFailed(result, msgErrorCode, enterPktHashCode);
				}
			}
		}
	}

	public int GetNewObjectID()
	{
		return Interlocked.Increment(ref _objectIDGenerator);
	}

	public virtual MsgErrorCode OnEnterChannel(VPlayer player, int hashCode)
	{
		return MsgErrorCode.Success;
	}

	public void OnLevelLoadComplete(VPlayer player)
	{
		_levelLoadCompleteActorIDs.Add(player.SteamID);
		_waitThresholdTime = Hub.s.timeutil.GetCurrentTickMilliSec();
	}

	public virtual void OnExitChannel(VPlayer player)
	{
		foreach (CutSceneInfo value in _cutSceneInfos.Values)
		{
			value.RemoveParticipant(player.ObjectID);
		}
		SendToAllPlayers(new LeaveRoomSig
		{
			actorID = player.ObjectID
		});
	}

	public void OnActorChangeBT(VCreature creature)
	{
	}

	public virtual void OnCreatureAlive(VCreature creature)
	{
		_ = creature is VMonster;
	}

	public virtual void OnCreatureDeath(VCreature creature)
	{
	}

	public virtual void OnCreatureDying(VCreature creature, VCreature? attacker)
	{
	}

	public bool MoveObject(VActor actor, Vector3 oldPos, Vector3 newPos, ActorMoveCause cause)
	{
		if (_spaceGroup == null)
		{
			return false;
		}
		OnActorMove(actor, oldPos, newPos);
		actor.OnMovePosition(oldPos, newPos, cause);
		if (actor.VSpace != null && !_spaceGroup.MoveObject(actor, newPos))
		{
			actor.OnMoveFailed(oldPos, newPos);
			return false;
		}
		return true;
	}

	public bool ValidPosition(Vector3 position)
	{
		return _spaceGroup?.GetSpace(position) != null;
	}

	public VMonster? CreateMonster(int masterID, PosWithRot pos, bool isIndoor, string aiName = "", string btName = "", ReasonOfSpawn reasonOfSpawn = ReasonOfSpawn.Spawn)
	{
		MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(masterID);
		if (monsterInfo == null)
		{
			return null;
		}
		VMonster vMonster = new VMonster(masterID, GetNewObjectID(), monsterInfo.Name, pos, isIndoor, this, aiName, btName, reasonOfSpawn);
		if (!PendAddActor(vMonster))
		{
			vMonster.Dispose();
			return null;
		}
		return vMonster;
	}

	public List<FieldSkillObject> CreateFieldSkillObject(int masterID, PosWithRot pos, bool isIndoor, Vector3? surfaceNormal, ISkillContext? context, ReasonOfSpawn reasonOfSpawn)
	{
		List<FieldSkillObject> list = new List<FieldSkillObject>();
		FieldSkillInfo fieldSkillData = Hub.s.dataman.ExcelDataManager.GetFieldSkillData(masterID);
		if (fieldSkillData == null)
		{
			return list;
		}
		Quaternion quaternion = Quaternion.Euler(0f, pos.yaw, 0f);
		foreach (KeyValuePair<int, FieldSkillMemberInfo> fieldSkillMemberInfo in fieldSkillData.FieldSkillMemberInfos)
		{
			PosWithRot posWithRot = pos.Clone();
			Vector3 vector = new Vector3(fieldSkillMemberInfo.Value.FieldOffset.x, fieldSkillMemberInfo.Value.FieldOffset.y, fieldSkillMemberInfo.Value.FieldOffset.z);
			Vector3 vector2 = quaternion * vector;
			posWithRot.x += vector2.x;
			posWithRot.y += vector2.y;
			posWithRot.z += vector2.z;
			FieldSkillObject fieldSkillObject = new FieldSkillObject(GetNewObjectID(), masterID, fieldSkillMemberInfo.Key, "", posWithRot, isIndoor, surfaceNormal, this, context, reasonOfSpawn);
			if (!PendAddActor(fieldSkillObject))
			{
				fieldSkillObject.Dispose();
			}
			else
			{
				list.Add(fieldSkillObject);
			}
		}
		return list;
	}

	public VProjectileObject? CreateProjectileObject(int masterID, PosWithRot pos, bool isIndoor, ISkillContext? context, ReasonOfSpawn reasonOfSpawn)
	{
		if (Hub.s.dataman.ExcelDataManager.GetProjectileInfo(masterID) == null)
		{
			return null;
		}
		int newObjectID = GetNewObjectID();
		VProjectileObject vProjectileObject = new VProjectileObject(newObjectID, masterID, $"Projectile{masterID}:{newObjectID}", pos, isIndoor, this, context, reasonOfSpawn);
		if (!PendAddActor(vProjectileObject))
		{
			Logger.RError($"CreateProjectileObject() AddActor failed. masterID: {newObjectID}");
			vProjectileObject.Dispose();
			return null;
		}
		return vProjectileObject;
	}

	protected bool AddActor(VActor actor)
	{
		if (_vActorDict.AddActor(actor.ObjectID, actor) != VWorldErrorCode.None)
		{
			return false;
		}
		return true;
	}

	protected bool PendAddActor(VActor actor)
	{
		_commandExecutor.Invoke(delegate
		{
			AddActor(actor);
		});
		return true;
	}

	public MsgErrorCode AddPlayer(VPlayer player, int hashCode)
	{
		int num = (from x in _vPlayerDict
			where x.Value.SteamID == player.SteamID
			select x.Key).FirstOrDefault();
		if (num != 0)
		{
			PendRemoveActor(num);
		}
		if (_vPlayerDict.AddActor(player.ObjectID, player) != VWorldErrorCode.None)
		{
			return MsgErrorCode.DuplicatePlayer;
		}
		if (!_playReportDict.ContainsKey(player.ObjectID))
		{
			_playReportDict.Add(player.ObjectID, new PlayReportData(player.ObjectID));
		}
		return OnEnterChannel(player, hashCode);
	}

	public bool RemovePlayer(int actorID, bool backupFlag)
	{
		VPlayer vPlayer = FindPlayerByObjectID(actorID);
		if (vPlayer == null)
		{
			return false;
		}
		vPlayer.SetToMoveRoomType(GetToMoveRoomType());
		_levelLoadCompleteActorIDs.Remove(vPlayer.SteamID);
		OnExitChannel(vPlayer);
		if (!_vPlayerDict.RemoveActor(actorID))
		{
			return false;
		}
		if (_playReportDict.ContainsKey(vPlayer.ObjectID))
		{
			_playReportDict.Remove(vPlayer.ObjectID);
		}
		ExitSpace(vPlayer);
		vPlayer.OnExitChannel(backupFlag);
		_playerContas.Remove(vPlayer.UID);
		vPlayer.Dispose();
		return true;
	}

	public void PendRemovePlayer(int actorID, bool backup, bool kill)
	{
		_commandExecutor.Invoke(delegate
		{
			if (kill)
			{
				VPlayer vPlayer = FindPlayerByObjectID(actorID);
				if (vPlayer != null && vPlayer.IsAliveStatus())
				{
					vPlayer.StatControlUnit?.AdjustHP(0L, full: true);
				}
				_eventTimer.CreateTimerEvent(delegate
				{
					RemovePlayer(actorID, backup);
				}, vPlayer?.DyingWaitTime ?? 1000);
			}
			else
			{
				RemovePlayer(actorID, backup);
			}
		});
	}

	public void PendRemoveActor(int objectID)
	{
		_commandExecutor.Invoke(delegate
		{
			RemoveActor(objectID);
		});
	}

	public void DestroyFakeObject(int objectID)
	{
		SendToAllPlayers(new DestroyActorSig
		{
			actorID = objectID
		});
		PendRemoveActor(objectID);
	}

	private bool RemoveActor(int obectID)
	{
		VActor vActor = FindActorByObjectID(obectID);
		if (vActor == null)
		{
			return false;
		}
		if (!_vActorDict.RemoveActor(obectID))
		{
			return false;
		}
		vActor.Dispose();
		return true;
	}

	public bool HasNearestPoly(Vector3 pos)
	{
		return Hub.s.vworld?.HasNearestPoly(pos) ?? false;
	}

	public Vector3 GetReachableDistancePos(Vector3 startPos, double angle, float distance)
	{
		Vector3 posWithAngleDistance = Misc.GetPosWithAngleDistance(startPos, angle, distance);
		if (!FindNearestPoly(posWithAngleDistance, out var nearestPos, 2.5f) && !FindNearestPoly(startPos, out nearestPos, 2.5f))
		{
			return startPos;
		}
		if (!MoveAlongSurface(startPos, nearestPos, out nearestPos, ignore: true))
		{
			return startPos;
		}
		return nearestPos;
	}

	public bool FindNearestPoly(Vector3 pos, out Vector3 nearestPos, float maxDistance = 100f)
	{
		if (Hub.s?.vworld == null)
		{
			nearestPos = Vector3.zero;
			return false;
		}
		nearestPos = Hub.s.vworld.FindNearestPoly(pos, maxDistance);
		if (nearestPos == Vector3.zero)
		{
			return false;
		}
		return true;
	}

	public bool FindNearestPositionInDistance(Vector3 pos, float distance, out Vector3 nearestPos)
	{
		if (Hub.s.vworld == null)
		{
			nearestPos = Vector3.zero;
			return false;
		}
		nearestPos = Hub.s.vworld.FindNearestPositionInDistance(pos, distance);
		if (nearestPos == Vector3.zero)
		{
			return false;
		}
		return true;
	}

	public bool FindPath(Vector3 startPos, Vector3 endPos, out List<Vector3>? path)
	{
		if (Hub.s.vworld == null)
		{
			path = null;
			return false;
		}
		if (!Hub.s.vworld.FindPath(startPos, endPos, out path))
		{
			path = null;
			return false;
		}
		return true;
	}

	public bool VerticalRaycast(Vector3 startPos, float maxDistance, bool upDir, out Vector3 nearestPoint)
	{
		if (Hub.s.vworld == null)
		{
			nearestPoint = Vector3.zero;
			return false;
		}
		return Hub.s.vworld.VerticalRaycast(startPos, maxDistance, upDir, out nearestPoint);
	}

	public double GetDistanceByNavMesh(Vector3 startPos, Vector3 endPos)
	{
		if (Hub.s.vworld == null)
		{
			throw new Exception("VWorld is null");
		}
		return Hub.s.vworld.GetNavDistance(startPos, endPos);
	}

	public bool MoveAlongSurface(Vector3 startPos, Vector3 endPos, out Vector3 resultPos, bool ignore)
	{
		if (Hub.s.vworld == null)
		{
			resultPos = Vector3.zero;
			return false;
		}
		resultPos = Hub.s.vworld.MoveAlongSurface(startPos, endPos, ignore);
		if (resultPos == Vector3.zero)
		{
			return false;
		}
		return true;
	}

	public bool MoveAlongSurfaceSequentially(Vector3 startPos, List<Vector3> moves, out List<Vector3> movedPositions)
	{
		if (Hub.s.vworld == null)
		{
			movedPositions = new List<Vector3>();
			return false;
		}
		return Hub.s.vworld.MoveAlongSurfaceSequentially(startPos, moves, out movedPositions);
	}

	public bool IsHitWall(Vector3 startPos, Vector3 endPos, out Vector3 hitPos)
	{
		if (Hub.s.vworld == null)
		{
			hitPos = Vector3.zero;
			return false;
		}
		return Hub.s.vworld.IsHitWall(startPos, endPos, out hitPos);
	}

	public float DistanceToWall(Vector3 pos, Vector3 dir, float maxRadius)
	{
		if (Hub.s.vworld == null)
		{
			return 0f;
		}
		return Hub.s.vworld.DistanceToWall(pos, dir, maxRadius);
	}

	public bool GetRandomReachablePointInRadius(Vector3 pos, float minRad, float maxRad, out Vector3 resultPos)
	{
		if (Hub.s.vworld == null)
		{
			resultPos = Vector3.zero;
			return false;
		}
		return Hub.s.vworld.GetRandomReachablePointInRadius(pos, minRad, maxRad, out resultPos);
	}

	public bool EnterSpace(VActor actor)
	{
		return _spaceGroup.AddObject(actor, actor.PositionVector);
	}

	public void ExitSpace(VActor actor)
	{
		_spaceGroup.RemoveObject(actor);
	}

	public IVRoomProperty GetProperty()
	{
		return Property;
	}

	public void Shutdown()
	{
		_commandExecutor.Invoke(delegate
		{
			Vacate(backup: true);
			_terminated = true;
		});
	}

	protected void Vacate(bool backup)
	{
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			PendRemovePlayer(value.ObjectID, backup, kill: false);
		}
		OnVacateRoom(backup);
	}

	public virtual bool Initialize()
	{
		InitSpawn();
		InitLevel();
		InitCutScenes();
		return true;
	}

	public virtual void ApplyBaseGameSessionInfo(GameSessionInfo gameSessionInfo)
	{
		_playerContas.Clear();
		SpawnLootingObjectsFromGameSessionInfo(gameSessionInfo);
		foreach (KeyValuePair<long, long> playerContum in gameSessionInfo.PlayerConta)
		{
			_playerContas.Add(playerContum.Key, playerContum.Value);
		}
		_playerReservedItems.Clear();
		foreach (KeyValuePair<long, List<int>> item in gameSessionInfo.ItemsToProvide)
		{
			if (!_playerReservedItems.TryGetValue(item.Key, out List<int> value))
			{
				value = new List<int>();
				_playerReservedItems.Add(item.Key, value);
			}
			value.AddRange(item.Value);
		}
		_currentDay = gameSessionInfo.CurrentGameCount;
		_currentSessionCount = gameSessionInfo.IterationCount;
		BoostedItemMasterID = gameSessionInfo.BoostedItemMasterID;
		BoostedItemRate = gameSessionInfo.BoostedRate;
	}

	protected void SpawnLootingObjectsFromGameSessionInfo(GameSessionInfo gameSessionInfo)
	{
		foreach (VActor item in _vActorDict.Values.Where((VActor x) => x is VLootingObject).ToList())
		{
			PendRemoveActor(item.ObjectID);
		}
		if (this is DeathMatchRoom)
		{
			return;
		}
		LootingObjectSpawnVolumeData lootingObjectSpawnVolumeData = _lootingObjectSpawnVolumes.FirstOrDefault();
		if (lootingObjectSpawnVolumeData == null)
		{
			Logger.RError("[SpawnLootingObjectsFromGameSessionInfo] collectingVolumeData is missing");
			return;
		}
		_stashes.Clear();
		foreach (KeyValuePair<int, ItemElement> stash in gameSessionInfo.Stashes)
		{
			_stashes[stash.Key] = stash.Value.Clone();
		}
		_tramUpgradeList.Clear();
		_tramUpgradeList.AddRange(gameSessionInfo.TramUpgradeList);
		_closedTramUpgradeObject.Clear();
		foreach (KeyValuePair<int, LevelObject> allTramUpgradeLevelObject in Hub.s.dynamicDataMan.GetAllTramUpgradeLevelObjects())
		{
			if (allTramUpgradeLevelObject.Value is ITramUpgradeLevelObject tramUpgradeLevelObject && !_tramUpgradeList.Contains(tramUpgradeLevelObject.TramUpgradeID))
			{
				_closedTramUpgradeObject.Add(allTramUpgradeLevelObject.Key);
			}
		}
		foreach (DrainLootingObjectInfo lootingObject in gameSessionInfo.LootingObjects)
		{
			PosWithRot posWithRot = lootingObject.PosWithRot.Clone();
			if (lootingObjectSpawnVolumeData != null)
			{
				Quaternion quaternion = Quaternion.Euler(0f, lootingObjectSpawnVolumeData.Rotation.eulerAngles.y, 0f);
				Vector3 position = lootingObjectSpawnVolumeData.Position + quaternion * posWithRot.toVector3();
				float y = position.y;
				Vector3 nearestPointOnNavMesh = Hub.s.navman.GetNearestPointOnNavMesh(position, 1f);
				if (nearestPointOnNavMesh != Vector3.zero)
				{
					y = nearestPointOnNavMesh.y;
				}
				posWithRot.x = position.x;
				posWithRot.y = y;
				posWithRot.z = position.z;
				posWithRot.yaw += quaternion.eulerAngles.y;
			}
			else
			{
				Logger.RWarn($"[LootingObjects] RoomMasterID:{MasterID} has NO collectingVolume!");
			}
			SpawnLootingObject(lootingObject.ItemElement, posWithRot, isIndoor: false, lootingObject.ReasonOfSpawn);
		}
	}

	public virtual void OnVacateRoom(bool backup)
	{
		List<VActor> list = _vActorDict.Values.Where((VActor x) => x is VLootingObject).ToList();
		_waitThresholdTime = Hub.s.timeutil.GetInfiniteTickMilliSec();
		foreach (VActor item in list)
		{
			PendRemoveActor(item.ObjectID);
		}
	}

	public void GetLevelObjectInfos(ref LevelLoadCompleteRes res)
	{
		foreach (ILevelObjectInfo value in _levelObjects.Values)
		{
			res.levelObjects.Add(value.toLevelObjectInfo());
		}
	}

	public CommandExecutor GetCommandExecutor()
	{
		return _commandExecutor;
	}

	public long GetNewSkillSyncID()
	{
		return Interlocked.Increment(ref _skillSyncIDGenerator);
	}

	public long GetNewItemID()
	{
		if (RoomManager == null)
		{
			Logger.RError("[IVRoom] GetNewItemID(). RoomManager is null");
			return 0L;
		}
		return RoomManager.GetNewItemID();
	}

	public List<VActor> GetActorsInRange(VActor owner, float range, bool includeSelf)
	{
		VSpace[] allAroundSectors = _spaceGroup.GetAllAroundSectors();
		List<VActor> result = new List<VActor>();
		VSpace[] array = allAroundSectors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].IterateObject(delegate(ISpaceActor spaceActor)
			{
				if ((spaceActor != owner || includeSelf) && spaceActor is VActor vActor && Vector3.Distance(owner.PositionVector, vActor.PositionVector) <= range)
				{
					result.Add(vActor);
				}
			});
		}
		return result;
	}

	public List<VCreature> GetCreaturesInRange(VActor owner, float range)
	{
		VSpace[] allAroundSectors = _spaceGroup.GetAllAroundSectors();
		List<VCreature> result = new List<VCreature>();
		VSpace[] array = allAroundSectors;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].IterateObject(delegate(ISpaceActor spaceActor)
			{
				if (spaceActor != owner && spaceActor is VCreature vCreature && Vector3.Distance(owner.PositionVector, vCreature.PositionVector) <= range)
				{
					result.Add(vCreature);
				}
			});
		}
		return result;
	}

	public (List<VActor> preserveObjects, List<VActor> removeCandidateObjects) ClassifyLootingObjectsInCollectingVolume()
	{
		List<VActor> list = new List<VActor>();
		List<VActor> list2 = new List<VActor>();
		LootingObjectSpawnVolumeData lootingObjectSpawnVolumeData = _lootingObjectSpawnVolumes.FirstOrDefault();
		if (lootingObjectSpawnVolumeData == null)
		{
			return (preserveObjects: list, removeCandidateObjects: list2);
		}
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is VLootingObject vLootingObject && lootingObjectSpawnVolumeData.Bounds.Contains(value.PositionVector))
			{
				ItemElement itemElement = vLootingObject.GetItemElement();
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemElement.ItemMasterID);
				if (itemInfo == null)
				{
					Logger.RError($"ClassifyLootingObjects failed, GetItemInfo failed. itemMasterID: {itemElement.ItemMasterID}");
				}
				else if (itemInfo.PreserveOnWipe)
				{
					list.Add(vLootingObject);
				}
				else
				{
					list2.Add(vLootingObject);
				}
			}
		}
		return (preserveObjects: list, removeCandidateObjects: list2);
	}

	private (List<VActor> toRemove, List<VActor> toPreserve) DetermineObjectsToRemove(List<VActor> removeCandidateObjects)
	{
		List<VActor> objectsToRemove = new List<VActor>();
		List<VActor> list = new List<VActor>();
		if (removeCandidateObjects.Count == 0)
		{
			return (toRemove: objectsToRemove, toPreserve: list);
		}
		if (IsAllPlayerWastedOrDead())
		{
			objectsToRemove.AddRange(removeCandidateObjects);
			return (toRemove: objectsToRemove, toPreserve: list);
		}
		int num = _vPlayerDict.Values.Count((VPlayer x) => x.ReasonOfDeath == ReasonOfDeath.DungeonEnd || x.Wasted);
		if (num <= 0)
		{
			list.AddRange(removeCandidateObjects);
			return (toRemove: objectsToRemove, toPreserve: list);
		}
		objectsToRemove = removeCandidateObjects.Take(num).ToList();
		list = removeCandidateObjects.Where((VActor x) => !objectsToRemove.Contains(x)).ToList();
		return (toRemove: objectsToRemove, toPreserve: list);
	}

	protected virtual RoomDrainInfo ExtractRoomInfo(bool includeDisappearItem = true)
	{
		RoomDrainInfo roomDrainInfo = new RoomDrainInfo(RoomID, (_currentDayDefered != 0) ? _currentDayDefered : ((this is DungeonRoom) ? (_currentDay + 1) : _currentDay));
		if (_currentDayDefered != 0)
		{
			_currentDayDefered = 0;
		}
		roomDrainInfo.SetBoostedItem(BoostedItemMasterID, BoostedItemRate);
		(List<VActor> preserveObjects, List<VActor> removeCandidateObjects) tuple = ClassifyLootingObjectsInCollectingVolume();
		List<VActor> item = tuple.preserveObjects;
		List<VActor> item2 = tuple.removeCandidateObjects;
		List<VActor> item3 = DetermineObjectsToRemove(item2).toPreserve;
		if (includeDisappearItem)
		{
			ProcessDisappearedItems();
		}
		List<VActor> mergedObjects = item.Concat(item3).ToList();
		AddLootingObjectsToResult(roomDrainInfo, mergedObjects);
		roomDrainInfo.SetStash(_stashes);
		roomDrainInfo.SetTramUpgradeList(_tramUpgradeList);
		AddPlayerContaToResult(roomDrainInfo);
		return roomDrainInfo;
	}

	protected void ProcessDisappearedItems()
	{
		_disappearedItems.Clear();
		List<VActor> item = ClassifyLootingObjectsInCollectingVolume().removeCandidateObjects;
		List<VActor> item2 = DetermineObjectsToRemove(item).toRemove;
		foreach (VActor item3 in item2)
		{
			if (item3 is VLootingObject vLootingObject)
			{
				_disappearedItems.Add(vLootingObject.GetItemElement());
			}
		}
	}

	private void AddLootingObjectsToResult(RoomDrainInfo result, List<VActor> mergedObjects)
	{
		foreach (VActor mergedObject in mergedObjects)
		{
			foreach (LootingObjectSpawnVolumeData lootingObjectSpawnVolume in _lootingObjectSpawnVolumes)
			{
				if (lootingObjectSpawnVolume.Bounds.Contains(mergedObject.PositionVector))
				{
					Quaternion rotation = Quaternion.Euler(0f, lootingObjectSpawnVolume.Rotation.eulerAngles.y, 0f);
					Vector3 vector = Quaternion.Inverse(rotation) * (mergedObject.Position.toVector3() - lootingObjectSpawnVolume.Position);
					PosWithRot posWithRot = new PosWithRot();
					posWithRot.x = vector.x;
					posWithRot.y = vector.y;
					posWithRot.z = vector.z;
					posWithRot.yaw = mergedObject.Position.yaw + Quaternion.Inverse(rotation).eulerAngles.y;
					posWithRot.pitch = mergedObject.Position.pitch;
					PosWithRot position = posWithRot;
					VLootingObject vLootingObject = mergedObject as VLootingObject;
					result.AddLootingObject(vLootingObject.GetItemElement(), position, vLootingObject.ReasonOfSpawn);
				}
			}
		}
	}

	private void AddPlayerContaToResult(RoomDrainInfo result)
	{
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (value.StatControlUnit == null)
			{
				Logger.RWarn($"ExtractDrainInfo failed, player.StatControlUnit is null. playerID: {value.ObjectID}");
			}
			else
			{
				result.SetPlayerContaValue(value.UID, value.StatControlUnit.GetCurrentConta() - GetContaRecoveryRate());
			}
		}
	}

	public MsgErrorCode HandleLevelObject(int actorID, int levelObjectID, int state, bool occupy, out int prevState, bool resetRemainCount = false, bool successWhenNotFound = false)
	{
		prevState = state;
		if (!_levelObjects.TryGetValue(levelObjectID, out ILevelObjectInfo value))
		{
			if (successWhenNotFound)
			{
				return MsgErrorCode.Success;
			}
			return MsgErrorCode.LevelObjectNotFound;
		}
		if (_closedTramUpgradeObject.Contains(levelObjectID))
		{
			if (successWhenNotFound)
			{
				return MsgErrorCode.Success;
			}
			return MsgErrorCode.CantAction;
		}
		if (!(value is OccupiedLevelObjectInfo occupiedLevelObjectInfo))
		{
			if (!(value is StateLevelObjectInfo stateLevelObjectInfo))
			{
				if (value is TouchLevelObjectInfo levelObject)
				{
					MsgErrorCode msgErrorCode = LevelObjectEventInternal(levelObject, actorID, state, state);
					if (msgErrorCode != MsgErrorCode.Success)
					{
						return msgErrorCode;
					}
				}
			}
			else
			{
				if (resetRemainCount)
				{
					stateLevelObjectInfo.ResetActionRemainingCurrentCount(stateLevelObjectInfo.CurrentState, state);
				}
				if (!stateLevelObjectInfo.CanChangeState(state))
				{
					return MsgErrorCode.CantAction;
				}
				int currentState = stateLevelObjectInfo.CurrentState;
				MsgErrorCode msgErrorCode2 = LevelObjectEventInternal(stateLevelObjectInfo, actorID, currentState, state);
				if (msgErrorCode2 != MsgErrorCode.Success)
				{
					return msgErrorCode2;
				}
				stateLevelObjectInfo.ChangeState(state);
				prevState = stateLevelObjectInfo.PrevState;
				stateLevelObjectInfo.DecreaseActionRemainingCurrentCount(prevState, stateLevelObjectInfo.CurrentState);
				SendToAll(new UseLevelObjectSig
				{
					actorID = actorID,
					changedLevelObject = stateLevelObjectInfo.toLevelObjectInfo()
				});
			}
		}
		else
		{
			if (resetRemainCount)
			{
				occupiedLevelObjectInfo.ResetActionRemainingCurrentCount(occupiedLevelObjectInfo.CurrentState, state);
			}
			if (!occupiedLevelObjectInfo.CanChangeState(state, actorID))
			{
				return MsgErrorCode.CantAction;
			}
			int currentState2 = occupiedLevelObjectInfo.CurrentState;
			VActor vActor = FindActorByObjectID(actorID);
			if (vActor == null)
			{
				return MsgErrorCode.ActorNotFound;
			}
			if (!(vActor is VCreature vCreature))
			{
				return MsgErrorCode.RequestPlayerMismatch;
			}
			if (occupiedLevelObjectInfo.OccupiedActorID != 0 && occupiedLevelObjectInfo.OccupiedActorID != actorID)
			{
				return MsgErrorCode.CantAction;
			}
			if (vCreature.OccupiedLevelObjectInfo != null && vCreature.OccupiedLevelObjectInfo != occupiedLevelObjectInfo && occupy)
			{
				return MsgErrorCode.CantAction;
			}
			MsgErrorCode msgErrorCode3 = LevelObjectEventInternal(occupiedLevelObjectInfo, actorID, currentState2, state);
			if (msgErrorCode3 != MsgErrorCode.Success)
			{
				return msgErrorCode3;
			}
			occupiedLevelObjectInfo.ChangeState(state);
			prevState = occupiedLevelObjectInfo.PrevState;
			occupiedLevelObjectInfo.DecreaseActionRemainingCurrentCount(prevState, occupiedLevelObjectInfo.CurrentState);
			if (occupy)
			{
				occupiedLevelObjectInfo.ChangeOccupy(actorID);
				vCreature.SetOccupiedLevelObjectInfo(occupiedLevelObjectInfo);
			}
			else
			{
				occupiedLevelObjectInfo.ChangeOccupy(0);
				vCreature.SetOccupiedLevelObjectInfo(null);
			}
			SendToAll(new UseLevelObjectSig
			{
				actorID = actorID,
				changedLevelObject = occupiedLevelObjectInfo.toLevelObjectInfo()
			});
		}
		return MsgErrorCode.Success;
	}

	public virtual void ResetEnvironment(bool failed = false)
	{
		_commandExecutor.Invoke(delegate
		{
			InitLevel();
			InitSpawn();
			InitCutScenes();
			_startNotified = false;
		});
	}

	public bool ExecuteLazyInitialization()
	{
		if (_onLazyInitDelegate != null)
		{
			_onLazyInitDelegate(this);
			_onLazyInitDelegate = null;
			return true;
		}
		return false;
	}

	public long GetContaValue(VCreature creature)
	{
		if (creature is VPlayer vPlayer)
		{
			if (_playerContas.TryGetValue(vPlayer.UID, out var value))
			{
				return value;
			}
			Logger.RWarn($"GetOxygenValue failed, playerBaseOxygens not found. playerID: {vPlayer.ObjectID}");
			return 0L;
		}
		return 0L;
	}

	public int GetMemberCount()
	{
		return _vPlayerDict.Count;
	}

	public bool CheckLevelObjectStateChange(int levelObjectID, int fromState, int state)
	{
		if (!_levelObjects.TryGetValue(levelObjectID, out ILevelObjectInfo value))
		{
			return false;
		}
		if (value is StateLevelObjectInfo stateLevelObjectInfo)
		{
			if (stateLevelObjectInfo.CurrentState == fromState)
			{
				return stateLevelObjectInfo.CanChangeState(state);
			}
			return false;
		}
		if (value is TouchLevelObjectInfo)
		{
			return true;
		}
		return false;
	}

	public bool CheckConditionGroup(GameConditionGroup conditionGroups, int actorID)
	{
		return CheckConditionGroupInternal(conditionGroups, new List<int> { actorID });
	}

	public bool CheckConditionGroup(GameConditionGroup conditionGroups, List<int> actorIDs)
	{
		return CheckConditionGroupInternal(conditionGroups, actorIDs);
	}

	private bool CheckConditionGroupInternal(GameConditionGroup conditionGroups, List<int> actorIDs)
	{
		if (conditionGroups == null || conditionGroups.ConditionDict == null || conditionGroups.ConditionDict.Count == 0)
		{
			return true;
		}
		foreach (KeyValuePair<int, ImmutableDictionary<int, IGameCondition>> item in conditionGroups.ConditionDict)
		{
			ImmutableDictionary<int, IGameCondition> value = item.Value;
			bool flag = true;
			foreach (KeyValuePair<int, IGameCondition> item2 in value)
			{
				IGameCondition value2 = item2.Value;
				bool flag2 = false;
				switch (value2.GetLinkedParamType())
				{
				case GameConditionParamType.None:
					flag2 = CheckConditionInternal(value2);
					break;
				case GameConditionParamType.Actor:
					flag2 = CheckConditionInternal(value2, new GameCondtionParamActor(actorIDs[0]));
					break;
				case GameConditionParamType.Actors:
					flag2 = CheckConditionInternal(value2, new GameCondtionParamActors(actorIDs));
					break;
				}
				if (!(flag = flag && flag2))
				{
					break;
				}
			}
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	private void ExecuteActionInternal(ImmutableList<IGameAction> actions, ILevelObjectInfo levelObject, int actorID, int delay)
	{
		Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
		foreach (IGameAction action in actions)
		{
			switch (action.GetLinkedParamType())
			{
			case GameActionParamType.Actor:
				dictionary.Add(action.Clone(), new GameActionParamActor(actorID));
				break;
			case GameActionParamType.Position:
				dictionary.Add(action.Clone(), new GameActionParamPosition(levelObject.Pos.toPosWithRot(levelObject.DataOrigin.transform.rotation.eulerAngles.y), levelObject.DataOrigin.IsIndoor));
				break;
			default:
				dictionary.Add(action.Clone(), null);
				break;
			}
		}
		if (!EnqueueEventAction(dictionary, delay))
		{
			Logger.RWarn($"ExecuteActionInternal failed, RunEventAction failed. actorID: {actorID}");
		}
	}

	private MsgErrorCode LevelObjectEventInternal(ILevelObjectInfo levelObject, int actorID, int fromState, int toState)
	{
		GameConditionGroup gameConditionGroup = levelObject.GetGameConditionGroup(fromState, toState);
		if (gameConditionGroup != null && !CheckConditionGroup(gameConditionGroup, actorID))
		{
			return MsgErrorCode.CantAction;
		}
		int gameActionDelay = levelObject.GetGameActionDelay(fromState, toState);
		ImmutableList<IGameAction> gameActions = levelObject.GetGameActions(fromState, toState);
		if (gameActions != null)
		{
			ExecuteActionInternal(gameActions, levelObject, actorID, gameActionDelay);
		}
		return MsgErrorCode.Success;
	}

	public bool CheckContaSafePosition(VActor actor)
	{
		if (actor.IsIndoor)
		{
			return true;
		}
		return CheckSafeArea(actor);
	}

	public bool CheckSafeArea(VActor actor)
	{
		if (CheckStartingArea(actor) || CheckCanopyArea(actor))
		{
			return true;
		}
		return false;
	}

	public bool CheckStartingArea(VActor actor)
	{
		foreach (TriggerVolumeData playerStartingVolume in _playerStartingVolumes)
		{
			if (playerStartingVolume.Bounds.Contains(actor.PositionVector))
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckCanopyArea(VActor actor)
	{
		foreach (CanopyVolumeData canopyVolume in _canopyVolumes)
		{
			if (canopyVolume.Bounds.Contains(actor.PositionVector))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCutSceneComplete(string cutSceneName)
	{
		if (!_cutSceneInfos.TryGetValue(cutSceneName, out CutSceneInfo value))
		{
			return false;
		}
		bool num = value.CheckComplete();
		if (num && value.NeedToBroadcast)
		{
			OnCutsceneComplete(cutSceneName);
		}
		return num;
	}

	public void PlayAnimation(string animationName)
	{
		SendToAllPlayers(new PlayAnimationSig
		{
			animationName = animationName
		});
	}

	private void ManagePlayCutScene()
	{
		IEnumerable<CutSceneInfo> enumerable = _cutSceneInfos.Values.Where((CutSceneInfo x) => x.IsPlaying);
		enumerable.Count();
		foreach (CutSceneInfo item in enumerable)
		{
			if (item.IsBroadcastTimeElapsed())
			{
				SendToAllPlayers(new PlayCutSceneSig
				{
					cutSceneName = item.Name
				});
			}
		}
	}

	public MsgErrorCode PlayCutScene(string cutSceneName, bool needToBroadcast)
	{
		if (!_cutSceneInfos.TryGetValue(cutSceneName, out CutSceneInfo cutSceneInfo))
		{
			Logger.RError("PlayCutScene failed, cutScene not found. cutSceneName: " + cutSceneName);
			return MsgErrorCode.CutSceneNotFound;
		}
		if (cutSceneInfo.IsPlaying)
		{
			Logger.RError("PlayCutScene failed, cutScene already playing. cutSceneName: " + cutSceneName);
			return MsgErrorCode.CutSceneAlreadyPlaying;
		}
		cutSceneInfo.SetBroadcast(needToBroadcast);
		Hub.s.timeutil.GetCurrentTickMilliSec();
		cutSceneInfo.StartPlay();
		cutSceneInfo.AddParticipant(_vPlayerDict.Values.Select((VPlayer x) => x.ObjectID).ToList());
		_eventTimer.CreateTimerEvent(delegate
		{
			if (cutSceneInfo.State == CutSceneState.Playing && cutSceneInfo.CheckComplete())
			{
				OnCutsceneComplete(cutSceneName);
			}
		}, cutSceneInfo.PlayTime + 2000);
		return MsgErrorCode.Success;
	}

	protected virtual void OnCutsceneComplete(string cutSceneName)
	{
		SendToAllPlayers(new CutSceneCompleteSig
		{
			cutSceneName = cutSceneName
		});
	}

	public bool IsAllPlayerWastedOrDead()
	{
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (value.IsAliveStatus() && !value.Wasted)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsAllPlayerDead()
	{
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (value.IsAliveStatus())
			{
				return false;
			}
		}
		return true;
	}

	public bool UseTeleportStartPoint(VCreature creature, MapMarker_TeleportStartPoint startPoint)
	{
		MapMarker_TeleportEndPoint teleportEndPoint = Hub.s.dynamicDataMan.GetTeleportEndPoint(startPoint.EndCallSign);
		if (teleportEndPoint == null)
		{
			Logger.RError("[UseTeleportStartPoint] failed, GetTeleportEndPoint failed. callSign: " + startPoint.EndCallSign);
			return false;
		}
		creature?.SetIsIndoor(teleportEndPoint.IsIndoor);
		creature.Teleport(teleportEndPoint.Pos, TeleportReason.Sequence);
		return true;
	}

	public MapMarker_TeleportStartPoint? GetTeleportStartPoint(VCreature creature, BTTargetPickRule rule, AIRangeType rangeType, bool checkHeight)
	{
		List<MapMarker_TeleportStartPoint> teleportStartPointByType = Hub.s.dynamicDataMan.GetTeleportStartPointByType(TeleportType.BTNormal);
		if (teleportStartPointByType.Count == 0)
		{
			return null;
		}
		MapMarker_TeleportStartPoint result = null;
		List<Vector3> path;
		switch (rule)
		{
		case BTTargetPickRule.MaxDistance:
			foreach (MapMarker_TeleportStartPoint item in teleportStartPointByType.OrderByDescending((MapMarker_TeleportStartPoint point) => (rangeType == AIRangeType.Absolute) ? ((double)Misc.Distance(creature.PositionVector, point.Pos.toVector3(), checkHeight)) : GetDistanceByNavMesh(creature.PositionVector, point.Pos.toVector3())))
			{
				if (FindPath(creature.PositionVector, item.Pos.toVector3(), out path))
				{
					result = item;
					break;
				}
			}
			break;
		case BTTargetPickRule.MinDistance:
			foreach (MapMarker_TeleportStartPoint item2 in teleportStartPointByType.OrderBy((MapMarker_TeleportStartPoint point) => (rangeType == AIRangeType.Absolute) ? ((double)Misc.Distance(creature.PositionVector, point.Pos.toVector3(), checkHeight)) : GetDistanceByNavMesh(creature.PositionVector, point.Pos.toVector3())))
			{
				if (FindPath(creature.PositionVector, item2.Pos.toVector3(), out path))
				{
					result = item2;
					break;
				}
			}
			break;
		case BTTargetPickRule.Random:
			foreach (MapMarker_TeleportStartPoint item3 in teleportStartPointByType.OrderBy((MapMarker_TeleportStartPoint point) => Guid.NewGuid()))
			{
				if (FindPath(creature.PositionVector, item3.Pos.toVector3(), out path))
				{
					result = item3;
					break;
				}
			}
			break;
		default:
			Logger.RError($"GetTeleportStartPoint failed, invalid rule. rule: {rule}");
			break;
		}
		return result;
	}

	public Vector3 GetRandomTargetPoint(VCreature creature, BTTargetPickRule rule, AIRangeType rangeType, bool checkHeight)
	{
		Vector3 result = Vector3.zero;
		List<Vector3> path;
		switch (rule)
		{
		case BTTargetPickRule.MaxDistance:
			foreach (Vector3 item in from point in _targetPoints.Values
				where point.IsInDoor == creature.IsIndoor
				select point.Pos.toVector3() into point
				orderby (rangeType == AIRangeType.Absolute) ? ((double)Misc.Distance(creature.PositionVector, point, checkHeight)) : GetDistanceByNavMesh(creature.PositionVector, point) descending
				select point)
			{
				if (FindPath(creature.PositionVector, item, out path))
				{
					result = item;
					break;
				}
			}
			break;
		case BTTargetPickRule.MinDistance:
			foreach (Vector3 item2 in from point in _targetPoints.Values
				where point.IsInDoor == creature.IsIndoor
				select point.Pos.toVector3() into point
				orderby (rangeType == AIRangeType.Absolute) ? ((double)Misc.Distance(creature.PositionVector, point, checkHeight)) : GetDistanceByNavMesh(creature.PositionVector, point)
				select point)
			{
				if (FindPath(creature.PositionVector, item2, out path))
				{
					result = item2;
					break;
				}
			}
			break;
		case BTTargetPickRule.Random:
			foreach (Vector3 item3 in from point in _targetPoints.Values
				where point.IsInDoor == creature.IsIndoor
				select point.Pos.toVector3() into point
				orderby Guid.NewGuid()
				select point)
			{
				if (FindPath(creature.PositionVector, item3, out path))
				{
					result = item3;
					break;
				}
			}
			break;
		default:
			Logger.RError($"GetRandomTargetPoint failed, invalid rule. rule: {rule}");
			break;
		}
		return result;
	}

	public void SetSpawnHold(bool hold)
	{
		_spawnHoldDebug = hold;
	}

	public void RecoverAllMemberConta()
	{
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			value.StatControlUnit?.RecoverConta(Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue);
		}
	}

	public void SetCurrentDayDebug(int day)
	{
		_currentDayDefered = day;
	}

	public void SetCurrentCycleForDebug(int cycle)
	{
		Hub.s.vworld.VRoomManager.SetCurrentCycleForDebug(cycle);
		_currentSessionCount = cycle;
	}

	public List<FieldSkillObject> GetFieldSkillObjects()
	{
		List<FieldSkillObject> list = new List<FieldSkillObject>();
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is FieldSkillObject item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	protected virtual void OnAllMemberEntered()
	{
		SyncEnterRoom();
		if (!_startNotified)
		{
			RunEnterRoomCutScene();
			_startNotified = true;
		}
	}

	public int GetDeadPlayerCount()
	{
		return _vPlayerDict.Values.Count((VPlayer x) => !x.IsAliveStatus());
	}

	public void ToggleDebug(bool flag)
	{
		_turnOnDebug = flag;
	}

	public List<Vector3>? GetHolePointsOnTrackVolume(Vector3 targetPos)
	{
		foreach (VerticalTrackVolumeData verticalTrackVolumeData in _verticalTrackVolumeDatas)
		{
			if (verticalTrackVolumeData.Bounds.Contains(targetPos))
			{
				return verticalTrackVolumeData.HolePoints;
			}
		}
		return null;
	}

	public void ResetBoostedItem()
	{
		if (Hub.s.dataman.ExcelDataManager.GetDungeonCandidateMasterID(_currentSessionCount).Count == 0)
		{
			Logger.RError($"RollDiceDungeon failed, candidates is empty. _currentSessionCount: {_currentSessionCount}");
			BoostedItemMasterID = 0;
			BoostedItemRate = 0f;
			return;
		}
		if (SimpleRandUtil.Next(0, 10001) >= Hub.s.dataman.ExcelDataManager.Consts.C_BonusItemAppearRate)
		{
			BoostedItemMasterID = 0;
			BoostedItemRate = 0f;
			return;
		}
		float boostedItemRate = (float)SimpleRandUtil.Next(Hub.s.dataman.ExcelDataManager.Consts.C_BonusItemRateMin, Hub.s.dataman.ExcelDataManager.Consts.C_BonusItemRateMax + 1) / 10000f;
		int randomBoostItemMasterID = Hub.s.dataman.ExcelDataManager.GetRandomBoostItemMasterID();
		if (randomBoostItemMasterID == 0)
		{
			Logger.RError($"RollDiceDungeon failed, boostItemMasterID is 0. _currentSessionCount: {_currentSessionCount}");
			BoostedItemMasterID = 0;
			BoostedItemRate = 0f;
		}
		else
		{
			BoostedItemMasterID = randomBoostItemMasterID;
			BoostedItemRate = boostedItemRate;
		}
	}

	protected virtual void OnDecideRoom(Command command)
	{
		EnableAIControl(enable: false);
		SyncEndCutScene();
		_onEmptyEventActionQueue.Enqueue(command);
	}

	public void EnableAggroLogging(bool enable)
	{
		EnableAggroLog = enable;
	}

	public void EnableAIControl(bool enable)
	{
		EnableAIController = enable;
	}

	protected abstract void OnEnterRoomFailed(SessionContext context, MsgErrorCode errorCode, int hashCode);

	public abstract int GetContaValue(VActor actor, bool isRun);

	protected abstract void SyncEnterRoom();

	protected abstract void RunEnterRoomCutScene();

	protected abstract void SyncEndCutScene();

	public virtual bool DamageAppliable()
	{
		return false;
	}

	public virtual bool IsPlayable()
	{
		return !_stopped.IsOn;
	}

	protected void ResumeRoom()
	{
		_stopped.Off();
		_blockEventAction.Off();
	}

	public void IncreaseItemCarryCount(VActor actor, int count = 1)
	{
		if (!_playReportDict.TryGetValue(actor.ObjectID, out PlayReportData value))
		{
			Logger.RWarn($"[IVRoom.IncreaseItemCarryCount] _playReportDict dont have key. {actor.ObjectID}");
		}
		else
		{
			value.IncreaseItemCarryCount(count);
		}
	}

	public void IncreaseDamageToAlly(VActor actor, long damage)
	{
		if (!_playReportDict.TryGetValue(actor.ObjectID, out PlayReportData value))
		{
			Logger.RWarn($"[IVRoom.IncreaseDamageToAlly] _playReportDict dont have key. {actor.ObjectID}");
		}
		else
		{
			value.IncreaseDamageToAlly(damage);
		}
	}

	public void IncreaseTimeInStartingVolume(VActor actor, long second)
	{
		if (!_playReportDict.TryGetValue(actor.ObjectID, out PlayReportData value))
		{
			Logger.RWarn($"[IVRoom.IncreaseTimeInStartingVolume] _playReportDict dont have key. {actor.ObjectID}");
		}
		else
		{
			value.IncreaseTimeInStartingVolume(second);
		}
	}

	public void IncreaseMimicEncounterCount(VActor actor, int count = 1)
	{
		if (!_playReportDict.TryGetValue(actor.ObjectID, out PlayReportData value))
		{
			Logger.RWarn($"[IVRoom.IncreaseMimicEncounterCount] _playReportDict dont have key. {actor.ObjectID}");
		}
		else
		{
			value.IncreaseMimicEncounterCount(count);
		}
	}

	public List<VActor> GetActorsExcept(VActor self, List<int> objectIDs)
	{
		List<VActor> list = new List<VActor>();
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (value.IsAliveStatus() && value.ObjectID != self.ObjectID && !objectIDs.Contains(value.ObjectID))
			{
				list.Add(value);
			}
		}
		foreach (VActor value2 in _vActorDict.Values)
		{
			if (value2.IsAliveStatus() && value2.ObjectID != self.ObjectID && !(value2 is VPlayer) && !objectIDs.Contains(value2.ObjectID))
			{
				list.Add(value2);
			}
		}
		return list;
	}

	public void AddEventTimer(OnTimerHandler onTimerHandler, long duration, bool repeat = false, string? timerName = null)
	{
		_eventTimer.CreateTimerEvent(onTimerHandler, duration, repeat, timerName);
	}

	public abstract VRoomType GetToMoveRoomType();

	public void TryReleaseOccupiedLevelObject(int actorID)
	{
		foreach (ILevelObjectInfo value in _levelObjects.Values)
		{
			OccupiedLevelObjectInfo occupiedLevelObjectInfo = value as OccupiedLevelObjectInfo;
			if (occupiedLevelObjectInfo == null)
			{
				continue;
			}
			LevelObject dataOrigin = occupiedLevelObjectInfo.DataOrigin;
			if (((object)dataOrigin != null && dataOrigin.LevelObjectType == LevelObjectClientType.RandomTeleporter) || occupiedLevelObjectInfo.OccupiedActorID != actorID)
			{
				continue;
			}
			_eventTimer.CreateTimerEvent(delegate
			{
				int prevState;
				MsgErrorCode msgErrorCode = HandleLevelObject(actorID, occupiedLevelObjectInfo.ID, 0, occupy: false, out prevState);
				if (msgErrorCode != MsgErrorCode.Success)
				{
					Logger.RWarn($"[TryReleaseOccupiedLevelObject] fail. {occupiedLevelObjectInfo.ID}. errorCode:{msgErrorCode}");
				}
			}, 50L);
			break;
		}
	}

	public bool HangItem(int index, ItemElement itemElement)
	{
		if (_stashes.ContainsKey(index))
		{
			return false;
		}
		if (index > 4)
		{
			return false;
		}
		_stashes.Add(index, itemElement);
		SendToAllPlayers(new StashStatusSig
		{
			stashedItems = _stashes.ToDictionary<KeyValuePair<int, ItemElement>, int, ItemInfo>((KeyValuePair<int, ItemElement> entry) => entry.Key, (KeyValuePair<int, ItemElement> entry) => entry.Value.toItemInfo())
		});
		return true;
	}

	public bool UnhangItem(int index, out ItemElement? itemElement)
	{
		itemElement = null;
		if (!_stashes.ContainsKey(index))
		{
			return false;
		}
		itemElement = _stashes[index];
		_stashes.Remove(index);
		SendToAllPlayers(new StashStatusSig
		{
			stashedItems = _stashes.ToDictionary<KeyValuePair<int, ItemElement>, int, ItemInfo>((KeyValuePair<int, ItemElement> entry) => entry.Key, (KeyValuePair<int, ItemElement> entry) => entry.Value.toItemInfo())
		});
		return true;
	}

	public bool IsTramUpgradeActive(int itemMasterID)
	{
		return _tramUpgradeList.Contains(itemMasterID);
	}

	public void ManageSoundPlayData()
	{
		long currentTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		foreach (string item in (from x in _soundClipDict
			where x.Value + 10000 < currentTime
			select x.Key).ToList())
		{
			_soundClipDict.Remove(item);
		}
	}

	public void PlaySound(string soundClipID, PosWithRot pos)
	{
		if (!_soundClipDict.ContainsKey(soundClipID))
		{
			SendToAll(new PlaySoundSig
			{
				soundClipID = soundClipID,
				pos = pos.toVector3()
			});
			_soundClipDict[soundClipID] = Hub.s.timeutil.GetCurrentTickMilliSec();
		}
	}

	public void OnActorMove(VActor actor, Vector3 prevPos, Vector3 currentPos)
	{
		if (!(actor is VCreature vCreature))
		{
			return;
		}
		CheckTriggerVolumeEvent(actor, prevPos, currentPos, MapTrigger.eCheckTypeFlag.Enter);
		bool flag = false;
		foreach (VerticalTrackVolumeData verticalTrackVolumeData in _verticalTrackVolumeDatas)
		{
			if (verticalTrackVolumeData.IsInBounds(currentPos))
			{
				flag |= vCreature.SetFallPath(enable: true);
				if (flag)
				{
					break;
				}
			}
		}
		if (!flag)
		{
			vCreature.SetFallPath(enable: false);
		}
	}

	public void CheckTriggerVolumeEvent(VActor actor, Vector3 prevPos, Vector3 currentPos, MapTrigger.eCheckTypeFlag checkTypeFlag)
	{
		if (!IsPlayable())
		{
			return;
		}
		foreach (TriggerVolumeData playerTriggerVolume in _playerTriggerVolumes)
		{
			if (!playerTriggerVolume.IsTriggered(prevPos, currentPos, this, checkTypeFlag) || playerTriggerVolume.EventActionGroup.Count <= 0)
			{
				continue;
			}
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			foreach (IGameAction item in playerTriggerVolume.EventActionGroup)
			{
				dictionary.Add(item.Clone(), new GameActionParamActor(actor.ObjectID));
			}
			EnqueueEventAction(dictionary, playerTriggerVolume.ActionDelay);
		}
	}

	private bool CheckConditionInternal(IGameCondition condition, IGameCondtionParam? param = null)
	{
		BaseGameCondition baseGameCondition = condition as BaseGameCondition;
		switch (baseGameCondition.ObjType)
		{
		case DefCondition.CHECK_ITEM:
		{
			if (!(baseGameCondition is GameConditionCheckItem gameConditionCheckItem))
			{
				return false;
			}
			if (param == null || !(param is GameCondtionParamActor gameCondtionParamActor))
			{
				return false;
			}
			VActor vActor3 = FindActorByObjectID(gameCondtionParamActor.ActorID);
			if (vActor3 == null)
			{
				return false;
			}
			if (!(vActor3 is VPlayer vPlayer2))
			{
				return false;
			}
			bool? flag = vPlayer2.InventoryControlUnit?.CheckItem(gameConditionCheckItem.ItemMasterID, gameConditionCheckItem.Count);
			if (flag.HasValue)
			{
				return flag.Value;
			}
			return false;
		}
		case DefCondition.CHECK_SCRAP_WEIGHT:
		{
			if (!(baseGameCondition is GameConditionCheckScrapWeight gameConditionCheckScrapWeight))
			{
				return false;
			}
			if (param == null || !(param is GameCondtionParamActors gameCondtionParamActors))
			{
				return false;
			}
			int num = 0;
			foreach (int actorID in gameCondtionParamActors.ActorIDs)
			{
				VActor vActor = FindActorByObjectID(actorID);
				if (vActor == null)
				{
					Logger.RWarn($"[GameCondition] CHECK_SCRAP_WEIGHT. targetActor is null {actorID}", sendToLogServer: false, useConsoleOut: true, "maptrigger");
					return false;
				}
				if (!(vActor is VPlayer vPlayer))
				{
					Logger.RWarn($"[GameCondition] CHECK_SCRAP_WEIGHT. targetActor is not VPlayer {actorID}", sendToLogServer: false, useConsoleOut: true, "maptrigger");
					return false;
				}
				num += vPlayer.InventoryControlUnit.TotalWeight;
			}
			return num > gameConditionCheckScrapWeight.Weight;
		}
		case DefCondition.CHECK_ACTOR_TYPE:
		{
			if (!(baseGameCondition is GameConditionCheckActorType gameConditionCheckActorType))
			{
				return false;
			}
			if (param == null || !(param is GameCondtionParamActors gameCondtionParamActors2))
			{
				return false;
			}
			bool result = false;
			foreach (int actorID2 in gameCondtionParamActors2.ActorIDs)
			{
				VActor vActor2 = FindActorByObjectID(actorID2);
				if (vActor2 == null)
				{
					return false;
				}
				if (!(vActor2 is VCreature vCreature))
				{
					return false;
				}
				if (vCreature.ActorType == gameConditionCheckActorType.ActorType)
				{
					result = true;
					break;
				}
			}
			return result;
		}
		case DefCondition.CHECK_TRAM_REPAIR:
			if (!(baseGameCondition is GameConditionCheckTramRepair gameConditionCheckTramRepair))
			{
				return false;
			}
			if (!(this is MaintenanceRoom maintenanceRoom))
			{
				return false;
			}
			return gameConditionCheckTramRepair.IsTramRepaired == maintenanceRoom.IsTramRestored();
		default:
			return false;
		}
	}

	public bool EnqueueEventAction(Dictionary<IGameAction, IGameActionParam?> actionDict, int delay)
	{
		if (_blockEventAction.IsOn)
		{
			return false;
		}
		if (actionDict.Count == 0)
		{
			Logger.RError("[EnqueueEventAction] failed, action is null or empty.");
			return false;
		}
		Queue<(IGameAction, IGameActionParam)> queue = new Queue<(IGameAction, IGameActionParam)>();
		foreach (KeyValuePair<IGameAction, IGameActionParam> item in actionDict)
		{
			queue.Enqueue((item.Key, item.Value));
		}
		ActionQueuePack actionQueuePack = new ActionQueuePack(delay, queue);
		_eventActionGroups.TryAdd(actionQueuePack.ID, actionQueuePack);
		return true;
	}

	private void PumpEventAction()
	{
		List<int> list = new List<int>();
		foreach (ActionQueuePack value2 in _eventActionGroups.Values)
		{
			if (value2.IsWaitComplete())
			{
				if (value2.Delay > 0 && value2.StartTime == 0L)
				{
					ScheduleDelayedAction(value2);
				}
				else
				{
					ProcessActionQueue(value2.ActionQueue);
				}
				if (value2.ActionQueue.Count == 0)
				{
					list.Add(value2.ID);
				}
			}
		}
		list.ForEach(delegate(int x)
		{
			_eventActionGroups.TryRemove(x, out ActionQueuePack _);
		});
		if (_eventActionGroups.Count == 0 && _onEmptyEventActionQueue.Count > 0 && _onEmptyEventActionQueue.TryDequeue(out Command result))
		{
			result();
		}
	}

	private void ScheduleDelayedAction(ActionQueuePack actionQueuePack)
	{
		if (actionQueuePack.StartTime == 0L)
		{
			_eventTimer.CreateTimerEvent(delegate
			{
				ProcessActionQueue(actionQueuePack.ActionQueue);
			}, actionQueuePack.Delay);
			actionQueuePack.StartWait();
		}
	}

	private void ProcessActionQueue(Queue<(IGameAction, IGameActionParam?)> actionQueue)
	{
		if (actionQueue.Count == 0)
		{
			return;
		}
		(IGameAction, IGameActionParam) result;
		while (actionQueue.TryPeek(out result))
		{
			var (gameAction, param) = result;
			if (gameAction.State == GameActionState.Ready)
			{
				ProcessReadyAction(actionQueue, gameAction, param);
			}
			if (gameAction.State == GameActionState.Progress)
			{
				if (!gameAction.IsComplete())
				{
					break;
				}
				gameAction.SetComplete();
			}
			if (gameAction.State == GameActionState.Complete || gameAction.State == GameActionState.Failed)
			{
				actionQueue.Dequeue();
			}
		}
	}

	private void ProcessReadyAction(Queue<(IGameAction, IGameActionParam?)> actionQueue, IGameAction gameAction, IGameActionParam? param)
	{
		if (!RunEventActionInternal(gameAction, param))
		{
			Logger.RWarn($"[PumpEventAction] RunEventActionInternal failed. {gameAction}");
			gameAction.SetFailed();
		}
		else if (gameAction.HasCompleteChecker())
		{
			if (gameAction.IsComplete())
			{
				gameAction.SetComplete();
			}
			else
			{
				gameAction.Progress();
			}
		}
		else
		{
			gameAction.SetComplete();
		}
	}

	private bool RunEventActionInternal(IGameAction action, IGameActionParam? param = null)
	{
		switch ((action as GameAction).ActionType)
		{
		case DefAction.MAP_COMPLETE:
			if (!(action is GameActionMapComplete))
			{
				return false;
			}
			if (!(this is DungeonRoom dungeonRoom2))
			{
				return false;
			}
			return dungeonRoom2.CheckComplete(DungeonCompleteCheckType.Triggered);
		case DefAction.SESSION_DECISION:
			if (!(action is GameActionSessionDecision))
			{
				return false;
			}
			if (!(this is MaintenanceRoom maintenanceRoom))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor5))
			{
				return false;
			}
			maintenanceRoom.SessionDecision(gameActionParamActor5.ActorID);
			return true;
		case DefAction.ACTIVATE_SPAWNPOINT:
			if (!(action is GameActionActivateSpawnPoint gameActionActivateSpawnPoint))
			{
				return false;
			}
			foreach (string spawnPointName in gameActionActivateSpawnPoint.SpawnPointNameList)
			{
				TriggerSpawnByEvent(spawnPointName);
			}
			return true;
		case DefAction.PLAY_SOUND:
			if (!(action is GameActionPlaySound gameActionPlaySound))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamPosition gameActionParamPosition2))
			{
				return false;
			}
			PlaySound(gameActionPlaySound.SoundClipKey, gameActionParamPosition2.Position);
			return true;
		case DefAction.CHANGE_MUTABLE_STAT:
		{
			if (!(action is GameActionChangeMutableStat gameActionChangeMutableStat))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor8))
			{
				return false;
			}
			VActor vActor6 = FindActorByObjectID(gameActionParamActor8.ActorID);
			if (vActor6 == null)
			{
				return false;
			}
			switch (gameActionChangeMutableStat.StatType)
			{
			case MutableStatType.HP:
				if (!(vActor6 is VPlayer vPlayer6))
				{
					return false;
				}
				vPlayer6.StatControlUnit?.InstantChargeHP(gameActionChangeMutableStat.Value);
				return true;
			case MutableStatType.Conta:
				if (!(vActor6 is VPlayer vPlayer5))
				{
					return false;
				}
				vPlayer5.StatControlUnit?.IncreaseConta(gameActionChangeMutableStat.Value);
				return true;
			default:
				Logger.RError($"[RunEventActionInternal] ChangeMutableStat failed, Invalid StatType. {gameActionChangeMutableStat.StatType}");
				return false;
			}
		}
		case DefAction.ADD_ABNORMAL:
		{
			if (!(action is GameActionAddAbnormal gameActionAddAbnormal))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor2))
			{
				return false;
			}
			VActor vActor2 = FindActorByObjectID(gameActionParamActor2.ActorID);
			if (vActor2 == null)
			{
				return false;
			}
			if (!(vActor2 is VPlayer vPlayer2))
			{
				return false;
			}
			return vPlayer2.AbnormalControlUnit?.AppendAbnormal(vPlayer2.ObjectID, gameActionAddAbnormal.AbnormalMasterID) ?? false;
		}
		case DefAction.DECREASE_ITEM_COUNT:
		{
			if (!(action is GameActionDecreaseItem gameActionDecreaseItem))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor9))
			{
				return false;
			}
			VActor vActor7 = FindActorByObjectID(gameActionParamActor9.ActorID);
			if (vActor7 == null)
			{
				return false;
			}
			if (!(vActor7 is VPlayer vPlayer7))
			{
				return false;
			}
			return vPlayer7.InventoryControlUnit?.DecreaseItem(gameActionDecreaseItem.ItemMasterID, gameActionDecreaseItem.Count) ?? false;
		}
		case DefAction.CHANGE_MUTABLE_STAT_RANDOM:
		{
			if (!(action is GameActionChangeMutableStatRandom gameActionChangeMutableStatRandom))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor3))
			{
				return false;
			}
			VActor vActor3 = FindActorByObjectID(gameActionParamActor3.ActorID);
			if (vActor3 == null)
			{
				return false;
			}
			switch (gameActionChangeMutableStatRandom.StatType)
			{
			case MutableStatType.HP:
			{
				if (!(vActor3 is VPlayer vPlayer4))
				{
					return false;
				}
				long value = SimpleRandUtil.Next(gameActionChangeMutableStatRandom.MinValue, gameActionChangeMutableStatRandom.MaxValue);
				vPlayer4.StatControlUnit?.InstantChargeHP(value);
				return true;
			}
			case MutableStatType.Conta:
			{
				if (!(vActor3 is VPlayer vPlayer3))
				{
					return false;
				}
				long amount = SimpleRandUtil.Next(gameActionChangeMutableStatRandom.MinValue, gameActionChangeMutableStatRandom.MaxValue);
				vPlayer3.StatControlUnit?.RecoverConta(amount);
				return true;
			}
			default:
				return false;
			}
		}
		case DefAction.TELEPORT:
		{
			if (!(action is GameActionTeleport gameActionTeleport))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor10))
			{
				return false;
			}
			VActor vActor8 = FindActorByObjectID(gameActionParamActor10.ActorID);
			if (vActor8 == null)
			{
				return false;
			}
			MapMarker_TeleportStartPoint teleportStartPoint = Hub.s.dynamicDataMan.GetTeleportStartPoint(gameActionTeleport.TeleportStartPointCallSign);
			if (teleportStartPoint == null)
			{
				Logger.RError("[RunEventActionInternal] failed, GetTeleportStartPoint failed. callSign: " + gameActionTeleport.TeleportStartPointCallSign);
				return false;
			}
			MapMarker_TeleportEndPoint teleportEndPoint = Hub.s.dynamicDataMan.GetTeleportEndPoint(teleportStartPoint.EndCallSign);
			if (teleportEndPoint == null)
			{
				Logger.RError("[RunEventActionInternal] failed, GetTeleportEndPoint failed. callSign: " + teleportStartPoint.EndCallSign);
				return false;
			}
			PosWithRot targetRandomPos2 = teleportEndPoint.GetTargetRandomPos();
			if (vActor8 is VCreature vCreature3)
			{
				vCreature3.SetIsIndoor(teleportEndPoint.IsIndoor);
			}
			vActor8.Teleport(targetRandomPos2, TeleportReason.LevelObject);
			return true;
		}
		case DefAction.RANDOM_TELEPORT:
		{
			if (!(action is GameActionRandomTeleport))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor6))
			{
				return false;
			}
			VActor vActor4 = FindActorByObjectID(gameActionParamActor6.ActorID);
			if (vActor4 == null)
			{
				return false;
			}
			List<MapMarker_TeleportEndPoint> allRandomTeleportEndPoints = Hub.s.dynamicDataMan.GetAllRandomTeleportEndPoints();
			if (allRandomTeleportEndPoints == null || allRandomTeleportEndPoints.Count <= 0)
			{
				Logger.RError("[RunEventActionInternal] failed, GetRandomTeleportEndPoints failed.");
				return false;
			}
			List<MapMarker_TeleportEndPoint> list = allRandomTeleportEndPoints.OrderBy((MapMarker_TeleportEndPoint x) => Guid.NewGuid()).ToList();
			MapMarker_TeleportEndPoint mapMarker_TeleportEndPoint = null;
			for (int num = 0; num < list.Count; num++)
			{
				if (VWorldUtil.Distance(vActor4.PositionVector, list[num].transform.position) >= (double)Hub.s.dataman.ExcelDataManager.Consts.C_RandomTeleportEndRangeMin * 0.01)
				{
					mapMarker_TeleportEndPoint = list[num];
					break;
				}
			}
			if (mapMarker_TeleportEndPoint == null)
			{
				Logger.RWarn("[RunEventActionInternal] RandomTeleport. Cant find endpoint. using first.");
				mapMarker_TeleportEndPoint = list[0];
			}
			PosWithRot targetRandomPos = mapMarker_TeleportEndPoint.GetTargetRandomPos();
			if (vActor4 is VCreature vCreature)
			{
				vCreature.SetIsIndoor(mapMarker_TeleportEndPoint.IsIndoor);
			}
			vActor4.Teleport(targetRandomPos, TeleportReason.RandomTeleport);
			return true;
		}
		case DefAction.HANDLE_LEVELOBJECT:
		{
			if (!(action is GameActionHandleLevelObject gameActionHandleLevelObject))
			{
				return false;
			}
			ILevelObjectInfo levelObjectInfo = FindLevelObjectByName(gameActionHandleLevelObject.LevelObjectName);
			if (levelObjectInfo == null)
			{
				Logger.RError("[RunEventActionInternal] HANDLE_LEVELOBJECT failed, FindLevelObjectByName failed. LevelObjectName: " + gameActionHandleLevelObject.LevelObjectName);
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor4))
			{
				return false;
			}
			int prevState;
			return HandleLevelObject(FindActorByObjectID(gameActionParamActor4.ActorID)?.ObjectID ?? 0, levelObjectInfo.ID, gameActionHandleLevelObject.LevelObjectState, gameActionHandleLevelObject.Occupy, out prevState, gameActionHandleLevelObject.ResetRemainCount, successWhenNotFound: true) == MsgErrorCode.Success;
		}
		case DefAction.TELEPORT_TO_SPAWNPOINT:
			if (!(action is GameActionTeleportToSpawnPoint))
			{
				return false;
			}
			foreach (VPlayer value2 in _vPlayerDict.Values)
			{
				if (value2.IsAliveStatus())
				{
					MapMarker_CreatureSpawnPoint playerStartPoint = Hub.s.dynamicDataMan.GetPlayerStartPoint((int)(value2.UID % 4));
					if (!(playerStartPoint == null))
					{
						value2.Teleport(playerStartPoint.pos, TeleportReason.EventAction);
					}
				}
			}
			return true;
		case DefAction.SPAWN_FIELD_SKILL:
			if (!(action is GameActionSpawnFieldSkill gameActionSpawnFieldSkill))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamPosition gameActionParamPosition4))
			{
				return false;
			}
			SpawnFieldSkill(gameActionSpawnFieldSkill.FieldSkillMasterID, gameActionParamPosition4.Position, gameActionParamPosition4.IsIndoor, null, null, null, ReasonOfSpawn.EventAction);
			return true;
		case DefAction.SPAWN_FIELD_SKILL_RANDOM:
		{
			if (!(action is GameActionSpawnFieldSkillRandom gameActionSpawnFieldSkillRandom))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamPosition gameActionParamPosition))
			{
				return false;
			}
			int masterID = gameActionSpawnFieldSkillRandom.FieldSkillMasterIDList[SimpleRandUtil.Next(0, gameActionSpawnFieldSkillRandom.FieldSkillMasterIDList.Count)];
			SpawnFieldSkill(masterID, gameActionParamPosition.Position, gameActionParamPosition.IsIndoor, null, null, null, ReasonOfSpawn.EventAction);
			return true;
		}
		case DefAction.SPAWN_FIELD_SKILL_MAPMARKER:
			if (!(action is GameActionSpawnFieldSkillMapMarker gameActionSpawnFieldSkillMapMarker))
			{
				return false;
			}
			TriggerSpawnByEvent(gameActionSpawnFieldSkillMapMarker.MapMarkerName);
			return true;
		case DefAction.PLAY_ANIMATION:
			if (!(action is GameActionPlayAnimation gameActionPlayAnimation))
			{
				return false;
			}
			PlayAnimation(gameActionPlayAnimation.AnimationName);
			return true;
		case DefAction.ENABLE_AI:
			if (!(action is GameActionEnableAI))
			{
				return false;
			}
			EnableAIControl(enable: true);
			return true;
		case DefAction.PLAY_CUTSCENE:
		{
			GameActionPlayCutscene gameActionPlayCutscene = action as GameActionPlayCutscene;
			if (gameActionPlayCutscene == null)
			{
				return false;
			}
			MsgErrorCode msgErrorCode = PlayCutScene(gameActionPlayCutscene.CutsceneName, gameActionPlayCutscene.NeedToBroadcast);
			if (msgErrorCode != MsgErrorCode.Success)
			{
				Logger.RError($"[RunEventActionInternal] PlayCutScene failed. CutsceneName: {gameActionPlayCutscene.CutsceneName}, ErrorCode: {msgErrorCode}");
				return false;
			}
			gameActionPlayCutscene.RegisterCompleteDelegate(() => IsCutSceneComplete(gameActionPlayCutscene.CutsceneName));
			return true;
		}
		case DefAction.WAIT:
			if (!(action is GameActionWait gameActionWait))
			{
				return false;
			}
			gameActionWait.Progress();
			return true;
		case DefAction.ARCHIVE_LOOTING_OBJECT:
			if (!(action is GameActionArchiveLootingObject))
			{
				return false;
			}
			if (!(this is MaintenanceRoom maintenanceRoom4))
			{
				return false;
			}
			maintenanceRoom4.ArchiveCollectingVolume();
			return true;
		case DefAction.SETINVINCIBLE:
		{
			if (!(action is GameActionSetInvincible gameActionSetInvincible))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor7))
			{
				return false;
			}
			VActor vActor5 = FindActorByObjectID(gameActionParamActor7.ActorID);
			if (vActor5 == null)
			{
				return false;
			}
			if (!(vActor5 is VCreature vCreature2))
			{
				return false;
			}
			vCreature2.SetInvincible(gameActionSetInvincible.Period);
			return true;
		}
		case DefAction.REMOVE_FIELD_SKILL_NEARBY:
			if (!(action is GameActionRemoveFieldSkillNearby gameActionRemoveFieldSkillNearby))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamPosition gameActionParamPosition3))
			{
				return false;
			}
			foreach (FieldSkillObject item in GetFieldSkillObjectsInRange(gameActionParamPosition3.Position.toVector3(), 0f, gameActionRemoveFieldSkillNearby.Radius))
			{
				PendRemoveActor(item.ObjectID);
			}
			return true;
		case DefAction.REBUILD_NAVMESH:
			if (!(action is GameActionRebuildNavMesh))
			{
				return false;
			}
			Hub.s.navman.Rebuild();
			return true;
		case DefAction.CHANGE_TRAM_PARTS:
			if (!(action is GameActionChangeTramParts))
			{
				return false;
			}
			if (!(this is MaintenanceRoom maintenanceRoom3))
			{
				return false;
			}
			maintenanceRoom3.ApplyTramUpgrage();
			return true;
		case DefAction.RESPAWN_ARCHIVED_OBJECT:
			if (!(action is GameActionRespawnArchivedObject))
			{
				return false;
			}
			if (!(this is MaintenanceRoom maintenanceRoom2))
			{
				return false;
			}
			maintenanceRoom2.RespawnCollectingVolume();
			return true;
		case DefAction.INSTANT_KILL:
		{
			if (!(action is GameActionInstantKill gameActionInstantKill))
			{
				return false;
			}
			ILevelObjectInfo levelObjectInfo2 = FindLevelObjectByName(gameActionInstantKill.LevelObjectName);
			if (levelObjectInfo2 == null)
			{
				Logger.RError("[RunEventActionInternal] INSTANT_KILL failed, FindLevelObjectByName failed. LevelObjectName: " + gameActionInstantKill.LevelObjectName);
				return false;
			}
			if (!(this is DungeonRoom dungeonRoom))
			{
				return false;
			}
			dungeonRoom.ApplyInstantKillInBounds(levelObjectInfo2.DataOrigin.BoundElementImmutableList);
			return true;
		}
		case DefAction.CHARGE_ITEM:
		{
			if (!(action is GameActionChargeItem))
			{
				return false;
			}
			if (param == null || !(param is GameActionParamActor gameActionParamActor))
			{
				return false;
			}
			VActor vActor = FindActorByObjectID(gameActionParamActor.ActorID);
			if (vActor == null)
			{
				return false;
			}
			if (!(vActor is VPlayer vPlayer))
			{
				return false;
			}
			vPlayer.InventoryControlUnit?.ChargeItem();
			return true;
		}
		default:
			return false;
		}
	}

	public ILevelObjectInfo? FindLevelObjectByType(Vector3 pos, double distance, LevelObjectClientType type, bool checkHeight)
	{
		return (from x in _levelObjects
			where (x.Value.DataOrigin.LevelObjectType == type || type == LevelObjectClientType.All) && !_closedTramUpgradeObject.Contains(x.Value.ID) && ((checkHeight && Math.Abs(x.Value.Pos.y - pos.y) <= (float)Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold) || !checkHeight) && (double)Misc.Distance(x.Value.Pos, pos) < distance
			select x.Value).FirstOrDefault();
	}

	public List<ILevelObjectInfo> FindLevelObjectsByType(Vector3 originPos, LevelObjectClientType type, bool checkHeight, AIRangeType rangeType)
	{
		return (from x in _levelObjects
			where x.Value.DataOrigin.LevelObjectType == type && !_closedTramUpgradeObject.Contains(x.Value.ID) && ((checkHeight && Math.Abs(x.Value.Pos.y - originPos.y) <= (float)Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold) || !checkHeight)
			orderby (rangeType != AIRangeType.ByNavMesh) ? ((double)Misc.Distance(x.Value.Pos, originPos)) : GetDistanceByNavMesh(x.Value.Pos, originPos)
			select x.Value).ToList();
	}

	public ILevelObjectInfo? FindLevelObjectByName(string name)
	{
		return _levelObjects.Values.FirstOrDefault((ILevelObjectInfo lo) => lo.Name == name);
	}

	protected abstract long GetContaRecoveryRate();

	public void TriggerSpawnByEvent(string key)
	{
		SpawnedActorData spawnedActorData = FindSpawnDataByKey(key);
		if (spawnedActorData != null)
		{
			if (spawnedActorData.SpawnWaitTime > 0)
			{
				ProcessDelayedSpawn(spawnedActorData);
			}
			else
			{
				ProcessImmediateSpawn(spawnedActorData);
			}
		}
	}

	private SpawnedActorData FindSpawnDataByKey(string key)
	{
		return _spawnedActorDatas.Values.FirstOrDefault((SpawnedActorData data) => data.ActorID == 0 && data.SpawnType == SpawnType.EventAction && data.Name == key);
	}

	private void ProcessDelayedSpawn(SpawnedActorData spawnedActorData)
	{
		_eventTimer.CreateTimerEvent(delegate
		{
			ExecuteSpawn(spawnedActorData);
		}, spawnedActorData.SpawnWaitTime);
		if (spawnedActorData.MarkerType != MapMarkerType.FieldSkill)
		{
			spawnedActorData.SetActorID(999999);
		}
	}

	private void ProcessImmediateSpawn(SpawnedActorData spawnedActorData)
	{
		ExecuteSpawn(spawnedActorData);
	}

	private void ExecuteSpawn(SpawnedActorData spawnedActorData)
	{
		switch (spawnedActorData.MarkerType)
		{
		case MapMarkerType.Creature:
			ExecuteCreatureSpawn(spawnedActorData);
			break;
		case MapMarkerType.LootingObject:
			ExecuteLootingObjectSpawn(spawnedActorData);
			break;
		case MapMarkerType.FieldSkill:
			ExecuteFieldSkillSpawn(spawnedActorData);
			break;
		default:
			Logger.RError($"TriggerSpawnByEvent failed, Invalid spawn point type. masterID: {spawnedActorData.MasterID}");
			break;
		}
	}

	private void ExecuteCreatureSpawn(SpawnedActorData spawnedActorData)
	{
		if (spawnedActorData.CanSpawn() && !SpawnMonster(spawnedActorData.MasterID, spawnedActorData, spawnedActorData.IsIndoor, spawnedActorData.AIName, spawnedActorData.BTName, ReasonOfSpawn.EventAction))
		{
			Logger.RError($"TriggerSpawnByEvent failed, SpawnMonster failed. masterID: {spawnedActorData.MasterID}");
		}
	}

	private void ExecuteLootingObjectSpawn(SpawnedActorData spawnedActorData)
	{
		if (spawnedActorData.MasterID <= 0 || !spawnedActorData.CanSpawn())
		{
			return;
		}
		if (Hub.s.dataman.ExcelDataManager.GetItemInfo(spawnedActorData.MasterID) == null)
		{
			Logger.RError($"TriggerSpawnByEvent failed, GetItemInfo failed. masterID: {spawnedActorData.MasterID}");
			return;
		}
		ItemElement itemElement = null;
		if (itemElement == null)
		{
			Logger.RError($"TriggerSpawnByEvent failed, targetItem is null. masterID: {spawnedActorData.MasterID}");
			return;
		}
		int num = SpawnLootingObject(itemElement, spawnedActorData.Pos, spawnedActorData.IsIndoor, ReasonOfSpawn.EventAction);
		if (num == 0)
		{
			Logger.RError($"TriggerSpawnByEvent failed, SpawnLootingObject failed. masterID: {spawnedActorData.MasterID}");
		}
		else
		{
			spawnedActorData.SetActorID(num);
		}
	}

	private void ExecuteFieldSkillSpawn(SpawnedActorData spawnedActorData)
	{
		if (spawnedActorData.MasterID > 0 && spawnedActorData.CanSpawn())
		{
			if (Hub.s.dataman.ExcelDataManager.GetFieldSkillData(spawnedActorData.MasterID) == null)
			{
				Logger.RError($"TriggerSpawnByEvent failed, GetFieldSkillInfo failed. masterID: {spawnedActorData.MasterID}");
			}
			else
			{
				SpawnFieldSkill(spawnedActorData.MasterID, spawnedActorData.Pos, spawnedActorData.IsIndoor, null, null, null, ReasonOfSpawn.EventAction);
			}
		}
	}

	public void AddGameEventLog(IGameEventLog log)
	{
		_logDumps.Add(log);
	}

	public List<IGameEventLog> GetGameEventLogs()
	{
		List<IGameEventLog> list = new List<IGameEventLog>();
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			list.AddRange(value.GetGameEventLogs());
		}
		list.AddRange(_logDumps);
		_logDumps.Clear();
		return list;
	}

	public int SpawnFieldSkill(int masterID, PosWithRot pos, bool isIndoor, Vector3? surfaceNormal, VActor? target, ISkillContext? skillContext, ReasonOfSpawn reasonOfSpawn)
	{
		if (Hub.s.dataman.ExcelDataManager.GetFieldSkillData(masterID) == null)
		{
			return 0;
		}
		PosWithRot pos2 = ((target != null) ? target.GetPosition() : pos);
		List<FieldSkillObject> list = CreateFieldSkillObject(masterID, pos2, isIndoor, surfaceNormal, skillContext, reasonOfSpawn);
		if (list.Count == 0)
		{
			return 0;
		}
		return list[0].ObjectID;
	}

	public void ReserveCreateMonster(long waitTime, PosWithRot pos, int monsterMasterID, bool isIndoor, ReasonOfSpawn reasonOfSpawn)
	{
		_eventTimer.CreateTimerEvent(delegate
		{
			if (CreateMonster(monsterMasterID, pos, isIndoor, "", "", reasonOfSpawn) == null)
			{
				Logger.RError($"CreateMonster by reserve failed. monsterMasterID: {monsterMasterID}");
			}
		}, waitTime);
	}

	protected bool SpawnMonster(int masterID, SpawnedActorData spawnData, bool isIndoor, string aiName = "", string btName = "", ReasonOfSpawn reasonOfSpawn = ReasonOfSpawn.Spawn)
	{
		VMonster vMonster = CreateMonster(masterID, spawnData.Pos, isIndoor, aiName, btName, reasonOfSpawn);
		if (vMonster == null)
		{
			return false;
		}
		spawnData.SetActorID(vMonster.ObjectID);
		vMonster.SetSpawnPointIndex(spawnData.Index);
		return true;
	}

	public void KillAllMonster()
	{
		IterateAllActor(delegate(VActor actor)
		{
			if (actor is VMonster vMonster)
			{
				vMonster.ForcedDying();
			}
		});
	}

	public void KillAllPlayer()
	{
		IterateAllPlayer(delegate(VPlayer player)
		{
			player.ForcedDying();
		});
	}
}
