using System.Collections.Immutable;
using System.Linq;
using Bifrost.SpawnableMonsterGroup;

public class SpawnableMonsterInfo
{
	public readonly int ID;

	public ImmutableDictionary<int, int> MonsterRateDict = ImmutableDictionary<int, int>.Empty;

	public readonly int MaxRate;

	public SpawnableMonsterInfo(SpawnableMonsterGroup_MasterData data)
	{
		ID = data.id;
		ImmutableDictionary<int, int>.Builder builder = ImmutableDictionary.CreateBuilder<int, int>();
		int num = 0;
		foreach (SpawnableMonsterGroup_candidate item in data.SpawnableMonsterGroup_candidateval.OrderBy((SpawnableMonsterGroup_candidate m) => m.monster_id).ToList())
		{
			num += item.rate;
			builder.Add(item.monster_id, num);
		}
		MonsterRateDict = builder.ToImmutable();
		MaxRate = num;
	}
}
