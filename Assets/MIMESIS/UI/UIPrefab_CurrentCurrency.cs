using TMPro;

public class UIPrefab_CurrentCurrency : UIPrefabScript
{
	public const string UEID_CurrentMoney = "CurrentMoney";

	public const string UEID_Txt_CurrentMoney = "Txt_CurrentMoney";

	private TMP_Text _UE_CurrentMoney;

	private TMP_Text _UE_Txt_CurrentMoney;

	public TMP_Text UE_CurrentMoney => _UE_CurrentMoney ?? (_UE_CurrentMoney = PickText("CurrentMoney"));

	public TMP_Text UE_Txt_CurrentMoney => _UE_Txt_CurrentMoney ?? (_UE_Txt_CurrentMoney = PickText("Txt_CurrentMoney"));

	protected override void OnShow()
	{
		base.OnShow();
		UE_CurrentMoney.enabled = true;
		UE_Txt_CurrentMoney.enabled = true;
	}

	public void UpdateCurrentCurrency(int currentMoney)
	{
		UE_Txt_CurrentMoney.text = $"${currentMoney}";
	}
}
