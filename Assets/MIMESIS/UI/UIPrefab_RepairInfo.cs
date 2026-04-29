using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_RepairInfo : UIPrefabScript
{
	public const string UEID_BG = "BG";

	public const string UEID_CurrencyInfo = "CurrencyInfo";

	public const string UEID_CurrentMoney = "CurrentMoney";

	public const string UEID_Slash = "Slash";

	public const string UEID_RepairMoney = "RepairMoney";

	public const string UEID_RepairInfo = "RepairInfo";

	public const string UEID_RepairInfoText = "RepairInfoText";

	private Image _UE_BG;

	private Transform _UE_CurrencyInfo;

	private TMP_Text _UE_CurrentMoney;

	private TMP_Text _UE_Slash;

	private TMP_Text _UE_RepairMoney;

	private Transform _UE_RepairInfo;

	private TMP_Text _UE_RepairInfoText;

	[SerializeField]
	public string keyRepairingText;

	[SerializeField]
	public string keyRepairCompleteText;

	[SerializeField]
	public Color colorCannotRepair;

	[SerializeField]
	public Color colorCanRepair;

	[SerializeField]
	public Color textColorCannotRepair_1;

	[SerializeField]
	public Color textColorCannotRepair_2;

	[SerializeField]
	public Color textColorCanRepair_1;

	[SerializeField]
	public Color textColorCanRepair_2;

	private VertexGradient gradientTextColorCannotRepair;

	private VertexGradient gradientTextColorCanRepair;

	private string tempRepairInfoStringKey = "";

	public Image UE_BG => _UE_BG ?? (_UE_BG = PickImage("BG"));

	public Transform UE_CurrencyInfo => _UE_CurrencyInfo ?? (_UE_CurrencyInfo = PickTransform("CurrencyInfo"));

	public TMP_Text UE_CurrentMoney => _UE_CurrentMoney ?? (_UE_CurrentMoney = PickText("CurrentMoney"));

	public TMP_Text UE_Slash => _UE_Slash ?? (_UE_Slash = PickText("Slash"));

	public TMP_Text UE_RepairMoney => _UE_RepairMoney ?? (_UE_RepairMoney = PickText("RepairMoney"));

	public Transform UE_RepairInfo => _UE_RepairInfo ?? (_UE_RepairInfo = PickTransform("RepairInfo"));

	public TMP_Text UE_RepairInfoText => _UE_RepairInfoText ?? (_UE_RepairInfoText = PickText("RepairInfoText"));

	private void Start()
	{
		gradientTextColorCannotRepair = new VertexGradient(textColorCannotRepair_1, textColorCannotRepair_1, textColorCannotRepair_2, textColorCannotRepair_2);
		gradientTextColorCanRepair = new VertexGradient(textColorCanRepair_1, textColorCanRepair_1, textColorCanRepair_2, textColorCanRepair_2);
	}

	private void OnEnable()
	{
		if (Hub.s != null && Hub.s.lcman != null)
		{
			Hub.s.lcman.onLanguageChanged += OnLanguageChanged;
		}
	}

	private void OnDisable()
	{
		if (Hub.s != null && Hub.s.lcman != null)
		{
			Hub.s.lcman.onLanguageChanged -= OnLanguageChanged;
		}
	}

	public void SetCurrencyInfoMode()
	{
		UE_CurrencyInfo.gameObject.SetActive(value: true);
		UE_RepairInfo.gameObject.SetActive(value: false);
	}

	private void SetRepairInfoMode()
	{
		UE_CurrencyInfo.gameObject.SetActive(value: false);
		UE_RepairInfo.gameObject.SetActive(value: true);
		UE_RepairInfoText.colorGradient = gradientTextColorCanRepair;
	}

	public void OnCurrencyChanged(int current, int target)
	{
		UE_CurrentMoney.SetText($"${current}");
		UE_RepairMoney.SetText($"${target}");
		UE_BG.color = ((current < target) ? colorCannotRepair : colorCanRepair);
		UE_CurrentMoney.colorGradient = ((current < target) ? gradientTextColorCannotRepair : gradientTextColorCanRepair);
		UE_Slash.colorGradient = ((current < target) ? gradientTextColorCannotRepair : gradientTextColorCanRepair);
		UE_RepairMoney.colorGradient = ((current < target) ? gradientTextColorCannotRepair : gradientTextColorCanRepair);
		MaintenanceScene maintenanceScene = Hub.s.pdata.main as MaintenanceScene;
		if (maintenanceScene.isRepairing)
		{
			UE_BG.color = colorCanRepair;
		}
		else if (maintenanceScene.isRepaired)
		{
			UE_BG.color = colorCanRepair;
		}
	}

	private void OnLanguageChanged()
	{
		UE_RepairInfoText.SetText(Hub.GetL10NText(tempRepairInfoStringKey ?? ""));
	}

	public void OnStartRepair()
	{
		SetRepairInfoMode();
		UE_RepairInfoText.SetText(Hub.GetL10NText(keyRepairingText ?? ""));
		tempRepairInfoStringKey = keyRepairingText;
	}

	public void OnRepairCompleted()
	{
		SetRepairInfoMode();
		UE_RepairInfoText.SetText(Hub.GetL10NText(keyRepairCompleteText ?? ""));
		tempRepairInfoStringKey = keyRepairCompleteText;
		UE_BG.color = colorCanRepair;
	}
}
