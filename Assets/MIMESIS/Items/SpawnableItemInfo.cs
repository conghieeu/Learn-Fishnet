using System.Collections.Immutable;
using System.Linq;
using Bifrost.SpawnableMiscGroup;

public class SpawnableItemInfo
{
	public readonly int ID;

	public ImmutableDictionary<int, int> MiscRateDict = ImmutableDictionary<int, int>.Empty;

	public SpawnableItemInfo(SpawnableMiscGroup_MasterData data)
	{
		ID = data.id;
		ImmutableDictionary<int, int>.Builder builder = ImmutableDictionary.CreateBuilder<int, int>();
		foreach (SpawnableMiscGroup_candidate item in data.SpawnableMiscGroup_candidateval.OrderBy((SpawnableMiscGroup_candidate m) => m.misc_id).ToList())
		{
			if (builder.ContainsKey(item.misc_id))
			{
				Logger.RError("SpawnableItemInfo: MiscRateDict already contains key: " + item.misc_id);
			}
			else
			{
				builder.Add(item.misc_id, item.rate);
			}
		}
		MiscRateDict = builder.ToImmutable();
	}
}
