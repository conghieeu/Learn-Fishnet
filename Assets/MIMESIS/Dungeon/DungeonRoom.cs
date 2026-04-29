using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluServerBase.Threading;
using UnityEngine;

public class DungeonRoom : IVroom
{
	private DungeonState _state;

	private long _sessionEndTime;

	private long _currentTime;

	private long _elapsedTime;

	private AtomicFlag _gameComplete = new AtomicFlag(value: false);

	private TimeSpan _prevSyncTime;

	private DungeonMasterInfo _dungeonMasterInfo;

	private AtomicFlag _triggered = new AtomicFlag(value: false);

	private int _normalMonsterThreatLimit;

	private int _normalMonsterThreatRemain;

	private long _lastNormalMonsterSpawnTime;

	private int _normalMonsterSpawnThreatMinThreshold = int.MaxValue;

	private int _mimicSpawnCountMax;

	private int _mimicSpawnCountRemain;

	private long _lastMimicSpawnTime;

	private DungeonWeather _weather;

	private long _blackoutTime;

	private long _lastReportEventSec;

	private bool _customSpawnOnStartMapProcessed;

	private bool _prevDungeonFailed;

	public DungeonRoom(VRoomManager roomManager, long roomID, IVRoomProperty property)
		: base(roomManager, roomID, property)
	{
		DungeonProperty dungeonProperty = property as DungeonProperty;
		DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(dungeonProperty.DungeonMasterID);
		if (dungeonInfo == null)
		{
			throw new Exception("Invalid DungeonMasterID on creating VGameRoom");
		}
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		_sessionEndTime = Hub.s.timeutil.ChangeTimeSec2Milli(dungeonInfo.Duration) + currentTickMilliSec;
		_currentTime = currentTickMilliSec;
		_dungeonMasterInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo((Property as DungeonProperty).DungeonMasterID);
		if (_dungeonMasterInfo == null)
		{
			Logger.RError("Invalid DungeonMasterID");
		}
		_weather = new DungeonWeather(dungeonProperty.DungeonMasterID, dungeonProperty.RandomDungeonSeed);
		if (_weather == null)
		{
			Logger.RError("Invalid weather");
		}
		_normalMonsterThreatLimit = SimpleRandUtil.Next(_dungeonMasterInfo.ThreatMin, _dungeonMasterInfo.ThreatMax);
		_normalMonsterThreatRemain = _normalMonsterThreatLimit;
		_mimicSpawnCountMax = SimpleRandUtil.Next(_dungeonMasterInfo.MimicSpawnCountMin, _dungeonMasterInfo.MimicSpawnCountMax);
		_mimicSpawnCountRemain = _mimicSpawnCountMax;
		_lastNormalMonsterSpawnTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		_lastMimicSpawnTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (_dungeonMasterInfo.SpawnableMonsterInfo == null)
		{
			return;
		}
		foreach (KeyValuePair<int, int> item in _dungeonMasterInfo.SpawnableMonsterInfo.MonsterRateDict)
		{
			MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(item.Key);
			if (monsterInfo == null)
			{
				Logger.RError($"Invalid monsterID: {item}");
			}
			else if (monsterInfo.ThreatValue > 0 && _normalMonsterSpawnThreatMinThreshold > monsterInfo.ThreatValue)
			{
				_normalMonsterSpawnThreatMinThreshold = monsterInfo.ThreatValue;
			}
		}
	}

	public override bool Initialize()
	{
		base.Initialize();
		SetDungeonState(DungeonState.OnPlaying);
		return true;
	}

