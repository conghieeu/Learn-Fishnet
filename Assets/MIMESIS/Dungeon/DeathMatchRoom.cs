using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluServerBase.Threading;

public class DeathMatchRoom : IVroom
{
	private DeathMatchRoomState _state;

	private AtomicFlag _gameComplete = new AtomicFlag(value: false);

	private int _remainItemSpawnCount;

	private int _remainMonsterSpawnCount;

	private Dictionary<int, Dictionary<ActorType, List<(int, int, long)>>> _playerKillInfos = new Dictionary<int, Dictionary<ActorType, List<(int, int, long)>>>();

	private Dictionary<int, (long deadTime, ActorType actorType, int actorID, int masterID)> _playerDeadInfos = new Dictionary<int, (long, ActorType, int, int)>();

	private Dictionary<int, List<int>> _monsterKillInfos = new Dictionary<int, List<int>>();

	private Dictionary<int, DeathMatchPlayerResult> _gameResult = new Dictionary<int, DeathMatchPlayerResult>();

	public DeathMatchRoom(VRoomManager roomManager, long roomID, IVRoomProperty property)
		: base(roomManager, roomID, property)
	{
		_remainItemSpawnCount = Hub.s.dataman.ExcelDataManager.Consts.C_DeathmatchRoomSpawnableItemCount;
		_remainMonsterSpawnCount = Hub.s.dataman.ExcelDataManager.Consts.C_DeathmatchRoomSpawnableMonsterCount;
	}

