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

public class MaintenanceRoom : IVroom
{
	private MaintenenceRoomState _state;

	private bool _restored = true;

	private CycleMasterInfo _masterInfo;

	private Dictionary<int, int> _priceForItems = new Dictionary<int, int>();

	private List<DrainLootingObjectInfo> _archivedDrainLootingObjectInfos = new List<DrainLootingObjectInfo>();

	private bool _everDeparted;

	private VRoomType _toMoveRoomType = VRoomType.Waiting;

	private bool _roomRecycled;

	private (int leftPanel, int rightPanel) _candidateTramUpgrades = (leftPanel: 0, rightPanel: 0);

	private int _selectedUpgradeMasterID;

	public MaintenanceRoom(VRoomManager roomManager, long roomID, IVRoomProperty property)
		: base(roomManager, roomID, property)
	{
		base.Currency = Hub.s.dataman.ExcelDataManager.Consts.C_InitialMoney;
		_masterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (_masterInfo == null)
		{
			throw new Exception("MaintenenceRoomMasterInfo is null");
		}
		InitShopItems();
		AddGameEventLog(new GELInitMainteneceRoom
		{
			RoomSessionID = property.SessionID,
			ShopItems = _priceForItems.Select((KeyValuePair<int, int> x) => new GELShopData
			{
				ItemMasterID = x.Key,
				Price = x.Value
			}).ToList(),
			Timestamp = Hub.s.timeutil.GetTimestamp()
		});
		_startNotified = true;
		_roomRecycled = false;
	}

