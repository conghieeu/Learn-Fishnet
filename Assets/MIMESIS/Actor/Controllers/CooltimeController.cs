using System;
using System.Collections.Generic;
using ReluProtocol.Enum;

public class CooltimeController : IVActorController, IDisposable
{
	private VActor _self;

	private long _syncIDGenerator;

	private Dictionary<CooltimeType, ICooltimeManager> _cooltimeManagerDict = new Dictionary<CooltimeType, ICooltimeManager>();

	public VActorControllerType type { get; } = VActorControllerType.Cooltime;

	public CooltimeController(VActor self)
	{
		_self = self;
		_cooltimeManagerDict.Add(CooltimeType.Skill, new SkillCooltimeManager(this));
	}

	public void Initialize()
	{
		_syncIDGenerator = 1L;
		foreach (ICooltimeManager value in _cooltimeManagerDict.Values)
		{
			value.Clear();
		}
	}

	public void Update(long deltaTime)
	{
		bool flag = false;
		foreach (ICooltimeManager value in _cooltimeManagerDict.Values)
		{
			value.Update(deltaTime);
			if (value.Changed())
			{
				flag = true;
			}
		}
		if (flag)
		{
			SyncChangedInfo();
		}
	}

	private void SyncChangedInfo()
	{
		bool flag = false;
		CooltimeSig sig = new CooltimeSig
		{
			actorID = _self.ObjectID
		};
		foreach (ICooltimeManager value in _cooltimeManagerDict.Values)
		{
			if (value.Changed())
			{
				flag = true;
				value.CollectChangedInfo(ref sig);
				value.OnSyncComplete();
			}
		}
		if (flag)
		{
			_self.SendToMe(sig);
		}
	}

	public long GetNewSyncID()
	{
		return _syncIDGenerator++;
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		return MsgErrorCode.Success;
	}

	public bool IsCooltime(CooltimeType type, int masterID)
	{
		if (!_cooltimeManagerDict.TryGetValue(type, out var value))
		{
			return false;
		}
		if (value.IsGlobalCooltime())
		{
			return true;
		}
		return value.IsCooltime(masterID);
	}

	public void AddCooltime(CooltimeType type, int masterID, long globalCooltime, long idCooltime, bool sync = true)
	{
		if (_cooltimeManagerDict.TryGetValue(type, out var value))
		{
			if (globalCooltime > 0)
			{
				value.AddCooltime(GetNewSyncID(), globalCooltime, masterID, global: true, sync);
			}
			if (idCooltime > 0)
			{
				value.AddCooltime(GetNewSyncID(), idCooltime, masterID, global: false, sync);
			}
		}
	}

	public T? GetCooltimeManager<T>(CooltimeType type) where T : ICooltimeManager
	{
		if (!_cooltimeManagerDict.TryGetValue(type, out var value))
		{
			return null;
		}
		return value as T;
	}

	public void Dispose()
	{
	}

	public void WaitInitDone()
	{
	}

	public void PostUpdate(long deltaTime)
	{
	}

	public string GetDebugString()
	{
		return string.Empty;
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}
}
