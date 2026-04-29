using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public class GameConditionGroup
{
	public readonly ImmutableDictionary<int, ImmutableDictionary<int, IGameCondition>> ConditionDict;

	public GameConditionGroup(ImmutableDictionary<int, ImmutableDictionary<int, IGameCondition>> conditionDict)
	{
		ConditionDict = conditionDict;
	}

	public GameConditionGroup(GameConditionGroup conditionGroup)
	{
		ImmutableDictionary<int, ImmutableDictionary<int, IGameCondition>>.Builder builder = ImmutableDictionary.CreateBuilder<int, ImmutableDictionary<int, IGameCondition>>();
		foreach (KeyValuePair<int, ImmutableDictionary<int, IGameCondition>> item in conditionGroup.ConditionDict)
		{
			ImmutableDictionary<int, IGameCondition>.Builder builder2 = ImmutableDictionary.CreateBuilder<int, IGameCondition>();
			foreach (KeyValuePair<int, IGameCondition> item2 in item.Value)
			{
				builder2.Add(item2.Key, item2.Value.Clone());
			}
			builder.Add(item.Key, builder2.ToImmutable());
		}
		ConditionDict = builder.ToImmutable();
	}

	public void ForceComplete()
	{
		if (ConditionDict.Count != 0)
		{
			KeyValuePair<int, ImmutableDictionary<int, IGameCondition>> keyValuePair = ConditionDict.FirstOrDefault();
			if (keyValuePair.Value.Count != 0)
			{
				keyValuePair.Value.FirstOrDefault().Value.ForceComplete();
			}
		}
	}

	public bool IsComplete()
	{
		if (ConditionDict.Count == 0)
		{
			return true;
		}
		bool flag = true;
		foreach (ImmutableDictionary<int, IGameCondition> value in ConditionDict.Values)
		{
			foreach (IGameCondition value2 in value.Values)
			{
				flag &= value2.IsComplete();
			}
			if (flag)
			{
				return true;
			}
		}
		return false;
	}
}
