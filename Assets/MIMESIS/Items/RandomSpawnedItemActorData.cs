using System.Collections.Generic;
using System.Collections.Immutable;
using Bifrost.Cooked;

public class RandomSpawnedItemActorData : SpawnedActorData
{
	private List<string> _itemPropertyKeys = new List<string>();

	private int _maxRate;

	public ImmutableDictionary<int, (int rate, int meanPrice)> Candidates { get; private set; } = ImmutableDictionary<int, (int, int)>.Empty;

	public RandomSpawnedItemActorData(MapMarker_LootingObjectSpawnPoint spawnPointData, ImmutableDictionary<int, (int rate, int meanPrice)> itemRateDict)
		: base(spawnPointData)
	{
		if (spawnPointData.itemPropertyKey.Length > 0)
		{
			string[] collection = spawnPointData.itemPropertyKey.Split(',');
			_itemPropertyKeys.AddRange(collection);
		}
		ImmutableDictionary<int, (int, int)>.Builder builder = ImmutableDictionary.CreateBuilder<int, (int, int)>();
		foreach (KeyValuePair<int, (int, int)> item in itemRateDict)
		{
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(item.Key);
			if (itemInfo == null)
			{
				Logger.RError("ItemInfo is null. MasterID : " + item.Key);
			}
			else if (_itemPropertyKeys.Count == 0 || _itemPropertyKeys.Contains(itemInfo.PropertyKey))
			{
				_maxRate += item.Value.Item1;
				builder.Add(item.Key, (_maxRate, item.Value.Item2));
			}
		}
		Candidates = builder.ToImmutable();
	}

	public (int itemMasterID, int remainValue, int itemValue) GetPickedItemValue(int remainValue)
	{
		if (remainValue <= 0)
		{
			return (itemMasterID: 0, remainValue: 0, itemValue: 0);
		}
		int num = SimpleRandUtil.Next(0, _maxRate);
		foreach (KeyValuePair<int, (int, int)> candidate in Candidates)
		{
			if (num < candidate.Value.Item1)
			{
				return (itemMasterID: candidate.Key, remainValue: remainValue - candidate.Value.Item2, itemValue: candidate.Value.Item2);
			}
		}
		return (itemMasterID: 0, remainValue: 0, itemValue: 0);
	}

	public int GetPickedItemValue()
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
				return candidate.Key;
			}
		}
		return 0;
	}
}
