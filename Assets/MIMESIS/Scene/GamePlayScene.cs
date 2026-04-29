using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Cooked;
using DunGen;
using Mimic.Actors;
using ModUtility;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.CrashReportHandler;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GamePlayScene : GameMainBase
{
	private struct CachedRenderableComponents
	{
		public List<Behaviour> BehaviorList;

		public List<Renderer> RendererList;

		public List<DecalProjector> DecalList;

		public CachedRenderableComponents(List<Behaviour> behaviorList, List<Renderer> rendererList, List<DecalProjector> decalList)
		{
			BehaviorList = new List<Behaviour>(behaviorList);
			RendererList = new List<Renderer>(rendererList);
			DecalList = new List<DecalProjector>(decalList);
		}

		public void SetEnabledRenderableComponent(bool enable)
		{
			SetEnabledBehaviorComponent(enable);
			SetEnabledRendererComponent(enable);
			SetEnabledDecalComponent(enable);
		}

		public void SetEnabledBehaviorComponent(bool enable)
		{
			if (BehaviorList == null)
			{
				return;
			}
			foreach (Behaviour behavior in BehaviorList)
			{
				if (behavior != null)
				{
					behavior.enabled = enable;
				}
			}
		}

		public void SetEnabledRendererComponent(bool enable)
		{
			if (RendererList == null)
			{
				return;
			}
			foreach (Renderer renderer in RendererList)
			{
				if (renderer != null)
				{
					renderer.enabled = enable;
				}
			}
		}

		public void SetEnabledDecalComponent(bool enable)
		{
			if (DecalList == null)
			{
				return;
			}
			foreach (DecalProjector decal in DecalList)
			{
				if (decal != null)
				{
					decal.enabled = enable;
				}
			}
		}
	}

	[Serializable]
	public class RandomPlacerCategory
	{
		public string category;

		public int n;

		public GameObject[] nodes;
	}

	[SerializeField]
	private UIPrefab_GameInfo uiPrefab_GameInfo;

	[SerializeField]
	private TMP_Text departureTimer;

	[SerializeField]
	public TMP_Text currency;

	[SerializeField]
	private AlarmClock alarmClock;

	[SerializeField]
	private RuntimeDungeon runtimeDungeon;

	[SerializeField]
	private SkyAndWeatherSystem skyAndWeather;

	[SerializeField]
	private SkyAndWeatherSystem.eWeather defaultWeather;

	[SerializeField]
	private SkyAndWeatherSystem.eCloudDensity defaultCloudDensity;

	[SerializeField]
	private float defaultRainIntensity;

	[SerializeField]
	private Transform[] randomCanopy;

	[SerializeField]
	protected Transform ActorRoot;

	protected Transform lootingObjectSpawnRoot;

	private SkyAndWeatherSystem.eWeatherPreset currentWeather;

	private Coroutine? mainLoopRunner;

	private Coroutine netLoopRunner;

	private bool isAvatarIndoor;

	public Action<bool> OnIndoorOutdoorChanged;

	private bool isEndDungeon;

	private bool oldCameraTargetIsOutdoor = true;

	private int cameraTargetIsOutdoorChanged;

	private int cameraTargetTileChanged;

	private Tile currentCameraTargetTile;

	private Transform indoorRoot;

	private Transform outdoorRoot;

	private float worldTime = 9f;

	private bool debug_weatherFF;

	private bool debug_weatherNoon;

	private float localGameLoopElapsedTime;

	private UIPrefab_WeatherForecast weatherForecastUI;

	[SerializeField]
	private GameObject randomTeleportStartPrefab;

	[SerializeField]
	private GameObject test_raindropVFX;

	private GameObject test_raindropInstance;

	[SerializeField]
	private BoxCollider outdoorCollider;

	private CachedRenderableComponents cachedStaticRenderableComps;

	[SerializeField]
	private RandomPlacerCategory[] randomPlacer;

	public bool IsAvatarIndoor => isAvatarIndoor;

	public bool IsCameraTargetOutdoorChanged => cameraTargetIsOutdoorChanged > 0;

	public bool IsCameraTargetTileChanged => cameraTargetTileChanged > 0;

	public bool IsGameLoopRunning { get; private set; }

	public int DungeonMasterID => base.pdata.dungeonMasterID;

	public int RandDungeonSeed => base.pdata.randDungeonSeed;

	public BoxCollider OutdoorBoxCollier => outdoorCollider;

	public Transform GetActorRoot()
	{
		return ActorRoot;
	}

	public Transform GetOutdoorRoot()
	{
		return outdoorRoot;
	}

	public Transform GetOutdoorLootingObjectRoot()
	{
		return lootingObjectSpawnRoot;
	}

	protected override void Awake()
	{
		if (!(Hub.s == null))
		{
			base.Awake();
			indoorRoot = BGRoot.Find("indoor");
			outdoorRoot = BGRoot.Find("outdoor");
			if (lootingObjectSpawnRoot == null)
			{
				CreateOutdoorLootingObjectSpawnRoot();
			}
		}
	}

	protected override void OnDestroy()
	{
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.ExitGame);
		base.OnDestroy();
		if (!(Hub.s == null))
		{
			base.uiman.CloseAllDialogueBox();
			Hub.s.navman.Clear();
			Hub.s.dynamicDataMan.Clear();
		}
	}

	protected override void Start()
	{
		if (Hub.s == null)
		{
			return;
		}
		base.Start();
		DungeonMasterInfo value = Hub.s.dataman.ExcelDataManager.DungeonInfoDict.FirstOrDefault<KeyValuePair<int, DungeonMasterInfo>>((KeyValuePair<int, DungeonMasterInfo> d) => d.Key == DungeonMasterID).Value;
		if (value != null)
		{
			StartSceneLoading(value.LoadingSceneName);
		}
		else
		{
			StartSceneLoading("Default");
		}
		base.pdata.serverRoomState = Hub.PersistentData.eServerRoomState.InGame;
		base.pdata.GameState = Hub.PersistentData.eGameState.InGame;
		if (skyAndWeather == null)
		{
			skyAndWeather = outdoorRoot.GetComponentInChildren<SkyAndWeatherSystem>();
			if (skyAndWeather == null)
			{
				Logger.RError("SkyAndWeatherSystem not found");
				return;
			}
		}
		if (value != null)
		{
			skyAndWeather.SetPollutionLevel(value.PollutionLevel);
		}
		if (outdoorCollider == null)
		{
			Volume[] componentsInChildren = skyAndWeather.transform.parent.GetComponentsInChildren<Volume>();
			foreach (Volume volume in componentsInChildren)
			{
				outdoorCollider = volume.GetComponent<BoxCollider>();
				if (outdoorCollider != null)
				{
					break;
				}
			}
		}
		InitCommonUI();
		CreateSpectatorHUD();
		mainLoopRunner = StartCoroutine(CorRun());
	}

	public void SetTramUpgrade()
	{
		if (Hub.s.tramUpgrade != null)
		{
			Hub.s.tramUpgrade.Init(base.pdata.TramUpgradeIDs);
			if (tramRepairRules != null)
			{
				Hub.s.tramUpgrade.ApplyTramUpgradePartsToTramAtSceneInit(tramRepairRules);
			}
		}
	}

	private void TryCreateGameRoom()
	{
		if (base.pdata.ClientMode == NetworkClientMode.Host)
		{
			Logger.RLog("CreateGameRoom");
			Hub.s.vworld?.CreateGameRoom();
		}
	}

	private IEnumerator TryEnterGameRoom(long roomID)
	{
		Logger.RLog("EnterGameRoom");
		EnterDungeonRes enterGameRoomRes = null;
		SendPacketWithCallback(new EnterDungeonReq
		{
			roomID = roomID
		}, delegate(EnterDungeonRes _res)
		{
			enterGameRoomRes = _res;
			if (_res == null)
			{
				base.pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
				goBackToMainMenuFlag = true;
			}
			else if (_res.errorCode != MsgErrorCode.Success)
			{
				goBackToMainMenuFlag = true;
				base.pdata.lastResponseError = _res.errorCode;
			}
			else
			{
				base.pdata.MyActorID = _res.playerInfo.actorID;
				OnWeatherSync(_res.currentWeatherMasterID, _res.forecastWeatherMasterID);
			}
		}, destroyToken, 60000, disconnectWhenTimeout: true);
		yield return new WaitUntil(() => goBackToMainMenuFlag || enterGameRoomRes != null);
		if (goBackToMainMenuFlag)
		{
			OnNetDisconnected(DisconnectReason.ConnectionError, "Failed to EnterGameRoom");
		}
	}

	private IEnumerator CorRun()
	{
		Logger.RLog($"GamePlayScene CorRun: dayCount = {base.pdata.DayCount}, cycleCount = {base.pdata.CycleCount}");
		skyAndWeather?.TurnOn(defaultWeather, defaultCloudDensity, defaultRainIntensity);
		isAvatarIndoor = false;
		if (CheckNetworkConnection())
		{
			yield break;
		}
		base.netman2.PurgeMsg();
		base.uiman.ui_sceneloading.SetLoadingText("STRING_LOADING");
		if (runtimeDungeon != null)
		{
			base.pdata.ResetDunGenTileIDSeed();
			DungeonMasterInfo? obj = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(DungeonMasterID) ?? throw new Exception($"DungeonMasterInfo not found on GamePlayScene Loading CorRun for ID: {DungeonMasterID}");
			int maxDungenRate = obj.MaxDungenRate;
			int randVal = new SyncRandom(RandDungeonSeed).Next(0, maxDungenRate);
			CrashReportHandler.SetUserMetadata("rand_dungeon_seed", RandDungeonSeed.ToString());
			Logger.RLog($"RandDungeonSeed: {RandDungeonSeed}, DungeonId : {DungeonMasterID}");
			string randomDungenName = obj.GetRandomDungenName(randVal);
			if (randomDungenName == string.Empty)
			{
				throw new Exception($"DungeonFlowName not found on GamePlayScene Loading CorRun for ID: {DungeonMasterID}");
			}
			bool done = false;
			runtimeDungeon.Generator.OnGenerationComplete += delegate
			{
				done = true;
			};
			runtimeDungeon.Generator.Seed = base.pdata.randDungeonSeed;
			if (!Hub.s.tableman.dungenFlowTable.TryGet(randomDungenName, out DungenFlowTable.Row row) || row == null)
			{
				throw new Exception("DungeonFlowName not found on GamePlayScene Loading CorRun for ID: " + randomDungenName);
			}
			runtimeDungeon.Generator.DungeonFlow = row.flow;
			runtimeDungeon.Generate();
			int num = 3;
			while (runtimeDungeon.Generator.Status == GenerationStatus.Failed && num > 0)
			{
				runtimeDungeon.Generator.Seed++;
				runtimeDungeon.Generate();
				num--;
			}
			if (runtimeDungeon.Generator.Status == GenerationStatus.Failed)
			{
				throw new Exception("Dungeon Generation Failed after retries");
			}
			yield return new WaitUntil(() => done);
		}
		yield return new WaitForSecondsRealtime(0.1f);
		yield return new WaitForFixedUpdate();
		if (randomCanopy != null)
		{
			DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(DungeonMasterID);
			if (dungeonInfo != null)
			{
				int canopyCount = dungeonInfo.CanopyCount;
				int num2 = randomCanopy.Length - canopyCount;
				SyncRandom syncRandom = new SyncRandom(RandDungeonSeed);
				while (num2 > 0)
				{
					int num3 = syncRandom.Next(0, randomCanopy.Length);
					Transform transform = randomCanopy[num3];
					if (!(transform == null) && transform.gameObject.activeSelf)
					{
						num2--;
						transform.gameObject.SetActive(value: false);
					}
				}
			}
		}
		ApplyRandomPlacer();
		ImplantRandomTeleport();
		if (tramRepairRules != null)
		{
			tramRepairRules.ApplyDestructionPartsToTramInGamePlayScene(base.pdata.CycleCount, base.pdata.DayCount);
			int num4 = base.pdata.CycleCount - 1;
			tramRepairRules.ApplyNewPartsToTram(num4);
			Logger.RLog($"ApplyDestructionPartsToTramInGamePlayScene: {base.pdata.CycleCount}, {base.pdata.DayCount}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
			Logger.RLog($"ApplyNewPartsToTram: {num4}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
			tramRepairRules.ApplySteamItemPartsToTramInCommon(base.pdata.AppliedTramSkin);
			Hub.s.tramUpgrade.ApplyTramUpgradePartsToTramAtSceneInit(tramRepairRules);
		}
		Hub.s.dynamicDataMan.Build(BGRoot, RandDungeonSeed);
		CollectTeleporterPositions();
		MakeCachedStaticRenderableComponents();
		interactObjectHelper.InitAllLevelObjects();
		skyAndWeather?.OnSceneLoadComplete();
		TurnOffInDoorRenderables();
		Hub.s.dLAcademyManager.GetAreaForDL(Vector3.zero, out var _, forceReset: true);
		BuildNavMesh();
		TryCreateGameRoom();
		Logger.RLog("Waiting CompleteMakingRoom");
		yield return new WaitUntil(() => base.pdata.completeMakingRoomSig != null);
		long roomUID = base.pdata.completeMakingRoomSig.nextRoomInfo.roomUID;
		yield return TryEnterGameRoom(roomUID);
		SetRemainDays();
		yield return SpawnMyAvatar();
		yield return TryLevelLoad();
		if (base.pdata.lastResponseError != MsgErrorCode.Success)
		{
			yield break;
		}
		if (Hub.s.tramUpgrade != null)
		{
			Hub.s.tramUpgrade.Init(base.pdata.TramUpgradeIDs);
			if (tramRepairRules != null)
			{
				Hub.s.tramUpgrade.ApplyTramUpgradePartsToTramAtSceneInit(tramRepairRules);
			}
		}
		InitCommonUIValue();
		SetGameStatusUI();
		if (uiPrefab_GameInfo != null)
		{
			uiPrefab_GameInfo.ApplyMap(DungeonMasterID);
			uiPrefab_GameInfo.UpdateTime(TimeSpan.FromHours(9.0));
			uiPrefab_GameInfo.Show();
		}
		if (tramConsole != null)
		{
			tramConsole.UpdateLeverState(maintenanceDecision: false);
			tramConsole.UpdateWantedPoster(netSyncGameData.boostedItem, netSyncGameData.boostedRatio);
		}
		netLoopRunner = StartCoroutine(CorNetLoop());
		base.uiman.ui_sceneloading.SetLoadingText("STRING_LOADING_WAIT");
		Logger.RLog("Waiting EnteringCompleteAll in GamePlayScene");
		yield return new WaitUntil(() => EnteringCompleteAll);
		SetEnableInputForMyAvatar();
		yield return WaitForMinimumLoadingTime();
		if (EnteringCutSceneNameQueue.Count > 0)
		{
			yield return TryPlayEnteringCutScene();
		}
		else
		{
			EndSceneLoading();
		}
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.EnterGame);
		Hub.s.voiceman.SetVoiceMode(VoiceMode.Player);
		IsGameLoopRunning = true;
		IsGameLogicRunning = true;
		Logger.RLog("GamePlayScene Started");
		yield return WaitForEndGame();
		IsGameLogicRunning = false;
		if (!(Hub.s == null))
		{
			PrepareExiting();
			yield return TryPlayExitingCutScene();
		}
	}

	private void ImplantRandomTeleport()
	{
		if (BGRoot == null)
		{
			return;
		}
		if (randomTeleportStartPrefab == null)
		{
			Logger.RError("Random Teleport Prefabs are not set");
			return;
		}
		DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(DungeonMasterID);
		if (dungeonInfo == null)
		{
			Logger.RError($"DungeonMasterInfo not found for ID: {DungeonMasterID}");
			return;
		}
		SyncRandom syncRandom = new SyncRandom(RandDungeonSeed);
		if (syncRandom.Next(0, 10000) >= dungeonInfo.randomTeleporterChance)
		{
			return;
		}
		List<DungenUtilBlockerMarker> list = (from b in BGRoot.GetComponentsInChildren<DungenUtilBlockerMarker>(includeInactive: false)
			where b.canReplacedWithRandomTeleportStart
			select b).ToList();
		int num = syncRandom.Next(dungeonInfo.randomTeleporterStartMin, dungeonInfo.randomTeleporterStartMax + 1);
		if (list.Count < num)
		{
			Logger.RError("blockers count is not enough for random teleporter start");
			return;
		}
		MapMarker_TeleportEndPoint[] componentsInChildren = BGRoot.GetComponentsInChildren<MapMarker_TeleportEndPoint>(includeInactive: false);
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			Logger.RError("MapMarker_RandomTeleportEndPoint count is not enough for random teleporter end");
			return;
		}
		for (int num2 = 0; num2 < num; num2++)
		{
			DungenUtilBlockerMarker dungenUtilBlockerMarker = list[syncRandom.Next(0, list.Count)];
			Vector3 localPosition = dungenUtilBlockerMarker.transform.localPosition;
			Quaternion localRotation = dungenUtilBlockerMarker.transform.localRotation;
			Vector3 localScale = dungenUtilBlockerMarker.transform.localScale;
			Transform parent = dungenUtilBlockerMarker.transform.parent;
			GameObject gameObject = null;
			gameObject = ((!(dungenUtilBlockerMarker.RandomTeleportCustomPrefab != null)) ? randomTeleportStartPrefab : dungenUtilBlockerMarker.RandomTeleportCustomPrefab);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, parent);
			gameObject2.transform.localPosition = localPosition;
			gameObject2.transform.localRotation = localRotation;
			gameObject2.transform.localScale = localScale;
			RandomTeleporterLevelObject component = gameObject2.GetComponent<RandomTeleporterLevelObject>();
			if (component == null)
			{
				Logger.RError("RandomTeleporterLevelObject component not found on RandomTeleportStart prefab");
				continue;
			}
			component.levelObjectName = component.levelObjectName.Replace("{index}", num2.ToString());
			foreach (KeyValuePair<int, Dictionary<int, LevelObject.StateActionInfo>> item in component.StateActionsMap)
			{
				Dictionary<int, LevelObject.StateActionInfo> dictionary = component.StateActionsMap[item.Key];
				foreach (KeyValuePair<int, LevelObject.StateActionInfo> item2 in dictionary)
				{
					LevelObject.StateActionInfo stateActionInfo = dictionary[item2.Key];
					stateActionInfo.action = stateActionInfo.action.Replace("{index}", num2.ToString());
				}
			}
			MapMarker_TeleportStartPoint component2 = gameObject2.GetComponent<MapMarker_TeleportStartPoint>();
			if (component2 == null)
			{
				Logger.RError("MapMarker_RandomTeleportStartPoint component not found on RandomTeleportStart prefab");
				break;
			}
			component2.CallSign = component2.CallSign.Replace("{index}", num2.ToString());
			UnityEngine.Object.DestroyImmediate(dungenUtilBlockerMarker.gameObject);
			list.Remove(dungenUtilBlockerMarker);
		}
	}

	private IEnumerator WaitForEndGame()
	{
		long c_GameTimeScaleFactor = Hub.s.dataman.ExcelDataManager.Consts.C_GameTimeScaleFactor;
		float GameTimeScale = (float)c_GameTimeScaleFactor / 1000f;
		while (true)
		{
			if (Hub.s == null)
			{
				IsGameLoopRunning = false;
				yield break;
			}
			if (isEndDungeon)
			{
				IsGameLoopRunning = false;
			}
			if (EnterExitingProcess)
			{
				break;
			}
			interactObjectHelper.Step();
			localGameLoopElapsedTime += Time.deltaTime;
			if (debug_weatherNoon)
			{
				worldTime = 12f;
			}
			else if (debug_weatherFF)
			{
				worldTime += Time.deltaTime * 0.5f;
			}
			else
			{
				worldTime += Time.deltaTime * GameTimeScale / 3600f;
			}
			if (worldTime > 24f)
			{
				worldTime -= 24f;
			}
			if (IsCameraTargetOutdoor() && skyAndWeather != null)
			{
				skyAndWeather?.SetTime(worldTime);
			}
			if (cameraTargetIsOutdoorChanged > 0)
			{
				cameraTargetIsOutdoorChanged--;
			}
			if (cameraTargetTileChanged > 0)
			{
				cameraTargetTileChanged--;
			}
			yield return null;
		}
		IsGameLoopRunning = false;
	}

	protected override void Update()
	{
		if (!(Hub.s == null))
		{
			base.Update();
			if (IsGameLoopRunning)
			{
				UpdateCameraTargetCulling();
			}
		}
	}

	private void PrepareExiting()
	{
		TurnOnOutDoorRenderables();
		if (skyAndWeather != null)
		{
			skyAndWeather?.TurnOn(currentWeather);
			skyAndWeather?.SetTime(worldTime);
		}
		UnmuteWeatherSfx();
		if (IsSpectatorMode())
		{
			spectatorui.Hide();
		}
		if (_myAvatar != null)
		{
			_myAvatar.ClearEffectPlayer();
		}
		Hub.s.voiceman.SetVoiceMode(VoiceMode.Observer);
		UIManager uIManager = Hub.s.uiman;
		uIManager.OnSetupSpectatorCamera = (UnityAction<ProtoActor>)Delegate.Remove(uIManager.OnSetupSpectatorCamera, new UnityAction<ProtoActor>(OnSetupSpectatorCamera));
		base.pdata.main.DestroyAllActors();
		base.pdata.main.HideAllLootingObjects();
		interactObjectHelper.ClearCrosshair();
	}

	private void UpdateCameraTargetCulling()
	{
		if (base.dungenCuller == null || !base.dungenCuller.enabled)
		{
			return;
		}
		Tile tile = currentCameraTargetTile;
		if (!Hub.s.cameraman.TryGetCurrentSpectatorTarget(out ProtoActor target))
		{
			target = GetMyAvatar();
		}
		if (target != null)
		{
			Transform transform = target.transform;
			if (currentCameraTargetTile == null)
			{
				currentCameraTargetTile = base.dungenCuller.relumod_FastFindCurrentTile(transform, 1f);
			}
			else if (!currentCameraTargetTile.Bounds.Contains(transform.position + Vector3.up * 1f))
			{
				currentCameraTargetTile = base.dungenCuller.relumod_FastFindCurrentTile(transform, 1f);
			}
		}
		if (tile != currentCameraTargetTile)
		{
			cameraTargetTileChanged = 4;
		}
	}

	public bool IsCameraTargetOutdoor()
	{
		bool flag = false;
		flag = ((!IsSpectatorMode()) ? (!isAvatarIndoor) : ((!Hub.s.cameraman.TryGetCurrentSpectatorTarget(out ProtoActor target)) ? (!isAvatarIndoor) : (!CheckActorIsIndoor(target))));
		if (oldCameraTargetIsOutdoor != flag)
		{
			cameraTargetIsOutdoorChanged = 3;
			oldCameraTargetIsOutdoor = flag;
		}
		return flag;
	}

	private void Generator_OnGenerationComplete(DungeonGenerator generator)
	{
		throw new NotImplementedException();
	}

	public Tile? FindCurrentTile(in Vector3 position)
	{
		if (base.dungenCuller == null)
		{
			return null;
		}
		return base.dungenCuller.relumod_FastFindCurrentTile(position, 1f);
	}

	public override Transform GetActorSpawnRootTransform(in Vector3 spawnPos = default(Vector3))
	{
		Transform actorRoot = GetActorRoot();
		if (actorRoot != null)
		{
			return actorRoot;
		}
		return GetBGRoot();
	}

	private IEnumerator CorNetLoop()
	{
		while (!(Hub.s == null))
		{
			if (_myAvatar != null)
			{
				_myAvatar.OnBeforeTick();
			}
			yield return new WaitForSeconds(1f / base.gameConfig.playerActor.sendPositionFrequency);
		}
	}

	public override void OnPlayerSpawn(ProtoActor actor)
	{
		base.OnPlayerSpawn(actor);
		if (actor.AmIAvatar())
		{
			DlHelper.OnOwnerPlayerSpawn(protoActorMap, actor.ActorID);
		}
	}

	public override void OnPlayerDespawn(ProtoActor actor)
	{
		base.OnPlayerDespawn(actor);
		if (actor.AmIAvatar())
		{
			DlHelper.OnOwnerPlayerDespawn();
		}
	}

	protected override void OnCurrencyChanged(int prev, int curr)
	{
		base.OnCurrencyChanged(prev, curr);
		currency.SetText(netSyncGameData.currency.ToString());
	}

	protected override void OnTimeChanged(TimeSpan currentTime)
	{
		if (alarmClock != null)
		{
			alarmClock.UpdateTime(currentTime);
		}
		if (uiPrefab_GameInfo != null)
		{
			uiPrefab_GameInfo.UpdateTime(currentTime);
		}
		if (!debug_weatherNoon && !debug_weatherFF)
		{
			worldTime = currentTime.Hours;
		}
	}

	protected override void OnWeatherSync(int currentWeatherMasterID, int nextWeatherMasterID)
	{
		if (!(skyAndWeather != null))
		{
			return;
		}
		WeatherInfo weatherInfo = Hub.s.dataman.ExcelDataManager.GetWeatherInfo(currentWeatherMasterID);
		if (weatherInfo == null)
		{
			Logger.RError($"WeatherDataTable is null, currentWeatherMasterID : {currentWeatherMasterID}");
			return;
		}
		string text = weatherInfo.Name.ToLower();
		if (text == SkyAndWeatherSystem.eWeatherPreset.Sunny.ToString().ToLower())
		{
			currentWeather = SkyAndWeatherSystem.eWeatherPreset.Sunny;
			skyAndWeather.SetWeatherPreset(SkyAndWeatherSystem.eWeatherPreset.Sunny);
			PlayWeatherSfx(SkyAndWeatherSystem.eWeatherPreset.Sunny);
		}
		else if (text == SkyAndWeatherSystem.eWeatherPreset.Rain.ToString().ToLower())
		{
			currentWeather = SkyAndWeatherSystem.eWeatherPreset.Rain;
			skyAndWeather.SetWeatherPreset(SkyAndWeatherSystem.eWeatherPreset.Rain);
			PlayWeatherSfx(SkyAndWeatherSystem.eWeatherPreset.Rain);
		}
		else if (text == SkyAndWeatherSystem.eWeatherPreset.HeavyRain.ToString().ToLower())
		{
			currentWeather = SkyAndWeatherSystem.eWeatherPreset.HeavyRain;
			skyAndWeather.SetWeatherPreset(SkyAndWeatherSystem.eWeatherPreset.HeavyRain);
			PlayWeatherSfx(SkyAndWeatherSystem.eWeatherPreset.HeavyRain);
		}
		else if (text == SkyAndWeatherSystem.eWeatherPreset.Squall.ToString().ToLower())
		{
			currentWeather = SkyAndWeatherSystem.eWeatherPreset.Squall;
			skyAndWeather.SetWeatherPreset(SkyAndWeatherSystem.eWeatherPreset.Squall);
			PlayWeatherSfx(SkyAndWeatherSystem.eWeatherPreset.Squall);
		}
		else
		{
			Logger.RError("Unknown weather name : " + text);
		}
		if (nextWeatherMasterID == 4 && currentWeatherMasterID != nextWeatherMasterID)
		{
			if (weatherForecastUI == null)
			{
				weatherForecastUI = base.uiman.InstatiateUIPrefab<UIPrefab_WeatherForecast>(base.uiman.prefab_WeathweForecast);
			}
			weatherForecastUI.Show();
		}
	}

	public bool IsSpectatorMode()
	{
		if (spectatorui != null)
		{
			return spectatorui.isActiveAndEnabled;
		}
		return false;
	}

	protected override void OnSetupSpectatorCamera(ProtoActor actor)
	{
		base.OnSetupSpectatorCamera(actor);
		bool flag = CheckActorIsIndoor(actor);
		SwitchToIndoorOrOutdoorRenderables(flag);
		Hub.s.voiceman.SetTransmitterChannelRecv(flag);
	}

	public override void OnActorTeleported(ProtoActor actor, Vector3 pos)
	{
		if (actor == null)
		{
			Logger.RError("GamePlayScene::OnActorTeleported called with null actor");
		}
		else if (!EnterExitingProcess)
		{
			OnActorTeleported_RenderableCulling(actor);
		}
	}

	public bool IsWeatherForcastUIActive()
	{
		if (weatherForecastUI == null)
		{
			return false;
		}
		return weatherForecastUI.isActiveAndEnabled;
	}

	private void SetRemainDays()
	{
		int dayCount = base.pdata.DayCount;
		if (tramConsole != null)
		{
			tramConsole.SetDayCount(dayCount);
		}
	}

	public long GetRandomDeadActorUID()
	{
		List<ProtoActor> list = protoActorMap.Values.Where((ProtoActor actor) => actor != null && actor.ActorType == ActorType.Player && actor.dead).ToList();
		if (list.Count == 0)
		{
			return -1L;
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		return list[index].UID;
	}

	public long GetMaxDistancePlayerUID(Vector3 origin)
	{
		IEnumerable<ProtoActor> enumerable = protoActorMap.Values.Where((ProtoActor actor) => actor != null && actor.ActorType == ActorType.Player && !actor.dead);
		if (!enumerable.Any())
		{
			return -1L;
		}
		ProtoActor protoActor = null;
		float num = float.MinValue;
		foreach (ProtoActor item in enumerable)
		{
			float sqrMagnitude = (item.transform.position - origin).sqrMagnitude;
			if (sqrMagnitude > num)
			{
				num = sqrMagnitude;
				protoActor = item;
			}
		}
		return protoActor.UID;
	}

	private void CreateOutdoorLootingObjectSpawnRoot()
	{
		if (outdoorRoot == null)
		{
			Logger.RError("Outdoor root is null, cannot create LootingObjectSpawnRoot");
			return;
		}
		GameObject gameObject = new GameObject("LootingObjectSpawnRoot");
		gameObject.transform.SetParent(outdoorRoot, worldPositionStays: false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		lootingObjectSpawnRoot = gameObject.transform;
	}

	public override void OnLeaveRoomSig(LeaveRoomSig sig)
	{
		Logger.RLog($"LeaveRoom, {sig.actorID}");
		if (sig.actorID == base.pdata.MyActorID)
		{
			StartCoroutine(ClearRoomSequence(sig));
		}
		else if (!(Hub.s == null) && !(Hub.s.cameraman == null))
		{
			OnPlayerDespawn(sig.actorID);
		}
	}

	private IEnumerator ClearRoomSequence(LeaveRoomSig sig)
	{
		Logger.RLog("ClearRoom");
		yield return new WaitUntil(() => ExitingCutSceneNameQueue.Count <= 0);
		base.cameraman.OnEndDungeon(isSuccess: true);
		foreach (ProtoActor value in protoActorMap.Values)
		{
			if (value.ActorType == ActorType.Player)
			{
				OnPlayerDespawn(value);
			}
		}
		StopCoroutine(mainLoopRunner);
		if (netLoopRunner != null)
		{
			StopCoroutine(netLoopRunner);
			netLoopRunner = null;
		}
		yield return gameStatusUI.Cor_Hide();
		Hub.LoadScene("InTramWaitingScene");
	}

	public override bool IsGameSessionEnd()
	{
		return isEndDungeon;
	}

	private List<object> MakeLostStashParameterForResult(List<ItemInfo> itemInfos)
	{
		itemInfos.ConvertAll((ItemInfo i) => ProtoActor.Inventory.GetItemMasterInfo(i.itemMasterID)).ToList();
		itemInfos.Sort((ItemInfo i1, ItemInfo i2) => (i1.price > i2.price) ? 1 : ((i1.price != i2.price) ? (-1) : 0));
		List<object> parameters = new List<object> { itemInfos.Count };
		itemInfos.ForEach(delegate(ItemInfo i)
		{
			if (i != null)
			{
				ItemMasterInfo itemMasterInfo = ProtoActor.Inventory.GetItemMasterInfo(i.itemMasterID);
				parameters.Add(itemMasterInfo.Name);
				parameters.Add(i.price);
			}
		});
		return parameters;
	}

	public override void OnEndDungeonSig(EndDungeonSig sig)
	{
		Logger.RLog("EndDungeon");
		isEndDungeon = true;
		base.pdata.preLostStashItems.Clear();
		base.pdata.preLostStashItems = sig.result.removedItems;
		List<object> parameters = new List<object>();
		bool allPlayerDead = true;
		parameters.Add(base.pdata.DayCount);
		parameters.Add(sig.result.success);
		parameters.Add(sig.result.playerStatus.Count);
		sig.result.playerStatus.ToList().ForEach(delegate(KeyValuePair<int, (PlayerResultStatus resultStatus, AwardType awardType)> ps)
		{
			if (protoActorMap.TryGetValue(ps.Key, out ProtoActor value))
			{
				(PlayerResultStatus, AwardType) value2 = ps.Value;
				string item = ResolveNickName(value, value.nickName);
				parameters.Add(item);
				UIPrefab_SurvivalResult.eActorSurvivalState eActorSurvivalState = UIPrefab_SurvivalResult.eActorSurvivalState.Survive;
				switch (value2.Item1)
				{
				case PlayerResultStatus.Alived:
					allPlayerDead = false;
					break;
				case PlayerResultStatus.Dead:
					eActorSurvivalState = UIPrefab_SurvivalResult.eActorSurvivalState.Killed;
					break;
				case PlayerResultStatus.Wasted:
					eActorSurvivalState = UIPrefab_SurvivalResult.eActorSurvivalState.Wasted;
					break;
				default:
					Logger.RError($"Unknown reason of death : {value2}");
					eActorSurvivalState = UIPrefab_SurvivalResult.eActorSurvivalState.Unknown;
					break;
				}
				parameters.Add(eActorSurvivalState);
				AwardType awardType = AwardType.None;
				switch (value2.Item2)
				{
				case AwardType.None:
					awardType = AwardType.None;
					break;
				case AwardType.BestCarryItem:
					awardType = AwardType.BestCarryItem;
					break;
				case AwardType.BestDamageToAlly:
					awardType = AwardType.BestDamageToAlly;
					break;
				case AwardType.BestMimicEncounter:
					awardType = AwardType.BestMimicEncounter;
					break;
				case AwardType.BestCamper:
					awardType = AwardType.BestCamper;
					break;
				}
				parameters.Add(awardType);
			}
		});
		if (sig.result.success)
		{
			parameters.AddRange(MakeLostStashParameterForResult(sig.result.removedItems));
		}
		Hub.s.audioman.PlaySfx(tramHornAudioKey);
		if (_myAvatar != null)
		{
			_myAvatar?.DontMove();
		}
		AddEndingSequence(() => EndDungeonSequence(sig.result.success, parameters));
		base.pdata.DayCount++;
	}

	private IEnumerator EndDungeonSequence(bool survival, List<object> popupParams)
	{
		float seconds = Hub.s.tableman.uiprefabs.ShowTimerDialog("SurvivalResult", 0f, popupParams.ToArray());
		Logger.RLog("EndDungeonSequence");
		yield return new WaitForSeconds(seconds);
		yield return true;
	}

	public float GetLocalGameLoopElapsedTimeSec()
	{
		return localGameLoopElapsedTime;
	}

	public string GetLocalGameLoopElapsedTimeSecStr()
	{
		return $"{TimeSpan.FromSeconds(localGameLoopElapsedTime):hh\\:mm\\:ss}";
	}

	public override void OnPlayerDeath(ProtoActor actor, in ProtoActor.ActorDeathInfo actorDeathInfo)
	{
		base.OnPlayerDeath(actor, in actorDeathInfo);
		if (actor.AmIAvatar())
		{
			skyAndWeather?.SuppressRainScreenVFX();
		}
	}

	public void OnMapComplete()
	{
		skyAndWeather?.SuppressRainScreenVFX();
	}

	private void MakeCachedStaticRenderableComponents()
	{
		if (outdoorRoot == null)
		{
			return;
		}
		List<Behaviour> list = new List<Behaviour>();
		List<Renderer> list2 = new List<Renderer>();
		List<DecalProjector> list3 = new List<DecalProjector>();
		list.AddRange(outdoorRoot.GetComponentsInChildren<Terrain>());
		Light[] componentsInChildren = outdoorRoot.GetComponentsInChildren<Light>();
		foreach (Light light in componentsInChildren)
		{
			if (light.type != LightType.Directional)
			{
				list.Add(light);
			}
		}
		list2.AddRange(outdoorRoot.GetComponentsInChildren<MeshRenderer>());
		list2.AddRange(outdoorRoot.GetComponentsInChildren<SkinnedMeshRenderer>());
		list2.AddRange(outdoorRoot.GetComponentsInChildren<BillboardRenderer>());
		list3.AddRange(outdoorRoot.GetComponentsInChildren<DecalProjector>());
		cachedStaticRenderableComps = new CachedRenderableComponents(list, list2, list3);
	}

	private void SetEnabledStaticRenderableComponent(bool enable)
	{
		cachedStaticRenderableComps.SetEnabledRenderableComponent(enable);
	}

	public void MoveToIndoorOrOutdoor(bool indoor)
	{
		if (isAvatarIndoor != indoor)
		{
			isAvatarIndoor = indoor;
			SwitchToIndoorOrOutdoorRenderables(isAvatarIndoor);
			Hub.s.voiceman.SetTransmitterChannelRecv(isAvatarIndoor);
			OnIndoorOutdoorChanged?.Invoke(isAvatarIndoor);
		}
	}

	public void SwitchToIndoorOrOutdoorRenderables(bool isIndoor)
	{
		if (isIndoor)
		{
			TurnOffOutDoorRenderables();
			skyAndWeather?.TurnOff();
			TurnOnInDoorRenderables();
			MuteWeatherSfx();
		}
		else
		{
			TurnOnOutDoorRenderables();
			skyAndWeather?.TurnOn(currentWeather);
			TurnOffInDoorRenderables();
			UnmuteWeatherSfx();
		}
	}

	public void TurnOffInDoorRenderables()
	{
		if (base.dungenCuller != null)
		{
			base.dungenCuller.relumod_Suppress();
		}
	}

	public void TurnOnInDoorRenderables()
	{
		if (base.dungenCuller != null)
		{
			base.dungenCuller.relumod_Unsuppress();
		}
	}

	private void SetRootTransformRenderablesEnabled(Transform root, bool enable)
	{
		if (root == null || root.gameObject == null)
		{
			return;
		}
		MeshRenderer[] componentsInChildren = root.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = enable;
		}
		SkinnedMeshRenderer[] componentsInChildren2 = root.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = enable;
		}
		Light[] componentsInChildren3 = root.GetComponentsInChildren<Light>();
		foreach (Light light in componentsInChildren3)
		{
			if (light.type != LightType.Directional)
			{
				light.enabled = enable;
			}
		}
	}

	public void TurnOffOutDoorRenderables()
	{
		SetEnabledStaticRenderableComponent(enable: false);
		SetRootTransformRenderablesEnabled(lootingObjectSpawnRoot, enable: false);
	}

	public void TurnOnOutDoorRenderables()
	{
		SetEnabledStaticRenderableComponent(enable: true);
		SetRootTransformRenderablesEnabled(lootingObjectSpawnRoot, enable: true);
	}

	public bool CheckActorIsIndoor(ProtoActor actor)
	{
		return CheckPosIsIndoor(actor.transform.position);
	}

	public bool CheckPosIsIndoor(in Vector3 pos)
	{
		BoxCollider outdoorBoxCollier = OutdoorBoxCollier;
		if ((object)outdoorBoxCollier == null)
		{
			return false;
		}
		return !outdoorBoxCollier.bounds.Contains(pos);
	}

	private void OnActorTeleported_RenderableCulling(ProtoActor actor)
	{
		if (actor == null || actor.AmIAvatar())
		{
			return;
		}
		ProtoActor protoActor = null;
		if (IsSpectatorMode())
		{
			if (Hub.s.cameraman.TryGetCurrentSpectatorTarget(out ProtoActor target))
			{
				protoActor = target;
			}
			else
			{
				int? spectatorTargetActorID = Hub.s.cameraman.SpectatorTargetActorID;
				if (!spectatorTargetActorID.HasValue)
				{
					Logger.RError("SpectatorTargetActorID is null, cannot find target actor for spectator mode.");
					return;
				}
				protoActor = GetActorByActorID(spectatorTargetActorID.Value);
				if ((object)protoActor != null && !protoActor.dead)
				{
					Logger.RError($"Invalid target actor for spectator mode, ActorID : {spectatorTargetActorID.Value}");
				}
			}
		}
		else
		{
			protoActor = Hub.s.pdata.main?.GetMyAvatar() ?? null;
		}
		if (!(protoActor == null))
		{
			bool flag = CheckActorIsIndoor(protoActor);
			SwitchToIndoorOrOutdoorRenderables(flag);
			Hub.s.voiceman.SetTransmitterChannelRecv(flag);
		}
	}

	private void ApplyRandomPlacer()
	{
		if (randomPlacer == null)
		{
			return;
		}
		SyncRandom syncRandom = new SyncRandom(RandDungeonSeed);
		RandomPlacerCategory[] array = randomPlacer;
		foreach (RandomPlacerCategory randomPlacerCategory in array)
		{
			if (randomPlacerCategory.n <= 0 || randomPlacerCategory.nodes == null || randomPlacerCategory.nodes.Length == 0)
			{
				continue;
			}
			int n = randomPlacerCategory.n;
			int num = 0;
			GameObject[] nodes = randomPlacerCategory.nodes;
			foreach (GameObject gameObject in nodes)
			{
				if (gameObject == null)
				{
					Logger.RError("randomPlacer 설정 오류");
					continue;
				}
				gameObject.SetActive(value: false);
				num++;
			}
			int num2 = 0;
			while (num2 < n && num2 < num)
			{
				int num3 = syncRandom.Next(0, randomPlacerCategory.nodes.Length);
				GameObject gameObject2 = randomPlacerCategory.nodes[num3];
				if (!(gameObject2 == null))
				{
					gameObject2.SetActive(value: true);
					num2++;
				}
			}
		}
	}
}