	public override void InitSpawn()
	{
		base.InitSpawn();
		foreach (KeyValuePair<int, MapMarker_LootingObjectSpawnPoint> item in from x in Hub.s.dynamicDataMan.GetAllLootingObjectSpawnPoints()
			orderby Guid.NewGuid()
			select x)
		{
			if (_spawnedActorDatas.ContainsKey(item.Value.ID) || item.Value.masterID != 0)
			{
				continue;
			}
			SpawnableItemInfo spawnableItemData = Hub.s.dataman.ExcelDataManager.GetSpawnableItemData(Hub.s.dataman.ExcelDataManager.Consts.C_DeathmatchRoomSpawnableItemId);
			if (spawnableItemData == null)
			{
				Logger.RError($"InitSpawn failed, GetSpawnableItemData failed. masterID: {Hub.s.dataman.ExcelDataManager.Consts.C_DeathmatchRoomSpawnableItemId}");
				continue;
			}
			ImmutableDictionary<int, (int, int)>.Builder builder = ImmutableDictionary.CreateBuilder<int, (int, int)>();
			foreach (KeyValuePair<int, int> item2 in spawnableItemData.MiscRateDict)
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
			if (_spawnedActorDatas.ContainsKey(item3.Value.ID) || item3.Value.masterID != 0 || item3.Value.spawnType != SpawnType.OnStartMap)
			{
				continue;
			}
			SpawnableMonsterInfo spawnableMonsterData = Hub.s.dataman.ExcelDataManager.GetSpawnableMonsterData(Hub.s.dataman.ExcelDataManager.Consts.C_DeathmatchRoomSpawnableMonsterId);
			if (spawnableMonsterData == null)
			{
				Logger.RError($"InitSpawn failed, GetSpawnableMonsterData failed. masterID: {Hub.s.dataman.ExcelDataManager.Consts.C_DeathmatchRoomSpawnableMonsterId}");
				continue;
			}
			ImmutableDictionary<int, (int, int)>.Builder builder2 = ImmutableDictionary.CreateBuilder<int, (int, int)>();
			foreach (KeyValuePair<int, int> item4 in spawnableMonsterData.MonsterRateDict)
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
			RandomSpawnedMonsterActorData value2 = new RandomSpawnedMonsterActorData(item3.Value, builder2.ToImmutable(), spawnableMonsterData.MaxRate);
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
		EnableAIControl(enable: false);
	}

	public override void ManageSpawnData()
	{
		base.ManageSpawnData();
		foreach (SpawnedActorData item in _spawnedActorDatas.Values.Where((SpawnedActorData x) => x.MarkerType == MapMarkerType.LootingObject && x.SpawnType == SpawnType.OnStartMap))
		{
			if (item.ActorID != 0 || item.MasterID > 0 || !item.CanSpawn())
			{
				continue;
			}
			if (_remainItemSpawnCount <= 0)
			{
				break;
			}
			int pickedItemValue = (item as RandomSpawnedItemActorData).GetPickedItemValue();
			if (pickedItemValue == 0)
			{
				break;
			}
			if (Hub.s.dataman.ExcelDataManager.GetItemInfo(pickedItemValue) == null)
			{
				Logger.RError($"ManageSpawnData failed, GetItemInfo failed. masterID: {item.MasterID}");
				continue;
			}
			ItemElement newItemElement = GetNewItemElement(pickedItemValue, isFake: false, item.StackCount, item.Durability, item.DefaultGauge);
			if (newItemElement == null)
			{
				Logger.RError($"ManageSpawnData failed, targetItem is null. masterID: {item.MasterID}");
				continue;
			}
			int num = SpawnLootingObject(newItemElement, item.Pos, item.IsIndoor, ReasonOfSpawn.Spawn, item.Index);
			if (num == 0)
			{
				Logger.RError($"ManageSpawnData failed, SpawnLootingObject failed. masterID: {item.MasterID}");
				continue;
			}
			item.SetActorID(num);
			_remainItemSpawnCount--;
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
			int num2 = SpawnFieldSkill(fieldSkillData.MasterID, item2.Pos, item2.IsIndoor, item2.SurfaceNormalVector, null, null, ReasonOfSpawn.Spawn);
			if (num2 == 0)
			{
				Logger.RError($"ManageSpawnData failed, SpawnFieldSkill failed. masterID: {item2.MasterID}");
			}
			else
			{
				item2.SetActorID(num2);
			}
		}
		if (_spawnHoldDebug)
		{
			return;
		}
		Hub.s.timeutil.GetCurrentTickMilliSec();
		foreach (SpawnedActorData item3 in (from x in _spawnedActorDatas.Values
			where x.MarkerType == MapMarkerType.Creature && x.SpawnType == SpawnType.OnStartMap
			orderby Guid.NewGuid()
			select x).ToList())
		{
			if (item3.CanSpawn())
			{
				if (_remainMonsterSpawnCount <= 0)
				{
					break;
				}
				int pickedMonsterValue = (item3 as RandomSpawnedMonsterActorData).GetPickedMonsterValue();
				if (pickedMonsterValue == 0)
				{
					break;
				}
				if (!SpawnMonster(pickedMonsterValue, item3, item3.IsIndoor, item3.AIName, item3.BTName))
				{
					Logger.RError($"ManageSpawnData failed, SpawnMonster failed. masterID: {item3.MasterID}");
				}
				_remainMonsterSpawnCount--;
			}
		}
	}

	public override void OnUpdate(long delta)
	{
		base.OnUpdate(delta);
	}

	public override void OnActorEvent(VActorEventArgs args)
	{
		if (_gameComplete.IsOn || !(args is GameActorDeadEventArgs e))
		{
			return;
		}
		try
		{
			if (e.Victim is VPlayer)
			{
				VActor vActor = FindActorByObjectID(e.AttackerActorID);
				if (vActor != null)
				{
					if (vActor is VPlayer vPlayer && e.Victim != vPlayer)
					{
						if (!_playerKillInfos.TryGetValue(vPlayer.ObjectID, out var value))
						{
							Logger.RWarn($"OnActorEvent failed, player kill info not found. playerObjectID: {vPlayer.ObjectID}, playerObjID: {e.Victim.ObjectID}");
							return;
						}
						value[ActorType.Player].Add((e.Victim.MasterID, e.Victim.ObjectID, Hub.s.timeutil.GetCurrentTickMilliSec()));
					}
					else if (vActor is VMonster)
					{
						if (!_monsterKillInfos.TryGetValue(vActor.MasterID, out var value2))
						{
							value2 = new List<int>();
							_monsterKillInfos.Add(vActor.MasterID, value2);
						}
						value2.Add(e.Victim.ObjectID);
					}
					if (!_playerDeadInfos.ContainsKey(e.Victim.ObjectID))
					{
						_playerDeadInfos.Add(e.Victim.ObjectID, (Hub.s.timeutil.GetCurrentTickMilliSec(), vActor.ActorType, vActor.ObjectID, vActor.MasterID));
					}
				}
				else
				{
					_playerDeadInfos.Add(e.Victim.ObjectID, (Hub.s.timeutil.GetCurrentTickMilliSec(), ActorType.System, 0, 0));
				}
			}
			else
			{
				if (!(e.Victim is VMonster vMonster) || e.AttackerActorID == 0)
				{
					return;
				}
				VActor vActor2 = FindActorByObjectID(e.AttackerActorID);
				if (vActor2 != null && vActor2 is VPlayer vPlayer2)
				{
					if (!_playerKillInfos.TryGetValue(vPlayer2.ObjectID, out var value3))
					{
						Logger.RWarn($"OnActorEvent failed, player kill info not found. playerObjectID: {vPlayer2.ObjectID}, monsterMasterID: {vMonster.MasterID}");
					}
					else
					{
						value3[ActorType.Monster].Add((vMonster.MasterID, vMonster.ObjectID, Hub.s.timeutil.GetCurrentTickMilliSec()));
					}
				}
			}
		}
		finally
		{
			SendScoreBoard();
			_eventTimer.CreateTimerEvent(delegate
			{
				CheckComplete();
			}, 1000L);
		}
	}

	public override MsgErrorCode OnEnterChannel(VPlayer player, int hashCode)
	{
		base.OnEnterChannel(player, hashCode);
		EnterDeathMatchRoomRes enterDeathMatchRoomRes = new EnterDeathMatchRoomRes(hashCode);
		PlayerInfo info = enterDeathMatchRoomRes.playerInfo;
		player.GetMyActorInfo(ref info);
		player.SendToMe(enterDeathMatchRoomRes);
		return MsgErrorCode.Success;
	}

	protected override MsgErrorCode CanEnterChannel(long playerUID)
	{
		if (_state != DeathMatchRoomState.Ready && _state != DeathMatchRoomState.OnPlaying)
		{
			Logger.RError($"ProcessEnterWaitQueue failed, room state is not ready. state: {_state}");
			return MsgErrorCode.InvalidRoomState;
		}
		return base.CanEnterChannel(playerUID);
	}

	public void CheckComplete()
	{
		if (!_gameComplete.IsOn)
		{
			if (_vPlayerDict.Count<KeyValuePair<int, VPlayer>>((KeyValuePair<int, VPlayer> x) => x.Value.IsAliveStatus()) == 1 && _vActorDict.Values.Count((VActor x) => x is VMonster && x.IsAliveStatus()) == 0 && _state == DeathMatchRoomState.OnPlaying)
			{
				SetState(DeathMatchRoomState.LastManStanding);
			}
			else if (IsAllPlayerDead() && _state == DeathMatchRoomState.OnPlaying)
			{
				SetState(DeathMatchRoomState.NoWinner);
			}
		}
	}

	public override int GetContaValue(VActor actor, bool isRun)
	{
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"GetContaValue failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return 0;
		}
		return cycleMasterInfo.DeathMatchRoomCycleInfo.contaValue;
	}

