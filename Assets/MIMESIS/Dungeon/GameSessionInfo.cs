using System.Collections.Generic;
using ReluProtocol;

public class GameSessionInfo
{
	private int _targetCurrency;

	public List<DrainLootingObjectInfo> LootingObjects = new List<DrainLootingObjectInfo>();

	private bool _dirtyFlag;

	public int NextDungeonMasterID { get; private set; }

	public int RandomDungeonSeed { get; private set; }

	public long CurrentSessionID { get; private set; }

	public VGameSessionState GameSessionState { get; private set; } = VGameSessionState.Ready;

	public int CurrentGameCount { get; private set; }

	public Dictionary<ulong, bool> TotalPlayerSteamIDs { get; private set; } = new Dictionary<ulong, bool>();

	public Dictionary<ulong, long> FixedPlayerSteamIDs { get; private set; } = new Dictionary<ulong, long>();

	public int IterationCount { get; private set; } = 1;

	public Dictionary<long, long> PlayerConta { get; private set; } = new Dictionary<long, long>();

	public int BoostedItemMasterID { get; private set; }

	public float BoostedRate { get; private set; }

	public Dictionary<long, List<int>> ItemsToProvide { get; private set; } = new Dictionary<long, List<int>>();

	public Dictionary<int, ItemElement> Stashes { get; private set; } = new Dictionary<int, ItemElement>();

	public List<int> TramUpgradeList { get; private set; } = new List<int>();

	public bool PrevDungeonSuccess { get; private set; }

	public bool SetGameSessionState(VGameSessionState nextState, VGameSessionState prevState)
	{
		if (GameSessionState != prevState && prevState != VGameSessionState.Invalid)
		{
			Logger.RWarn("GameSessionInfo: SetGameSessionState - Invalid state transition. Current state: " + GameSessionState.ToString() + ", Prev state: " + prevState);
		}
		GameSessionState = nextState;
		return true;
	}

	public void InitCurrentSessionID()
	{
		CurrentSessionID = SimpleRandUtil.Next(0, int.MaxValue);
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(IterationCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError("GameSessionInfo: InitCurrentSessionID - Not exist CycleMasterInfo. IterationCount: " + IterationCount);
		}
		else
		{
			_targetCurrency = cycleMasterInfo.Quota;
		}
	}

	public void SetNextDungeonMasterID(int masterID, int randomDungeonSeed)
	{
		NextDungeonMasterID = masterID;
		RandomDungeonSeed = randomDungeonSeed;
		_dirtyFlag = true;
	}

	public bool AddPlayerSteamID(ulong steamID, bool isHost)
	{
		if (TotalPlayerSteamIDs.ContainsKey(steamID))
		{
			return false;
		}
		if (isHost && TotalPlayerSteamIDs.ContainsValue(isHost))
		{
			Logger.RWarn("GameSessionInfo: AddPlayerUID - Already exist host player. steamID: " + steamID);
			return false;
		}
		if (TotalPlayerSteamIDs.Count >= Hub.s.dataman.ExcelDataManager.Consts.C_MaxPlayerCount)
		{
			Logger.RWarn("GameSessionInfo: AddPlayerUID - Exceed max player count. steamID: " + steamID);
			return false;
		}
		TotalPlayerSteamIDs.Add(steamID, isHost);
		return true;
	}

	public void RemoveSteamID(ulong steamID)
	{
		TotalPlayerSteamIDs.Remove(steamID);
		if (!FixedPlayerSteamIDs.TryGetValue(steamID, out var value))
		{
			Logger.RWarn("GameSessionInfo: RemoveSteamID - Not exist playerUID. steamID: " + steamID);
			return;
		}
		FixedPlayerSteamIDs.Remove(steamID);
		PlayerConta.Remove(value);
	}

	public void SetPlayerUIDs(Dictionary<ulong, long> playerUIDs)
	{
		FixedPlayerSteamIDs.Clear();
		foreach (KeyValuePair<ulong, long> playerUID in playerUIDs)
		{
			if (!TotalPlayerSteamIDs.ContainsKey(playerUID.Key))
			{
				Logger.RWarn("GameSessionInfo: SetPlayerUIDs - Not exist playerUID. steamID: " + playerUID.Key);
			}
			else
			{
				FixedPlayerSteamIDs.Add(playerUID.Key, playerUID.Value);
			}
		}
	}

