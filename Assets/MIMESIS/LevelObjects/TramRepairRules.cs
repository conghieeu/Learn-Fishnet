using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TramRepairRules : MonoBehaviour
{
	[Serializable]
	private class PartsReconstruction
	{
		public string key;

		public List<GameObject> activateParts;

		public List<GameObject> deactivateParts;
	}

	[Serializable]
	private class TramUpgradePart
	{
		public string upgradeID;

		public List<GameObject> upgradeParts;
	}

	[SerializeField]
	private List<PartsReconstruction> upgradeByRepairInfos;

	[SerializeField]
	private List<PartsReconstruction> brokenByTravelInfos;

	[SerializeField]
	private List<PartsReconstruction> appliedBySteamItemInfos;

	[SerializeField]
	private List<TramUpgradePart> upgradeByTramUpgradeInfos;

	private Dictionary<string, List<GameObject>> activatePartsByKeyDict = new Dictionary<string, List<GameObject>>();

	private Dictionary<string, List<GameObject>> deactivatePartsByKeyDict = new Dictionary<string, List<GameObject>>();

	private void Awake()
	{
		foreach (PartsReconstruction upgradeByRepairInfo in upgradeByRepairInfos)
		{
			activatePartsByKeyDict.Add(upgradeByRepairInfo.key, upgradeByRepairInfo.activateParts);
			deactivatePartsByKeyDict.Add(upgradeByRepairInfo.key, upgradeByRepairInfo.deactivateParts);
		}
		foreach (PartsReconstruction brokenByTravelInfo in brokenByTravelInfos)
		{
			activatePartsByKeyDict.Add(brokenByTravelInfo.key, brokenByTravelInfo.activateParts);
			deactivatePartsByKeyDict.Add(brokenByTravelInfo.key, brokenByTravelInfo.deactivateParts);
		}
		foreach (PartsReconstruction appliedBySteamItemInfo in appliedBySteamItemInfos)
		{
			activatePartsByKeyDict.Add(appliedBySteamItemInfo.key, appliedBySteamItemInfo.activateParts);
			deactivatePartsByKeyDict.Add(appliedBySteamItemInfo.key, appliedBySteamItemInfo.deactivateParts);
		}
		foreach (TramUpgradePart upgradeByTramUpgradeInfo in upgradeByTramUpgradeInfos)
		{
			activatePartsByKeyDict.Add(upgradeByTramUpgradeInfo.upgradeID, upgradeByTramUpgradeInfo.upgradeParts);
		}
	}

	private void OnDestroy()
	{
		activatePartsByKeyDict.Clear();
		deactivatePartsByKeyDict.Clear();
	}

	public void ApplyNewPartsToTram(int repairTimes)
	{
		int currentRepairCount = repairTimes - 1;
		int applyRepairTimes = repairTimes;
		int num = Hub.s.dataman.ExcelDataManager.CycleDict.Values.Max((CycleMasterInfo x) => x.CycleCount);
		if (applyRepairTimes > num)
		{
			currentRepairCount = num;
			applyRepairTimes = num;
		}
		string currentPartsKey = null;
		string newPartsKey = null;
		if (currentRepairCount >= 0)
		{
			CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.CycleDict.Values.FirstOrDefault((CycleMasterInfo c) => c.CycleCount == currentRepairCount);
			currentPartsKey = ((cycleMasterInfo == null) ? "" : cycleMasterInfo.MaintenanceRoomCycleInfo.TramPartsNames[0]);
		}
		CycleMasterInfo cycleMasterInfo2 = Hub.s.dataman.ExcelDataManager.CycleDict.Values.FirstOrDefault((CycleMasterInfo c) => c.CycleCount == applyRepairTimes);
		if (cycleMasterInfo2 != null)
		{
			newPartsKey = cycleMasterInfo2.MaintenanceRoomCycleInfo.TramPartsNames[0];
		}
		ChangeParts(currentPartsKey, newPartsKey);
	}

	public void ApplyDestructionPartsToTramInTramWaitingScene(int cycleCount, int dayCount)
	{
		int num = Hub.s.dataman.ExcelDataManager.CycleDict.Values.Max((CycleMasterInfo x) => x.CycleCount);
		if (cycleCount > num)
		{
			cycleCount = num;
		}
		string value = null;
		Hub.s.dataman.ExcelDataManager.CycleDict.Values.FirstOrDefault((CycleMasterInfo c) => c.CycleCount == cycleCount)?.WaitingRoomCycleInfo.DestructionPartsNames.TryGetValue(dayCount, out value);
		ChangeParts("", value);
	}

	public void ApplyDestructionPartsToTramInGamePlayScene(int cycleCount, int dayCount)
	{
		int num = Hub.s.dataman.ExcelDataManager.CycleDict.Values.Max((CycleMasterInfo x) => x.CycleCount);
		if (cycleCount > num)
		{
			cycleCount = num;
		}
		string value = null;
		Hub.s.dataman.ExcelDataManager.CycleDict.Values.FirstOrDefault((CycleMasterInfo c) => c.CycleCount == cycleCount)?.DungeonCycleInfo.DestructionPartsNames.TryGetValue(dayCount, out value);
		ChangeParts("", value);
	}

	public void ApplyDestructionPartsToTramInMaintenanceScene(int prevCycleCount, int cycleCount)
	{
		int num = Hub.s.dataman.ExcelDataManager.CycleDict.Values.Max((CycleMasterInfo x) => x.CycleCount);
		if (prevCycleCount > num)
		{
			prevCycleCount = num;
		}
		if (cycleCount > num)
		{
			cycleCount = num;
		}
		string currentPartsKey = null;
		string newPartsKey = null;
		CycleMasterInfo cycleMasterInfo = Hub.s.dataman.ExcelDataManager.CycleDict.Values.FirstOrDefault((CycleMasterInfo c) => c.CycleCount == prevCycleCount);
		if (cycleMasterInfo != null)
		{
			currentPartsKey = cycleMasterInfo.MaintenanceRoomCycleInfo.DestructionPartsName;
		}
		CycleMasterInfo cycleMasterInfo2 = Hub.s.dataman.ExcelDataManager.CycleDict.Values.FirstOrDefault((CycleMasterInfo c) => c.CycleCount == cycleCount);
		if (cycleMasterInfo2 != null)
		{
			newPartsKey = cycleMasterInfo2.MaintenanceRoomCycleInfo.DestructionPartsName;
		}
		ChangeParts(currentPartsKey, newPartsKey);
	}

	public void ApplySteamItemPartsToTramInCommon(List<string> appliedTramSkin)
	{
		foreach (string item in appliedTramSkin)
		{
			ChangeParts("", item);
		}
	}

	public void InactivateAllTramUpgradeParts()
	{
		foreach (TramUpgradePart upgradeByTramUpgradeInfo in upgradeByTramUpgradeInfos)
		{
			foreach (GameObject upgradePart in upgradeByTramUpgradeInfo.upgradeParts)
			{
				if (upgradePart != null)
				{
					upgradePart.SetActive(value: false);
				}
				else
				{
					Logger.RError("InactivateAllTramUpgradeParts: part is null");
				}
			}
		}
	}

	public List<GameObject> ApplyTramUpgradePartsToTram(string upgradePart)
	{
		List<GameObject> list = new List<GameObject>();
		if (activatePartsByKeyDict.TryGetValue(upgradePart, out var value))
		{
			foreach (GameObject item in value)
			{
				if (item != null)
				{
					item.SetActive(value: true);
					list.Add(item);
					Logger.RLog("ApplyTramUpgradePartsToTram: " + upgradePart + " - " + item.name);
				}
				else
				{
					Logger.RError("ApplyTramUpgradePartsToTram: " + upgradePart + " - part is null");
				}
			}
		}
		return list;
	}

	private void ChangeParts(string currentPartsKey, string newPartsKey)
	{
		if (currentPartsKey != null && currentPartsKey.Length > 0)
		{
			if (activatePartsByKeyDict.TryGetValue(currentPartsKey, out var value))
			{
				foreach (GameObject item in value)
				{
					if (item != null)
					{
						item.SetActive(value: false);
					}
				}
			}
			if (deactivatePartsByKeyDict.TryGetValue(currentPartsKey, out var value2))
			{
				foreach (GameObject item2 in value2)
				{
					if (item2 != null)
					{
						item2.SetActive(value: true);
					}
				}
			}
		}
		if (newPartsKey == null || newPartsKey.Length <= 0)
		{
			return;
		}
		if (activatePartsByKeyDict.TryGetValue(newPartsKey, out var value3))
		{
			foreach (GameObject item3 in value3)
			{
				if (item3 != null)
				{
					item3.SetActive(value: true);
				}
			}
		}
		if (!deactivatePartsByKeyDict.TryGetValue(newPartsKey, out var value4))
		{
			return;
		}
		foreach (GameObject item4 in value4)
		{
			if (item4 != null)
			{
				item4.SetActive(value: false);
			}
		}
	}
}
