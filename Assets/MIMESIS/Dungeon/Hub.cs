using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Mimic.Actors;
using Mimic.Audio;
using Mimic.InputSystem;
using ModUtility;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluReplay;
using Steamworks;
using UnityEngine;
using UnityEngine.CrashReportHandler;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Hub : MonoBehaviour
{
	public class PersistentData
	{
		public enum eServerRoomState
		{
			Nowhere = 0,
			PreGame = 1,
			InGame = 2
		}

		public enum eGameState
		{
			Prepare = 0,
			PrepareDoneWithPublicLobby = 1,
			PrepareDone = 2,
			Waiting = 3,
			InGame = 4,
			Maintenance = 5,
			RepairDirectionWait = 6,
			RepairDirection = 7,
			RepairDirectionEnd = 8,
			LeaveNext = 9,
			FailDirection = 10,
			GoDeathMatch = 11,
			Restart = 12
		}

		public bool IsPublicLobby;

		public List<int> UserSteamInventoryItemDefIDs = new List<int>();

		public Dictionary<int, string> AppliedItemSkinDictionary = new Dictionary<int, string>();

		public List<string> AppliedTramSkin = new List<string>();

		public List<int> AppliedAdditionalItemIDs = new List<int>();

		public int LastSelectedTabIndex = -1;

		public string GUID = SystemInfo.deviceUniqueIdentifier;

		public string ClientSessionID = DateTime.Now.Ticks.ToString();

		public string ClientRoomSessionID = string.Empty;

		public bool ClientRoomFirstCycle;

		public int CycleCount = 1;

		public int DayCount = 1;

		public bool Repaired = true;

		public List<int> TramUpgradeIDs = new List<int>();

		public long PlayerUID;

		public int PlayerIndexInRoom;

		public string MyNickName = "Player Unknown";

		public GameMainBase? main;

		public bool SessionJoined;

		public CompleteMakingRoomSig? completeMakingRoomSig;

		public EnableWaitingRoomSig? enableWaitingRoomSig;

		public EnableDeathMatchRoomSig? enableDeathMatchRoomSig;

		public MsgErrorCode lastResponseError;

		public int randDungeonSeed;

		private int dungenTileID;

		public int dungeonMasterID;

		public int hitContextID;

		public Dictionary<int, int> itemPrices = new Dictionary<int, int>();

		public Dictionary<long, ulong> actorUIDToSteamID = new Dictionary<long, ulong>();

		public List<int> nextGameDungeonMasterIDs = new List<int>();

		public int MyActorID;

		public List<ItemInfo> preLostStashItems = new List<ItemInfo>();

		public float fsrAutoScale = 1f;

		public float fsrAutoScaleAdaptation = 0.05f;

		public eServerRoomState serverRoomState;

		private eGameState gameState;

		public string[] commandLineArgs;

		public float _debug_timeStampForProfile;

		public (int leftPanel, int rightPanel) TramUpgradeCandidate { get; set; } = (leftPanel: 0, rightPanel: 0);

		public NetworkClientMode ClientMode { get; set; }

		public string GameServerAddressOrSteamId { get; set; } = "127.0.0.1";

		public int GameServerPort { get; set; }

		public bool WithRelay { get; set; }

		public int SaveSlotID { get; set; } = -1;

		public eGameState GameState
		{
			get
			{
				return gameState;
			}
			set
			{
				Logger.RLog($"{gameState} -> {value}");
				gameState = value;
			}
		}

		public PersistentData()
		{
			ClientMode = NetworkClientMode.Host;
			PlayerIndexInRoom = -1;
		}

		public void ResetDunGenTileIDSeed()
		{
			dungenTileID = 0;
		}

		public long GenerateHitContextID()
		{
			return Interlocked.Increment(ref hitContextID);
		}

		public int GenerateDunGenTileID()
		{
			dungenTileID++;
			return dungenTileID;
		}

		public void ResetCycleInfos()
		{
			CycleCount = 1;
			DayCount = 1;
			Repaired = true;
		}

		public ulong GetUserSteamIDUInt64()
		{
			return ulong.Parse(SteamUser.GetSteamID().ToString());
		}

		public string GetUserSteamIDString()
		{
			return SteamUser.GetSteamID().ToString();
		}

		public ulong GetJoinedLobbyID()
		{
			return s.steamInviteDispatcher.joinedLobbyID.m_SteamID;
		}
	}

	internal class HitCheckDrawData
	{
		public int ActorID { get; set; }

		public Vector3 Position { get; set; }

		public Rotator Rotation { get; set; }

		public HitCheckShapeType ShapeType { get; set; }

		public Vector3 Extent { get; set; }

		public float Radius { get; set; }

		public float Length { get; set; }

		public float InnerRadius { get; set; }

		public float OuterRadius { get; set; }

		public float Height { get; set; }

		public float Angle { get; set; }

		public HitCheckDrawData(int actorID, HitCheckDrawInfo info)
		{
			ActorID = actorID;
			Position = info.center;
			Rotation = info.rotation;
			ShapeType = info.shapeType;
			if (info is CubeHitCheckDrawInfo cubeHitCheckDrawInfo)
			{
				Extent = cubeHitCheckDrawInfo.Extent;
			}
			else if (info is CapsuleHitCheckDrawInfo capsuleHitCheckDrawInfo)
			{
				Radius = (float)capsuleHitCheckDrawInfo.Radius;
				Length = (float)capsuleHitCheckDrawInfo.Length;
			}
			else if (info is SphereHitCheckDrawInfo sphereHitCheckDrawInfo)
			{
				Radius = (float)sphereHitCheckDrawInfo.Radius;
			}
			else if (info is FanHitCheckDrawInfo fanHitCheckDrawInfo)
			{
				InnerRadius = fanHitCheckDrawInfo.InnerRad;
				OuterRadius = fanHitCheckDrawInfo.OuterRad;
				Height = fanHitCheckDrawInfo.Height;
				Angle = fanHitCheckDrawInfo.Angle;
			}
			else if (info is TorusHitCheckDrawInfo torusHitCheckDrawInfo)
			{
				InnerRadius = torusHitCheckDrawInfo.InnerRad;
				OuterRadius = torusHitCheckDrawInfo.OuterRad;
				Height = torusHitCheckDrawInfo.Height;
			}
		}
	}

	public static Hub s;

	[SerializeField]
	private GameObject prefab_VoiceManager;

	[SerializeField]
	private GameObject prefab_KOSManager;

	[SerializeField]
	internal GameConfig gameConfig;

	public static readonly WaitForSeconds oneSecond = new WaitForSeconds(1f);

	public static readonly WaitForSeconds halfSecond = new WaitForSeconds(0.5f);

	private static int? _DefaultOnlyLayerMask = null;

	private static int? _DefaultAndActorLayerMask = null;

	private static int? _WallLayerMask = null;

	private static int? _AllWallsLayerMask = null;

	private List<Action> onExitPlaymode = new List<Action>();

	internal PersistentData pdata;

	internal SteamConnector SteamConnector = new SteamConnector();

	internal L10NManager lcman;

	internal RenderPerformanceManager rpmman;

	internal ReplayManager replayManager;

	internal SteamInviteDispatcher steamInviteDispatcher;

	internal SteamInventoryManager steamInventoryManager;

	internal GameSettingManager gameSettingManager;

	internal ResidualObjectManager residualObject;

	internal DecalManager decalManamger;

	internal ThinPerfHud thinPerfHud;

	internal KOSManager kosManager;

	internal FlagImageLoader flagImageLoader;

	internal TooltipManager tooltipManager;

	internal TramUpgradeManager tramUpgrade;

	private Dictionary<string, string> initProfilingResult = new Dictionary<string, string>();

	private Stopwatch initProfilingStopwatch = new Stopwatch();

	private List<ulong> _replayUploadWhiteList = new List<ulong>();

	private List<HitCheckDrawData> _currentHitChecks = new List<HitCheckDrawData>();

	private bool _showHitChecks;

	public static int DefaultOnlyLayerMask
	{
		get
		{
			int valueOrDefault = _DefaultOnlyLayerMask.GetValueOrDefault();
			if (!_DefaultOnlyLayerMask.HasValue)
			{
				valueOrDefault = 1 << LayerMask.NameToLayer("Default");
				_DefaultOnlyLayerMask = valueOrDefault;
			}
			return _DefaultOnlyLayerMask.Value;
		}
	}

	public static int DefaultAndActorLayerMask
	{
		get
		{
			int valueOrDefault = _DefaultAndActorLayerMask.GetValueOrDefault();
			if (!_DefaultAndActorLayerMask.HasValue)
			{
				valueOrDefault = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Actor"));
				_DefaultAndActorLayerMask = valueOrDefault;
			}
			return _DefaultAndActorLayerMask.Value;
		}
	}

	public static int RaySensorLayerMask
	{
		get
		{
			int valueOrDefault = _WallLayerMask.GetValueOrDefault();
			if (!_WallLayerMask.HasValue)
			{
				valueOrDefault = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Actor")) | (1 << LayerMask.NameToLayer("InvisibleWall"));
				_WallLayerMask = valueOrDefault;
			}
			return _WallLayerMask.Value;
		}
	}

	public static int AllWallsLayerMask
	{
		get
		{
			int valueOrDefault = _AllWallsLayerMask.GetValueOrDefault();
			if (!_AllWallsLayerMask.HasValue)
			{
				valueOrDefault = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("InvisibleWall"));
				_AllWallsLayerMask = valueOrDefault;
			}
			return _AllWallsLayerMask.Value;
		}
	}

	public static int RaySensorLayerMaskForMimic
	{
		get
		{
			int valueOrDefault = _WallLayerMask.GetValueOrDefault();
			if (!_WallLayerMask.HasValue)
			{
				valueOrDefault = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Actor")) | (1 << LayerMask.NameToLayer("InvisibleWall")) | (1 << LayerMask.NameToLayer("MimicOnly"));
				_WallLayerMask = valueOrDefault;
			}
			return _WallLayerMask.Value;
		}
	}

	public static int AllWallsLayerMaskForMimic
	{
		get
		{
			int valueOrDefault = _AllWallsLayerMask.GetValueOrDefault();
			if (!_AllWallsLayerMask.HasValue)
			{
				valueOrDefault = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("InvisibleWall")) | (1 << LayerMask.NameToLayer("MimicOnly"));
				_AllWallsLayerMask = valueOrDefault;
			}
			return _AllWallsLayerMask.Value;
		}
	}

	internal TableManager tableman { get; private set; }

	internal CameraManager cameraman { get; private set; }

	internal DebugConsole console { get; private set; }

	internal UIManager uiman { get; private set; }

	internal LegacyAudioManager legacyAudio { get; private set; }

	internal AudioManager audioman { get; private set; }

	internal VfxManager vfxman { get; private set; }

	internal InputManager inputman { get; private set; }

	internal NavManager navman { get; private set; }

	internal DynamicDataManager dynamicDataMan { get; private set; }

	internal VoiceManager voiceman { get; private set; }

	internal DataManager dataman { get; private set; }

	internal NetworkManagerV2 netman2 { get; private set; }

	internal APIRequestHandler apihandler { get; private set; }

	internal TimeUtil timeutil { get; private set; }

	internal APIUriMapper urimapper { get; private set; }

	internal VWorld? vworld { get; private set; }

	internal MsgPairGenerator msggenerator { get; private set; }

	internal DLAcademyManager dLAcademyManager { get; private set; }

	internal GlobalShaderValueManager globalShaderValueManager { get; private set; }

	public List<ulong> ReplayUploadWhiteList => _replayUploadWhiteList;

	[Conditional("INIT_PROFILING")]
	private void StartInitProfiling()
	{
		initProfilingStopwatch.Restart();
	}

	[Conditional("INIT_PROFILING")]
	private void EndInitProfiling(string stepName)
	{
		initProfilingStopwatch.Stop();
		initProfilingResult[stepName] = initProfilingStopwatch.ElapsedMilliseconds.ToString();
	}

	[Conditional("INIT_PROFILING")]
	private void ReportInitProfiling()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("=== Init Profiling Result ===\n");
		foreach (KeyValuePair<string, string> item in initProfilingResult)
		{
			stringBuilder.Append(item.Key + " : " + item.Value + " ms\n");
		}
		stringBuilder.Append("=== End of Init Profiling ===\n");
		Logger.RLog(stringBuilder.ToString());
	}

	private void Awake()
	{
		if (s != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			Logger.RError("Hub must be only one.");
			return;
		}
		UnityEngine.Debug.Log("[AwakeLogs] Hub.Awake ->");
		Application.quitting -= Quit;
		Application.quitting += Quit;
		Application.wantsToQuit -= WantToQuit;
		Application.wantsToQuit += WantToQuit;
		Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
		Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
		Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
		Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneLoaded += OnSceneLoaded;
		console = GetComponentInChildren<DebugConsole>();
		Logger.Init(delegate
		{
		});
		tableman = GetComponentInChildren<TableManager>();
		cameraman = GetComponentInChildren<CameraManager>();
		uiman = GetComponentInChildren<UIManager>();
		legacyAudio = GetComponentInChildren<LegacyAudioManager>();
		audioman = GetComponentInChildren<AudioManager>();
		vfxman = GetComponentInChildren<VfxManager>();
		inputman = GetComponentInChildren<InputManager>();
		navman = GetComponentInChildren<NavManager>();
		dynamicDataMan = GetComponentInChildren<DynamicDataManager>();
		dataman = GetComponentInChildren<DataManager>();
		netman2 = GetComponentInChildren<NetworkManagerV2>();
		apihandler = GetComponentInChildren<APIRequestHandler>();
		timeutil = GetComponentInChildren<TimeUtil>();
		urimapper = GetComponentInChildren<APIUriMapper>();
		msggenerator = GetComponentInChildren<MsgPairGenerator>();
		dLAcademyManager = GetComponentInChildren<DLAcademyManager>();
		globalShaderValueManager = GetComponentInChildren<GlobalShaderValueManager>();
		lcman = GetComponentInChildren<L10NManager>();
		gameSettingManager = GetComponentInChildren<GameSettingManager>();
		rpmman = GetComponentInChildren<RenderPerformanceManager>();
		residualObject = GetComponentInChildren<ResidualObjectManager>();
		replayManager = new ReplayManager();
		decalManamger = GetComponentInChildren<DecalManager>();
		thinPerfHud = GetComponentInChildren<ThinPerfHud>();
		flagImageLoader = GetComponentInChildren<FlagImageLoader>();
		tooltipManager = GetComponentInChildren<TooltipManager>();
		tramUpgrade = GetComponentInChildren<TramUpgradeManager>();
		steamInviteDispatcher = GetComponentInChildren<SteamInviteDispatcher>();
		LoadUploadWhiteListFromFile();
		pdata = new PersistentData();
		pdata.GameServerPort = gameConfig.gameSetting.ServerPort;
		pdata.commandLineArgs = Environment.GetCommandLineArgs();
		string[] commandLineArgs = pdata.commandLineArgs;
		for (int num = 0; num < commandLineArgs.Length; num++)
		{
			_ = commandLineArgs[num];
		}
		QualitySettings.maxQueuedFrames = 0;
		s = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		urimapper.Initialize();
		SetupCrashReportCustomMetadata();
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		Logger.RLog("[AwakeLogs] Hub.Awake <-");
	}

	private void Start()
	{
		Logger.RLog("[AwakeLogs] Hub.Start ->");
		Logger.RLog("[AwakeLogs] Hub.Start > voiceman");
		voiceman = UnityEngine.Object.Instantiate(prefab_VoiceManager, base.transform).GetComponent<VoiceManager>();
		Logger.RLog("[AwakeLogs] Hub.Start > kosManager");
		kosManager = UnityEngine.Object.Instantiate(prefab_KOSManager, base.transform).GetComponent<KOSManager>();
		Logger.RLog("[AwakeLogs] Hub.Start <-");
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
	{
		CrashReportHandler.SetUserMetadata("last_loaded_scene_name", scene.name);
	}

	private T GetOrCreateComponent<T>(T presetValue = null) where T : Component
	{
		if (presetValue != null)
		{
			return presetValue;
		}
		if (base.gameObject.TryGetComponent<T>(out var component))
		{
			return component;
		}
		return base.gameObject.AddComponent<T>();
	}

	public void CreateVWorld(int saveSlotID)
	{
		if (vworld != null)
		{
			vworld.OnDestroy();
			vworld = null;
		}
		vworld = new VWorld("0.2.7", saveSlotID);
		vworld.Initialize();
	}

	public void DestroyVWorld()
	{
		if (vworld != null)
		{
			vworld.OnDestroy();
			vworld = null;
		}
		RemoveAllHitCheckVisualizations();
	}

	private void OnDestroy()
	{
		try
		{
			voiceman.Shutdown();
		}
		catch
		{
		}
		try
		{
			vworld?.OnDestroy();
		}
		catch
		{
		}
		s = null;
	}

	private void FixedUpdate()
	{
		if (vworld != null)
		{
			vworld.Update();
		}
	}

	private void OnDrawGizmos()
	{
		DrawHitCheckVisualizations();
	}

	public static Keyboard GetKeyboard()
	{
		return Keyboard.current;
	}

	public static Mouse GetMouse()
	{
		return Mouse.current;
	}

	public static Gamepad GetGamepad()
	{
		return Gamepad.current;
	}

	public static void LoadScene(string sceneName)
	{
		Logger.RLog("LoadScene: " + sceneName);
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	public static void LoadSceneAdditive(string sceneName)
	{
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
	}

	public void RegisterOnExitPlayMode(Action callback)
	{
		onExitPlaymode.Add(callback);
	}

	private static void Quit()
	{
	}

	private static bool WantToQuit()
	{
		ModHelper.SetTimingCallback(null);
		QuittingProcess.isQuitting = true;
		Logger.WantToQuit();
		LoadScene("QuitScene");
		return true;
	}

	public static void ApplicationQuit()
	{
		Logger.RLog("ApplicationQuit");
		Application.Quit();
	}

	public void ToggleHitCheckVisualization(bool show)
	{
		_showHitChecks = show;
		if (!show)
		{
			_currentHitChecks.Clear();
		}
	}

	internal void UpdateHitCheckVisualizations(int actorID, List<HitCheckDrawInfo> newHitChecks)
	{
		if (!_showHitChecks)
		{
			return;
		}
		RemoveHitCheckVisualizations(actorID);
		foreach (HitCheckDrawInfo newHitCheck in newHitChecks)
		{
			HitCheckDrawData item = new HitCheckDrawData(newHitCheck.actorID, newHitCheck);
			_currentHitChecks.Add(item);
		}
	}

	public void RemoveHitCheckVisualizations(int actorID)
	{
		if (_showHitChecks)
		{
			_currentHitChecks.RemoveAll((HitCheckDrawData x) => x.ActorID == actorID);
		}
	}

	public void RemoveAllHitCheckVisualizations()
	{
		_currentHitChecks.Clear();
	}

	private void DrawHitCheckVisualizations()
	{
		if (!_showHitChecks || _currentHitChecks.Count == 0)
		{
			return;
		}
		foreach (HitCheckDrawData currentHitCheck in _currentHitChecks)
		{
			DrawHitCheck(currentHitCheck);
		}
	}

	private void DrawHitCheck(HitCheckDrawData data)
	{
		Color color = new Color(1f, 0f, 0f, 1f);
		Color color2 = new Color(0f, 1f, 0f, 1f);
		Color color3 = new Color(0f, 0f, 1f, 1f);
		Color cyan = Color.cyan;
		Color yellow = Color.yellow;
		switch (data.ShapeType)
		{
		case HitCheckShapeType.Cube:
			DebugDraw_Box(data.Position, data.Rotation, data.Extent, color);
			break;
		case HitCheckShapeType.Capsule:
			DrawCapsule(data.Position, data.Rotation, data.Length, data.Radius, color3);
			break;
		case HitCheckShapeType.Sphere:
			DrawSphere(data.Position, data.Rotation, data.Radius, color2);
			break;
		case HitCheckShapeType.Fan:
			DrawFan(data.Position, data.Rotation, data.InnerRadius, data.OuterRadius, data.Height, data.Angle, cyan);
			break;
		case HitCheckShapeType.Torus:
			DrawTorus(data.Position, data.Rotation, data.InnerRadius, data.OuterRadius, data.Height, yellow);
			break;
		}
	}

	private void DrawCapsule(Vector3 centerPosition, Rotator rotation, float length, float radius, Color color)
	{
		Quaternion quaternion = rotation.ToQuaternion();
		DrawCapsule(centerPosition, quaternion, length, radius, color);
	}

	private void DrawCapsule(Vector3 centerPosition, Quaternion quaternion, float length, float radius, Color color, int segments = 8)
	{
		_ = quaternion * Vector3.up;
		Vector3 vector = quaternion * Vector3.right;
		Vector3 vector2 = quaternion * Vector3.forward;
		Vector3 vector3 = Vector3.up * length * 0.5f;
		Vector3 vector4 = centerPosition + quaternion * vector3;
		Vector3 vector5 = centerPosition + quaternion * -vector3;
		DebugDraw_Octahedron(vector4, radius, color, 1f);
		DebugDraw_Octahedron(vector5, radius, color, 1f);
		for (int i = 0; i < segments; i++)
		{
			float f = MathF.PI * 2f * (float)i / (float)segments;
			float f2 = MathF.PI * 2f * (float)(i + 1) / (float)segments;
			Vector3 vector6 = Mathf.Cos(f) * vector + Mathf.Sin(f) * vector2;
			Vector3 vector7 = Mathf.Cos(f2) * vector + Mathf.Sin(f2) * vector2;
			Vector3 start = vector5 + vector6 * radius;
			Vector3 end = vector5 + vector7 * radius;
			Vector3 vector8 = vector4 + vector6 * radius;
			Vector3 end2 = vector4 + vector7 * radius;
			DebugDraw_Line(start, end, color, 1f);
			DebugDraw_Line(vector8, end2, color, 1f);
			DebugDraw_Line(start, vector8, color, 1f);
		}
		float num = radius * 2f;
		Vector3 end3 = vector4 + vector2 * num;
		DebugDraw_Arrow(vector4, end3, color, 1f);
	}

	private void DrawCapsule2(Vector3 centerPostion, Quaternion quaternion, float length, float radius, int segments, Color color)
	{
		Vector3 vector = quaternion * Vector3.up;
		Vector3 vector2 = quaternion * Vector3.right;
		Vector3 vector3 = quaternion * Vector3.forward;
		Vector3 vector4 = centerPostion + vector * (length / 2f);
		Vector3 vector5 = centerPostion - vector * (length / 2f);
		for (int i = 0; i < segments; i++)
		{
			float f = MathF.PI * 2f * (float)i / (float)segments;
			float f2 = MathF.PI * 2f * (float)(i + 1) / (float)segments;
			Vector3 vector6 = Mathf.Cos(f) * vector2 + Mathf.Sin(f) * vector3;
			Vector3 vector7 = Mathf.Cos(f2) * vector2 + Mathf.Sin(f2) * vector3;
			Vector3 start = vector5 + vector6 * radius;
			Vector3 end = vector5 + vector7 * radius;
			Vector3 vector8 = vector4 + vector6 * radius;
			Vector3 end2 = vector4 + vector7 * radius;
			DebugDraw_Line(start, end, color, 0.05f);
			DebugDraw_Line(vector8, end2, color, 0.05f);
			DebugDraw_Line(start, vector8, color, 0.05f);
		}
		int num = segments / 2;
		for (int j = 1; j < num; j++)
		{
			float f3 = MathF.PI / 2f * (float)j / (float)num;
			float num2 = Mathf.Sin(f3);
			float num3 = Mathf.Cos(f3);
			for (int k = 0; k < segments; k++)
			{
				float f4 = MathF.PI * 2f * (float)k / (float)segments;
				float f5 = MathF.PI * 2f * (float)(k + 1) / (float)segments;
				Vector3 vector9 = (Mathf.Cos(f4) * vector2 + Mathf.Sin(f4) * vector3) * num3 + vector * num2;
				Vector3 vector10 = (Mathf.Cos(f5) * vector2 + Mathf.Sin(f5) * vector3) * num3 + vector * num2;
				Vector3 start2 = vector4 + vector9 * radius;
				Vector3 end3 = vector4 + vector10 * radius;
				Vector3 start3 = vector5 - vector9 * radius;
				Vector3 end4 = vector5 - vector10 * radius;
				DebugDraw_Line(start2, end3, color, 0.05f);
				DebugDraw_Line(start3, end4, color, 0.05f);
			}
		}
		float num4 = radius * 2f;
		Vector3 end5 = vector4 + vector3 * num4;
		DebugDraw_Arrow(vector4, end5, color, 0.05f);
	}

	private void DrawCylinder(Vector3 centerPosition, Quaternion quaternion, float height, float radius, Color color, int segments = 8)
	{
		Vector3 vector = quaternion * Vector3.up;
		Vector3 vector2 = quaternion * Vector3.right;
		Vector3 vector3 = quaternion * Vector3.forward;
		Vector3 vector4 = vector * (height / 2f);
		Vector3 vector5 = centerPosition - vector4;
		Vector3 vector6 = centerPosition + vector4;
		for (int i = 0; i < segments; i++)
		{
			float f = MathF.PI * 2f * (float)i / (float)segments;
			float f2 = MathF.PI * 2f * (float)(i + 1) / (float)segments;
			Vector3 vector7 = Mathf.Cos(f) * vector2 + Mathf.Sin(f) * vector3;
			Vector3 vector8 = Mathf.Cos(f2) * vector2 + Mathf.Sin(f2) * vector3;
			Vector3 start = vector5 + vector7 * radius;
			Vector3 end = vector5 + vector8 * radius;
			Vector3 vector9 = vector6 + vector7 * radius;
			Vector3 end2 = vector6 + vector8 * radius;
			DebugDraw_Line(start, end, color, 0.05f);
			DebugDraw_Line(vector9, end2, color, 0.05f);
			DebugDraw_Line(start, vector9, color, 0.05f);
		}
		float num = radius * 2f;
		Vector3 end3 = vector6 + vector3 * num;
		DebugDraw_Arrow(vector6, end3, color, 0.05f);
	}

	private void DrawSphere(Vector3 centerPosition, Rotator rotation, float radius, Color color)
	{
		Quaternion quaternion = rotation.ToQuaternion();
		DrawSphere(centerPosition, quaternion, radius, color);
	}

	private void DrawSphere(Vector3 centerPosition, Quaternion quaternion, float radius, Color color, int segments = 8)
	{
		for (int i = 0; i < segments; i++)
		{
			float f = MathF.PI * ((float)i / (float)segments);
			float f2 = MathF.PI * ((float)(i + 1) / (float)segments);
			float y = Mathf.Cos(f) * radius;
			float y2 = Mathf.Cos(f2) * radius;
			float num = Mathf.Sin(f) * radius;
			float num2 = Mathf.Sin(f2) * radius;
			for (int j = 0; j < segments; j++)
			{
				float f3 = MathF.PI * 2f * ((float)j / (float)segments);
				float f4 = MathF.PI * 2f * ((float)(j + 1) / (float)segments);
				Vector3 vector = new Vector3(num * Mathf.Cos(f3), y, num * Mathf.Sin(f3));
				Vector3 vector2 = new Vector3(num * Mathf.Cos(f4), y, num * Mathf.Sin(f4));
				Vector3 vector3 = new Vector3(num2 * Mathf.Cos(f3), y2, num2 * Mathf.Sin(f3));
				Vector3 vector4 = new Vector3(num2 * Mathf.Cos(f4), y2, num2 * Mathf.Sin(f4));
				vector = quaternion * vector + centerPosition;
				vector2 = quaternion * vector2 + centerPosition;
				vector3 = quaternion * vector3 + centerPosition;
				vector4 = quaternion * vector4 + centerPosition;
				UnityEngine.Debug.DrawLine(vector, vector2, color);
				UnityEngine.Debug.DrawLine(vector, vector3, color);
				if (i == segments - 1)
				{
					UnityEngine.Debug.DrawLine(vector3, vector4, color);
				}
				if (j == segments - 1)
				{
					UnityEngine.Debug.DrawLine(vector2, vector4, color);
				}
			}
		}
	}

	private void DrawFan(Vector3 centerPosition, Rotator rotation, float innerRadius, float outerRadius, float height, float angle, Color color, int segments = 16)
	{
		if (segments <= 0 || innerRadius > outerRadius)
		{
			return;
		}
		Quaternion quaternion = rotation.ToQuaternion();
		Vector3 vector = quaternion * Vector3.up;
		Vector3 vector2 = quaternion * Vector3.forward;
		Vector3 vector3 = quaternion * Vector3.right;
		Vector3 vector4 = vector * (height * 0.5f);
		Vector3 vector5 = centerPosition - vector4;
		Vector3 vector6 = centerPosition + vector4;
		float num = angle * (MathF.PI / 180f) * 0.5f;
		for (int i = 0; i <= 1; i++)
		{
			Vector3 vector7 = ((i == 0) ? vector5 : vector6);
			for (int j = 0; j < segments; j++)
			{
				float t = (float)j / (float)segments;
				float t2 = (float)(j + 1) / (float)segments;
				float f = Mathf.Lerp(0f - num, num, t);
				float f2 = Mathf.Lerp(0f - num, num, t2);
				Vector3 vector8 = Mathf.Cos(f) * vector2 + Mathf.Sin(f) * vector3;
				Vector3 vector9 = Mathf.Cos(f2) * vector2 + Mathf.Sin(f2) * vector3;
				Vector3 start = vector7 + vector8 * innerRadius;
				Vector3 end = vector7 + vector9 * innerRadius;
				UnityEngine.Debug.DrawLine(start, end, color);
			}
			for (int k = 0; k < segments; k++)
			{
				float t3 = (float)k / (float)segments;
				float t4 = (float)(k + 1) / (float)segments;
				float f3 = Mathf.Lerp(0f - num, num, t3);
				float f4 = Mathf.Lerp(0f - num, num, t4);
				Vector3 vector10 = Mathf.Cos(f3) * vector2 + Mathf.Sin(f3) * vector3;
				Vector3 vector11 = Mathf.Cos(f4) * vector2 + Mathf.Sin(f4) * vector3;
				Vector3 start2 = vector7 + vector10 * outerRadius;
				Vector3 end2 = vector7 + vector11 * outerRadius;
				UnityEngine.Debug.DrawLine(start2, end2, color);
			}
			Vector3 vector12 = Mathf.Cos(0f - num) * vector2 + Mathf.Sin(0f - num) * vector3;
			Vector3 vector13 = Mathf.Cos(num) * vector2 + Mathf.Sin(num) * vector3;
			UnityEngine.Debug.DrawLine(vector7 + vector12 * innerRadius, vector7 + vector12 * outerRadius, color);
			UnityEngine.Debug.DrawLine(vector7 + vector13 * innerRadius, vector7 + vector13 * outerRadius, color);
		}
		for (int l = 0; l <= segments; l++)
		{
			float t5 = (float)l / (float)segments;
			float f5 = Mathf.Lerp(0f - num, num, t5);
			Vector3 vector14 = Mathf.Cos(f5) * vector2 + Mathf.Sin(f5) * vector3;
			Vector3 start3 = vector5 + vector14 * innerRadius;
			Vector3 end3 = vector6 + vector14 * innerRadius;
			Vector3 start4 = vector5 + vector14 * outerRadius;
			Vector3 end4 = vector6 + vector14 * outerRadius;
			UnityEngine.Debug.DrawLine(start3, end3, color);
			UnityEngine.Debug.DrawLine(start4, end4, color);
		}
		float num2 = outerRadius * 0.5f;
		Vector3 end5 = vector6 + vector2 * num2;
		DebugDraw_Arrow(vector6, end5, color);
	}

	private void DrawTorus(Vector3 centerPosition, Rotator rotation, float innerRadius, float outerRadius, float height, Color color, int segments = 16)
	{
		Quaternion quaternion = rotation.ToQuaternion();
		Vector3 vector = quaternion * Vector3.up;
		Vector3 vector2 = quaternion * Vector3.forward;
		Vector3 vector3 = quaternion * Vector3.right;
		Vector3 vector4 = vector * (height * 0.5f);
		Vector3 vector5 = centerPosition - vector4;
		Vector3 vector6 = centerPosition + vector4;
		for (int i = 0; i <= 1; i++)
		{
			Vector3 vector7 = ((i == 0) ? vector5 : vector6);
			for (int j = 0; j < segments; j++)
			{
				float f = MathF.PI * 2f * (float)j / (float)segments;
				float f2 = MathF.PI * 2f * (float)(j + 1) / (float)segments;
				Vector3 vector8 = Mathf.Cos(f) * vector2 + Mathf.Sin(f) * vector3;
				Vector3 vector9 = Mathf.Cos(f2) * vector2 + Mathf.Sin(f2) * vector3;
				Vector3 start = vector7 + vector8 * innerRadius;
				Vector3 end = vector7 + vector9 * innerRadius;
				UnityEngine.Debug.DrawLine(start, end, color);
			}
			for (int k = 0; k < segments; k++)
			{
				float f3 = MathF.PI * 2f * (float)k / (float)segments;
				float f4 = MathF.PI * 2f * (float)(k + 1) / (float)segments;
				Vector3 vector10 = Mathf.Cos(f3) * vector2 + Mathf.Sin(f3) * vector3;
				Vector3 vector11 = Mathf.Cos(f4) * vector2 + Mathf.Sin(f4) * vector3;
				Vector3 start2 = vector7 + vector10 * outerRadius;
				Vector3 end2 = vector7 + vector11 * outerRadius;
				UnityEngine.Debug.DrawLine(start2, end2, color);
			}
		}
		for (int l = 0; l < segments; l++)
		{
			float f5 = MathF.PI * 2f * (float)l / (float)segments;
			Vector3 vector12 = Mathf.Cos(f5) * vector2 + Mathf.Sin(f5) * vector3;
			Vector3 start3 = vector5 + vector12 * innerRadius;
			Vector3 end3 = vector6 + vector12 * innerRadius;
			Vector3 start4 = vector5 + vector12 * outerRadius;
			Vector3 end4 = vector6 + vector12 * outerRadius;
			UnityEngine.Debug.DrawLine(start3, end3, color);
			UnityEngine.Debug.DrawLine(start4, end4, color);
		}
		int num = 8;
		for (int m = 0; m < num; m++)
		{
			float f6 = MathF.PI * 2f * (float)m / (float)num;
			Vector3 vector13 = Mathf.Cos(f6) * vector2 + Mathf.Sin(f6) * vector3;
			for (int n = 0; n < segments / 2; n++)
			{
				float t = (float)n / (float)(segments / 2);
				float t2 = (float)(n + 1) / (float)(segments / 2);
				Vector3 start5 = vector5 + vector13 * Mathf.Lerp(innerRadius, outerRadius, t);
				Vector3 end5 = vector5 + vector13 * Mathf.Lerp(innerRadius, outerRadius, t2);
				UnityEngine.Debug.DrawLine(start5, end5, color);
				start5 = vector6 + vector13 * Mathf.Lerp(innerRadius, outerRadius, t);
				end5 = vector6 + vector13 * Mathf.Lerp(innerRadius, outerRadius, t2);
				UnityEngine.Debug.DrawLine(start5, end5, color);
			}
		}
		float num2 = outerRadius * 0.5f;
		Vector3 end6 = vector6 + vector2 * num2;
		DebugDraw_Arrow(vector6, end6, color);
	}

	public static void DebugDraw_Line(Vector3 start, Vector3 end, Color color, float duration = 0f)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration);
	}

	public static void DebugDraw_Octahedron(Vector3 position, float radius, Color color, float duration = 0f)
	{
		UnityEngine.Debug.DrawLine(position + new Vector3(0f, radius, 0f), position + new Vector3(0f - radius, 0f, 0f), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f - radius, 0f, 0f), position + new Vector3(0f, 0f - radius, 0f), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f, 0f - radius, 0f), position + new Vector3(radius, 0f, 0f), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(radius, 0f, 0f), position + new Vector3(0f, radius, 0f), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f - radius, 0f, 0f), position + new Vector3(0f, 0f, 0f - radius), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f, 0f, 0f - radius), position + new Vector3(radius, 0f, 0f), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(radius, 0f, 0f), position + new Vector3(0f, 0f, radius), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f, 0f, radius), position + new Vector3(0f - radius, 0f, 0f), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f, radius, 0f), position + new Vector3(0f, 0f, 0f - radius), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f, 0f, 0f - radius), position + new Vector3(0f, 0f - radius, 0f), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f, 0f - radius, 0f), position + new Vector3(0f, 0f, radius), color, duration);
		UnityEngine.Debug.DrawLine(position + new Vector3(0f, 0f, radius), position + new Vector3(0f, radius, 0f), color, duration);
	}

	public static void DebugDraw_Box(Vector3 center, Rotator rotation, Vector3 size, Color color, float duration = 0f)
	{
		Vector3 vector = size * 0.5f;
		Vector3[] array = new Vector3[8]
		{
			new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z),
			new Vector3(vector.x, 0f - vector.y, 0f - vector.z),
			new Vector3(vector.x, vector.y, 0f - vector.z),
			new Vector3(0f - vector.x, vector.y, 0f - vector.z),
			new Vector3(0f - vector.x, 0f - vector.y, vector.z),
			new Vector3(vector.x, 0f - vector.y, vector.z),
			new Vector3(vector.x, vector.y, vector.z),
			new Vector3(0f - vector.x, vector.y, vector.z)
		};
		Vector3[] array2 = new Vector3[8];
		Quaternion quaternion = rotation.ToQuaternion();
		for (int i = 0; i < 8; i++)
		{
			array2[i] = center + quaternion * array[i];
		}
		UnityEngine.Debug.DrawLine(array2[0], array2[1], color, duration);
		UnityEngine.Debug.DrawLine(array2[1], array2[2], color, duration);
		UnityEngine.Debug.DrawLine(array2[2], array2[3], color, duration);
		UnityEngine.Debug.DrawLine(array2[3], array2[0], color, duration);
		UnityEngine.Debug.DrawLine(array2[4], array2[5], color, duration);
		UnityEngine.Debug.DrawLine(array2[5], array2[6], color, duration);
		UnityEngine.Debug.DrawLine(array2[6], array2[7], color, duration);
		UnityEngine.Debug.DrawLine(array2[7], array2[4], color, duration);
		UnityEngine.Debug.DrawLine(array2[0], array2[4], color, duration);
		UnityEngine.Debug.DrawLine(array2[1], array2[5], color, duration);
		UnityEngine.Debug.DrawLine(array2[2], array2[6], color, duration);
		UnityEngine.Debug.DrawLine(array2[3], array2[7], color, duration);
	}

	public static void DebugDraw_Arrow(Vector3 start, Vector3 end, Color color, float duration = 0f)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration);
		if (!(Vector3.Distance(start, end) < 0.01f))
		{
			Vector3 forward = end - start;
			Vector3 vector = Quaternion.LookRotation(forward) * Quaternion.Euler(0f, 210f, 0f) * Vector3.forward;
			Vector3 vector2 = Quaternion.LookRotation(forward) * Quaternion.Euler(0f, 150f, 0f) * Vector3.forward;
			UnityEngine.Debug.DrawRay(end, vector * 0.5f, color, duration);
			UnityEngine.Debug.DrawRay(end, vector2 * 0.5f, color, duration);
		}
	}

	public static void DebugDraw_Path(List<Vector3> path, Color color, float duration = 0f)
	{
		if (path.Count >= 2)
		{
			for (int i = 0; i < path.Count - 1; i++)
			{
				DebugDraw_Arrow(path[i], path[i + 1], color, duration);
			}
		}
	}

	public bool CheckPosIsIndoor(Vector3 position)
	{
		if (s == null || s.pdata == null || s.pdata.main == null)
		{
			return false;
		}
		if (!(s.pdata.main is GamePlayScene gamePlayScene))
		{
			return false;
		}
		return gamePlayScene.CheckPosIsIndoor(in position);
	}

	public static void DebugDraw_line_long(Vector3 start, Vector3 end, float length, Color color, float duration = 0f)
	{
		Vector3 vector = end - start;
		vector.Normalize();
		UnityEngine.Debug.DrawLine(start, start + vector * length, color, duration);
	}

	internal static void DrawGizmo_Arrow(Vector3 from, Vector3 to, Color color)
	{
		Gizmos.color = color;
		Gizmos.DrawLine(from, to);
		Vector3 normalized = (from - to).normalized;
		if (normalized.x == 0f)
		{
			Gizmos.DrawRay(to, Quaternion.Euler(30f, 0f, 0f) * normalized * 0.1f);
			Gizmos.DrawRay(to, Quaternion.Euler(-30f, 0f, 0f) * normalized * 0.1f);
		}
		else
		{
			Gizmos.DrawRay(to, Quaternion.Euler(0f, 30f, 0f) * normalized * 0.1f);
			Gizmos.DrawRay(to, Quaternion.Euler(0f, -30f, 0f) * normalized * 0.1f);
		}
	}

	internal static void DrawDebugTextOnActor(int actorID, string text)
	{
		if (!(s == null) && s.pdata != null && !(s.pdata.main == null))
		{
			ProtoActor actorByActorID = s.pdata.main.GetActorByActorID(actorID);
			if (!(actorByActorID == null))
			{
				actorByActorID.ShowDebugText(text);
			}
		}
	}

	internal static void HideDebugTextOnActor(int actorID)
	{
		if (!(s == null) && s.pdata != null && !(s.pdata.main == null))
		{
			ProtoActor actorByActorID = s.pdata.main.GetActorByActorID(actorID);
			if (!(actorByActorID == null))
			{
				actorByActorID.HideDebugText();
			}
		}
	}

	internal static string GetL10NText(string key, params object[] formattingArgs)
	{
		if (s == null || s.lcman == null)
		{
			return "|L10N error|";
		}
		return s.lcman.GetText(key, formattingArgs);
	}

	private void LoadUploadWhiteListFromFile()
	{
		_replayUploadWhiteList.Clear();
		string path = Path.Combine(Application.persistentDataPath, "ReplayUploadWhiteList.txt");
		if (!File.Exists(path))
		{
			return;
		}
		try
		{
			string[] array = File.ReadAllLines(path);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (!string.IsNullOrEmpty(text))
				{
					if (ulong.TryParse(text, out var result))
					{
						_replayUploadWhiteList.Add(result);
					}
					else
					{
						Logger.RLog("Failed to parse line as ulong: " + text);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.RError("Error loading ReplayUploadWhiteList: " + ex.Message);
		}
	}

	private void SetupCrashReportCustomMetadata()
	{
		CrashReportHandler.SetUserMetadata("is_editor", "false");
		CrashReportHandler.SetUserMetadata("is_steam", "true");
	}
}
