using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mimic.Actors;
using ModUtility;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluReplay.Shared;
using UnityEngine;

public class DeathMatchScene : GameMainBase
{
	[SerializeField]
	private VideoCutScenePlayer deathMatchEnteringCutScenePlayer;

	[SerializeField]
	protected Transform ActorRoot;

	private Coroutine? mainLoopRunner;

	private Coroutine netLoopRunner;

	private bool isEndDungeon;

	[SerializeField]
	private BoxCollider outdoorCollider;

	public bool IsGameStarted;

	public BoxCollider? OutdoorBoxCollier => outdoorCollider;

	public Transform GetActorRoot()
	{
		return ActorRoot;
	}

	protected override void Awake()
	{
		if (!(Hub.s == null))
		{
			base.Awake();
		}
	}

	protected override void OnDestroy()
	{
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.ExitDeathMatch);
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
		if (!(Hub.s == null))
		{
			base.Start();
			StartSceneLoading("DeathMatch");
			base.pdata.serverRoomState = Hub.PersistentData.eServerRoomState.InGame;
			base.pdata.GameState = Hub.PersistentData.eGameState.InGame;
			InitCommonUI();
			CreateSpectatorHUD();
			mainLoopRunner = StartCoroutine(CorRun());
		}
	}

	private new void BuildNavMesh()
	{
		Hub.s.navman.Build();
	}

	private void TryInitHostDeathMatchRoom()
	{
		if (base.pdata.ClientMode == NetworkClientMode.Host)
		{
			Logger.RLog("InitDeathMatchRoom by host");
			Hub.s.vworld?.InitDeathMatchRoom();
		}
	}

	private IEnumerator TryEnterDeathMatchRoom()
	{
		Logger.RLog("EnterDeathMatchRoom");
		EnterDeathMatchRoomRes enterDeathMatchRoomRes = null;
		SendPacketWithCallback(new EnterDeathMatchRoomReq(), delegate(EnterDeathMatchRoomRes _res)
		{
			enterDeathMatchRoomRes = _res;
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
				Logger.RLog($"EnterDeathMatchRoom completed: id={base.pdata.MyActorID}");
			}
		}, destroyToken, 60000, disconnectWhenTimeout: true);
		yield return new WaitUntil(() => goBackToMainMenuFlag || enterDeathMatchRoomRes != null);
		if (goBackToMainMenuFlag)
		{
			OnNetDisconnected(DisconnectReason.ConnectionError, "Failed to EnterDeathMatchRoom");
		}
	}

	private IEnumerator CorShowCommonUI()
	{
		yield return null;
		ShowCommonUI();
	}

	private IEnumerator CorRun()
	{
		base.pdata.ResetCycleInfos();
		base.pdata.enableDeathMatchRoomSig = null;
		EnteringCompleteAll = false;
		TryInitHostDeathMatchRoom();
		if (CheckNetworkConnection())
		{
			yield break;
		}
		base.netman2.PurgeMsg();
		BuildNavMesh();
		Hub.s.dynamicDataMan.Build(BGRoot);
		interactObjectHelper.InitAllLevelObjects();
		Hub.s.dLAcademyManager.GetAreaForDL(Vector3.zero, out var _, forceReset: true);
		if (ReplaySharedData.IsReplayPlayMode)
		{
			EndSceneLoading();
			Hub.s.replayManager.OnGamePlaySceneLoadedComplete();
		}
		else
		{
			Logger.RLog("Waiting EnableDeathMatchRoom");
			yield return new WaitUntil(() => base.pdata.enableDeathMatchRoomSig != null);
			yield return TryEnterDeathMatchRoom();
			yield return SpawnMyAvatar();
			_myAvatar.DontMove();
			yield return TryLevelLoad();
			if (base.pdata.lastResponseError != MsgErrorCode.Success)
			{
				yield break;
			}
			SetGameStatusUI();
			netLoopRunner = StartCoroutine(CorNetLoop());
			Logger.RLog("Waiting EnteringCompleteAll in DeathMatchScene");
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
			ShowCommonUI();
			if (crosshairui != null)
			{
				crosshairui.Show();
			}
			PlayDeathMatchSfx();
			_myAvatar.CancelDontMove();
		}
		ModHelper.InvokeTimingCallback(ModHelper.eTiming.EnterDeathMatch);
		IsGameLogicRunning = true;
		while (true)
		{
			if (Hub.s == null)
			{
				yield break;
			}
			if (ExitingCutSceneNameQueue.Count > 0)
			{
				break;
			}
			interactObjectHelper.Step();
			_ = (float)Hub.s.dataman.ExcelDataManager.Consts.C_GameTimeScaleFactor / 1000f;
			yield return null;
		}
		IsGameLogicRunning = false;
		yield return TryPlayExitingCutScene();
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
		if (gameStatusUI != null)
		{
			gameStatusUI.AddMember(actor.netSyncActorData.actorName);
		}
		spawnedPlayerActorList.Add(actor);
		if (actor.AmIAvatar())
		{
			Hub.s.voiceman.SetVoiceMode(VoiceMode.Player);
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
	}

	public override void OnLeaveRoomSig(LeaveRoomSig sig)
	{
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
		base.pdata.ResetCycleInfos();
		Hub.LoadScene("MaintenanceScene");
	}

	public override bool IsGameSessionEnd()
	{
		return isEndDungeon;
	}

	[PacketHandler(false)]
	private void OnPacket(EndDeathMatchSig sig)
	{
		List<DeathMatchPlayerResult> list = sig.deathMatchPlayerResults.Values.ToList();
		string l10NText = Hub.GetL10NText("UI_PREFAB_SURVIVAL_RESULT_SURVIVAL");
		string l10NText2 = Hub.GetL10NText("UI_BATTLE_DEATH_REASON_SURVIVAL");
		string l10NText3 = Hub.GetL10NText("UI_BATTLE_DEATH_REASON_MIMIC");
		string l10NText4 = Hub.GetL10NText("UI_BATTLE_DEATH_REASON_PLAYER");
		List<object> list2 = new List<object>();
		foreach (DeathMatchPlayerResult item3 in list)
		{
			ProtoActor actorByActorID = base.pdata.main.GetActorByActorID(item3.actorID);
			if (actorByActorID == null)
			{
				Logger.RError($"Actor with ID {item3.actorID} not found in EndDeathMatchSig processing.");
				list2.Add("Unknown");
				list2.Add(false);
				list2.Add("Unknown");
				continue;
			}
			list2.Add(actorByActorID.nickName);
			if (item3.reasonofDead.actorType == ActorType.Player)
			{
				ProtoActor actorByActorID2 = base.pdata.main.GetActorByActorID(item3.reasonofDead.actorID);
				if (actorByActorID2 != null)
				{
					string item = l10NText4.Replace("[usernickname:]", actorByActorID2.nickName);
					list2.Add(false);
					list2.Add(item);
				}
				else
				{
					string item2 = l10NText4.Replace("[usernickname:]", "Unknown");
					list2.Add(false);
					list2.Add(item2);
				}
			}
			else if (item3.reasonofDead.actorType == ActorType.Monster)
			{
				if (Hub.s.dataman.ExcelDataManager.GetMonsterInfo(item3.reasonofDead.masterID) != null)
				{
					list2.Add(false);
					list2.Add(l10NText3);
				}
				else
				{
					list2.Add(false);
					list2.Add(l10NText3);
				}
			}
			else if (item3.reasonofDead.actorType == ActorType.None)
			{
				list2.Add(true);
				list2.Add(l10NText);
			}
			else
			{
				list2.Add(false);
				list2.Add(l10NText2);
			}
		}
		float overrideDurationSec = (float)Hub.s.dataman.ExcelDataManager.Consts.C_UI_PopupDurationDeathmatchReport * 0.001f;
		Hub.s.tableman.uiprefabs.ShowTimerDialog("DeathMatchResult", overrideDurationSec, list2.ToArray());
	}

	[PacketHandler(false)]
	protected void OnPacket(DeathMatchRoomScoreBoardSig sig)
	{
		if (sig != null && sig.deathMatchScordBoards.TryGetValue(base.pdata.MyActorID, out var value))
		{
			OnKillCountChanged(_myAvatar, value.killCount);
		}
		else
		{
			Logger.RError($"DeathMatchRoomScoreBoardSig received but no score board found for actorID: {base.pdata.MyActorID}");
		}
	}
}
