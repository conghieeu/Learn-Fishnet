using Bifrost.Cooked;
using ReluProtocol;
using UnityEngine;

public class Pedometer : SocketAttachable, IGaugeableItem, ISpawnableItem
{
	[SerializeField]
	private UIPrefab_Pedometer pedometerUI;

	[SerializeField]
	[Range(0f, 9999f)]
	private int _number;

	private int maxGauge = -1;

	private int itemMasterID = -1;

	public override void OnAttachToSocket()
	{
		itemMasterID = base.item?.ItemMasterID ?? (-1);
	}

	public override void OnDetachFromSocket()
	{
		pedometerUI?.StopBlinking();
		pedometerUI?.SetNumber(0, maxGauge);
	}

	public void OnSpawned(ItemInfo itemInfo)
	{
		itemMasterID = itemInfo.itemMasterID;
		OnGaugeChanged(itemInfo.itemID, itemInfo.remainGauge);
	}

	public void OnGaugeChanged(long itemID, int remainGauge)
	{
		if (maxGauge < 0)
		{
			maxGauge = GetMaxGaugeFromTable();
		}
		if (remainGauge < 0)
		{
			remainGauge = maxGauge - 1;
			if (!pedometerUI.IsBlinking)
			{
				pedometerUI.StartBlinking();
			}
		}
		SetNumber(remainGauge, maxGauge);
	}

	private void SetNumber(int number, int maxNumber)
	{
		if (!(pedometerUI == null))
		{
			pedometerUI.SetNumber(number, maxNumber);
			_number = number;
		}
	}

	private int GetMaxGaugeFromTable()
	{
		if (!(Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID) is ItemEquipmentInfo itemEquipmentInfo))
		{
			return -1;
		}
		return itemEquipmentInfo.MaxGauge;
	}
}
