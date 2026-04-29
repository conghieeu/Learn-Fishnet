using System.Collections.Generic;
using System.Linq;
using WorldServer.Controller.Cooltime.Info;

public class SkillCooltimeManager : ICooltimeManager
{
	private Dictionary<int, long> _skillPressedTime = new Dictionary<int, long>();

	public SkillCooltimeManager(CooltimeController controller)
		: base(controller)
	{
	}

	public override bool AddCooltime(long syncID, long duration, int masterID, bool global, bool sync)
	{
		if (_skillPressedTime.TryGetValue(masterID, out var value))
		{
			RemoveCooltime(value);
			if (_cooltimeDict.TryGetValue(value, out var value2))
			{
				value2.SetDelete();
			}
		}
		SkillCooltimeElement elem = new SkillCooltimeElement(masterID, syncID, duration, global, sync);
		if (!AddCooltime(elem))
		{
			return false;
		}
		_skillPressedTime[masterID] = syncID;
		return true;
	}

	public override bool IsCooltime(int id)
	{
		return _skillPressedTime.ContainsKey(id);
	}

	public override void RemoveCooltime(long syncID)
	{
		int num = (from x in _skillPressedTime
			where x.Value == syncID
			select x.Key).FirstOrDefault();
		if (num != 0)
		{
			RemoveCooltime(num);
		}
	}

	protected override void EmptyTrashBucket()
	{
		foreach (long syncID in _trashBucket)
		{
			int num = (from x in _skillPressedTime
				where x.Value == syncID
				select x.Key).FirstOrDefault();
			if (num != 0)
			{
				_skillPressedTime.Remove(num);
			}
		}
		base.EmptyTrashBucket();
	}

	public bool RemoveCooltime(int masterID)
	{
		if (_skillPressedTime.TryGetValue(masterID, out var value))
		{
			if (_cooltimeDict.TryGetValue(value, out var value2))
			{
				value2.SetDelete();
				return true;
			}
			_skillPressedTime.Remove(masterID);
		}
		return false;
	}

	public override void Clear()
	{
		base.Clear();
		_skillPressedTime.Clear();
	}
}
