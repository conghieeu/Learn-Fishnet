using System;
using System.Collections.Generic;
using Mimic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_Inventory : UIPrefabScript
{
	public class Slot
	{
		public Image image;

		public TMP_Text stackCount;

		public Image frame;

		public Transform waitEvent;
	}

	public const string UEID_rootNode = "rootNode";

	public const string UEID_InvenFrame1 = "InvenFrame1";

	public const string UEID_InvenImage1 = "InvenImage1";

	public const string UEID_InvenWaitEvent1 = "InvenWaitEvent1";

	public const string UEID_InvenFrame2 = "InvenFrame2";

	public const string UEID_InvenImage2 = "InvenImage2";

	public const string UEID_InvenWaitEvent2 = "InvenWaitEvent2";

	public const string UEID_InvenFrame3 = "InvenFrame3";

	public const string UEID_InvenImage3 = "InvenImage3";

	public const string UEID_InvenWaitEvent3 = "InvenWaitEvent3";

	public const string UEID_InvenFrame4 = "InvenFrame4";

	public const string UEID_InvenImage4 = "InvenImage4";

	public const string UEID_InvenWaitEvent4 = "InvenWaitEvent4";

	public const string UEID_stackCount1 = "stackCount1";

	public const string UEID_stackCount2 = "stackCount2";

	public const string UEID_stackCount3 = "stackCount3";

	public const string UEID_stackCount4 = "stackCount4";

	public const string UEID_Weight = "Weight";

	private Image _UE_rootNode;

	private Image _UE_InvenFrame1;

	private Image _UE_InvenImage1;

	private Transform _UE_InvenWaitEvent1;

	private Image _UE_InvenFrame2;

	private Image _UE_InvenImage2;

	private Transform _UE_InvenWaitEvent2;

	private Image _UE_InvenFrame3;

	private Image _UE_InvenImage3;

	private Transform _UE_InvenWaitEvent3;

	private Image _UE_InvenFrame4;

	private Image _UE_InvenImage4;

	private Transform _UE_InvenWaitEvent4;

	private TMP_Text _UE_stackCount1;

	private TMP_Text _UE_stackCount2;

	private TMP_Text _UE_stackCount3;

	private TMP_Text _UE_stackCount4;

	private TMP_Text _UE_Weight;

	private List<Slot> inventorySlots = new List<Slot>();

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public Image UE_InvenFrame1 => _UE_InvenFrame1 ?? (_UE_InvenFrame1 = PickImage("InvenFrame1"));

	public Image UE_InvenImage1 => _UE_InvenImage1 ?? (_UE_InvenImage1 = PickImage("InvenImage1"));

	public Transform UE_InvenWaitEvent1 => _UE_InvenWaitEvent1 ?? (_UE_InvenWaitEvent1 = PickTransform("InvenWaitEvent1"));

	public Image UE_InvenFrame2 => _UE_InvenFrame2 ?? (_UE_InvenFrame2 = PickImage("InvenFrame2"));

	public Image UE_InvenImage2 => _UE_InvenImage2 ?? (_UE_InvenImage2 = PickImage("InvenImage2"));

	public Transform UE_InvenWaitEvent2 => _UE_InvenWaitEvent2 ?? (_UE_InvenWaitEvent2 = PickTransform("InvenWaitEvent2"));

	public Image UE_InvenFrame3 => _UE_InvenFrame3 ?? (_UE_InvenFrame3 = PickImage("InvenFrame3"));

	public Image UE_InvenImage3 => _UE_InvenImage3 ?? (_UE_InvenImage3 = PickImage("InvenImage3"));

	public Transform UE_InvenWaitEvent3 => _UE_InvenWaitEvent3 ?? (_UE_InvenWaitEvent3 = PickTransform("InvenWaitEvent3"));

	public Image UE_InvenFrame4 => _UE_InvenFrame4 ?? (_UE_InvenFrame4 = PickImage("InvenFrame4"));

	public Image UE_InvenImage4 => _UE_InvenImage4 ?? (_UE_InvenImage4 = PickImage("InvenImage4"));

	public Transform UE_InvenWaitEvent4 => _UE_InvenWaitEvent4 ?? (_UE_InvenWaitEvent4 = PickTransform("InvenWaitEvent4"));

	public TMP_Text UE_stackCount1 => _UE_stackCount1 ?? (_UE_stackCount1 = PickText("stackCount1"));

	public TMP_Text UE_stackCount2 => _UE_stackCount2 ?? (_UE_stackCount2 = PickText("stackCount2"));

	public TMP_Text UE_stackCount3 => _UE_stackCount3 ?? (_UE_stackCount3 = PickText("stackCount3"));

	public TMP_Text UE_stackCount4 => _UE_stackCount4 ?? (_UE_stackCount4 = PickText("stackCount4"));

	public TMP_Text UE_Weight => _UE_Weight ?? (_UE_Weight = PickText("Weight"));

	private new void Awake()
	{
		base.Awake();
		inventorySlots.Add(new Slot
		{
			image = UE_InvenImage1,
			stackCount = UE_stackCount1,
			frame = UE_InvenFrame1,
			waitEvent = UE_InvenWaitEvent1
		});
		inventorySlots.Add(new Slot
		{
			image = UE_InvenImage2,
			stackCount = UE_stackCount2,
			frame = UE_InvenFrame2,
			waitEvent = UE_InvenWaitEvent2
		});
		inventorySlots.Add(new Slot
		{
			image = UE_InvenImage3,
			stackCount = UE_stackCount3,
			frame = UE_InvenFrame3,
			waitEvent = UE_InvenWaitEvent3
		});
		inventorySlots.Add(new Slot
		{
			image = UE_InvenImage4,
			stackCount = UE_stackCount4,
			frame = UE_InvenFrame4,
			waitEvent = UE_InvenWaitEvent4
		});
	}

	public void UpdateSlot(in List<InventoryItem> inventoryItems, int currentInventorySlotIndex)
	{
		float num = 0f;
		for (int i = 0; i < Hub.s.gameConfig.playerActor.maxGenericInventorySlot; i++)
		{
			if (i > inventoryItems.Count)
			{
				Logger.RError("inventory slot count mismatch");
				return;
			}
			if (i >= inventorySlots.Count)
			{
				throw new IndexOutOfRangeException("inventory size mismatch");
			}
			Slot slot = inventorySlots[i];
			if (slot == null)
			{
				continue;
			}
			slot.frame.enabled = i == currentInventorySlotIndex;
			if (inventoryItems[i] != null)
			{
				InventoryItem inventoryItem = inventoryItems[i];
				num += (float)inventoryItem.MasterInfo.Weight;
				if (inventoryItem.StackCount != 0)
				{
					num += (float)((inventoryItem.StackCount - 1) * inventoryItem.MasterInfo.Weight);
				}
				if (inventoryItem.MasterInfo.VisibleGaugeCount)
				{
					int num2 = inventoryItem.StackCount;
					if (num2 == 0)
					{
						num2 = inventoryItem.RemainGauge;
					}
					slot.stackCount.text = num2.ToString();
				}
				else if (inventoryItem.MasterInfo.VisibleDurabilityCount)
				{
					int durability = inventoryItem.Durability;
					slot.stackCount.text = durability.ToString();
				}
				else
				{
					slot.stackCount.text = "";
				}
				MMLootingObjectTable.SkinnedItemInfo item = Hub.s.tableman.lootingObject.FindSkinnedItemInfo(inventoryItem.ItemMasterID).skinnedItemInfo;
				if (item == null)
				{
					Logger.RError($"skinnedItemInfo is null @ UpdateSlot: ItemMasterID={inventoryItem.ItemMasterID}");
					return;
				}
				slot.image.enabled = true;
				slot.image.sprite = Hub.s.tableman.iconSprite.GetSprite(item.iconSpriteId);
				slot.waitEvent.gameObject.SetActive(inventoryItem.IsWaitingEvent);
			}
			else
			{
				slot.stackCount.text = "";
				slot.image.enabled = false;
				slot.waitEvent.gameObject.SetActive(value: false);
			}
		}
		UE_Weight.SetText((num * 0.001f).ToString("n1") + " Kg");
	}
}
