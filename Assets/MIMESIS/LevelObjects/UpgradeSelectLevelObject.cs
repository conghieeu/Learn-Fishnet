using Bifrost.TramupgradeData;
using DarkTonic.MasterAudio;
using Mimic.Actors;
using UnityEngine;

public class UpgradeSelectLevelObject : SimpleSwitchLevelObject
{
	private bool isLeftSelected;

	private int leftUpgradeMasterID;

	private int rightUpgradeMasterID;

	private bool upgradeCompleted;

	[Header("Tram Upgrade Selector")]
	[SerializeField]
	private UpgradeSelectScreen leftUpgradeSelectScreen;

	[SerializeField]
	private UpgradeSelectScreen rightUpgradeSelectScreen;

	[SerializeField]
	private Animator selectAnimator;

	[SerializeField]
	private string leftSelectAnim = "";

	[SerializeField]
	private string rightSelectAnim = "";

	[SerializeField]
	private Animator leftSmallMonitorAnimator;

	[SerializeField]
	private Animator rightSmallMonitorAnimator;

	[SerializeField]
	private string smallMonitorSelectAnim = "";

	[SerializeField]
	private string smallMonitorDeselectAnim = "";

	[SerializeField]
	private string textKeyFirstCycle = "STRING_TRAMUPGRADE_NEWGAME_TEXT";

	[SerializeField]
	private string textKeyAllUpgradesCompleted = "STRING_TRAMUPGRADE_UPGRADECOMPLET";

	[SerializeField]
	private string textKeyUpgradeApplied = "STRING_TRAMUPGRADE_APPLIED_TEXT";

	private EventSounds upgradeSelectEventSounds;

	public void Init(int leftPanel, int rightPanel)
	{
		Logger.RLog($"UpgradeSelectLevelObject::Init: leftPanel={leftPanel}, rightPanel={rightPanel}");
		upgradeCompleted = false;
		leftUpgradeMasterID = leftPanel;
		rightUpgradeMasterID = rightPanel;
		isLeftSelected = true;
		if (Hub.s.tramUpgrade != null)
		{
			if (Hub.s.tramUpgrade.IsUpgraded(leftPanel))
			{
				leftUpgradeSelectScreen.Init(leftPanel, uppgradeApplied: true, isSelected: true);
				upgradeCompleted = true;
			}
			else
			{
				leftUpgradeSelectScreen.Init(leftPanel, uppgradeApplied: false, isSelected: true);
			}
			if (Hub.s.tramUpgrade.IsUpgraded(rightPanel))
			{
				isLeftSelected = false;
				rightUpgradeSelectScreen.Init(rightPanel, uppgradeApplied: true, isSelected: true);
				upgradeCompleted = true;
			}
			else
			{
				rightUpgradeSelectScreen.Init(rightPanel, uppgradeApplied: false, isSelected: false);
			}
		}
		if (selectAnimator != null)
		{
			upgradeSelectEventSounds = selectAnimator.GetComponent<EventSounds>();
		}
		if (isLeftSelected)
		{
			ChangeSelected(0, disableSounds: true);
		}
		else
		{
			ChangeSelected(1, disableSounds: true);
		}
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int nextState)
	{
		bool result = base.IsTriggerable(protoActor, nextState);
		if (Hub.s.tramUpgrade != null && (leftUpgradeMasterID == 0 || rightUpgradeMasterID == 0 || Hub.s.tramUpgrade.IsRepairing || Hub.s.tramUpgrade.IsRepaireCompleted || Hub.s.tramUpgrade.IsUpgraded(leftUpgradeMasterID) || Hub.s.tramUpgrade.IsUpgraded(rightUpgradeMasterID)))
		{
			return false;
		}
		return result;
	}

	protected override void OnSwitchInitialized()
	{
	}

	protected override void OnSwitchStateChanged(bool isOn)
	{
		ToggleUpgrade();
	}

	private void ToggleUpgrade()
	{
		if (!(Hub.s.tramUpgrade != null) || leftUpgradeMasterID == 0 || rightUpgradeMasterID == 0 || Hub.s.tramUpgrade.IsUpgraded(leftUpgradeMasterID) || Hub.s.tramUpgrade.IsUpgraded(rightUpgradeMasterID))
		{
			return;
		}
		if (upgradeSelectEventSounds != null)
		{
			upgradeSelectEventSounds.disableSounds = true;
		}
		MaintenanceScene maintenanceScene = Hub.s.pdata.main as MaintenanceScene;
		if (maintenanceScene != null)
		{
			if (isLeftSelected)
			{
				leftUpgradeSelectScreen.SetSelected(isSelected: false);
				rightUpgradeSelectScreen.SetSelected(isSelected: true);
				maintenanceScene.SelectUpgrade(1);
			}
			else
			{
				leftUpgradeSelectScreen.SetSelected(isSelected: true);
				rightUpgradeSelectScreen.SetSelected(isSelected: false);
				maintenanceScene.SelectUpgrade(0);
			}
		}
	}

