using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluServerBase.Threading;
using UnityEngine;

public class VRoomManager
{
	private Dictionary<long, IVroom> _vrooms = new Dictionary<long, IVroom>();

	private CommandExecutor _commandExecutor;

	private long _roomIDGenerator;

	private long _itemIDGenerator;

	private GameSessionInfo _gameSessionInfo = new GameSessionInfo();

	private EventTimer _eventTimer = new EventTimer();

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	public VRoomManager()
	{
		_commandExecutor = CommandExecutor.CreateCommandExecutor(GetType().ToString(), 0L);
	}

	public void Initialize()
	{
		_gameSessionInfo.Reset(VGameSessionState.Invalid, init: true);
	}

	public void InitMaintenenceRoom(string hostToken, int saveSlotID, string hostPlayerName)
	{
		MapMasterInfo mapInfo = Hub.s.dataman.ExcelDataManager.GetMapInfo(Hub.s.dataman.ExcelDataManager.Consts.C_MaintenanceRoomMapMasterId);
		if (mapInfo == null)
		{
			throw new Exception("Invalid Lobby Room Data");
		}
		if (_vrooms.Values.FirstOrDefault((IVroom x) => x is MaintenanceRoom) is MaintenanceRoom maintenanceRoom)
		{
			if (!maintenanceRoom.ExecuteLazyInitialization())
			{
				maintenanceRoom.ResetEnvironment();
				maintenanceRoom.ApplyBaseGameSessionInfo(_gameSessionInfo);
				if (_gameSessionInfo.GameSessionState == VGameSessionState.WaitStartSession)
				{
					maintenanceRoom.ResetBoostedItem();
				}
			}
			return;
		}
		AddVRoom(new MaintenanceRoomProperty(_gameSessionInfo.CurrentSessionID, hostToken, saveSlotID, new Vector2(-mapInfo.SizeX / 2, -mapInfo.SizeY / 2), new Vector2(mapInfo.SizeX / 2, mapInfo.SizeY / 2), mapInfo.SectorSize, _gameSessionInfo.GetCurrencyThreshold()), delegate(IVroom room)
		{
			room.Initialize();
			if (!_gameSessionInfo.SetGameSessionState(VGameSessionState.WaitStartSession, VGameSessionState.Ready))
			{
				throw new Exception("Invalid Game Session State");
			}
			if (saveSlotID != -1 && MonoSingleton<PlatformMgr>.Instance.IsSaveFileExist(MMSaveGameData.GetSaveFileName(saveSlotID)))
			{
				MMSaveGameData mMSaveGameData = MonoSingleton<PlatformMgr>.Instance.Load<MMSaveGameData>(MMSaveGameData.GetSaveFileName(saveSlotID));
				if (mMSaveGameData != null)
				{
					if (room is MaintenanceRoom maintenanceRoom2)
					{
						_gameSessionInfo.Reset(VGameSessionState.Invalid);
						_gameSessionInfo.ApplyLoadedGameData(mMSaveGameData);
						maintenanceRoom2.ApplyBaseGameSessionInfo(_gameSessionInfo);
						maintenanceRoom2.ApplyLoadedGameData(mMSaveGameData, _gameSessionInfo);
					}
					if (saveSlotID == 0)
					{
						Hub.s.pdata.SaveSlotID = mMSaveGameData.SlotID;
						Hub.s.vworld.SaveSlotID = mMSaveGameData.SlotID;
					}
				}
				else
				{
					Logger.RError("[LoadGameData] VRoomManager.InitMaintenenceRoom() FAIL! loadedGameData != null");
					_gameSessionInfo.Reset(VGameSessionState.Invalid);
					room.ApplyBaseGameSessionInfo(_gameSessionInfo);
					room.ResetBoostedItem();
				}
			}
			else
			{
				room.ApplyBaseGameSessionInfo(_gameSessionInfo);
				room.ResetBoostedItem();
				if (saveSlotID != -1 && room is MaintenanceRoom maintenanceRoom3)
				{
					List<string> playerNames = new List<string> { hostPlayerName };
					maintenanceRoom3.SaveGameData(saveSlotID, playerNames);
				}
			}
		}, lazeInit: true);
	}

