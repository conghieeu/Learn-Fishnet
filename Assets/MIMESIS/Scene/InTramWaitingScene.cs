using System;
using System.Collections;
using System.Collections.Generic;
using Bifrost.Cooked;
using ModUtility;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluReplay.Shared;
using UnityEngine;

public class InTramWaitingScene : GameMainBase
{
	[SerializeField]
	private UIPrefab_MapSelector uiPrefab_MapSelector;

	[SerializeField]
	private MapSelector mapSelector;

	[SerializeField]
	private UIPrefab_RepairScreenInTram uiPrefab_RepairScreenInTram;

	[SerializeField]
	private GuidanceAlarm guidanceAlarm;

	private bool startGameTriggered;

	private bool endSessionTriggered;

	private bool hostStartGameWithCheat;

	private int dungeonIdWithCheat;

	private StartGameSig? startGameSig;

	private EndSessionSig? endSessionSig;

	private Coroutine netLoopRunner;

	private bool resettingRoomFlag;

	private List<int> dungeonCandidateIDs = new List<int>();

	private int dungeonIndex = -1;

	private bool pullingTramStartLever;

	private bool sessionJoined => base.pdata.SessionJoined;

	public bool IsHost => base.pdata.ClientMode == NetworkClientMode.Host;

	public List<int> GetDunGeonCandidateIDs()
	{
		return dungeonCandidateIDs;
	}

	public int GetDungeonIndex()
	{
		return dungeonIndex;
	}

	protected override void Start()
	{
		if (Hub.s == null)
		{
			return;
		}
		base.Start();
		StartSceneLoading("InTramWaiting");
		InitCommonUI();
		if (IsHost)
		{
			if (ReplaySharedData.GetMode() != ReplaySharedData.E_MODE.RECORD)
			{
				ReplaySharedData.SetRecordMode();
			}
		}
		else
		{
			ReplaySharedData.SetNormalMode();
		}
		StartCoroutine(CorRun());
		Hub.s.lcman.onLanguageChanged += SetRemainDays;
	}

