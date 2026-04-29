using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Cooked;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_SteamInventory : UIPrefabScript
{
	public const string UEID_ToggleGroup = "ToggleGroup";

	public const string UEID_ToggleButton1 = "ToggleButton1";

	public const string UEID_ToggleButton2 = "ToggleButton2";

	public const string UEID_ToggleButton3 = "ToggleButton3";

	public const string UEID_EmptyListText = "EmptyListText";

	public const string UEID_Scroll_View = "Scroll_View";

	public const string UEID_Content = "Content";

	public const string UEID_Scroll_View_ForTramSkin = "Scroll_View_ForTramSkin";

	public const string UEID_ContentForTramSkin = "ContentForTramSkin";

	public const string UEID_ButtonClose = "ButtonClose";

	private Transform _UE_ToggleGroup;

	private Image _UE_ToggleButton1;

	private Image _UE_ToggleButton2;

	private Image _UE_ToggleButton3;

	private TMP_Text _UE_EmptyListText;

	private Image _UE_Scroll_View;

	private Transform _UE_Content;

	private Image _UE_Scroll_View_ForTramSkin;

	private Transform _UE_ContentForTramSkin;

	private Button _UE_ButtonClose;

	private Action<string> _OnButtonClose;

	[SerializeField]
	public GameObject prefab_SteamInventoryItem;

	[SerializeField]
	public GameObject prefab_SteamInventoryItemForTramSkin;

	public List<int> CurrentAppliedItemDefIds = new List<int>();

	public int CurrentSelectedTramSkinDefID;

	public Dictionary<int, int> CurrentSelectedItemSkinDefIDs = new Dictionary<int, int>();

	public List<int> CurrentSelectedStartItemDefIDs = new List<int>();

	public Dictionary<int, UIPrefab_SteamInventoryItem> CurrentInventoryItemDict = new Dictionary<int, UIPrefab_SteamInventoryItem>();

	public Transform UE_ToggleGroup => _UE_ToggleGroup ?? (_UE_ToggleGroup = PickTransform("ToggleGroup"));

	public Image UE_ToggleButton1 => _UE_ToggleButton1 ?? (_UE_ToggleButton1 = PickImage("ToggleButton1"));

	public Image UE_ToggleButton2 => _UE_ToggleButton2 ?? (_UE_ToggleButton2 = PickImage("ToggleButton2"));

	public Image UE_ToggleButton3 => _UE_ToggleButton3 ?? (_UE_ToggleButton3 = PickImage("ToggleButton3"));

	public TMP_Text UE_EmptyListText => _UE_EmptyListText ?? (_UE_EmptyListText = PickText("EmptyListText"));

	public Image UE_Scroll_View => _UE_Scroll_View ?? (_UE_Scroll_View = PickImage("Scroll_View"));

	public Transform UE_Content => _UE_Content ?? (_UE_Content = PickTransform("Content"));

	public Image UE_Scroll_View_ForTramSkin => _UE_Scroll_View_ForTramSkin ?? (_UE_Scroll_View_ForTramSkin = PickImage("Scroll_View_ForTramSkin"));

	public Transform UE_ContentForTramSkin => _UE_ContentForTramSkin ?? (_UE_ContentForTramSkin = PickTransform("ContentForTramSkin"));

	public Button UE_ButtonClose => _UE_ButtonClose ?? (_UE_ButtonClose = PickButton("ButtonClose"));

	public Action<string> OnButtonClose
	{
		get
		{
			return _OnButtonClose;
		}
		set
		{
			_OnButtonClose = value;
			SetOnButtonClick("ButtonClose", value);
		}
	}

	private void Start()
	{
		AddMouseOverEnterEvent(UE_ToggleButton1.gameObject, UE_ToggleButton1.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_ToggleButton2.gameObject, UE_ToggleButton2.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_ToggleButton3.gameObject, UE_ToggleButton3.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_ButtonClose.gameObject, UE_ButtonClose.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ToggleButton1.gameObject, UE_ToggleButton1.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ToggleButton2.gameObject, UE_ToggleButton2.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ToggleButton3.gameObject, UE_ToggleButton3.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ButtonClose.gameObject, UE_ButtonClose.GetComponentInChildren<TMP_Text>());
	}

	protected override void OnShow()
	{
		base.OnShow();
		UE_ButtonClose.GetComponentInChildren<TMP_Text>().color = Color.white;
		if (UE_ToggleButton1 != null)
		{
			UE_ToggleButton1.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleButtonValueChanged("Tab1", isOn);
			});
		}
		if (UE_ToggleButton2 != null)
		{
			UE_ToggleButton2.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleButtonValueChanged("Tab2", isOn);
			});
		}
		if (UE_ToggleButton3 != null)
		{
			UE_ToggleButton3.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleButtonValueChanged("Tab3", isOn);
			});
		}
		UE_ButtonClose.onClick.AddListener(delegate
		{
			Hub.s.uiman.CloseSteamInventory();
		});
		if (UE_EmptyListText != null)
		{
			UE_EmptyListText.gameObject.SetActive(value: false);
		}
		LoadPlayerData();
		if (Hub.s.pdata.LastSelectedTabIndex != -1)
		{
			string tabName = "Tab" + (Hub.s.pdata.LastSelectedTabIndex + 1);
			StartCoroutine(SelectToggleTab(tabName));
		}
		else
		{
			StartCoroutine(SelectToggleTab("Tab1"));
		}
	}

	protected override void OnHide()
	{
		base.OnHide();
		ApplyPlayerData();
		if (UE_ToggleButton1 != null)
		{
			UE_ToggleButton1.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		}
		if (UE_ToggleButton2 != null)
		{
			UE_ToggleButton2.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		}
		if (UE_ToggleButton3 != null)
		{
			UE_ToggleButton3.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		}
		UE_ButtonClose.onClick.RemoveAllListeners();
		CurrentAppliedItemDefIds.Clear();
		CurrentSelectedTramSkinDefID = 0;
		CurrentSelectedItemSkinDefIDs.Clear();
		CurrentSelectedStartItemDefIDs.Clear();
		CurrentInventoryItemDict.Clear();
		Hub.s.tooltipManager.ResetState();
	}

	private IEnumerator SelectToggleTab(string tabName)
	{
		yield return null;
		OnToggleButtonValueChanged(tabName, isOn: true);
		if (!(EventSystem.current != null))
		{
			yield break;
		}
		switch (tabName)
		{
		case "Tab1":
			if (UE_ToggleButton1 != null)
			{
				EventSystem.current.SetSelectedGameObject(UE_ToggleButton1.gameObject);
				UE_ToggleButton1.GetComponent<Toggle>().OnSelect(null);
			}
			break;
		case "Tab2":
			if (UE_ToggleButton2 != null)
			{
				EventSystem.current.SetSelectedGameObject(UE_ToggleButton2.gameObject);
				UE_ToggleButton2.GetComponent<Toggle>().OnSelect(null);
			}
			break;
		case "Tab3":
			if (UE_ToggleButton3 != null)
			{
				EventSystem.current.SetSelectedGameObject(UE_ToggleButton3.gameObject);
				UE_ToggleButton3.GetComponent<Toggle>().OnSelect(null);
			}
			break;
		}
	}

	private void OnToggleButtonValueChanged(string tabName, bool isOn)
	{
		if (isOn)
		{
			switch (tabName)
			{
			case "Tab1":
				UE_ToggleButton1.GetComponent<Toggle>().isOn = true;
				UE_ToggleButton2.GetComponent<Toggle>().isOn = false;
				UE_ToggleButton3.GetComponent<Toggle>().isOn = false;
				InitializeTab1();
				break;
			case "Tab2":
				UE_ToggleButton2.GetComponent<Toggle>().isOn = true;
				UE_ToggleButton1.GetComponent<Toggle>().isOn = false;
				UE_ToggleButton3.GetComponent<Toggle>().isOn = false;
				InitializeTab2();
				break;
			case "Tab3":
				UE_ToggleButton3.GetComponent<Toggle>().isOn = true;
				UE_ToggleButton1.GetComponent<Toggle>().isOn = false;
				UE_ToggleButton2.GetComponent<Toggle>().isOn = false;
				InitializeTab3();
				break;
			}
		}
	}

	private void ClearContent()
	{
		CurrentInventoryItemDict.Clear();
		foreach (Transform item in UE_Content)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in UE_ContentForTramSkin)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		UE_Scroll_View.gameObject.SetActive(value: false);
		UE_Scroll_View_ForTramSkin.gameObject.SetActive(value: false);
	}

	private void ApplyPlayerData()
	{
		string value = string.Join(",", CurrentAppliedItemDefIds.Select((int itemID) => itemID.ToString()));
		PlayerPrefs.SetString("AppliedSteamInventoryItemDefIDs", value);
		PlayerPrefs.Save();
	}

	private void LoadPlayerData()
	{
		string text = PlayerPrefs.GetString("AppliedSteamInventoryItemDefIDs");
		Logger.RLog("appliedSteamInventoryItemDefIDs=" + text);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		CurrentAppliedItemDefIds = text.Split(',').Select(int.Parse).ToList();
		List<int> notAppliedItemDefIDs = new List<int>();
		foreach (int currentAppliedItemDefId in CurrentAppliedItemDefIds)
		{
			PromotionRewardInfo value;
			if (!Hub.s.pdata.UserSteamInventoryItemDefIDs.Contains(currentAppliedItemDefId))
			{
				notAppliedItemDefIDs.Add(currentAppliedItemDefId);
			}
			else if (Hub.s.dataman.ExcelDataManager.PromotionRewardDictByItemDefId.TryGetValue(currentAppliedItemDefId, out value))
			{
				if (value.IsTramSkin())
				{
					CurrentSelectedTramSkinDefID = currentAppliedItemDefId;
				}
				if (value.IsItemSkin())
				{
					CurrentSelectedItemSkinDefIDs.Add(value.ChangeItemPrefab.Value.itemMasterId, currentAppliedItemDefId);
				}
				if (value.IsStartItem())
				{
					CurrentSelectedStartItemDefIDs.Add(currentAppliedItemDefId);
				}
			}
			else
			{
				notAppliedItemDefIDs.Add(currentAppliedItemDefId);
			}
		}
		CurrentAppliedItemDefIds.RemoveAll((int itemDefID) => notAppliedItemDefIDs.Contains(itemDefID));
	}

	private void InitializeTab1()
	{
		ClearContent();
		UE_Scroll_View_ForTramSkin.gameObject.SetActive(value: true);
		foreach (int userSteamInventoryItemDefID in Hub.s.pdata.UserSteamInventoryItemDefIDs)
		{
			if (Hub.s.dataman.ExcelDataManager.PromotionRewardDictByItemDefId.TryGetValue(userSteamInventoryItemDefID, out PromotionRewardInfo value) && value.IsTramSkin())
			{
				GameObject obj = UnityEngine.Object.Instantiate(prefab_SteamInventoryItemForTramSkin, UE_ContentForTramSkin);
				bool isChecked = false;
				if (CurrentAppliedItemDefIds.Contains(userSteamInventoryItemDefID))
				{
					isChecked = true;
				}
				UIPrefab_SteamInventoryItem component = obj.GetComponent<UIPrefab_SteamInventoryItem>();
				component.SetSteamInventoryItemData("Tab1", userSteamInventoryItemDefID, isChecked, ItemCheckUpdated);
				component.Show();
				CurrentInventoryItemDict.Add(userSteamInventoryItemDefID, component);
			}
		}
		if (CurrentInventoryItemDict.Count == 0)
		{
			if (UE_EmptyListText != null)
			{
				UE_EmptyListText.gameObject.SetActive(value: true);
				UE_EmptyListText.text = Hub.GetL10NText("STRING_NO_TRAM_SKIN");
			}
		}
		else if (UE_EmptyListText != null)
		{
			UE_EmptyListText.gameObject.SetActive(value: false);
		}
		Hub.s.pdata.LastSelectedTabIndex = 0;
	}

	private void InitializeTab2()
	{
		ClearContent();
		UE_Scroll_View.gameObject.SetActive(value: true);
		foreach (int userSteamInventoryItemDefID in Hub.s.pdata.UserSteamInventoryItemDefIDs)
		{
			if (Hub.s.dataman.ExcelDataManager.PromotionRewardDictByItemDefId.TryGetValue(userSteamInventoryItemDefID, out PromotionRewardInfo value) && value.IsItemSkin())
			{
				GameObject obj = UnityEngine.Object.Instantiate(prefab_SteamInventoryItem, UE_Content);
				bool isChecked = false;
				if (CurrentAppliedItemDefIds.Contains(userSteamInventoryItemDefID))
				{
					isChecked = true;
				}
				UIPrefab_SteamInventoryItem component = obj.GetComponent<UIPrefab_SteamInventoryItem>();
				component.SetSteamInventoryItemData("Tab2", userSteamInventoryItemDefID, isChecked, ItemCheckUpdated);
				component.Show();
				CurrentInventoryItemDict.Add(userSteamInventoryItemDefID, component);
			}
		}
		if (CurrentInventoryItemDict.Count == 0)
		{
			if (UE_EmptyListText != null)
			{
				UE_EmptyListText.gameObject.SetActive(value: true);
				UE_EmptyListText.text = Hub.GetL10NText("STRING_NO_ITEM_SKIN");
			}
		}
		else if (UE_EmptyListText != null)
		{
			UE_EmptyListText.gameObject.SetActive(value: false);
		}
		Hub.s.pdata.LastSelectedTabIndex = 1;
	}

	private void InitializeTab3()
	{
		ClearContent();
		UE_Scroll_View.gameObject.SetActive(value: true);
		foreach (int userSteamInventoryItemDefID in Hub.s.pdata.UserSteamInventoryItemDefIDs)
		{
			if (Hub.s.dataman.ExcelDataManager.PromotionRewardDictByItemDefId.TryGetValue(userSteamInventoryItemDefID, out PromotionRewardInfo value) && value.IsStartItem())
			{
				GameObject obj = UnityEngine.Object.Instantiate(prefab_SteamInventoryItem, UE_Content);
				bool isChecked = false;
				if (CurrentAppliedItemDefIds.Contains(userSteamInventoryItemDefID))
				{
					isChecked = true;
				}
				UIPrefab_SteamInventoryItem component = obj.GetComponent<UIPrefab_SteamInventoryItem>();
				component.SetSteamInventoryItemData("Tab3", userSteamInventoryItemDefID, isChecked, ItemCheckUpdated);
				component.Show();
				CurrentInventoryItemDict.Add(userSteamInventoryItemDefID, component);
			}
		}
		if (CurrentInventoryItemDict.Count == 0)
		{
			if (UE_EmptyListText != null)
			{
				UE_EmptyListText.gameObject.SetActive(value: true);
				UE_EmptyListText.text = Hub.GetL10NText("STRING_NO_START_ITEM");
			}
		}
		else if (UE_EmptyListText != null)
		{
			UE_EmptyListText.gameObject.SetActive(value: false);
		}
		Hub.s.pdata.LastSelectedTabIndex = 2;
	}

	private void ItemCheckUpdated(string tabName, int itemDefID, bool isChecked)
	{
		Logger.RLog($"ItemCheckUpdated: tabName={tabName}, itemDefID={itemDefID}, isChecked={isChecked}");
		if (isChecked)
		{
			switch (tabName)
			{
			case "Tab1":
			{
				CurrentAppliedItemDefIds.Remove(CurrentSelectedTramSkinDefID);
				if (CurrentInventoryItemDict.TryGetValue(CurrentSelectedTramSkinDefID, out var value2))
				{
					value2.SetChecked(isChecked: false);
				}
				CurrentSelectedTramSkinDefID = itemDefID;
				CurrentAppliedItemDefIds.Add(CurrentSelectedTramSkinDefID);
				if (CurrentInventoryItemDict.TryGetValue(CurrentSelectedTramSkinDefID, out value2))
				{
					value2.SetChecked(isChecked: true);
				}
				break;
			}
			case "Tab2":
			{
				if (!Hub.s.dataman.ExcelDataManager.PromotionRewardDictByItemDefId.TryGetValue(itemDefID, out PromotionRewardInfo value3))
				{
					break;
				}
				int item = value3.ChangeItemPrefab.Value.itemMasterId;
				_ = value3.ChangeItemPrefab.Value;
				if (CurrentSelectedItemSkinDefIDs.TryGetValue(item, out var value4))
				{
					CurrentSelectedItemSkinDefIDs.Remove(item);
					CurrentSelectedItemSkinDefIDs.Add(item, itemDefID);
					CurrentAppliedItemDefIds.Remove(value4);
					if (CurrentInventoryItemDict.TryGetValue(value4, out var value5))
					{
						value5.SetChecked(isChecked: false);
					}
					CurrentAppliedItemDefIds.Add(itemDefID);
					if (CurrentInventoryItemDict.TryGetValue(itemDefID, out value5))
					{
						value5.SetChecked(isChecked: true);
					}
				}
				else
				{
					CurrentSelectedItemSkinDefIDs.Add(item, itemDefID);
					CurrentAppliedItemDefIds.Add(itemDefID);
					if (CurrentInventoryItemDict.TryGetValue(itemDefID, out var value6))
					{
						value6.SetChecked(isChecked: true);
					}
				}
				break;
			}
			case "Tab3":
			{
				CurrentSelectedStartItemDefIDs.Add(itemDefID);
				CurrentAppliedItemDefIds.Add(itemDefID);
				if (CurrentInventoryItemDict.TryGetValue(itemDefID, out var value))
				{
					value.SetChecked(isChecked: true);
				}
				break;
			}
			}
			return;
		}
		switch (tabName)
		{
		case "Tab1":
		{
			if (CurrentInventoryItemDict.TryGetValue(CurrentSelectedTramSkinDefID, out var value8))
			{
				value8.SetChecked(isChecked: false);
			}
			CurrentAppliedItemDefIds.Remove(CurrentSelectedTramSkinDefID);
			CurrentSelectedTramSkinDefID = 0;
			break;
		}
		case "Tab2":
		{
			if (!Hub.s.dataman.ExcelDataManager.PromotionRewardDictByItemDefId.TryGetValue(itemDefID, out PromotionRewardInfo value9))
			{
				break;
			}
			int item2 = value9.ChangeItemPrefab.Value.itemMasterId;
			_ = value9.ChangeItemPrefab.Value;
			if (CurrentSelectedItemSkinDefIDs.TryGetValue(item2, out var value10))
			{
				CurrentSelectedItemSkinDefIDs.Remove(item2);
				CurrentAppliedItemDefIds.Remove(value10);
				if (CurrentInventoryItemDict.TryGetValue(itemDefID, out var value11))
				{
					value11.SetChecked(isChecked: false);
				}
			}
			break;
		}
		case "Tab3":
		{
			CurrentSelectedStartItemDefIDs.Remove(itemDefID);
			CurrentAppliedItemDefIds.Remove(itemDefID);
			if (CurrentInventoryItemDict.TryGetValue(itemDefID, out var value7))
			{
				value7.SetChecked(isChecked: false);
			}
			break;
		}
		}
	}
}