	public void InitWaitingRoom()
	{
		MapMasterInfo mapInfo = Hub.s.dataman.ExcelDataManager.GetMapInfo(Hub.s.dataman.ExcelDataManager.Consts.C_WaitingRoomMapMasterId);
		if (mapInfo == null)
		{
			throw new Exception("Invalid Lobby Room Data");
		}
		AddVRoom(new WaitingRoomProperty(_gameSessionInfo.CurrentSessionID, new Vector2(-mapInfo.SizeX / 2, -mapInfo.SizeY / 2), new Vector2(mapInfo.SizeX / 2, mapInfo.SizeY / 2), mapInfo.SectorSize, _gameSessionInfo.GetCurrencyThreshold()), delegate(IVroom room)
		{
			if (!(room is VWaitingRoom vWaitingRoom))
			{
				throw new Exception("Invalid Room Type");
			}
			vWaitingRoom.ApplyBaseGameSessionInfo(_gameSessionInfo);
			vWaitingRoom.RollDiceDungeon();
			if (_gameSessionInfo.CurrentGameCount == 4)
			{
				vWaitingRoom.SetState(WaitingRoomState.DecisionNextGame);
			}
			Hub.s.vworld?.BroadcastToAll(new EnableWaitingRoomSig());
		});
	}

	public void InitDeathMatchRoom()
	{
		MapMasterInfo mapInfo = Hub.s.dataman.ExcelDataManager.GetMapInfo(Hub.s.dataman.ExcelDataManager.Consts.C_DeathmatchRoomMapMasterId);
		if (mapInfo == null)
		{
			throw new Exception("Invalid DeathMatch Room Data");
		}
		if (_vrooms.Values.FirstOrDefault((IVroom x) => x is DeathMatchRoom) is DeathMatchRoom deathMatchRoom)
		{
			deathMatchRoom.ResetEnvironment();
			deathMatchRoom.ApplyBaseGameSessionInfo(_gameSessionInfo);
			Hub.s.vworld?.ReadyToDeathMatchPktRecording();
			Hub.s.vworld?.BroadcastToAll(new EnableDeathMatchRoomSig());
		}
		else
		{
			AddVRoom(new DeathMatchRoomProperty(_gameSessionInfo.CurrentSessionID, new Vector2(-mapInfo.SizeX / 2, -mapInfo.SizeY / 2), new Vector2(mapInfo.SizeX / 2, mapInfo.SizeY / 2), mapInfo.SectorSize, _gameSessionInfo.GetCurrencyThreshold()), delegate(IVroom room)
			{
				room.ApplyBaseGameSessionInfo(_gameSessionInfo);
				Hub.s.vworld?.ReadyToDeathMatchPktRecording();
				Hub.s.vworld?.BroadcastToAll(new EnableDeathMatchRoomSig());
			});
		}
	}

	public void CreateGameRoom()
	{
		_commandExecutor.Invoke(delegate
		{
			if (_gameSessionInfo.GameSessionState != VGameSessionState.OnPlaying)
			{
				throw new Exception("Invalid Game Session State");
			}
			DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(_gameSessionInfo.NextDungeonMasterID);
			if (dungeonInfo == null)
			{
				throw new Exception($"Invalid dungeonInfo Room Data. NextDungeonMasterID: {_gameSessionInfo.NextDungeonMasterID}");
			}
			MapMasterInfo mapInfo = Hub.s.dataman.ExcelDataManager.GetMapInfo(dungeonInfo.MapID);
			if (mapInfo == null)
			{
				throw new Exception("Invalid Game Room Data");
			}
			AddVRoom(new DungeonProperty(_gameSessionInfo.CurrentSessionID, _gameSessionInfo.NextDungeonMasterID, _gameSessionInfo.RandomDungeonSeed, "hostToken", new Vector2(-mapInfo.SizeX / 2, -mapInfo.SizeY / 2), new Vector2(mapInfo.SizeX / 2, mapInfo.SizeY / 2), mapInfo.SectorSize, _gameSessionInfo.GetCurrencyThreshold(), _gameSessionInfo.CurrentGameCount), delegate(IVroom room)
			{
				room.ApplyBaseGameSessionInfo(_gameSessionInfo);
			});
		});
	}

