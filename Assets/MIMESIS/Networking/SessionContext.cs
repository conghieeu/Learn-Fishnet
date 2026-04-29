using System;
using System.Runtime.CompilerServices;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluServerBase.Threading;

public class SessionContext : IContext, IDisposable
{
	private VPlayer? _vPlayer;

	private AtomicFlag _isDisposed = new AtomicFlag(value: false);

	private long _heartBeatLastTick;

	private CommandExecutor _commandExecutor;

	private PlayerInfoSnapshot? _playerSnapshot;

	private EventTimer _eventTimer = new EventTimer();

	private OnPreSessionCtxSendEventHandler? _OnPreSessionCtxSendEvent;

	private int _enterPktHashCode;

	private bool isRunPktRecording;

	public ISession Session { get; protected set; }

	public int ServerID { get; }

	public PlayerInfoSnapshot? PlayerInfoSnapshot => _playerSnapshot;

	public string GUID => _playerSnapshot?.Guid ?? string.Empty;

	public ulong SteamID { get; private set; }

	public string NickName
	{
		get
		{
			object obj;
			if (_vPlayer == null)
			{
				obj = _playerSnapshot?.Name;
				if (obj == null)
				{
					return "no name reluman";
				}
			}
			else
			{
				obj = _vPlayer.ActorName;
			}
			return (string)obj;
		}
	}

	public int EnterPktHashCode => _enterPktHashCode;

	public bool MyGameEntered { get; private set; }

	public bool OtherGameEntered { get; private set; }

	public SessionContext(ISession session)
	{
		Session = session;
		_commandExecutor = CommandExecutor.CreateCommandExecutor("SessionContext", session.ID);
		_heartBeatLastTick = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (session is VirtualAcceptSession)
		{
			_eventTimer.CreateTimerEvent(SetupDefaultHost, 3L);
		}
		session.SetContext(this);
	}

	public void SetupDefaultHost()
	{
	}

