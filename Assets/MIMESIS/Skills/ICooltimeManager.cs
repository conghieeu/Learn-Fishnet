using System.Collections.Generic;
using ReluProtocol.Enum;

public abstract class ICooltimeManager
{
	protected delegate void CooltimeChangedHandler(ICooltimeElement elem, CooltimeChangeType type);

	protected CooltimeController _parent;

	protected Dictionary<long, ICooltimeElement> _cooltimeDict = new Dictionary<long, ICooltimeElement>();

	protected HashSet<long> _trashBucket = new HashSet<long>();

	protected HashSet<long> _newSyncIDs = new HashSet<long>();

	public ICooltimeElement? GlobalCooltime { get; protected set; }

	protected event CooltimeChangedHandler? CooltimeChanged;

	public ICooltimeManager(CooltimeController parent)
	{
		_parent = parent;
	}

	public void Update(long delta)
	{
		_trashBucket.Clear();
		foreach (KeyValuePair<long, ICooltimeElement> item in _cooltimeDict)
		{
			item.Value.Update(delta);
			if (item.Value.DeferToDelete)
			{
				_trashBucket.Add(item.Key);
			}
		}
	}

	protected bool AddCooltime(ICooltimeElement elem)
	{
		if (_cooltimeDict.ContainsKey(elem.SyncID))
		{
			return false;
		}
		_cooltimeDict.Add(elem.SyncID, elem);
		_newSyncIDs.Add(elem.SyncID);
		return true;
	}

	protected virtual void EmptyTrashBucket()
	{
		foreach (long item in _trashBucket)
		{
			if (GlobalCooltime != null && GlobalCooltime.SyncID == item)
			{
				GlobalCooltime = null;
			}
			_cooltimeDict.Remove(item);
		}
	}

	public void OnSyncComplete()
	{
		_newSyncIDs.Clear();
		EmptyTrashBucket();
	}

	public bool Changed()
	{
		if (_newSyncIDs.Count <= 0)
		{
			return _trashBucket.Count > 0;
		}
		return true;
	}

	public void CollectChangedInfo(ref CooltimeSig sig)
	{
		foreach (long newSyncID in _newSyncIDs)
		{
			ICooltimeElement cooltimeElement = _cooltimeDict[newSyncID];
			this.CooltimeChanged?.Invoke(cooltimeElement, CooltimeChangeType.Add);
			if (cooltimeElement.Sync)
			{
				cooltimeElement.FillCooltimeSig(ref sig, CooltimeChangeType.Add);
			}
		}
		foreach (long item in _trashBucket)
		{
			ICooltimeElement cooltimeElement2 = _cooltimeDict[item];
			this.CooltimeChanged?.Invoke(cooltimeElement2, CooltimeChangeType.Remove);
			if (cooltimeElement2.Sync)
			{
				cooltimeElement2.FillCooltimeSig(ref sig, CooltimeChangeType.Remove);
			}
		}
	}

	public bool IsGlobalCooltime()
	{
		return GlobalCooltime != null;
	}

	public virtual void Clear()
	{
		GlobalCooltime = null;
		_cooltimeDict.Clear();
		_trashBucket.Clear();
		_newSyncIDs.Clear();
	}

	public abstract bool AddCooltime(long syncID, long duration, int masterID, bool global, bool sync);

	public abstract bool IsCooltime(int id);

	public abstract void RemoveCooltime(long syncID);
}
