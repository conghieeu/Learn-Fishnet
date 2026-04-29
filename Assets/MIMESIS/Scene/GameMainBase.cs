using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using DLAgent;
using DarkTonic.MasterAudio;
using Dissonance;
using Dissonance.Integrations.FishNet;
using DunGen;
using Mimic;
using Mimic.Actors;
using Mimic.Audio;
using Mimic.Character;
using Mimic.Character.HitSystem;
using Mimic.InputSystem;
using Mimic.Voice.SpeechSystem;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluReplay.Shared;
using Steamworks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.CrashReportHandler;
using UnityEngine.Events;

public class GameMainBase : MonoBehaviour
{
	public class SyncRandom
	{
		private System.Random random;

		public SyncRandom(int seed)
		{
			random = new System.Random(seed);
		}

		public int Next(int minValue, int maxValue)
		{
			return random.Next(minValue, maxValue);
		}
	}

	protected class NetSyncGameData
	{
		public int currency;

		public int targetCurrency;

		public TimeSpan currentTime = TimeSpan.Zero;

		public int dayCount;

		public ItemMasterInfo? boostedItem;

		public float boostedRatio = 1f;

		public Dictionary<int, ItemInfo> stashesOnHanger = new Dictionary<int, ItemInfo>();
	}

	public class FieldSkillSignEffectPlayData
	{
		public ProtoActor actor;

		public float startTimeSec;

		public long itemId;

		public long itemMasterId;

		public bool isHandheldItem;

		public bool isCancelRequested;

		public FieldSkillSignEffectPlayData(ProtoActor actor, float startTimeSec, long itemId, bool isHandheldItem, long itemMasterId)
		{
			this.actor = actor;
			this.startTimeSec = startTimeSec;
			this.itemId = itemId;
			this.isHandheldItem = isHandheldItem;
			this.itemMasterId = itemMasterId;
			isCancelRequested = false;
		}
	}

	public class LevelObjectHelper
	{
		private LevelObject? activeLevelObject;

		private Dictionary<int, LootingLevelObject> lootingObjects = new Dictionary<int, LootingLevelObject>();

		private Dictionary<int, VendingMachineLevelObject> vendingMachines = new Dictionary<int, VendingMachineLevelObject>();

		private Dictionary<int, StashHangerLevelObject> stashHangerLevelObjects = new Dictionary<int, StashHangerLevelObject>();

		private MapRerollLevelObject mapRerollLevelObject;

		private ScrapScanLevelObject scrapScanLevelObject;

		private List<LootingLevelObject> destroyDelayedLootingObjects = new List<LootingLevelObject>();

		private GameMainBase main;

		private List<LevelObject> allLevelObjects = new List<LevelObject>();

		private TilesByXZ<LevelObject> levelObjectTiles = new TilesByXZ<LevelObject>(5f);

		private ProtoActor? _myAvatar => main._myAvatar;

		private UIPrefab_crosshair crosshairui => main.crosshairui;

		public LevelObjectHelper(GameMainBase main)
		{
			this.main = main;
		}

		public void ClearCrosshair()
		{
			if (crosshairui != null)
			{
				crosshairui.HideCrosshair();
				crosshairui.HideText();
			}
		}

		public void Step()
		{
			if (_myAvatar == null || Hub.s == null || Camera.main == null)
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
			float maxMapObjectInteractionDistance = main.gameConfig.playerActor.maxMapObjectInteractionDistance;
			if (LevelObjectUtility.TryPick4(levelObjectTiles, ray.origin, ray.direction.normalized, maxMapObjectInteractionDistance, out LevelObject pickedLevelObject))
			{
				if (activeLevelObject != null && activeLevelObject != pickedLevelObject)
				{
					activeLevelObject.TryInteractEnd(main._myAvatar);
				}
				if (pickedLevelObject != null)
				{
					if (pickedLevelObject.NeedToShowCrossHair(main._myAvatar))
					{
						crosshairui.ShowCrosshair(pickedLevelObject.GetCrossHairType(main._myAvatar), pickedLevelObject.GetCrossHairAnimDuration());
						crosshairui.ShowText(pickedLevelObject, main._myAvatar);
					}
					else
					{
						crosshairui.HideCrosshair();
						crosshairui.HideText();
					}
				}
			}
			else
			{
				if (activeLevelObject != null)
				{
					activeLevelObject.TryInteractEnd(main._myAvatar);
				}
				crosshairui.HideCrosshair();
				crosshairui.HideText();
			}
			activeLevelObject = pickedLevelObject;
		}

		public void InitAllLevelObjects()
		{
			allLevelObjects.Clear();
			allLevelObjects.AddRange(Hub.s.dynamicDataMan.GetAllLevelObjects(excludeClientOnly: false).Values);
			foreach (LevelObject allLevelObject in allLevelObjects)
			{
				if (allLevelObject is VendingMachineLevelObject vendingMachineLevelObject)
				{
					vendingMachines.Add(vendingMachineLevelObject.levelObjectID, vendingMachineLevelObject);
				}
				if (allLevelObject is StashHangerLevelObject stashHangerLevelObject)
				{
					stashHangerLevelObjects.Add(stashHangerLevelObject.index, stashHangerLevelObject);
				}
				if (allLevelObject is MapRerollLevelObject mapRerollLevelObject)
				{
					this.mapRerollLevelObject = mapRerollLevelObject;
				}
				if (allLevelObject is ScrapScanLevelObject scrapScanLevelObject)
				{
					this.scrapScanLevelObject = scrapScanLevelObject;
				}
			}
			List<TextLevelObject> collection = ReLUGameKitUtility.MakeListByHierarchyOrder<TextLevelObject>(main.GetBGRoot());
			allLevelObjects.AddRange(collection);
			levelObjectTiles.AddItems(allLevelObjects);
		}

		public List<LevelObject> GetAllLevelObjects()
		{
			return allLevelObjects;
		}

		public List<LevelObject> CollectLootingObjects()
		{
			List<LevelObject> list = new List<LevelObject>();
			list.AddRange(lootingObjects.Values);
			return list;
		}

		public bool TryPerformInteraction()
		{
			if (_myAvatar == null || activeLevelObject == null || !activeLevelObject.gameObject.activeInHierarchy)
			{
				return false;
			}
			return activeLevelObject.TryInteract(_myAvatar);
		}

		public bool TryPerformInteractionEnd()
		{
			if (_myAvatar == null || activeLevelObject == null || !activeLevelObject.gameObject.activeInHierarchy)
			{
				return false;
			}
			return activeLevelObject.TryInteractEnd(_myAvatar);
		}

		public void TryDestroyLootingItem(int itemActorId)
		{
			if (lootingObjects.TryGetValue(itemActorId, out LootingLevelObject value))
			{
				value.OnDespawnByDestroy();
				lootingObjects.Remove(itemActorId);
				if (allLevelObjects.Contains(value))
				{
					allLevelObjects.Remove(value);
					levelObjectTiles.RemoveItem(value.transform.position, value);
				}
				SliceItemWhenDestoryed(null, value.transform, value.itemMasterID);
				if (value != null && value.gameObject != null)
				{
					UnityEngine.Object.Destroy(value.gameObject);
				}
			}
		}

		public float SliceItemWhenDestoryed(Transform? puppetTransform, Transform itemTransform, int itemMasterId)
		{
			MeshSlicer componentInChildren = itemTransform.GetComponentInChildren<MeshSlicer>();
			if (componentInChildren != null)
			{
				Vector3 externalForce = Vector3.up * 10f;
				if (ProtoActor.Inventory.GetItemMasterInfo(itemMasterId) is ItemEquipmentInfo itemEquipmentInfo)
				{
					externalForce = ((!(puppetTransform != null)) ? itemEquipmentInfo.DirectionWhenDestroyed : (puppetTransform.rotation * itemEquipmentInfo.DirectionWhenDestroyed));
				}
				try
				{
					componentInChildren.Switch(externalForce);
				}
				catch (NullReferenceException)
				{
					Logger.RError("Failed to switch MeshSlicer on " + itemTransform.gameObject.name + ".");
				}
				return componentInChildren.fragmentLifeTime + componentInChildren.fragmentFadeTime;
			}
			return 0f;
		}

		public (bool isSkinned, GameObject spawnedItemObject) TrySpawnItemObject(int itemMasterID, Transform parentTransform)
		{
			var (item, skinnedItemInfo) = Hub.s.tableman.lootingObject.FindSkinnedItemInfo(itemMasterID);
			if (skinnedItemInfo == null)
			{
				Logger.RError($"skinnedItemInfo is null @ TrySpawnLootingObject: lootingInfo.masterID={itemMasterID}");
				return (isSkinned: false, spawnedItemObject: null);
			}
			LevelObject component = UnityEngine.Object.Instantiate(skinnedItemInfo.prefab, parentTransform).GetComponent<LevelObject>();
			component.maxInteractionDistance = 0f;
			return (isSkinned: item, spawnedItemObject: component.gameObject);
		}

		public void TrySpawnLootingObject(LootingObjectInfo lootingInfo)
		{
			if (ReplaySharedData.IsReplayPlayMode && lootingObjects.ContainsKey(lootingInfo.actorID))
			{
				return;
			}
			MMLootingObjectTable.SkinnedItemInfo item = Hub.s.tableman.lootingObject.FindSkinnedItemInfo(lootingInfo.masterID).skinnedItemInfo;
			if (item == null)
			{
				Logger.RError($"skinnedItemInfo is null @ TrySpawnLootingObject: lootingInfo.masterID={lootingInfo.masterID}");
				return;
			}
			Vector3 position = lootingInfo.position.toVector3();
			Transform transform = null;
			if (main.dungenCuller != null)
			{
				Tile tile = main.dungenCuller.relumod_FastFindCurrentTile(position, 1f);
				transform = ((tile != null) ? tile.transform : ((!(main is GamePlayScene gamePlayScene)) ? main.GetBGRoot() : gamePlayScene.GetOutdoorLootingObjectRoot()));
			}
			else
			{
				transform = main.GetBGRoot();
			}
			LootingLevelObject component = UnityEngine.Object.Instantiate(item.prefab, transform).GetComponent<LootingLevelObject>();
			component.ActorID = lootingInfo.actorID;
			component.itemMasterID = lootingInfo.masterID;
			component.marketPrice = lootingInfo.linkedItemInfo.price;
			component.isFake = lootingInfo.linkedItemInfo.isFake;
			component.transform.SetParent(transform);
			component.transform.position = position;
			component.transform.rotation = Quaternion.Euler(0f, lootingInfo.position.yaw, 0f);
			if (main.dungenCuller != null)
			{
				Tile componentInParent = component.GetComponentInParent<Tile>();
				if (componentInParent != null)
				{
					main.dungenCuller.relumod_AddRenderersToTile(componentInParent, component.transform);
				}
			}
			component.OnSpawn(lootingInfo.reasonOfSpawn);
			if (component.TryGetComponent<ISpawnableItem>(out var component2))
			{
				component2.OnSpawned(lootingInfo.linkedItemInfo);
			}
			lootingObjects.TryAdd(component.ActorID, component);
			allLevelObjects.Add(component);
			levelObjectTiles.AddItem(component.transform.position, component);
			main.OnStashChanged();
		}

		public bool TryDespawnLootingObject(int actorID)
		{
			if (lootingObjects.TryGetValue(actorID, out LootingLevelObject value))
			{
				if (main.dungenCuller != null)
				{
					Tile componentInParent = value.GetComponentInParent<Tile>();
					if (componentInParent != null)
					{
						main.dungenCuller.relumod_RemoveRenderersFromTile(componentInParent, value.transform);
					}
				}
				if (!destroyDelayedLootingObjects.Contains(value))
				{
					UnityEngine.Object.Destroy(value.gameObject);
				}
				lootingObjects.Remove(actorID);
				if (allLevelObjects.Contains(value))
				{
					allLevelObjects.Remove(value);
					levelObjectTiles.RemoveItem(value.transform.position, value);
				}
				main.OnStashChanged();
				return true;
			}
			return false;
		}

		public void TryTeleportLootingObject(TeleportSig sig)
		{
			if (lootingObjects.TryGetValue(sig.actorID, out LootingLevelObject value))
			{
				value.transform.SetPositionAndRotation(sig.pos.toVector3(), Quaternion.Euler(0f, sig.pos.yaw, 0f));
			}
		}

		public void OnBuyItemSig(BuyItemSig sig)
		{
			if (vendingMachines.TryGetValue(sig.machineIndex, out VendingMachineLevelObject value))
			{
				value.OnBuyItem();
			}
		}

		public void OnHangItem(int index, ItemInfo itemInfo)
		{
			if (stashHangerLevelObjects.TryGetValue(index, out StashHangerLevelObject value))
			{
				value.OnHangItem(itemInfo);
			}
		}

		public void OnUnhangItem(int index)
		{
			if (stashHangerLevelObjects.TryGetValue(index, out StashHangerLevelObject value))
			{
				value.OnUnhangItem();
			}
		}

		public void OnScrapScanning(int remainValue)
		{
			if (scrapScanLevelObject != null)
			{
				scrapScanLevelObject.OnScrapScanning(remainValue);
			}
		}

		public void UpdateDungeonCandidates(int newDungeonMasterID1, int newDungeonMasterID2)
		{
			if (mapRerollLevelObject != null)
			{
				mapRerollLevelObject.UpdateDungeonCandidates(newDungeonMasterID1, newDungeonMasterID2);
			}
		}

		public void NotifyTramStartLeverPullingStarted()
		{
			if (mapRerollLevelObject != null)
			{
				mapRerollLevelObject.NotifyTramStartLeverPullingStarted();
			}
		}

		public void OnChangeLevelObjectStateSig(UseLevelObjectSig packet)
		{
			if (packet.changedLevelObject != null && Hub.s.dynamicDataMan.GetAllLevelObjects(excludeClientOnly: false).TryGetValue(packet.changedLevelObject.levelObjectID, out LevelObject value))
			{
				value.OnChangeLevelObjectStateSig(packet.actorID, packet.changedLevelObject.OccupiedActorID, packet.changedLevelObject.prevState, packet.changedLevelObject.CurrentState);
			}
		}