	~SessionContext()
	{
		Dispose(disposing: false);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && _isDisposed.On())
		{
			Hub.s?.vworld.OnUnregistPlayer(SteamID);
			if (_vPlayer != null)
			{
				_vPlayer.VRoom.PendRemovePlayer(_vPlayer.ObjectID, backup: false, kill: false);
				_vPlayer = null;
			}
			BeginClose(DisconnectReason.ByServer);
			_commandExecutor.Dispose();
			_eventTimer.Dispose();
		}
	}

	public int GetSessionID()
	{
		return Session.ID;
	}

	public void BeginClose(DisconnectReason reason)
	{
		Session.BeginClose(reason);
	}

	public SendResult Send(IMsg msg)
	{
		_OnPreSessionCtxSendEvent?.Invoke(msg, this);
		return Session.SendLink(msg);
	}

	public void Update()
	{
		_commandExecutor.Execute();
		_eventTimer.Update();
		if (Session is VirtualAcceptSession virtualAcceptSession)
		{
			virtualAcceptSession.OnUpdate();
		}
	}

	public SendResult SendError<TResponse>(MsgErrorCode errorCode, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) where TResponse : IResMsg, new()
	{
		return Session.SendError<TResponse>(errorCode, file, line);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	public void HandleHeartBeat()
	{
		_heartBeatLastTick = Hub.s.timeutil.GetCurrentTickMilliSec();
	}

	public void PostHandler<T>(OnClientSessionDispatchEventHandler<T> handler, T msg) where T : IMsg, new()
	{
		_commandExecutor.Invoke(delegate
		{
			handler(this, msg);
		});
	}

	public void PostAsyncHandler<T>(OnClientSessionDispatchAsyncEventHandler<T> handler, T msg) where T : IMsg, new()
	{
		_commandExecutor.Invoke(async delegate
		{
			await handler(this, msg);
		});
	}

	public void PostPlayerHandler<T>(OnPlayerDispatchEventHandler<T> handler, T msg) where T : IMsg, new()
	{
		if (_vPlayer != null)
		{
			_vPlayer.PostHandler(handler, msg);
		}
	}

	public void PostPlayerAsyncHandler<T>(OnPlayerDispatchAsyncEventHandler<T> handler, T msg) where T : IMsg, new()
	{
		if (_vPlayer != null)
		{
			_vPlayer.PostAsyncHandler(handler, msg);
		}
	}

	public bool ExistPlayer()
	{
		return _vPlayer != null;
	}

	public void CreatePlayerSnapshot(bool backupFlag)
	{
		if (_vPlayer == null)
		{
			Logger.RWarn("SessionContext's player is null");
			return;
		}
		_playerSnapshot = global::PlayerInfoSnapshot.Generate(_vPlayer, backupFlag);
		_vPlayer = null;
	}

	public long GetPlayerUID()
	{
		return _playerSnapshot?.UID ?? 0;
	}

	public VPlayer? CreatePlayer(int objectID, IVroom room, PosWithRot pos, bool isIndoor)
	{
		if (_vPlayer != null)
		{
			Logger.RError("SessionContext's player is not null");
			return null;
		}
		if (_playerSnapshot == null)
		{
			Logger.RError("SessionContext's player snapshot is null");
			return null;
		}
		return _vPlayer = new VPlayer(this, objectID, _playerSnapshot.MasterID, Session is VirtualAcceptSession, _playerSnapshot.Name, _playerSnapshot.VoiceUID, pos, isIndoor, room, ReasonOfSpawn.Spawn);
	}

	public void Login(long playerUID, string guid, ulong steamID, string nickName, string voiceUID, bool isHost, int hashCode)
	{
		if (_playerSnapshot != null)
		{
			Send(new JoinServerRes(hashCode)
			{
				errorCode = MsgErrorCode.AlreadyLoggedIn
			});
			return;
		}
		_playerSnapshot = global::PlayerInfoSnapshot.Generate(playerUID, isHost, 1, nickName, voiceUID, steamID, guid, VRoomType.Maintenance);
		SteamID = steamID;
		MsgErrorCode msgErrorCode = Hub.s.vworld.RegistPlayer(steamID, isHost);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			Send(new JoinServerRes(hashCode)
			{
				errorCode = msgErrorCode
			});
			return;
		}
		Send(new JoinServerRes(hashCode)
		{
			playerUID = _playerSnapshot.UID,
			isHost = _playerSnapshot.IsHost,
			roomInfo = new MaintenenceRoomInfo
			{
				ID = Hub.s.vworld.GetMaintenenceRoomID(),
				CycleCountForClient = (isHost ? 1 : Hub.s.vworld.GetMaintenenceRoomCycleInfos().cycleCount),
				dayCountForClient = (isHost ? 1 : Hub.s.vworld.GetMaintenenceRoomCycleInfos().dayCount),
				repairedForClient = (isHost || Hub.s.vworld.GetMaintenenceRoomCycleInfos().repaired),
				tramUpgradeListForClient = Hub.s.vworld.GetMaintenenceRoomTramUpgradeList()
			}
		});
		Hub.s.vworld.BroadcastToAll(new JoinServerSig
		{
			playerUID = _playerSnapshot.UID,
			steamID = steamID,
			nickName = nickName,
			voiceUID = voiceUID
		});
	}

	public void SetPreSessionCtxSendEvent(OnPreSessionCtxSendEventHandler handler)
	{
		_OnPreSessionCtxSendEvent = (OnPreSessionCtxSendEventHandler)Delegate.Combine(_OnPreSessionCtxSendEvent, handler);
	}

	public void SetEnterPacketHashCode(int hashCode)
	{
		_enterPktHashCode = hashCode;
	}

	public void ApplyMyGameEntered()
	{
		MyGameEntered = true;
	}

	public void ApplyOtherGameEntered()
	{
		OtherGameEntered = true;
	}

	public void ResetSnapShotRoomType()
	{
		_playerSnapshot?.ResetRoomType();
	}

	public VRoomType GetVRoomType()
	{
		if (_vPlayer == null && _playerSnapshot == null)
		{
			Logger.RError("SessionContext's player and player snapshot are all null");
			return VRoomType.Invalid;
		}
		if (_vPlayer == null)
		{
			return _playerSnapshot?.RoomType ?? VRoomType.Invalid;
		}
		return _vPlayer.VRoom.Property.vRoomType;
	}
}
