using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluServerBase.Threading;

public class SessionManager
{
	private int _sessionIDGenerator;

	private readonly Dictionary<long, SessionContext> m_Contexts = new Dictionary<long, SessionContext>();

	private CommandExecutor _commandExecutor;

	private SessionContext? _hostSessionContext;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	private Dictionary<string, (long timestamp, NetworkGrade grade)> _networkGrades = new Dictionary<string, (long, NetworkGrade)>();

	private long _lastSyncNetworkGradeTick;

	private bool _disableHeartBeatCheck;

	public SessionManager()
	{
		_commandExecutor = CommandExecutor.CreateCommandExecutor("SessionManager", 0L);
	}

	public void Initialize()
	{
	}

	public int GetNewSessionID()
	{
		return Interlocked.Increment(ref _sessionIDGenerator);
	}

	public void Add(SessionContext context)
	{
		_commandExecutor.Invoke(delegate
		{
			if (!m_Contexts.TryAdd(context.GetSessionID(), context))
			{
				Logger.RError($"duplicated session id, failed to add session, sessionId: {context.GetSessionID()}");
			}
		});
	}

	public void Remove(long sessionID, DisconnectReason reason)
	{
		_commandExecutor.Invoke(delegate
		{
			if (!m_Contexts.Remove(sessionID, out SessionContext value))
			{
				Logger.RError($"session does not exist, failed to remove session, sessionId: {sessionID}, reason: {reason}");
			}
			else
			{
				if (value.NickName != "no name reluman")
				{
					BroadcastToAll(new LeaveServerSig
					{
						steamID = value.SteamID,
						nickName = value.NickName
					});
				}
				_networkGrades.Remove(value.NickName);
				value.Dispose();
				value.BeginClose(reason);
			}
		});
	}

	public void KickAll()
	{
		foreach (KeyValuePair<long, SessionContext> context in m_Contexts)
		{
			Remove(context.Key, DisconnectReason.KickByServer);
		}
	}

	public void OnUpdate()
	{
		_commandExecutor.Execute();
		foreach (KeyValuePair<long, SessionContext> context in m_Contexts)
		{
			context.Value.Update();
		}
		long currentTick = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (currentTick - _lastSyncNetworkGradeTick > 5000 && _networkGrades.Count() > 0)
		{
			_lastSyncNetworkGradeTick = currentTick;
			BroadcastToAll(new NetworkGradeSig
			{
				grades = _networkGrades.ToDictionary<KeyValuePair<string, (long, NetworkGrade)>, string, NetworkGrade>((KeyValuePair<string, (long timestamp, NetworkGrade grade)> x) => x.Key, (KeyValuePair<string, (long timestamp, NetworkGrade grade)> x) => x.Value.grade)
			});
		}
		if (_disableHeartBeatCheck)
		{
			return;
		}
		foreach (string expiredUserName in (from x in _networkGrades
			where currentTick - x.Value.timestamp > 60000
			select x.Key).ToList())
		{
			long key = m_Contexts.FirstOrDefault<KeyValuePair<long, SessionContext>>((KeyValuePair<long, SessionContext> x) => x.Value.NickName == expiredUserName).Key;
			if (key != 0L)
			{
				Remove(key, DisconnectReason.ByServer);
			}
			else
			{
				Logger.RError("Failed to remove session for expired network grade user: " + expiredUserName);
			}
		}
	}

	public void OnPostUpdate()
	{
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
		_commandExecutor.Dispose();
		foreach (KeyValuePair<long, SessionContext> context in m_Contexts)
		{
			context.Value.Dispose();
		}
		m_Contexts.Clear();
	}

	public ISession? GetSession(long sessionID)
	{
		if (!m_Contexts.TryGetValue(sessionID, out SessionContext value))
		{
			return null;
		}
		return value.Session;
	}

	public bool SetHost(SessionContext context)
	{
		if (_hostSessionContext != null)
		{
			return false;
		}
		_hostSessionContext = context;
		Add(context);
		return true;
	}

	public bool RemoveHost()
	{
		if (_hostSessionContext == null)
		{
			return false;
		}
		Remove(_hostSessionContext.GetSessionID(), DisconnectReason.ByServer);
		_hostSessionContext = null;
		return true;
	}

	public bool ExistHost()
	{
		return _hostSessionContext != null;
	}

	public void BroadcastToAll(IMsg msg)
	{
		_commandExecutor.Invoke(delegate
		{
			foreach (KeyValuePair<long, SessionContext> context in m_Contexts)
			{
				context.Value.Send(msg);
			}
		});
	}

	public void SetNetworkGrade(SessionContext context, NetworkGrade grade)
	{
		if (context.NickName == "no name reluman")
		{
			Logger.RWarn("[SetNetworkGrade] HeartBeatReq is too fast. SessionContext not login yet.");
		}
		else
		{
			_networkGrades[context.NickName] = (Hub.s.timeutil.GetCurrentTickMilliSec(), grade);
		}
	}

	public void SetDisableHeartBeatCheck(bool disable)
	{
		_disableHeartBeatCheck = disable;
	}

	public int GetSessionCount(VRoomType roomType)
	{
		if (roomType == VRoomType.Maintenance)
		{
			return m_Contexts.Count;
		}
		return m_Contexts.Count - Hub.s.vworld.GetRoomMemberCount(VRoomType.Maintenance);
	}
}
