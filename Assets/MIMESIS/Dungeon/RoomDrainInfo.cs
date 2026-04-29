using System.Collections.Generic;
using System.Linq;
using ReluProtocol;
using ReluProtocol.Enum;

public class RoomDrainInfo
{
	private List<DrainLootingObjectInfo> LootingObjects = new List<DrainLootingObjectInfo>();

	private Dictionary<long, long> _playerRemainOxygens = new Dictionary<long, long>();

	public long RoomID { get; private set; }

	public int DayCount { get; private set; }

	public int BoostedItemMasterID { get; private set; }

	public float BoostedRate { get; private set; }

	public Dictionary<int, ItemElement> Stashes { get; private set; } = new Dictionary<int, ItemElement>();

	public List<int> TramUpgradeList { get; private set; } = new List<int>();

	public Dictionary<long, List<int>> ItemsToProvide { get; private set; } = new Dictionary<long, List<int>>();

	public RoomDrainInfo(long roomID, int dayCount)
	{
		RoomID = roomID;
		DayCount = dayCount;
	}

	public void IncreaseDayCount()
	{
		DayCount++;
	}

	public void AddLootingObject(ItemElement itemElement, PosWithRot position, ReasonOfSpawn reasonOfSpawn)
	{
		if (LootingObjects.Any((DrainLootingObjectInfo x) => x.ItemElement == itemElement))
		{
			Logger.RWarn("RoomDrainInfo: AddLootingObject - Already exist itemElement: " + itemElement);
			return;
		}
		LootingObjects.Add(new DrainLootingObjectInfo
		{
			ItemElement = itemElement,
			PosWithRot = position,
			ReasonOfSpawn = reasonOfSpawn
		});
	}

	public List<DrainLootingObjectInfo> GetLootingObjects()
	{
		return LootingObjects;
	}

	public void SetPlayerContaValue(long playerUID, long contaValue)
	{
		if (_playerRemainOxygens.ContainsKey(playerUID))
		{
			Logger.RWarn("RoomDrainInfo: SetPlayerRemainOxygen - Already exist playerUID: " + playerUID);
			_playerRemainOxygens[playerUID] = contaValue;
		}
		else
		{
			_playerRemainOxygens.Add(playerUID, contaValue);
		}
	}

	public Dictionary<long, long> GetPlayerRemainOxygens()
	{
		return _playerRemainOxygens;
	}

	public void SetBoostedItem(int itemID, float value)
	{
		BoostedItemMasterID = itemID;
		BoostedRate = value;
	}

	public void AddReservedItem(long playerUID, int itemMasterID)
	{
		if (!ItemsToProvide.ContainsKey(playerUID))
		{
			ItemsToProvide.Add(playerUID, new List<int>());
		}
		if (!ItemsToProvide[playerUID].Contains(itemMasterID))
		{
			ItemsToProvide[playerUID].Add(itemMasterID);
		}
	}

	public void SetTramUpgradeList(List<int> upgradeList)
	{
		TramUpgradeList.Clear();
		TramUpgradeList.AddRange(upgradeList);
	}

	public void SetStash(Dictionary<int, ItemElement> stashes)
	{
		Stashes.Clear();
		foreach (KeyValuePair<int, ItemElement> stash in stashes)
		{
			Stashes[stash.Key] = stash.Value.Clone();
		}
	}
}