	public override void InitSpawn()
	{
		base.InitSpawn();
		foreach (KeyValuePair<int, MapMarker_LootingObjectSpawnPoint> item in from x in Hub.s.dynamicDataMan.GetAllLootingObjectSpawnPoints()
			orderby Guid.NewGuid()
			select x)
		{
			if (_spawnedActorDatas.ContainsKey(item.Value.ID) || item.Value.masterID != 0 || _dungeonMasterInfo.SpawnableItemInfo == null)
			{
				continue;
			}
			ImmutableDictionary<int, (int, int)>.Builder builder = ImmutableDictionary.CreateBuilder<int, (int, int)>();
			foreach (KeyValuePair<int, int> item2 in _dungeonMasterInfo.SpawnableItemInfo.MiscRateDict)
			{
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(item2.Key);
				if (itemInfo == null)
				{
					Logger.RError($"InitSpawn failed, GetItemInfo failed. masterID: {item2.Key}");
				}
				else
				{
					builder.Add(item2.Key, (item2.Value, itemInfo.GetMeanPrice()));
				}
			}
			RandomSpawnedItemActorData value = new RandomSpawnedItemActorData(item.Value, builder.ToImmutable());
			_spawnedActorDatas.Add(item.Value.ID, value);
		}
		foreach (KeyValuePair<int, MapMarker_CreatureSpawnPoint> item3 in from x in Hub.s.dynamicDataMan.GetAllMonsterSpawnPoints()
			orderby Guid.NewGuid()
			select x)
		{
			if (_spawnedActorDatas.ContainsKey(item3.Value.ID) || item3.Value.masterID != 0 || item3.Value.spawnType != SpawnType.Periodic || _dungeonMasterInfo.SpawnableMonsterInfo == null)
			{
				continue;
			}
			ImmutableDictionary<int, (int, int)>.Builder builder2 = ImmutableDictionary.CreateBuilder<int, (int, int)>();
			foreach (KeyValuePair<int, int> item4 in _dungeonMasterInfo.SpawnableMonsterInfo.MonsterRateDict)
			{
				MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(item4.Key);
				if (monsterInfo == null)
				{
					Logger.RError($"InitSpawn failed, GetMonsterInfo failed. masterID: {item4.Key}");
				}
				else
				{
					builder2.Add(item4.Key, (item4.Value, monsterInfo.ThreatValue));
				}
			}
			RandomSpawnedMonsterActorData value2 = new RandomSpawnedMonsterActorData(item3.Value, builder2.ToImmutable(), _dungeonMasterInfo.SpawnableMonsterInfo.MaxRate);
			_spawnedActorDatas.Add(item3.Value.ID, value2);
		}
		foreach (KeyValuePair<int, MapMarker_FieldSkillSpawnPoint> item5 in from x in Hub.s.dynamicDataMan.GetAllFieldSkillSpawnPoints()
			orderby Guid.NewGuid()
			select x)
		{
			if (!_spawnedActorDatas.ContainsKey(item5.Value.ID) && item5.Value.masterID == 0)
			{
				RandomSpawnedFieldSkillActorData value3 = new RandomSpawnedFieldSkillActorData(item5.Value);
				_spawnedActorDatas.Add(item5.Value.ID, value3);
			}
		}
	}

	public override void OnActorExit(VActor actor)
	{
		base.OnActorExit(actor);
		if (actor.SpawnPointIndex == 0 || !_spawnedActorDatas.TryGetValue(actor.SpawnPointIndex, out SpawnedActorData _) || !(actor is VMonster vMonster))
		{
			return;
		}
		MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(vMonster.MasterID);
		if (monsterInfo == null)
		{
			Logger.RError($"OnActorExit failed, GetMonsterInfo failed. masterID: {vMonster.MasterID}");
		}
		else if (monsterInfo.MonsterType == MonsterType.Mimic)
		{
			if (actor.SpawnPointIndex > 0 && _spawnedActorDatas[actor.SpawnPointIndex].MasterID == 0)
			{
				_mimicSpawnCountRemain++;
			}
		}
		else if (monsterInfo.ThreatValue > 0 && actor.SpawnPointIndex > 0 && _spawnedActorDatas[actor.SpawnPointIndex].MasterID == 0)
		{
			_normalMonsterThreatRemain += monsterInfo.ThreatValue;
		}
	}