	private void AddVRoom(IVRoomProperty property, OnCreateRoomDelegate? deleFunc = null, bool lazeInit = false)
	{
		_commandExecutor.Invoke(delegate
		{
			IVroom vroom = null;
			switch (property.vRoomType)
			{
			case VRoomType.Maintenance:
				vroom = new MaintenanceRoom(this, GenerateRoomID(), property);
				break;
			case VRoomType.Waiting:
				vroom = new VWaitingRoom(this, GenerateRoomID(), property);
				break;
			case VRoomType.Game:
				vroom = new DungeonRoom(this, GenerateRoomID(), property);
				break;
			case VRoomType.DeathMatch:
				vroom = new DeathMatchRoom(this, GenerateRoomID(), property);
				break;
			default:
				throw new Exception("Invalid VRoomType");
			}
			if (!lazeInit)
			{
				vroom.Initialize();
				if (deleFunc != null)
				{
					deleFunc(vroom);
				}
			}
			else if (deleFunc != null)
			{
				vroom.SetLazyInitDelegate(deleFunc);
			}
			_vrooms.Add(vroom.RoomID, vroom);
			if (vroom is DungeonRoom)
			{
				_eventTimer.CreateTimerEvent(delegate
				{
					Hub.s.vworld.BroadcastToAll(new CompleteMakingRoomSig
					{
						nextRoomInfo = new RoomInfo
						{
							roomUID = vroom.RoomID,
							roomType = VRoomType.Game
						}
					});
				}, 1000L);
			}
		});
	}

	public void OnUpdate(long delta)
	{
		_commandExecutor.Execute();
		_eventTimer.Update();
		foreach (IVroom value in _vrooms.Values)
		{
			value.OnUpdate(delta);
		}
	}

	public long GenerateRoomID()
	{
		return Interlocked.Increment(ref _roomIDGenerator);
	}

	public long GetNewItemID()
	{
		return Interlocked.Increment(ref _itemIDGenerator);
	}

	public long GetLobbySessionID()
	{
		return _gameSessionInfo.CurrentSessionID;
	}