	public void ChangeSelected(int index, bool disableSounds = false)
	{
		if (upgradeSelectEventSounds != null)
		{
			upgradeSelectEventSounds.disableSounds = disableSounds;
		}
		isLeftSelected = index == 0;
		if (index == 0)
		{
			leftUpgradeSelectScreen.SetSelected(isSelected: true);
			rightUpgradeSelectScreen.SetSelected(isSelected: false);
			if (selectAnimator != null && leftSelectAnim != "")
			{
				selectAnimator.Play(leftSelectAnim);
			}
			if (leftSmallMonitorAnimator != null && smallMonitorSelectAnim != "")
			{
				leftSmallMonitorAnimator.Play(smallMonitorSelectAnim);
			}
			if (rightSmallMonitorAnimator != null && smallMonitorDeselectAnim != "")
			{
				rightSmallMonitorAnimator.Play(smallMonitorDeselectAnim);
			}
			PlayTriggerSound(0, 1);
		}
		else
		{
			leftUpgradeSelectScreen.SetSelected(isSelected: false);
			rightUpgradeSelectScreen.SetSelected(isSelected: true);
			if (selectAnimator != null && rightSelectAnim != "")
			{
				selectAnimator.Play(rightSelectAnim);
			}
			if (rightSmallMonitorAnimator != null && smallMonitorSelectAnim != "")
			{
				rightSmallMonitorAnimator.Play(smallMonitorSelectAnim);
			}
			if (leftSmallMonitorAnimator != null && smallMonitorDeselectAnim != "")
			{
				leftSmallMonitorAnimator.Play(smallMonitorDeselectAnim);
			}
			PlayTriggerSound(1, 0);
		}
	}

	public void OnUpgradeCompleted(int upgradeMasterID)
	{
		if (upgradeMasterID == 0)
		{
			Logger.RLog("UpgradeSelectLevelObject::OnUpgradeCompleted : first cycle or all upgrades are completed");
			return;
		}
		upgradeCompleted = true;
		if (upgradeMasterID == leftUpgradeMasterID)
		{
			if (leftUpgradeSelectScreen != null)
			{
				leftUpgradeSelectScreen.OnUpgradeApplied();
			}
		}
		else if (upgradeMasterID == rightUpgradeMasterID && rightUpgradeSelectScreen != null)
		{
			rightUpgradeSelectScreen.OnUpgradeApplied();
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			return "";
		}
		if (Hub.s.pdata.CycleCount == 1 && Hub.s.pdata.DayCount == 1)
		{
			return Hub.GetL10NText(textKeyFirstCycle);
		}
		if (upgradeCompleted)
		{
			int num = 0;
			num = ((!isLeftSelected) ? rightUpgradeMasterID : leftUpgradeMasterID);
			if (Hub.s.dataman.ExcelDataManager.TramupgradeDataDict.TryGetValue(num, out TramupgradeData_MasterData value))
			{
				return Hub.GetL10NText(textKeyUpgradeApplied) + "\n" + Hub.GetL10NText(value.upgrade_name);
			}
		}
		if (isLeftSelected)
		{
			if (leftUpgradeMasterID == 0)
			{
				return Hub.GetL10NText(textKeyAllUpgradesCompleted);
			}
			if (Hub.s.dataman.ExcelDataManager.TramupgradeDataDict.TryGetValue(leftUpgradeMasterID, out TramupgradeData_MasterData value2))
			{
				return Hub.GetL10NText(value2.tooltip);
			}
			Logger.RError("UpgradeSelectLevelObject::GetSimpleText : leftUpgradeMasterID is not found");
		}
		else
		{
			if (rightUpgradeMasterID == 0)
			{
				return Hub.GetL10NText(textKeyAllUpgradesCompleted);
			}
			if (Hub.s.dataman.ExcelDataManager.TramupgradeDataDict.TryGetValue(rightUpgradeMasterID, out TramupgradeData_MasterData value3))
			{
				return Hub.GetL10NText(value3.tooltip);
			}
			Logger.RError("UpgradeSelectLevelObject::GetSimpleText : rightUpgradeMasterID is not found");
		}
		return "";
	}
}
