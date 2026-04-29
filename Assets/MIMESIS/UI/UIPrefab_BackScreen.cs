using TMPro;
using UnityEngine;

public class UIPrefab_BackScreen : UIPrefabScript
{
	public const string UEID_TargetCurrency = "TargetCurrency";

	public const string UEID_RepairValue = "RepairValue";

	public const string UEID_CollectValue = "CollectValue";

	public const string UEID_CurrentMoney = "CurrentMoney";

	public const string UEID_Txt_RepairValue = "Txt_RepairValue";

	public const string UEID_Txt_CollectValue = "Txt_CollectValue";

	public const string UEID_Txt_CurrentMoney = "Txt_CurrentMoney";

	private Transform _UE_TargetCurrency;

	private TMP_Text _UE_RepairValue;

	private TMP_Text _UE_CollectValue;

	private TMP_Text _UE_CurrentMoney;

	private TMP_Text _UE_Txt_RepairValue;

	private TMP_Text _UE_Txt_CollectValue;

	private TMP_Text _UE_Txt_CurrentMoney;

	public Transform UE_TargetCurrency => _UE_TargetCurrency ?? (_UE_TargetCurrency = PickTransform("TargetCurrency"));

	public TMP_Text UE_RepairValue => _UE_RepairValue ?? (_UE_RepairValue = PickText("RepairValue"));

	public TMP_Text UE_CollectValue => _UE_CollectValue ?? (_UE_CollectValue = PickText("CollectValue"));

	public TMP_Text UE_CurrentMoney => _UE_CurrentMoney ?? (_UE_CurrentMoney = PickText("CurrentMoney"));

	public TMP_Text UE_Txt_RepairValue => _UE_Txt_RepairValue ?? (_UE_Txt_RepairValue = PickText("Txt_RepairValue"));

	public TMP_Text UE_Txt_CollectValue => _UE_Txt_CollectValue ?? (_UE_Txt_CollectValue = PickText("Txt_CollectValue"));

	public TMP_Text UE_Txt_CurrentMoney => _UE_Txt_CurrentMoney ?? (_UE_Txt_CurrentMoney = PickText("Txt_CurrentMoney"));

	public void SetCurrentCurrencyMode()
	{
		UE_TargetCurrency.gameObject.SetActive(value: true);
		UE_RepairValue.enabled = false;
		UE_CollectValue.enabled = false;
		UE_Txt_RepairValue.enabled = false;
		UE_Txt_CollectValue.enabled = false;
		UE_CurrentMoney.enabled = true;
		UE_Txt_CurrentMoney.enabled = true;
	}

	public void SetCollectedCurrencyMode()
	{
		UE_TargetCurrency.gameObject.SetActive(value: true);
		UE_RepairValue.enabled = true;
		UE_CollectValue.enabled = true;
		UE_Txt_RepairValue.enabled = true;
		UE_Txt_CollectValue.enabled = true;
		UE_CurrentMoney.enabled = false;
		UE_Txt_CurrentMoney.enabled = false;
	}

	public void UpdateCurrentCurrency(int currentMoney)
	{
		UE_Txt_CurrentMoney.text = $"${currentMoney}";
	}

	public void UpdateCollectedCurrency(int stashToCurrency, int targetCurrency)
	{
		UE_Txt_RepairValue.text = $"${targetCurrency}";
		UE_Txt_CollectValue.text = $"${stashToCurrency}";
	}
}
