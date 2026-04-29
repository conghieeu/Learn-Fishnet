using System;
using System.Collections;
using System.Collections.Generic;
using ModUtility;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using UnityEngine;
using UnityEngine.CrashReportHandler;
using UnityEngine.Events;

public class MaintenanceScene : GameMainBase
{
	[SerializeField]
	private Transform maintenanceRoomRoot;

	[SerializeField]
	private Transform failDirectionRoot;

	[SerializeField]
	protected UIPrefab_CurrentCurrency ui_CurrentCurrency;

	[SerializeField]
	protected UIPrefab_RepairInfo ui_RepairInfo;

	[SerializeField]
	protected UpgradeSelectLevelObject UpgradeSelectLevelObject;

	private bool hostStartSessionFlag;

	private Coroutine netLoopRunner;

	private bool isFirstSession;

	public bool isRepairing;

	public bool isRepaired;

	private bool _prepareFirstCycle;

	private bool _isGameOver;

	private int _actorIdPullingTramStartLever;

	[SerializeField]
	private float publicTramOffTextDurationSec = 3f;

	private GameObject waitForReturnTramDialogBox;

	private bool sessionJoined => base.pdata.SessionJoined;

	public bool IsGameOver => _isGameOver;

	public int ActorIdPullingTramStartLever => _actorIdPullingTramStartLever;

	public bool PrepareFirstCycle
	{
		get
		{
			return _prepareFirstCycle;
		}
		set
		{
			_prepareFirstCycle = value;
		}
	}

	public void SetDayCountProgressText()
	{
		if (tramConsole != null)
		{
			tramConsole.SetDayCountInMaintenanceScene(!PrepareFirstCycle);
		}
	}

	private void PlayFail()
	{
		if (maintenanceRoomRoot != null && failDirectionRoot != null)
		{
			maintenanceRoomRoot.gameObject.SetActive(value: false);
			failDirectionRoot.gameObject.SetActive(value: true);
		}
	}

	protected override void Start()
	{
		if (Hub.s == null)
		{
			return;
		}
		base.Start();
		if (base.netman2.State != NetworkManagerState.Connected)
		{
			StartSceneLoading("FirstEnter");
		}
		else
		{
			StartSceneLoading("Maintenance");
		}
		Hub.s.steamInviteDispatcher.joinSuccess = false;
		PlayDefaultWeatherSfx();
		if (Hub.s.uiman.inviteLoadingUI != null)
		{
			Hub.s.uiman.inviteLoadingUI.Hide();
		}
		InitCommonUI(isMaintenanceScene: true);
		if (ui_CurrentCurrency != null && !ui_CurrentCurrency.isActiveAndEnabled)
		{
			ui_CurrentCurrency.Show();
		}
		if (ui_RepairInfo != null)
		{
			if (!ui_RepairInfo.isActiveAndEnabled)
			{
				ui_RepairInfo.Show();
			}
			ui_RepairInfo.SetCurrencyInfoMode();
		}
		CreateSpectatorHUD();
		StartCoroutine(CorRun());
		Hub.s.lcman.onLanguageChanged += OnLanguageChanged_dayCount;
		CutScenePlayer obj = cutScenePlayer;
		obj.OnPrePlayCutScene = (UnityAction<CutScenePlayer.CutScene>)Delegate.Combine(obj.OnPrePlayCutScene, new UnityAction<CutScenePlayer.CutScene>(OnPrePlayCutScene));
		CutScenePlayer obj2 = cutScenePlayer;
		obj2.OnPostPlayCutScene = (UnityAction<CutScenePlayer.CutScene>)Delegate.Combine(obj2.OnPostPlayCutScene, new UnityAction<CutScenePlayer.CutScene>(OnPostPlayCutScene));
	}

