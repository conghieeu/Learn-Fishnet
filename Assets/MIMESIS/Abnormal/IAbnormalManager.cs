using System.Collections.Generic;
using ReluProtocol.Enum;

public abstract class IAbnormalManager
{
	protected AbnormalController _abnormalController;

	protected Dictionary<long, IAbnormalElement> _abnormals = new Dictionary<long, IAbnormalElement>();

	protected HashSet<long> NewAbnormalSyncIDs = new HashSet<long>();

	protected HashSet<long> _trashBuckets = new HashSet<long>();

	protected HashSet<long> _changedAbnormalSyncIDs = new HashSet<long>();

	public IAbnormalManager(AbnormalController abnormalController)
	{
		_abnormalController = abnormalController;
	}

	protected AbnormalHandleResult AddAbnormal(IAbnormalElement abnormal)
	{
		if (_abnormals.ContainsKey(abnormal.FixedValue.SyncID))
		{
			return AbnormalHandleResult.Failed;
		}
		_abnormals.Add(abnormal.FixedValue.SyncID, abnormal);
		NewAbnormalSyncIDs.Add(abnormal.FixedValue.SyncID);
		return AbnormalHandleResult.Success;
	}

	public bool GetAbnormalElement(long syncID, out IAbnormalElement? abnormalElement)
	{
		if (_abnormals.TryGetValue(syncID, out abnormalElement))
		{
			return true;
		}
		return false;
	}

	public bool ExistAbnormalElement(long syncID)
	{
		return _abnormals.ContainsKey(syncID);
	}

	public virtual void Update(long delta)
	{
		_trashBuckets.Clear();
		_changedAbnormalSyncIDs.Clear();
		if (_abnormals.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<long, IAbnormalElement> abnormal in _abnormals)
		{
			IAbnormalElement value = abnormal.Value;
			if (value.DeferToDelete)
			{
				_trashBuckets.Add(value.FixedValue.SyncID);
				continue;
			}
			value.Update(delta);
			if (value.Expired())
			{
				_trashBuckets.Add(value.FixedValue.SyncID);
			}
			if (value.Changed)
			{
				value.ClearChanged();
				_changedAbnormalSyncIDs.Add(value.FixedValue.SyncID);
			}
		}
		if (Changed())
		{
			OnCompleteUpdate();
		}
	}

	public bool Changed()
	{
		if (NewAbnormalSyncIDs.Count <= 0 && _trashBuckets.Count <= 0)
		{
			return _changedAbnormalSyncIDs.Count > 0;
		}
		return true;
	}

	public void OnCompleteUpdate()
	{
		DisposeOnList();
		SyncElementDispose();
		RefineInfo();
	}

	private void DisposeObject(long syncID)
	{
		if (_abnormals.TryGetValue(syncID, out var value))
		{
			value.OnElementDispose?.Invoke();
			_abnormalController.DisposeElement(value.FixedValue.AbnormalObjectID, value.FixedValue.SyncID);
		}
	}

	private void SyncElementDispose()
	{
		foreach (long trashBucket in _trashBuckets)
		{
			DisposeObject(trashBucket);
		}
	}

	private void EmptyTrashBucket()
	{
		foreach (long trashBucket in _trashBuckets)
		{
			_abnormals.Remove(trashBucket);
		}
		_trashBuckets.Clear();
	}

	public void OnSyncComplete()
	{
		foreach (long trashBucket in _trashBuckets)
		{
			if (GetAbnormalElement(trashBucket, out IAbnormalElement abnormalElement))
			{
				abnormalElement.Dispose();
			}
		}
		EmptyTrashBucket();
		NewAbnormalSyncIDs.Clear();
		_changedAbnormalSyncIDs.Clear();
	}

	protected void DispelAll()
	{
		foreach (IAbnormalElement value in _abnormals.Values)
		{
			if (value.FixedValue.Dispelable)
			{
				value.SetDelete();
			}
		}
	}

	public bool DispelAbnormal(long syncID, bool force = false)
	{
		if (!_abnormals.TryGetValue(syncID, out var value))
		{
			return false;
		}
		if (value.FixedValue.Dispelable || force)
		{
			value.SetDelete();
		}
		return true;
	}

	public virtual void Clear()
	{
		_abnormals.Clear();
		NewAbnormalSyncIDs.Clear();
		_trashBuckets.Clear();
		_changedAbnormalSyncIDs.Clear();
	}

	public bool Find(int abnormalMasterID, int index, ref IAbnormalElement element)
	{
		foreach (IAbnormalElement value in _abnormals.Values)
		{
			if (value.AbnormalMasterID == abnormalMasterID && value.Index == index)
			{
				element = value;
				return true;
			}
		}
		return false;
	}

	public void CollectChangedInfo(ref AbnormalSig sig)
	{
		foreach (long newAbnormalSyncID in NewAbnormalSyncIDs)
		{
			CollectInfo(newAbnormalSyncID, AbnormalDataSyncType.Add, ref sig);
		}
		foreach (long changedAbnormalSyncID in _changedAbnormalSyncIDs)
		{
			CollectInfo(changedAbnormalSyncID, AbnormalDataSyncType.Change, ref sig);
		}
		foreach (long trashBucket in _trashBuckets)
		{
			CollectInfo(trashBucket, AbnormalDataSyncType.Remove, ref sig);
		}
	}

	protected abstract void RefineInfo();

	protected abstract void DisposeOnList();

	protected abstract bool CollectInfo(long syncID, AbnormalDataSyncType syncType, ref AbnormalSig sig);
}
