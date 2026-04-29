using System;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.Enum;

public abstract class ItemElement
{
	public readonly ItemType ItemType;

	public readonly int ItemMasterID;

	public readonly long ItemID;

	public readonly string Name;

	public readonly bool IsFake;

	public readonly int Price;

	public readonly bool IsItemCheckSpawnFieldSkill;

	private readonly ItemSpawnFieldSkillEnvConditionType _itemSpawnFieldSkillEnvConditionType;

	private readonly ItemSpawnFieldSkillEnclosureConditionType _itemSpawnFieldSkillEnclosureConditionType;

	private bool _prevIsSpawningFieldSkill;

	private long _leftSpawnWaitTime;

	private long _leftSpawnCheckTime;

	public readonly bool IsItemCheckBlackout;

	protected InventoryController? Parent;

	public bool ReserveDestroy { get; set; }

	public int FinalPrice
	{
		get
		{
			if ((Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID) ?? throw new Exception($"ItemElement: FinalPrice() itemInfo is null. itemMasterID: {ItemMasterID}")) is ItemEquipmentInfo itemEquipmentInfo && this is EquipmentItemElement equipmentItemElement)
			{
				if (equipmentItemElement.RemainAmount == -1 && itemEquipmentInfo.OverflowPrice != 0)
				{
					return itemEquipmentInfo.OverflowPrice;
				}
				if (itemEquipmentInfo.PriceIncPerGauge > 0)
				{
					return (int)((double)Price + (double)(itemEquipmentInfo.PriceIncPerGauge * equipmentItemElement.RemainAmount) * 0.01);
				}
			}
			return Price;
		}
	}

	public bool NeedCheckReleaseOnStartingVolume { get; set; }

	public abstract ItemElement Clone();

	public abstract ItemInfo toItemInfo();

	public abstract MMSaveGameDataItemElement ToMMSaveGameDataItemElement();

	public ItemElement(ItemType type, int itemMasterID, long itemID, bool isFake, int price, InventoryController? parent)
	{
		ItemType = type;
		ItemMasterID = itemMasterID;
		ItemID = itemID;
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
		if (itemInfo == null)
		{
			throw new Exception($"ItemElement: itemInfo is null. itemMasterID: {itemMasterID}");
		}
		Name = itemInfo.Name;
		IsFake = isFake;
		Price = ((price == 0) ? itemInfo.GetPrice() : price);
		IsItemCheckSpawnFieldSkill = itemInfo.SpawnFieldSkillID != 0;
		_itemSpawnFieldSkillEnvConditionType = itemInfo.SpawnFieldSkillEnvConditionType;
		_itemSpawnFieldSkillEnclosureConditionType = itemInfo.SpawnFieldSkillEnclosureConditionType;
		IsItemCheckBlackout = itemInfo is ItemEquipmentInfo itemEquipmentInfo && itemEquipmentInfo.BlackoutRate > 0;
		Parent = parent;
	}

	public void SetParent(InventoryController? parent)
	{
		Parent = parent;
	}

	public override string ToString()
	{
		return $"ItemType: {ItemType}, ItemMasterID: {ItemMasterID}, ItemID: {ItemID}, Name: {Name}, IsFake: {IsFake}";
	}

	public void SetReserveDestroy()
	{
		ReserveDestroy = true;
	}

	private bool IsSpawningFieldSkill(SkyAndWeatherSystem.eWeatherPreset weatherPreset, bool actorIsIndoor, bool actorIsInSafeArea)
	{
		if (_itemSpawnFieldSkillEnvConditionType switch
		{
			ItemSpawnFieldSkillEnvConditionType.All => 1, 
			ItemSpawnFieldSkillEnvConditionType.Sunny => (weatherPreset == SkyAndWeatherSystem.eWeatherPreset.Sunny) ? 1 : 0, 
			ItemSpawnFieldSkillEnvConditionType.Rainy => (weatherPreset != SkyAndWeatherSystem.eWeatherPreset.Sunny) ? 1 : 0, 
			_ => throw new NotImplementedException($"ItemSpawnFieldSkillEnvConditionType {_itemSpawnFieldSkillEnvConditionType}"), 
		} == 0)
		{
			return false;
		}
		bool result = _itemSpawnFieldSkillEnclosureConditionType switch
		{
			ItemSpawnFieldSkillEnclosureConditionType.All => true, 
			ItemSpawnFieldSkillEnclosureConditionType.Outdoor => !actorIsIndoor, 
			ItemSpawnFieldSkillEnclosureConditionType.Indoor => actorIsIndoor, 
			_ => throw new NotImplementedException($"ItemSpawnFieldSkillEnclosureConditionType {_itemSpawnFieldSkillEnclosureConditionType}"), 
		};
		if (actorIsInSafeArea)
		{
			return false;
		}
		return result;
	}

	public void CheckSpawnFieldSkillWaitPeriod(IVroom vroom, VActor actor)
	{
		if (IsItemCheckSpawnFieldSkill && _leftSpawnCheckTime > 0 && _leftSpawnWaitTime == 0L)
		{
			actor.SendInSight(new ItemSpawnFieldSkillWaitSig
			{
				itemID = ItemID,
				itemMasterID = ItemMasterID,
				actorID = actor.ObjectID,
				waitEvent = true
			}, includeSelf: true);
		}
	}

	public void CheckSpawnFieldSkill(IVroom vroom, VActor actor, long deltaTime)
	{
		if (!IsItemCheckSpawnFieldSkill)
		{
			return;
		}
		SkyAndWeatherSystem.eWeatherPreset currentWeatherPreset = vroom.CurrentWeatherPreset;
		bool isIndoor = actor.IsIndoor;
		bool actorIsInSafeArea = vroom.CheckSafeArea(actor);
		if (!IsSpawningFieldSkill(currentWeatherPreset, isIndoor, actorIsInSafeArea))
		{
			if (_prevIsSpawningFieldSkill && _leftSpawnWaitTime == 0L)
			{
				actor.SendInSight(new ItemSpawnFieldSkillWaitSig
				{
					itemID = ItemID,
					itemMasterID = ItemMasterID,
					actorID = actor.ObjectID,
					waitEvent = false
				}, includeSelf: true);
			}
			_leftSpawnWaitTime = 0L;
			_leftSpawnCheckTime = 0L;
			_prevIsSpawningFieldSkill = false;
			return;
		}
		if (!_prevIsSpawningFieldSkill)
		{
			_prevIsSpawningFieldSkill = true;
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
			if (itemInfo == null)
			{
				throw new Exception($"ItemElement: CheckSpawnFieldSkill() itemInfo is null. itemMasterID: {ItemMasterID}");
			}
			_leftSpawnCheckTime = SimpleRandUtil.Next(itemInfo.SpawnFieldSkillTimeMin, itemInfo.SpawnFieldSkillTimeMax);
			_leftSpawnWaitTime = _leftSpawnCheckTime - itemInfo.SpawnFieldSkillWaitTime;
			return;
		}
		if (_leftSpawnWaitTime != 0L)
		{
			_leftSpawnWaitTime -= deltaTime;
			if (_leftSpawnWaitTime <= 0)
			{
				_leftSpawnWaitTime = 0L;
				actor.SendInSight(new ItemSpawnFieldSkillWaitSig
				{
					itemID = ItemID,
					itemMasterID = ItemMasterID,
					actorID = actor.ObjectID,
					waitEvent = true
				}, includeSelf: true);
			}
		}
		if (_leftSpawnCheckTime == 0L)
		{
			return;
		}
		_leftSpawnCheckTime -= deltaTime;
		if (_leftSpawnCheckTime <= 0)
		{
			_leftSpawnCheckTime = 0L;
			actor.SendInSight(new ItemSpawnFieldSkillWaitSig
			{
				itemID = ItemID,
				itemMasterID = ItemMasterID,
				actorID = actor.ObjectID,
				waitEvent = false
			}, includeSelf: true);
			ItemMasterInfo itemInfo2 = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
			if (itemInfo2 == null)
			{
				throw new Exception($"ItemElement: CheckSpawnFieldSkill() itemInfo is null. itemMasterID: {ItemMasterID}");
			}
			if (itemInfo2.SpawnFieldSkillRate >= SimpleRandUtil.Next(0, 10000) && !vroom.CheckSafeArea(actor))
			{
				vroom.SpawnFieldSkill(itemInfo2.SpawnFieldSkillID, actor.Position, isIndoor, null, null, null, ReasonOfSpawn.ItemSpawn);
			}
			_leftSpawnCheckTime = SimpleRandUtil.Next(itemInfo2.SpawnFieldSkillTimeMin, itemInfo2.SpawnFieldSkillTimeMax);
			_leftSpawnWaitTime = _leftSpawnCheckTime - itemInfo2.SpawnFieldSkillWaitTime;
		}
	}

	public bool IsPromotionItem()
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
		if (itemInfo == null)
		{
			Logger.RWarn($"[ItemElement] IsPromotionItem() itemInfo is null. itemMasterID: {ItemMasterID}");
			return false;
		}
		return itemInfo.IsPromotionItem;
	}

	public bool IsPromotionItemHidden()
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(ItemMasterID);
		if (itemInfo == null)
		{
			Logger.RWarn($"[ItemElement] IsPromotionItemHidden() itemInfo is null. itemMasterID: {ItemMasterID}");
			return false;
		}
		return itemInfo.IsPromotionItemHidden;
	}
}