	protected override void OnDestroy()
	{
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.ExitMaintenance);
		if (!(Hub.s == null))
		{
			base.OnDestroy();
			if (netLoopRunner != null)
			{
				StopCoroutine(netLoopRunner);
				netLoopRunner = null;
			}
			base.uiman.CloseAllDialogueBox();
			Hub.s.navman.Clear();
			Hub.s.dynamicDataMan.Clear();
			if (Hub.s != null)
			{
				Hub.s.lcman.onLanguageChanged -= OnLanguageChanged_dayCount;
			}
			if (cutScenePlayer != null)
			{
				CutScenePlayer obj = cutScenePlayer;
				obj.OnPrePlayCutScene = (UnityAction<CutScenePlayer.CutScene>)Delegate.Remove(obj.OnPrePlayCutScene, new UnityAction<CutScenePlayer.CutScene>(OnPrePlayCutScene));
				CutScenePlayer obj2 = cutScenePlayer;
				obj2.OnPostPlayCutScene = (UnityAction<CutScenePlayer.CutScene>)Delegate.Remove(obj2.OnPostPlayCutScene, new UnityAction<CutScenePlayer.CutScene>(OnPostPlayCutScene));
			}
		}
	}

	private IEnumerator CorRun()
	{
		base.pdata.GameState = Hub.PersistentData.eGameState.Prepare;
		isRepaired = base.pdata.Repaired;
		Logger.RLog($"MaintenanceScene : cycleCount = {base.pdata.CycleCount}, dayCount = {base.pdata.DayCount}, repaired = {base.pdata.Repaired}");
		if (tramRepairRules != null)
		{
			if (!isRepaired && base.pdata.DayCount != 1)
			{
				tramRepairRules.ApplyDestructionPartsToTramInMaintenanceScene(0, base.pdata.CycleCount + 1);
				Logger.RLog($"ApplyDestructionPartsToTramInMaintenanceScene: ADD cycleCount = {base.pdata.CycleCount}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
			}
			int num = ((isRepaired && base.pdata.DayCount != 1) ? base.pdata.CycleCount : (base.pdata.CycleCount - 1));
			tramRepairRules.ApplyNewPartsToTram(num);
			Logger.RLog($"ApplyNewPartsToTram: {num}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
			tramRepairRules.ApplySteamItemPartsToTramInCommon(base.pdata.AppliedTramSkin);
		}
		Hub.s.dynamicDataMan.Build(BGRoot);
		interactObjectHelper.InitAllLevelObjects();
		BuildNavMesh();
		TryInitHostMaintenenceRoom();
		if (base.netman2.State == NetworkManagerState.Connected)
		{
			yield return CorEnterMaintenanceScene();
		}
	}

	private void TryInitHostMaintenenceRoom()
	{
		if (base.pdata.ClientMode == NetworkClientMode.Host)
		{
			Logger.RLog("InitHostRoom in MaintenanceScene");
			VWorld? vworld = Hub.s.vworld;
			if (vworld != null && !vworld.InitHostRoom(SystemInfo.deviceUniqueIdentifier, base.pdata.SaveSlotID, base.pdata.MyNickName))
			{
				Logger.RError("InitHostRoom fail. return to main menu");
			}
			else
			{
				Logger.RLog("TryInitHostMaintenenceRoom in MaintenanceScene completed");
			}
		}
	}

	private IEnumerator TryEnterMaintenanceRoom()
	{
		Logger.RLog("EnterMaintenanceRoom in MaintenanceScene");
		EnterMaintenanceRoomRes enterMaintenanceRoomRes = null;
		SendPacketWithCallback(new EnterMaintenanceRoomReq
		{
			playerUID = base.pdata.PlayerUID
		}, delegate(EnterMaintenanceRoomRes res)
		{
			if (res == null)
			{
				goBackToMainMenuFlag = true;
				base.pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
				Logger.RError("EnterMaintenanceRoomRes is null");
			}
			else if (res.errorCode != MsgErrorCode.Success)
			{
				goBackToMainMenuFlag = true;
				base.pdata.lastResponseError = res.errorCode;
			}
			else
			{
				enterMaintenanceRoomRes = res;
				base.pdata.MyActorID = res.playerInfo.actorID;
				base.pdata.itemPrices = res.itemPrices.Clone();
				Logger.RLog("EnterMaintenanceRoom in MaintenanceScene completed: RoomSessionID: " + res.roomSessionID);
				base.pdata.ClientRoomSessionID = res.roomSessionID;
				base.pdata.ClientRoomFirstCycle = res.cycleCount == 1 && res.dayCount == 1;
				base.pdata.CycleCount = res.cycleCount;
				base.pdata.DayCount = res.dayCount;
				base.pdata.TramUpgradeCandidate = res.tramUpgradeCandidate;
				Logger.RLog($"EnterMaintenanceRoom in MaintenanceScene completed: TramUpgradeCandidate = {res.tramUpgradeCandidate}");
				CopySessionConfigValueFrom(res.cycleCount, res.dayCount, res.repaired);
				netSyncGameData.currency = res.currency;
				if (base.pdata.ClientRoomFirstCycle)
				{
					Hub.s.apihandler.EnqueueAPI<APIJoinRoomLogRes>(new APIJoinRoomLogReq
					{
						guid = base.pdata.GUID,
						sessionID = base.pdata.ClientSessionID,
						roomSessionID = base.pdata.ClientRoomSessionID
					}, delegate(IResMsg resMsg)
					{
						if (resMsg.errorCode != MsgErrorCode.Success)
						{
							Logger.RError($"APIJoinRoomLogReq failed : {resMsg.errorCode}");
						}
					});
					CrashReportHandler.SetUserMetadata("connect_room_session_id", base.pdata.ClientRoomSessionID);
				}
			}
		}, destroyToken, 60000, disconnectWhenTimeout: true);
		yield return new WaitUntil(() => goBackToMainMenuFlag || enterMaintenanceRoomRes != null);
		if (goBackToMainMenuFlag)
		{
			OnNetDisconnected(DisconnectReason.ConnectionError, "Failed to EnterMaintenanceRoom");
		}
	}

	private IEnumerator CorEnterMaintenanceScene()
	{
		hostStartSessionFlag = false;
		EnteringCompleteAll = false;
		if (CheckNetworkConnection())
		{
			yield break;
		}
		base.netman2.PurgeMsg();
		yield return TryEnterMaintenanceRoom();
		if (base.pdata.lastResponseError != MsgErrorCode.Success)
		{
			yield break;
		}
		base.pdata.serverRoomState = Hub.PersistentData.eServerRoomState.PreGame;
		Hub.s.voiceman.SetVoiceMode(VoiceMode.PreGame);
		ConnectVoiceChat(base.pdata.GameServerAddressOrSteamId, base.pdata.WithRelay);
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
		SetDayCountProgressText();
		if (Hub.s.tramUpgrade != null)
		{
			Hub.s.tramUpgrade.UpdateByCandidate(base.pdata.TramUpgradeCandidate.leftPanel, base.pdata.TramUpgradeCandidate.rightPanel);
		}
		if (UpgradeSelectLevelObject != null)
		{
			UpgradeSelectLevelObject.Init(base.pdata.TramUpgradeCandidate.leftPanel, base.pdata.TramUpgradeCandidate.rightPanel);
		}
		if (tramConsole != null)
		{
			tramConsole.UpdateRepairState(isRepaired, isRepairing: false);
			tramConsole.UpdateWantedPoster(netSyncGameData.boostedItem, netSyncGameData.boostedRatio);
		}
		Hub.s.dLAcademyManager.GetAreaForDL(Vector3.zero, out var _, forceReset: true);
		if (tramConsole != null)
		{
			tramConsole.UpdateLeverState(base.pdata.GameState != Hub.PersistentData.eGameState.Prepare);
		}
		netLoopRunner = StartCoroutine(CorNetLoop());
		if (base.pdata.DayCount != 1)
		{
			yield return new WaitUntil(() => EnteringCompleteAll);
		}
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
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.EnterMaintenance);
		IsGameLogicRunning = true;
		yield return WaitForTramStart();
		IsGameLogicRunning = false;
		if (!(Hub.s == null))
		{
			yield return new WaitUntil(() => base.pdata.GameState != Hub.PersistentData.eGameState.Prepare && base.pdata.GameState != Hub.PersistentData.eGameState.Maintenance);
			yield return gameStatusUI.Cor_Hide();
			yield return new WaitUntil(() => base.pdata.serverRoomState == Hub.PersistentData.eServerRoomState.Nowhere);
			yield return TryPlayExitingCutScene();
			base.pdata.DayCount = 1;
			base.pdata.Repaired = false;
			base.pdata.IsPublicLobby = false;
			base.pdata.preLostStashItems.Clear();
			if (base.pdata.GameState == Hub.PersistentData.eGameState.PrepareDone || base.pdata.GameState == Hub.PersistentData.eGameState.LeaveNext)
			{
				yield return new WaitForSeconds(Hub.s.uiman.MaintenanceFadeOutSec);
				Hub.LoadScene("InTramWaitingScene");
			}
			if (base.pdata.GameState == Hub.PersistentData.eGameState.GoDeathMatch)
			{
				yield return new WaitForSeconds(Hub.s.uiman.MaintenanceFadeOutSec);
				Hub.LoadScene("scene_tram_end");
			}
			if (base.pdata.GameState == Hub.PersistentData.eGameState.Restart)
			{
				yield return new WaitForSeconds(Hub.s.uiman.MaintenanceFadeOutSec);
				Hub.LoadScene("MaintenanceScene");
			}
		}
	}

	private IEnumerator WaitForTramStart()
	{
		bool loopFlag = true;
		while (loopFlag && !(Hub.s == null))
		{
			if (hostStartSessionFlag)
			{
				if (_myAvatar != null)
				{
					_myAvatar.DontMove();
				}
				yield return CorStartGameSession();
				if (base.pdata.lastResponseError != MsgErrorCode.Success)
				{
					break;
				}
				hostStartSessionFlag = false;
			}
			if (base.pdata.GameState == Hub.PersistentData.eGameState.PrepareDone)
			{
				if (_myAvatar != null)
				{
					_myAvatar.DontMove();
				}
				break;
			}
			if (base.pdata.GameState == Hub.PersistentData.eGameState.Restart)
			{
				if (_myAvatar != null)
				{
					_myAvatar.DontMove();
				}
				break;
			}
			if (base.pdata.GameState == Hub.PersistentData.eGameState.LeaveNext)
			{
				if (_myAvatar != null)
				{
					_myAvatar.DontMove();
				}
				break;
			}
			if (base.pdata.GameState == Hub.PersistentData.eGameState.GoDeathMatch)
			{
				if (_myAvatar != null)
				{
					_myAvatar.DontMove();
				}
				break;
			}
			interactObjectHelper.Step();
			yield return null;
		}
	}

	public bool TriggerHostStartSession()
	{
		if (base.pdata.GameState == Hub.PersistentData.eGameState.Prepare)
		{
			hostStartSessionFlag = true;
			return true;
		}
		return false;
	}

	public void TryRepairTram()
	{
		if (base.pdata.GameState == Hub.PersistentData.eGameState.Prepare || isRepaired)
		{
			return;
		}
		SendPacketWithCallback(new RepairTramReq(), delegate(RepairTramRes _res)
		{
			if (_res == null)
			{
				base.pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
				Logger.RError("RepairTramRes is null");
			}
			else if (_res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError($"RepairTramRes failed - reason : {_res.errorCode}");
			}
		}, destroyToken);
	}

	private IEnumerator CorNetLoop()
	{
		int frameCounter = 0;
		while (!(Hub.s == null))
		{
			frameCounter++;
			if (_myAvatar != null)
			{
				_myAvatar.OnBeforeTick();
			}
			yield return new WaitForSeconds(1f / base.gameConfig.playerActor.sendPositionFrequency);
		}
	}

	private void CopySessionConfigValueFrom(int cyclecount, int daycount, bool repaired)
	{
		if (cyclecount == 1 && daycount == 1)
		{
			base.pdata.GameState = Hub.PersistentData.eGameState.Prepare;
		}
		else if (cyclecount >= 1 && daycount > 1)
		{
			base.pdata.GameState = (repaired ? Hub.PersistentData.eGameState.RepairDirectionEnd : Hub.PersistentData.eGameState.Maintenance);
		}
		PrepareFirstCycle = cyclecount == 1 && daycount == 1;
		isRepaired = repaired;
	}

	private void ConnectVoiceChat(string address, bool clientWithRelay)
	{
		if (!Hub.s.voiceman.IsConnected())
		{
			if (base.pdata.ClientMode == NetworkClientMode.Host)
			{
				Hub.s.voiceman.StartAsServer();
				Hub.s.voiceman.StartAsClient(address, clientWithRelay: false);
			}
			else
			{
				Hub.s.voiceman.StartAsClient(address, clientWithRelay);
			}
			CrashReportHandler.SetUserMetadata("connect_voice_address", address);
			CrashReportHandler.SetUserMetadata("connect_client_mode", (base.pdata.ClientMode == NetworkClientMode.Host) ? "host" : "remote");
		}
	}

	private IEnumerator CorStartGameSession()
	{
		SendPacketWithCallback(new StartSessionReq(), delegate(StartSessionRes _res)
		{
			if (_res == null)
			{
				base.pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
				Logger.RError("StartGameRes is null");
				goBackToMainMenuFlag = true;
			}
			else if (_res.errorCode != MsgErrorCode.Success)
			{
				base.pdata.lastResponseError = _res.errorCode;
				Logger.RError($"StartGameRes.errorCode : {_res.errorCode}");
				goBackToMainMenuFlag = true;
			}
		}, destroyToken);
		yield return new WaitUntil(() => goBackToMainMenuFlag || base.pdata.GameState == Hub.PersistentData.eGameState.PrepareDone || (base.pdata.IsPublicLobby && base.pdata.GameState == Hub.PersistentData.eGameState.PrepareDoneWithPublicLobby));
		if (goBackToMainMenuFlag)
		{
			OnNetDisconnected(DisconnectReason.ConnectionError, "Failed to start nextgame");
		}
	}

	private void CorEnterDeathMatchByCheat()
	{
		isRepaired = false;
		_prepareFirstCycle = false;
		base.pdata.GameState = Hub.PersistentData.eGameState.GoDeathMatch;
	}

	protected override void OnCurrencyChanged(int prev, int curr)
	{
		base.OnCurrencyChanged(prev, curr);
		if (tramConsole != null)
		{
			tramConsole.UpdateCurrentCurrency(curr);
		}
		if (ui_CurrentCurrency != null)
		{
			ui_CurrentCurrency.UpdateCurrentCurrency(curr);
		}
		if (ui_RepairInfo != null)
		{
			ui_RepairInfo.OnCurrencyChanged(curr, netSyncGameData.targetCurrency);
		}
	}

	public override void OnLeaveRoomSig(LeaveRoomSig sig)
	{
		if (sig.actorID == base.pdata.MyActorID)
		{
			base.pdata.serverRoomState = Hub.PersistentData.eServerRoomState.Nowhere;
			base.pdata.itemPrices = null;
		}
		OnPlayerDespawn(sig.actorID);
	}

	public override void OnCompleteGameSessionSig(CompleteGameSessionSig sig)
	{
	}

	[PacketHandler(false)]
	protected void OnPacket(StartSessionSig sig)
	{
		base.pdata.main.StartCoroutine(CheckPublicTramAndChangeGameState(publicTramOffTextDurationSec, Hub.PersistentData.eGameState.PrepareDone));
	}

	private IEnumerator CheckPublicTramAndChangeGameState(float durationSec, Hub.PersistentData.eGameState gameState)
	{
		yield return CorRefreshSteamLobbyData(null);
		if (base.pdata.ClientMode == NetworkClientMode.Host)
		{
			Hub.s.steamInviteDispatcher.UpdateLobbyData("PublicRoom", "false");
		}
		if (base.pdata.IsPublicLobby)
		{
			base.pdata.GameState = Hub.PersistentData.eGameState.PrepareDoneWithPublicLobby;
			yield return CorNextGameState(durationSec, gameState);
		}
		else
		{
			base.pdata.GameState = gameState;
			crosshairui.Hide();
			Hub.s.audioman.PlaySfx(tramHornAudioKey);
		}
	}

	private IEnumerator CorNextGameState(float durationSec, Hub.PersistentData.eGameState gameState)
	{
		yield return new WaitForSeconds(durationSec);
		base.pdata.GameState = gameState;
		if (_myAvatar != null)
		{
			_myAvatar.DontMove();
		}
		crosshairui.Hide();
		Hub.s.audioman.PlaySfx(tramHornAudioKey);
	}

	[PacketHandler(false)]
	protected void OnPacket(ChangeCurrencySig sig)
	{
		int currency = netSyncGameData.currency;
		netSyncGameData.currency = sig.currency;
		OnCurrencyChanged(currency, sig.currency);
	}

	[PacketHandler(false)]
	protected void OnPacket(StartRepairTramSig sig)
	{
		Logger.RLog("StartRepairTram");
		isRepairing = true;
		isRepaired = true;
		int remainCurrency = sig.remainCurrency;
		netSyncGameData.currency = sig.remainCurrency;
		OnCurrencyChanged(remainCurrency, netSyncGameData.currency);
		if (ui_RepairInfo != null)
		{
			ui_RepairInfo.OnStartRepair();
		}
		if (tramConsole != null)
		{
			tramConsole.UpdateRepairState(isRepaired, isRepairing);
		}
		if (Hub.s != null && Hub.s.tramUpgrade != null)
		{
			Hub.s.tramUpgrade.OnStartRepairTramSig(tramRepairRules);
		}
		base.pdata.GameState = Hub.PersistentData.eGameState.RepairDirection;
		EnteringCutSceneNameQueue.Clear();
		ExitingCutSceneNameQueue.Clear();
	}

	[PacketHandler(false)]
	protected void OnPacket(ChangeTramPartsSig sig)
	{
		Logger.RLog(string.Format("ChangeTramPartsSig pdata.CycleCount = {0}, sig.sessionCount = {1}, sig.upgradeList = [{2}]", base.pdata.CycleCount, sig.sessionCount, string.Join(",", sig.upgradeList)));
		base.pdata.CycleCount = sig.sessionCount + 1;
		base.pdata.DayCount = 1;
		if (Hub.s != null && Hub.s.tramUpgrade != null)
		{
			Hub.s.tramUpgrade.OnChangeTramPartsSig(sig, tramRepairRules);
		}
		base.pdata.TramUpgradeIDs = sig.upgradeList;
		if (tramRepairRules != null)
		{
			tramRepairRules.ApplyDestructionPartsToTramInMaintenanceScene(base.pdata.CycleCount, 0);
			int num = base.pdata.CycleCount - 1;
			tramRepairRules.ApplyNewPartsToTram(num);
			Logger.RLog($"ChangeTramPartsSig ApplyDestructionPartsToTramInMaintenanceScene: REMOVE cycleCount = {base.pdata.CycleCount}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
			Logger.RLog($"ChangeTramPartsSig ApplyNewPartsToTram: cycleCount = {num}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
		}
		Logger.RLog($"ChangeTramPartsSig INC pdata.CycleCount = {base.pdata.CycleCount}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
	}

	[PacketHandler(false)]
	protected void OnPacket(EndRepairTramSig sig)
	{
		Logger.RLog("EndRepairTram");
		isRepairing = false;
		if (ui_RepairInfo != null)
		{
			ui_RepairInfo.OnRepairCompleted();
		}
		if (tramConsole != null)
		{
			tramConsole.UpdateRepairState(isRepaired, isRepairing);
		}
		base.pdata.GameState = Hub.PersistentData.eGameState.RepairDirectionEnd;
		if (Hub.s != null && Hub.s.tramUpgrade != null)
		{
			Hub.s.tramUpgrade.OnEndRepairTramSig(UpgradeSelectLevelObject);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(TramStatusSig sig)
	{
		Logger.RLog($"TramStatusSig - restored : {sig.restored}, status - {sig.status}");
		if (sig.status == TramStatus.NotStarted)
		{
			_actorIdPullingTramStartLever = 0;
		}
		else if (sig.status == TramStatus.EngineStarted)
		{
			_actorIdPullingTramStartLever = sig.triggeredActorID;
		}
		else if (sig.status == TramStatus.Engaged)
		{
			_actorIdPullingTramStartLever = sig.triggeredActorID;
			if (isRepaired)
			{
				base.pdata.main.StartCoroutine(CheckPublicTramAndChangeGameState(publicTramOffTextDurationSec, Hub.PersistentData.eGameState.LeaveNext));
			}
			else
			{
				base.pdata.main.StartCoroutine(CheckPublicTramAndChangeGameState(publicTramOffTextDurationSec, Hub.PersistentData.eGameState.GoDeathMatch));
			}
		}
	}

	protected override void OnCutSceneCompleteSig(CutSceneCompleteSig sig)
	{
		Logger.RLog("CutSceneComplete : " + sig.cutSceneName);
	}

	protected override void OnPlayCutSceneSig(PlayCutSceneSig sig)
	{
		if (base.pdata.GameState == Hub.PersistentData.eGameState.RepairDirection)
		{
			StartCoroutine(CorPlayCutScene(sig.cutSceneName));
		}
		else if (base.pdata.GameState == Hub.PersistentData.eGameState.FailDirection)
		{
			StartCoroutine(CorPlayCutScene(sig.cutSceneName));
		}
	}

	protected override void OnPlayAnimationSig(PlayAnimationSig sig)
	{
	}

	[PacketHandler(false)]
	protected void OnPacket(ReturnMembertoMaintenenceRoomSig sig)
	{
		if (waitForReturnTramDialogBox != null)
		{
			waitForReturnTramDialogBox.GetComponent<UIPrefab_dialogueBox>().OnOK_button?.Invoke("");
			waitForReturnTramDialogBox = null;
		}
		base.pdata.GameState = Hub.PersistentData.eGameState.Maintenance;
		List<object> list = new List<object>();
		list.Add(Hub.GetL10NText("TRAM_WELCOME_BACK"));
		Hub.s.tableman.uiprefabs.ShowTimerDialog("ToastSimple", 0f, list.ToArray());
		base.pdata.itemPrices = sig.itemPrices.Clone();
		foreach (VendingMachineLevelObject vendingMachineLevelObject in Hub.s.dynamicDataMan.GetVendingMachineLevelObjects())
		{
			if (vendingMachineLevelObject != null)
			{
				vendingMachineLevelObject.Refresh();
			}
		}
		netSyncGameData.targetCurrency = sig.targetCurrency;
		OnStashChanged();
		OnCurrencyChanged(0, 0);
		if (tramConsole != null)
		{
			tramConsole.UpdateRepairState(isRepaired: false, isRepairing: false);
		}
	}

	[PacketHandler(false)]
	protected void OnPacket(GameOverSig sig)
	{
		Logger.RLog("GameOver");
		_isGameOver = true;
	}

	[PacketHandler(false)]
	protected void OnPacket(SaveGameDataSig sig)
	{
		Logger.RLog($"SaveGameDataSig auto={sig.auto}");
		if (sig.auto)
		{
			OnSaving(GetMyAvatar(), auto: true);
		}
	}

	private void OnLanguageChanged_dayCount()
	{
		SetDayCountProgressText();
	}

	private void OnPrePlayCutScene(CutScenePlayer.CutScene cutSceneInfo)
	{
		if (cutSceneInfo.name == "SequenceRepair")
		{
			TramCutSceneHelper componentInChildren = GetBGRoot().GetComponentInChildren<TramCutSceneHelper>();
			if (componentInChildren != null)
			{
				componentInChildren.OnPrePlayTramRepairCutScene(cutSceneInfo);
			}
		}
		else if (cutSceneInfo.name == "SequenceRepairBack")
		{
			TramCutSceneHelper componentInChildren2 = GetBGRoot().GetComponentInChildren<TramCutSceneHelper>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.OnPrePlayTramRepairBackCutScene(cutSceneInfo);
			}
		}
	}

	private void OnPostPlayCutScene(CutScenePlayer.CutScene cutSceneInfo)
	{
		if (cutSceneInfo.name == "SequenceRepair")
		{
			TramCutSceneHelper componentInChildren = GetBGRoot().GetComponentInChildren<TramCutSceneHelper>();
			if (componentInChildren != null)
			{
				componentInChildren.OnPostPlayTramRepairCutScene(cutSceneInfo);
			}
		}
		else if (cutSceneInfo.name == "SequenceRepairBack")
		{
			TramCutSceneHelper componentInChildren2 = GetBGRoot().GetComponentInChildren<TramCutSceneHelper>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.OnPostPlayTramRepairBackCutScene(cutSceneInfo);
			}
		}
	}

	public void SelectUpgrade(int index)
	{
		int selectedUpgradeMasterID = 0;
		switch (index)
		{
		case 0:
			selectedUpgradeMasterID = base.pdata.TramUpgradeCandidate.leftPanel;
			break;
		case 1:
			selectedUpgradeMasterID = base.pdata.TramUpgradeCandidate.rightPanel;
			break;
		}
		SendPacketWithCallback(new PickTramUpgradeReq
		{
			selectedUpgradeMasterID = selectedUpgradeMasterID
		}, delegate(PickTramUpgradeRes res)
		{
			if (res == null)
			{
				Logger.RError("PickTramUpgradeRes is null.");
			}
			else if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError($"PickTramUpgradeReq failed : {res.errorCode}");
			}
		}, destroyToken);
	}

	[PacketHandler(false)]
	private void OnPacket(PickTramUpgradeSig sig)
	{
		int num = -1;
		if (sig.selectedUpgradeMasterID == base.pdata.TramUpgradeCandidate.leftPanel)
		{
			num = 0;
		}
		else if (sig.selectedUpgradeMasterID == base.pdata.TramUpgradeCandidate.rightPanel)
		{
			num = 1;
		}
		if (num != -1 && UpgradeSelectLevelObject != null)
		{
			UpgradeSelectLevelObject.ChangeSelected(num);
		}
	}
}