		public void PlaySignOfFieldSkillByItem(long actorId, long itemId, long itemMasterId)
		{
			if (lootingObjects.TryGetValue((int)actorId, out LootingLevelObject value) && value.itemMasterID == itemMasterId)
			{
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo((int)itemMasterId);
				if (itemInfo != null && itemInfo.SpawnFieldSkillWaitEffectName.Length > 0)
				{
					string spawnFieldSkillWaitEffectName = itemInfo.SpawnFieldSkillWaitEffectName;
					string spawnFieldSkillWaitEffectSocket = itemInfo.SpawnFieldSkillWaitEffectSocket;
					float duration = (float)itemInfo.SpawnFieldSkillWaitEffectDurationMSec / 1000f;
					Hub.s.pdata.main.PlayFieldSkillSignEffect(value.transform, spawnFieldSkillWaitEffectSocket, spawnFieldSkillWaitEffectName, duration);
				}
			}
		}

		public void StopSignOfFieldSkillByItem(long actorId, long itemId, long itemMasterId)
		{
			if (lootingObjects.TryGetValue((int)actorId, out LootingLevelObject value) && value.itemMasterID == itemMasterId)
			{
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo((int)itemMasterId);
				if (itemInfo != null && itemInfo.SpawnFieldSkillWaitEffectName.Length > 0)
				{
					string spawnFieldSkillWaitEffectName = itemInfo.SpawnFieldSkillWaitEffectName;
					Hub.s.pdata.main.StopFieldSkillSignEffect(value.transform, spawnFieldSkillWaitEffectName);
				}
			}
		}
	}

	public class PacketMediator
	{
		public enum ePacketRecipientType
		{
			GameMain = 0,
			Avatar = 1,
			Actor = 2,
			Ignore = 3
		}

		private GameMainBase main;

		public readonly int defaultAddress = -1;

		private readonly ImmutableHashSet<MsgType> verboseMsgTypes = ImmutableHashSet.Create<MsgType>(MsgType.C2G_HeartBeatRes, MsgType.C2S_AnnounceImmutableStatSig, MsgType.C2S_AnnounceMutableStatSig, MsgType.C2S_ChangeViewPointRes, MsgType.C2S_ChangeViewPointSig, MsgType.C2S_MoveStartRes, MsgType.C2S_MoveStartSig, MsgType.C2S_MoveStopRes, MsgType.C2S_MoveStopSig, MsgType.C2S_NetworkGradeSig, MsgType.C2S_RelayPacket, MsgType.C2S_RoomStatusSig);

		private readonly ImmutableHashSet<MsgType> ignoreMsgTypes = ImmutableHashSet.Create<MsgType>(MsgType.C2S_ChangeSprintModeSig, MsgType.C2S_ToggleSprintRes, MsgType.C2S_UseItemRes, MsgType.C2S_UseTempActionRes);

		private Dictionary<(Type, int), Action<IMsg>> handlers = new Dictionary<(Type, int), Action<IMsg>>();

		private HashSet<MsgType> avatarResPakcetsSet = new HashSet<MsgType>();

		public PacketMediator(GameMainBase main)
		{
			this.main = main;
			AutoRegisterHandlers(main, defaultAddress);
		}

		private bool TryDequeueMsg(out (bool, IMsg) msgPair)
		{
			if (Hub.s.replayManager.IsReplayPlayMode)
			{
				IMsg msg;
				bool flag = Hub.s.replayManager.OnTryDequeueMsg(out msg);
				if (msg == null)
				{
					msgPair = (false, new IMsg(MsgType.Invalid));
					return false;
				}
				msgPair = (flag, msg);
				return flag;
			}
			return main.netman2.TryDequeueMsg(out msgPair);
		}

		public void PumpPackets()
		{
			if (Hub.s == null)
			{
				return;
			}
			(bool, IMsg) msgPair;
			while (TryDequeueMsg(out msgPair))
			{
				var (flag, msg) = msgPair;
				verboseMsgTypes.Contains(msg.msgType);
				if (ignoreMsgTypes.Contains(msg.msgType))
				{
					continue;
				}
				CrashReportHandler.SetUserMetadata("last_process_packet_type", msg.msgType.ToString());
				int item;
				if (msg is IActorMsg actorMsg)
				{
					item = actorMsg.actorID;
				}
				else if (avatarResPakcetsSet.Contains(msg.msgType))
				{
					int num = defaultAddress;
					if (!(main != null) || !(main.GetMyAvatar() != null))
					{
						continue;
					}
					num = main.GetMyAvatar().ActorID;
					item = num;
				}
				else
				{
					item = defaultAddress;
				}
				if (handlers.TryGetValue((msg.GetType(), item), out Action<IMsg> value))
				{
					value(msg);
				}
				else if (msg.msgType != MsgType.C2S_AnnounceMutableStatSig && flag)
				{
				}
			}
		}

