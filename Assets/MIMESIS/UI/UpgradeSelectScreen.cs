using System;
using System.Collections.Generic;
using Bifrost.TramupgradeData;
using UnityEngine;

public class UpgradeSelectScreen : MonoBehaviour
{
	[Serializable]
	public class UpgradeScreenSpriteAnim
	{
		public string name;

		public GameObject upgradeScreen;
	}

	private int upgradeMasterID;

	private bool isSelected;

	private bool isSelectable;

	private bool isNotUsable;

	private bool isUpgradeApplied;

	private bool isAllUpgradesCompleted;

	[SerializeField]
	private List<UpgradeScreenSpriteAnim> upgradeScreenSpriteAnims;

	private Dictionary<string, GameObject> upgradeScreenSpriteAnimsDict = new Dictionary<string, GameObject>();

	private List<GameObject> upgradeScreenSpriteAnimsList = new List<GameObject>();

	[SerializeField]
	private GameObject backgroundImage;

	private void Awake()
	{
		foreach (UpgradeScreenSpriteAnim upgradeScreenSpriteAnim in upgradeScreenSpriteAnims)
		{
			if (upgradeScreenSpriteAnim.upgradeScreen != null)
			{
				upgradeScreenSpriteAnimsDict.Add(upgradeScreenSpriteAnim.name, upgradeScreenSpriteAnim.upgradeScreen);
				upgradeScreenSpriteAnimsList.Add(upgradeScreenSpriteAnim.upgradeScreen);
			}
			else
			{
				Logger.RError("UpgradeSelectLevelObject::Awake : " + upgradeScreenSpriteAnim.name + " upgradeScreen is null");
			}
		}
	}

	private void InactiveAllScreens()
	{
		foreach (GameObject upgradeScreenSpriteAnims in upgradeScreenSpriteAnimsList)
		{
			upgradeScreenSpriteAnims.SetActive(value: false);
		}
	}

	public void Init(int upgradeMasterID, bool uppgradeApplied, bool isSelected)
	{
		this.upgradeMasterID = upgradeMasterID;
		this.isSelected = isSelected;
		isSelectable = false;
		isUpgradeApplied = uppgradeApplied;
		isAllUpgradesCompleted = false;
		if (Hub.s.pdata.DayCount == 1 && Hub.s.pdata.CycleCount == 1)
		{
			isNotUsable = true;
			OnUpdateScreen();
			return;
		}
		if (Hub.s.dataman.ExcelDataManager.TramupgradeDataDict.Count == Hub.s.pdata.TramUpgradeIDs.Count)
		{
			isAllUpgradesCompleted = true;
			OnUpdateScreen();
			return;
		}
		if (upgradeMasterID != 0)
		{
			isSelectable = true;
		}
		else
		{
			isNotUsable = true;
			isSelectable = false;
		}
		OnUpdateScreen();
	}

	public void OnUpdateScreen()
	{
		InactiveAllScreens();
		string key = "";
		if (isNotUsable)
		{
			key = "NotAvailable";
			if (backgroundImage != null)
			{
				backgroundImage.SetActive(value: false);
			}
		}
		else if (isAllUpgradesCompleted)
		{
			key = "None";
			if (backgroundImage != null)
			{
				backgroundImage.SetActive(value: false);
			}
		}
		else if (isSelectable)
		{
			if (Hub.s.dataman.ExcelDataManager.TramupgradeDataDict.TryGetValue(upgradeMasterID, out TramupgradeData_MasterData value))
			{
				key = value.name;
			}
			else
			{
				Logger.RError($"UpgradeSelectLevelObject::OnUpdateScreen : {upgradeMasterID} is not found");
			}
			if (backgroundImage != null)
			{
				backgroundImage.SetActive(value: true);
			}
		}
		if (upgradeScreenSpriteAnimsDict.TryGetValue("Upgraded", out var value2))
		{
			value2.SetActive(isUpgradeApplied);
			if (isUpgradeApplied)
			{
				if (backgroundImage != null)
				{
					backgroundImage.SetActive(value: false);
				}
				return;
			}
		}
		if (upgradeScreenSpriteAnimsDict.TryGetValue(key, out var value3))
		{
			value3.SetActive(value: true);
			if (upgradeScreenSpriteAnimsDict.TryGetValue("Selected", out var value4))
			{
				value4.SetActive(isSelectable && isSelected);
			}
		}
	}

	public bool IsSelected()
	{
		return isSelected;
	}

	public void SetSelected(bool isSelected)
	{
		this.isSelected = isSelected;
		OnUpdateScreen();
	}

	public void OnUpgradeApplied()
	{
		isUpgradeApplied = true;
		OnUpdateScreen();
	}
}