	public void InitShopItems()
	{
		_priceForItems.Clear();
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"[InitShopItems] cycleMasterInfo is null. use default. _currentSessionCount={_currentSessionCount}");
			cycleMasterInfo = _masterInfo;
		}
		_masterInfo = cycleMasterInfo;
		List<(int masterid, int price)>? shopGroupPriceList = Hub.s.dataman.ExcelDataManager.GetShopGroupPriceList(_masterInfo.MaintenanceRoomCycleInfo.ShopGroupID);
		if (shopGroupPriceList == null)
		{
			Logger.RError("Invalid ShopGroupID");
		}
		foreach (var item in shopGroupPriceList)
		{
			_priceForItems.Add(item.masterid, item.price);
		}
	}

	public override bool Initialize()
	{
		_currentDay = 1;
		if (!base.Initialize())
		{
			return false;
		}
		return true;
	}

	public override MsgErrorCode OnEnterChannel(VPlayer player, int hashCode)
	{
		base.OnEnterChannel(player, hashCode);
		EnterMaintenanceRoomRes enterMaintenanceRoomRes = new EnterMaintenanceRoomRes(hashCode)
		{
			cycleCount = _currentSessionCount,
			dayCount = _currentDay,
			repaired = _restored,
			memberList = _vPlayerDict.Values.Select((VPlayer x) => x.GetPlayerInfo()).ToList(),
			currency = base.Currency,
			itemPrices = _priceForItems,
			inPlaying = (_state == MaintenenceRoomState.WaitReturn),
			roomSessionID = Property.SessionID.ToString(),
			tramUpgradeCandidate = _candidateTramUpgrades
		};
		PlayerInfo info = enterMaintenanceRoomRes.playerInfo;
		player.GetMyActorInfo(ref info);
		player.SendToMe(enterMaintenanceRoomRes);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode OnRequestStartSession()
	{
		if (_state != MaintenenceRoomState.Ready)
		{
			Logger.RError($"OnRequestStartGame failed, state is not Open. state: {_state}");
			return MsgErrorCode.InvalidMaintenenceRoomState;
		}
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (currentTickMilliSec - _resumeTime < 1000)
		{
			Logger.RError($"OnRequestStartGame failed, not enough time passed. currentTime: {currentTickMilliSec}, _resumeTime: {_resumeTime}");
			return MsgErrorCode.NotEnoughTimePassed;
		}
		base.Currency = 0;
		OnDecideRoom(delegate
		{
			SendToAllPlayers(new StartSessionSig());
			Hub.s.vworld.PendStartSession(_vPlayerDict.ToDictionary<KeyValuePair<int, VPlayer>, ulong, long>((KeyValuePair<int, VPlayer> x) => x.Value.SteamID, (KeyValuePair<int, VPlayer> x) => x.Value.UID), ExtractRoomInfo());
		});
		_restored = false;
		return MsgErrorCode.Success;
	}

	public void Wipeout()
	{
		_commandExecutor.Invoke(delegate
		{
			foreach (VActor item in _vActorDict.Values.Where((VActor x) => x is VLootingObject).ToList())
			{
				PendRemoveActor(item.ObjectID);
			}
			_playerReservedItems.Clear();
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
			_restored = false;
			Vacate(backup: true);
			ResumeRoom();
		});
	}

	public override void OnVacateRoom(bool backup)
	{
		base.OnVacateRoom(backup);
		SetState(backup ? MaintenenceRoomState.WaitReturn : MaintenenceRoomState.Ready);
	}

	private void ApplyGameSessionInfo(GameSessionInfo info, bool needToSpawn)
	{
		if (Property.TargetCurrency != info.GetCurrencyThreshold())
		{
			Property.OverwriteTargetCurrency(info.GetCurrencyThreshold());
		}
		if (needToSpawn)
		{
			SpawnLootingObjectsFromGameSessionInfo(info);
		}
		else
		{
			_playerReservedItems.Clear();
			foreach (KeyValuePair<long, List<int>> item in info.ItemsToProvide)
			{
				if (!_playerReservedItems.TryGetValue(item.Key, out List<int> value))
				{
					value = new List<int>();
					_playerReservedItems.Add(item.Key, value);
				}
				value.AddRange(item.Value);
			}
		}
		_playerContas.Clear();
		foreach (KeyValuePair<long, long> playerContum in info.PlayerConta)
		{
			_playerContas.Add(playerContum.Key, playerContum.Value);
		}
		base.BoostedItemMasterID = info.BoostedItemMasterID;
		base.BoostedItemRate = info.BoostedRate;
	}

	public void OnCompleteGame(GameSessionInfo info)
	{
		_commandExecutor.Invoke(delegate
		{
			ResumeRoom();
			if (_state != MaintenenceRoomState.WaitReturn)
			{
				Logger.RError($"OnCompleteGame failed, state is not WaitReturn. state: {_state}");
			}
			else
			{
				_everDeparted = true;
				_resumeTime = Hub.s.timeutil.GetCurrentTickMilliSec();
				ApplyGameSessionInfo(info, needToSpawn: true);
				RollTramUpgradeCandidate();
				_currentDay = info.CurrentGameCount;
				_currentSessionCount = info.IterationCount;
				SetState(MaintenenceRoomState.DecisionNextGame);
				_toMoveRoomType = VRoomType.DeathMatch;
				_startNotified = false;
				_waitThresholdTime = Hub.s.timeutil.GetCurrentTickMilliSec();
				SendToAll(new ReturnMembertoMaintenenceRoomSig
				{
					itemPrices = _priceForItems,
					targetCurrency = Property.TargetCurrency
				});
				List<VActor> item = ClassifyLootingObjectsInCollectingVolume().removeCandidateObjects;
				List<int> list = new List<int>();
				foreach (VActor item2 in item)
				{
					if (item2 is VLootingObject vLootingObject)
					{
						ItemElement itemElement = vLootingObject.GetItemElement();
						if (itemElement != null)
						{
							list.Add(itemElement.ItemMasterID);
						}
					}
				}
				AddGameEventLog(new GELDecideSession
				{
					RoomSessionID = Property.SessionID,
					SessionCycleCount = _currentSessionCount,
					ReplacedShopItems = _priceForItems.Select((KeyValuePair<int, int> x) => new GELShopData
					{
						ItemMasterID = x.Key,
						Price = x.Value
					}).ToList(),
					ScrapItems = list,
					Timestamp = Hub.s.timeutil.GetTimestamp()
				});
			}
		});
	}

	public void OnCompleteSession(GameSessionInfo info, bool promoted)
	{
		_commandExecutor.Invoke(delegate
		{
			ResumeRoom();
			_resumeTime = Hub.s.timeutil.GetCurrentTickMilliSec();
			ApplyGameSessionInfo(info, needToSpawn: false);
			_currentDay = info.CurrentGameCount;
			_currentSessionCount = info.IterationCount;
			_restored = !promoted;
			base.Currency = ((!promoted) ? Hub.s.dataman.ExcelDataManager.Consts.C_InitialMoney : 0);
			SetState(promoted ? MaintenenceRoomState.WaitReturn : MaintenenceRoomState.Ready);
			if (!promoted)
			{
				_roomRecycled = true;
			}
		});
	}

	public void SessionDecision(int triggeredActorID, bool force = false)
	{
		SendToAll(new TramStatusSig
		{
			triggeredActorID = triggeredActorID,
			restored = _restored,
			status = TramStatus.Engaged
		});
		if (_state != MaintenenceRoomState.DecisionNextGame && !force)
		{
			Logger.RError($"SessionDecision failed, state is not DecisionNextGame. state: {_state}");
			return;
		}
		SetState(MaintenenceRoomState.Complete);
		_playerStartingVolumes.FirstOrDefault();
		base.Currency = 0;
		OnDecideRoom(delegate
		{
			if (_restored)
			{
				ResetBoostedItem();
				_currentDay = 1;
				Hub.s.vworld.Promote(_vPlayerDict.ToDictionary<KeyValuePair<int, VPlayer>, ulong, long>((KeyValuePair<int, VPlayer> x) => x.Value.SteamID, (KeyValuePair<int, VPlayer> x) => x.Value.UID), ExtractRoomInfo());
				_currentSessionCount++;
				List<VActor> item = ClassifyLootingObjectsInCollectingVolume().removeCandidateObjects;
				List<int> list = new List<int>();
				foreach (VActor item2 in item)
				{
					if (item2 is VLootingObject vLootingObject)
					{
						ItemElement itemElement = vLootingObject.GetItemElement();
						if (itemElement != null)
						{
							list.Add(itemElement.ItemMasterID);
						}
					}
				}
				AddGameEventLog(new GELSuccessSession
				{
					RoomSessionID = Property.SessionID,
					remainScraps = list,
					SessionCycleCount = _currentSessionCount,
					RemainCurrency = base.Currency,
					Timestamp = Hub.s.timeutil.GetTimestamp()
				});
			}
			else
			{
				SendToAllPlayers(new GameOverSig());
				Hub.s.vworld.PendStartDeathMatch(_vPlayerDict.ToDictionary<KeyValuePair<int, VPlayer>, ulong, long>((KeyValuePair<int, VPlayer> x) => x.Value.SteamID, (KeyValuePair<int, VPlayer> x) => x.Value.UID), ExtractRoomInfo());
			}
		});
	}

	public void SetState(MaintenenceRoomState state)
	{
		_state = state;
	}

	protected override long GetContaRecoveryRate()
	{
		return 0L;
	}

	public (MsgErrorCode, int) RepairTrain()
	{
		if (_restored)
		{
			return (MsgErrorCode.AlreadyRestored, base.Currency);
		}
		if (base.Currency >= Property.TargetCurrency)
		{
			base.Currency -= Property.TargetCurrency;
			_restored = true;
			CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
			if (cycleMasterInfo == null)
			{
				Logger.RError($"[RepairTrain] cycleMasterInfo is null. use default. _currentSessionCount={_currentSessionCount}");
				cycleMasterInfo = _masterInfo;
			}
			_masterInfo = cycleMasterInfo;
			ImmutableArray<IGameAction> repairActions = _masterInfo.MaintenanceRoomCycleInfo.RepairActions;
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator = repairActions.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IGameAction current = enumerator.Current;
				dictionary.Add(current.Clone(), null);
			}
			EnqueueEventAction(dictionary, 0);
			_onEmptyEventActionQueue.Enqueue(delegate
			{
				SendToAllPlayers(new EndRepairTramSig());
				if (MMSaveGameData.CheckSaveSlotID(Hub.s.vworld.SaveSlotID, includeAutoSlot: false) && SaveGameData(Hub.s.vworld.SaveSlotID, null, isAutoSave: true) == MsgErrorCode.Success)
				{
					IterateAllPlayer(delegate(VPlayer player)
					{
						if (player.IsHost)
						{
							player.SendToMe(new SaveGameDataSig
							{
								auto = true
							});
						}
					});
				}
			});
			_toMoveRoomType = VRoomType.Waiting;
			return (MsgErrorCode.Success, base.Currency);
		}
		return (MsgErrorCode.NotEnoughCurrency, base.Currency);
	}

	public MsgErrorCode ReinforceItem(int targetItemMasterID, int price, PosWithRot targetPos)
	{
		ItemElement newItemElement = GetNewItemElement(targetItemMasterID, isFake: false);
		SpawnLootingObject(newItemElement, targetPos, isIndoor: false, ReasonOfSpawn.Reinforce);
		AddCurrency(-price);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode BuyItem(int itemMasterID, VCreature creature)
	{
		Vector3 reachableDistancePos = GetReachableDistancePos(creature.PositionVector, creature.Position.yaw, 0.6f);
		if (reachableDistancePos == Vector3.zero)
		{
			return MsgErrorCode.InvalidPosition;
		}
		if (!GetRandomReachablePointInRadius(reachableDistancePos, 0f, 0.3f, out var resultPos))
		{
			return MsgErrorCode.InvalidPosition;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		int value = 0;
		int remainAmount = 0;
		int remainDurability = 0;
		if (!_priceForItems.TryGetValue(itemMasterID, out value))
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (itemInfo is ItemEquipmentInfo itemEquipmentInfo)
		{
			remainAmount = itemEquipmentInfo.InitialGauge;
			remainDurability = itemEquipmentInfo.MaxDurability;
		}
		if (base.Currency < value)
		{
			return MsgErrorCode.NotEnoughCurrency;
		}
		AddCurrency(-value);
		if (SpawnLootingObject((itemInfo.ItemType == ItemType.Consumable) ? ((ItemElement)new ConsumableItemElement(itemMasterID, GetNewItemID(), isFake: false)) : ((ItemElement)new EquipmentItemElement(itemMasterID, GetNewItemID(), isFake: false, remainDurability, remainAmount)), resultPos.toPosWithRot(0f), isIndoor: false, ReasonOfSpawn.Buying) == 0)
		{
			return MsgErrorCode.BuyingItemSpawnFailed;
		}
		AddGameEventLog(new GELPurchaseItem
		{
			RoomSessionID = base.SessionID,
			SessionCycleCount = base.SessionCycleCount,
			ItemMasterID = itemMasterID,
			Price = value,
			Timestamp = Hub.s.timeutil.GetTimestamp()
		});
		return MsgErrorCode.Success;
	}

	public void AddCurrency(int amount)
	{
		base.Currency = Math.Max(0, base.Currency + amount);
		SendToAll(new ChangeCurrencySig
		{
			currency = base.Currency
		});
	}

	public (MsgErrorCode, int) PutIntoToilet(ItemElement element)
	{
		if (Hub.s.dataman.ExcelDataManager.GetItemInfo(element.ItemMasterID) == null)
		{
			return (MsgErrorCode.ItemNotFound, base.Currency);
		}
		int num = element.FinalPrice;
		if (base.BoostedItemMasterID == element.ItemMasterID)
		{
			num = (int)((float)num * base.BoostedItemRate);
		}
		base.Currency += num;
		return (MsgErrorCode.Success, base.Currency);
	}

	public void ArchiveCollectingVolume()
	{
		_archivedDrainLootingObjectInfos.Clear();
		LootingObjectSpawnVolumeData lootingObjectSpawnVolumeData = _lootingObjectSpawnVolumes?.FirstOrDefault();
		if (lootingObjectSpawnVolumeData == null)
		{
			Logger.RError("[ArchiveCollectingVolume] collectingVolumeData is missing");
			return;
		}
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is VLootingObject vLootingObject && lootingObjectSpawnVolumeData.Bounds.Contains(value.PositionVector))
			{
				Quaternion rotation = Quaternion.Euler(0f, lootingObjectSpawnVolumeData.Rotation.eulerAngles.y, 0f);
				Vector3 vector = Quaternion.Inverse(rotation) * (value.Position.toVector3() - lootingObjectSpawnVolumeData.Position);
				PosWithRot posWithRot = new PosWithRot();
				posWithRot.x = vector.x;
				posWithRot.y = vector.y;
				posWithRot.z = vector.z;
				posWithRot.yaw = value.Position.yaw + Quaternion.Inverse(rotation).eulerAngles.y;
				posWithRot.pitch = value.Position.pitch;
				PosWithRot posWithRot2 = posWithRot;
				_archivedDrainLootingObjectInfos.Add(new DrainLootingObjectInfo
				{
					ItemElement = vLootingObject.GetItemElement(),
					PosWithRot = posWithRot2,
					ReasonOfSpawn = vLootingObject.ReasonOfSpawn
				});
				PendRemoveActor(value.ObjectID);
			}
		}
	}

	public void RespawnCollectingVolume()
	{
		LootingObjectSpawnVolumeData lootingObjectSpawnVolumeData = _lootingObjectSpawnVolumes?.FirstOrDefault();
		if (lootingObjectSpawnVolumeData == null)
		{
			Logger.RError("[RespawnCollectingVolume] collectingVolumeData is missing");
			return;
		}
		foreach (VActor value in _vActorDict.Values)
		{
			if (value is VLootingObject vLootingObject && lootingObjectSpawnVolumeData.Bounds.Contains(vLootingObject.PositionVector))
			{
				Vector3 position = lootingObjectSpawnVolumeData.Position;
				float y = lootingObjectSpawnVolumeData.FloorY;
				Vector3 nearestPointOnNavMesh = Hub.s.navman.GetNearestPointOnNavMesh(position, 1f);
				if (nearestPointOnNavMesh != Vector3.zero)
				{
					y = nearestPointOnNavMesh.y;
				}
				position.x += SimpleRandUtil.Next(-0.5f, 0.5f);
				position.y = y;
				position.z += SimpleRandUtil.Next(-0.5f, 0.5f);
				vLootingObject.SetPosition(new PosWithRot
				{
					pos = position,
					rot = new Vector3(vLootingObject.Position.pitch, vLootingObject.Position.yaw, vLootingObject.Position.roll)
				}, ActorMoveCause.Teleport);
				SendToAll(new TeleportSig
				{
					actorID = vLootingObject.ObjectID,
					pos = vLootingObject.Position,
					reason = TeleportReason.ForceMoveSync
				});
			}
		}
		foreach (DrainLootingObjectInfo archivedDrainLootingObjectInfo in _archivedDrainLootingObjectInfos)
		{
			PosWithRot posWithRot = archivedDrainLootingObjectInfo.PosWithRot.Clone();
			if (lootingObjectSpawnVolumeData != null)
			{
				Quaternion quaternion = Quaternion.Euler(0f, lootingObjectSpawnVolumeData.Rotation.eulerAngles.y, 0f);
				Vector3 position2 = lootingObjectSpawnVolumeData.Position + quaternion * posWithRot.toVector3();
				float y2 = position2.y;
				Vector3 nearestPointOnNavMesh2 = Hub.s.navman.GetNearestPointOnNavMesh(position2, 1f);
				if (nearestPointOnNavMesh2 != Vector3.zero)
				{
					y2 = nearestPointOnNavMesh2.y;
				}
				posWithRot.x = position2.x;
				posWithRot.y = y2;
				posWithRot.z = position2.z;
				posWithRot.yaw += quaternion.eulerAngles.y;
			}
			SpawnLootingObject(archivedDrainLootingObjectInfo.ItemElement, posWithRot, isIndoor: false, archivedDrainLootingObjectInfo.ReasonOfSpawn);
		}
		_archivedDrainLootingObjectInfos.Clear();
	}

	protected override void OnEnterRoomFailed(SessionContext context, MsgErrorCode errorCode, int hashCode)
	{
		context.Send(new EnterMaintenanceRoomRes(hashCode)
		{
			errorCode = errorCode
		});
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
		ImmutableArray<IGameAction> enterActions = cycleMasterInfo.MaintenanceRoomCycleInfo.EnterActions;
		if (enterActions.Length > 0)
		{
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator = enterActions.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IGameAction current = enumerator.Current;
				dictionary.Add(current.Clone(), null);
			}
			EnqueueEventAction(dictionary, 0);
		}
	}

	public override int GetContaValue(VActor actor, bool isRun)
	{
		return 0;
	}

	protected override void OnDecideRoom(Command command)
	{
		base.OnDecideRoom(command);
		Stop(tickStop: false);
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"OnDecideRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		ImmutableArray<IGameAction> exitActions = cycleMasterInfo.MaintenanceRoomCycleInfo.ExitActions;
		if (exitActions.Length > 0)
		{
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator = exitActions.GetEnumerator();
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
		List<string> enterCutSceneNames = cycleMasterInfo.MaintenanceRoomCycleInfo.GetEnterCutSceneNames();
		Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
		foreach (string item in enterCutSceneNames)
		{
			dictionary.Add(new GameActionPlayCutscene(item, needToBroadCast: false), null);
		}
		dictionary.Add(new GameActionEnableAI(), null);
		EnqueueEventAction(dictionary, 0);
		if ((IsFirstMaintenanceRoomEntryInFirstCycle() && (!IsFirstMaintenanceRoomEntryInFirstCycle() || !_roomRecycled)) || !MMSaveGameData.CheckSaveSlotID(Hub.s.vworld.SaveSlotID, includeAutoSlot: false))
		{
			return;
		}
		_onEmptyEventActionQueue.Enqueue(delegate
		{
			if (SaveGameData(Hub.s.vworld.SaveSlotID, null, isAutoSave: true) == MsgErrorCode.Success)
			{
				IterateAllPlayer(delegate(VPlayer player)
				{
					if (player.IsHost)
					{
						player.SendToMe(new SaveGameDataSig
						{
							auto = true
						});
					}
				});
			}
		});
	}

	protected override void SyncEnterRoom()
	{
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"SendAllMembersEnterRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		List<string> enterCutSceneNames = cycleMasterInfo.MaintenanceRoomCycleInfo.GetEnterCutSceneNames();
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
		List<string> exitCutSceneNames = cycleMasterInfo.MaintenanceRoomCycleInfo.GetExitCutSceneNames();
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

	public List<int> GetSupplyItemMasterIDs(long playerUID)
	{
		if (_state != MaintenenceRoomState.Ready)
		{
			return new List<int>();
		}
		if (!_playerReservedItems.TryGetValue(playerUID, out List<int> value))
		{
			return new List<int>();
		}
		return value;
	}

	public (MsgErrorCode, int) PlayTramRepairCutSceneForDebug()
	{
		_restored = false;
		base.Currency += Property.TargetCurrency;
		return RepairTrain();
	}

	public override VRoomType GetToMoveRoomType()
	{
		return _toMoveRoomType;
	}

	public override void ResetEnvironment(bool failed = false)
	{
		base.ResetEnvironment(failed);
		_toMoveRoomType = VRoomType.Waiting;
	}

	public void ApplyLoadedGameData(MMSaveGameData saveGameData, GameSessionInfo gameSessionInfo)
	{
		base.Currency = saveGameData.Currency;
		_currentDay = saveGameData.DayCount;
		_currentSessionCount = saveGameData.CycleCount;
		_restored = saveGameData.TramRepaired;
		base.BoostedItemMasterID = saveGameData.BoostedItemMasterID;
		base.BoostedItemRate = saveGameData.BoostedItemRate;
		if (_priceForItems != null && _priceForItems.Count > 0 && saveGameData.PriceForItems != null && saveGameData.PriceForItems.Count > 0)
		{
			foreach (int key in saveGameData.PriceForItems.Keys)
			{
				if (_priceForItems.ContainsKey(key) && saveGameData.PriceForItems[key] != _priceForItems[key])
				{
					_priceForItems[key] = saveGameData.PriceForItems[key];
				}
			}
		}
		SpawnItemsFromSaveGameData(saveGameData);
		_stashes.Clear();
		if (saveGameData.Stashes != null)
		{
			foreach (KeyValuePair<int, MMSaveGameDataItemElement> stash in saveGameData.Stashes)
			{
				_stashes[stash.Key] = stash.Value.ToItemElement(this);
			}
		}
		_tramUpgradeList.Clear();
		if (saveGameData.TramUpgradeList != null)
		{
			_tramUpgradeList.AddRange(saveGameData.TramUpgradeList);
		}
		_closedTramUpgradeObject.Clear();
		foreach (KeyValuePair<int, LevelObject> allTramUpgradeLevelObject in Hub.s.dynamicDataMan.GetAllTramUpgradeLevelObjects())
		{
			if (allTramUpgradeLevelObject.Value is ITramUpgradeLevelObject tramUpgradeLevelObject && !_tramUpgradeList.Contains(tramUpgradeLevelObject.TramUpgradeID))
			{
				_closedTramUpgradeObject.Add(allTramUpgradeLevelObject.Key);
			}
		}
		_candidateTramUpgrades = (leftPanel: 0, rightPanel: 0);
		_selectedUpgradeMasterID = 0;
		if (saveGameData.TramUpgradeCandidateList != null)
		{
			bool flag = false;
			for (int i = 0; i < saveGameData.TramUpgradeCandidateList.Count; i++)
			{
				if (_tramUpgradeList.Contains(saveGameData.TramUpgradeCandidateList[i]))
				{
					flag = true;
					_selectedUpgradeMasterID = saveGameData.TramUpgradeCandidateList[i];
				}
				switch (i)
				{
				case 0:
					_candidateTramUpgrades.leftPanel = saveGameData.TramUpgradeCandidateList[i];
					break;
				case 1:
					_candidateTramUpgrades.rightPanel = saveGameData.TramUpgradeCandidateList[i];
					break;
				default:
					Logger.RWarn($"[ApplyLoadedGameData] TramUpgradeCandidateList has more than 2 items. index={i}");
					break;
				}
			}
			if (!_restored && flag)
			{
				RollTramUpgradeCandidate();
			}
			else if (_selectedUpgradeMasterID == 0)
			{
				_selectedUpgradeMasterID = _candidateTramUpgrades.leftPanel;
			}
		}
		else if (saveGameData.DayCount >= 4 && !saveGameData.TramRepaired)
		{
			RollTramUpgradeCandidate();
		}
		if (!IsFirstMaintenanceRoomEntryInFirstCycle())
		{
			_state = MaintenenceRoomState.DecisionNextGame;
		}
		if (Property.TargetCurrency != gameSessionInfo.GetCurrencyThreshold())
		{
			Property.OverwriteTargetCurrency(gameSessionInfo.GetCurrencyThreshold());
		}
	}

	protected void SpawnItemsFromSaveGameData(MMSaveGameData saveGameData)
	{
		foreach (MMSaveGameDataLootingObjectInfo lootingObjectInfo in saveGameData.LootingObjectInfos)
		{
			ItemElement itemElement = lootingObjectInfo.ItemElement.ToItemElement(this);
			if (itemElement != null)
			{
				SpawnLootingObject(itemElement, lootingObjectInfo.PosWithRot.ToPosWitRot(), isIndoor: false, lootingObjectInfo.ReasonOfSpawn);
			}
			else
			{
				Logger.RWarn($"[SpawnItemsFromSaveGameData] itemElement is null. itemElement.ItemMasterID={itemElement.ItemMasterID}");
			}
		}
		foreach (MMSaveGameDataItemElement playerItemElement in saveGameData.PlayerItemElements)
		{
			ItemElement itemElement2 = playerItemElement.ToItemElement(this);
			if (itemElement2 != null)
			{
				PosWithRot pos = CreateRandomPosWithRotForNewMaintenance();
				SpawnLootingObject(itemElement2, pos, isIndoor: false, ReasonOfSpawn.ActorDying);
			}
			else
			{
				Logger.RWarn($"[SpawnItemsFromSaveGameData] playerItemElement is null. playerItemElement.ItemMasterID={playerItemElement.ItemMasterID}");
			}
		}
	}

	protected PosWithRot CreateRandomPosWithRotForNewMaintenance()
	{
		PosWithRot posWithRot = new PosWithRot();
		Quaternion quaternion = Quaternion.Euler(0f, SimpleRandUtil.Next(0, 360), 0f);
		Vector3 position = new Vector3(-4f + (float)SimpleRandUtil.Next(-100, 100) / 100f, 0.5f, 16f + (float)SimpleRandUtil.Next(-100, 100) / 100f);
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
		return posWithRot;
	}

	private bool IsFirstMaintenanceRoomEntryInFirstCycle()
	{
		if (_currentSessionCount == 1)
		{
			return _currentDay == 1;
		}
		return false;
	}

	public MsgErrorCode SaveGameData(int saveSlotID, List<string> playerNames = null, bool isAutoSave = false)
	{
		try
		{
			MMSaveGameData mMSaveGameData = new MMSaveGameData();
			mMSaveGameData.SlotID = saveSlotID;
			mMSaveGameData.RegDateTime = DateTime.Now.ToUniversalTime();
			mMSaveGameData.CycleCount = _currentSessionCount;
			mMSaveGameData.DayCount = _currentDay;
			mMSaveGameData.TramRepaired = _restored;
			mMSaveGameData.Currency = base.Currency;
			if (playerNames == null || playerNames.Count == 0)
			{
				mMSaveGameData.PlayerNames = new List<string>();
				foreach (VPlayer value in _vPlayerDict.Values)
				{
					if (value != null)
					{
						if (value.IsHost)
						{
							mMSaveGameData.PlayerNames.Insert(0, value.ActorName);
						}
						else
						{
							mMSaveGameData.PlayerNames.Add(value.ActorName);
						}
					}
				}
			}
			else
			{
				mMSaveGameData.PlayerNames = playerNames.Clone();
			}
			mMSaveGameData.LootingObjectInfos = new List<MMSaveGameDataLootingObjectInfo>();
			foreach (DrainLootingObjectInfo archivedDrainLootingObjectInfo in _archivedDrainLootingObjectInfos)
			{
				if (!archivedDrainLootingObjectInfo.ItemElement.IsPromotionItem())
				{
					mMSaveGameData.LootingObjectInfos.Add(new MMSaveGameDataLootingObjectInfo
					{
						ItemElement = archivedDrainLootingObjectInfo.ItemElement.ToMMSaveGameDataItemElement(),
						PosWithRot = archivedDrainLootingObjectInfo.PosWithRot.ToMMSaveGameDataPosWithRot(),
						ReasonOfSpawn = archivedDrainLootingObjectInfo.ReasonOfSpawn
					});
				}
			}
			foreach (VActor value2 in _vActorDict.Values)
			{
				VLootingObject lootingObj = value2 as VLootingObject;
				if (lootingObj != null && (_archivedDrainLootingObjectInfos.Count <= 0 || !_archivedDrainLootingObjectInfos.Any((DrainLootingObjectInfo x) => x.ItemElement.ItemID == lootingObj.GetItemElement().ItemID)) && !lootingObj.GetItemElement().IsPromotionItem())
				{
					mMSaveGameData.LootingObjectInfos.Add(new MMSaveGameDataLootingObjectInfo
					{
						ItemElement = lootingObj.GetItemElement().ToMMSaveGameDataItemElement(),
						PosWithRot = lootingObj.Position.ToMMSaveGameDataPosWithRot(),
						ReasonOfSpawn = lootingObj.ReasonOfSpawn
					});
				}
			}
			mMSaveGameData.PlayerItemElements = new List<MMSaveGameDataItemElement>();
			foreach (VPlayer value3 in _vPlayerDict.Values)
			{
				if (value3 == null)
				{
					continue;
				}
				foreach (ItemElement value4 in value3.InventoryControlUnit.GetAllItemElements().Values)
				{
					if (value4 != null && value4.ItemMasterID != 0 && !value4.ReserveDestroy && !value4.IsPromotionItem())
					{
						mMSaveGameData.PlayerItemElements.Add(value4.ToMMSaveGameDataItemElement());
					}
				}
			}
			mMSaveGameData.BoostedItemMasterID = base.BoostedItemMasterID;
			mMSaveGameData.BoostedItemRate = base.BoostedItemRate;
			mMSaveGameData.PriceForItems = _priceForItems?.Clone();
			mMSaveGameData.Stashes = new Dictionary<int, MMSaveGameDataItemElement>();
			foreach (KeyValuePair<int, ItemElement> stash in _stashes)
			{
				mMSaveGameData.Stashes[stash.Key] = stash.Value.ToMMSaveGameDataItemElement();
			}
			mMSaveGameData.TramUpgradeList = _tramUpgradeList.Clone();
			mMSaveGameData.TramUpgradeCandidateList = new List<int>();
			mMSaveGameData.TramUpgradeCandidateList.Add(_candidateTramUpgrades.leftPanel);
			mMSaveGameData.TramUpgradeCandidateList.Add(_candidateTramUpgrades.rightPanel);
			int slotID = ((!isAutoSave) ? saveSlotID : 0);
			MonoSingleton<PlatformMgr>.Instance.Save(MMSaveGameData.GetSaveFileName(slotID), mMSaveGameData);
		}
		catch (Exception e)
		{
			Logger.RError(e);
			return MsgErrorCode.InvalidErrorCode;
		}
		return MsgErrorCode.Success;
	}

	public (int cycleCount, int dayCount, bool repaired) GetMaintenenceRoomCycleInfos()
	{
		return (cycleCount: _currentSessionCount, dayCount: _currentDay, repaired: _restored);
	}

	public bool IsTramRestored()
	{
		return _restored;
	}

	protected override SpawnPointData? GetPlayerStartPoint()
	{
		List<SpawnPointData> list = _playerStartSpawnPoints.Values.Where((SpawnPointData x) => x.IsFirstSpawnPoint != _everDeparted).ToList();
		if (list.Count == 0)
		{
			Logger.RWarn($"GetPlayerStartPoint warning, no matching firstSpawnPoint found. everDeparted: {_everDeparted}. use all spawn points.");
			list = _playerStartSpawnPoints.Values.ToList();
		}
		int index = _playerSpawnPointIndex++ % list.Count;
		if (list.Count > 0)
		{
			return list[index];
		}
		return null;
	}

	private void RollTramUpgradeCandidate()
	{
		List<int> list = (from x in Hub.s.dataman.ExcelDataManager.GetTramUpgradeMasterIDs()
			where !base.TramUpgradeList.Contains(x)
			orderby Guid.NewGuid()
			select x).ToList();
		if (list.Count >= 2)
		{
			_candidateTramUpgrades.leftPanel = list[0];
			_candidateTramUpgrades.rightPanel = list[1];
		}
		else if (list.Count == 1)
		{
			_candidateTramUpgrades.leftPanel = list[0];
			_candidateTramUpgrades.rightPanel = 0;
		}
		else
		{
			_candidateTramUpgrades.leftPanel = 0;
			_candidateTramUpgrades.rightPanel = 0;
		}
		_selectedUpgradeMasterID = _candidateTramUpgrades.leftPanel;
	}

	public bool PickUpgradeCandidate(int masterID, bool force = false)
	{
		if (_restored && !force)
		{
			Logger.RWarn("PickUpgradeCandidate warning, tram already restored.");
			return false;
		}
		if (masterID != _candidateTramUpgrades.leftPanel && masterID != _candidateTramUpgrades.rightPanel && !force)
		{
			Logger.RWarn($"PickUpgradeCandidate warning, invalid masterID picked. masterID: {masterID}, leftPanel: {_candidateTramUpgrades.leftPanel}, rightPanel: {_candidateTramUpgrades.rightPanel}");
			return false;
		}
		_selectedUpgradeMasterID = masterID;
		SendToAllPlayers(new PickTramUpgradeSig
		{
			selectedUpgradeMasterID = masterID
		});
		return true;
	}

	public bool ApplyTramUpgrage()
	{
		if (_selectedUpgradeMasterID != 0)
		{
			_tramUpgradeList.Add(_selectedUpgradeMasterID);
			int tramLevelObjectIDByUpgradeID = Hub.s.dynamicDataMan.GetTramLevelObjectIDByUpgradeID(_selectedUpgradeMasterID);
			if (tramLevelObjectIDByUpgradeID > 0 && _closedTramUpgradeObject.Contains(tramLevelObjectIDByUpgradeID))
			{
				_closedTramUpgradeObject.Remove(tramLevelObjectIDByUpgradeID);
			}
		}
		SendToAllPlayers(new ChangeTramPartsSig
		{
			sessionCount = base.SessionCycleCount,
			upgradeList = new List<int>(base.TramUpgradeList)
		});
		return true;
	}
}
