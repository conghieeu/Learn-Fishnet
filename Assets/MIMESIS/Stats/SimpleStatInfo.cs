using System.Collections.Generic;

public class SimpleStatInfo
{
	public Dictionary<StatType, long> ImmutableStats = new Dictionary<StatType, long>();

	public Dictionary<MutableStatType, long> MutableStats = new Dictionary<MutableStatType, long>();
}