	public override void ManageSpawnData()
	{
		base.ManageSpawnData();
		if (!_customSpawnOnStartMapProcessed)
		{
			int num = 0;
			int num2 = SimpleRandUtil.Next(_dungeonMasterInfo.MiscMinVal, _dungeonMasterInfo.MiscMaxVal);
			if (_prevDungeonFailed && _currentDay == 3 && _currentSessionCount <= Hub.s.dataman.ExcelDataManager.Consts.C_ScrapBoostMaxSession)
			{
				num2 = (int)((double)(num2 * Hub.s.dataman.ExcelDataManager.Consts.C_ScrapBoostValue) * 0.0001);
			}
			foreach (SpawnedActorData item in _spawnedActorDatas.Values.Where((SpawnedActorData x) => x.MarkerType == MapMarkerType.LootingObject && x.SpawnType == SpawnType.OnStartMap))
			{
				if (item.ActorID != 0 || item.MasterID > 0 || !item.CanSpawn())
				{
					continue;
				}
				if (num2 <= 0)
				{
					break;
				}
				(int, int, int) pickedItemValue = (item as RandomSpawnedItemActorData).GetPickedItemValue(num2);
				if (pickedItemValue.Item1 == 0)
				{
					break;
				}
				if (Hub.s.dataman.ExcelDataManager.GetItemInfo(pickedItemValue.Item1) == null)
				{
					Logger.RError($"ManageSpawnData failed, GetItemInfo failed. masterID: {item.MasterID}");
					continue;
				}
				ItemElement newItemElement = GetNewItemElement(pickedItemValue.Item1, isFake: false, item.StackCount, item.Durability, item.DefaultGauge);
				if (newItemElement == null)
				{
					Logger.RError($"ManageSpawnData failed, targetItem is null. masterID: {item.MasterID}");
					continue;
				}
				int num3 = SpawnLootingObject(newItemElement, item.Pos, item.IsIndoor, ReasonOfSpawn.Spawn, item.Index);
				if (num3 == 0)
				{
					Logger.RError($"ManageSpawnData failed, SpawnLootingObject failed. masterID: {item.MasterID}");
					continue;
				}
				item.SetActorID(num3);
				num2 = pickedItemValue.Item2;
				num++;
			}
			foreach (SpawnedActorData item2 in _spawnedActorDatas.Values.Where((SpawnedActorData x) => x.MarkerType == MapMarkerType.FieldSkill && x.SpawnType == SpawnType.OnStartMap))
			{
				if (item2.ActorID != 0 || item2.MasterID == 0 || !item2.CanSpawn())
				{
					continue;
				}
				FieldSkillInfo fieldSkillData = Hub.s.dataman.ExcelDataManager.GetFieldSkillData(item2.MasterID);
				if (fieldSkillData == null)
				{
					Logger.RError($"ManageSpawnData failed, GetFieldSkillData failed. masterID: {item2.MasterID}");
					continue;
				}
				int num4 = SpawnFieldSkill(fieldSkillData.MasterID, item2.Pos, item2.IsIndoor, item2.SurfaceNormalVector, null, null, ReasonOfSpawn.Spawn);
				if (num4 == 0)
				{
					Logger.RError($"ManageSpawnData failed, SpawnFieldSkill failed. masterID: {item2.MasterID}");
				}
				else
				{
					item2.SetActorID(num4);
				}
			}
			_customSpawnOnStartMapProcessed = true;
		}
		if (_spawnHoldDebug)
		{
			return;
		}
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (_lastNormalMonsterSpawnTime + _dungeonMasterInfo.NormalMonsterSpawnPeriod < currentTickMilliSec && _normalMonsterThreatRemain >= _normalMonsterSpawnThreatMinThreshold)
		{
			List<SpawnedActorData> list = (from x in _spawnedActorDatas.Values
				where x.MarkerType == MapMarkerType.Creature && x.SpawnType == SpawnType.Periodic
				orderby Guid.NewGuid()
				select x).ToList();
			int normalMonsterSpawnTryCount = _dungeonMasterInfo.NormalMonsterSpawnTryCount;
			while (normalMonsterSpawnTryCount-- > 0 && _normalMonsterSpawnThreatMinThreshold <= _normalMonsterThreatRemain)
			{
				if (SimpleRandUtil.Next(0, 10001) > _dungeonMasterInfo.NormalMonsterSpawnRate)
				{
					continue;
				}
				int num5 = _normalMonsterThreatRemain;
				int num6 = 0;
				foreach (SpawnedActorData item3 in list)
				{
					if (!item3.CanSpawn())
					{
						continue;
					}
					if (num5 <= 0)
					{
						break;
					}
					if (!IsPlayerInSpawnSightRange(item3.PosVector))
					{
						(num6, num5, _) = (item3 as RandomSpawnedMonsterActorData).GetPickedMonsterValue(_normalMonsterThreatRemain);
						if (num6 == 0)
						{
							break;
						}
						if (SpawnMonster(num6, item3, item3.IsIndoor, item3.AIName, item3.BTName))
						{
							_normalMonsterThreatRemain = num5;
							Hub.s.dataman.ExcelDataManager.GetMonsterInfo(num6);
							break;
						}
						Logger.RError($"ManageSpawnData failed, SpawnMonster failed. masterID: {item3.MasterID}");
					}
				}
			}
			_lastNormalMonsterSpawnTime = currentTickMilliSec;
		}
		if (_lastMimicSpawnTime + _dungeonMasterInfo.MimicSpawnPeriod >= currentTickMilliSec || _mimicSpawnCountRemain <= 0)
		{
			return;
		}
		List<SpawnedActorData> list2 = (from x in _spawnedActorDatas.Values
			where x.MarkerType == MapMarkerType.Creature && x.SpawnType == SpawnType.Periodic
			orderby Guid.NewGuid()
			select x).ToList();
		int mimicSpawnTryCount = _dungeonMasterInfo.MimicSpawnTryCount;
		while (mimicSpawnTryCount-- > 0 && _mimicSpawnCountRemain > 0)
		{
			if (SimpleRandUtil.Next(0, 10001) > _dungeonMasterInfo.MimicSpawnRate)
			{
				continue;
			}
			foreach (SpawnedActorData item4 in list2)
			{
				if (item4.CanSpawn() && !IsPlayerInSpawnSightRange(item4.PosVector))
				{
					if (SpawnMonster(20000001, item4, item4.IsIndoor, item4.AIName, item4.BTName))
					{
						_mimicSpawnCountRemain--;
						break;
					}
					Logger.RError("ManageSpawnData failed about mimic, SpawnMonster failed. ");
				}
			}
		}
		_lastMimicSpawnTime = currentTickMilliSec;
	}

