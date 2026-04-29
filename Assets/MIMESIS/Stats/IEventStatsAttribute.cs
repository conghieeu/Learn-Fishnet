using System.Collections.Generic;
using ReluProtocol.Enum;

public abstract class IEventStatsAttribute
{
	public readonly EventStatsType EventStatsType;

	public readonly List<(StatType, long)> StatDict = new List<(StatType, long)>();

	public IEventStatsAttribute(EventStatsType eventStatsType)
	{
		EventStatsType = eventStatsType;
	}
}
