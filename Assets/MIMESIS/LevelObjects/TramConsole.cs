using Bifrost.Cooked;
using TMPro;
using UnityEngine;

public class TramConsole : MonoBehaviour
{
	[SerializeField]
	protected UIPrefab_BackScreen ui_backScreen;

	[SerializeField]
	protected Transform nextSceneLever;

	[SerializeField]
	protected Transform resultDecisionLever;

	[SerializeField]
	protected UIPrefab_RepairScreenInTram ui_RepairScreenInTram;

	[SerializeField]
	protected UIPrefab_WantedPoster ui_WantedPoster;

	[SerializeField]
	public TMP_Text? dayCountProgress;

	public void InitCommonUI(bool isMaintenanceScene)
	{
		if (ui_backScreen != null && !ui_backScreen.isActiveAndEnabled)
		{
			ui_backScreen.Show();
		}
		if (ui_RepairScreenInTram != null)
		{
			if (isMaintenanceScene)
			{
				ui_RepairScreenInTram.Show();
			}
			else
			{
				ui_RepairScreenInTram.Hide();
			}
		}
		if (ui_WantedPoster != null && !ui_WantedPoster.isActiveAndEnabled)
		{
			ui_WantedPoster.Show();
		}
		if (ui_backScreen != null)
		{
			ui_backScreen.SetCollectedCurrencyMode();
		}
	}

	public void UpdateRepairState(bool isRepaired, bool isRepairing)
	{
		if (ui_RepairScreenInTram != null)
		{
			ui_RepairScreenInTram.UpdateRepairState(isRepaired, isRepairing);
		}
	}

	public void UpdateLeverState(bool maintenanceDecision)
	{
		if (nextSceneLever != null)
		{
			nextSceneLever.gameObject.SetActive(!maintenanceDecision);
		}
		if (resultDecisionLever != null)
		{
			resultDecisionLever.gameObject.SetActive(maintenanceDecision);
		}
	}

	public void UpdateCurrentCurrency(int currentCurrency)
	{
		if (ui_backScreen != null)
		{
			ui_backScreen.UpdateCurrentCurrency(currentCurrency);
		}
	}

	public void UpdateCollectedCurrency(int stashToCurrency, int targetCurrency)
	{
		if (ui_backScreen != null)
		{
			ui_backScreen.UpdateCollectedCurrency(stashToCurrency, targetCurrency);
		}
	}

	public void UpdateWantedPoster(ItemMasterInfo? itemMasterInfo, float priceRatio)
	{
		if (ui_WantedPoster != null)
		{
			ui_WantedPoster.SetWantedWallPaper(itemMasterInfo);
		}
	}

	public void SetDayCount(int dayCount)
	{
		if (dayCountProgress != null)
		{
			int num = (int)Hub.s.dataman.ExcelDataManager.Consts.C_AvailableDayPerSession - dayCount + 1;
			switch (num)
			{
			case 0:
				dayCountProgress.text = Hub.GetL10NText("dayCountProgressText_C");
				break;
			case 1:
				dayCountProgress.text = Hub.GetL10NText("dayCountProgressText_B", num);
				break;
			default:
				dayCountProgress.text = Hub.GetL10NText("dayCountProgressText_B", num);
				break;
			}
		}
	}

	public void SetDayCountInMaintenanceScene(bool dayZero)
	{
		if (dayCountProgress != null)
		{
			if (dayZero)
			{
				dayCountProgress.text = Hub.GetL10NText("dayCountProgressText_B", 0);
			}
			else
			{
				dayCountProgress.text = Hub.GetL10NText("dayCountProgressText_B", 3);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
