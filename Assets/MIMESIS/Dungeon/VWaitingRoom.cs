using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluServerBase.Threading;

public class VWaitingRoom : IVroom
{
	private WaitingRoomState _state;

	private bool _leverTriggered;

	private bool _rolled;

	public int CurrentSelectedDungeonMasterID { get; private set; }

	public int NextCandidateDungeonMasterID { get; private set; }

	public VWaitingRoom(VRoomManager roomManager, long roomID, IVRoomProperty property)
		: base(roomManager, roomID, property)
	{
	}

	public override bool Initialize()
	{
		_currentDay = 1;
		if (!base.Initialize())
		{
			return false;
		}
		RollDiceDungeon();
		return true;
	}

	protected override MsgErrorCode CanEnterChannel(long playerUID)
	{
		return base.CanEnterChannel(playerUID);
	}

	public override MsgErrorCode OnEnterChannel(VPlayer player, int hashCode)
	{
		base.OnEnterChannel(player, hashCode);
		EnterWaitingRoomRes enterWaitingRoomRes = new EnterWaitingRoomRes(hashCode)
		{
			nextGameDungeonMasterIDs = new List<int> { CurrentSelectedDungeonMasterID, NextCandidateDungeonMasterID },
			memberList = _vPlayerDict.Values.Select((VPlayer x) => x.GetPlayerInfo()).ToList()
		};
		PlayerInfo info = enterWaitingRoomRes.myVPlayerInfo;
		player.GetMyActorInfo(ref info);
		player.SendToMe(enterWaitingRoomRes);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode OnRequestEndSession()
	{
		OnDecideRoom(delegate
		{
			SendToAllPlayers(new EndSessionSig());
			Hub.s.vworld?.PendEndGame(_vPlayerDict.ToDictionary<KeyValuePair<int, VPlayer>, ulong, long>((KeyValuePair<int, VPlayer> x) => x.Value.SteamID, (KeyValuePair<int, VPlayer> x) => x.Value.UID), ExtractRoomInfo());
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode OnRequestChangeNextDungeonID(int selectedDungeonMasterID)
	{
		if (_state != WaitingRoomState.Ready)
		{
			Logger.RError($"OnRequestChangeNextDungeonID failed, state is not Open. state: {_state}");
			return MsgErrorCode.InvalidRoomState;
		}
		CurrentSelectedDungeonMasterID = selectedDungeonMasterID;
		SendToAllPlayers(new ChangeNextDungeonSig
		{
			selectedDungeonMasterID = CurrentSelectedDungeonMasterID
		});
		return MsgErrorCode.Success;
	}

	public override int GetContaValue(VActor actor, bool isRun)
	{
		return 0;
	}

	public MsgErrorCode OnRequestStartGame(int forceDungeonMasterID = 0)
	{
		if (_leverTriggered)
		{
			return MsgErrorCode.AlreadyTriggered;
		}
		_leverTriggered = true;
		if (_state != WaitingRoomState.Ready)
		{
			Logger.RError($"OnRequestStartGame failed, state is not Open. state: {_state}");
			return MsgErrorCode.InvalidRoomState;
		}
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (currentTickMilliSec - _resumeTime < 1000)
		{
			Logger.RError($"OnRequestStartGame failed, not enough time passed. currentTime: {currentTickMilliSec}, _resumeTime: {_resumeTime}");
			return MsgErrorCode.NotEnoughTimePassed;
		}
		OnDecideRoom(delegate
		{
			int num = ((forceDungeonMasterID == 0) ? CurrentSelectedDungeonMasterID : forceDungeonMasterID);
			int num2 = SimpleRandUtil.Next(0, int.MaxValue);
			Hub.s.vworld?.ReadyToGamePktRecording(num, num2);
			SendToAllPlayers(new StartGameSig
			{
				selectedDungeonMasterID = num,
				randDungeonSeed = num2
			});
			RollDiceDungeon();
			Hub.s.vworld.PendStartGame(_vPlayerDict.ToDictionary<KeyValuePair<int, VPlayer>, ulong, long>((KeyValuePair<int, VPlayer> x) => x.Value.SteamID, (KeyValuePair<int, VPlayer> x) => x.Value.UID), num, num2, ExtractRoomInfo());
		});
		return MsgErrorCode.Success;
	}

	public void Wipeout(bool backup, bool reset)
	{
		_commandExecutor.Invoke(delegate
		{
			if (reset)
			{
				foreach (VActor item in _vActorDict.Values.Where((VActor x) => x is VLootingObject).ToList())
				{
					PendRemoveActor(item.ObjectID);
				}
			}
			Vacate(backup);
		});
	}

	public override void OnVacateRoom(bool backup)
	{
		base.OnVacateRoom(backup);
		_leverTriggered = false;
	}

	public bool RollDiceDungeon()
	{
		List<int> dungeonCandidateMasterID = Hub.s.dataman.ExcelDataManager.GetDungeonCandidateMasterID(_currentSessionCount);
		if (dungeonCandidateMasterID.Count == 0)
		{
			Logger.RError($"RollDiceDungeon failed, candidates is empty. _currentSessionCount: {_currentSessionCount}");
			return false;
		}
		CurrentSelectedDungeonMasterID = dungeonCandidateMasterID[SimpleRandUtil.Next(0, dungeonCandidateMasterID.Count)];
		dungeonCandidateMasterID.Remove(CurrentSelectedDungeonMasterID);
		NextCandidateDungeonMasterID = dungeonCandidateMasterID[SimpleRandUtil.Next(0, dungeonCandidateMasterID.Count)];
		return true;
	}

	public void SetState(WaitingRoomState state)
	{
		_state = state;
	}

	protected override long GetContaRecoveryRate()
	{
		return 0L;
	}

	protected override void OnEnterRoomFailed(SessionContext context, MsgErrorCode errorCode, int hashCode)
	{
		context.Send(new EnterWaitingRoomRes(hashCode)
		{
			errorCode = errorCode
		});
	}

	public override void ResetEnvironment(bool failed = false)
	{
		base.ResetEnvironment(failed);
		if (failed)
		{
			CurrentSelectedDungeonMasterID = 0;
			NextCandidateDungeonMasterID = 0;
		}
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
		ImmutableArray<IGameAction> enterAction = cycleMasterInfo.WaitingRoomCycleInfo.GetEnterAction(_currentDay);
		if (enterAction.Length > 0)
		{
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator = enterAction.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IGameAction current = enumerator.Current;
				dictionary.Add(current.Clone(), null);
			}
			EnqueueEventAction(dictionary, 0);
		}
	}

	protected override void OnDecideRoom(Command command)
	{
		base.OnDecideRoom(command);
		Stop();
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
		List<string> enterCutSceneNames = cycleMasterInfo.WaitingRoomCycleInfo.GetEnterCutSceneNames(_currentDay);
		Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
		foreach (string item in enterCutSceneNames)
		{
			dictionary.Add(new GameActionPlayCutscene(item, needToBroadCast: false), null);
		}
		dictionary.Add(new GameActionEnableAI(), null);
		EnqueueEventAction(dictionary, 0);
		EnableAIControl(enable: false);
	}

	protected override void SyncEnterRoom()
	{
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(_currentSessionCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError($"SendAllMembersEnterRoom failed, GetCycleMasterInfo failed. _currentSessionCount: {_currentSessionCount}");
			return;
		}
		List<string> enterCutSceneNames = cycleMasterInfo.WaitingRoomCycleInfo.GetEnterCutSceneNames(_currentDay);
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
		List<string> exitCutSceneNames = cycleMasterInfo.WaitingRoomCycleInfo.GetExitCutSceneNames(_currentDay);
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

	public override VRoomType GetToMoveRoomType()
	{
		if (_state != WaitingRoomState.DecisionNextGame)
		{
			return VRoomType.Game;
		}
		return VRoomType.Maintenance;
	}

	public MsgErrorCode RollDungeonByRequest(VPlayer triggeredPlayer, int hashCode)
	{
		if (_rolled)
		{
			return MsgErrorCode.AlreadyTriggered;
		}
		_rolled = true;
		RollDiceDungeon();
		triggeredPlayer.SendToMe(new RollDungeonRes
		{
			hashCode = hashCode
		});
		SendToAllPlayers(new RollDungeonSig
		{
			newDungeonMasterIDs = (CurrentSelectedDungeonMasterID, NextCandidateDungeonMasterID)
		});
		return MsgErrorCode.Success;
	}
}