		public void AutoRegisterHandlers(object obj, int address)
		{
			MethodInfo[] methods = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (MethodInfo method in methods)
			{
				object[] customAttributes = method.GetCustomAttributes(typeof(PacketHandlerAttribute), inherit: true);
				if (customAttributes.Length == 0)
				{
					continue;
				}
				PacketHandlerAttribute packetHandlerAttribute = (PacketHandlerAttribute)customAttributes[0];
				if (packetHandlerAttribute == null)
				{
					continue;
				}
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length != 1)
				{
					Logger.RError("[PacketMediator] Invalid Packet Handler Method: " + method.Name);
					continue;
				}
				Type parameterType = parameters[0].ParameterType;
				if (!parameterType.IsSubclassOf(typeof(IMsg)))
				{
					Logger.RError("[PacketMediator] Invalid Packet Handler Method: " + method.Name);
					continue;
				}
				if (packetHandlerAttribute.isAvatarResPacket)
				{
					AvatarResPacketHandlerAttribute avatarResPacketHandlerAttribute = packetHandlerAttribute as AvatarResPacketHandlerAttribute;
					avatarResPakcetsSet.Add(avatarResPacketHandlerAttribute.msgType);
				}
				Action<IMsg> action = delegate(IMsg packet)
				{
					method.Invoke(obj, new object[1] { packet });
				};
				if (action == null)
				{
					Logger.RError("[PacketMediator] Invalid Packet Handler Method: " + method.Name);
				}
				else if (handlers.ContainsKey((parameterType, address)))
				{
					Logger.RError($"[PacketMediator] Handler already exists for {parameterType} #{address}");
				}
				else if (parameterType.IsSubclassOf(typeof(IActorMsg)) && address == defaultAddress)
				{
					Logger.RError("[PacketMediator] Can't register handler IActorMsg in default address: " + method.Name + "(" + parameterType.Name + ")");
				}
				else
				{
					handlers.Add((parameterType, address), action);
				}
			}
		}
	}

	private List<Vector3> indoorTeleporterPositions = new List<Vector3>();

	private List<Vector3> outdoorTeleporterPositions = new List<Vector3>();

	[SerializeField]
	private GameObject uiprefab_spectator;

	[SerializeField]
	private GameObject prefab_ui_ingame;

	[SerializeField]
	protected GameObject prefab_ui_crosshair;

	[SerializeField]
	protected GameObject prefab_ui_inventory;

	[SerializeField]
	protected GameObject prefab_ui_GameStatus;

	[SerializeField]
	protected GameObject prefab_ui_playerEnterInfo;

	[SerializeField]
	protected Transform BGRoot;

	[SerializeField]
	protected CutScenePlayer cutScenePlayer;

	[SerializeField]
	protected TramConsole tramConsole;

	[SerializeField]
	protected string tramHornAudioKey = "tram_horn";

	[SerializeField]
	protected string tramArrivalAudioKey = "tram_stop";

	[SerializeField]
	protected TramRepairRules tramRepairRules;

	protected AdjacentRoomCulling _dungenCuller;

	protected List<ProtoActor> spawnedPlayerActorList = new List<ProtoActor>();

	protected Dictionary<int, ProtoActor> protoActorMap = new Dictionary<int, ProtoActor>();

	protected List<(FieldSkillObjectInfo fieldSkillObjectInfo, GameObject go)> fieldSkillObjects = new List<(FieldSkillObjectInfo, GameObject)>();

	protected PacketMediator packetMediator;

	protected LevelObjectHelper interactObjectHelper;

	protected ProjectileHelper projectileHelper;

	protected UIPrefab_InGame ingameui;

	protected UIPrefab_crosshair crosshairui;

	protected UIPrefab_Inventory inventoryui;

	[HideInInspector]
	public UIPrefab_Spectator spectatorui;

	protected UIPrefab_GameStatus gameStatusUI;

	protected UIPrefab_PlayerEnterInfo playerEnterInfoUI;

	protected UIPrefab_GameTips gametipsui;

	protected ProtoActor? _myAvatar;

	protected NetSyncGameData netSyncGameData = new NetSyncGameData();

	private List<ITimeSyncable> timeSyncables = new List<ITimeSyncable>();

	protected bool goBackToMainMenuFlag;

	protected CancellationToken destroyToken;

	protected bool EnteringCompleteAll;

	protected Queue<string> EnteringCutSceneNameQueue = new Queue<string>();

	protected bool EnterExitingProcess;

	protected Queue<string> ExitingCutSceneNameQueue = new Queue<string>();

	[HideInInspector]
	public Dictionary<long, ulong> playerSteamIDs = new Dictionary<long, ulong>();

	private readonly Dictionary<string, bool> playingCutScenes = new Dictionary<string, bool>();

	private const float PACKET_TIMEOUT = 3f;

	public bool IsGameLogicRunning;

	private Dictionary<string, string> steamIDToNameCache = new Dictionary<string, string>();

	private List<Func<IEnumerator>> endingSequenceList = new List<Func<IEnumerator>>();

	private Dictionary<(Transform, string), (IEnumerator, GameObject)> fieldSkillSignEffect = new Dictionary<(Transform, string), (IEnumerator, GameObject)>();

	private Dictionary<(ProtoActor, long), FieldSkillSignEffectPlayData> fieldSkillSignEffectAtProtoActor = new Dictionary<(ProtoActor, long), FieldSkillSignEffectPlayData>();

	public DLHelper DlHelper = new DLHelper();

	private List<(string, Action<string>)> cheatList = new List<(string, Action<string>)>();

	private bool toggleVisibleDebugBTStateInfoText;

	[Header("SFX")]
	[SerializeField]
	private SkyAndWeatherSystem.eWeatherPreset defaultWeatherPreset;

	[SerializeField]
	private WeatherSfxTable weatherSfxTable;

	private int _currentWeatherIndex = -1;

	private PlaySoundResult? _oldWeatherSfxResult;

	private PlaySoundResult? _newWeatherSfxResult;

	[SerializeField]
	private GameObject? soundHitPrefab;

	public AdjacentRoomCulling dungenCuller
	{
		get
		{
			if (_dungenCuller == null)
			{
				_dungenCuller = Camera.main.GetComponent<AdjacentRoomCulling>();
			}
			return _dungenCuller;
		}
	}

	protected CameraManager cameraman => Hub.s.cameraman;

	protected NetworkManagerV2 netman2 => Hub.s.netman2;

	protected UIManager uiman => Hub.s.uiman;

	protected Hub.PersistentData pdata => Hub.s.pdata;

	protected GameConfig gameConfig => Hub.s.gameConfig;

	protected DebugConsole console => Hub.s.console;

	public int CurrentCurrency => netSyncGameData.currency;

	public int TargetCurrency => netSyncGameData.targetCurrency;

	public TimeSpan CurrentTime => netSyncGameData.currentTime;

	public int GetPlayersCount()
	{
		int num = 0;
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.ActorType == ActorType.Player)
			{
				num++;
			}
		}
		return num;
	}

	public int GetRealPrice(int itemMasterId, int instantPrice)
	{
		if (netSyncGameData.boostedItem != null && netSyncGameData.boostedItem.MasterID == itemMasterId)
		{
			return (int)((double)instantPrice * Math.Round(netSyncGameData.boostedRatio, 1));
		}
		return instantPrice;
	}

	public List<ProtoActor> GetPlayersInRange(Vector3 originPosition, float distanceMeters)
	{
		List<ProtoActor> list = new List<ProtoActor>();
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.ActorType == ActorType.Player && !value.dead && (originPosition - value.transform.position).sqrMagnitude <= distanceMeters * distanceMeters)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<ProtoActor> GetMonstersInRange(int monsterMasterId, Vector3 originPosition, float distanceMeters)
	{
		List<ProtoActor> list = new List<ProtoActor>();
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.ActorType == ActorType.Monster && value.monsterMasterID == monsterMasterId && (originPosition - value.transform.position).sqrMagnitude <= distanceMeters * distanceMeters)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<ProtoActor> GetAllPlayers()
	{
		List<ProtoActor> list = new List<ProtoActor>();
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.ActorType == ActorType.Player)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<Vector3> GetTeleporterPositions(bool isIndoor)
	{
		if (isIndoor)
		{
			return indoorTeleporterPositions;
		}
		return outdoorTeleporterPositions;
	}

	protected void CollectTeleporterPositions()
	{
		foreach (KeyValuePair<string, MapMarker_TeleportStartPoint> allTeleportStartPoint in Hub.s.dynamicDataMan.GetAllTeleportStartPoints())
		{
			if (allTeleportStartPoint.Value.gameObject != null && allTeleportStartPoint.Value.gameObject.TryGetComponent<TeleporterLevelObject>(out var component))
			{
				if (component.DestinationIsToInDoor)
				{
					outdoorTeleporterPositions.Add(allTeleportStartPoint.Value.transform.position);
				}
				else
				{
					indoorTeleporterPositions.Add(allTeleportStartPoint.Value.transform.position);
				}
			}
		}
	}

	public Transform GetBGRoot()
	{
		return BGRoot;
	}

	public void RLog(string msg, bool sendToServer = false)
	{
	}

	public Dictionary<int, ProtoActor> GetProtoActorMap()
	{
		return protoActorMap;
	}

	public ProtoActor? GetMyAvatar()
	{
		return _myAvatar;
	}

	protected virtual void Awake()
	{
		if (!(Hub.s == null))
		{
			Hub.s.uiman?.OnNewSceneAwake();
			interactObjectHelper = new LevelObjectHelper(this);
			projectileHelper = new ProjectileHelper();
			packetMediator = new PacketMediator(this);
			destroyToken = base.destroyCancellationToken;
			AutoFSR();
		}
	}

	protected virtual void OnDestroy()
	{
		if (Hub.s == null)
		{
			return;
		}
		if (ingameui != null)
		{
			UnityEngine.Object.Destroy(ingameui.gameObject);
			ingameui = null;
		}
		if (crosshairui != null)
		{
			UnityEngine.Object.Destroy(crosshairui.gameObject);
			crosshairui = null;
		}
		if (inventoryui != null)
		{
			UnityEngine.Object.Destroy(inventoryui.gameObject);
			inventoryui = null;
		}
		if (gameStatusUI != null)
		{
			UnityEngine.Object.Destroy(gameStatusUI.gameObject);
			gameStatusUI = null;
		}
		if (playerEnterInfoUI != null)
		{
			UnityEngine.Object.Destroy(playerEnterInfoUI.gameObject);
			playerEnterInfoUI = null;
		}
		HideSpectatorHUD();
		foreach (var fieldSkillObject in fieldSkillObjects)
		{
			if (fieldSkillObject.go != null)
			{
				UnityEngine.Object.Destroy(fieldSkillObject.go);
			}
		}
		fieldSkillObjects.Clear();
	}

	protected virtual void Start()
	{
		if (!(Hub.s == null))
		{
			pdata.main = this;
			Hub.s.gameSettingManager.LoadLiftGammaGainFromCurrentScene();
			Hub.s.dLAcademyManager.Reset();
			EnteringCompleteAll = false;
			EnterExitingProcess = false;
			CreateAllSoundGroupCreators();
		}
	}

	protected void StartSceneLoading(string loadingSceneName)
	{
		if (Hub.s.uiman.ui_sceneloading != null)
		{
			Hub.s.uiman.ui_sceneloading.SetLoadingScene(loadingSceneName);
			Hub.s.uiman.ui_sceneloading.Show();
		}
	}

	public void EndSceneLoading()
	{
		if (Hub.s.uiman.ui_sceneloading != null)
		{
			Hub.s.uiman.ui_sceneloading.Hide();
		}
	}

	public IEnumerator WaitForMinimumLoadingTime()
	{
		yield return Hub.s.uiman.ui_sceneloading.WaitForMinimumLoadingTime();
	}

	protected virtual void Update()
	{
		if (!(Hub.s == null))
		{
			packetMediator.PumpPackets();
			UpdateSpectatorPlayerList();
			UpdateForDebug();
		}
	}

	protected virtual void UpdateForDebug()
	{
	}

	private void SpawnFieldSkill(FieldSkillObjectInfo fieldSkillObjectInfo)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Hub.s.tableman.actor.Get("FieldSkillActor"));
		if (gameObject != null)
		{
			FieldSkillActor component = gameObject.GetComponent<FieldSkillActor>();
			if (component != null)
			{
				float t = component.Spawn(fieldSkillObjectInfo);
				fieldSkillObjects.Add((fieldSkillObjectInfo, gameObject));
				UnityEngine.Object.Destroy(gameObject.gameObject, t);
				packetMediator.AutoRegisterHandlers(component, component.ActorID);
			}
		}
	}

	private bool TryDespawnFieldSkill(int actorId)
	{
		(FieldSkillObjectInfo, GameObject) tuple = fieldSkillObjects.Find(((FieldSkillObjectInfo fieldSkillObjectInfo, GameObject go) f) => f.fieldSkillObjectInfo.actorID == actorId);
		var (fieldSkillObjectInfo, gameObject) = tuple;
		if (fieldSkillObjectInfo != null || gameObject != null)
		{
			if (fieldSkillObjects.Remove(tuple))
			{
				return true;
			}
			Logger.RError($"{tuple.Item1.actorID} fieldskilldata is not found");
		}
		return false;
	}

	public void SendPacketWithCallback<T>(IMsg packet, OnClientDispatchEventHandler<T> callback, CancellationToken cancellationToken, int timeoutms = 60000, bool disconnectWhenTimeout = false) where T : IResMsg, new()
	{
		netman2.SendWithCallback(packet, callback, cancellationToken, timeoutms, disconnectWhenTimeout);
	}

	public void SendPacket(IMsg packet)
	{
		netman2.SendNoCallback(packet);
	}

	public void AddTimeSyncable(ITimeSyncable timeSyncable)
	{
		timeSyncables.Add(timeSyncable);
	}

	public void RemoveTimeSyncable(ITimeSyncable timeSyncable)
	{
		timeSyncables.Remove(timeSyncable);
	}

	public List<Hurtbox>? GetAllHurtboxes()
	{
		return (from actor in protoActorMap.Values
			where actor != null && actor.Hurtbox != null
			select actor.Hurtbox).ToList();
	}

	public virtual void OnPlayerDeath(ProtoActor actor, in ProtoActor.ActorDeathInfo actorDeathInfo)
	{
		if (actor.AmIAvatar())
		{
			if (crosshairui != null)
			{
				crosshairui.Hide();
			}
			if (inventoryui != null)
			{
				inventoryui.Hide();
			}
			if (ingameui != null)
			{
				ingameui.SetVisibleStaminaGauge(visible: false);
				ingameui.SetVisibleKillCount(visible: false);
			}
			IndoorOutdoorDetector indoorOutdoorDetector = actor.GetIndoorOutdoorDetector();
			if (indoorOutdoorDetector != null)
			{
				indoorOutdoorDetector.ApplyAudioSettings(isIndoor: false);
			}
			Hub.s.voiceman.SetVoiceMode(VoiceMode.Observer);
			Hub.s.inputman.SetCapturing(on: false);
			StartCoroutine(CorDying(actor, actorDeathInfo.ReasonOfDeath, actorDeathInfo.AttackerActorID, actorDeathInfo.LinkedMasterID));
		}
		else
		{
			cameraman.OnPlayerDeath(actor);
		}
	}

	private string[] GetSpectatorKeyBindList(InputAction action)
	{
		List<string> list = Hub.s.inputman.GetKeyBind(action).Split(',').ToList();
		List<string> list2 = new List<string>();
		foreach (string item2 in list)
		{
			string item;
			if (item2.StartsWith("m_"))
			{
				string text = item2.Substring(item2.IndexOf('_') + 1);
				item = ((!text.StartsWith("r")) ? ((!text.StartsWith("l")) ? "UNKNOWN" : "LMB") : "RMB");
			}
			else
			{
				item = item2.Substring(item2.IndexOf('_') + 1).ToUpper();
			}
			list2.Add(item);
		}
		return list2.ToArray();
	}

	private void UpdateSpectatorPlayerList()
	{
		if (spectatorui == null || !spectatorui.isActiveAndEnabled || spawnedPlayerActorList.Count == 0)
		{
			return;
		}
		bool flag = false;
		List<Tuple<int, bool, bool>> list = new List<Tuple<int, bool, bool>>();
		for (int i = 0; i < spawnedPlayerActorList.Count; i++)
		{
			ProtoActor protoActor = spawnedPlayerActorList[i];
			if (!(protoActor == null))
			{
				int actorID = protoActor.ActorID;
				bool dead = protoActor.dead;
				flag = ((!ReplaySharedData.IsReplayPlayMode) ? Hub.s.voiceman.IsPlayerSpeaking(actorID) : protoActor.IsVoicePlaying());
				list.Add(new Tuple<int, bool, bool>(actorID, dead, flag));
			}
		}
		spectatorui.UpdatePlayerListView(list, destroyToken);
	}

	protected virtual void OnSetupSpectatorCamera(ProtoActor actor)
	{
		UpdateSpectatorHUD(actor);
	}

	public string ResolveNickName(string steamID, string nameToUseIfNoSteamID)
	{
		string text = nameToUseIfNoSteamID;
		if (steamIDToNameCache.ContainsKey(steamID))
		{
			text = steamIDToNameCache[steamID];
		}
		else if (!string.IsNullOrEmpty(steamID) && steamID != "0")
		{
			text = SteamFriends.GetFriendPersonaName(new CSteamID(ulong.Parse(steamID)));
			steamIDToNameCache.Add(steamID, text);
		}
		return text;
	}

	public string ResolveNickName(ProtoActor actor, string nameToUseIfNoSteamID)
	{
		string steamID = ResolveSteamID(actor.UID);
		return ResolveNickName(steamID, nameToUseIfNoSteamID);
	}

	public string ResolveSteamID(long playerUID)
	{
		ProtoActor myAvatar = GetMyAvatar();
		if (myAvatar != null && playerUID == myAvatar.UID)
		{
			return pdata.GetUserSteamIDString();
		}
		if (pdata.actorUIDToSteamID.TryGetValue(playerUID, out var value))
		{
			return value.ToString();
		}
		Logger.RWarn($"SteamID 누락: UID={playerUID}");
		return "0";
	}

	protected void CreateSpectatorHUD()
	{
		if (spectatorui == null)
		{
			spectatorui = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_Spectator>(uiprefab_spectator);
			spectatorui.Hide();
			string[] spectatorKeyBindList = GetSpectatorKeyBindList(InputAction.PreviousSpectatorTarget);
			string[] spectatorKeyBindList2 = GetSpectatorKeyBindList(InputAction.NextSpectatorTarget);
			spectatorui.SetKeyDescText(spectatorKeyBindList, spectatorKeyBindList2);
			UIManager uIManager = Hub.s.uiman;
			uIManager.OnSetupSpectatorCamera = (UnityAction<ProtoActor>)Delegate.Combine(uIManager.OnSetupSpectatorCamera, new UnityAction<ProtoActor>(OnSetupSpectatorCamera));
			CameraManager cameraManager = Hub.s.cameraman;
			cameraManager.OnElapsedTimeCheckForChangeSpectatorTarget = (UnityAction<float, float>)Delegate.Combine(cameraManager.OnElapsedTimeCheckForChangeSpectatorTarget, new UnityAction<float, float>(OnElapsedTimeCheckForChangeSpectatorTarget));
		}
	}

	public void OnElapsedTimeCheckForChangeSpectatorTarget(float elapsedTime, float delaySeconds)
	{
		if (!(spectatorui == null))
		{
			if (elapsedTime == 0f)
			{
				spectatorui.ShowNextChangeTime();
			}
			else if (elapsedTime < 0f)
			{
				spectatorui.HideNextChangeTime();
				return;
			}
			spectatorui.UpdateNextChangeTime(elapsedTime, delaySeconds);
		}
	}

	public void ShowSpectatorHUD()
	{
		if (!(this == null))
		{
			if (spectatorui != null)
			{
				spectatorui.Show();
			}
			if (inventoryui != null)
			{
				inventoryui.Show();
			}
			if (ingameui != null)
			{
				ingameui.Show();
			}
			UpdateSpectatorHUD();
		}
	}

	private void UpdateSpectatorHUD(ProtoActor actor)
	{
		if (!(actor == null) && !(spectatorui == null))
		{
			if (!spectatorui.CheckActiveSpectatedPlayerName())
			{
				spectatorui.SetActiveSpectatedPlayerName(isActive: true);
			}
			string spectatedPlayerName = ResolveNickName(actor, actor.netSyncActorData.actorName);
			spectatorui.SetSpectatedPlayerName(spectatedPlayerName);
			OnHpChanged(actor, actor.netSyncActorData.hp, actor.netSyncActorData.maxHP);
			OnContaChanged(actor, actor.netSyncActorData.conta, actor.netSyncActorData.maxConta);
			OnStaminaChanged(actor, actor.netSyncActorData.stamina, actor.netSyncActorData.maxStamina);
			UpdateInventoryUI(actor);
		}
	}

	private void UpdateSpectatorHUD()
	{
		if (!(Hub.s == null) && !(Hub.s.cameraman == null) && Hub.s.cameraman.TryGetCurrentSpectatorTarget(out ProtoActor target))
		{
			UpdateSpectatorHUD(target);
		}
	}

	public void HideSpectatorHUD()
	{
		if (!(Hub.s == null) && !(Hub.s.uiman == null) && !(this == null))
		{
			if (spectatorui != null)
			{
				spectatorui.Hide();
			}
			if (inventoryui != null)
			{
				inventoryui.Hide();
			}
			if (ingameui != null)
			{
				ingameui.Hide();
			}
			UIManager uIManager = Hub.s.uiman;
			uIManager.OnSetupSpectatorCamera = (UnityAction<ProtoActor>)Delegate.Remove(uIManager.OnSetupSpectatorCamera, new UnityAction<ProtoActor>(OnSetupSpectatorCamera));
			CameraManager cameraManager = Hub.s.cameraman;
			cameraManager.OnElapsedTimeCheckForChangeSpectatorTarget = (UnityAction<float, float>)Delegate.Remove(cameraManager.OnElapsedTimeCheckForChangeSpectatorTarget, new UnityAction<float, float>(OnElapsedTimeCheckForChangeSpectatorTarget));
		}
	}

	private IEnumerator CorDying(ProtoActor actor, ReasonOfDeath reasonOfDeath, int attackerActorID, int skillMasterID)
	{
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
		if (skillInfo != null && !string.IsNullOrWhiteSpace(skillInfo.DeadCameraSocketName))
		{
			Transform transform = SocketNodeMarker.FindFirstInHierarchy(actor.transform, skillInfo.DeadCameraSocketName);
			if (transform != null && transform.TryGetComponent<CinemachineCamera>(out var component))
			{
				ProtoActor actorByActorID = GetActorByActorID(attackerActorID);
				if (actorByActorID != null)
				{
					component.LookAt = SocketNodeMarker.FindFirstInHierarchy(actorByActorID.transform, skillInfo.DeadCameraLookAt);
				}
				float deadCameraBlendTime = skillInfo.DeadCameraBlendTime;
				Hub.s.cameraman.BlendTo(component, float.PositiveInfinity, deadCameraBlendTime, 0f);
			}
		}
		else
		{
			bool flag = reasonOfDeath == ReasonOfDeath.Conta;
			string deadCameraSocketName = gameConfig.playerActor.deadCameraSocketName;
			Transform transform2 = SocketNodeMarker.FindFirstInHierarchy(actor.transform, deadCameraSocketName);
			if (transform2 != null && transform2.TryGetComponent<CinemachineCamera>(out var component2))
			{
				component2.LookAt = (flag ? actor.transform : null);
				float deadCameraBlendTime2 = gameConfig.playerActor.deadCameraBlendTime;
				Hub.s.cameraman.BlendTo(component2, float.PositiveInfinity, deadCameraBlendTime2, 0f);
			}
			if (flag)
			{
				float deadByContaFadeOutTime = gameConfig.playerActor.deadByContaFadeOutTime;
				float deadByContaFadeInterval = gameConfig.playerActor.deadByContaFadeInterval;
				float deadByContaFadeInTime = gameConfig.playerActor.deadByContaFadeInTime;
				StartCoroutine(CorPseudoBlink(deadByContaFadeOutTime, deadByContaFadeInterval, deadByContaFadeInTime));
				OnContaChanged(actor, Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue, Hub.s.dataman.ExcelDataManager.Consts.C_MaxContaValue);
			}
		}
		if (reasonOfDeath != ReasonOfDeath.Conta)
		{
			actor.SwitchPersonViewMode(PersonViewMode.Third);
		}
		float deadCameraDuration = gameConfig.playerActor.deadCameraDuration;
		ingameui.isDead = true;
		ingameui.OnHpChanged(1L, 1L);
		Hub.s.audioman.PlaySfx("death_beep", base.gameObject.transform);
		yield return new WaitForSeconds(deadCameraDuration);
		if (!IsPlayerAllDead())
		{
			yield return new WaitForSeconds(4f);
		}
		if (actor.AmIAvatar())
		{
			Hub.s.cameraman.CancelBlendTo();
			OnContaChanged(actor, 0L, 0L);
		}
		if (IsPlayerAllDead())
		{
			yield break;
		}
		HideCommonUI();
		ingameui.isDead = false;
		ingameui.OnHpChanged(11L, 10L);
		if (actor.AmIAvatar())
		{
			cameraman.OnPlayerDeath(actor);
			if (reasonOfDeath == ReasonOfDeath.Conta)
			{
				actor.SwitchPersonViewMode(PersonViewMode.Third);
			}
			ShowSpectatorHUD();
			actor.ClearEffectPlayer();
		}
	}

	private IEnumerator CorPseudoBlink(float fadeOutDuration, float fadeWaitInterval, float fadeInDuration)
	{
		Hub.s.uiman.FadeOut(Color.black, fadeOutDuration);
		yield return new WaitForSeconds(fadeWaitInterval);
		Hub.s.uiman.FadeIn(Color.black, fadeInDuration);
	}

	public virtual void OnPlayerSpawn(ProtoActor actor)
	{
		if (actor.AmIAvatar())
		{
			if (crosshairui != null)
			{
				crosshairui.Show();
			}
			if (inventoryui != null)
			{
				inventoryui.Show();
			}
		}
		if (gameStatusUI != null)
		{
			gameStatusUI.AddMember(actor.netSyncActorData.actorName);
		}
		spawnedPlayerActorList.Add(actor);
	}

	public virtual void OnPlayerDespawn(ProtoActor actor)
	{
		gameStatusUI.RemoveMember(actor.netSyncActorData.actorName);
		spawnedPlayerActorList.Remove(actor);
	}

	public virtual void OnPlayerDespawn(int actorID)
	{
		if (!(Hub.s == null) && !(Hub.s.cameraman == null))
		{
			bool isGameSessionEnded = IsGameSessionEnd();
			Hub.s.cameraman.OnPlayerDespawn(actorID, isGameSessionEnded);
			ProtoActor actorByActorID = GetActorByActorID(actorID);
			if (!(actorByActorID == null))
			{
				OnPlayerDespawn(actorByActorID);
			}
		}
	}

	public void UpdateInventoryUI(ProtoActor actor)
	{
		if (Hub.s == null || Hub.s.cameraman == null || actor == null)
		{
			return;
		}
		if (Hub.s.cameraman.IsSpectatorMode || Hub.s.replayManager.IsReplayPlayMode)
		{
			if (!Hub.s.cameraman.IsCurrentSpectatorTarget(actor))
			{
				return;
			}
		}
		else if (!actor.AmIAvatar())
		{
			return;
		}
		inventoryui.UpdateSlot(actor.GetInventoryItems(), actor.GetSelectedInventorySlotIndex());
	}

	public bool IsPlayerAllDead()
	{
		return protoActorMap.Values.FirstOrDefault((ProtoActor actor) => actor.ActorType == ActorType.Player && !actor.dead) == null;
	}

	public bool IsAllPlayerInTram()
	{
		List<ProtoActor> allPlayers = GetAllPlayers();
		List<bool> list = new List<bool>(allPlayers.Select((ProtoActor p) => false));
		List<(MapTrigger, Bounds)> inTramVolume = Hub.s.dynamicDataMan.GetInTramVolume();
		int num = 0;
		foreach (var item2 in inTramVolume)
		{
			if (item2.Item1.usageType != MapTrigger.eUsageType.ClientOnly_InsideTramVolume)
			{
				continue;
			}
			for (int num2 = 0; num2 < allPlayers.Count; num2++)
			{
				if (!allPlayers[num2].dead)
				{
					num++;
					Bounds item = item2.Item2;
					if (item.Contains(allPlayers[num2].gameObject.transform.position))
					{
						list[num2] = true;
					}
				}
			}
		}
		return list.Count((bool f) => f) == num;
	}

	public void DestroyAllActors()
	{
		foreach (int item in protoActorMap.Keys.ToList())
		{
			if (protoActorMap.TryGetValue(item, out ProtoActor value))
			{
				if (value != null)
				{
					Hub.s.residualObject.PreserveAllInChildren(value);
					UnityEngine.Object.Destroy(value.gameObject);
				}
				protoActorMap.Remove(item);
			}
		}
	}

	public void HideAllLootingObjects()
	{
		foreach (LevelObject item in interactObjectHelper.CollectLootingObjects())
		{
			if (item != null)
			{
				item.gameObject.SetActive(value: false);
			}
		}
	}

	public ProtoActor? GetActorByPlayerUID(long playerUID)
	{
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.UID == playerUID)
			{
				return value;
			}
		}
		return null;
	}

	public ProtoActor? GetActorByActorID(int actorID)
	{
		if (protoActorMap.TryGetValue(actorID, out ProtoActor value))
		{
			return value;
		}
		return null;
	}

	private bool CanUpdateInGameUI(ProtoActor actor)
	{
		if (Hub.s == null || Hub.s.cameraman == null || ingameui == null)
		{
			return false;
		}
		if (Hub.s.cameraman.IsSpectatorMode || Hub.s.replayManager.IsReplayPlayMode)
		{
			if (!Hub.s.cameraman.IsCurrentSpectatorTarget(actor))
			{
				return false;
			}
		}
		else if (!actor.AmIAvatar())
		{
			return false;
		}
		return true;
	}

	public virtual void OnHpChanged(ProtoActor actor, long currVal, long maxVal)
	{
		if (CanUpdateInGameUI(actor))
		{
			ingameui.OnHpChanged(currVal, maxVal);
		}
	}

	public virtual void OnContaChanged(ProtoActor actor, long currVal, long maxVal)
	{
		if (CanUpdateInGameUI(actor))
		{
			ingameui.OnContaChanged(currVal, maxVal);
		}
	}

	public virtual void OnStaminaChanged(ProtoActor actor, long currVal, long maxVal)
	{
		if (CanUpdateInGameUI(actor))
		{
			ingameui.OnStaminaChanged(currVal, maxVal);
		}
	}

	public void OnKillCountChanged(ProtoActor actor, int killCount)
	{
		if (CanUpdateInGameUI(actor))
		{
			ingameui.OnKillCountChanged(killCount);
		}
	}

	public void OnSaving(ProtoActor actor, bool auto)
	{
		if (CanUpdateInGameUI(actor))
		{
			ingameui.OnSaving(auto);
		}
	}

	public void UpdateCurrency(int currentCurrency)
	{
		if (currentCurrency < 0)
		{
			Logger.RError($"UpdateCurrency: currentCurrency={currentCurrency}");
			return;
		}
		int currency = netSyncGameData.currency;
		netSyncGameData.currency = currentCurrency;
		OnCurrencyChanged(currency, currentCurrency);
	}

	protected virtual void OnCurrencyChanged(int prev, int curr)
	{
		if (ingameui != null)
		{
			ingameui.OnCurrencyChanged(curr);
		}
	}

	protected virtual void OnTimeChanged(TimeSpan currentTime)
	{
	}

	protected virtual void OnWeatherSync(int currentWeatherMasterID, int nextWeatherMasterID)
	{
	}

	protected bool CheckNetworkConnection()
	{
		if (netman2.State == NetworkManagerState.Disconnected)
		{
			OnNetDisconnected(netman2.LastDisconnectReason);
			return true;
		}
		return false;
	}

	private (bool, bool, string, string) CheckDisconnectedReason(MsgErrorCode errorCode, DisconnectReason reason)
	{
		switch (errorCode)
		{
		case MsgErrorCode.PlayerCountExceeded:
			return (false, true, "INVITE_DO_NOT_ENTER_TITLE", "INVITE_DO_NOT_ENTER_ROOM_FULL");
		case MsgErrorCode.ClientErrorSendFailed:
		case MsgErrorCode.ClientErrorSendTimeout:
			return (false, true, "STRING_CLIENT_FAIL_SEND_PACKET_TITLE", "STRING_CLIENT_FAIL_SEND_PACKET_DESCRIPTION");
		default:
			return reason switch
			{
				DisconnectReason.ByClient => (false, false, "", ""), 
				DisconnectReason.ConnectionError => (false, true, "STRING_HOST_NETWORK_ERROR_TITLE", "STRING_HOST_NETWORK_ERROR"), 
				_ => (true, true, "", ""), 
			};
		}
	}

	public virtual void OnNetDisconnected(DisconnectReason reason, string errorMessage = "")
	{
		HideCommonUI();
		Hub.s.inputman.SetCapturing(on: false);
		Hub.s.pdata.main.EndSceneLoading();
		Hub.s.uiman.HideGameTips();
		Logger.RLog($"disconnected reason: {reason}, lastResponseError: {pdata.lastResponseError}");
		bool flag = true;
		if (reason == DisconnectReason.ByClient)
		{
			string text = "";
			string text2 = "";
			bool flag2 = false;
			(flag, flag2, text, text2) = CheckDisconnectedReason(pdata.lastResponseError, reason);
			if (flag2)
			{
				Hub.s.tableman.uiprefabs.ShowDialog("NetworkError", delegate
				{
					GoBackToMainMenu();
				}, null, text, text2);
			}
			else
			{
				GoBackToMainMenu();
			}
		}
		else if (reason == DisconnectReason.ByServer)
		{
			flag = false;
			Hub.s.tableman.uiprefabs.ShowDialog("Disconnected", delegate
			{
				GoBackToMainMenu();
			}, null, "", "");
		}
		else if (reason == DisconnectReason.ConnectionError)
		{
			string text3 = "";
			string text4 = "";
			bool flag3 = false;
			(flag, flag3, text3, text4) = CheckDisconnectedReason(pdata.lastResponseError, reason);
			if (flag3)
			{
				Hub.s.tableman.uiprefabs.ShowDialog("NetworkError", delegate
				{
					GoBackToMainMenu();
				}, null, text3, text4);
			}
			else
			{
				GoBackToMainMenu();
			}
		}
		else if (reason == DisconnectReason.None && pdata.lastResponseError == MsgErrorCode.Success)
		{
			flag = false;
			Hub.s.tableman.uiprefabs.ShowDialog("Disconnected", delegate
			{
				GoBackToMainMenu();
			}, null, "", "");
		}
		else
		{
			string text5 = "";
			string text6 = "";
			bool flag4 = false;
			(flag, flag4, text5, text6) = CheckDisconnectedReason(pdata.lastResponseError, reason);
			if (flag4)
			{
				Hub.s.tableman.uiprefabs.ShowDialog("Disconnected", delegate
				{
					GoBackToMainMenu();
				}, null, text5, text6);
			}
			else
			{
				GoBackToMainMenu();
			}
		}
		if (flag)
		{
			throw new MimicException("Disconnected: " + reason.ToString() + ", lastResponseError: " + pdata.lastResponseError);
		}
	}

	protected virtual void OnStashChanged()
	{
		if (tramConsole != null)
		{
			int stashToCurrency = GetValueInCollectingVolume() + GetValueInStash();
			int targetCurrency = TargetCurrency;
			tramConsole.UpdateCollectedCurrency(stashToCurrency, targetCurrency);
		}
	}

	public bool TryPerformInteraction()
	{
		return interactObjectHelper.TryPerformInteraction();
	}

	public bool TryPerformInteractionEnd()
	{
		return interactObjectHelper.TryPerformInteractionEnd();
	}

	protected void AddProtoActor(ProtoActor protoActor)
	{
		protoActorMap[protoActor.ActorID] = protoActor;
		packetMediator.AutoRegisterHandlers(protoActor, protoActor.ActorID);
		if (Hub.s.replayManager.IsReplayPlayMode && protoActor.IsPlayer())
		{
			Hub.s.replayManager.OnPlayerSpawn(protoActor);
		}
	}

	public virtual Transform GetActorSpawnRootTransform(in Vector3 spawnPos = default(Vector3))
	{
		return GetBGRoot();
	}

	public virtual void OnActorTeleported(ProtoActor actor, Vector3 pos)
	{
		dungenCuller.OnTeleported();
	}

	public bool ResolveLevelLoadCompleteRes(LevelLoadCompleteRes res)
	{
		TimeSpan currentTime = res.currentTime;
		netSyncGameData.currentTime = currentTime;
		foreach (ITimeSyncable timeSyncable in timeSyncables)
		{
			timeSyncable.OnTimeSync(currentTime);
		}
		OnTimeChanged(currentTime);
		netSyncGameData.targetCurrency = res.targetCurrency;
		PlayerInfo selfInfo = res.selfInfo;
		pdata.CycleCount = res.sessionCount;
		pdata.DayCount = res.dayCount;
		pdata.TramUpgradeIDs = res.tramUpgradeList.Clone();
		pdata.TramUpgradeIDs = pdata.TramUpgradeIDs.Distinct().ToList();
		Logger.RLog($"ResolveLevelLoadCompleteRes: sessionCount = {res.sessionCount}, dayCount = {res.dayCount}");
		netSyncGameData.boostedItem = Hub.s.dataman.ExcelDataManager.GetItemInfo(res.boostedItem.Item1);
		netSyncGameData.boostedRatio = res.boostedItem.Item2;
		netSyncGameData.stashesOnHanger.Clear();
		foreach (KeyValuePair<int, ItemInfo> stash in res.stashes)
		{
			ItemInfo itemInfo = stash.Value.Clone();
			netSyncGameData.stashesOnHanger[stash.Key] = itemInfo;
			interactObjectHelper.OnHangItem(stash.Key, itemInfo);
		}
		OnStashChanged();
		if (_myAvatar == null)
		{
			Logger.RError("ResolveLevelLoadCompleteRes _myAvatar is null");
			return false;
		}
		_myAvatar.InvalidateMoveSyncTarget();
		_myAvatar.SetPositionAndRotationForce(selfInfo.position.toVector3(), Quaternion.Euler(0f, selfInfo.position.yaw, 0f));
		_myAvatar.SetAsMyAvatar(selfInfo, res.firstEnterMap);
		AddProtoActor(_myAvatar);
		return true;
	}

	protected IEnumerator SpawnMyAvatar()
	{
		Vector3 spawnPos = Vector3.zero;
		GameObject gameObject = UnityEngine.Object.Instantiate(Hub.s.tableman.actor.Get("ProtoActor"), parent: GetActorSpawnRootTransform(in spawnPos), position: spawnPos, rotation: Quaternion.Euler(0f, 0f, 0f));
		if (gameObject == null)
		{
			Logger.RError("instantiate Avatar failed (1)");
			yield break;
		}
		ProtoActor component = gameObject.GetComponent<ProtoActor>();
		if (component == null)
		{
			Logger.RError("instantiate Avatar failed (2)");
			yield break;
		}
		component.InstantiatePuppetForMyAvatar();
		component.SetInputDisableReason(ProtoActor.EInputDisableReason.GameNotStarted);
		_myAvatar = component;
	}

	protected void SetEnableInputForMyAvatar()
	{
		if (_myAvatar != null)
		{
			_myAvatar.ClearInputDisableReason(ProtoActor.EInputDisableReason.GameNotStarted);
		}
	}

	protected IEnumerator TryLevelLoad()
	{
		Logger.RLog("LevelLoad");
		LevelLoadCompleteRes levelLoadCompleteRes = null;
		SendPacketWithCallback(new LevelLoadCompleteReq(), delegate(LevelLoadCompleteRes _res)
		{
			if (_res == null)
			{
				levelLoadCompleteRes = null;
				pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
				Logger.RError("LevelLoadCompleteRes is null");
				goBackToMainMenuFlag = true;
			}
			else if (_res.errorCode != MsgErrorCode.Success)
			{
				pdata.lastResponseError = _res.errorCode;
				Logger.RError($"LevelLoadCompleteRes.errorCode : {_res.errorCode}");
				goBackToMainMenuFlag = true;
			}
			else
			{
				levelLoadCompleteRes = _res;
				ResolveLevelLoadCompleteRes(_res);
			}
		}, destroyToken, 60000, disconnectWhenTimeout: true);
		yield return new WaitUntil(() => goBackToMainMenuFlag || levelLoadCompleteRes != null);
		if (goBackToMainMenuFlag)
		{
			levelLoadCompleteRes = null;
			OnNetDisconnected(DisconnectReason.ConnectionError, "Failed to load level");
		}
	}

	protected void GoBackToMainMenu(bool needToDisconnect = false)
	{
		StartCoroutine(CorGoBackToMainMenu(needToDisconnect));
	}

	protected IEnumerator CorGoBackToMainMenu(bool needToDisconnect)
	{
		if (Hub.s.uiman.ui_sceneloading != null)
		{
			Hub.s.uiman.ui_sceneloading.Hide();
		}
		pdata.serverRoomState = Hub.PersistentData.eServerRoomState.Nowhere;
		if (needToDisconnect)
		{
			netman2.Disconnect(DisconnectReason.ByClient);
		}
		pdata.lastResponseError = MsgErrorCode.Success;
		pdata.SessionJoined = false;
		pdata.completeMakingRoomSig = null;
		Hub.s.steamInviteDispatcher.LeaveLobby();
		Logger.RLog("GoBackToMainMenu");
		yield return new WaitUntil(() => netman2.State == NetworkManagerState.Disconnected);
		Hub.s.voiceman.Shutdown();
		Hub.s.DestroyVWorld();
		Hub.LoadScene("MainMenuScene");
	}

	private int GetValueInCollectingVolume()
	{
		int num = 0;
		List<(MapTrigger, Bounds)> collectingVolumes = Hub.s.dynamicDataMan.GetCollectingVolumes();
		IEnumerable<LootingLevelObject> enumerable = from l in interactObjectHelper.CollectLootingObjects()
			select l as LootingLevelObject;
		foreach (var item2 in collectingVolumes)
		{
			if (item2.Item1.usageType != MapTrigger.eUsageType.Server_CollectingVolume)
			{
				continue;
			}
			foreach (LootingLevelObject item3 in enumerable)
			{
				Bounds item = item2.Item2;
				if (item.Contains(item3.gameObject.transform.position) && Hub.s.dataman.ExcelDataManager.GetItemInfo(item3.itemMasterID) != null)
				{
					int realPrice = GetRealPrice(item3.itemMasterID, item3.marketPrice);
					if (realPrice > 0)
					{
						num += realPrice;
					}
				}
			}
		}
		return num;
	}

	private int GetValueInStash()
	{
		int num = 0;
		foreach (ItemInfo value in netSyncGameData.stashesOnHanger.Values)
		{
			num += value.price;
		}
		return num;
	}

	protected void InitCommonUI(bool isMaintenanceScene = false)
	{
		if (ingameui == null)
		{
			ingameui = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_InGame>(prefab_ui_ingame);
		}
		if (crosshairui == null)
		{
			crosshairui = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_crosshair>(prefab_ui_crosshair);
		}
		if (inventoryui == null)
		{
			inventoryui = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_Inventory>(prefab_ui_inventory);
		}
		if (gameStatusUI == null)
		{
			gameStatusUI = uiman.InstatiateUIPrefab<UIPrefab_GameStatus>(prefab_ui_GameStatus, eUIHeight.Top);
		}
		if (playerEnterInfoUI == null && prefab_ui_playerEnterInfo != null)
		{
			playerEnterInfoUI = uiman.InstatiateUIPrefab<UIPrefab_PlayerEnterInfo>(prefab_ui_playerEnterInfo, eUIHeight.Top);
		}
		if (gametipsui == null)
		{
			gametipsui = uiman.gametipsui ?? uiman.InstatiateUIPrefab<UIPrefab_GameTips>(Hub.s.uiman.prefab_ui_gametips);
		}
		if (tramConsole != null)
		{
			tramConsole.InitCommonUI(isMaintenanceScene);
		}
		if (playerEnterInfoUI != null && !playerEnterInfoUI.isActiveAndEnabled)
		{
			playerEnterInfoUI.Show();
		}
		HideCommonUI();
	}

	protected void InitCommonUIValue()
	{
		if (ingameui != null)
		{
			ingameui.Show();
		}
		if (playerEnterInfoUI != null)
		{
			playerEnterInfoUI.Show();
		}
		ProtoActor myAvatar = GetMyAvatar();
		if (myAvatar != null)
		{
			OnHpChanged(myAvatar, myAvatar.netSyncActorData.hp, myAvatar.netSyncActorData.maxHP);
			OnContaChanged(myAvatar, myAvatar.netSyncActorData.conta, myAvatar.netSyncActorData.maxConta);
			OnStaminaChanged(myAvatar, myAvatar.netSyncActorData.stamina, myAvatar.netSyncActorData.maxStamina);
			OnCurrencyChanged(netSyncGameData.currency, netSyncGameData.currency);
			OnStashChanged();
		}
	}

	public void HideCommonUI()
	{
		if (ingameui != null)
		{
			ingameui.Hide();
		}
		if (crosshairui != null)
		{
			crosshairui.Hide();
		}
		if (inventoryui != null)
		{
			inventoryui.Hide();
		}
		if (gameStatusUI != null)
		{
			gameStatusUI.Hide();
		}
		if (playerEnterInfoUI != null)
		{
			playerEnterInfoUI.Hide();
		}
		if (uiman.gametipsui != null)
		{
			uiman.gametipsui.Hide();
		}
		uiman.HideIngameMenu();
	}

	public void ShowCommonUI()
	{
		if (ingameui != null)
		{
			ingameui.Show();
		}
		if (inventoryui != null)
		{
			inventoryui.Show();
		}
		if (playerEnterInfoUI != null)
		{
			playerEnterInfoUI.Show();
		}
		if (uiman.gametipsui != null)
		{
			uiman.gametipsui.Show();
		}
		ProtoActor myAvatar = GetMyAvatar();
		if (myAvatar != null)
		{
			OnHpChanged(myAvatar, myAvatar.netSyncActorData.hp, myAvatar.netSyncActorData.maxHP);
			OnContaChanged(myAvatar, myAvatar.netSyncActorData.conta, myAvatar.netSyncActorData.maxConta);
			OnStaminaChanged(myAvatar, myAvatar.netSyncActorData.stamina, myAvatar.netSyncActorData.maxStamina);
			OnCurrencyChanged(netSyncGameData.currency, netSyncGameData.currency);
			OnStashChanged();
		}
	}

	public void EnableKillCountUI(bool enable)
	{
		if (ingameui != null)
		{
			ingameui.SetVisibleKillCount(enable);
			ingameui.OnKillCountChanged(0);
		}
	}

	public void ToggleGameStatusUI()
	{
		if (gameStatusUI != null)
		{
			if (!gameStatusUI.isActiveAndEnabled)
			{
				gameStatusUI.Show();
				Hub.s.inputman.SetCapturing(on: false);
			}
			else
			{
				gameStatusUI.Hide();
				Hub.s.inputman.SetCapturing(on: true);
			}
		}
	}

	public void ShowGameStatusUI()
	{
		if (gameStatusUI != null && !gameStatusUI.isActiveAndEnabled)
		{
			gameStatusUI.Show();
		}
	}

	public void HideGameStatusUI()
	{
		if (gameStatusUI != null && gameStatusUI.isActiveAndEnabled)
		{
			gameStatusUI.Hide();
		}
	}

	public void SetGameStatusUI()
	{
		DissonanceComms dissonanceComms = DissonanceFishNetComms.Instance.Comms;
		TMP_Dropdown selectMicDropdown = gameStatusUI.UE_Dropdown_selectMic.GetComponent<TMP_Dropdown>();
		List<string> list = new List<string>();
		dissonanceComms.GetMicrophoneDevices(list);
		if (list.Count == 0)
		{
			list.Add("No microphone");
		}
		else
		{
			selectMicDropdown.onValueChanged.AddListener(delegate(int value)
			{
				string text = selectMicDropdown.options[value].text;
				dissonanceComms.MicrophoneName = text;
			});
			selectMicDropdown.ClearOptions();
			foreach (string item in list)
			{
				_ = item;
				selectMicDropdown.AddOptions(list);
			}
		}
		gameStatusUI.OnOK_button = delegate
		{
			gameStatusUI.Hide();
			Hub.s.inputman.SetCapturing(on: true);
		};
		gameStatusUI.OnCancel_button = delegate
		{
			netman2.Disconnect(DisconnectReason.ByClient);
		};
		gameStatusUI.UE_quotaAmount.SetText(netSyncGameData.targetCurrency.ToString());
	}

	public void SetMimicSelectedColor(bool useDraw)
	{
		int num = 20000001;
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.ActorType == ActorType.Monster && value.monsterMasterID == num)
			{
				value.UseMimicSelectedColor(useDraw);
			}
		}
	}

	private void UpdatePlayerInfo(List<PlayerStatusInfo> playerStatusInfos)
	{
		foreach (PlayerStatusInfo playerStatusInfo in playerStatusInfos)
		{
			if (protoActorMap.TryGetValue(playerStatusInfo.actorID, out ProtoActor value))
			{
				value.UpdateStatus(playerStatusInfo);
			}
		}
	}

	public virtual bool IsGameSessionEnd()
	{
		return false;
	}

	public void AddEndingSequence(Func<IEnumerator> coroutineFunc)
	{
		endingSequenceList.Add(coroutineFunc);
	}

	public void RemoveEndingSequence(Func<IEnumerator> coroutineFunc)
	{
		endingSequenceList.Remove(coroutineFunc);
	}

	public IEnumerator StartEndingSequence()
	{
		foreach (Func<IEnumerator> endingSequence in endingSequenceList)
		{
			yield return StartCoroutine(endingSequence());
		}
	}

	public virtual void OnEndDungeonSig(EndDungeonSig sig)
	{
	}

	public virtual void OnLeaveRoomSig(LeaveRoomSig sig)
	{
	}

	public virtual void OnCompleteGameSessionSig(CompleteGameSessionSig sig)
	{
	}

	public void PlayFieldSkillSignEffectAtProtoActor(ProtoActor actor, long itemId, long itemMasterId, float elapsedSec)
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo((int)itemMasterId);
		if (itemInfo != null && itemInfo.SpawnFieldSkillWaitEffectName.Length > 0)
		{
			string spawnFieldSkillWaitEffectName = itemInfo.SpawnFieldSkillWaitEffectName;
			string spawnFieldSkillWaitEffectSocket = itemInfo.SpawnFieldSkillWaitEffectSocket;
			float num = (float)itemInfo.SpawnFieldSkillWaitEffectDurationMSec / 1000f;
			Transform socketRoot = actor.transform;
			bool isHandheldItem = false;
			InventoryItem handheldItem = actor.GetHandheldItem();
			if (handheldItem != null && handheldItem.ItemID == itemId)
			{
				socketRoot = handheldItem.Transform;
				isHandheldItem = true;
			}
			FieldSkillSignEffectPlayData fieldSkillSignEffectPlayData = new FieldSkillSignEffectPlayData(actor, Hub.s.timeutil.GetCurrentTickSec(), itemId, isHandheldItem, itemMasterId);
			fieldSkillSignEffectAtProtoActor[(actor, itemId)] = fieldSkillSignEffectPlayData;
			StartCoroutine(PlayEffect(fieldSkillSignEffectPlayData, socketRoot, spawnFieldSkillWaitEffectSocket, spawnFieldSkillWaitEffectName, num - elapsedSec));
		}
	}

	public void CheckFieldSkillSignEffectAtProtoActor(ProtoActor actor, long prevItemId, long prevItemMasterId, long currentHandheldItemId, long currentHandheldItemMasterId, bool prevItemIsInInventory)
	{
		for (int i = 0; i < fieldSkillSignEffectAtProtoActor.Count; i++)
		{
			if (fieldSkillSignEffectAtProtoActor.ElementAt(i).Key.Item1 == actor)
			{
				FieldSkillSignEffectPlayData value = fieldSkillSignEffectAtProtoActor.ElementAt(i).Value;
				if (value.isHandheldItem && value.itemId == prevItemId && !prevItemIsInInventory)
				{
					value.isCancelRequested = true;
				}
				if (!value.isHandheldItem && value.itemId == currentHandheldItemId)
				{
					value.isCancelRequested = true;
					long itemId = value.itemId;
					long itemMasterId = value.itemMasterId;
					fieldSkillSignEffectAtProtoActor.Remove((actor, itemId));
					PlayFieldSkillSignEffectAtProtoActor(actor, itemId, itemMasterId, (float)Hub.s.timeutil.GetCurrentTickSec() - value.startTimeSec);
				}
				if (value.isHandheldItem && value.itemId == prevItemId && prevItemIsInInventory)
				{
					value.isCancelRequested = true;
					long itemId2 = value.itemId;
					long itemMasterId2 = value.itemMasterId;
					fieldSkillSignEffectAtProtoActor.Remove((actor, itemId2));
					PlayFieldSkillSignEffectAtProtoActor(actor, itemId2, itemMasterId2, (float)Hub.s.timeutil.GetCurrentTickSec() - value.startTimeSec);
				}
			}
		}
	}

	public void CancelFieldSkillSignEffectAtProtoActor(ProtoActor actor, long itemId)
	{
		if (fieldSkillSignEffectAtProtoActor.TryGetValue((actor, itemId), out FieldSkillSignEffectPlayData value) && value.itemId == itemId)
		{
			value.isCancelRequested = true;
			fieldSkillSignEffectAtProtoActor.Remove((actor, itemId));
		}
	}

	private IEnumerator PlayEffect(FieldSkillSignEffectPlayData playData, Transform socketRoot, string socketName, string particleName, float durationSec)
	{
		Transform transform = SocketNodeMarker.FindFirstInHierarchy(socketRoot, socketName);
		if (transform != null)
		{
			GameObject particle = Hub.s.vfxman.InstantiateVfx(particleName, transform);
			if (particle != null)
			{
				particle.SetActive(value: true);
				while ((float)Hub.s.timeutil.GetCurrentTickSec() - playData.startTimeSec < durationSec)
				{
					if (playData.isCancelRequested)
					{
						particle.SetActive(value: false);
						UnityEngine.Object.Destroy(particle, 0.1f);
						yield break;
					}
					yield return null;
				}
				if (particle != null)
				{
					particle.SetActive(value: false);
					UnityEngine.Object.Destroy(particle, 0.5f);
				}
			}
		}
		fieldSkillSignEffectAtProtoActor.Remove((playData.actor, playData.itemId));
	}

	public void PlayFieldSkillSignEffect(Transform item, string socketName, string particleName, float duration)
	{
		Transform transform = SocketNodeMarker.FindFirstInHierarchy(item, socketName);
		if (transform != null)
		{
			GameObject gameObject = Hub.s.vfxman.InstantiateVfx(particleName, transform);
			if (gameObject != null)
			{
				IEnumerator enumerator = PlayEffect(gameObject, duration);
				fieldSkillSignEffect[(item, particleName)] = (enumerator, gameObject);
				StartCoroutine(enumerator);
			}
		}
	}

	private IEnumerator PlayEffect(GameObject particle, float duration)
	{
		if (particle != null)
		{
			particle.SetActive(value: true);
			yield return new WaitForSeconds(duration);
			if (particle != null)
			{
				particle.SetActive(value: false);
				UnityEngine.Object.Destroy(particle, 0.5f);
			}
		}
	}

	public void StopFieldSkillSignEffect(Transform item, string particleName)
	{
		if (fieldSkillSignEffect.TryGetValue((item, particleName), out (IEnumerator, GameObject) value))
		{
			StopCoroutine(value.Item1);
			if (value.Item2 != null)
			{
				UnityEngine.Object.Destroy(value.Item2, 0.5f);
			}
			fieldSkillSignEffect.Remove((item, particleName));
		}
	}

	protected IEnumerator TryPlayEnteringCutScene()
	{
		if (EnteringCutSceneNameQueue.Count <= 0)
		{
			yield break;
		}
		while (EnteringCutSceneNameQueue.Count > 0)
		{
			string text = EnteringCutSceneNameQueue.Dequeue().Trim();
			if (text.Length > 0)
			{
				yield return CorPlayCutScene(text);
			}
		}
		EnteringCutSceneNameQueue.Clear();
		EndSceneLoading();
	}

	protected IEnumerator TryPlayExitingCutScene()
	{
		int playedCutSceneCount = 0;
		while (ExitingCutSceneNameQueue.Count > 0)
		{
			string text = ExitingCutSceneNameQueue.Peek().Trim();
			if (text.Length > 0)
			{
				yield return CorPlayCutScene(text);
				int num = playedCutSceneCount + 1;
				playedCutSceneCount = num;
			}
			ExitingCutSceneNameQueue.Dequeue();
			if (playedCutSceneCount == 1)
			{
				StartCoroutine(StartEndingSequence());
			}
		}
		ExitingCutSceneNameQueue.Clear();
	}

	[PacketHandler(false)]
	protected void OnPacket(AllMemberEnterRoomSig sig)
	{
		if (EnteringCompleteAll)
		{
			return;
		}
		Logger.RLog("AllMemberEnterRoomSig received in " + pdata.main.name);
		EnteringCompleteAll = true;
		if (sig.enterCutsceneNames.Count > 0)
		{
			sig.enterCutsceneNames.ForEach(delegate(string cutsceneName)
			{
				EnteringCutSceneNameQueue.Enqueue(cutsceneName);
			});
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(BeginEndRoomCutSceneSig sig)
	{
		Logger.RLog("BeginEndRoomCutSceneSig received in " + pdata.main.name);
		if (sig.exitCutsceneNames.Count > 0)
		{
			sig.exitCutsceneNames.ForEach(delegate(string cutsceneName)
			{
				ExitingCutSceneNameQueue.Enqueue(cutsceneName.Trim());
			});
		}
		EnterExitingProcess = true;
	}

	[PacketHandler(false)]
	protected void OnPacket(HeartBeatRes res)
	{
		int networkGrade = (int)((double)(Hub.s.timeutil.GetCurrentTickMilliSec() - res.clientSendTime) * 0.5);
		Hub.s.netman2.SetNetworkGrade(networkGrade);
		Hub.s.netman2.SetLastHeartBeatSeqId(res.seqID);
	}

	[PacketHandler(false)]
	protected void OnPacket(NetworkGradeSig sig)
	{
		if (!(Hub.s == null) && !(Hub.s.uiman.inGameMenu == null))
		{
			Hub.s.uiman.inGameMenu.SetPingImage(sig);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(RoomStatusSig sig)
	{
		if (!(Hub.s == null) && !(Hub.s.uiman == null))
		{
			UpdatePlayerInfo(sig.playerStatusInfos);
		}
	}

	protected IEnumerator TurnOnOffAllLight(bool on)
	{
		int delayCount = 10;
		List<BlackOut> blackoutProps = Hub.s.dynamicDataMan.GetBlackOutProps();
		int i = 0;
		while (i < blackoutProps.Count)
		{
			if (blackoutProps[i] != null)
			{
				if (on)
				{
					blackoutProps[i].OnRecover();
				}
				else
				{
					blackoutProps[i].OnBlackOut();
				}
			}
			if (i != 0 && delayCount % i == 0)
			{
				yield return new WaitForSeconds(0.2f);
			}
			int num = i + 1;
			i = num;
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(BlackoutSig sig)
	{
		if (Hub.s == null || pdata == null || pdata.main == null)
		{
			return;
		}
		if (sig.isBlackout)
		{
			StartCoroutine(TurnOnOffAllLight(on: false));
			ProtoActor myAvatar = pdata.main.GetMyAvatar();
			if (!(myAvatar != null))
			{
				return;
			}
			myAvatar.OnBlackOut();
			myAvatar.AddIncomingEvent(SpeechEvent_IncomingType.Blackout, Time.realtimeSinceStartup + myAvatar.GetIncomingEventExpireTime(SpeechEvent_IncomingType.Blackout));
			if (pdata.ClientMode == NetworkClientMode.Host)
			{
				foreach (ProtoActor value in protoActorMap.Values)
				{
					if (value != null && value.ActorType == ActorType.Monster)
					{
						value.AddIncomingEvent(SpeechEvent_IncomingType.Blackout, Time.realtimeSinceStartup + value.GetIncomingEventExpireTime(SpeechEvent_IncomingType.Blackout));
					}
				}
			}
			if (sig.reasonOfBlackout == ReasonOfBlackout.Item)
			{
				ProtoActor actorByActorID = pdata.main.GetActorByActorID(sig.ownerActorID);
				if (actorByActorID != null)
				{
					actorByActorID.OnBlackOutByItem(sig.ownerActorID);
				}
			}
		}
		else
		{
			StartCoroutine(TurnOnOffAllLight(on: true));
			pdata.main.GetMyAvatar()?.OnBlackOut(blackout: false);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(ItemSpawnFieldSkillWaitSig sig)
	{
		if (Hub.s == null || pdata == null || pdata.main == null)
		{
			return;
		}
		ProtoActor actorByActorID = pdata.main.GetActorByActorID((int)sig.actorID);
		if (actorByActorID != null)
		{
			if (sig.waitEvent)
			{
				Hub.s.pdata.main.PlayFieldSkillSignEffectAtProtoActor(actorByActorID, sig.itemID, sig.itemMasterID, 0f);
			}
			else
			{
				Hub.s.pdata.main.CancelFieldSkillSignEffectAtProtoActor(actorByActorID, sig.itemID);
			}
			actorByActorID.OnItemSpawnFieldSkillWaitSig(sig.itemID, sig.waitEvent);
		}
		else if (sig.waitEvent)
		{
			interactObjectHelper.PlaySignOfFieldSkillByItem(sig.actorID, sig.itemID, sig.itemMasterID);
		}
		else
		{
			interactObjectHelper.StopSignOfFieldSkillByItem(sig.actorID, sig.itemID, sig.itemMasterID);
		}
	}

	private void AutoFSR()
	{
		if (!(Hub.s == null) && Hub.s.pdata != null && !Hub.s.rpmman.IsCurrentResolutionBelowFHD())
		{
			StartCoroutine(AutoFSRRunner());
		}
	}

	private IEnumerator AutoFSRRunner()
	{
		if (!Hub.s.rpmman.IsURPAssetAvailable())
		{
			yield break;
		}
		float targetFrameRate = ((QualitySettings.vSyncCount != 0) ? 60f : Mathf.Min(Application.targetFrameRate, 60f));
		float oldTime = Time.timeSinceLevelLoad;
		int oldFrameCount = Time.renderedFrameCount;
		yield return new WaitForSeconds(10f);
		WaitForSeconds waiter = new WaitForSeconds(10f);
		float renderScale;
		while (true)
		{
			renderScale = 1080f / (float)Screen.height;
			float timeSinceLevelLoad = Time.timeSinceLevelLoad;
			int renderedFrameCount = Time.renderedFrameCount;
			float num = timeSinceLevelLoad - oldTime;
			int num2 = renderedFrameCount - oldFrameCount;
			oldTime = timeSinceLevelLoad;
			oldFrameCount = renderedFrameCount;
			if (num <= 0f || num2 <= 0)
			{
				yield return waiter;
				continue;
			}
			float num3 = (float)num2 / num;
			float num4 = 5f;
			if (num3 < targetFrameRate - num4)
			{
				break;
			}
			yield return waiter;
		}
		Hub.s.rpmman.SetRenderScale(renderScale);
	}

	public (bool isSkinned, GameObject spawnedItemObject) TrySpawnItemObject(int itemMasterID, Transform parentTransform)
	{
		return interactObjectHelper.TrySpawnItemObject(itemMasterID, parentTransform);
	}

	protected void BuildNavMesh()
	{
		Physics.SyncTransforms();
		Hub.s.navman.Build();
	}

	[PacketHandler(false)]
	protected void OnPacket(CutSceneCompleteSig sig)
	{
		OnCutSceneCompleteSig(sig);
	}

	[PacketHandler(false)]
	protected void OnPacket(PlayCutSceneSig sig)
	{
		if (EnteringCutSceneNameQueue.Count > 0 || ExitingCutSceneNameQueue.Count > 0)
		{
			Logger.RLog($"PlayCutSceneSig ignored due to ongoing cutscene queue. EnteringCutSceneNameQueue: {EnteringCutSceneNameQueue.Count}, ExitingCutSceneNameQueue: {ExitingCutSceneNameQueue.Count}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
		}
		else
		{
			OnPlayCutSceneSig(sig);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(PlayAnimationSig sig)
	{
		OnPlayAnimationSig(sig);
	}

	protected virtual void OnCutSceneCompleteSig(CutSceneCompleteSig sig)
	{
	}

	protected virtual void OnPlayCutSceneSig(PlayCutSceneSig sig)
	{
		string cutSceneName = sig.cutSceneName.Trim();
		Logger.RLog("PlayCutScene");
		StartCoroutine(CorPlayCutScene(cutSceneName));
	}

	protected virtual void OnPlayAnimationSig(PlayAnimationSig sig)
	{
		Logger.RLog("PlayAnimationSig");
	}

	protected IEnumerator CorPlayCutScene(string cutSceneName)
	{
		if (playingCutScenes.ContainsKey(cutSceneName) && playingCutScenes[cutSceneName])
		{
			yield break;
		}
		playingCutScenes[cutSceneName] = true;
		if (cutScenePlayer != null)
		{
			cutScenePlayer.PlayCutScene(cutSceneName);
		}
		bool inputLocked = false;
		DynamicDataManager.CutSceneData cutSceneData = Hub.s.dynamicDataMan.GetCutSceneInfos().Find((DynamicDataManager.CutSceneData c) => c.name == cutSceneName);
		if (cutSceneData != null)
		{
			if (cutSceneData.lockInput && _myAvatar != null)
			{
				_myAvatar.DontMove();
				inputLocked = true;
			}
			yield return new WaitForSeconds(cutSceneData.duration);
		}
		else
		{
			Logger.RError("CutScene " + cutSceneName + " not found in CutSceneInfos.");
		}
		if (inputLocked && _myAvatar != null)
		{
			_myAvatar.CancelDontMove();
		}
		playingCutScenes[cutSceneName] = false;
	}

	protected IEnumerator CorRefreshSteamLobbyData(Action<bool> onLobbyDataUpdated)
	{
		ulong joinedLobbyID = Hub.s.pdata.GetJoinedLobbyID();
		SteamLobbyChecker lobbyChecker = new SteamLobbyChecker(joinedLobbyID, onLobbyDataUpdated);
		lobbyChecker.RequestData();
		yield return new WaitUntil(() => lobbyChecker.IsLobbyDataUpdated);
		Hub.s.pdata.IsPublicLobby = lobbyChecker.IsPublicLobby;
		lobbyChecker.Dispose();
		lobbyChecker = null;
	}

	public List<LevelObject> CollectLevelObjects()
	{
		return interactObjectHelper.GetAllLevelObjects();
	}

	public List<FieldSkillObjectInfo> GetSprinklerFieldSkillInRange(Vector3 position, float range)
	{
		return (from f in fieldSkillObjects
			where f.fieldSkillObjectInfo.fieldSkillMasterID == 70007 && f.go != null && Vector3.Distance(f.go.transform.position, position) <= range
			select f.fieldSkillObjectInfo).ToList();
	}

	public List<FieldSkillObjectInfo> GetPaintballFieldSkillInRange(Vector3 position, float range)
	{
		return (from f in fieldSkillObjects
			where (f.fieldSkillObjectInfo.fieldSkillMasterID == 70010 || f.fieldSkillObjectInfo.fieldSkillMasterID == 70011 || f.fieldSkillObjectInfo.fieldSkillMasterID == 70012 || f.fieldSkillObjectInfo.fieldSkillMasterID == 70013 || f.fieldSkillObjectInfo.fieldSkillMasterID == 70014 || f.fieldSkillObjectInfo.fieldSkillMasterID == 70015 || f.fieldSkillObjectInfo.fieldSkillMasterID == 70016 || f.fieldSkillObjectInfo.fieldSkillMasterID == 70017 || f.fieldSkillObjectInfo.fieldSkillMasterID == 70018) && f.go != null && Vector3.Distance(f.go.transform.position, position) <= range
			select f.fieldSkillObjectInfo).ToList();
	}

	public List<FieldSkillObjectInfo> GetPaintspotFieldSkillInRange(Vector3 position, float range)
	{
		return (from f in fieldSkillObjects
			where f.fieldSkillObjectInfo.fieldSkillMasterID == 71001 && f.go != null && Vector3.Distance(f.go.transform.position, position) <= range
			select f.fieldSkillObjectInfo).ToList();
	}

	public List<FieldSkillObjectInfo> GetHeliumGasFieldSkillInRange(Vector3 position, float range)
	{
		return (from f in fieldSkillObjects
			where f.fieldSkillObjectInfo.fieldSkillMasterID == 72001 && f.go != null && Vector3.Distance(f.go.transform.position, position) <= range
			select f.fieldSkillObjectInfo).ToList();
	}

	public List<FieldSkillObjectInfo> GetLightningFieldSkillInRange(Vector3 position, float range)
	{
		return (from f in fieldSkillObjects
			where f.fieldSkillObjectInfo.fieldSkillMasterID == 73001 && f.go != null && Vector3.Distance(f.go.transform.position, position) <= range
			select f.fieldSkillObjectInfo).ToList();
	}

	public List<ProtoActor> GetAllActorsInRange(Vector3 originPosition, float distanceMeters, ActorType actorType)
	{
		List<ProtoActor> list = new List<ProtoActor>();
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.ActorType == actorType && !value.dead)
			{
				Vector3 vector = originPosition - value.transform.position;
				if (!(Mathf.Abs(vector.y) > (float)Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold) && !(vector.x * vector.x + vector.z * vector.z > distanceMeters * distanceMeters))
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public ProtoActor? GetMinDistanceActor(Vector3 originPosition, float maxDistanceMeters, ActorType actorType)
	{
		ProtoActor result = null;
		float num = float.MaxValue;
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.ActorType == actorType && !value.dead && !(Mathf.Abs((originPosition - value.transform.position).y) > (float)Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold))
			{
				PathFindResult route = Hub.s.navman.GetRoute(originPosition, value.transform.position);
				if (route.Success && !(route.Length > maxDistanceMeters) && route.Length < num)
				{
					num = route.Length;
					result = value;
				}
			}
		}
		return result;
	}

	[Conditional("DEV")]
	public void debug_LogLoadingProfiler(string msg, bool resetTimer = false)
	{
		if (resetTimer)
		{
			pdata._debug_timeStampForProfile = Time.realtimeSinceStartup;
		}
		float num = Time.realtimeSinceStartup - pdata._debug_timeStampForProfile;
		pdata._debug_timeStampForProfile = Time.realtimeSinceStartup;
		Logger.RLog($"[LoadingImpact] {msg} {num:F2}", sendToLogServer: false, useConsoleOut: true, "loading");
	}

	[Conditional("DEV")]
	public virtual void UpdateInputKeyForDebug()
	{
		if (Hub.GetKeyboard().f2Key.wasPressedThisFrame)
		{
			toggleVisibleDebugBTStateInfoText = !toggleVisibleDebugBTStateInfoText;
		}
	}

	[Conditional("DEV")]
	public void SetVisibleDebugBTStateInfoText(bool visible)
	{
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (!(value == null) && value.ActorType == ActorType.Monster)
			{
				if (visible)
				{
					value.ShowDebugBTStateInfoText();
				}
				else
				{
					value.HideDebugBTStateInfoText();
				}
			}
		}
	}

	[Conditional("DEV")]
	protected void AddCheat(string key, Action<string> func)
	{
		if (!(Hub.s == null) && !(Hub.s.console == null) && !cheatList.Exists(((string, Action<string>) c) => c.Item1 == key))
		{
			cheatList.Add((key, func));
		}
	}

	[Conditional("DEV")]
	private void PurgeAllCheats()
	{
		if (Hub.s == null || Hub.s.console == null)
		{
			return;
		}
		foreach (var cheat in cheatList)
		{
			_ = cheat;
		}
		cheatList.Clear();
	}

	public void TryDestroyItem(Transform? puppetTransform, Transform itemTransform, int itemMasterId)
	{
		interactObjectHelper.SliceItemWhenDestoryed(puppetTransform, itemTransform, itemMasterId);
	}

	private bool IsActorLifeCycleDead(in VCreatureLifeCycle actorLifeCycle)
	{
		if (actorLifeCycle != VCreatureLifeCycle.Dead && actorLifeCycle != VCreatureLifeCycle.Dying)
		{
			return actorLifeCycle == VCreatureLifeCycle.ForceDying;
		}
		return true;
	}

	[PacketHandler(false)]
	protected void OnPacket(CompleteMakingRoomSig packet)
	{
		Logger.RLog("CompleteMakingRoom in " + pdata.main.name);
		pdata.completeMakingRoomSig = packet;
	}

	[PacketHandler(false)]
	protected void OnPacket(EnableWaitingRoomSig sig)
	{
		pdata.enableWaitingRoomSig = sig;
	}

	[PacketHandler(false)]
	protected void OnPacket(EnableDeathMatchRoomSig sig)
	{
		Logger.RLog("EnableDeathMatchRoom in " + pdata.main.name);
		pdata.enableDeathMatchRoomSig = sig;
	}

	[PacketHandler(false)]
	protected void OnPacket(EndDungeonSig sig)
	{
		Logger.RLog("EndDungeon in " + pdata.main.name);
		OnEndDungeonSig(sig);
	}

	[PacketHandler(false)]
	protected void OnPacket(LeaveRoomSig sig)
	{
		Logger.RLog($"id : {sig.actorID} LeaveRoom in {pdata.main.name}");
		OnLeaveRoomSig(sig);
	}

	[PacketHandler(false)]
	protected void OnPacket(CompleteGameSessionSig sig)
	{
		Logger.RLog("CompleteGameSession in " + pdata.main.name);
		OnCompleteGameSessionSig(sig);
	}

	[PacketHandler(false)]
	protected void OnPacket(JoinServerSig sig)
	{
		Logger.RLog($"id : {sig.playerUID}, nickName : {sig.nickName} JoinServer in {pdata.main.name}");
		if (playerEnterInfoUI != null)
		{
			string userName = ResolveNickName(sig.steamID.ToString(), sig.nickName);
			playerEnterInfoUI.AddPlayerInfo(userName, isEntering: true);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(LeaveServerSig sig)
	{
		Logger.RLog("nickName : " + sig.nickName + " LeaveServer in " + pdata.main.name);
		if (playerEnterInfoUI != null)
		{
			string userName = ResolveNickName(sig.steamID.ToString(), sig.nickName);
			playerEnterInfoUI.AddPlayerInfo(userName, isEntering: false);
		}
	}

	[PacketHandler(false)]
	protected void OnChangeLevelObjectStateSig(UseLevelObjectSig packet)
	{
		if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main != null)
		{
			Hub.s.pdata.main.interactObjectHelper.OnChangeLevelObjectStateSig(packet);
		}
	}

	[PacketHandler(false)]
	protected void OnSightInSig(SightInSig sightInSig)
	{
		Hub.s.pdata.main.playerSteamIDs.Clear();
		if (sightInSig.sightReason != SightReason.Spawned)
		{
			return;
		}
		foreach (PlayerInfo playerInfo in sightInSig.playerInfos)
		{
			if (Hub.s.pdata.actorUIDToSteamID.ContainsKey(playerInfo.UID))
			{
				Hub.s.pdata.actorUIDToSteamID.Remove(playerInfo.UID);
				Hub.s.pdata.actorUIDToSteamID.Add(playerInfo.UID, playerInfo.steamID);
				Logger.RLog($"actorUIDToSteamID updated: {playerInfo.UID} -> {playerInfo.steamID}");
			}
			else
			{
				Hub.s.pdata.actorUIDToSteamID.Add(playerInfo.UID, playerInfo.steamID);
				Logger.RLog($"actorUIDToSteamID added: {playerInfo.UID} -> {playerInfo.steamID}");
			}
			if (Hub.s.uiman.inGameMenu != null)
			{
				Hub.s.uiman.inGameMenu.SetRemoteVolumeControllerCoroutineStart();
			}
			if (!protoActorMap.ContainsKey(playerInfo.actorID))
			{
				Vector3 spawnPos = playerInfo.position.toVector3();
				GameObject gameObject = UnityEngine.Object.Instantiate(Hub.s.tableman.actor.Get("ProtoActor"), parent: GetActorSpawnRootTransform(in spawnPos), position: spawnPos, rotation: Quaternion.Euler(0f, playerInfo.position.yaw, 0f));
				if (gameObject == null)
				{
					Logger.RError("instantiate playerActor failed (1)");
					return;
				}
				ProtoActor component = gameObject.GetComponent<ProtoActor>();
				if (component == null)
				{
					Logger.RError("instantiate playerActor failed (2)");
					return;
				}
				component.SetAsOtherPlayer(playerInfo);
				AddProtoActor(component);
				if (IsActorLifeCycleDead(playerInfo.actorLifeCycle))
				{
					component.OnActorDeathOnReconnect();
				}
			}
			else
			{
				Logger.RLog($"[sightInSig] pass spawn : {playerInfo.actorID} {playerInfo.actorName} {playerInfo.UID} {playerInfo.steamID}");
			}
		}
		foreach (OtherCreatureInfo monsterInfo in sightInSig.monsterInfos)
		{
			if (!protoActorMap.ContainsKey(monsterInfo.actorID))
			{
				Vector3 spawnPos2 = monsterInfo.position.toVector3();
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Hub.s.tableman.actor.Get("ProtoActor"), parent: GetActorSpawnRootTransform(in spawnPos2), position: spawnPos2, rotation: Quaternion.Euler(0f, monsterInfo.position.yaw, 0f));
				if (gameObject2 == null)
				{
					Logger.RError("instantiate playerActor failed (3)");
					return;
				}
				ProtoActor component2 = gameObject2.GetComponent<ProtoActor>();
				if (component2 == null)
				{
					Logger.RError("instantiate playerActor failed (4)");
					return;
				}
				component2.Teleport(monsterInfo.position.toVector3(), new Vector3(0f, monsterInfo.position.yaw, 0f));
				component2.SetAsMonster(monsterInfo);
				AddProtoActor(component2);
				if (IsActorLifeCycleDead(monsterInfo.actorLifeCycle))
				{
					component2.OnActorDeathOnReconnect();
				}
			}
		}
		foreach (PlayerInfo playerInfo2 in sightInSig.playerInfos)
		{
			if (protoActorMap.TryGetValue(playerInfo2.actorID, out ProtoActor value))
			{
				value.Attached_ResolveOtherInfo(playerInfo2);
				value.Aura_ResolveOtherInfo(playerInfo2);
			}
		}
		foreach (OtherCreatureInfo monsterInfo2 in sightInSig.monsterInfos)
		{
			if (protoActorMap.TryGetValue(monsterInfo2.actorID, out ProtoActor value2))
			{
				value2.Attached_ResolveOtherInfo(monsterInfo2);
				value2.Aura_ResolveOtherInfo(monsterInfo2);
			}
		}
		foreach (LootingObjectInfo lootingObjectInfo in sightInSig.lootingObjectInfos)
		{
			interactObjectHelper.TrySpawnLootingObject(lootingObjectInfo);
		}
		foreach (ProjectileObjectInfo projectileObjectInfo in sightInSig.projectileObjectInfos)
		{
			Vector3 position = projectileObjectInfo.position.toVector3();
			Quaternion rotation = Quaternion.Euler(projectileObjectInfo.position.pitch, projectileObjectInfo.position.yaw, projectileObjectInfo.position.roll);
			ProjectileActor projectileActor = projectileHelper.Spawn(projectileObjectInfo.masterID, projectileObjectInfo.actorID, position, rotation);
			if (projectileActor != null)
			{
				packetMediator.AutoRegisterHandlers(projectileActor, projectileActor.ActorID);
			}
		}
		foreach (FieldSkillObjectInfo fieldSkillObjectInfo in sightInSig.fieldSkillObjectInfos)
		{
			SpawnFieldSkill(fieldSkillObjectInfo);
		}
	}

	[PacketHandler(false)]
	protected void OnSightOutSig(SightOutSig sightOutSig)
	{
		if (sightOutSig.sightReason != SightReason.Despawned)
		{
			return;
		}
		foreach (int actorID in sightOutSig.actorIDs)
		{
			if (protoActorMap.TryGetValue(actorID, out ProtoActor value))
			{
				if (value != null && !value.RemainAfterDeath)
				{
					UnityEngine.Object.Destroy(value.gameObject);
				}
				protoActorMap.Remove(actorID);
			}
			else
			{
				TryDespawnFieldSkill(actorID);
				interactObjectHelper.TryDespawnLootingObject(actorID);
				projectileHelper.TryDespawn(actorID);
			}
			Hub.s.RemoveHitCheckVisualizations(actorID);
		}
	}

	[PacketHandler(false)]
	protected void OnTeleportSig(TeleportSig sig)
	{
		if (protoActorMap.TryGetValue(sig.actorID, out ProtoActor value) && value != null)
		{
			value.OnTeleportSig(sig);
		}
		else
		{
			interactObjectHelper.TryTeleportLootingObject(sig);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(TimeSyncSig sig)
	{
		TimeSpan currentTime = sig.currentTime;
		if (currentTime.Hours % 3 == 0)
		{
			Logger.RLog($"TimeSyncSig : currentTime = {currentTime.Hours}");
		}
		TimeSpan currentTime2 = sig.currentTime;
		netSyncGameData.currentTime = currentTime2;
		foreach (ITimeSyncable timeSyncable in timeSyncables)
		{
			timeSyncable.OnTimeSync(currentTime2);
		}
		OnTimeChanged(currentTime2);
		OnWeatherSync(sig.currentWeatherMasterID, sig.forecastWeatherMasterID);
	}

	[PacketHandler(false)]
	protected void OnPacket(SyncImmutableStatSig syncImmutableStatSig)
	{
		if (_myAvatar != null)
		{
			_myAvatar.ResolveImmutableStats(syncImmutableStatSig.ImmutableStats);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(PlaySoundSig sig)
	{
	}

	[PacketHandler(false)]
	protected void OnPacket(DestroyActorSig sig)
	{
		interactObjectHelper.TryDestroyLootingItem(sig.actorID);
	}

	[PacketHandler(false)]
	protected void OnPacket(FieldHitTargetSig sig)
	{
		SkillSequenceInfo skillSequenceInfo = Hub.s.dataman.ExcelDataManager.GetSkillSequenceInfo(sig.skillSequenceMasterID);
		if (skillSequenceInfo == null || skillSequenceInfo.AbnormalMasterIDs.Count() != 0 || skillSequenceInfo.SequenceType == SkillSeqType.Abnormal)
		{
			return;
		}
		foreach (TargetHitInfo targetHitInfo in sig.targetHitInfos)
		{
			ProtoActor actorByActorID = GetActorByActorID(targetHitInfo.targetID);
			if (!(actorByActorID != null))
			{
				continue;
			}
			if (skillSequenceInfo.SkillTagetEffectId != 0)
			{
				actorByActorID.PlaySkillHitEffect(skillSequenceInfo.SkillTagetEffectId);
			}
			if (targetHitInfo.actionAbnormalHitType != CCType.None)
			{
				actorByActorID.ApplyImmobilize(targetHitInfo);
			}
			if (skillSequenceInfo.RagDollForceDirection != Vector3.zero)
			{
				(FieldSkillObjectInfo, GameObject) tuple = fieldSkillObjects.Find(((FieldSkillObjectInfo fieldSkillObjectInfo, GameObject go) f) => f.fieldSkillObjectInfo.actorID == sig.fieldSkillObjectID);
				var (fieldSkillObjectInfo, gameObject) = tuple;
				if ((fieldSkillObjectInfo != null || gameObject != null) && tuple.Item2 != null)
				{
					actorByActorID.LastDamagedForceDirection = Quaternion.Euler(tuple.Item2.transform.position - actorByActorID.transform.position) * skillSequenceInfo.RagDollForceDirection;
				}
			}
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(ProjectileHitTargetSig sig)
	{
		ProjectileInfo projectileInfo = Hub.s.dataman.ExcelDataManager.GetProjectileInfo(sig.projectileMasterID);
		if (projectileInfo == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < projectileInfo.AbnormalMasterIDs.Length; i++)
		{
			int num = projectileInfo.AbnormalMasterIDs[i];
			if (num > 0)
			{
				AbnormalInfo abnormalInfo = Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(num);
				if (abnormalInfo != null && abnormalInfo.SkillTargetEffectId != 0)
				{
					flag = true;
					break;
				}
			}
		}
		foreach (TargetHitInfo targetHitInfo in sig.targetHitInfos)
		{
			ProtoActor actorByActorID = GetActorByActorID(targetHitInfo.targetID);
			if (!(actorByActorID != null))
			{
				continue;
			}
			if (projectileInfo.ProjectileCollisionEffectName != string.Empty)
			{
				GameObject gameObject = Hub.s.vfxman.InstantiateVfx(projectileInfo.ProjectileCollisionEffectName, actorByActorID.transform);
				if (gameObject != null)
				{
					gameObject.transform.position = targetHitInfo.hitPosition.toVector3();
					gameObject.transform.rotation = targetHitInfo.hitPosition.toRotation();
					IEnumerator routine = PlayEffect(gameObject, 10f);
					StartCoroutine(routine);
				}
			}
			if (!flag)
			{
				actorByActorID.PlaySkillHitEffect(projectileInfo.LinkSkillTargetEffectDataId);
			}
		}
		if (sig.targetHitInfos.Count == 0 && projectileInfo.ProjectileCollisionEffectName != string.Empty)
		{
			GameObject gameObject2 = Hub.s.vfxman.InstantiateVfx(projectileInfo.ProjectileCollisionEffectName, GetBGRoot());
			if (gameObject2 != null)
			{
				gameObject2.transform.position = sig.hitPos;
				IEnumerator routine2 = PlayEffect(gameObject2, 10f);
				StartCoroutine(routine2);
			}
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(BuyItemSig sig)
	{
		interactObjectHelper.OnBuyItemSig(sig);
	}

	[PacketHandler(false)]
	protected void OnPacket(GetRemainScrapValueSig sig)
	{
		if (Hub.s.pdata.main as GamePlayScene != null)
		{
			interactObjectHelper.OnScrapScanning(sig.remainValue);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(RollDungeonSig sig)
	{
		if (Hub.s.pdata.main as InTramWaitingScene != null)
		{
			interactObjectHelper.UpdateDungeonCandidates(sig.newDungeonMasterIDs.Item1, sig.newDungeonMasterIDs.Item2);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(StashStatusSig sig)
	{
		List<int> list = new List<int>();
		foreach (int key in netSyncGameData.stashesOnHanger.Keys)
		{
			if (!sig.stashedItems.ContainsKey(key))
			{
				list.Add(key);
			}
		}
		foreach (int item in list)
		{
			netSyncGameData.stashesOnHanger.Remove(item);
			interactObjectHelper.OnUnhangItem(item);
		}
		foreach (KeyValuePair<int, ItemInfo> stashedItem in sig.stashedItems)
		{
			if (netSyncGameData.stashesOnHanger.TryGetValue(stashedItem.Key, out ItemInfo value))
			{
				if (value != stashedItem.Value)
				{
					netSyncGameData.stashesOnHanger[stashedItem.Key] = stashedItem.Value;
				}
			}
			else
			{
				netSyncGameData.stashesOnHanger[stashedItem.Key] = stashedItem.Value;
				interactObjectHelper.OnHangItem(stashedItem.Key, stashedItem.Value);
			}
		}
		OnStashChanged();
	}

	protected void CreateAllSoundGroupCreators()
	{
		if (!(weatherSfxTable == null))
		{
			if (weatherSfxTable.weatherSoundGroupCreator != null)
			{
				UnityEngine.Object.Instantiate(weatherSfxTable.weatherSoundGroupCreator);
			}
			if (weatherSfxTable.outdoorBgmSoundGroupCreator != null)
			{
				UnityEngine.Object.Instantiate(weatherSfxTable.outdoorBgmSoundGroupCreator);
			}
			if (!string.IsNullOrEmpty(weatherSfxTable.outdoorBgmSfxId) && this as GamePlayScene != null)
			{
				Hub.s.audioman.PlaySfx(weatherSfxTable.outdoorBgmSfxId);
			}
			if (soundHitPrefab != null)
			{
				UnityEngine.Object.Instantiate(soundHitPrefab);
			}
		}
	}

	protected void PlayDefaultWeatherSfx()
	{
		PlayWeatherSfx(defaultWeatherPreset);
	}

	protected void PlayWeatherSfx(SkyAndWeatherSystem.eWeatherPreset weatherPreset)
	{
		if (weatherSfxTable == null || _currentWeatherIndex == (int)weatherPreset)
		{
			return;
		}
		if (_oldWeatherSfxResult != null)
		{
			if (_oldWeatherSfxResult.ActingVariation != null)
			{
				_oldWeatherSfxResult.ActingVariation.FadeOutNowAndStop();
			}
			_oldWeatherSfxResult = null;
		}
		if (weatherSfxTable.TryGetSfxId(weatherPreset, out string sfxId))
		{
			_newWeatherSfxResult = Hub.s.audioman.PlaySfxAtPosition(sfxId, Vector3.zero);
			_oldWeatherSfxResult = _newWeatherSfxResult;
		}
		_currentWeatherIndex = (int)weatherPreset;
	}

	protected void PlayDeathMatchSfx()
	{
		if (_oldWeatherSfxResult != null)
		{
			if (_oldWeatherSfxResult.ActingVariation != null)
			{
				_oldWeatherSfxResult.ActingVariation.FadeOutNowAndStop();
			}
			_oldWeatherSfxResult = null;
		}
		_newWeatherSfxResult = Hub.s.audioman.PlaySfxAtPosition("Sound_Env_Weather.Sound_Env_Weather_ThunderStorm", Vector3.zero);
		_oldWeatherSfxResult = _newWeatherSfxResult;
	}

	protected void MuteWeatherSfx()
	{
		if (!(weatherSfxTable == null))
		{
			string weatherBusName = weatherSfxTable.weatherBusName;
			if (!string.IsNullOrWhiteSpace(weatherBusName))
			{
				Hub.s.audioman.MuteBus(weatherBusName);
			}
			if (!string.IsNullOrWhiteSpace(weatherSfxTable.outdoorBgmBusName))
			{
				Hub.s.audioman.MuteBus(weatherSfxTable.outdoorBgmBusName);
			}
		}
	}

	protected void UnmuteWeatherSfx()
	{
		if (!(weatherSfxTable == null))
		{
			string weatherBusName = weatherSfxTable.weatherBusName;
			if (!string.IsNullOrWhiteSpace(weatherBusName))
			{
				Hub.s.audioman.UnmuteBus(weatherBusName);
			}
			if (!string.IsNullOrWhiteSpace(weatherSfxTable.outdoorBgmBusName))
			{
				Hub.s.audioman.UnmuteBus(weatherSfxTable.outdoorBgmBusName);
			}
		}
	}
}
