using System;
using System.Collections.Generic;

public class EventStats : CommonStats
{
	private bool _changed;

	private Dictionary<long, Dictionary<int, AbnormalStatsAttribute>> _abnormalStatsDict = new Dictionary<long, Dictionary<int, AbnormalStatsAttribute>>();

	public EventStats()
		: base(StatCategory.Event)
	{
		_changed = false;
	}

	public bool RefreshStats()
	{
		if (_changed)
		{
			CollectStats();
		}
		_changed = false;
		return true;
	}

	public void CollectStats()
	{
		long[] array = new long[15];
		foreach (StatType value in Enum.GetValues(typeof(StatType)))
		{
			if (value != StatType.Invalid && value != StatType.MAX)
			{
				array[(int)value] = elements[value].Value;
				elements[value].Set(0L);
				elements[value].Sync();
			}
		}
		foreach (Dictionary<int, AbnormalStatsAttribute> value2 in _abnormalStatsDict.Values)
		{
			foreach (KeyValuePair<int, AbnormalStatsAttribute> item in value2)
			{
				elements[item.Value.type].Add(item.Value.value);
			}
		}
		foreach (StatType value3 in Enum.GetValues(typeof(StatType)))
		{
			if (value3 != StatType.Invalid && value3 != StatType.MAX)
			{
				if (array[(int)value3] == elements[value3].Value)
				{
					elements[value3].Sync();
				}
				else if (!elements[value3].IsDirty)
				{
					elements[value3].Sync(flag: true);
				}
			}
		}
		CollectDirtyStats();
	}

	public bool Exist()
	{
		return _abnormalStatsDict.Count > 0;
	}

	public bool AddAbnormalStats(long syncID, StatType type, long value, int index)
	{
		if (_abnormalStatsDict.TryGetValue(syncID, out var value2))
		{
			value2.Add(index, new AbnormalStatsAttribute
			{
				type = type,
				value = value
			});
		}
		else
		{
			_abnormalStatsDict[syncID] = new Dictionary<int, AbnormalStatsAttribute> { 
			{
				index,
				new AbnormalStatsAttribute
				{
					type = type,
					value = value
				}
			} };
		}
		_changed = true;
		return true;
	}

	public bool UpdateAbnormalStats(long syncID, long value, int index)
	{
		if (!_abnormalStatsDict.TryGetValue(syncID, out var value2))
		{
			return false;
		}
		if (!value2.TryGetValue(index, out var value3))
		{
			return false;
		}
		value3.value = value;
		_changed = true;
		return true;
	}

	public bool RemoveAbnormalStats(long syncID)
	{
		if (!_abnormalStatsDict.ContainsKey(syncID))
		{
			return false;
		}
		_abnormalStatsDict.Remove(syncID);
		_changed = true;
		return true;
	}
}