	protected override long GetContaRecoveryRate()
	{
		return Hub.s.dataman.ExcelDataManager.Consts.C_ContaRecoveryValue;
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
			CheckComplete();
		}, 0L);
	}

	private void GenerateGameResult()
	{
		_gameResult.Clear();
		foreach (KeyValuePair<int, Dictionary<ActorType, List<(int, int, long)>>> playerKillInfo in _playerKillInfos)
		{
			DeathMatchPlayerResult deathMatchPlayerResult = new DeathMatchPlayerResult
			{
				actorID = playerKillInfo.Key,
				killCount = playerKillInfo.Value[ActorType.Monster].Count,
				lastKillTime = ((playerKillInfo.Value[ActorType.Monster].Count > 0) ? playerKillInfo.Value[ActorType.Monster].Max(((int, int, long) x) => x.Item3) : 0)
			};
			if (_playerDeadInfos.TryGetValue(playerKillInfo.Key, out (long, ActorType, int, int) value))
			{
				deathMatchPlayerResult.reasonofDead = (actorType: value.Item2, actorID: value.Item3, masterID: value.Item4);
			}
			if (_gameResult.ContainsKey(playerKillInfo.Key))
			{
				Logger.RError($"GetGameEndResult failed, duplicate player result found. actorID: {playerKillInfo.Key}");
			}
			else
			{
				_gameResult.Add(playerKillInfo.Key, deathMatchPlayerResult);
			}
		}
	}

	public void SendScoreBoard()
	{
		DeathMatchRoomScoreBoardSig deathMatchRoomScoreBoardSig = new DeathMatchRoomScoreBoardSig();
		foreach (KeyValuePair<int, Dictionary<ActorType, List<(int, int, long)>>> playerKillInfo in _playerKillInfos)
		{
			DeathMatchPlayerResult deathMatchPlayerResult = new DeathMatchPlayerResult
			{
				actorID = playerKillInfo.Key,
				killCount = playerKillInfo.Value[ActorType.Monster].Count,
				lastKillTime = ((playerKillInfo.Value[ActorType.Monster].Count > 0) ? playerKillInfo.Value[ActorType.Monster].Max(((int, int, long) x) => x.Item3) : 0)
			};
			if (_playerDeadInfos.TryGetValue(playerKillInfo.Key, out (long, ActorType, int, int) value))
			{
				deathMatchPlayerResult.reasonofDead = (actorType: value.Item2, actorID: value.Item3, masterID: value.Item4);
			}
			if (!deathMatchRoomScoreBoardSig.deathMatchScordBoards.ContainsKey(playerKillInfo.Key))
			{
				deathMatchRoomScoreBoardSig.deathMatchScordBoards.Add(playerKillInfo.Key, deathMatchPlayerResult);
			}
		}
		SendToAllPlayers(deathMatchRoomScoreBoardSig);
	}

	protected override RoomDrainInfo ExtractRoomInfo(bool includeDisappearItem = true)
	{
		RoomDrainInfo roomDrainInfo = new RoomDrainInfo(RoomID, 1);
		roomDrainInfo.SetBoostedItem(base.BoostedItemMasterID, base.BoostedItemRate);
		if (IsAllPlayerDead())
		{
			return roomDrainInfo;
		}
		VPlayer vPlayer = _vPlayerDict.Values.FirstOrDefault((VPlayer x) => x.IsAliveStatus());
		if (vPlayer == null)
		{
			KeyValuePair<int, (long, ActorType, int, int)> keyValuePair = _playerDeadInfos.OrderByDescending((KeyValuePair<int, (long deadTime, ActorType actorType, int actorID, int masterID)> x) => x.Value.deadTime).FirstOrDefault();
			if (keyValuePair.Key > 0)
			{
				vPlayer = FindPlayerByObjectID(keyValuePair.Key);
				if (vPlayer == null)
				{
					Logger.RError($"ExtractRoomInfo failed, last dead player not found. actorID: {keyValuePair.Key}");
				}
			}
			else
			{
				Logger.RError("ExtractRoomInfo failed, no alive player and no dead player info.");
			}
		}
		ItemDropInfo itemDropInfo = Hub.s.dataman.ExcelDataManager.GetItemDropInfo(Hub.s.dataman.ExcelDataManager.Consts.C_DeathmatchRoomMVPItemGroupId);
		if (itemDropInfo != null)
		{
			List<int> dropItemList = itemDropInfo.GetDropItemList();
			if (dropItemList.Count > 0)
			{
				foreach (int item in dropItemList)
				{
					roomDrainInfo.AddReservedItem(vPlayer.UID, item);
				}
			}
		}
		return roomDrainInfo;
	}

	private void SetState(DeathMatchRoomState state)
	{
		if (_state == state)
		{
			return;
		}
		_state = state;
		switch (_state)
		{
		case DeathMatchRoomState.LastManStanding:
		case DeathMatchRoomState.NoWinner:
			EnableAIControl(enable: false);
			GenerateGameResult();
			SendToAllPlayers(new EndDeathMatchSig
			{
				deathMatchPlayerResults = _gameResult
			});
			Hub.s.replayManager.OnStopRecording();
			_eventTimer.CreateTimerEvent(delegate
			{
				OnDecideRoom(delegate
				{
					SetState(DeathMatchRoomState.End);
				});
			}, Hub.s.dataman.ExcelDataManager.Consts.C_UI_PopupDurationDeathmatchReport);
			_gameComplete.On();
			Stop();
			break;
		case DeathMatchRoomState.End:
			foreach (VActor value in _vActorDict.Values)
			{
				if (value is VLootingObject)
				{
					PendRemoveActor(value.ObjectID);
				}
			}
			Hub.s.vworld.TerminateSession(_vPlayerDict.ToDictionary<KeyValuePair<int, VPlayer>, ulong, long>((KeyValuePair<int, VPlayer> x) => x.Value.SteamID, (KeyValuePair<int, VPlayer> x) => x.Value.UID), ExtractRoomInfo());
			break;
		}
	}

	protected override void OnAllMemberEntered()
	{
		foreach (VPlayer value2 in _vPlayerDict.Values)
		{
			if (!_playerKillInfos.TryGetValue(value2.ObjectID, out var value))
			{
				value = new Dictionary<ActorType, List<(int, int, long)>>
				{
					{
						ActorType.Monster,
						new List<(int, int, long)>()
					},
					{
						ActorType.Player,
						new List<(int, int, long)>()
					}
				};
				_playerKillInfos.Add(value2.ObjectID, value);
			}
		}
		base.OnAllMemberEntered();
		SetState(DeathMatchRoomState.OnPlaying);
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"OnAllMemberEntered failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		ImmutableArray<IGameAction> enterActions = cycleMasterInfo.DeathMatchRoomCycleInfo.EnterActions;
		if (enterActions.Length > 0)
		{
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator2 = enterActions.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				IGameAction current2 = enumerator2.Current;
				dictionary.Add(current2.Clone(), null);
			}
			EnqueueEventAction(dictionary, 0);
		}
	}

	public void Wipeout()
	{
		_commandExecutor.Invoke(delegate
		{
			foreach (VActor item in _vActorDict.Values.Where((VActor x) => x is VLootingObject).ToList())
			{
				PendRemoveActor(item.ObjectID);
			}
			foreach (VActor value in _vActorDict.Values)
			{
				if (value is VMonster)
				{
					PendRemoveActor(value.ObjectID);
				}
				if (value is FieldSkillObject)
				{
					PendRemoveActor(value.ObjectID);
				}
			}
			Vacate(backup: false);
			_state = DeathMatchRoomState.Ready;
			Hub.s.vworld.RemoveDeathMatchRoom();
		});
	}

	protected override void OnDecideRoom(Command command)
	{
		base.OnDecideRoom(command);
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"OnDecideRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		ImmutableArray<IGameAction> endActions = cycleMasterInfo.DeathMatchRoomCycleInfo.EndActions;
		if (endActions.Length > 0)
		{
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator = endActions.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IGameAction current = enumerator.Current;
				dictionary.Add(current.Clone(), null);
			}
			EnqueueEventAction(dictionary, 0);
		}
		BlockEventAction();
	}

	protected override void RunEnterRoomCutScene()
	{
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"SendAllMembersEnterRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		List<string> enterCutSceneNames = cycleMasterInfo.DeathMatchRoomCycleInfo.GetEnterCutSceneNames();
		Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
		foreach (string item in enterCutSceneNames)
		{
			dictionary.Add(new GameActionPlayCutscene(item, needToBroadCast: false), null);
		}
		dictionary.Add(new GameActionEnableAI(), null);
		EnqueueEventAction(dictionary, 0);
	}

	protected override void SyncEnterRoom()
	{
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"SendAllMembersEnterRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		List<string> enterCutSceneNames = cycleMasterInfo.DeathMatchRoomCycleInfo.GetEnterCutSceneNames();
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
			Logger.RError($"SendAllMembersEnterRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		List<string> exitCutSceneNames = cycleMasterInfo.DeathMatchRoomCycleInfo.GetExitCutSceneNames();
		SendToAllPlayers(new BeginEndRoomCutSceneSig
		{
			exitCutsceneNames = exitCutSceneNames
		});
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

	public override VRoomType GetToMoveRoomType()
	{
		return VRoomType.Maintenance;
	}
}
