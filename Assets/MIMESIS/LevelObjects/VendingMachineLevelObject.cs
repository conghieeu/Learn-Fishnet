using System.Collections;
using System.Text;
using Bifrost.Cooked;
using Mimic.Actors;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class VendingMachineLevelObject : SwitchLevelObject
{
	public int itemMasterID;

	public TextMeshPro debugText;

	private int price;

	[SerializeField]
	private MMF_Player feedbackEffect;

	[SerializeField]
	private float resellWatingTime = 1f;

	private bool isResellWaiting;

	[SerializeField]
	private Transform vendingMachineVanillaRoot;

	[SerializeField]
	private Transform vendingMachineItemRoot;

	private GameObject? vendingMachineItem;

	private void Start()
	{
		SetPanel();
		base.crossHairType = CrosshairType.VendingMachine;
	}

	private void OnEnable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged += SetPanel;
		}
	}

	private void OnDisable()
	{
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged -= SetPanel;
		}
	}

	private void SetPanel()
	{
		StartCoroutine(SetVendingMachineItemProcess());
		StartCoroutine(SetPanelProcess());
	}

	private IEnumerator SetPanelProcess()
	{
		yield return new WaitUntil(() => Hub.s != null && Hub.s.pdata.itemPrices != null);
		yield return new WaitUntil(() => Hub.s.pdata.itemPrices.ContainsKey(itemMasterID));
		ItemMasterInfo itemMasterInfo = ProtoActor.Inventory.GetItemMasterInfo(itemMasterID);
		if (Hub.s.pdata.itemPrices.TryGetValue(itemMasterID, out var value))
		{
			price = value;
			debugText.text = Hub.GetL10NText(itemMasterInfo.Name) + "\n$" + value;
		}
	}

	private IEnumerator SetVendingMachineItemProcess()
	{
		yield return new WaitUntil(() => Hub.s != null && Hub.s.pdata.main != null);
		if (vendingMachineItemRoot != null)
		{
			if (vendingMachineItem != null)
			{
				Object.DestroyImmediate(vendingMachineItem);
				vendingMachineItem = null;
			}
			(bool isSkinned, GameObject spawnedItemObject) tuple = Hub.s.pdata.main.TrySpawnItemObject(itemMasterID, vendingMachineItemRoot);
			bool item = tuple.isSkinned;
			GameObject item2 = tuple.spawnedItemObject;
			vendingMachineItem = item2;
			if (item)
			{
				vendingMachineVanillaRoot.gameObject.SetActive(value: false);
				yield break;
			}
			vendingMachineVanillaRoot.gameObject.SetActive(value: true);
			Object.DestroyImmediate(vendingMachineItem);
			vendingMachineItem = null;
		}
	}

	public void Refresh()
	{
		SetPanel();
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (Time.time - lastTransitionTime >= minTransitionInterval && price <= Hub.s.pdata.main.CurrentCurrency)
		{
			return !isResellWaiting;
		}
		return false;
	}

	protected override void Trigger(ProtoActor protoActor, int newState)
	{
		_ = base.State;
		protoActor.BuyItemByMasterId(itemMasterID, levelObjectID, delegate
		{
			OnBuyItem();
		});
		isResellWaiting = true;
		StartCoroutine(ResellWatingCoroutine());
	}

	private IEnumerator ResellWatingCoroutine()
	{
		yield return new WaitForSeconds(resellWatingTime);
		isResellWaiting = false;
	}

	public void OnBuyItem()
	{
		PlayTriggerSound(0, 0);
		if (feedbackEffect != null)
		{
			feedbackEffect.PlayFeedbacks();
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		ItemMasterInfo itemMasterInfo = ProtoActor.Inventory.GetItemMasterInfo(itemMasterID);
		if (itemMasterInfo == null)
		{
			return "";
		}
		string value = (isResellWaiting ? Hub.GetL10NText("VENDING_MACHINE_IN_PURCHASE") : ((price > Hub.s.pdata.main.CurrentCurrency) ? Hub.GetL10NText("VENDING_MACHINE_CANT_PURCHASE") : Hub.GetL10NText("STRING_ITEM_PURCHASE").Replace("[itemname:]", Hub.GetL10NText(itemMasterInfo.Name))));
		string value2 = string.Empty;
		if (!string.IsNullOrWhiteSpace(itemMasterInfo.VendingMachineTooltip))
		{
			value2 = Hub.GetL10NText(itemMasterInfo.VendingMachineTooltip);
		}
		string value3 = $"${price.ToString(),5}";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(value);
		if (!string.IsNullOrWhiteSpace(value2))
		{
			stringBuilder.AppendLine(value2);
		}
		stringBuilder.AppendLine(value3);
		return stringBuilder.ToString();
	}
}
