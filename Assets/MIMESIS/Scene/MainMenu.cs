using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bifrost.Cooked;
using Dissonance.Integrations.FishNet;
using ModUtility;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using Steamworks;
using UnityEngine;
using UnityEngine.CrashReportHandler;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
	private UIPrefab_MainMenu ui_mainmenu;

	[SerializeField]
	private GameObject uiprefab_mainmenu;

	[HideInInspector]
	public ServerListInformer sli;

	private bool joinRoom;

	[SerializeField]
	private GameObject uiprefab_loadtram;

	[SerializeField]
	private GameObject uiprefab_newtram;

	[SerializeField]
	private GameObject uiprefab_newtrampopup;

	private int _selectedSaveSlotID = -1;

	private string lobbyId;

	private string friendSteamID;

	private bool joinSessionResponse;

	private UIManager uiman => Hub.s.uiman;

	private Hub.PersistentData pdata => Hub.s.pdata;

	private void LoadSaveAndCreateRoom(UIPrefab_LoadTram loadtram, int slotID)
	{
		uiman.ui_escapeStack.Remove(loadtram);
		loadtram.Hide();
		_selectedSaveSlotID = slotID;
		Hub.s.pdata.SaveSlotID = _selectedSaveSlotID;
		MMSaveGameData loadedSaveData = loadtram.GetLoadedSaveData(_selectedSaveSlotID);
		if (loadedSaveData != null)
		{
			Hub.s.pdata.CycleCount = loadedSaveData.CycleCount;
			Hub.s.pdata.DayCount = loadedSaveData.DayCount;
			Hub.s.pdata.Repaired = loadedSaveData.TramRepaired;
			Hub.s.pdata.TramUpgradeIDs = ((loadedSaveData.TramUpgradeList != null) ? loadedSaveData.TramUpgradeList.Clone() : new List<int>());
			Hub.s.pdata.TramUpgradeIDs = Hub.s.pdata.TramUpgradeIDs.Distinct().ToList();
			StartCoroutine(CreateRoom());
		}
		else
		{
			Logger.RLog("No save data found");
			CreateNewGameInSlot(loadtram, slotID);
		}
	}

	private void CreateNewGameInSlot(UIPrefab_NewTram newtram, int slotID)
	{
		uiman.ui_escapeStack.Remove(newtram);
		newtram.Hide();
		Hub.s.pdata.ResetCycleInfos();
		Hub.s.pdata.SaveSlotID = slotID;
		TryDeleteSaveGameData(slotID);
		StartCoroutine(CreateRoom());
	}

	private void CreateNewGameInSlot(UIPrefab_LoadTram newtram, int slotID)
	{
		uiman.ui_escapeStack.Remove(newtram);
		newtram.Hide();
		Hub.s.pdata.ResetCycleInfos();
		Hub.s.pdata.SaveSlotID = slotID;
		StartCoroutine(CreateRoom());
	}

	private void HandleNewGameSlotSelection(UIPrefab_NewTram newtram, UIPrefab_NewTramPopUp newtrampopup, int slotID)
	{
		_selectedSaveSlotID = slotID;
		if (newtram.IsSlotEmpty(_selectedSaveSlotID))
		{
			CreateNewGameInSlot(newtram, _selectedSaveSlotID);
			return;
		}
		uiman.ui_escapeStack.Add(newtrampopup);
		EventSystem.current?.SetSelectedGameObject(null);
		newtrampopup.Show();
	}

	private void Start()
	{
		uiman.tempPlayerVolumeDictionary.Clear();
		uiman.tempPlayerVolumeMuteDictionary.Clear();
		Hub.s.steamInviteDispatcher.joinSuccess = false;
		string text = Hub.s.steamInviteDispatcher.joinedLobbyID.ToString();
		if (text != "")
		{
			if (ulong.TryParse(text, out var result))
			{
				SteamMatchmaking.LeaveLobby(new CSteamID(result));
			}
			else
			{
				Logger.RLog("Failed to parse joined lobby ID: " + text);
			}
		}
		if (Hub.s == null)
		{
			return;
		}
		uiman.ShowTitleBGPrecess();
		sli = new ServerListInformer();
		CommandLineAnalyzer commandLineAnalyzer = new CommandLineAnalyzer();
		commandLineAnalyzer.Analyze(pdata.commandLineArgs);
		if (commandLineAnalyzer.canExcute)
		{
			if (commandLineAnalyzer.hostMode)
			{
				StartCoroutine(CreateRoom());
				return;
			}
			if (commandLineAnalyzer.participantMode)
			{
				StartCoroutine(JoinRoomWithAddress(commandLineAnalyzer.participantAddress, commandLineAnalyzer.participantPort));
				return;
			}
		}
		ui_mainmenu = uiman.InstatiateUIPrefab<UIPrefab_MainMenu>(uiprefab_mainmenu);
		UIPrefab_NewTram newtram = uiman.InstatiateUIPrefab<UIPrefab_NewTram>(uiprefab_newtram, eUIHeight.Top);
		UIPrefab_LoadTram loadtram = uiman.InstatiateUIPrefab<UIPrefab_LoadTram>(uiprefab_loadtram, eUIHeight.Top);
		UIPrefab_NewTramPopUp newtrampopup = uiman.InstatiateUIPrefab<UIPrefab_NewTramPopUp>(uiprefab_newtrampopup, eUIHeight.Top);
		loadtram.Hide();
		newtram.Hide();
		newtrampopup.Hide();
		string text2 = PlayerPrefs.GetString("gamehostAddress", "127.0.0.1");
		ui_mainmenu.UE_HostAddress.text = text2;
		ui_mainmenu.OnLoadButton = delegate
		{
			EventSystem.current?.SetSelectedGameObject(null);
			loadtram.InitSaveInfoList();
			uiman.ui_escapeStack.Add(loadtram);
			loadtram.Show();
		};
		loadtram.OnSavedFile0 = delegate
		{
			LoadSaveAndCreateRoom(loadtram, 0);
		};
		loadtram.OnSavedFile1 = delegate
		{
			LoadSaveAndCreateRoom(loadtram, 1);
		};
		loadtram.OnSavedFile2 = delegate
		{
			LoadSaveAndCreateRoom(loadtram, 2);
		};
		loadtram.OnSavedFile3 = delegate
		{
			LoadSaveAndCreateRoom(loadtram, 3);
		};
		loadtram.OnButtonClose = delegate
		{
			uiman.ui_escapeStack.Remove(loadtram);
			loadtram.Hide();
		};
		ui_mainmenu.OnHostButton = delegate
		{
			EventSystem.current?.SetSelectedGameObject(null);
			Hub.s.pdata.SaveSlotID = -1;
			newtram.InitSaveInfoList();
			uiman.ui_escapeStack.Add(newtram);
			newtram.Show();
		};
		newtram.OnButtonClose = delegate
		{
			uiman.ui_escapeStack.Remove(newtram);
			newtram.Hide();
		};
		newtram.OnSavedFile1 = delegate
		{
			HandleNewGameSlotSelection(newtram, newtrampopup, 1);
		};
		newtram.OnSavedFile2 = delegate
		{
			HandleNewGameSlotSelection(newtram, newtrampopup, 2);
		};
		newtram.OnSavedFile3 = delegate
		{
			HandleNewGameSlotSelection(newtram, newtrampopup, 3);
		};
		newtrampopup.OnDeleteAndCreateNew = delegate
		{
			uiman.ui_escapeStack.Remove(newtrampopup);
			newtrampopup.Hide();
			CreateNewGameInSlot(newtram, _selectedSaveSlotID);
		};
		newtrampopup.OnCancel = delegate
		{
			_selectedSaveSlotID = -1;
			uiman.ui_escapeStack.Remove(newtrampopup);
			newtrampopup.Hide();
		};
		ui_mainmenu.OnConnectButton = delegate
		{
			string[] array = ui_mainmenu.UE_HostAddress.text.Split(':');
			if (array.Length != 2)
			{
				Logger.RError("Invalid address format");
			}
			else
			{
				string text3 = array[0];
				int port = Convert.ToInt32(array[1]);
				PlayerPrefs.SetString("gamehostAddress", text3 ?? "");
				StartCoroutine(JoinRoomWithAddress(text3, port));
			}
		};
		ui_mainmenu.OnRefreshButton = delegate
		{
			ui_mainmenu.StartConnecting();
			sli.UpdateHostInfo();
		};
		uiman.HideGameTips();
		ui_mainmenu.Show();
		ui_mainmenu.StartConnecting();
		Hub.s.steamInviteDispatcher.lobbyMain = this;
		if (Hub.s != null && Hub.s.steamInviteDispatcher.isFirstLobby)
		{
			StartCoroutine(CheckInvite());
		}
		CheckMic();
		pdata.completeMakingRoomSig = null;
		pdata.enableWaitingRoomSig = null;
		pdata.lastResponseError = MsgErrorCode.Success;
		pdata.itemPrices = null;
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.EnterMainMenu);
	}

	private IEnumerator CheckInvite()
	{
		Hub.s.steamInviteDispatcher.isFirstLobby = false;
		if (!Hub.s.steamInviteDispatcher.joinOnce)
		{
			yield break;
		}
		lobbyId = Hub.s.steamInviteDispatcher.GetLobbyIDFromCommandLine();
		if (lobbyId == null)
		{
			yield break;
		}
		if (ulong.TryParse(lobbyId, out var result))
		{
			CSteamID lobbyID = new CSteamID(result);
			Logger.RLog($"JoinByInvitation : Joining lobby: {lobbyID}");
			SteamMatchmaking.JoinLobby(lobbyID);
			yield return new WaitUntil(() => SteamMatchmaking.GetLobbyOwner(lobbyID) != CSteamID.Nil);
			Hub.s.steamInviteDispatcher.joinOnce = false;
		}
		else
		{
			Logger.RLog("Failed to parse lobby ID: " + lobbyId);
		}
	}

	private void OnDestroy()
	{
		if (!(Hub.s == null))
		{
			_ = Hub.s.console != null;
			ModHelper.InvokeTimingCallback(ModHelper.eTiming.ExitMainMenu);
			if (ui_mainmenu != null)
			{
				UnityEngine.Object.Destroy(ui_mainmenu.gameObject);
				ui_mainmenu = null;
			}
			if (sli != null)
			{
				sli.Dispose();
				sli = null;
			}
		}
	}

	private void Update()
	{
		sli.Update();
	}

	private void CheckMic()
	{
		List<string> list = new List<string>();
		DissonanceFishNetComms.Instance.Comms.GetMicrophoneDevices(list);
		string[] devices = Microphone.devices;
		for (int i = 0; i < devices.Length; i++)
		{
		}
		for (int j = 0; j < list.Count; j++)
		{
		}
	}

	private void TryInitHostRoom()
	{
		if (pdata.ClientMode == NetworkClientMode.Host)
		{
			Logger.RLog("InitHostRoom in MainMenu");
			VWorld? vworld = Hub.s.vworld;
			if (vworld != null && !vworld.InitHostRoom(SystemInfo.deviceUniqueIdentifier, pdata.SaveSlotID, pdata.MyNickName))
			{
				Logger.RError("InitHostRoom fail. return to main menu");
			}
			else
			{
				Logger.RLog("TryInitHostMaintenenceRoom in MainMenu completed");
			}
		}
	}

	private IEnumerator CreateRoom()
	{
		pdata.ClientMode = NetworkClientMode.Host;
		pdata.GameServerAddressOrSteamId = "localhost";
		pdata.GameServerPort = 22220;
		Hub.s.CreateVWorld(pdata.SaveSlotID);
		TryInitHostRoom();
		Hub.s.apihandler.EnqueueAPI<APINewRoomLogRes>(new APINewRoomLogReq
		{
			guid = Hub.s.pdata.GUID,
			sessionID = Hub.s.pdata.ClientSessionID,
			roomSessionID = Hub.s.vworld.GetHostSessionID().ToString()
		}, delegate(IResMsg res)
		{
			if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError($"APINewRoomLogReq failed : {res.errorCode}");
			}
		});
		if (Hub.s.pdata.ClientMode == NetworkClientMode.Host)
		{
			Hub.s.steamInviteDispatcher.joinedLobbyID = CSteamID.Nil;
			Hub.s.steamInviteDispatcher.CreateLobby();
		}
		if (Hub.s.netman2.State != NetworkManagerState.Connected)
		{
			yield return CorTryToConnect();
			if (Hub.s.netman2.State != NetworkManagerState.Connected)
			{
				Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate
				{
				}, null, "STRING_NOT_CONNECTED_NETWORK_TITLE", "STRING_NOT_CONNECTED_NETWORK_DESCRIPTION");
				ui_mainmenu.EndConnecting();
				yield break;
			}
			if (pdata.lastResponseError != MsgErrorCode.Success)
			{
				Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate
				{
				}, null, "STRING_NOT_CONNECTED_NETWORK_TITLE", "STRING_NOT_CONNECTED_NETWORK_DESCRIPTION");
				ui_mainmenu.EndConnecting();
				yield break;
			}
		}
		yield return new WaitUntil(() => Hub.s.steamInviteDispatcher.isLobbyCreated || Hub.s.steamInviteDispatcher.joinedLobbyID != CSteamID.Nil);
		yield return TryJoinSession();
		if (pdata.lastResponseError != MsgErrorCode.Success)
		{
			yield return HandleJoinSessionFailed();
			ui_mainmenu.EndConnecting();
		}
		else
		{
			string value = SteamUtil.UserSteamID ?? "failedSteamID";
			CrashReportHandler.SetUserMetadata("connect_token", value);
			yield return Enter();
		}
	}

	private IEnumerator CorTryToConnect()
	{
		Logger.RLog("TryToConnect in MainMenu");
		string gameServerAddressOrSteamId = pdata.GameServerAddressOrSteamId;
		int gameServerPort = pdata.GameServerPort;
		if (gameServerAddressOrSteamId == string.Empty || gameServerPort == 0)
		{
			Logger.RError("GameServerAddress or GameServerPort is not set");
			yield break;
		}
		Hub.s.netman2.Initialize();
		Hub.s.netman2.ClearServInfo(ServerType.Game);
		if (pdata.ClientMode == NetworkClientMode.Host)
		{
			Hub.s.netman2.AddServInfo(ServerType.Game, gameServerAddressOrSteamId, gameServerPort);
		}
		else
		{
			Hub.s.netman2.AddServInfo(ServerType.Game, gameServerAddressOrSteamId, gameServerPort, pdata.WithRelay);
		}
		Hub.s.netman2.Connect();
		yield return new WaitUntil(() => Hub.s.netman2.State != NetworkManagerState.Connecting);
		if (Hub.s.netman2.State == NetworkManagerState.Connected)
		{
			Logger.RLog("TryToConnect in MainMenu completed");
		}
		else
		{
			pdata.lastResponseError = MsgErrorCode.CantConnect;
		}
	}

	private long GetSessionId(NetworkClientMode clientMode)
	{
		long num = 0L;
		if (clientMode == NetworkClientMode.Host)
		{
			return Hub.s.vworld.GetHostSessionID();
		}
		return 1L;
	}

	private void JoinSessionResponse(JoinServerRes _res)
	{
		if (_res == null)
		{
			pdata.SessionJoined = false;
			pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
			Logger.RError("JoinServerRes is null");
		}
		else if (_res.errorCode != MsgErrorCode.Success)
		{
			pdata.SessionJoined = false;
			pdata.lastResponseError = _res.errorCode;
		}
		else
		{
			pdata.lastResponseError = MsgErrorCode.Success;
			pdata.SessionJoined = true;
			pdata.PlayerUID = _res.playerUID;
			if (pdata.ClientMode != NetworkClientMode.Host)
			{
				pdata.CycleCount = _res.roomInfo.CycleCountForClient;
				pdata.DayCount = _res.roomInfo.dayCountForClient;
				pdata.Repaired = _res.roomInfo.repairedForClient;
				pdata.TramUpgradeIDs = _res.roomInfo.tramUpgradeListForClient.Clone();
				pdata.TramUpgradeIDs = pdata.TramUpgradeIDs.Distinct().ToList();
			}
			Logger.RLog(string.Format("JoinSession in MainMenu completed: playerUID={0}, ClientMode={1}, CycleCount={2}, DayCount={3}, Repaired={4}, tramUpgradeIDs = [{5}]", _res.playerUID, pdata.ClientMode, pdata.CycleCount, pdata.DayCount, pdata.Repaired, string.Join(",", pdata.TramUpgradeIDs)));
		}
		joinSessionResponse = true;
	}

	private IEnumerator TryJoinSession()
	{
		joinSessionResponse = false;
		Logger.RLog("JoinSession in Lobby");
		GetSessionId(pdata.ClientMode);
		JoinServerReq msg = new JoinServerReq
		{
			guid = SystemInfo.deviceUniqueIdentifier,
			steamID = Hub.s.pdata.GetUserSteamIDUInt64(),
			nickName = pdata.MyNickName,
			voiceUID = Hub.s.voiceman.GetLocalPlayerUID(),
			isHost = (pdata.ClientMode == NetworkClientMode.Host),
			clientVersion = "0.2.7"
		};
		CancellationToken cancellationToken = new CancellationTokenSource().Token;
		Hub.s.netman2.SendWithCallback<JoinServerRes>(msg, JoinSessionResponse, cancellationToken);
		long timeout = Hub.s.timeutil.GetCurrentTickMilliSec() + 20000;
		yield return new WaitUntil(() => joinSessionResponse || Hub.s.timeutil.GetCurrentTickMilliSec() > timeout || cancellationToken.IsCancellationRequested);
		if (!joinSessionResponse)
		{
			pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
			Logger.RError("JoinSession timeout");
		}
		else if (pdata.lastResponseError != MsgErrorCode.Success)
		{
			Hub.s.steamInviteDispatcher.joinSuccess = false;
			SteamMatchmaking.LeaveLobby(Hub.s.steamInviteDispatcher.joinedLobbyID);
			switch (pdata.lastResponseError)
			{
			case MsgErrorCode.GameAlreadyStarted:
				Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate
				{
				}, null, "INVITE_DO_NOT_ENTER_TITLE", "INVITE_DO_NOT_ENTER_ALREADY_DEPARTED");
				yield break;
			case MsgErrorCode.PlayerCountExceeded:
				Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate
				{
				}, null, "INVITE_DO_NOT_ENTER_TITLE", "INVITE_DO_NOT_ENTER_ROOM_FULL");
				yield break;
			}
			Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate
			{
			}, null, "INVITE_DO_NOT_ENTER_TITLE", "INVITE_DO_NOT_ENTER_COMMON", pdata.lastResponseError.ToString());
			Logger.RLog($"JoinSession failed: {pdata.lastResponseError}");
		}
		else
		{
			Logger.RLog("JoinSession completed");
		}
	}

	private IEnumerator HandleJoinSessionFailed()
	{
		Hub.s.netman2.Disconnect(DisconnectReason.ByClient);
		pdata.lastResponseError = MsgErrorCode.Success;
		pdata.SessionJoined = false;
		yield return new WaitUntil(() => Hub.s.netman2.State == NetworkManagerState.Disconnected);
	}

	public IEnumerator JoinRoomWithAddress(string addressOrSteamId, int port, bool withRelay = false)
	{
		yield return sli.TryConnect(addressOrSteamId, port, withRelay);
		if (sli.networkState != NetworkManagerState.Connected)
		{
			yield break;
		}
		pdata.ClientMode = NetworkClientMode.Participant;
		pdata.GameServerAddressOrSteamId = addressOrSteamId;
		pdata.GameServerPort = port;
		pdata.WithRelay = withRelay;
		if (Hub.s.netman2.State != NetworkManagerState.Connected)
		{
			yield return CorTryToConnect();
			if (Hub.s.netman2.State != NetworkManagerState.Connected)
			{
				SteamMatchmaking.LeaveLobby(Hub.s.steamInviteDispatcher.joinedLobbyID);
				Hub.s.steamInviteDispatcher.joinSuccess = false;
				Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate
				{
				}, null, "STRING_NOT_CONNECTED_NETWORK_TITLE", "STRING_NOT_CONNECTED_NETWORK_DESCRIPTION");
				ui_mainmenu.EndConnecting();
				yield break;
			}
			if (pdata.lastResponseError != MsgErrorCode.Success)
			{
				SteamMatchmaking.LeaveLobby(Hub.s.steamInviteDispatcher.joinedLobbyID);
				Hub.s.steamInviteDispatcher.joinSuccess = false;
				Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate
				{
				}, null, "STRING_NOT_CONNECTED_NETWORK_TITLE", "STRING_NOT_CONNECTED_NETWORK_DESCRIPTION");
				ui_mainmenu.EndConnecting();
				yield break;
			}
		}
		yield return TryJoinSession();
		if (pdata.lastResponseError != MsgErrorCode.Success)
		{
			yield return HandleJoinSessionFailed();
			ui_mainmenu.EndConnecting();
		}
		else
		{
			yield return Enter();
			joinRoom = true;
		}
	}

	private IEnumerator Enter()
	{
		if (ui_mainmenu != null)
		{
			yield return ui_mainmenu.Cor_Hide();
			UnityEngine.Object.Destroy(ui_mainmenu.gameObject);
			ui_mainmenu = null;
		}
		if (Hub.s.uiman.ui_sceneloading != null)
		{
			Hub.s.uiman.ui_sceneloading.Show();
		}
		ulong joinedLobbyID = Hub.s.pdata.GetJoinedLobbyID();
		SteamLobbyChecker lobbyChecker = new SteamLobbyChecker(joinedLobbyID);
		lobbyChecker.RequestData();
		yield return new WaitForSeconds(Hub.s.uiman.WaitingRoomFadeOutSec);
		yield return new WaitUntil(() => lobbyChecker.IsLobbyDataUpdated);
		pdata.IsPublicLobby = lobbyChecker.IsPublicLobby;
		Hub.s.tableman.lootingObject.ClearSkinnedItemInfos();
		pdata.AppliedTramSkin.Clear();
		pdata.AppliedItemSkinDictionary.Clear();
		pdata.AppliedAdditionalItemIDs.Clear();
		foreach (int item in lobbyChecker.AppliedSteamItemDefIdListFromLobby)
		{
			if (Hub.s.dataman.ExcelDataManager.PromotionRewardDictByItemDefId.TryGetValue(item, out PromotionRewardInfo value))
			{
				if (value.IsTramSkin())
				{
					pdata.AppliedTramSkin = value.AddTramPartsNames;
				}
				else if (value.IsItemSkin())
				{
					pdata.AppliedItemSkinDictionary.Add(value.ChangeItemPrefab.Value.itemMasterId, value.ChangeItemPrefab.Value.skinName);
				}
				else if (value.IsStartItem())
				{
					pdata.AppliedAdditionalItemIDs.Add(value.StartItemId.Value);
				}
			}
		}
		lobbyChecker.Dispose();
		lobbyChecker = null;
		Hub.LoadScene("MaintenanceScene");
	}

	public void RefreshServer()
	{
		ui_mainmenu.StartConnecting();
		sli.UpdateHostInfo();
	}

	private bool TryDeleteSaveGameData(int saveSlotID)
	{
		if (!MMSaveGameData.CheckSaveSlotID(saveSlotID))
		{
			Logger.RError($"[TryDeleteSaveGameData] fail. check saveSlotID = {saveSlotID}");
			return false;
		}
		if (!MonoSingleton<PlatformMgr>.Instance.IsSaveFileExist(MMSaveGameData.GetSaveFileName(saveSlotID)))
		{
			return false;
		}
		MonoSingleton<PlatformMgr>.Instance.Delete(MMSaveGameData.GetSaveFileName(saveSlotID));
		return true;
	}
}
