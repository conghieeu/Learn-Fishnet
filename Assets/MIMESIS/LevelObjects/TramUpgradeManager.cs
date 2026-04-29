using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bifrost.TramupgradeData;
using UnityEngine;

public class TramUpgradeManager : MonoBehaviour
{
	private ISet<int> upgradeIDs = new HashSet<int>();

	private ISet<int> addedUpgradeIDs = new HashSet<int>();

	private ISet<int> removedUpgradeIDs = new HashSet<int>();

	private List<ITramUpgradeLevelObject> upgradedAtThisCycle = new List<ITramUpgradeLevelObject>();

	private bool playWhenEnterTrigger;

	private bool playWhenRepaireCompleted;

	public bool IsEnterTrigger;

	public bool IsRepairing;

	public bool IsRepaireCompleted;

	public bool IsUpgraded(int upgradeID)
	{
		return addedUpgradeIDs.Contains(upgradeID);
	}

	public void Init(List<int> currentUpgradeIDs)
	{
		Logger.RLog("TramUpgradeManager::Init: currentUpgradeIDs = [" + string.Join(",", currentUpgradeIDs) + "]");
		upgradeIDs.Clear();
		upgradeIDs.UnionWith(currentUpgradeIDs);
		addedUpgradeIDs.Clear();
		removedUpgradeIDs.Clear();
		upgradedAtThisCycle.Clear();
		playWhenEnterTrigger = false;
		playWhenRepaireCompleted = false;
		IsEnterTrigger = false;
		IsRepairing = false;
		IsRepaireCompleted = false;
	}

	public void UpdateByCandidate(int leftPanel, int rightPanel)
	{
		if (leftPanel != 0 && upgradeIDs.Contains(leftPanel))
		{
			addedUpgradeIDs.Add(leftPanel);
			IsRepaireCompleted = true;
		}
		if (rightPanel != 0 && upgradeIDs.Contains(rightPanel))
		{
			addedUpgradeIDs.Add(rightPanel);
			IsRepaireCompleted = true;
		}
	}

	public void OnStartRepairTramSig(TramRepairRules tramRepairRules)
	{
		IsRepairing = true;
		IsRepaireCompleted = false;
	}

	public void OnChangeTramPartsSig(ChangeTramPartsSig sig, TramRepairRules tramRepairRules)
	{
		HashSet<int> other = new HashSet<int>(upgradeIDs);
		upgradeIDs.Clear();
		upgradeIDs.UnionWith(sig.upgradeList);
		addedUpgradeIDs.Clear();
		addedUpgradeIDs.UnionWith(sig.upgradeList);
		addedUpgradeIDs.ExceptWith(other);
		removedUpgradeIDs.Clear();
		removedUpgradeIDs.UnionWith(other);
		removedUpgradeIDs.ExceptWith(sig.upgradeList);
		ApplyTramUpgradePartsToTramAfterRepair(tramRepairRules);
	}

	public void OnEndRepairTramSig(UpgradeSelectLevelObject upgradeSelectLevelObject)
	{
		IsRepairing = false;
		IsRepaireCompleted = true;
		if (upgradeSelectLevelObject != null)
		{
			if (addedUpgradeIDs.Count > 0)
			{
				upgradeSelectLevelObject.OnUpgradeCompleted(addedUpgradeIDs.FirstOrDefault());
			}
			else
			{
				upgradeSelectLevelObject.OnUpgradeCompleted(0);
			}
		}
	}

	private void ApplyTramUpgradePartsToTramAfterRepair(TramRepairRules tramRepairRules)
	{
		tramRepairRules.InactivateAllTramUpgradeParts();
		foreach (int upgradeID in upgradeIDs)
		{
			if (!Hub.s.dataman.ExcelDataManager.TramupgradeDataDict.TryGetValue(upgradeID, out TramupgradeData_MasterData value))
			{
				continue;
			}
			if (addedUpgradeIDs.Contains(upgradeID))
			{
				foreach (GameObject item in tramRepairRules.ApplyTramUpgradePartsToTram(value.upgrade_tram_parts_name))
				{
					if (!(item == null))
					{
						ITramUpgradeLevelObject componentInChildren = item.GetComponentInChildren<ITramUpgradeLevelObject>();
						if (componentInChildren != null)
						{
							upgradedAtThisCycle.Add(componentInChildren);
						}
					}
				}
				if (value.anim_pointoftime == 1)
				{
					playWhenEnterTrigger = true;
				}
				else if (value.anim_pointoftime == 2)
				{
					playWhenRepaireCompleted = true;
				}
			}
			else
			{
				tramRepairRules.ApplyTramUpgradePartsToTram(value.upgrade_tram_parts_name);
			}
		}
		PlayUpgradeEffect();
	}

	public void ApplyTramUpgradePartsToTramAtSceneInit(TramRepairRules tramRepairRules)
	{
		tramRepairRules.InactivateAllTramUpgradeParts();
		foreach (int upgradeID in upgradeIDs)
		{
			if (Hub.s.dataman.ExcelDataManager.TramupgradeDataDict.TryGetValue(upgradeID, out TramupgradeData_MasterData value))
			{
				tramRepairRules.ApplyTramUpgradePartsToTram(value.upgrade_tram_parts_name);
			}
		}
	}

	public void PlayUpgradeEffect()
	{
		if (upgradedAtThisCycle.Count != 0)
		{
			StartCoroutine(PlayUpgradeEffectCoroutine());
		}
	}

	private IEnumerator PlayUpgradeEffectCoroutine()
	{
		if (playWhenEnterTrigger && playWhenRepaireCompleted)
		{
			Logger.RError("playWhenEnterTrigger and playWhenRepaireCompleted are both true");
		}
		foreach (ITramUpgradeLevelObject item in upgradedAtThisCycle)
		{
			if (item.IsUpgradeActive)
			{
				item.PrepareUpgradeEffect();
			}
		}
		if (playWhenEnterTrigger)
		{
			yield return new WaitUntil(() => IsEnterTrigger);
		}
		else if (playWhenRepaireCompleted)
		{
			yield return new WaitUntil(() => IsRepaireCompleted);
		}
		foreach (ITramUpgradeLevelObject item2 in upgradedAtThisCycle)
		{
			if (item2.IsUpgradeActive)
			{
				item2.PlayUpgradeEffect();
			}
		}
		upgradedAtThisCycle.Clear();
		playWhenEnterTrigger = false;
		playWhenRepaireCompleted = false;
		IsEnterTrigger = false;
		IsRepaireCompleted = false;
	}
}
