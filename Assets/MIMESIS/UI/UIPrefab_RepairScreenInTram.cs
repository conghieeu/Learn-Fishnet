using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_RepairScreenInTram : UIPrefabScript
{
	public const string UEID_LeftDisplay = "LeftDisplay";

	public const string UEID_LeftText = "LeftText";

	public const string UEID_RightDisplay = "RightDisplay";

	public const string UEID_RightText = "RightText";

	private Image _UE_LeftDisplay;

	private TMP_Text _UE_LeftText;

	private Image _UE_RightDisplay;

	private TMP_Text _UE_RightText;

	[SerializeField]
	private string leftTextKeyBeforeRepair;

	[SerializeField]
	private string rightTextKeyBeforeRepair;

	[SerializeField]
	private string leftTextKeyAfterRepair;

	[SerializeField]
	private string rightTextKeyAfterRepair;

	[SerializeField]
	private Color warningTextColor1;

	[SerializeField]
	private Color warningTextColor2;

	[SerializeField]
	private Color reapiredTextColor1;

	[SerializeField]
	private Color reapiredTextColor2;

	[SerializeField]
	private Color warningDisplayColor;

	[SerializeField]
	private Color repairedDisplayColor;

	private bool repaired;

	private bool repairing;

	public Image UE_LeftDisplay => _UE_LeftDisplay ?? (_UE_LeftDisplay = PickImage("LeftDisplay"));

	public TMP_Text UE_LeftText => _UE_LeftText ?? (_UE_LeftText = PickText("LeftText"));

	public Image UE_RightDisplay => _UE_RightDisplay ?? (_UE_RightDisplay = PickImage("RightDisplay"));

	public TMP_Text UE_RightText => _UE_RightText ?? (_UE_RightText = PickText("RightText"));

	private void Start()
	{
		if (UE_LeftDisplay != null)
		{
			UE_LeftDisplay.color = Color.white;
		}
		if (UE_RightDisplay != null)
		{
			UE_RightDisplay.color = Color.white;
		}
		if (UE_LeftText != null)
		{
			UE_LeftText.SetText("");
		}
		if (UE_RightText != null)
		{
			UE_RightText.SetText("");
		}
	}

	private void OnEnable()
	{
		Hub.s.lcman.onLanguageChanged += OnLanguageChanged;
	}

	private void OnDisable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged -= OnLanguageChanged;
		}
	}

	public void OnLanguageChanged()
	{
		UpdateRepairState(repaired, repairing);
	}

	public void UpdateRepairState(bool isRepaired, bool isRepairing)
	{
		repaired = isRepaired;
		repairing = isRepairing;
		if (isRepairing)
		{
			if (UE_LeftDisplay != null)
			{
				UE_LeftDisplay.color = repairedDisplayColor;
			}
			if (UE_RightDisplay != null)
			{
				UE_RightDisplay.color = repairedDisplayColor;
			}
			if (UE_LeftText != null)
			{
				UE_LeftText.SetText(Hub.GetL10NText(leftTextKeyAfterRepair));
				UE_LeftText.colorGradient = new VertexGradient(reapiredTextColor1, reapiredTextColor1, reapiredTextColor2, reapiredTextColor2);
			}
			if (UE_RightText != null)
			{
				UE_RightText.SetText(Hub.GetL10NText(rightTextKeyAfterRepair));
				UE_RightText.colorGradient = new VertexGradient(reapiredTextColor1, reapiredTextColor1, reapiredTextColor2, reapiredTextColor2);
			}
		}
		else if (isRepaired)
		{
			if (UE_LeftDisplay != null)
			{
				UE_LeftDisplay.color = repairedDisplayColor;
			}
			if (UE_RightDisplay != null)
			{
				UE_RightDisplay.color = repairedDisplayColor;
			}
			if (UE_LeftText != null)
			{
				UE_LeftText.SetText(Hub.GetL10NText(leftTextKeyAfterRepair));
				UE_LeftText.colorGradient = new VertexGradient(reapiredTextColor1, reapiredTextColor1, reapiredTextColor2, reapiredTextColor2);
			}
			if (UE_RightText != null)
			{
				UE_RightText.SetText(Hub.GetL10NText(rightTextKeyAfterRepair));
				UE_RightText.colorGradient = new VertexGradient(reapiredTextColor1, reapiredTextColor1, reapiredTextColor2, reapiredTextColor2);
			}
		}
		else
		{
			if (UE_LeftDisplay != null)
			{
				UE_LeftDisplay.color = warningDisplayColor;
			}
			if (UE_RightDisplay != null)
			{
				UE_RightDisplay.color = warningDisplayColor;
			}
			if (UE_LeftText != null)
			{
				UE_LeftText.SetText(Hub.GetL10NText(leftTextKeyBeforeRepair));
				UE_LeftText.colorGradient = new VertexGradient(warningTextColor1, warningTextColor1, warningTextColor2, warningTextColor2);
			}
			if (UE_RightText != null)
			{
				UE_RightText.SetText(Hub.GetL10NText(rightTextKeyBeforeRepair));
				UE_RightText.colorGradient = new VertexGradient(warningTextColor1, warningTextColor1, warningTextColor2, warningTextColor2);
			}
		}
	}
}
