using System;
using System.Collections.Generic;

public class CommonStats
{
	public readonly StatCategory StatCategory;

	public Dictionary<StatType, StatsElement> elements = new Dictionary<StatType, StatsElement>();

	public HashSet<StatType> AlteredStatTypes { get; protected set; } = new HashSet<StatType>();

	public CommonStats(StatCategory category)
	{
		StatCategory = category;
		foreach (StatType value in Enum.GetValues(typeof(StatType)))
		{
			if (value != StatType.Invalid)
			{
				elements[value] = new StatsElement(0L);
			}
		}
	}

	public long GetStatValue(StatType statType)
	{
		return elements[statType].Value;
	}

	public void ClearStats(bool force = false)
	{
		if (force)
		{
			foreach (StatType value in Enum.GetValues(typeof(StatType)))
			{
				if (value != StatType.Invalid)
				{
					if (elements[value].Value != 0L)
					{
						AlteredStatTypes.Add(value);
					}
					elements[value].Set(0L);
				}
			}
			return;
		}
		foreach (StatType value2 in Enum.GetValues(typeof(StatType)))
		{
			if (value2 != StatType.Invalid && elements[value2].Value != 0L)
			{
				elements[value2].Set(0L);
			}
		}
	}

	public void OnSyncComplete()
	{
		if (AlteredStatTypes.Count == 0)
		{
			return;
		}
		AlteredStatTypes.Clear();
		foreach (KeyValuePair<StatType, StatsElement> element in elements)
		{
			element.Value.Sync();
		}
	}

	public void CollectDirtyStats()
	{
		AlteredStatTypes.Clear();
		foreach (KeyValuePair<StatType, StatsElement> element in elements)
		{
			if (element.Value.IsDirty)
			{
				AlteredStatTypes.Add(element.Key);
			}
		}
	}
}
