using System;
using Bifrost.Cooked;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_SteamInventoryItem : UIPrefabScript, ITooltipTargetable, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public const string UEID_Button = "Button";

	public const string UEID_ItemIcon = "ItemIcon";

	public const string UEID_Checked = "Checked";

	private Button _UE_Button;

	private Action<string> _OnButton;

	private Image _UE_ItemIcon;

	private Image _UE_Checked;

	[Header("TMP Texts")]
	[SerializeField]
	private TMP_Text skinNameText;

	[SerializeField]
	private TMP_Text skinInfoText;

	[Header("Selection Colors")]
	[SerializeField]
	private Color normalColor = Color.white;

	[SerializeField]
	private Color highlightedColor = Color.white;

	[SerializeField]
	private Color selectedColor = Color.white;

	private string tabName;

	private int inventoryItemID;

	private bool isChecked;

	private bool isHovered;

	private Action<string, int, bool> onCheckUpdated;

	private string itemName;

	private string itemDescription;

	public Button UE_Button => _UE_Button ?? (_UE_Button = PickButton("Button"));

	public Action<string> OnButton
	{
		get
		{
			return _OnButton;
		}
		set
		{
			_OnButton = value;
			SetOnButtonClick("Button", value);
		}
	}

	public Image UE_ItemIcon => _UE_ItemIcon ?? (_UE_ItemIcon = PickImage("ItemIcon"));

	public Image UE_Checked => _UE_Checked ?? (_UE_Checked = PickImage("Checked"));

	protected override void OnShow()
	{
		base.OnShow();
		if (UE_Button != null)
		{
			UE_Button.onClick.RemoveAllListeners();
			UE_Button.onClick.AddListener(ToggleChecked);
			UE_Button.transition = Selectable.Transition.None;
		}
		ApplySelectionVisual();
	}

	public void ToggleChecked()
	{
		isChecked = !isChecked;
		ApplySelectionVisual();
		onCheckUpdated?.Invoke(tabName, inventoryItemID, isChecked);
	}

	public void SetSteamInventoryItemData(string tabName, int inventoryItemID, bool isChecked, Action<string, int, bool> onCheckUpdated)
	{
		this.tabName = tabName;
		this.inventoryItemID = inventoryItemID;
		this.isChecked = isChecked;
		this.onCheckUpdated = onCheckUpdated;
		if (Hub.s.dataman.ExcelDataManager.PromotionRewardDict.TryGetValue(inventoryItemID, out PromotionRewardInfo value))
		{
			if (UE_ItemIcon != null)
			{
				Logger.RLog("UIIconName=" + value.UIIconName + ", if you can't see, check MMIconTable.");
				UE_ItemIcon.sprite = Hub.s.tableman.iconSprite.GetSprite(value.UIIconName);
			}
			itemDescription = Hub.GetL10NText(value.MouseOverTooltip);
			if (skinNameText != null && value.RewardName != null)
			{
				skinNameText.text = Hub.GetL10NText(value.RewardName);
			}
			if (skinInfoText != null)
			{
				skinInfoText.text = Hub.GetL10NText(value.MouseOverTooltip);
			}
		}
		ApplySelectionVisual();
	}

	public void SetChecked(bool isChecked)
	{
		this.isChecked = isChecked;
		ApplySelectionVisual();
	}

	private void ApplySelectionVisual()
	{
		if (UE_Checked != null)
		{
			UE_Checked.gameObject.SetActive(isChecked);
		}
		Color targetColor = (isChecked ? selectedColor : ((!isHovered) ? normalColor : highlightedColor));
		if (UE_ItemIcon != null)
		{
			UE_ItemIcon.CrossFadeColor(targetColor, 0.1f, ignoreTimeScale: true, useAlpha: true);
		}
		if (UE_Button != null && UE_Button.targetGraphic != null)
		{
			UE_Button.targetGraphic.CrossFadeColor(targetColor, 0.1f, ignoreTimeScale: true, useAlpha: true);
		}
		if (skinNameText != null)
		{
			skinNameText.CrossFadeColor(targetColor, 0.1f, ignoreTimeScale: true, useAlpha: true);
		}
		if (skinInfoText != null)
		{
			skinInfoText.CrossFadeColor(targetColor, 0.1f, ignoreTimeScale: true, useAlpha: true);
		}
	}

	public string GetTooltipText()
	{
		return itemDescription;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHovered = true;
		ApplySelectionVisual();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovered = false;
		ApplySelectionVisual();
	}
}