	public long GetMaintenenceRoomUID()
	{
		if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is MaintenanceRoom) is MaintenanceRoom maintenanceRoom))
		{
			Logger.RError("Lobby not found");
			return 0L;
		}
		return maintenanceRoom.RoomID;
	}

	public (int cycleCount, int dayCount, bool repaired) GetMaintenenceRoomCycleInfos()
	{
		if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is MaintenanceRoom) is MaintenanceRoom maintenanceRoom))
		{
			Logger.RError("Lobby not found");
			return (cycleCount: 1, dayCount: 1, repaired: true);
		}
		return maintenanceRoom.GetMaintenenceRoomCycleInfos();
	}

	public List<int> GetMaintenenceRoomTramUpgradeList()
	{
		if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is MaintenanceRoom) is MaintenanceRoom maintenanceRoom))
		{
			Logger.RError("Lobby not found");
			return new List<int>();
		}
		return maintenanceRoom.TramUpgradeList.Clone();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposing || !_disposed.On())
		{
			return;
		}
		foreach (IVroom value in _vrooms.Values)
		{
			value.Dispose();
		}
		_vrooms.Clear();
		_eventTimer.Dispose();
		_commandExecutor.Dispose();
	}

	public void EnterMaintenenceRoom(SessionContext context, int hashCode)
	{
		_commandExecutor.Invoke(delegate
		{
			MsgErrorCode msgErrorCode = MsgErrorCode.Success;
			try
			{
				if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is MaintenanceRoom) is MaintenanceRoom maintenanceRoom))
				{
					Logger.RError("MaintenanceRoom not found");
					msgErrorCode = MsgErrorCode.RoomNotFound;
				}
				else if (maintenanceRoom.GetMemberCount() >= Hub.s.dataman.ExcelDataManager.Consts.C_MaxPlayerCount)
				{
					msgErrorCode = MsgErrorCode.PlayerCountExceeded;
				}
				else
				{
					context.SetEnterPacketHashCode(hashCode);
					MsgErrorCode msgErrorCode2 = maintenanceRoom.EnterRoom(context);
					if (msgErrorCode2 != MsgErrorCode.Success)
					{
						msgErrorCode = msgErrorCode2;
					}
				}
			}
			finally
			{
				if (msgErrorCode != MsgErrorCode.Success)
				{
					context.Send(new EnterMaintenanceRoomRes(hashCode)
					{
						errorCode = msgErrorCode
					});
					Hub.s.vworld?.Disconnect(context.GetSessionID());
				}
			}
		});
	}

	public void EnterWaitingRoom(SessionContext context, int hashCode)
	{
		_commandExecutor.Invoke(delegate
		{
			MsgErrorCode msgErrorCode = MsgErrorCode.Success;
			try
			{
				if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is VWaitingRoom) is VWaitingRoom vWaitingRoom))
				{
					Logger.RError("Lobby not found");
					msgErrorCode = MsgErrorCode.RoomNotFound;
				}
				else if (vWaitingRoom.GetMemberCount() >= Hub.s.dataman.ExcelDataManager.Consts.C_MaxPlayerCount)
				{
					msgErrorCode = MsgErrorCode.PlayerCountExceeded;
				}
				else
				{
					context.SetEnterPacketHashCode(hashCode);
					MsgErrorCode msgErrorCode2 = vWaitingRoom.EnterRoom(context);
					if (msgErrorCode2 != MsgErrorCode.Success)
					{
						msgErrorCode = msgErrorCode2;
					}
				}
			}
			finally
			{
				if (msgErrorCode != MsgErrorCode.Success)
				{
					context.Send(new EnterWaitingRoomRes(hashCode)
					{
						errorCode = msgErrorCode
					});
				}
			}
		});
	}

	public void EnterDungeon(SessionContext context, int hashCode, long roomUID)
	{
		MsgErrorCode msgErrorCode = MsgErrorCode.Success;
		try
		{
			if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is DungeonRoom && x.RoomID == roomUID) is DungeonRoom dungeonRoom))
			{
				Logger.RError(string.Format("GameRoom not found. requested roomUID: {0}. existing rooms: {1}", roomUID, string.Join(", ", from x in _vrooms
					where x.Value is DungeonRoom
					select x.Value.RoomID)));
				msgErrorCode = MsgErrorCode.RoomNotFound;
			}
			else
			{
				context.SetEnterPacketHashCode(hashCode);
				msgErrorCode = dungeonRoom.EnterRoom(context);
			}
		}
		finally
		{
			if (msgErrorCode != MsgErrorCode.Success)
			{
				context.Send(new EnterDungeonRes(hashCode)
				{
					errorCode = msgErrorCode
				});
			}
		}
	}

	public void EnterDeathMatchRoom(SessionContext context, int hashCode)
	{
		MsgErrorCode msgErrorCode = MsgErrorCode.Success;
		try
		{
			if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is DeathMatchRoom) is DeathMatchRoom deathMatchRoom))
			{
				Logger.RError("deathMatchRoom not found. ");
				msgErrorCode = MsgErrorCode.RoomNotFound;
			}
			else
			{
				context.SetEnterPacketHashCode(hashCode);
				msgErrorCode = deathMatchRoom.EnterRoom(context);
			}
		}
		finally
		{
			if (msgErrorCode != MsgErrorCode.Success)
			{
				context.Send(new EnterDeathMatchRoomRes(hashCode)
				{
					errorCode = msgErrorCode
				});
			}
		}
	}

	public void Invoke(Command command)
	{
		_commandExecutor.Invoke(command);
	}

	public void PendStartGame(Dictionary<ulong, long> playerUIDs, int nextDungeonMasterID, int randomDungeonSeed, RoomDrainInfo drainInfo)
	{
		if (_gameSessionInfo.CurrentGameCount > 3)
		{
			Logger.RWarn($"PendStartGame failed, game count is less than max. gameCount: {_gameSessionInfo.CurrentGameCount}, maxGameCount: {3}");
			return;
		}
		if (_gameSessionInfo.GameSessionState != VGameSessionState.PreGame)
		{
			Logger.RWarn($"PendStartGame failed, state is not WaitGame. state: {_gameSessionInfo.GameSessionState}");
			return;
		}
		_commandExecutor.Invoke(delegate
		{
			_gameSessionInfo.SetNextDungeonMasterID(nextDungeonMasterID, randomDungeonSeed);
			_gameSessionInfo.ApplyDrainInfo(drainInfo);
			_gameSessionInfo.SetPlayerUIDs(playerUIDs);
			_gameSessionInfo.SetGameSessionState(VGameSessionState.OnPlaying, VGameSessionState.PreGame);
			RequestWipeoutWaitingRoom(backup: true, reset: true);
			_eventTimer.CreateTimerEvent(delegate
			{
				foreach (long item in (from x in _vrooms
					where x.Value is VWaitingRoom
					select x.Key).ToList())
				{
					if (_vrooms.TryGetValue(item, out IVroom value))
					{
						value.Dispose();
						_vrooms.Remove(item);
					}
					else
					{
						Logger.RError($"Room not found. roomID: {item}");
					}
				}
			}, 1000L);
		});
	}

	public void PendStartDeathMatch(Dictionary<ulong, long> playerUIDs, RoomDrainInfo drainInfo)
	{
		_commandExecutor.Invoke(delegate
		{
			_gameSessionInfo.ApplyDrainInfo(drainInfo);
			_gameSessionInfo.SetPlayerUIDs(playerUIDs);
			_gameSessionInfo.SetGameSessionState(VGameSessionState.DeathMatch, VGameSessionState.EndGame);
			_eventTimer.CreateTimerEvent(delegate
			{
				RequestWipeoutMaintenanceRoom();
			}, 5000L);
		});
	}

	public bool PendEndGame(Dictionary<ulong, long> playerUIDs, RoomDrainInfo drainInfo)
	{
		if (_gameSessionInfo.CurrentGameCount < 3)
		{
			Logger.RWarn($"PendEndGame failed, game count is less than max. gameCount: {_gameSessionInfo.CurrentGameCount}, maxGameCount: {3}");
			return false;
		}
		if (_gameSessionInfo.GameSessionState != VGameSessionState.AfterGame)
		{
			Logger.RWarn($"PendEndGame failed, state is not OnPlaying. state: {_gameSessionInfo.GameSessionState}");
			return false;
		}
		_commandExecutor.Invoke(delegate
		{
			_gameSessionInfo.ApplyDrainInfo(drainInfo);
			_gameSessionInfo.SetPlayerUIDs(playerUIDs);
			_gameSessionInfo.SetGameSessionState(VGameSessionState.EndGame, VGameSessionState.AfterGame);
			if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is MaintenanceRoom) is MaintenanceRoom maintenanceRoom))
			{
				Logger.RError("MaintenanceRoom not found");
			}
			else
			{
				maintenanceRoom.InitShopItems();
				RequestWipeoutWaitingRoom(backup: true, reset: true);
				_eventTimer.CreateTimerEvent(delegate
				{
					foreach (long item in (from x in _vrooms
						where x.Value is VWaitingRoom
						select x.Key).ToList())
					{
						if (_vrooms.TryGetValue(item, out IVroom value))
						{
							value.Dispose();
							_vrooms.Remove(item);
						}
						else
						{
							Logger.RError($"Room not found. roomID: {item}");
						}
					}
				}, 1000L);
				maintenanceRoom.OnCompleteGame(_gameSessionInfo);
			}
		});
		return true;
	}

	public void PendStartSession(Dictionary<ulong, long> playerUIDs, RoomDrainInfo drainInfo)
	{
		if (_gameSessionInfo.CurrentGameCount > 3)
		{
			Logger.RWarn($"PendStartSession failed, game count is less than max. gameCount: {_gameSessionInfo.CurrentGameCount}, maxGameCount: {3}");
			return;
		}
		_commandExecutor.Invoke(delegate
		{
			_gameSessionInfo.ApplyDrainInfo(drainInfo);
			_gameSessionInfo.SetPlayerUIDs(playerUIDs);
			_gameSessionInfo.SetGameSessionState(VGameSessionState.PreGame, VGameSessionState.WaitStartSession);
			RequestWipeoutMaintenanceRoom();
		});
	}

	public void OnFinishGame(RoomDrainInfo drainInfo, bool prevDungeonSuccess)
	{
		_commandExecutor.Invoke(delegate
		{
			_gameSessionInfo.ApplyDrainInfo(drainInfo);
			_gameSessionInfo.ApplyPrevDungeonState(prevDungeonSuccess);
			if (_gameSessionInfo.CurrentGameCount > 3)
			{
				_gameSessionInfo.SetGameSessionState(VGameSessionState.AfterGame, VGameSessionState.OnPlaying);
			}
			else
			{
				_gameSessionInfo.SetGameSessionState(VGameSessionState.PreGame, VGameSessionState.OnPlaying);
			}
			RequestStopDungeonRoom(drainInfo.RoomID);
		});
	}

	public void TerminateSession(Dictionary<ulong, long> playerUIDs, RoomDrainInfo drainInfo)
	{
		_commandExecutor.Invoke(delegate
		{
			_gameSessionInfo.Reset(_gameSessionInfo.GameSessionState);
			_gameSessionInfo.ApplyDrainInfo(drainInfo);
			Hub.s.vworld.BroadcastToAll(new CompleteGameSessionSig
			{
				success = false,
				nextTargetCurrency = 0
			});
			_eventTimer.CreateTimerEvent(delegate
			{
				ApplyNewSessionInfo(promoted: false);
				_eventTimer.CreateTimerEvent(delegate
				{
					RequestWipeoutDeathMatchRoom();
				}, 2000L);
			}, 5000L);
		});
	}

	public void PromoteSession(Dictionary<ulong, long> playerUIDs, RoomDrainInfo drainInfo)
	{
		_commandExecutor.Invoke(delegate
		{
			_gameSessionInfo.ApplyDrainInfo(drainInfo);
			_gameSessionInfo.SetPlayerUIDs(playerUIDs);
			int nextTargetCurrency = _gameSessionInfo.Promote();
			Hub.s.vworld.BroadcastToAll(new CompleteGameSessionSig
			{
				success = true,
				nextTargetCurrency = nextTargetCurrency
			});
			_eventTimer.CreateTimerEvent(delegate
			{
				RequestWipeoutMaintenanceRoom();
			}, 5000L);
			_eventTimer.CreateTimerEvent(delegate
			{
				ApplyNewSessionInfo(promoted: true);
			}, 7000L);
		});
	}

	public void ApplyNewSessionInfo(bool promoted)
	{
		_commandExecutor.Invoke(delegate
		{
			foreach (long item in (from x in _vrooms
				where x.Value is DungeonRoom
				select x.Key).ToList())
			{
				if (_vrooms.TryGetValue(item, out IVroom value))
				{
					value.Dispose();
					_vrooms.Remove(item);
				}
				else
				{
					Logger.RError($"Room not found. roomID: {item}");
				}
			}
			if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is MaintenanceRoom) is MaintenanceRoom maintenanceRoom))
			{
				Logger.RError("MaintenanceRoom not found");
			}
			else
			{
				maintenanceRoom.OnCompleteSession(_gameSessionInfo, promoted);
			}
		});
	}

	private void RequestStopDungeonRoom(long roomID)
	{
		_commandExecutor.Invoke(delegate
		{
			_eventTimer.CreateTimerEvent(delegate
			{
				if (!_vrooms.TryGetValue(roomID, out IVroom value))
				{
					Logger.RError($"Room not found. roomID: {roomID}");
				}
				else if (!(value is DungeonRoom dungeonRoom))
				{
					Logger.RError($"Invalid Room Type. roomID: {roomID}");
				}
				else
				{
					dungeonRoom.Shutdown();
				}
			}, 3000L);
		});
	}

	private void RequestWipeoutWaitingRoom(bool backup, bool reset)
	{
		if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is VWaitingRoom) is VWaitingRoom vWaitingRoom))
		{
			Logger.RError("WaitingRoom not found");
		}
		else
		{
			vWaitingRoom.Wipeout(backup, reset);
		}
	}

	private void RequestWipeoutMaintenanceRoom()
	{
		if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is MaintenanceRoom) is MaintenanceRoom maintenanceRoom))
		{
			Logger.RError("WaitingRoom not found");
		}
		else
		{
			maintenanceRoom.Wipeout();
		}
	}

	private void RequestWipeoutDeathMatchRoom()
	{
		_commandExecutor.Invoke(delegate
		{
			if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is DeathMatchRoom) is DeathMatchRoom deathMatchRoom))
			{
				Logger.RError("DeathMatchRoom not found");
			}
			else
			{
				deathMatchRoom.Wipeout();
			}
		});
	}

	public void RemoveDeathMatchRoom()
	{
		_commandExecutor.Invoke(delegate
		{
			if (!(_vrooms.Values.FirstOrDefault((IVroom x) => x is DeathMatchRoom) is DeathMatchRoom deathMatchRoom))
			{
				Logger.RError("DeathMatchRoom not found");
			}
			else
			{
				deathMatchRoom.Dispose();
				_vrooms.Remove(deathMatchRoom.RoomID);
			}
		});
	}

	public MsgErrorCode OnRegistPlayer(ulong steamID, bool isHost)
	{
		if (!_gameSessionInfo.CanEnterSession())
		{
			return MsgErrorCode.GameAlreadyStarted;
		}
		if (!_gameSessionInfo.AddPlayerSteamID(steamID, isHost))
		{
			return MsgErrorCode.PlayerCountExceeded;
		}
		return MsgErrorCode.Success;
	}

	public void OnUnregistPlayer(ulong steamID)
	{
		_gameSessionInfo.RemoveSteamID(steamID);
	}

	public int GetInPlayMemberCount(VRoomType roomType)
	{
		return Hub.s.vworld.GetSessionCount(roomType);
	}

	public int GetRoomMemberCount(VRoomType roomType)
	{
		IVroom vroom = _vrooms.Values.Where((IVroom x) => x.Property.vRoomType == roomType).FirstOrDefault();
		if (vroom == null)
		{
			Logger.RError($"Room not found. roomType: {roomType}");
			return 0;
		}
		return vroom.GetMemberCount();
	}

	public void SetCurrentCycleForDebug(int cycle)
	{
		_gameSessionInfo.SetCurrentCycleForDebug(cycle);
	}
}
