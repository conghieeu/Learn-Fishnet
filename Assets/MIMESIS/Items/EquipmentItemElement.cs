using System;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;

public sealed class EquipmentItemElement : ItemElement
{
	public readonly EquipPartsType PartsType;

	public readonly bool Rechargable;

	public readonly int FunctionGroupID;

	private bool _needToSync;

	private int _remainGaugeRate;

	public int RemainDurability { get; private set; }

	public int RemainAmount { get; private set; }

	public long TurnOnTime { get; private set; }

	public long lastCheckTime { get; private set; }

	public EquipmentItemElement(int itemMasterID, long itemID, bool isFake, int remainDurability = 0, int remainAmount = 0, int price = 0, InventoryController? parent = null)
		: base(ItemType.Equipment, itemMasterID, itemID, isFake, price, parent)
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
		if (itemInfo == null || !(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			throw new Exception($"EquipmentItemElement: itemInfo is not ItemEquipmentInfo. itemMasterID: {itemMasterID}");
		}
		PartsType = itemEquipmentInfo.EquipPartsType;
		int num = SimpleRandUtil.Next(itemEquipmentInfo.MinDurability, itemEquipmentInfo.MaxDurability);
		RemainDurability = ((remainDurability == 0) ? num : remainDurability);
		RemainAmount = remainAmount;
		Rechargable = itemEquipmentInfo.UseCharge;
		FunctionGroupID = itemEquipmentInfo.FunctionGroupID;
	}

	public override ItemInfo toItemInfo()
	{
		return new ItemInfo
		{
			itemType = ItemType,
			itemID = ItemID,
			itemMasterID = ItemMasterID,
			durability = RemainDurability,
			remainGauge = RemainAmount,
			isTurnOn = (TurnOnTime != 0),
			isFake = IsFake,
			price = base.FinalPrice
		};
	}

	public override MMSaveGameDataItemElement ToMMSaveGameDataItemElement()
	{
		return new MMSaveGameDataItemElement
		{
			ItemType = ItemType,
			ItemMasterID = ItemMasterID,
			RemainDurability = RemainDurability,
			RemainAmount = RemainAmount,
			Price = Price
		};
	}

	public void SetDurability(int durability)
	{
		RemainDurability = durability;
	}

	public void SetAmount(int amount)
	{
		RemainAmount = amount;
	}

	public bool CheckElapsed()
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
		if (itemInfo == null || !(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			return false;
		}
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		int decGaugePerPeriod = itemEquipmentInfo.DecGaugePerPeriod;
		if (currentTickMilliSec - lastCheckTime > decGaugePerPeriod)
		{
			lastCheckTime = currentTickMilliSec;
			return true;
		}
		return false;
	}

	public bool IsChargable()
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
		if (itemInfo == null)
		{
			return false;
		}
		if (!(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			return false;
		}
		if (itemEquipmentInfo.UseCharge)
		{
			return true;
		}
		return false;
	}

	public void IncGauge(int amount)
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
		if (itemInfo == null || !(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			Logger.RError($"EquipmentItemElement: itemInfo is not ItemEquipmentInfo. itemMasterID: {ItemMasterID}");
		}
		else
		{
			if (RemainAmount == -1)
			{
				return;
			}
			RemainAmount += amount;
			if (RemainAmount > itemEquipmentInfo.MaxGauge)
			{
				if (itemEquipmentInfo.OverflowPrice != 0)
				{
					RemainAmount = -1;
				}
				else
				{
					RemainAmount = itemEquipmentInfo.MaxGauge;
				}
			}
			int num = (int)((float)RemainAmount / (float)itemEquipmentInfo.MaxGauge * 100f);
			if (_remainGaugeRate / 10 != num / 10)
			{
				_needToSync = true;
				_remainGaugeRate = num;
			}
		}
	}

	public bool NeedToSyncGauge()
	{
		if (_needToSync)
		{
			_needToSync = false;
			return true;
		}
		return false;
	}

	public bool Charge()
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
		if (itemInfo == null || !(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			Logger.RError($"EquipmentItemElement: itemInfo is not ItemEquipmentInfo. itemMasterID: {ItemMasterID}");
			return false;
		}
		SetAmount(itemEquipmentInfo.MaxGauge);
		return true;
	}

	public bool ChangeTurnStatus(bool isTurnOn)
	{
		if (isTurnOn)
		{
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
			if (itemInfo == null || !(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
			{
				return false;
			}
			if (TurnOnTime != 0L)
			{
				return false;
			}
			if (!IsFake && itemEquipmentInfo.DecGaugeInitial > RemainAmount)
			{
				return false;
			}
			TurnOnTime = Hub.s.timeutil.GetCurrentTickMilliSec();
			lastCheckTime = TurnOnTime;
			Parent?.OnTurnOnItem(this, onChangeTurnStatus: true);
			if (!IsFake)
			{
				RemainAmount -= itemEquipmentInfo.DecGaugeInitial;
			}
		}
		else
		{
			if (TurnOnTime == 0L)
			{
				return false;
			}
			TurnOnTime = 0L;
			lastCheckTime = 0L;
			Parent?.OnTurnOffItem(this);
		}
		Parent?.OnSyncStatusChange(this);
		return true;
	}

	public override string ToString()
	{
		return $"{base.ToString()}, PartsType: {PartsType}, RemainDurability: {RemainDurability}, RemainAmount: {RemainAmount}, TurnOnTime: {TurnOnTime}, lastCheckTime: {lastCheckTime}";
	}

	public override ItemElement Clone()
	{
		return new EquipmentItemElement(ItemMasterID, ItemID, IsFake, RemainDurability, RemainAmount, Price, Parent);
	}
}
