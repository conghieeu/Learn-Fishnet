using System.Collections.Generic;
using System.Collections.Immutable;

public class RandomSpawnedMonsterActorData : SpawnedActorData
{
	private readonly int _maxRate;

	public ImmutableDictionary<int, (int, int)> Candidates { get; private set; } = ImmutableDictionary<int, (int, int)>.Empty;

	public int PickedMonsterMasterID { get; private set; }

	public RandomSpawnedMonsterActorData(MapMarker_SpawnPoint spawnPointData, ImmutableDictionary<int, (int, int)> candidateElements, int maxRate)
		: base(spawnPointData)
	{
		Candidates = candidateElements;
		_maxRate = maxRate;
	}

	public (int pickedMonsterMasterID, int remainValue, int pickedThreatValue) GetPickedMonsterValue(int remainValue)
	{
		if (remainValue <= 0)
		{
			return (pickedMonsterMasterID: 0, remainValue: 0, pickedThreatValue: 0);
		}
		int num = SimpleRandUtil.Next(0, _maxRate);
		foreach (KeyValuePair<int, (int, int)> candidate in Candidates)
		{
			if (num < candidate.Value.Item1)
			{
				PickedMonsterMasterID = candidate.Key;
				return (pickedMonsterMasterID: PickedMonsterMasterID, remainValue: remainValue - candidate.Value.Item2, pickedThreatValue: candidate.Value.Item2);
			}
		}
		return (pickedMonsterMasterID: 0, remainValue: 0, pickedThreatValue: 0);
	}

	public int GetPickedMonsterValue()
	{
		if (Candidates.Count == 0)
		{
			return 0;
		}
		int num = SimpleRandUtil.Next(0, _maxRate);
		foreach (KeyValuePair<int, (int, int)> candidate in Candidates)
		{
			if (num < candidate.Value.Item1)
			{
				PickedMonsterMasterID = candidate.Key;
				return PickedMonsterMasterID;
			}
		}
		return 0;
	}
}
