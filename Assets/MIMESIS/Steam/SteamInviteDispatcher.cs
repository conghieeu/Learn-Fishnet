using System;
using System.Collections;
using System.Collections.Generic;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SteamInviteDispatcher : MonoBehaviour
{
	public const string VERSION_KEY = "Version";

	public const string LOCALE_KEY = "Locale";

	public const string IS_PUBLIC_KEY = "PublicRoom";

	public const string SERVER_PORT_KEY = "serverPort";

	public const string LOBBY_NAME_KEY = "LobbyName";

	public const string APPLIED_STEAM_INVENTORY_ITEM_IDS_KEY = "AppliedSteamInventoryItemDefIDs";

	public const string CYCLE_KEY = "Cycle";

	public const string REPAIR_STATUS_KEY = "RepairStatus";

	private bool isJoining;

	private Callback<GameLobbyJoinRequested_t> m_GameLobbyJoinRequested;

	private Callback<GameRichPresenceJoinRequested_t> m_GameRichPresenceJoinRequested;

	private Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;

	protected Callback<LobbyEnter_t> lobbyEnterCallback;

	protected Callback<LobbyCreated_t> lobbyCreatedCallback;

	private CSteamID targetLobbyID = CSteamID.Nil;

	public MainMenu lobbyMain;

	[HideInInspector]
	public bool joinOnce = true;

	[HideInInspector]
	public bool joinSuccess;

	public CSteamID joinedLobbyID = CSteamID.Nil;

	public string tempIDWhoInviteMe = " ";

	private int createLobbyCount;

	[HideInInspector]
	public bool isFirstLobby = true;

	private bool _inited;

	private bool _quitting;

	private int tempServerPort = 22220;

	private bool isRandomMatching;

	private bool isRandomMatchingTimeout;

	private bool isRoomSearching;

	private float randomMatchStartTime;

	private const float RANDOM_MATCH_TIMEOUT = 30f;

	private int randomMatchAttempts;

	private const int MAX_RANDOM_MATCH_ATTEMPTS = 10;

	private CallResult<LobbyMatchList_t> m_LobbyMatchList;

	private List<CSteamID> availableLobbies = new List<CSteamID>();

	private int currentLobbyAttemptIndex;

	[HideInInspector]
	public string lobbyName = "";

	public int refreshTime;

	public const int REFRESH_TIME = 10;

	public UIPrefab_PublicRoomList roomListUI;

	private int requestLobbyListStep;

	private bool isEditorMode;

	private bool isRequestPublicJoinLobby;

	private Action publicJoinLobbyCallback;

	private Coroutine setLobbyPublicCoroutine;

	public bool isLobbyCreated { get; private set; }

	public string HostMatchCode => joinedLobbyID.ToString();

	private void Start()
	{
		if (!SteamManager.Initialized)
		{
			base.enabled = false;
			Logger.RWarn("Steam not initialized. SteamInviteDispatcher disabled.");
			return;
		}
		m_LobbyMatchList = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchListReceived);
		m_GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
		m_GameRichPresenceJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(OnGameRichPresenceJoinRequested);
		lobbyEnterCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
		lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
		_inited = true;
	}

	private void OnApplicationQuit()
	{
		if (Hub.s.pdata.ClientMode == NetworkClientMode.Host)
		{
			SetLobbyPublic(isPublic: false);
		}
		SteamMatchmaking.LeaveLobby(joinedLobbyID);
	}

	public void DisplayStatus(string statusString)
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.SetRichPresence("status", statusString);
			SteamFriends.SetRichPresence("steam_display", statusString);
		}
	}

	private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
	{
		if (!(SceneManager.GetActiveScene().name != "MainMenuScene") && !joinSuccess)
		{
			JoinSteamMatchmakingLobby(callback.m_steamIDFriend.ToString(), callback.m_steamIDLobby.ToString());
		}
	}

	private void JoinSteamMatchmakingLobby(string steamID, string lobbyId)
	{
		if (Hub.s.uiman.inviteLoadingUI == null)
		{
			Hub.s.uiman.inviteLoadingUI = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_dialogueBox>(Hub.s.uiman.prefab_Invite_Loading, eUIHeight.OverTheTop);
		}
		Hub.s.uiman.inviteLoadingUI.Show();
		StartCoroutine(TimeoutJoinLobby());
		tempIDWhoInviteMe = steamID;
		if (ulong.TryParse(lobbyId, out var result))
		{
			SteamMatchmaking.JoinLobby(new CSteamID(result));
			return;
		}
		Hub.s.uiman.inviteLoadingUI.Hide();
		Hub.s.uiman.ShowConnectionFailed("INVITE_DO_NOT_ENTER_ROOM_DESTROYED", new string[1] { "0" });
	}

	private void OnGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t callback)
	{
		if (!(SceneManager.GetActiveScene().name != "MainMenuScene") && !joinSuccess)
		{
			if (Hub.s.uiman.inviteLoadingUI != null)
			{
				Hub.s.uiman.inviteLoadingUI = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_dialogueBox>(Hub.s.uiman.prefab_Invite_Loading, eUIHeight.OverTheTop);
			}
			Hub.s.uiman.inviteLoadingUI.Show();
			StartCoroutine(TimeoutJoinLobby());
			tempIDWhoInviteMe = callback.m_steamIDFriend.ToString();
			if (ulong.TryParse(callback.m_rgchConnect, out var result))
			{
				SteamMatchmaking.JoinLobby(new CSteamID(result));
				return;
			}
			Hub.s.uiman.inviteLoadingUI.Hide();
			Hub.s.uiman.ShowConnectionFailed("INVITE_DO_NOT_ENTER_ROOM_DESTROYED", new string[1] { "0" });
		}
	}

	private IEnumerator TimeoutJoinLobby()
	{
		yield return new WaitForSeconds(20f);
		if (isJoining)
		{
			Logger.RLog("로비 입장 대기 시간 초과");
			if (Hub.s.uiman.inviteLoadingUI != null)
			{
				Hub.s.uiman.inviteLoadingUI?.Hide();
			}
		}
	}

	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		if (!isRandomMatching && isJoining)
		{
			return;
		}
		if (!isRandomMatching)
		{
			isJoining = true;
		}
		EChatRoomEnterResponse eChatRoomEnterResponse = (EChatRoomEnterResponse)callback.m_EChatRoomEnterResponse;
		if (eChatRoomEnterResponse == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
		{
			isRoomSearching = false;
			joinedLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
			lobbyName = SteamMatchmaking.GetLobbyData(joinedLobbyID, "LobbyName");
			CSteamID lobbyOwner = SteamMatchmaking.GetLobbyOwner(joinedLobbyID);
			string lobbyData = SteamMatchmaking.GetLobbyData(joinedLobbyID, "serverPort");
			if (!int.TryParse(lobbyData, out tempServerPort))
			{
				Logger.RError("failed to parse port : " + lobbyData);
				tempServerPort = Hub.s.gameConfig.gameSetting.ServerPort;
			}
			string lobbyData2 = SteamMatchmaking.GetLobbyData(joinedLobbyID, "Version");
			string text = "EA0.2.7";
			if (lobbyData2 != text)
			{
				Logger.RError("version mismatch - host: " + lobbyData2 + ", current: " + text);
				SteamMatchmaking.LeaveLobby(joinedLobbyID);
				if (isRandomMatching)
				{
					TryJoinNextRandomLobby();
				}
				isJoining = false;
				return;
			}
			if (lobbyOwner.ToString() != SteamUser.GetSteamID().ToString())
			{
				Logger.RLog("entered - Version: " + lobbyData2 + ", Locale: " + SteamMatchmaking.GetLobbyData(joinedLobbyID, "Locale"));
				joinSuccess = true;
				if (isRandomMatching)
				{
					Logger.RLog("success match!");
					OnRandomMatchingSuccess();
				}
				JoinGameRoom(lobbyOwner.ToString());
			}
			isJoining = false;
			isRandomMatching = false;
			return;
		}
		Logger.RError($"lobby enter failed. code: {eChatRoomEnterResponse}");
		if (isRandomMatching)
		{
			TryJoinNextRandomLobby();
			return;
		}
		if (Hub.s == null || Hub.s.uiman == null)
		{
			Logger.RError("Hub.s or Hub.s.uiman is null, cannot show invite fail UI");
			return;
		}
		if (Hub.s.uiman.inviteLoadingUI != null)
		{
			Hub.s.uiman.inviteLoadingUI?.Hide();
		}
		isJoining = false;
		switch (eChatRoomEnterResponse)
		{
		case EChatRoomEnterResponse.k_EChatRoomEnterResponseDoesntExist:
		{
			UIManager uiman3 = Hub.s.uiman;
			string[] array3 = new string[1];
			int eChatRoomEnterResponse2 = (int)callback.m_EChatRoomEnterResponse;
			array3[0] = eChatRoomEnterResponse2.ToString();
			uiman3.ShowConnectionFailed("INVITE_DO_NOT_ENTER_ROOM_DESTROYED", array3);
			break;
		}
		case EChatRoomEnterResponse.k_EChatRoomEnterResponseFull:
		{
			UIManager uiman2 = Hub.s.uiman;
			string[] array2 = new string[1];
			int eChatRoomEnterResponse2 = (int)callback.m_EChatRoomEnterResponse;
			array2[0] = eChatRoomEnterResponse2.ToString();
			uiman2.ShowConnectionFailed("INVITE_DO_NOT_ENTER_ROOM_FULL", array2);
			break;
		}
		default:
		{
			UIManager uiman = Hub.s.uiman;
			string[] array = new string[1];
			int eChatRoomEnterResponse2 = (int)callback.m_EChatRoomEnterResponse;
			array[0] = eChatRoomEnterResponse2.ToString();
			uiman.ShowConnectionFailed("INVITE_DO_NOT_ENTER_COMMON", array);
			break;
		}
		}
	}

	private void OnRandomMatchingSuccess()
	{
		isRandomMatching = false;
		randomMatchAttempts = 0;
		availableLobbies.Clear();
	}

	private void OnRandomMatchingFailed(string errorCode)
	{
		isRandomMatching = false;
		randomMatchAttempts = 0;
		availableLobbies.Clear();
		if (Hub.s.uiman.inviteLoadingUI != null)
		{
			Hub.s.uiman.inviteLoadingUI.Hide();
		}
		Hub.s.uiman.ShowConnectionFailed(errorCode, null);
	}

	public void CancelRandomMatching()
	{
		if (isRandomMatching)
		{
			isRoomSearching = false;
			isRandomMatching = false;
			randomMatchAttempts = 0;
			availableLobbies.Clear();
			if (Hub.s.uiman.inviteLoadingUI != null)
			{
				Hub.s.uiman.inviteLoadingUI.Hide();
			}
		}
	}

	public string GetCurrentLocale()
	{
		return SteamUtils.GetIPCountry();
	}

	public void SetLobbyName(string lobbyName)
	{
		if (!isLobbyCreated)
		{
			Logger.RError("lobby is not created");
		}
		else
		{
			SteamMatchmaking.SetLobbyData(joinedLobbyID, "LobbyName", lobbyName);
		}
	}

	public void SetLobbyPublic(bool isPublic)
	{
		if (!isLobbyCreated)
		{
			Logger.RError("lobby is not created");
			return;
		}
		if (setLobbyPublicCoroutine != null)
		{
			StopCoroutine(setLobbyPublicCoroutine);
		}
		setLobbyPublicCoroutine = StartCoroutine(SetLobbyPublicCoroutine(isPublic));
	}

	private IEnumerator SetLobbyPublicCoroutine(bool isPublic)
	{
		yield return new WaitForSeconds(5f);
		bool flag;
		try
		{
			flag = Hub.s.uiman.inGameMenu.UE_PublicRoomToggle.GetComponent<Toggle>().isOn;
		}
		catch
		{
			flag = isPublic;
		}
		int cycleCount = Hub.s.pdata.CycleCount;
		int dayCount = Hub.s.pdata.DayCount;
		bool repaired = Hub.s.pdata.Repaired;
		if (flag)
		{
			if (cycleCount == 1)
			{
				if (dayCount == 1)
				{
					SteamMatchmaking.SetLobbyData(joinedLobbyID, "RepairStatus", "0");
				}
				else if (repaired)
				{
					SteamMatchmaking.SetLobbyData(joinedLobbyID, "RepairStatus", "2");
				}
				else
				{
					SteamMatchmaking.SetLobbyData(joinedLobbyID, "RepairStatus", "1");
				}
			}
			Hub.s.apihandler.EnqueueAPI<APIOpenPublicTramLogRes>(new APIOpenPublicTramLogReq
			{
				guid = Hub.s.pdata.GUID,
				sessionID = Hub.s.pdata.ClientSessionID,
				roomSessionID = Hub.s.pdata.ClientRoomSessionID,
				appVersion = "0.2.7"
			}, delegate(IResMsg res)
			{
				if (res.errorCode != MsgErrorCode.Success)
				{
					Logger.RError($"APIOpenPublicTramLogReq failed : {res.errorCode}");
				}
			});
		}
		SteamMatchmaking.SetLobbyData(joinedLobbyID, "Cycle", cycleCount.ToString());
		SteamMatchmaking.SetLobbyData(joinedLobbyID, "PublicRoom", flag.ToString().ToLower());
		setLobbyPublicCoroutine = null;
	}

	public void UpdateLobbyData(string key, string value)
	{
		SteamMatchmaking.SetLobbyData(joinedLobbyID, key, value);
	}

	public void StartRandomMatching()
	{
		if (isRandomMatching)
		{
			Logger.RWarn("already in random matching mode.");
			return;
		}
		if (joinSuccess)
		{
			Logger.RWarn(" already joined a lobby.");
			return;
		}
		isRoomSearching = true;
		isRandomMatching = true;
		randomMatchStartTime = Time.time;
		randomMatchAttempts = 0;
		availableLobbies.Clear();
		currentLobbyAttemptIndex = 0;
		SearchRandomLobby();
	}

	private void SearchRandomLobby()
	{
		string currentLocale = GetCurrentLocale();
		string pchValueToMatch = "EA0.2.7";
		SteamMatchmaking.AddRequestLobbyListStringFilter("Version", pchValueToMatch, ELobbyComparison.k_ELobbyComparisonEqual);
		SteamMatchmaking.AddRequestLobbyListStringFilter("Locale", currentLocale, ELobbyComparison.k_ELobbyComparisonEqual);
		SteamMatchmaking.AddRequestLobbyListStringFilter("PublicRoom", "true", ELobbyComparison.k_ELobbyComparisonEqual);
		SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
		SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterClose);
		SteamMatchmaking.AddRequestLobbyListResultCountFilter(20);
		SteamAPICall_t hAPICall = SteamMatchmaking.RequestLobbyList();
		m_LobbyMatchList.Set(hAPICall);
	}

	public void RequestLobbyOnce(bool _currentVersion = true, bool _mylocale = true, ELobbyDistanceFilter _distanceFilter = ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide, bool _isPublic = true, int _count = 50, UIPrefab_PublicRoomList _roomListUI = null)
	{
		if (refreshTime <= 0)
		{
			refreshTime = 10;
			StartCoroutine(GetLobbyListTimer());
			if (_roomListUI != null)
			{
				roomListUI = _roomListUI;
			}
			requestLobbyListStep = 1;
			RequestLobbyList(_currentVersion, _mylocale, _distanceFilter, _isPublic, _count);
		}
	}

	public void RequestLobbyList(int _count = 50, UIPrefab_PublicRoomList _roomListUI = null)
	{
		if (_count > 0 && refreshTime <= 0)
		{
			refreshTime = 10;
			StartCoroutine(GetLobbyListTimer());
			if (_roomListUI != null)
			{
				roomListUI = _roomListUI;
			}
			requestLobbyListStep = 0;
			availableLobbies.Clear();
			StartCoroutine(RequestLobbyListCoroutine(_count));
		}
	}

	private IEnumerator RequestLobbyListCoroutine(int _count)
	{
		_count /= 2;
		RequestLobbyList(_currentVersion: true, _mylocale: true, ELobbyDistanceFilter.k_ELobbyDistanceFilterClose, _isPublic: true, _count);
		yield return new WaitUntil(() => requestLobbyListStep == 1);
		RequestLobbyList(_currentVersion: true, _mylocale: false, ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide, _isPublic: true, _count * 2 - availableLobbies.Count);
		yield return new WaitUntil(() => requestLobbyListStep == 2);
	}

	private void RequestLobbyList(bool _currentVersion = true, bool _mylocale = true, ELobbyDistanceFilter _distanceFilter = ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide, bool _isPublic = true, int _count = 50)
	{
		string pchValueToMatch = (_isPublic ? "true" : "false");
		if (_currentVersion)
		{
			string pchValueToMatch2 = "EA0.2.7";
			SteamMatchmaking.AddRequestLobbyListStringFilter("Version", pchValueToMatch2, ELobbyComparison.k_ELobbyComparisonEqual);
		}
		if (_mylocale)
		{
			string currentLocale = GetCurrentLocale();
			SteamMatchmaking.AddRequestLobbyListStringFilter("Locale", currentLocale, ELobbyComparison.k_ELobbyComparisonEqual);
		}
		else
		{
			string currentLocale2 = GetCurrentLocale();
			SteamMatchmaking.AddRequestLobbyListStringFilter("Locale", currentLocale2, ELobbyComparison.k_ELobbyComparisonNotEqual);
		}
		SteamMatchmaking.AddRequestLobbyListStringFilter("PublicRoom", pchValueToMatch, ELobbyComparison.k_ELobbyComparisonEqual);
		SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
		SteamMatchmaking.AddRequestLobbyListDistanceFilter(_distanceFilter);
		SteamMatchmaking.AddRequestLobbyListResultCountFilter(_count);
		SteamAPICall_t hAPICall = SteamMatchmaking.RequestLobbyList();
		m_LobbyMatchList.Set(hAPICall);
	}

	private IEnumerator GetLobbyListTimer()
	{
		while (refreshTime > 0)
		{
			yield return new WaitForSeconds(1f);
			refreshTime--;
		}
	}

	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult == EResult.k_EResultOK)
		{
			Logger.RLog("로비 생성 성공!");
			isLobbyCreated = true;
			joinedLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
			SteamMatchmaking.SetLobbyData(joinedLobbyID, "Version", "EA0.2.7");
			SteamMatchmaking.SetLobbyData(joinedLobbyID, "serverPort", Hub.s.gameConfig.gameSetting.ServerPort.ToString());
			lobbyName = Hub.GetL10NText("STRING_PUBLIC_TRAM_TITLE_DEFAULT", Hub.s.pdata.MyNickName);
			SteamMatchmaking.SetLobbyData(joinedLobbyID, "LobbyName", lobbyName);
			string currentLocale = GetCurrentLocale();
			SteamMatchmaking.SetLobbyData(joinedLobbyID, "Locale", currentLocale);
			bool flag = PlayerPrefs.GetInt("TempLobbyIsOpen", 0) == 1;
			SteamMatchmaking.SetLobbyData(joinedLobbyID, "PublicRoom", flag.ToString().ToLower());
			PlayerPrefs.DeleteKey("TempLobbyIsOpen");
			string text = PlayerPrefs.GetString("AppliedSteamInventoryItemDefIDs");
			if (!string.IsNullOrEmpty(text))
			{
				SteamMatchmaking.SetLobbyData(joinedLobbyID, "AppliedSteamInventoryItemDefIDs", text);
			}
			Logger.RLog($"로비 데이터 설정 완료 - Locale: {currentLocale}, IsPublic: {flag}");
		}
		else
		{
			createLobbyCount++;
			if (createLobbyCount > 5)
			{
				Logger.RError("로비 생성 실패, 최대 시도 횟수 초과");
				return;
			}
			CreateLobby();
			Logger.RError("로비 생성 실패, 에러 코드: " + callback.m_eResult);
		}
	}

	public void JoinGameRoom(string steamID)
	{
		lobbyMain.StartCoroutine(lobbyMain.JoinRoomWithAddress(steamID, tempServerPort, withRelay: true));
	}

	public void RequestPublicJoinLobby(string lobbyID, Action callback)
	{
		isRequestPublicJoinLobby = true;
		if (!ulong.TryParse(lobbyID, out var result))
		{
			Logger.RWarn("Invalid lobby ID");
			return;
		}
		publicJoinLobbyCallback = callback;
		targetLobbyID = new CSteamID(result);
		SteamMatchmaking.RequestLobbyData(new CSteamID(result));
	}

	private void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
	{
		if (!(new CSteamID(pCallback.m_ulSteamIDLobby) != targetLobbyID))
		{
			publicJoinLobbyCallback?.Invoke();
			publicJoinLobbyCallback = null;
			isRequestPublicJoinLobby = false;
		}
	}

	private void OnLobbyMatchListReceived(LobbyMatchList_t callback, bool bIOFailure)
	{
		if (bIOFailure)
		{
			OnRandomMatchingFailed("RANDOM_MATCH_SEARCH_FAILED");
			return;
		}
		Logger.RLog($"search complete: {callback.m_nLobbiesMatching} found");
		for (int i = 0; i < callback.m_nLobbiesMatching; i++)
		{
			CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(i);
			if (!availableLobbies.Contains(lobbyByIndex))
			{
				availableLobbies.Add(lobbyByIndex);
			}
		}
		requestLobbyListStep++;
		if (requestLobbyListStep == 2)
		{
			if (availableLobbies.Count == 0)
			{
				roomListUI.SetEmptyListText();
			}
			else if (roomListUI != null)
			{
				roomListUI.SetRoomList(availableLobbies);
			}
		}
	}

	private IEnumerator TimeoutRandomMatching()
	{
		isRandomMatchingTimeout = false;
		yield return new WaitForSeconds(30f);
		if (!isRoomSearching)
		{
			isRandomMatchingTimeout = true;
			Logger.RWarn("timeout");
			OnRandomMatchingFailed("RANDOM_MATCH_TIMEOUT");
		}
	}

	private void TryJoinNextRandomLobby()
	{
		if (!isRandomMatchingTimeout)
		{
			if (randomMatchAttempts >= 10)
			{
				Logger.RWarn("max attempts reached");
				OnRandomMatchingFailed("RANDOM_MATCH_MAX_ATTEMPTS");
				return;
			}
			if (currentLobbyAttemptIndex >= availableLobbies.Count)
			{
				Logger.RWarn("all lobbies attempted, restarting search");
				SearchRandomLobby();
				return;
			}
			CSteamID cSteamID = availableLobbies[currentLobbyAttemptIndex];
			currentLobbyAttemptIndex++;
			randomMatchAttempts++;
			Logger.RLog($"try entering lobby ({randomMatchAttempts}/{10}): {cSteamID}");
			tempIDWhoInviteMe = "RandomMatch";
			SteamMatchmaking.JoinLobby(cSteamID);
		}
	}

	public void CreateLobby(bool isOpenForRandomMatch = false)
	{
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
		PlayerPrefs.SetInt("TempLobbyIsOpen", isOpenForRandomMatch ? 1 : 0);
	}

	public void LeaveLobby()
	{
		if (isLobbyCreated || !(joinedLobbyID != CSteamID.Nil))
		{
			SteamMatchmaking.LeaveLobby(joinedLobbyID);
			isLobbyCreated = false;
			joinedLobbyID = CSteamID.Nil;
			Logger.RLog("lobby destroyed");
		}
	}

	public void OpenSteamFriends()
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.ActivateGameOverlay("Friends");
		}
	}

	public void JoinFriendWithMatchKeyProcess(string matchKey)
	{
		if (string.IsNullOrEmpty(matchKey))
		{
			Logger.RError("키가 비어있습니다.");
		}
		else
		{
			StartCoroutine(JoinFriendWithMatchKey(matchKey));
		}
	}

	private IEnumerator JoinFriendWithMatchKey(string matchKey)
	{
		if (lobbyMain == null)
		{
			Logger.RWarn("lobbyMain is null");
			yield break;
		}
		Logger.RLog("코드 입력 입장 시도");
		JoinSteamMatchmakingLobby("", matchKey);
	}

	public string GetLobbyIDFromCommandLine()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		ulong result = 0uL;
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == "+connect_lobby" && i + 1 < commandLineArgs.Length)
			{
				if (ulong.TryParse(commandLineArgs[i + 1], out result))
				{
					return result.ToString();
				}
				Logger.RLog("로비 ID 파싱 실패: " + commandLineArgs[i + 1]);
			}
		}
		return null;
	}

	private IEnumerator WakeUp()
	{
		while (SteamManager.Initialized)
		{
			yield return new WaitForSeconds(300f);
			DisplayStatus("MIMESIS");
		}
	}

	public string GetInviteLink()
	{
		string[] obj = new string[6]
		{
			"steam://joinlobby/",
			2842990.ToString(),
			"/",
			null,
			null,
			null
		};
		CSteamID cSteamID = joinedLobbyID;
		obj[3] = cSteamID.ToString();
		obj[4] = "/";
		obj[5] = SteamUser.GetSteamID().ToString();
		string text = string.Concat(obj);
		Logger.RLog("초대 링크 생성: " + text);
		return text;
	}
}