	private void SetDungeonState(DungeonState state)
	{
		if (_state == state)
		{
			return;
		}
		_state = state;
		if (state != DungeonState.Failed && state != DungeonState.Success)
		{
			return;
		}
		EnableAIControl(enable: false);
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (value.Wasted)
			{
				value.InventoryControlUnit.ClearAllInventory();
			}
		}
		Dictionary<int, AwardType> playerAwards = PlayReportData.EvaluatePlayerAwards(_playReportDict);
		ProcessDisappearedItems();
		SendToAllPlayers(new EndDungeonSig
		{
			result = new GameRoomResult
			{
				success = (state == DungeonState.Success),
				playerStatus = _vPlayerDict.Values.ToDictionary((VPlayer x) => x.ObjectID, (VPlayer x) => (GetPlayerResultStatus(x), GetPlayAward(x))),
				removedItems = _disappearedItems.Select((ItemElement x) => x.toItemInfo()).ToList()
			}
		});
		_eventTimer.CreateTimerEvent(delegate
		{
			OnDecideRoom(delegate
			{
				Hub.s.vworld.OnFinishDungeon(ExtractRoomInfo(includeDisappearItem: false), GetGameEventLogs(), _state == DungeonState.Success);
			});
		}, Hub.s.dataman.ExcelDataManager.Consts.C_UI_PopupDurationDungeonStartLever + Hub.s.dataman.ExcelDataManager.Consts.C_UI_PopupDurationSurvivorReport);
		_gameComplete.On();
		AwardType GetPlayAward(VPlayer player)
		{
			if (playerAwards.ContainsKey(player.ObjectID))
			{
				return playerAwards[player.ObjectID];
			}
			return AwardType.None;
		}
		static PlayerResultStatus GetPlayerResultStatus(VPlayer player)
		{
			if (player.ReasonOfDeath != ReasonOfDeath.None)
			{
				return PlayerResultStatus.Dead;
			}
			if (player.Wasted)
			{
				return PlayerResultStatus.Wasted;
			}
			return PlayerResultStatus.Alived;
		}
	}

	public override void ApplyBaseGameSessionInfo(GameSessionInfo gameSessionInfo)
	{
		base.ApplyBaseGameSessionInfo(gameSessionInfo);
		_prevDungeonFailed = !gameSessionInfo.PrevDungeonSuccess;
	}

	public override void OnUpdate(long delta)
	{
		_currentTime += delta;
		_elapsedTime += delta;
		base.OnUpdate(delta);
		if (_state != DungeonState.OnPlaying)
		{
			return;
		}
		if (_currentTime >= _sessionEndTime && !_gameComplete.IsOn)
		{
			TimeSyncSig msg = new TimeSyncSig
			{
				currentTime = new TimeSpan(24, 0, 0),
				currentWeatherMasterID = 2,
				forecastWeatherMasterID = 2
			};
			SendToAll(msg);
			CheckComplete(DungeonCompleteCheckType.TimeOver);
			return;
		}
		double num = (double)_elapsedTime * 0.001 * ((double)Hub.s.dataman.ExcelDataManager.Consts.C_GameTimeScaleFactor * 0.001);
		long num2 = VWorldUtil.ConvertTimeToSeconds(_dungeonMasterInfo.StartDisplayTime);
		TimeSpan timeSpan = TimeSpan.FromSeconds(num + (double)num2);
		if (_prevSyncTime.Hours != timeSpan.Hours)
		{
			_prevSyncTime = timeSpan;
			SkyAndWeatherSystem.eWeatherPreset currentWeatherPreset = base.CurrentWeatherPreset;
			int weatherMasterID = _weather.GetWeatherMasterID(timeSpan.Hours);
			WeatherInfo weatherInfo = Hub.s.dataman.ExcelDataManager.GetWeatherInfo(weatherMasterID);
			if (weatherInfo != null)
			{
				base.CurrentWeatherPreset = weatherInfo.WeatherPreset;
				if (_prevSyncTime.Hours == 0)
				{
					currentWeatherPreset = base.CurrentWeatherPreset;
				}
			}
			TimeSyncSig msg2 = new TimeSyncSig
			{
				currentTime = timeSpan,
				currentWeatherMasterID = weatherMasterID,
				forecastWeatherMasterID = _weather.GetWeatherForecastMasterID(timeSpan.Hours)
			};
			SendToAll(msg2);
			_ = base.CurrentWeatherPreset;
			CheckBlackout(currentWeatherPreset != base.CurrentWeatherPreset, weatherInfo.BlackoutRate, ReasonOfBlackout.Weather);
		}
		long currentTickSec = Hub.s.timeutil.GetCurrentTickSec();
		if (_lastReportEventSec == currentTickSec)
		{
			return;
		}
		_lastReportEventSec = currentTickSec;
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (CheckStartingArea(value))
			{
				IncreaseTimeInStartingVolume(value, 1L);
			}
		}
	}

	public override void OnActorEvent(VActorEventArgs args)
	{
		base.OnActorEvent(args);
		if (!_gameComplete.IsOn && args is GameActorDeadEventArgs e && e.Victim is VPlayer)
		{
			_eventTimer.CreateTimerEvent(delegate
			{
				CheckComplete(DungeonCompleteCheckType.PlayerDead);
			}, 1000L);
		}
	}

	public override MsgErrorCode OnEnterChannel(VPlayer player, int hashCode)
	{
		base.OnEnterChannel(player, hashCode);
		EnterDungeonRes enterDungeonRes = new EnterDungeonRes(hashCode)
		{
			shopGroupID = _dungeonMasterInfo.ShopGroupID,
			currentWeatherMasterID = _weather.GetWeatherMasterID(_prevSyncTime.Hours),
			forecastWeatherMasterID = _weather.GetWeatherForecastMasterID(_prevSyncTime.Hours)
		};
		PlayerInfo info = enterDungeonRes.playerInfo;
		player.GetMyActorInfo(ref info);
		player.SendToMe(enterDungeonRes);
		return MsgErrorCode.Success;
	}

	protected override MsgErrorCode CanEnterChannel(long playerUID)
	{
		if (_state != DungeonState.Ready && _state != DungeonState.OnPlaying)
		{
			Logger.RError($"ProcessEnterWaitQueue failed, room state is not ready. state: {_state}");
			return MsgErrorCode.InvalidRoomState;
		}
		return base.CanEnterChannel(playerUID);
	}

	private bool CheckInBoundOnTrigger()
	{
		TriggerVolumeData triggerVolumeData = _playerStartingVolumes.FirstOrDefault();
		if (triggerVolumeData == null)
		{
			Logger.RError("There is no start trigger volume");
			return false;
		}
		foreach (VPlayer value in _vPlayerDict.Values)
		{
			if (!triggerVolumeData.IsInBounds(value.PositionVector) && value.IsAliveStatus())
			{
				value.SetWasted();
			}
		}
		foreach (VPlayer value2 in _vPlayerDict.Values)
		{
			if (!value2.Wasted && triggerVolumeData.IsInBounds(value2.PositionVector))
			{
				SetDungeonState(DungeonState.Success);
				return true;
			}
		}
		SetDungeonState(DungeonState.Failed);
		return true;
	}

	public bool CheckComplete(DungeonCompleteCheckType type)
	{
		if (_gameComplete.IsOn)
		{
			return false;
		}
		switch (type)
		{
		case DungeonCompleteCheckType.PlayerDead:
			if (IsAllPlayerDead())
			{
				SetDungeonState(DungeonState.Failed);
				return true;
			}
			break;
		case DungeonCompleteCheckType.TimeOver:
			return CheckInBoundOnTrigger();
		case DungeonCompleteCheckType.Triggered:
		{
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (currentTickMilliSec - _resumeTime < 1000)
			{
				Logger.RError($"OnRequestStartGame failed, not enough time passed. currentTime: {currentTickMilliSec}, _resumeTime: {_resumeTime}");
				return false;
			}
			if (!_triggered.On())
			{
				return false;
			}
			return CheckInBoundOnTrigger();
		}
		}
		return true;
	}

	public void OverwriteEndTime(long plusTime)
	{
		_sessionEndTime = _currentTime + plusTime;
	}

	public TimeSpan GetCurrentTime()
	{
		long num = VWorldUtil.ConvertTimeToSeconds(_dungeonMasterInfo.StartDisplayTime);
		long c_GameTimeScaleFactor = Hub.s.dataman.ExcelDataManager.Consts.C_GameTimeScaleFactor;
		return TimeSpan.FromSeconds((double)_elapsedTime * 0.001 * ((double)c_GameTimeScaleFactor * 0.001) + (double)num);
	}

	public override int GetContaValue(VActor actor, bool isRun)
	{
		int num = (isRun ? _dungeonMasterInfo.ContaIncreaseValueRun : _dungeonMasterInfo.ContaIncreaseValueIdle);
		if (CheckContaSafePosition(actor))
		{
			return num;
		}
		float currentContaRate = _weather.GetCurrentContaRate(_prevSyncTime.Hours);
		return (int)((float)num * currentContaRate);
	}

	protected override long GetContaRecoveryRate()
	{
		return Hub.s.dataman.ExcelDataManager.Consts.C_ContaRecoveryValue;
	}

	public void ApplyInstantKillInBounds(ImmutableList<LevelObjectBoundElement> levelObjectBoundElements)
	{
		List<VActor> collection = ((IEnumerable<VActor>)_vPlayerDict.Values.Where((VPlayer x) => x.IsAliveStatus() && levelObjectBoundElements.Any((LevelObjectBoundElement b) => b.IsContainPosition(Vector3.MoveTowards(x.PositionVector, b.GetGlobalBottomCenterPos(x.PositionVector.y), x.HitCollisionRadius))))).ToList();
		List<VActor> source = _vActorDict.Values.Where((VActor x) => x is VCreature vCreature && vCreature.IsAliveStatus()).ToList();
		List<VActor> creaturesInBound = source.Where((VActor x) => levelObjectBoundElements.Any((LevelObjectBoundElement b) => b.IsContainPosition(Vector3.MoveTowards(x.PositionVector, b.GetGlobalBottomCenterPos(x.PositionVector.y), x.HitCollisionRadius)))).ToList();
		creaturesInBound.AddRange(collection);
		foreach (VActor item in creaturesInBound)
		{
			item.StatControlUnit?.AdjustHP(-item.StatControlUnit.GetCurrentHP());
		}
		List<Bounds> extentBounds = levelObjectBoundElements.Select((LevelObjectBoundElement x) => new Bounds(x.GetGlobalCenterPos(), x.GetGlobalSize() + new Vector3(6f, 0f, 6f))).ToList();
		foreach (VActor item2 in source.Where((VActor x) => !creaturesInBound.Contains(x) && extentBounds.Any((Bounds b) => b.Contains(x.PositionVector))).ToList())
		{
			item2.MovementControlUnit?.OnChangePosition();
		}
	}

	public void AdminChangeWeather(int weatherID)
	{
		_weather.SetWeatherOverrideAll(weatherID);
		_prevSyncTime = TimeSpan.Zero;
	}

	public void AdminAddWeatherScehdule(int weatherID, int startHour, int durationHour, bool forecast)
	{
		_weather.SetWeatherOverride(weatherID, startHour, durationHour, forecast);
		_prevSyncTime = TimeSpan.Zero;
	}

	public void AdminAddGametime(TimeSpan addTimeSpan)
	{
		long num = VWorldUtil.ConvertTimeToSeconds(_dungeonMasterInfo.StartDisplayTime);
		_ = _elapsedTime;
		double num2 = (double)_elapsedTime * 0.001 * ((double)Hub.s.dataman.ExcelDataManager.Consts.C_GameTimeScaleFactor * 0.001);
		TimeSpan.FromSeconds((double)num + num2);
		long num3 = (long)(addTimeSpan.TotalMilliseconds / ((double)Hub.s.dataman.ExcelDataManager.Consts.C_GameTimeScaleFactor * 0.001));
		_currentTime += num3;
		_elapsedTime += num3;
		double num4 = (double)_elapsedTime * 0.001 * ((double)Hub.s.dataman.ExcelDataManager.Consts.C_GameTimeScaleFactor * 0.001);
		TimeSpan.FromSeconds((double)num + num4);
	}

	public void ResetCurrentTime()
	{
		_currentTime = 0L;
	}

	protected override void OnEnterRoomFailed(SessionContext context, MsgErrorCode errorCode, int hashCode)
	{
		context.Send(new EnterDungeonRes(hashCode)
		{
			errorCode = errorCode
		});
	}

	public override void OnExitChannel(VPlayer player)
	{
		base.OnExitChannel(player);
		_eventTimer.CreateTimerEvent(delegate
		{
			CheckComplete(DungeonCompleteCheckType.PlayerDead);
		}, 0L);
	}

	public void CheckBlackout(bool needCheckNewBlackout, int blackoutRate, ReasonOfBlackout reasonOfBlackout, int ownerActorID = 0)
	{
		if (needCheckNewBlackout)
		{
			if (SimpleRandUtil.IsSuccessPerTenThousand(blackoutRate))
			{
				long num = SimpleRandUtil.Next(Hub.s.dataman.ExcelDataManager.Consts.C_BlackoutDurationMin, Hub.s.dataman.ExcelDataManager.Consts.C_BlackoutDurationMax);
				if (_blackoutTime != 0L)
				{
					_blackoutTime = Hub.s.timeutil.GetCurrentTickMilliSec() + num;
					return;
				}
				_blackoutTime = Hub.s.timeutil.GetCurrentTickMilliSec() + num;
				SendToAll(new BlackoutSig
				{
					isBlackout = true,
					reasonOfBlackout = reasonOfBlackout,
					ownerActorID = ownerActorID
				});
			}
		}
		else if (_blackoutTime != 0L && _blackoutTime <= Hub.s.timeutil.GetCurrentTickMilliSec())
		{
			_blackoutTime = 0L;
			SendToAll(new BlackoutSig
			{
				isBlackout = false
			});
		}
	}

	protected override void OnDecideRoom(Command command)
	{
		base.OnDecideRoom(command);
		Stop();
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"OnDecideRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		ImmutableArray<IGameAction> exitAction = cycleMasterInfo.DungeonCycleInfo.GetExitAction(_currentDay);
		if (exitAction.Length > 0)
		{
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator = exitAction.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IGameAction current = enumerator.Current;
				dictionary.Add(current.Clone(), null);
			}
			EnqueueEventAction(dictionary, 0);
		}
		BlockEventAction();
	}

	protected override void OnAllMemberEntered()
	{
		base.OnAllMemberEntered();
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"OnAllMemberEntered failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		ImmutableArray<IGameAction> enterAction = cycleMasterInfo.DungeonCycleInfo.GetEnterAction(_currentDay);
		if (enterAction.Length > 0)
		{
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator = enterAction.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IGameAction current = enumerator.Current;
				dictionary.Add(current.Clone(), null);
			}
			EnqueueEventAction(dictionary, 0);
			EnableAIControl(enable: false);
		}
	}

	protected override void RunEnterRoomCutScene()
	{
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"SendAllMembersEnterRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		List<string> enterCutSceneNames = cycleMasterInfo.DungeonCycleInfo.GetEnterCutSceneNames(_currentDay);
		Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
		foreach (string item in enterCutSceneNames)
		{
			dictionary.Add(new GameActionPlayCutscene(item, needToBroadCast: false), null);
		}
		dictionary.Add(new GameActionEnableAI(), null);
		EnqueueEventAction(dictionary, 0);
		EnableAIControl(enable: false);
	}

	protected override void SyncEnterRoom()
	{
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"SendAllMembersEnterRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		List<string> enterCutSceneNames = cycleMasterInfo.DungeonCycleInfo.GetEnterCutSceneNames(_currentDay);
		SendToAllPlayers(new AllMemberEnterRoomSig
		{
			enterCutsceneNames = enterCutSceneNames
		});
	}

	protected override void SyncEndCutScene()
	{
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"SyncEndCutScene failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		List<string> exitCutSceneNames = cycleMasterInfo.DungeonCycleInfo.GetExitCutSceneNames(_currentDay);
		SendToAllPlayers(new BeginEndRoomCutSceneSig
		{
			exitCutsceneNames = exitCutSceneNames
		});
		if (exitCutSceneNames.Count == 0)
		{
			Logger.RWarn($"SyncEndCutScene failed, cutsceneNames is empty. _currentDay: {_currentDay}");
		}
		Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
		foreach (string item in exitCutSceneNames)
		{
			dictionary.Add(new GameActionPlayCutscene(item, needToBroadCast: false), null);
		}
		EnqueueEventAction(dictionary, 0);
		EnableAIControl(enable: false);
	}

	public override bool DamageAppliable()
	{
		return true;
	}

	private bool IsPlayerInSpawnSightRange(Vector3 spawnPos)
	{
		if (!FindNearestPoly(spawnPos, out var nearestPos, 1f))
		{
			Logger.RWarn("[IsPlayerInSpawnSightRange] FindNearestPoly fail. SKIP!");
			return true;
		}
		List<(VActor, double)> playerActorsInRange = GetPlayerActorsInRange(spawnPos, 0f, Hub.s.dataman.ExcelDataManager.Consts.C_SpawnUnableFromRange, ignoreHeight: true);
		if (playerActorsInRange.Count > 0)
		{
			foreach (var item3 in playerActorsInRange)
			{
				VActor item = item3.Item1;
				double item2 = item3.Item2;
				double distanceByNavMesh = GetDistanceByNavMesh(nearestPos, item.PositionVector);
				if (distanceByNavMesh > 90000000.0 && item2 < (double)(Hub.s.dataman.ExcelDataManager.Consts.C_SpawnUnableFromRange / 2f))
				{
					Logger.RWarn("[IsPlayerInSpawnSightRange] GetDistanceByNavMesh() fail! SKIP!");
					return true;
				}
				if (distanceByNavMesh < (double)Hub.s.dataman.ExcelDataManager.Consts.C_SpawnUnableFromRange)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override VRoomType GetToMoveRoomType()
	{
		return VRoomType.Waiting;
	}

	public void HandleGetRemainScrapValue()
	{
		int num = 0;
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is VLootingObject { IsIndoor: not false } vLootingObject)
			{
				num += vLootingObject.GetItemElement().FinalPrice;
			}
		}
		SendToAllPlayers(new GetRemainScrapValueSig
		{
			remainValue = num
		});
	}
}