	public void ApplyPrevDungeonState(bool success)
	{
		PrevDungeonSuccess = success;
	}

	public void ApplyDrainInfo(RoomDrainInfo drainInfo)
	{
		CurrentGameCount = drainInfo.DayCount;
		LootingObjects.Clear();
		foreach (DrainLootingObjectInfo lootingObject in drainInfo.GetLootingObjects())
		{
			LootingObjects.Add(lootingObject);
		}
		PlayerConta.Clear();
		foreach (KeyValuePair<long, long> playerRemainOxygen in drainInfo.GetPlayerRemainOxygens())
		{
			PlayerConta.Add(playerRemainOxygen.Key, playerRemainOxygen.Value);
		}
		BoostedItemMasterID = drainInfo.BoostedItemMasterID;
		BoostedRate = drainInfo.BoostedRate;
		ItemsToProvide.Clear();
		foreach (KeyValuePair<long, List<int>> item in drainInfo.ItemsToProvide)
		{
			if (!ItemsToProvide.TryGetValue(item.Key, out var value))
			{
				value = new List<int>();
				ItemsToProvide.Add(item.Key, value);
			}
			value.AddRange(item.Value);
		}
		Stashes.Clear();
		foreach (KeyValuePair<int, ItemElement> stash in drainInfo.Stashes)
		{
			Stashes[stash.Key] = stash.Value?.Clone();
		}
		TramUpgradeList.Clear();
		TramUpgradeList.AddRange(drainInfo.TramUpgradeList);
	}

	public void ApplyLoadedGameData(MMSaveGameData saveGameData)
	{
		IterationCount = saveGameData.CycleCount;
		CurrentGameCount = saveGameData.DayCount;
		BoostedItemMasterID = saveGameData.BoostedItemMasterID;
		BoostedRate = saveGameData.BoostedItemRate;
		LootingObjects.Clear();
		PlayerConta.Clear();
		ItemsToProvide.Clear();
	}

	public void IncreaseIterationCount(bool reset = false)
	{
		if (reset)
		{
			IterationCount = 1;
		}
		else
		{
			IterationCount++;
		}
	}

	public int GetCurrencyThreshold()
	{
		if (IterationCount < 5)
		{
			_ = IterationCount;
		}
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.GetCycleMasterInfo(IterationCount);
		if (cycleMasterInfo == null)
		{
			Logger.RError("GameSessionInfo: GetCurrencyThreshold - Not exist CycleMasterInfo. IterationCount: " + IterationCount);
			return 0;
		}
		_targetCurrency = cycleMasterInfo.Quota;
		return _targetCurrency;
	}

	public int Promote()
	{
		IncreaseIterationCount();
		CurrentGameCount = 1;
		SetGameSessionState(VGameSessionState.PreGame, VGameSessionState.EndGame);
		NextDungeonMasterID = 0;
		foreach (long value in FixedPlayerSteamIDs.Values)
		{
			PlayerConta[value] = 0L;
		}
		return GetCurrencyThreshold();
	}

	public bool CanEnterSession()
	{
		if (GameSessionState != VGameSessionState.Ready && GameSessionState != VGameSessionState.WaitStartSession)
		{
			return GameSessionState == VGameSessionState.EndGame;
		}
		return true;
	}

	public void Reset(VGameSessionState prevState, bool init = false)
	{
		IncreaseIterationCount(reset: true);
		CurrentGameCount = 1;
		SetGameSessionState(init ? VGameSessionState.Ready : VGameSessionState.WaitStartSession, prevState);
		NextDungeonMasterID = 0;
		foreach (long value in FixedPlayerSteamIDs.Values)
		{
			PlayerConta[value] = 0L;
		}
		if (init)
		{
			InitCurrentSessionID();
		}
		LootingObjects.Clear();
		ItemsToProvide.Clear();
		Stashes.Clear();
	}

	public void SetBoostedItem(int itemID, float value)
	{
		BoostedItemMasterID = itemID;
		BoostedRate = value;
	}

	public void SetCurrentCycleForDebug(int cycle)
	{
		IterationCount = cycle;
		GameSessionState = VGameSessionState.EndGame;
	}
}
