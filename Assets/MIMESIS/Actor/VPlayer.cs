using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.Cooked;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;

public class VPlayer : VCreature
{
	private SessionContext _sessionContext;

	private bool _isHost;

	private string _voiceUID = string.Empty;

	private SimpleVUMeter _voiceVUMeter = new SimpleVUMeter();

	public bool IsHost => _isHost;

	public string VoiceUID => _voiceUID;

	public ulong SteamID => _sessionContext.SteamID;

	public bool MyGameEntered => _sessionContext.MyGameEntered;

	public bool OtherGameEntered => _sessionContext.OtherGameEntered;

	public bool Wasted { get; private set; }

	public VRoomType ToMoveRoomType { get; private set; }

	public bool LevelLoadCompleted { get; private set; }

	public VPlayer(SessionContext sessionContext, int actorID, int masterID, bool isHost, string actorName, string voiceUID, PosWithRot position, bool isIndoor, IVroom room, ReasonOfSpawn reasonOfSpawn)
		: base(ActorType.Player, actorID, masterID, actorName, position, isIndoor, room, sessionContext.GetPlayerUID(), reasonOfSpawn)
	{
		_sessionContext = sessionContext;
		_isHost = isHost;
		_voiceUID = voiceUID;
		PlayerMasterInfo playerInfo = Hub.s.dataman.ExcelDataManager.GetPlayerInfo(masterID);
		if (playerInfo == null)
		{
			throw new Exception("characterInfo is null");
		}
		ImmutableArray<int>.Enumerator enumerator = playerInfo.Factions.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			m_DefaultFactions.Add(current);
		}
		ResetFaction();
		InitController();
		base.PrefabName = playerInfo.FppPuppetName;
		base.MoveCollisionRadius = playerInfo.MoveCollisionRadius;
		base.HitCollisionRadius = playerInfo.HitCollisionRadius;
		IHitCheck hurtBox = Hub.s.dataman.AnimNotiManager.GetHurtBox(base.PrefabName);
		if (hurtBox == null)
		{
			Logger.RError("HitCheck is null for player prefab: " + base.PrefabName);
		}
		else
		{
			base.HitCheck = hurtBox;
		}
		base.Height = ((hurtBox != null && base.HitCheck is CapsuleHitCheck capsuleHitCheck) ? capsuleHitCheck.Length : 1.5f);
	}

	protected override void InitDefaultParam()
	{
		_sessionContext.ResetSnapShotRoomType();
		PlayerInfoSnapshot playerInfoSnapshot = _sessionContext.PlayerInfoSnapshot;
		foreach (KeyValuePair<int, ItemElement> item in playerInfoSnapshot.inventories.OrderBy((KeyValuePair<int, ItemElement> x) => x.Key))
		{
			if (VRoom.FindLootingObjectByItemID(item.Value.ItemID) == null)
			{
				base.InventoryControlUnit.AddInvenItem(item.Key, item.Value, sync: false);
			}
		}
		int num = 1;
		if (VRoom is MaintenanceRoom maintenanceRoom)
		{
			List<int> supplyItemMasterIDs = maintenanceRoom.GetSupplyItemMasterIDs(UID);
			if (supplyItemMasterIDs.Count > 0)
			{
				base.InventoryControlUnit.ClearAllInventory(sync: false);
				foreach (int item2 in supplyItemMasterIDs)
				{
					ItemElement newItemElement = VRoom.GetNewItemElement(item2, isFake: false);
					if (newItemElement == null)
					{
						Logger.RError($"InventoryController.GenerateFakeItemForAI: newItem is null. masterID: {item2}");
					}
					else
					{
						base.InventoryControlUnit.AddInvenItem(num++, newItemElement);
					}
				}
			}
		}
		base.InventoryControlUnit.HandleChangeActiveInvenSlot(playerInfoSnapshot.CurrentInventorySlot, sync: false);
	}

	public override void FillSightInSig(ref SightInSig sig)
	{
		PlayerInfo info = new PlayerInfo();
		GetOtherPlayerInfo(ref info);
		sig.playerInfos.Add(info);
	}

	public void HandleLevelLoadComplete(int hashCode)
	{
		if (!VRoom.EnterSpace(this))
		{
			SendToMe(new LevelLoadCompleteRes(hashCode)
			{
				errorCode = MsgErrorCode.CantEnterSpace
			});
			return;
		}
		LevelLoadCompleteRes res = new LevelLoadCompleteRes(hashCode)
		{
			targetCurrency = VRoom.Property.TargetCurrency,
			firstEnterMap = !MyGameEntered,
			boostedItem = (VRoom.BoostedItemMasterID, VRoom.BoostedItemRate),
			dayCount = VRoom.CurrentDay,
			sessionCount = VRoom.SessionCycleCount,
			tramUpgradeList = new List<int>(VRoom.TramUpgradeList)
		};
		foreach (KeyValuePair<int, ItemElement> stash in VRoom.Stashes)
		{
			res.stashes[stash.Key] = stash.Value.toItemInfo();
		}
		if (!MyGameEntered)
		{
			_sessionContext.ApplyMyGameEntered();
		}
		PlayerInfo info = res.selfInfo;
		GetMyActorInfo(ref info);
		LevelLoadCompleted = true;
		VRoom.GetLevelObjectInfos(ref res);
		VRoom.OnLevelLoadComplete(this);
		if (VRoom is DungeonRoom dungeonRoom)
		{
			res.currentTime = dungeonRoom.GetCurrentTime();
		}
		SendToMe(res);
	}

	public override SendResult SendToMe(IMsg msg)
	{
		if (msg is IActorMsg { actorID: 0 })
		{
			Logger.RWarn($"[Player:{UID}] actorID is 0, msgType: {msg.msgType}, hashCode: {msg.hashCode}");
			return SendResult.CantFoundRoutingTarget;
		}
		return _sessionContext.Send(msg);
	}

	public void PostHandler<T>(OnPlayerDispatchEventHandler<T> handler, T msg) where T : IMsg, new()
	{
		_commandExecutor.Invoke(delegate
		{
			handler(this, msg);
		});
	}

	public void PostAsyncHandler<T>(OnPlayerDispatchAsyncEventHandler<T> handler, T msg) where T : IMsg, new()
	{
		_commandExecutor.Invoke(async delegate
		{
			await handler(this, msg);
		});
	}

	public MsgErrorCode HandleChangeNextDungeonID(int selectedDungeonMasterID, int hashCode)
	{
		if (!(VRoom is VWaitingRoom vWaitingRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		MsgErrorCode msgErrorCode = vWaitingRoom.OnRequestChangeNextDungeonID(selectedDungeonMasterID);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		SendToMe(new ChangeNextDungeonRes(hashCode)
		{
			errorCode = msgErrorCode
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandleStartGame(int hashCode, int forceDungeonMasterID)
	{
		if (!(VRoom is VWaitingRoom vWaitingRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		MsgErrorCode msgErrorCode = vWaitingRoom.OnRequestStartGame(forceDungeonMasterID);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		SendToMe(new StartGameRes(hashCode));
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandleStartSession(int hashCode)
	{
		if (!(VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		MsgErrorCode msgErrorCode = maintenanceRoom.OnRequestStartSession();
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		SendToMe(new StartSessionRes(hashCode));
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandleEndSession(int hashCode)
	{
		if (!(VRoom is VWaitingRoom vWaitingRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		MsgErrorCode msgErrorCode = vWaitingRoom.OnRequestEndSession();
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		SendToMe(new EndSessionRes(hashCode));
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandleSaveGameDataReq(int saveSlotID, List<string> playerNames, int hashCode)
	{
		if (!IsHost)
		{
			return MsgErrorCode.CantSave_NotHost;
		}
		if (!(VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.CantSave_InvalidRoomType;
		}
		if (!MMSaveGameData.CheckSaveSlotID(saveSlotID, includeAutoSlot: false))
		{
			return MsgErrorCode.CantSave_InvalidSlot;
		}
		MsgErrorCode msgErrorCode = maintenanceRoom.SaveGameData(saveSlotID, playerNames);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		SendToMe(new SaveGameDataRes(hashCode));
		SendInSight(new SaveGameDataSig());
		return MsgErrorCode.Success;
	}

	public void OnExitChannel(bool backupFlag)
	{
		_sessionContext.CreatePlayerSnapshot(backupFlag);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && _disposed.On())
		{
			base.Dispose(disposing);
		}
	}

	public void GetMyActorInfo(ref PlayerInfo info)
	{
		OtherCreatureInfo info2 = info;
		GetOtherActorInfo(ref info2);
		info.currentInventorySlot = base.InventoryControlUnit.GetCurrentInvenSlotIndex();
	}

	public void GetOtherPlayerInfo(ref PlayerInfo info)
	{
		GetMyActorInfo(ref info);
		info.steamID = SteamID;
		info.firstEnterMap = !OtherGameEntered;
	}

	public PlayerSessionInfo GetPlayerInfo()
	{
		return new PlayerSessionInfo
		{
			isHost = _isHost,
			playerUID = UID,
			guid = _sessionContext.GUID,
			steamID = _sessionContext.SteamID
		};
	}

	public MsgErrorCode HandleGrapLootingObject(int lootingObjectID, int hashCode)
	{
		MsgErrorCode msgErrorCode = CanAction(VActorActionType.Looting);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		VActor vActor = VRoom.FindActorByObjectID(lootingObjectID);
		if (vActor == null || !(vActor is VLootingObject vLootingObject))
		{
			return MsgErrorCode.ActorNotFound;
		}
		if (base.InventoryControlUnit.InvenFull())
		{
			return MsgErrorCode.InventoryFull;
		}
		if (vLootingObject.IsFake())
		{
			VRoom.DestroyFakeObject(lootingObjectID);
			return MsgErrorCode.FakeItemRemoved;
		}
		if (vLootingObject.Assigned)
		{
			return MsgErrorCode.LootingObjectAlreadyAssigned;
		}
		vLootingObject.SetAssigned();
		ItemElement itemElement = vLootingObject.GetItemElement();
		int addedSlotIndex;
		MsgErrorCode msgErrorCode2 = base.InventoryControlUnit.HandleAddItem(itemElement, out addedSlotIndex, sync: true, byLooting: true);
		if (msgErrorCode2 != MsgErrorCode.Success)
		{
			return msgErrorCode2;
		}
		vLootingObject.CheckBlackoutByItem(base.ObjectID);
		if (!(VRoom is VWaitingRoom) && vLootingObject.GetItemElement().IsItemCheckSpawnFieldSkill)
		{
			vLootingObject.GetItemElement().CheckSpawnFieldSkillWaitPeriod(VRoom, this);
		}
		VRoom.PendRemoveActor(lootingObjectID);
		if (base.InventoryControlUnit.CurrentInventorySlot != addedSlotIndex)
		{
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemElement.ItemMasterID);
			if (itemInfo != null && itemInfo.ForbidChange)
			{
				base.InventoryControlUnit.HandleChangeActiveInvenSlot(addedSlotIndex);
			}
		}
		SendToMe(new GrapLootingObjectRes(hashCode)
		{
			inventoryInfos = base.InventoryControlUnit.GetInventoryInfos(),
			currentInvenSlotIndex = base.InventoryControlUnit.GetCurrentInvenSlotIndex()
		});
		base.EmotionControlUnit.OnLooting();
		base.ScrapMotionController.OnLooting();
		VRoom.CheckTriggerVolumeEvent(this, base.PositionVector, base.PositionVector, MapTrigger.eCheckTypeFlag.Inside);
		AddGameEventLog(new GELCollectItem
		{
			RoomSessionID = VRoom.SessionID,
			SessionCycleCount = VRoom.SessionCycleCount,
			StageCount = VRoom.CurrentDay,
			ItemMasterID = vLootingObject.GetItemElement().ItemMasterID,
			Timestamp = Hub.s.timeutil.GetTimestamp()
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandleLevelObject(int levelObjectID, int state, bool occupy, int hashCode)
	{
		MsgErrorCode msgErrorCode = CanAction(VActorActionType.UseLevelObject);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		int prevState;
		MsgErrorCode msgErrorCode2 = VRoom.HandleLevelObject(base.ObjectID, levelObjectID, state, occupy, out prevState);
		if (msgErrorCode2 != MsgErrorCode.Success)
		{
			return msgErrorCode2;
		}
		base.EmotionControlUnit?.OnMove();
		AddGameEventLog(new GELUseLevelObject
		{
			RoomSessionID = VRoom.SessionID,
			SessionCycleCount = VRoom.SessionCycleCount,
			StageCount = VRoom.CurrentDay,
			ActorType = ActorType,
			LevelObjectType = (LevelObjectType)state,
			FromState = prevState,
			ToState = state,
			Timestamp = Hub.s.timeutil.GetTimestamp()
		});
		SendToMe(new UseLevelObjectRes(hashCode)
		{
			fromState = prevState,
			toState = state
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandleBuyItem(int itemMasterID, int hashCode, int machineIndex)
	{
		if (!(VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		MsgErrorCode msgErrorCode = maintenanceRoom.BuyItem(itemMasterID, this);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			SendToMe(new BuyItemRes(hashCode)
			{
				errorCode = msgErrorCode,
				remainCurrency = maintenanceRoom.Currency
			});
			return msgErrorCode;
		}
		SendToMe(new BuyItemRes(hashCode)
		{
			remainCurrency = maintenanceRoom.Currency
		});
		SendInSight(new BuyItemSig
		{
			itemMasterID = itemMasterID,
			machineIndex = machineIndex
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandlePutIntoToilet(int hashCode)
	{
		if (!(VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		ItemElement itemElement = base.InventoryControlUnit.HandleExtractItem(0, adandonFree: true);
		if (itemElement == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		var (msgErrorCode, currency) = maintenanceRoom.PutIntoToilet(itemElement);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		SendToMe(new PutIntoToiletRes(hashCode));
		SendToChannel(new ChangeCurrencySig
		{
			currency = currency
		});
		return MsgErrorCode.Success;
	}

	public override void Update(long elapsed)
	{
		base.Update(elapsed);
		if (Hub.s.voiceman.TryGetVoiceAmplitude(_voiceUID, out var amplitude, out var isSpeaking))
		{
			_voiceVUMeter?.Update(amplitude, (float)elapsed * 0.001f, !isSpeaking);
		}
	}

	public long GetVoicePeak()
	{
		return (long)(_voiceVUMeter.Peak * 100f);
	}

	public override void CollectDebugInfo(ref DebugInfoSig sig)
	{
		CollectHitCheckInfo(base.HitCheck, ref sig);
	}

	public MsgErrorCode HandleRepairTrain(int hashCode)
	{
		if (!(VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		var (msgErrorCode, remainCurrency) = maintenanceRoom.RepairTrain();
		SendToMe(new RepairTramRes(hashCode)
		{
			errorCode = msgErrorCode
		});
		if (msgErrorCode == MsgErrorCode.Success)
		{
			SendToChannel(new StartRepairTramSig
			{
				remainCurrency = remainCurrency
			});
		}
		return MsgErrorCode.Success;
	}

	public PlayerStatusInfo GetPlayerStatusInfo()
	{
		PlayerStatusInfo obj = new PlayerStatusInfo
		{
			actorID = base.ObjectID,
			actorLifeCycle = LifeCycle,
			position = base.Position
		};
		Dictionary<int, ItemInfo> info = obj.inventories;
		base.InventoryControlUnit.GetInventoryInfo(ref info);
		obj.currentInventorySlot = base.InventoryControlUnit.GetCurrentInvenSlotIndex();
		return obj;
	}

	public void SetOtherGameEntered()
	{
		_sessionContext.ApplyOtherGameEntered();
	}

	public void SetWasted()
	{
		Wasted = true;
	}

	public void SetToMoveRoomType(VRoomType toMove)
	{
		ToMoveRoomType = toMove;
	}

	public MsgErrorCode HandleHangItem(int index, int hashcode)
	{
		ItemElement itemElement = base.InventoryControlUnit.HandleExtractItem(0, adandonFree: true);
		if (itemElement == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!VRoom.HangItem(index, itemElement))
		{
			return MsgErrorCode.CannotHandleItem;
		}
		SendToMe(new HangItemRes(hashcode));
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandleUnhangItem(int index, int hashcode)
	{
		MsgErrorCode msgErrorCode = CanAction(VActorActionType.Looting);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		if (!base.InventoryControlUnit.IsAnySlotEmpty())
		{
			return MsgErrorCode.CannotHandleItem;
		}
		if (!VRoom.UnhangItem(index, out ItemElement itemElement))
		{
			return MsgErrorCode.ItemNotFound;
		}
		int addedSlotIndex;
		MsgErrorCode msgErrorCode2 = base.InventoryControlUnit.HandleAddItem(itemElement, out addedSlotIndex, sync: true, byLooting: true);
		if (msgErrorCode2 != MsgErrorCode.Success)
		{
			return msgErrorCode2;
		}
		if (base.InventoryControlUnit.CurrentInventorySlot != addedSlotIndex)
		{
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemElement.ItemMasterID);
			if (itemInfo != null && itemInfo.ForbidChange)
			{
				base.InventoryControlUnit.HandleChangeActiveInvenSlot(addedSlotIndex);
			}
		}
		SendToMe(new UnhangItemRes(hashcode));
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandlePickTramUpgrade(int selectedUpgradeMasterID, int hashCode)
	{
		if (!(VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		if (!maintenanceRoom.PickUpgradeCandidate(selectedUpgradeMasterID))
		{
			return MsgErrorCode.ServerOperationFailed;
		}
		SendToMe(new PickTramUpgradeRes(hashCode));
		return MsgErrorCode.Success;
	}

	public MsgErrorCode HandleReinforceItem(int hashCode)
	{
		if (!(VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidRoomType;
		}
		int currentInventorySlotItemMasterID = base.InventoryControlUnit.GetCurrentInventorySlotItemMasterID();
		if (currentInventorySlotItemMasterID == 0)
		{
			return MsgErrorCode.ItemNotFound;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(currentInventorySlotItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			return MsgErrorCode.InvalidItem;
		}
		if (!itemEquipmentInfo.CanUpgrade)
		{
			return MsgErrorCode.InvalidItem;
		}
		MapMarker_LootingObjectSpawnPoint reinforceLootingObjectSpawnPoint = Hub.s.dynamicDataMan.GetReinforceLootingObjectSpawnPoint();
		if (reinforceLootingObjectSpawnPoint == null)
		{
			return MsgErrorCode.ServerOperationFailed;
		}
		if (maintenanceRoom.Currency < itemEquipmentInfo.UpgradeCost)
		{
			return MsgErrorCode.NotEnoughCurrency;
		}
		if (base.InventoryControlUnit.HandleExtractItem(0, adandonFree: true) == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		int upgradeItemMasterID = itemEquipmentInfo.UpgradeItemMasterID;
		MsgErrorCode msgErrorCode = maintenanceRoom.ReinforceItem(upgradeItemMasterID, itemEquipmentInfo.UpgradeCost, reinforceLootingObjectSpawnPoint.pos);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		SendToMe(new ReinforceItemRes(hashCode));
		return MsgErrorCode.Success;
	}
}