	protected override void OnDestroy()
	{
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.ExitTram);
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
				Hub.s.lcman.onLanguageChanged -= SetRemainDays;
			}
		}
	}

	private void TryInitHostWaitingRoom()
	{
		if (base.pdata.ClientMode == NetworkClientMode.Host)
		{
			Hub.s.vworld?.InitWaitingRoom();
		}
	}

	private IEnumerator CorRun()
	{
		Logger.RLog($"InTramWaitingScene CorRun: dayCount = {base.pdata.DayCount}, cycleCount = {base.pdata.CycleCount}");
		if (tramRepairRules != null)
		{
			tramRepairRules.ApplyDestructionPartsToTramInTramWaitingScene(base.pdata.CycleCount, base.pdata.DayCount);
			int num = base.pdata.CycleCount - 1;
			tramRepairRules.ApplyNewPartsToTram(num);
			Logger.RLog($"ApplyDestructionPartsToTramInTramWaitingScene: {base.pdata.CycleCount}, {base.pdata.DayCount}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
			Logger.RLog($"ApplyNewPartsToTram: {num}", sendToLogServer: false, useConsoleOut: true, "sceneflow");
			tramRepairRules.ApplySteamItemPartsToTramInCommon(base.pdata.AppliedTramSkin);
			Hub.s.tramUpgrade.ApplyTramUpgradePartsToTramAtSceneInit(tramRepairRules);
		}
		Hub.s.dynamicDataMan.Build(BGRoot);
		interactObjectHelper.InitAllLevelObjects();
		BuildNavMesh();
		TryInitHostWaitingRoom();
		yield return CorEnterWaitingScene();
	}

	private IEnumerator TryEnterWaitingRoom()
	{
		Logger.RLog("EnterWaitingRoom");
		EnterWaitingRoomRes enterWaitingRoomRes = null;
		SendPacketWithCallback(new EnterWaitingRoomReq
		{
			playerUID = base.pdata.PlayerUID
		}, delegate(EnterWaitingRoomRes res)
		{
			if (res == null)
			{
				goBackToMainMenuFlag = true;
				base.pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
				Logger.RError("EnterWaitingRoomRes is null.");
			}
			else if (res.errorCode != MsgErrorCode.Success)
			{
				goBackToMainMenuFlag = true;
				base.pdata.lastResponseError = res.errorCode;
			}
			else
			{
				enterWaitingRoomRes = res;
				base.pdata.nextGameDungeonMasterIDs.Clear();
				base.pdata.nextGameDungeonMasterIDs.AddRange(res.nextGameDungeonMasterIDs);
				base.pdata.MyActorID = res.myVPlayerInfo.actorID;
				if (base.pdata.ClientRoomFirstCycle && base.pdata.DayCount == 1)
				{
					Hub.s.apihandler.EnqueueAPI<APIEnterTramLogRes>(new APIEnterTramLogReq
					{
						guid = base.pdata.GUID,
						sessionID = base.pdata.ClientSessionID,
						roomSessionID = base.pdata.ClientRoomSessionID
					}, delegate(IResMsg resMsg)
					{
						if (resMsg.errorCode != MsgErrorCode.Success)
						{
							Logger.RError($"APIEnterTramLogReq failed : {resMsg.errorCode}");
						}
					});
				}
			}
		}, destroyToken, 60000, disconnectWhenTimeout: true);
		yield return new WaitUntil(() => goBackToMainMenuFlag || enterWaitingRoomRes != null);
		if (goBackToMainMenuFlag)
		{
			OnNetDisconnected(DisconnectReason.ConnectionError, "Failed to EnterWaitingRoom");
		}
	}

	private void ResolveWaitingRoomInfo()
	{
		Logger.RLog("ResolveWaitingRoomInfo");
		UpdateDungeonCandidates(base.pdata.nextGameDungeonMasterIDs);
		netSyncGameData.dayCount = base.pdata.DayCount;
	}

	private bool IsSessionEnd()
	{
		return netSyncGameData.dayCount - 1 == (int)Hub.s.dataman.ExcelDataManager.Consts.C_AvailableDayPerSession;
	}

	private IEnumerator CorEnterWaitingScene()
	{
		startGameTriggered = false;
		hostStartGameWithCheat = false;
		endSessionTriggered = false;
		base.pdata.GameState = Hub.PersistentData.eGameState.Waiting;
		if (CheckNetworkConnection())
		{
			yield break;
		}
		base.netman2.PurgeMsg();
		Logger.RLog("Waiting EnableWaitingRoom");
		yield return new WaitUntil(() => base.pdata.enableWaitingRoomSig != null);
		yield return TryEnterWaitingRoom();
		base.pdata.enableWaitingRoomSig = null;
		base.pdata.serverRoomState = Hub.PersistentData.eServerRoomState.PreGame;
		Hub.s.voiceman.SetVoiceMode(VoiceMode.PreGame);
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
		ResolveWaitingRoomInfo();
		ConfigureScreenInTram();
		Hub.s.dLAcademyManager.GetAreaForDL(Vector3.zero, out var _, forceReset: true);
		if (tramConsole != null)
		{
			tramConsole.UpdateLeverState(maintenanceDecision: false);
			tramConsole.UpdateWantedPoster(netSyncGameData.boostedItem, netSyncGameData.boostedRatio);
		}
		yield return new WaitForSeconds(0.1f);
		InitCommonUIValue();
		SetGameStatusUI();
		SetRemainDays();
		SetScreenInTram();
		netLoopRunner = StartCoroutine(CorNetLoop());
		Logger.RLog("Waiting EnteringCompleteAll in InTramWaitingScene");
		yield return new WaitUntil(() => EnteringCompleteAll);
		SetEnableInputForMyAvatar();
		yield return WaitForMinimumLoadingTime();
		if (EnteringCutSceneNameQueue.Count > 0)
		{
			StartCoroutine(TryPlayEnteringCutScene());
		}
		else
		{
			EndSceneLoading();
		}
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.EnterTram);
		IsGameLogicRunning = true;
		Logger.RLog("InTramWaitingScene Started");
		yield return WaitForNextScene();
		IsGameLogicRunning = false;
		if (Hub.s == null)
		{
			yield break;
		}
		Hub.s.audioman.PlaySfx(tramHornAudioKey);
		yield return TryPlayExitingCutScene();
		yield return gameStatusUI.Cor_Hide();
		if (IsSessionEnd())
		{
			yield return new WaitForSeconds(Hub.s.uiman.WaitingRoomFadeOutSec);
			Hub.LoadScene("MaintenanceScene");
			yield break;
		}
		Logger.RLog("Waiting StartGameSig in InTramWaitingScene");
		yield return new WaitUntil(() => startGameSig != null);
		string mapName = TryGetMapName(startGameSig);
		if (mapName.Length > 0)
		{
			yield return new WaitForSeconds(Hub.s.uiman.WaitingRoomFadeOutSec);
			Hub.LoadScene(mapName);
		}
	}

	private void SetRemainDays()
	{
		int dayCount = netSyncGameData.dayCount;
		if (tramConsole != null)
		{
			tramConsole.SetDayCount(dayCount);
		}
	}

	private IEnumerator TryStartGame()
	{
		StartGameRes startGameRes = null;
		SendPacketWithCallback(new StartGameReq
		{
			selectedDungeonMasterID = (hostStartGameWithCheat ? dungeonIdWithCheat : dungeonCandidateIDs[dungeonIndex])
		}, delegate(StartGameRes _res)
		{
			if (_res == null)
			{
				base.pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
				Logger.RError("StartGameRes is null.");
				goBackToMainMenuFlag = true;
			}
			else if (_res.errorCode != MsgErrorCode.Success && _res.errorCode != MsgErrorCode.AlreadyTriggered)
			{
				startGameRes = _res;
				base.pdata.lastResponseError = _res.errorCode;
				Logger.RError($"StartGameRes.errorCode : {_res.errorCode}");
				goBackToMainMenuFlag = true;
			}
			else
			{
				startGameRes = _res;
			}
		}, destroyToken);
		yield return new WaitUntil(() => goBackToMainMenuFlag || startGameRes != null);
		if (goBackToMainMenuFlag)
		{
			OnNetDisconnected(DisconnectReason.ConnectionError, "Failed to start game");
		}
	}

	private IEnumerator TryEndSession()
	{
		EndSessionRes endSessionRes = null;
		SendPacketWithCallback(new EndSessionReq(), delegate(EndSessionRes _res)
		{
			endSessionRes = _res;
			if (_res == null)
			{
				base.pdata.lastResponseError = MsgErrorCode.ClientErrorSendTimeout;
				Logger.RError("EndSessionRes is null.");
				goBackToMainMenuFlag = true;
			}
			else if (_res.errorCode != MsgErrorCode.Success)
			{
				base.pdata.lastResponseError = _res.errorCode;
				Logger.RError($"EndSessionRes.errorCode : {_res.errorCode}");
				goBackToMainMenuFlag = true;
			}
		}, destroyToken);
		yield return new WaitUntil(() => goBackToMainMenuFlag || endSessionRes != null);
		if (goBackToMainMenuFlag)
		{
			OnNetDisconnected(DisconnectReason.ConnectionError, "Failed to end session");
		}
	}

	private IEnumerator WaitForNextScene()
	{
		bool loopFlag = true;
		while (loopFlag && !(Hub.s == null))
		{
			if (startGameTriggered)
			{
				if (_myAvatar != null)
				{
					_myAvatar.DontMove();
				}
				if (!IsSessionEnd())
				{
					yield return TryStartGame();
				}
				startGameTriggered = false;
				break;
			}
			if (endSessionTriggered)
			{
				if (_myAvatar != null)
				{
					_myAvatar.DontMove();
				}
				yield return TryEndSession();
				if (IsSessionEnd())
				{
					break;
				}
				Logger.RError("can't exist");
				endSessionTriggered = false;
			}
			if (base.pdata.serverRoomState == Hub.PersistentData.eServerRoomState.Nowhere)
			{
				if (startGameSig != null || endSessionSig != null)
				{
					break;
				}
				if (resettingRoomFlag)
				{
					resettingRoomFlag = false;
					yield return new WaitForSeconds(Hub.s.uiman.WaitingRoomFadeOutSec);
					Hub.LoadScene("MaintenanceScene");
				}
			}
			interactObjectHelper.Step();
			yield return null;
		}
	}

	private string TryGetMapName(StartGameSig startGameSig)
	{
		int selectedDungeonMasterID = startGameSig.selectedDungeonMasterID;
		DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(selectedDungeonMasterID);
		if (dungeonInfo == null)
		{
			Logger.RError($"dungeonInfo is null. dungeonID = {selectedDungeonMasterID}");
			return "";
		}
		int mapID = dungeonInfo.MapID;
		MapMasterInfo mapInfo = Hub.s.dataman.ExcelDataManager.GetMapInfo(mapID);
		if (mapInfo == null)
		{
			Logger.RError($"roomInfo is null. mapID = {mapID}");
			return "";
		}
		string sceneName = mapInfo.SceneName;
		base.pdata.randDungeonSeed = startGameSig.randDungeonSeed;
		base.pdata.dungeonMasterID = startGameSig.selectedDungeonMasterID;
		return sceneName;
	}

	public bool TriggerHostStartGame()
	{
		if (!IsSessionEnd())
		{
			startGameTriggered = true;
		}
		else
		{
			endSessionTriggered = true;
		}
		crosshairui.Hide();
		return true;
	}

	public void SetPullingTramStartLever(bool pulling)
	{
		pullingTramStartLever = pulling;
		interactObjectHelper.NotifyTramStartLeverPullingStarted();
	}

	public bool IsPullingTramStartLever()
	{
		return pullingTramStartLever;
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

	private void ConfigureScreenInTram()
	{
		if (IsSessionEnd())
		{
			uiPrefab_MapSelector.Hide();
			uiPrefab_RepairScreenInTram.Show();
			if (guidanceAlarm != null)
			{
				guidanceAlarm.PlayDamagedTramAlarm();
			}
		}
		else
		{
			uiPrefab_RepairScreenInTram.Hide();
			uiPrefab_MapSelector.Show();
		}
	}

	private void SetScreenInTram()
	{
		if (IsSessionEnd())
		{
			uiPrefab_RepairScreenInTram.UpdateRepairState(isRepaired: false, isRepairing: false);
		}
		else
		{
			UpdateMapSelector();
		}
	}

	public bool MapChange(int index)
	{
		if (dungeonIndex == index)
		{
			SyncSelectedMap(index);
			return false;
		}
		int prevDungeonIndex = dungeonIndex;
		SendPacketWithCallback(new ChangeNextDungeonReq
		{
			selectedDungeonMasterID = dungeonCandidateIDs[index]
		}, delegate(ChangeNextDungeonRes res)
		{
			if (res.errorCode != MsgErrorCode.Success)
			{
				dungeonIndex = prevDungeonIndex;
			}
			else if (dungeonIndex != index)
			{
				dungeonIndex = index;
				SyncSelectedMap(dungeonIndex);
			}
		}, destroyToken);
		return true;
	}

	public void UpdateDungeonCandidates(List<int> dungeonCandidates)
	{
		dungeonCandidateIDs.Clear();
		dungeonCandidateIDs.AddRange(dungeonCandidates);
		if (uiPrefab_MapSelector != null && uiPrefab_MapSelector.isActiveAndEnabled)
		{
			UpdateMapSelector();
		}
	}

	private void UpdateMapSelector()
	{
		dungeonIndex = 0;
		uiPrefab_MapSelector.UpdateDate(netSyncGameData.dayCount);
		uiPrefab_MapSelector.ChangeMap(dungeonCandidateIDs[0], dungeonCandidateIDs[1]);
		if (base.pdata.DayCount != 4)
		{
			InitSelectedMap(dungeonIndex);
		}
	}

	private void InitSelectedMap(int newIndex)
	{
		uiPrefab_MapSelector.SelectMap(newIndex);
		if (mapSelector != null)
		{
			mapSelector.ChangeSelected(newIndex);
		}
	}

	private void SyncSelectedMap(int newIndex)
	{
		uiPrefab_MapSelector.SelectMap(newIndex);
		if (mapSelector != null)
		{
			mapSelector.OnSelectMap(newIndex);
		}
	}

	public void RerollMap(Action<bool> successCallback)
	{
		SendPacketWithCallback(new RollDungeonReq(), delegate(RollDungeonRes res)
		{
			if (res == null)
			{
				Logger.RError("RollDungeonRes is null.");
				successCallback?.Invoke(obj: false);
			}
			else if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError($"RollDungeonRes.errorCode : {res.errorCode}");
				successCallback?.Invoke(obj: false);
			}
			else
			{
				successCallback?.Invoke(obj: true);
				Logger.RLog("RollDungeonRes success.");
			}
		}, destroyToken);
	}

	public override void OnLeaveRoomSig(LeaveRoomSig sig)
	{
		if (sig.actorID == base.pdata.MyActorID)
		{
			base.pdata.serverRoomState = Hub.PersistentData.eServerRoomState.Nowhere;
			base.pdata.completeMakingRoomSig = null;
		}
		OnPlayerDespawn(sig.actorID);
	}

	[PacketHandler(false)]
	protected void OnPacket(StartGameSig sig)
	{
		Logger.RLog("Received StartGameSig in InTramWaitingScene");
		startGameSig = sig;
	}

	[PacketHandler(false)]
	protected void OnPacket(EndSessionSig sig)
	{
		Logger.RLog("Received EndSessionSig in InTramWaitingScene");
		endSessionSig = sig;
	}

	[PacketHandler(false)]
	protected void OnPacket(ChangeNextDungeonSig sig)
	{
		int num = dungeonIndex;
		dungeonIndex = dungeonCandidateIDs.FindIndex((int did) => did == sig.selectedDungeonMasterID);
		if (dungeonIndex >= 0 && num != dungeonIndex)
		{
			SyncSelectedMap(dungeonIndex);
		}
	}
}
