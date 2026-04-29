using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public class InventoryController : IVActorController, IDisposable
{
	private VCreature _self;

	private Dictionary<int, ItemElement?> _inventorySlots = new Dictionary<int, ItemElement>();

	private List<int> _removeInventorySlotKeys = new List<int>();

	private int _totalWeight;

	public VActorControllerType type { get; } = VActorControllerType.Inventory;

	public int CurrentInventorySlot { get; private set; }

	public int TotalWeight => _totalWeight;

	public InventoryController(VCreature self)
	{
		_self = self;
	}

	public void Initialize()
	{
		_inventorySlots.Clear();
		for (int i = 1; i <= 4; i++)
		{
			_inventorySlots.Add(i, null);
		}
		CurrentInventorySlot = 1;
	}

	public void Update(long deltaTime)
	{
		_removeInventorySlotKeys.Clear();
		if (_self.AbnormalControlUnit.IsCantHoldItem())
		{
			foreach (int item in _inventorySlots.Keys.ToList())
			{
				_self.HandleReleaseItem(0, item, releaseByCC: true);
			}
		}
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			ItemElement itemElement = _inventorySlots[inventorySlot.Key];
			if (itemElement == null || itemElement.IsFake || !(itemElement is EquipmentItemElement equipmentItemElement) || (equipmentItemElement.PartsType != EquipPartsType.ToggleEquip && equipmentItemElement.PartsType != EquipPartsType.AutoToggleEquip))
			{
				continue;
			}
			if (equipmentItemElement.RemainDurability <= 0 && !equipmentItemElement.ReserveDestroy)
			{
				_removeInventorySlotKeys.Add(inventorySlot.Key);
			}
			else
			{
				if (!equipmentItemElement.CheckElapsed() || equipmentItemElement.TurnOnTime == 0L)
				{
					continue;
				}
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement.ItemMasterID);
				if (itemInfo == null)
				{
					Logger.RError($"InventoryController.Update: itemInfo is null. itemMasterID: {equipmentItemElement.ItemMasterID}");
					continue;
				}
				if (!(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
				{
					Logger.RError($"InventoryController.Update: itemInfo is not ItemEquipmentInfo. itemMasterID: {equipmentItemElement.ItemMasterID}");
					continue;
				}
				equipmentItemElement.SetAmount(equipmentItemElement.RemainAmount - itemEquipmentInfo.DecGaugePerUse);
				if (equipmentItemElement.RemainAmount <= 0)
				{
					if (itemEquipmentInfo.UseDestroyByGauge)
					{
						_removeInventorySlotKeys.Add(inventorySlot.Key);
						continue;
					}
					equipmentItemElement.SetAmount(0);
					equipmentItemElement.ChangeTurnStatus(isTurnOn: false);
				}
				else
				{
					OnSyncStatusChange(equipmentItemElement);
				}
			}
		}
		if (_removeInventorySlotKeys.Count > 0)
		{
			foreach (int removeInventorySlotKey in _removeInventorySlotKeys)
			{
				RemoveInvenItem(removeInventorySlotKey, needToNotiDestroy: true, sync: true);
			}
		}
		if (_self.VRoom is VWaitingRoom)
		{
			return;
		}
		foreach (ItemElement value in _inventorySlots.Values)
		{
			if (value != null && value.IsItemCheckSpawnFieldSkill)
			{
				value.CheckSpawnFieldSkill(_self.VRoom, _self, deltaTime);
			}
		}
	}

	public void OnSyncStatusChange(ItemElement element)
	{
		_self.SendInSight(new ChangeEquipStatusSig
		{
			actorID = _self.ObjectID,
			changedItemInfo = element.toItemInfo()
		}, includeSelf: true);
	}

	public void OnTurnOnItem(ItemElement itemElement, bool onChangeTurnStatus)
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemElement.ItemMasterID);
		if (itemInfo == null)
		{
			Logger.RError($"InventoryController.OnTurnOnItem: itemInfo is null. itemMasterID: {itemElement.ItemMasterID}");
			return;
		}
		if (itemInfo.HandheldAuraSkillID != 0 && (onChangeTurnStatus || itemInfo is ItemEquipmentInfo { HandheldAuraSkillByGauge: false }))
		{
			_self.AuraControlUnit?.AddAura(itemInfo.HandheldAuraSkillID, defaultFlag: false);
		}
		if (itemInfo.HandheldAbnormalID != 0 && (onChangeTurnStatus || itemInfo is ItemEquipmentInfo { HandheldAbnormalByGauge: false }))
		{
			_self.AbnormalControlUnit?.AppendAbnormal(_self.ObjectID, itemInfo.HandheldAbnormalID, 0, ignoreImmuneCheck: false, 0, AbnormalReason.Item);
		}
	}

	public void OnTurnOffItem(ItemElement itemElement)
	{
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemElement.ItemMasterID);
		if (itemInfo == null)
		{
			Logger.RError($"InventoryController.OnTurnOffItem: itemInfo is null. itemMasterID: {itemElement.ItemMasterID}");
			return;
		}
		if (itemInfo.HandheldAuraSkillID != 0)
		{
			_self.AuraControlUnit?.RemoveAura(itemInfo.HandheldAuraSkillID, sync: true);
		}
		if (itemInfo.HandheldAbnormalID != 0)
		{
			_self.AbnormalControlUnit?.DispelAbnormal(_self.ObjectID, itemInfo.HandheldAbnormalID, force: true);
		}
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		if (actionType == VActorActionType.Looting)
		{
			if (_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) && value != null && value.ItemType == ItemType.Equipment)
			{
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(value.ItemMasterID);
				if (itemInfo == null)
				{
					Logger.RError("InventoryController.CanAction: itemInfo is null");
					return MsgErrorCode.MasterDataNotFound;
				}
				if (!(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
				{
					Logger.RError("InventoryController.CanAction: itemInfo is not ItemEquipmentInfo");
					return MsgErrorCode.ItemDataMismatch;
				}
				if (itemEquipmentInfo.ForbidChange)
				{
					return MsgErrorCode.ItemAddForbiddenByType;
				}
			}
			return MsgErrorCode.Success;
		}
		return MsgErrorCode.Success;
	}

	public void Dispose()
	{
	}

	public void WaitInitDone()
	{
	}

	public void PostUpdate(long deltaTime)
	{
	}

	private void UpdateInventory(int slotIndex, ItemElement? itemElement, bool sync = false)
	{
		if (!_inventorySlots.TryGetValue(slotIndex, out ItemElement value))
		{
			Logger.RError($"InventoryController.UpdateInventory: invalid slotIndex {slotIndex}");
			return;
		}
		value?.SetParent(null);
		_inventorySlots[slotIndex] = itemElement;
		if (CurrentInventorySlot == slotIndex)
		{
			_self.SendInSight(new ChangeItemLooksSig
			{
				actorID = _self.ObjectID,
				slotIndex = CurrentInventorySlot,
				onHandItem = (itemElement?.toItemInfo() ?? new ItemInfo()),
				activateTime = Hub.s.timeutil.GetCurrentTickMilliSec()
			}, sync);
			_self.EmotionControlUnit?.OnEquipChanged();
			_self.ScrapMotionController?.OnEquipChanged();
		}
		if (sync)
		{
			_self.SendInSight(new UpdateInvenSig
			{
				actorID = _self.ObjectID,
				inventoryInfos = GetInventoryInfos()
			}, includeSelf: true);
		}
		itemElement?.SetParent(this);
		OnChangeInventory();
	}

	public bool CanAddItem(ItemElement itemElement)
	{
		if (!CheckForbidAllowed())
		{
			return false;
		}
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value == null)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckForbidAllowed()
	{
		return true;
	}

	public MsgErrorCode HandleAddItem(ItemElement itemElement, out int addedSlotIndex, bool sync = false, bool byLooting = false)
	{
		addedSlotIndex = -1;
		if (!CheckForbidAllowed())
		{
			return MsgErrorCode.ItemAddForbiddenByType;
		}
		if (_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) && value == null)
		{
			if (byLooting)
			{
				OnAddItemByLooting(itemElement);
			}
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemElement.ItemMasterID);
			if (itemInfo != null)
			{
				if (itemElement is EquipmentItemElement equipmentItemElement)
				{
					if (itemInfo is ItemEquipmentInfo itemEquipmentInfo)
					{
						if (itemEquipmentInfo.EquipPartsType == EquipPartsType.AutoToggleEquip)
						{
							equipmentItemElement.ChangeTurnStatus(isTurnOn: true);
						}
						else if (!itemEquipmentInfo.HandheldAuraSkillByGauge || !itemEquipmentInfo.HandheldAbnormalByGauge)
						{
							OnTurnOnItem(itemElement, onChangeTurnStatus: false);
						}
					}
				}
				else
				{
					OnTurnOnItem(itemElement, onChangeTurnStatus: false);
				}
			}
			AddInvenItem(CurrentInventorySlot, itemElement, sync);
			addedSlotIndex = CurrentInventorySlot;
			return MsgErrorCode.Success;
		}
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value == null)
			{
				if (byLooting)
				{
					OnAddItemByLooting(itemElement);
				}
				AddInvenItem(inventorySlot.Key, itemElement, sync);
				addedSlotIndex = inventorySlot.Key;
				return MsgErrorCode.Success;
			}
		}
		return MsgErrorCode.InvenFull;
	}

	public void SetHandEmptyByAI()
	{
		if (_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) && value != null && value.IsFake)
		{
			RemoveInvenItem(CurrentInventorySlot, needToNotiDestroy: false, sync: true);
		}
	}

	public bool GenerateFakeItemForAI(int masterID)
	{
		if (Hub.s.dataman.ExcelDataManager.GetItemInfo(masterID) == null)
		{
			Logger.RError($"InventoryController.GenerateFakeItemForAI: itemInfo is null. masterID: {masterID}");
			return false;
		}
		ItemElement newItemElement = _self.VRoom.GetNewItemElement(masterID, isFake: true);
		if (newItemElement == null)
		{
			Logger.RError($"InventoryController.GenerateFakeItemForAI: newItem is null. masterID: {masterID}");
			return false;
		}
		if (_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) && value != null)
		{
			RemoveInvenItem(CurrentInventorySlot, needToNotiDestroy: false, sync: false);
		}
		HandleAddItem(newItemElement, out var _, sync: true);
		return true;
	}

	public bool CheckCurrentSlotInPickRules(List<BTInvenPickRule> pickRules)
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return false;
		}
		foreach (BTInvenPickRule pickRule in pickRules)
		{
			switch (pickRule)
			{
			case BTInvenPickRule.Consumable:
				if (value.ItemType == ItemType.Consumable)
				{
					return true;
				}
				break;
			case BTInvenPickRule.SkillEquip:
				if (value.ItemType == ItemType.Equipment && value is EquipmentItemElement { PartsType: EquipPartsType.SkillEquip })
				{
					return true;
				}
				break;
			case BTInvenPickRule.ToggleEquip:
				if (value.ItemType == ItemType.Equipment && value is EquipmentItemElement { PartsType: EquipPartsType.ToggleEquip })
				{
					return true;
				}
				break;
			case BTInvenPickRule.Rechargeable:
				if (value.ItemType == ItemType.Equipment && value is EquipmentItemElement { Rechargable: not false })
				{
					return true;
				}
				break;
			case BTInvenPickRule.Misc:
				if (value.ItemType == ItemType.Miscellany)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	public int GetTargetSlotIndex(List<BTInvenPickRule> pickRules)
	{
		foreach (BTInvenPickRule item in pickRules.OrderBy((BTInvenPickRule x) => Guid.NewGuid()))
		{
			switch (item)
			{
			case BTInvenPickRule.Consumable:
				foreach (KeyValuePair<int, ItemElement> item2 in _inventorySlots.OrderBy<KeyValuePair<int, ItemElement>, Guid>((KeyValuePair<int, ItemElement> x) => Guid.NewGuid()))
				{
					if (item2.Value != null && item2.Value.ItemType == ItemType.Consumable)
					{
						return item2.Key;
					}
				}
				break;
			case BTInvenPickRule.SkillEquip:
				foreach (KeyValuePair<int, ItemElement> item3 in _inventorySlots.OrderBy<KeyValuePair<int, ItemElement>, Guid>((KeyValuePair<int, ItemElement> x) => Guid.NewGuid()))
				{
					if (item3.Value != null && item3.Value.ItemType == ItemType.Equipment && item3.Value is EquipmentItemElement { PartsType: EquipPartsType.SkillEquip })
					{
						return item3.Key;
					}
				}
				break;
			case BTInvenPickRule.ToggleEquip:
				foreach (KeyValuePair<int, ItemElement> item4 in _inventorySlots.OrderBy<KeyValuePair<int, ItemElement>, Guid>((KeyValuePair<int, ItemElement> x) => Guid.NewGuid()))
				{
					if (item4.Value != null && item4.Value.ItemType == ItemType.Equipment && item4.Value is EquipmentItemElement { PartsType: EquipPartsType.ToggleEquip })
					{
						return item4.Key;
					}
				}
				break;
			case BTInvenPickRule.Rechargeable:
				foreach (KeyValuePair<int, ItemElement> item5 in _inventorySlots.OrderBy<KeyValuePair<int, ItemElement>, Guid>((KeyValuePair<int, ItemElement> x) => Guid.NewGuid()))
				{
					if (item5.Value != null && item5.Value.ItemType == ItemType.Equipment && item5.Value is EquipmentItemElement { Rechargable: not false })
					{
						return item5.Key;
					}
				}
				break;
			case BTInvenPickRule.Misc:
				foreach (KeyValuePair<int, ItemElement> item6 in _inventorySlots.OrderBy<KeyValuePair<int, ItemElement>, Guid>((KeyValuePair<int, ItemElement> x) => Guid.NewGuid()))
				{
					if (item6.Value != null && item6.Value.ItemType == ItemType.Miscellany)
					{
						return item6.Key;
					}
				}
				break;
			}
		}
		return 0;
	}

	public MsgErrorCode HandleChangeActiveInvenSlot(int slotIndex, bool sync = true, int hashcode = 0)
	{
		MsgErrorCode msgErrorCode = _self.CanAction(VActorActionType.ChangeInvenSlot);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		if (_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) && value != null)
		{
			if (!CheckForbidAllowed())
			{
				return MsgErrorCode.ItemAddForbiddenByType;
			}
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(value.ItemMasterID);
			if (itemInfo != null)
			{
				if (value.ItemType == ItemType.Equipment)
				{
					if (!(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
					{
						Logger.RError("InventoryController.HandleChangeActiveInvenSlot: weaponInfo is not ItemEquipmentInfo");
						return MsgErrorCode.ItemDataMismatch;
					}
					_self.SkillControlUnit?.SetSkillFromWeapon(null);
					if (value is EquipmentItemElement equipmentItemElement)
					{
						bool flag = false;
						if (equipmentItemElement.PartsType == EquipPartsType.AutoToggleEquip && equipmentItemElement.TurnOnTime != 0L)
						{
							flag = true;
						}
						if (equipmentItemElement.TurnOnTime != 0L && (itemEquipmentInfo.HandheldAuraSkillByGauge || itemEquipmentInfo.HandheldAbnormalByGauge))
						{
							flag = true;
						}
						if (flag)
						{
							equipmentItemElement.ChangeTurnStatus(isTurnOn: false);
						}
					}
				}
				else
				{
					OnTurnOffItem(value);
				}
			}
		}
		CurrentInventorySlot = slotIndex;
		if (_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value2) && value2 != null)
		{
			if (value2.ItemType == ItemType.Equipment)
			{
				if (value2 is EquipmentItemElement equipmentItemElement2)
				{
					ItemMasterInfo itemInfo2 = Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement2.ItemMasterID);
					if (itemInfo2 == null)
					{
						Logger.RError("InventoryController.HandleChangeActiveInvenSlot: weaponInfo is null");
						return MsgErrorCode.MasterDataNotFound;
					}
					if (!(itemInfo2 is ItemEquipmentInfo itemEquipmentInfo2))
					{
						Logger.RError("InventoryController.HandleChangeActiveInvenSlot: weaponInfo is not ItemEquipmentInfo");
						return MsgErrorCode.ItemDataMismatch;
					}
					if (itemEquipmentInfo2.EquipPartsType == EquipPartsType.AutoToggleEquip)
					{
						equipmentItemElement2.ChangeTurnStatus(isTurnOn: true);
					}
					else if (!itemEquipmentInfo2.HandheldAuraSkillByGauge || !itemEquipmentInfo2.HandheldAbnormalByGauge)
					{
						OnTurnOnItem(value2, onChangeTurnStatus: false);
					}
					_self.SkillControlUnit?.SetSkillFromWeapon(equipmentItemElement2);
				}
			}
			else
			{
				OnTurnOnItem(value2, onChangeTurnStatus: false);
			}
		}
		if (sync)
		{
			_self.SendToMe(new ChangeActiveInvenSlotRes(hashcode)
			{
				currentHandEquipmentInfo = (value2?.toItemInfo() ?? new ItemInfo()),
				currentInvenSlotIndex = CurrentInventorySlot
			});
			_self.SendInSight(new ChangeItemLooksSig
			{
				actorID = _self.ObjectID,
				slotIndex = CurrentInventorySlot,
				onHandItem = (value2?.toItemInfo() ?? new ItemInfo()),
				activateTime = Hub.s.timeutil.GetCurrentTickMilliSec()
			}, sync);
		}
		_self.EmotionControlUnit?.OnEquipChanged();
		_self.ScrapMotionController?.OnEquipChanged();
		return MsgErrorCode.Success;
	}

	private static void TurnOffItemElement(ItemElement element)
	{
		if (element is EquipmentItemElement equipmentItemElement)
		{
			equipmentItemElement.ChangeTurnStatus(isTurnOn: false);
		}
	}

	public ItemElement? HandleExtractItem(int slotNum = 0, bool adandonFree = false)
	{
		int num = ((slotNum > 0) ? slotNum : CurrentInventorySlot);
		if (!_inventorySlots.TryGetValue(num, out ItemElement value) || value == null)
		{
			return null;
		}
		if (value is EquipmentItemElement { PartsType: EquipPartsType.AutoReverseToggleEquip } equipmentItemElement)
		{
			equipmentItemElement.ChangeTurnStatus(isTurnOn: true);
		}
		else
		{
			TurnOffItemElement(value);
		}
		if (adandonFree && value.ItemType == ItemType.Consumable)
		{
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(value.ItemMasterID);
			if (itemInfo == null)
			{
				return null;
			}
			if (!(itemInfo is ItemConsumableInfo))
			{
				return null;
			}
			if (value.Price <= 0)
			{
				return null;
			}
		}
		RemoveInvenItem(num, needToNotiDestroy: false, sync: true);
		return value;
	}

	private void ReserveRemoveInvenItem(int slotIndex)
	{
		if (_inventorySlots.TryGetValue(slotIndex, out ItemElement value))
		{
			value?.SetReserveDestroy();
		}
	}

	public void OnAnimationEventDestroy(int skillMasterID)
	{
		foreach (KeyValuePair<int, ItemElement> item in _inventorySlots.Where<KeyValuePair<int, ItemElement>>((KeyValuePair<int, ItemElement> x) => x.Value != null && x.Value.ReserveDestroy).ToList())
		{
			ItemElement value = item.Value;
			if (value != null)
			{
				SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
				if (skillInfo != null && skillInfo.ShowDestroyEffect)
				{
					_self.SendInSight(new DestroyItemSig
					{
						actorID = _self.ObjectID,
						destroyedItemInfo = value.toItemInfo()
					}, includeSelf: true);
				}
				RemoveInvenItem(item.Key, needToNotiDestroy: true, sync: true);
			}
		}
	}

	public void RemoveInvenItem(int slotIndex, bool needToNotiDestroy, bool sync)
	{
		if (!_inventorySlots.TryGetValue(slotIndex, out ItemElement value) || value == null)
		{
			Logger.RError($"InventoryController.RemoveInvenItem: slotIndex not found in inventory. slotIndex: {slotIndex}");
			return;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(value.ItemMasterID);
		if (itemInfo == null)
		{
			Logger.RError($"InventoryController.RemoveInvenItem: itemInfo is null. itemMasterID: {value.ItemMasterID}");
			return;
		}
		OnTurnOffItem(value);
		if (needToNotiDestroy && itemInfo is ItemEquipmentInfo itemEquipmentInfo && itemEquipmentInfo.GameActionsWhenDestroy.Length > 0)
		{
			Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
			ImmutableArray<IGameAction>.Enumerator enumerator = itemEquipmentInfo.GameActionsWhenDestroy.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IGameAction current = enumerator.Current;
				dictionary.Add(current.Clone(), new GameActionParamPosition(_self.Position, _self.IsIndoor));
			}
			_self.VRoom.EnqueueEventAction(dictionary, 0);
		}
		UpdateInventory(slotIndex, null, sync);
	}

	public void AddInvenItem(int slotIndex, ItemElement itemElement, bool sync = true)
	{
		if (!_inventorySlots.TryGetValue(slotIndex, out ItemElement value) || value == null)
		{
			UpdateInventory(slotIndex, itemElement, sync);
		}
		if (slotIndex == CurrentInventorySlot && itemElement is EquipmentItemElement skillFromWeapon)
		{
			_self.SkillControlUnit?.SetSkillFromWeapon(skillFromWeapon);
		}
	}

	public (int, ConsumableItemElement?) GetBulletItemElement(BulletType bulletType)
	{
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value == null || inventorySlot.Value.ItemType != ItemType.Consumable || !(inventorySlot.Value is ConsumableItemElement consumableItemElement))
			{
				continue;
			}
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(consumableItemElement.ItemMasterID);
			if (itemInfo == null)
			{
				Logger.RWarn("InventoryController.HandleReloadWeapon: consumableInfo is null");
			}
			else if (!(itemInfo is ItemConsumableInfo itemConsumableInfo))
			{
				Logger.RWarn("InventoryController.HandleReloadWeapon: consumableInfo is not ItemConsumableInfo");
			}
			else if (itemConsumableInfo.ConsumeType == ConsumeItemType.Bullet && itemConsumableInfo.BulletType == bulletType)
			{
				if (consumableItemElement.RemainCount > 0)
				{
					return (inventorySlot.Key, consumableItemElement);
				}
				Logger.RWarn($"InventoryController.HandleReloadWeapon: consumableItemElement.RemainCount is 0. slotIndex: {inventorySlot.Key}. itemMasterID: {consumableItemElement.ItemMasterID}");
			}
		}
		return (0, null);
	}

	public MsgErrorCode HandleReloadWeapon()
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (value.ItemType != ItemType.Equipment)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (!(value is EquipmentItemElement equipmentItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (equipmentItemElement.PartsType != EquipPartsType.SkillEquip)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement.ItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		if (!(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (itemEquipmentInfo.WeaponType != WeaponType.Gun)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (itemEquipmentInfo.ReloadSkillMasterID == 0)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(itemEquipmentInfo.ReloadSkillMasterID);
		if (skillInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		bool flag = false;
		int num = 0;
		int num2 = itemEquipmentInfo.MaxGauge - equipmentItemElement.RemainAmount;
		while (!flag)
		{
			var (slotIndex, consumableItemElement) = GetBulletItemElement(skillInfo.BulletType);
			if (consumableItemElement == null)
			{
				flag = true;
				continue;
			}
			int num3 = num2 - num;
			if (num3 <= 0)
			{
				flag = true;
				continue;
			}
			int num4 = Math.Min(num3, consumableItemElement.RemainCount);
			if (consumableItemElement.RemainCount > num4)
			{
				consumableItemElement.DecreaseCount(num4);
				flag = true;
			}
			else
			{
				RemoveInvenItem(slotIndex, needToNotiDestroy: false, sync: true);
			}
			num += num4;
		}
		if (num == 0)
		{
			return MsgErrorCode.ItemNotFound;
		}
		equipmentItemElement.SetAmount(equipmentItemElement.RemainAmount + num);
		_self.SendInSight(new ReloadWeaponSig
		{
			actorID = _self.ObjectID,
			currentHandEquipmentInfo = equipmentItemElement.toItemInfo(),
			inventoryInfos = GetInventoryInfos(),
			currentInvenSlotIndex = CurrentInventorySlot
		}, includeSelf: true);
		return MsgErrorCode.Success;
	}

	public ItemInfo GetEquipInfo()
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return new ItemInfo();
		}
		return value?.toItemInfo() ?? new ItemInfo();
	}

	public void GetInventoryInfo(ref Dictionary<int, ItemInfo> info)
	{
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value != null)
			{
				info.Add(inventorySlot.Key, inventorySlot.Value.toItemInfo());
			}
		}
	}

	public Dictionary<int, ItemInfo> GetInventoryInfos()
	{
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value != null)
			{
				dictionary.Add(inventorySlot.Key, inventorySlot.Value.toItemInfo());
			}
		}
		return dictionary;
	}

	public int GetCurrentInvenSlotIndex()
	{
		return CurrentInventorySlot;
	}

	public MsgErrorCode OnHitSkill(int skillMasterID, int hitCount)
	{
		MsgErrorCode msgErrorCode = CanUseSkill(skillMasterID);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(value is EquipmentItemElement equipmentItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (equipmentItemElement.PartsType != EquipPartsType.SkillEquip)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement.ItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		if (!(itemInfo is ItemEquipmentInfo))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
		if (skillInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		if (skillInfo.DecDurabilityTypeWhenSkillUse == DecreaseDurabilityType.OnHitAnyone)
		{
			equipmentItemElement.SetDurability(equipmentItemElement.RemainDurability - skillInfo.DecDurabilityPerUse);
		}
		else
		{
			if (skillInfo.DecDurabilityTypeWhenSkillUse != DecreaseDurabilityType.PerHit)
			{
				return MsgErrorCode.Success;
			}
			equipmentItemElement.SetDurability(equipmentItemElement.RemainDurability - skillInfo.DecDurabilityPerUse * hitCount);
		}
		if (equipmentItemElement.RemainDurability <= 0)
		{
			ReserveRemoveInvenItem(CurrentInventorySlot);
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode OnUseSkill(int skillMasterID)
	{
		MsgErrorCode msgErrorCode = CanUseSkill(skillMasterID);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(value is EquipmentItemElement equipmentItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (equipmentItemElement.PartsType != EquipPartsType.SkillEquip)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement.ItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		ItemEquipmentInfo weaponItemInfo = itemInfo as ItemEquipmentInfo;
		if (weaponItemInfo == null)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
		if (skillInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		bool flag = false;
		if (skillInfo.DecDurabilityTypeWhenSkillUse == DecreaseDurabilityType.AnyTime && skillInfo.DecDurabilityPerUse != 0)
		{
			equipmentItemElement.SetDurability(equipmentItemElement.RemainDurability - skillInfo.DecDurabilityPerUse);
			flag = true;
		}
		if (skillMasterID != weaponItemInfo.ReloadSkillMasterID)
		{
			SkillInfo skillInfo2 = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
			if (skillInfo2 == null)
			{
				return MsgErrorCode.MasterDataNotFound;
			}
			if (skillInfo2.DecGauge != 0)
			{
				if (equipmentItemElement.RemainAmount < skillInfo2.DecGauge)
				{
					return MsgErrorCode.CanUseSkillByGauge;
				}
				equipmentItemElement.SetAmount(equipmentItemElement.RemainAmount - skillInfo2.DecGauge);
				flag = true;
			}
		}
		if (weaponItemInfo.SoundAggroPerUse > 0)
		{
			_self.VRoom.IterateAllMonster(delegate(VActor monster)
			{
				monster.AIControlUnit?.AddSoundAggroPoint(_self, weaponItemInfo.SoundAggroPerUse);
			});
		}
		if (equipmentItemElement.RemainDurability <= 0)
		{
			ReserveRemoveInvenItem(CurrentInventorySlot);
		}
		else if (flag)
		{
			_self.SendInSight(new ChangeEquipStatusSig
			{
				actorID = _self.ObjectID,
				changedItemInfo = equipmentItemElement.toItemInfo()
			}, includeSelf: true);
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode OnUseSkill_SpawnProjectile(int skillMasterID, VProjectileObject projectile)
	{
		MsgErrorCode msgErrorCode = CanUseSkill(skillMasterID);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(value is EquipmentItemElement equipmentItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (equipmentItemElement.PartsType != EquipPartsType.SkillEquip)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement.ItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		if (!(itemInfo is ItemEquipmentInfo))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
		if (skillInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		ProjectileInfo projectileInfo = Hub.s.dataman.ExcelDataManager.GetProjectileInfo(projectile.MasterID);
		if (projectileInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		if (skillInfo.DecDurabilityPerUse > 0 && projectileInfo.DecreaseDurabilityOnCollision > 0)
		{
			Logger.RError("InventoryController.OnUseSkill_SpawnProjectile: both skillInfo.DecDurabilityPerUse and projectileInfo.DecreaseDurabilityOnCollision are greater than 0.");
			return MsgErrorCode.InvalidItem;
		}
		bool flag = false;
		int num = 0;
		if (projectileInfo.RemoveFromInventory)
		{
			if (projectileInfo.SpawnItemMasterIDonCollision > 0)
			{
				if (projectileInfo.SpawnItemMasterIDonCollision == equipmentItemElement.ItemMasterID)
				{
					if (projectileInfo.CanInheritDurability)
					{
						ItemElement itemElement = HandleExtractItem();
						if (itemElement != null)
						{
							projectile.AttachItemElement(itemElement);
						}
					}
					else
					{
						flag = true;
						num = projectileInfo.SpawnItemMasterIDonCollision;
					}
				}
				else
				{
					flag = true;
					num = projectileInfo.SpawnItemMasterIDonCollision;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				ReserveRemoveInvenItem(CurrentInventorySlot);
			}
			if (num > 0)
			{
				ItemElement newItemElement = _self.VRoom.GetNewItemElement(num, isFake: false);
				if (newItemElement != null)
				{
					projectile.AttachItemElement(newItemElement);
				}
			}
		}
		else
		{
			bool flag2 = false;
			if (skillInfo.DecDurabilityTypeWhenSkillUse == DecreaseDurabilityType.PerSpawnProjectile && skillInfo.DecDurabilityPerUse != 0)
			{
				equipmentItemElement.SetDurability(equipmentItemElement.RemainDurability - skillInfo.DecDurabilityPerUse);
				flag2 = true;
			}
			if (equipmentItemElement.RemainDurability <= 0)
			{
				ReserveRemoveInvenItem(CurrentInventorySlot);
			}
			else if (flag2)
			{
				_self.SendInSight(new ChangeEquipStatusSig
				{
					actorID = _self.ObjectID,
					changedItemInfo = equipmentItemElement.toItemInfo()
				}, includeSelf: true);
			}
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode CanUseSkill(int skillMasterID)
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(value is EquipmentItemElement equipmentItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (equipmentItemElement.PartsType != EquipPartsType.SkillEquip)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement.ItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		if (!(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (!itemEquipmentInfo.HasSkill(skillMasterID))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
		if (skillInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		if (itemEquipmentInfo.WeaponType == WeaponType.Gun)
		{
			if (skillMasterID == itemEquipmentInfo.ReloadSkillMasterID)
			{
				if (GetBulletItemElement(skillInfo.BulletType).Item2 == null)
				{
					return MsgErrorCode.ItemNotFound;
				}
			}
			else if (skillInfo.DecGauge != 0 && equipmentItemElement.RemainAmount < skillInfo.DecGauge)
			{
				return MsgErrorCode.CanUseSkillByGauge;
			}
		}
		if (skillInfo.DecGauge != 0 && itemEquipmentInfo.HasSkill(skillMasterID) && equipmentItemElement.RemainAmount <= 0)
		{
			return MsgErrorCode.CanUseSkillByGauge;
		}
		return MsgErrorCode.Success;
	}

	public Dictionary<int, ItemElement> ExtractAllItemElement(bool turnOff)
	{
		Dictionary<int, ItemElement> dictionary = new Dictionary<int, ItemElement>();
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value != null)
			{
				if (turnOff)
				{
					TurnOffItemElement(inventorySlot.Value);
				}
				dictionary.Add(inventorySlot.Key, inventorySlot.Value);
			}
		}
		return dictionary;
	}

	public List<EquipmentItemElement> GetAllEquipItems()
	{
		List<EquipmentItemElement> list = new List<EquipmentItemElement>();
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value != null && inventorySlot.Value is EquipmentItemElement item)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public void ClearAllInventory(bool sync = true)
	{
		foreach (int item in (from x in _inventorySlots
			where x.Value != null
			select x.Key).ToList())
		{
			RemoveInvenItem(item, needToNotiDestroy: false, sync: false);
		}
		Initialize();
		OnChangeInventory();
		if (sync)
		{
			_self.SendInSight(new UpdateInvenSig
			{
				actorID = _self.ObjectID,
				inventoryInfos = GetInventoryInfos()
			}, includeSelf: true);
			_self.SendInSight(new ChangeItemLooksSig
			{
				actorID = _self.ObjectID,
				slotIndex = CurrentInventorySlot
			});
		}
	}

	public MsgErrorCode BarterItem(int hashCode)
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(value.ItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		if (itemInfo.ItemDropID == 0)
		{
			return MsgErrorCode.CantAction;
		}
		ItemDropInfo itemDropInfo = Hub.s.dataman.ExcelDataManager.GetItemDropInfo(itemInfo.ItemDropID);
		if (itemDropInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		List<int> dropItemList = itemDropInfo.GetDropItemList();
		if (dropItemList.Count == 0)
		{
			return MsgErrorCode.CantAction;
		}
		foreach (int item in dropItemList)
		{
			if (Hub.s.dataman.ExcelDataManager.GetItemInfo(item) == null)
			{
				return MsgErrorCode.MasterDataNotFound;
			}
			Vector3 reachableDistancePos = _self.VRoom.GetReachableDistancePos(_self.PositionVector, _self.Position.yaw, 0.6f);
			if (reachableDistancePos == Vector3.zero)
			{
				return MsgErrorCode.InvalidPosition;
			}
			if (!_self.VRoom.GetRandomReachablePointInRadius(reachableDistancePos, 0f, 0.3f, out var resultPos))
			{
				return MsgErrorCode.InvalidPosition;
			}
			RemoveInvenItem(CurrentInventorySlot, needToNotiDestroy: false, sync: false);
			ItemElement newItemElement = _self.VRoom.GetNewItemElement(item, isFake: false);
			if (newItemElement == null)
			{
				return MsgErrorCode.CantAction;
			}
			if (_self.VRoom.SpawnLootingObject(newItemElement, resultPos.toPosWithRot(0f), _self.IsIndoor, ReasonOfSpawn.Buying) == 0)
			{
				Logger.RError($"Failed to spawn looting object by releasing item. itemElement: {newItemElement}");
				return MsgErrorCode.BuyingItemSpawnFailed;
			}
			_self.SendToMe(new BarterItemRes(hashCode));
			_self.SendInSight(new UpdateInvenSig
			{
				actorID = _self.ObjectID,
				inventoryInfos = GetInventoryInfos()
			}, includeSelf: true);
			_self.AddGameEventLog(new GELPurchageCrowShop
			{
				RoomSessionID = _self.VRoom.SessionID,
				SessionCycleCount = _self.VRoom.SessionCycleCount,
				StageCount = _self.VRoom.CurrentDay,
				ShopMasterID = itemDropInfo.MasterID,
				inputItemMasterID = value.ItemMasterID,
				outputItemMasterID = item,
				Timestamp = DateTime.UtcNow
			});
		}
		return MsgErrorCode.Success;
	}

	public bool CheckItem(int itemMasterID, int count)
	{
		int num = 0;
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value != null && inventorySlot.Value.ItemMasterID == itemMasterID)
			{
				if (count == 0)
				{
					return true;
				}
				if (inventorySlot.Value is MiscellanyItemElement)
				{
					num++;
				}
				else if (inventorySlot.Value is ConsumableItemElement consumableItemElement)
				{
					num += consumableItemElement.RemainCount;
				}
				if (num >= count)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool DecreaseItem(int itemMasterID, int count)
	{
		if (!CheckItem(itemMasterID, count))
		{
			return false;
		}
		int num = count;
		foreach (KeyValuePair<int, ItemElement> item in _inventorySlots.ToList())
		{
			if (item.Value == null || item.Value.ItemMasterID != itemMasterID)
			{
				continue;
			}
			if (num > 0)
			{
				if (item.Value is ConsumableItemElement consumableItemElement)
				{
					if (consumableItemElement.RemainCount >= num)
					{
						DecreaseItem(item.Key, consumableItemElement, num);
						num = 0;
						return true;
					}
					num -= consumableItemElement.RemainCount;
					DecreaseItem(item.Key, consumableItemElement, consumableItemElement.RemainCount);
				}
				else if (item.Value is MiscellanyItemElement)
				{
					RemoveInvenItem(item.Key, needToNotiDestroy: false, sync: true);
					num--;
				}
			}
			if (num <= 0)
			{
				return true;
			}
		}
		return false;
	}

	private void DecreaseItem(int slotIndex, ConsumableItemElement consumableItemElement, int count, bool sync = true)
	{
		consumableItemElement.DecreaseCount(count);
		if (consumableItemElement.RemainCount <= 0)
		{
			RemoveInvenItem(slotIndex, needToNotiDestroy: false, sync);
		}
	}

	public bool UseItemByAI()
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return false;
		}
		if (value is ConsumableItemElement)
		{
			return UseItem(0, sync: false) == MsgErrorCode.Success;
		}
		if (value is MiscellanyItemElement)
		{
			return StartScrapMotion(_self.Position, 0, sync: false) == MsgErrorCode.Success;
		}
		if (value is EquipmentItemElement equipmentItemElement)
		{
			return ChangeEquipStatus(equipmentItemElement.TurnOnTime == 0) == MsgErrorCode.Success;
		}
		return false;
	}

	public MsgErrorCode UseItem(int hashCode, bool sync)
	{
		MsgErrorCode msgErrorCode = _self.CanAction(VActorActionType.UseItem);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(value is ConsumableItemElement consumableItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (consumableItemElement.RemainCount <= 0)
		{
			Logger.RError($"InventoryController.UseItem: consumableItemElement.RemainCount is 0. slotIndex: {CurrentInventorySlot}. itemMasterID: {consumableItemElement.ItemMasterID}");
			return MsgErrorCode.ItemNotFound;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(consumableItemElement.ItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		ItemConsumableInfo consumableItemInfo = itemInfo as ItemConsumableInfo;
		if (consumableItemInfo == null)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (consumableItemInfo.ConsumeType != ConsumeItemType.Potion)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		DecreaseItem(CurrentInventorySlot, consumableItemElement, 1, sync: false);
		Dictionary<IGameAction, IGameActionParam> dictionary = new Dictionary<IGameAction, IGameActionParam>();
		ImmutableArray<IGameAction>.Enumerator enumerator = consumableItemInfo.GameActions.GetEnumerator();
		while (enumerator.MoveNext())
		{
			IGameAction current = enumerator.Current;
			dictionary.Add(current.Clone(), new GameActionParamActor(_self.ObjectID));
		}
		_self.VRoom.EnqueueEventAction(dictionary, 0);
		if (consumableItemInfo.SoundAggroPerUse > 0)
		{
			_self.VRoom.IterateAllMonster(delegate(VActor monster)
			{
				monster.AIControlUnit?.AddSoundAggroPoint(_self, consumableItemInfo.SoundAggroPerUse);
			});
		}
		if (sync)
		{
			_self.SendToMe(new UseItemRes(hashCode));
			_self.SendInSight(new UpdateInvenSig
			{
				actorID = _self.ObjectID,
				inventoryInfos = GetInventoryInfos()
			}, includeSelf: true);
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode ChargeItem()
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(value is EquipmentItemElement equipmentItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (!equipmentItemElement.IsChargable())
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (!equipmentItemElement.Charge())
		{
			return MsgErrorCode.CantAction;
		}
		_self.SendInSight(new ChangeEquipStatusSig
		{
			actorID = _self.ObjectID,
			changedItemInfo = equipmentItemElement.toItemInfo()
		}, includeSelf: true);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode ChangeEquipStatus(bool isTurnOn, bool sync = true, int hashCode = 0)
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(value is EquipmentItemElement equipmentItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (equipmentItemElement.PartsType != EquipPartsType.ToggleEquip && equipmentItemElement.PartsType != EquipPartsType.AutoToggleEquip)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if ((isTurnOn && equipmentItemElement.TurnOnTime != 0L) || (!isTurnOn && equipmentItemElement.TurnOnTime == 0L))
		{
			return MsgErrorCode.CantChangeItemTurnStatus;
		}
		if (!equipmentItemElement.ChangeTurnStatus(isTurnOn))
		{
			return MsgErrorCode.CantChangeItemTurnStatus;
		}
		if (isTurnOn && equipmentItemElement.FunctionGroupID != 0)
		{
			bool flag = false;
			foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
			{
				if (inventorySlot.Key != CurrentInventorySlot && inventorySlot.Value != null && inventorySlot.Value is EquipmentItemElement { TurnOnTime: not 0L } equipmentItemElement2 && equipmentItemElement.FunctionGroupID == equipmentItemElement2.FunctionGroupID)
				{
					equipmentItemElement2.ChangeTurnStatus(isTurnOn: false);
					flag = true;
				}
			}
			if (flag && sync)
			{
				_self.SendInSight(new UpdateInvenSig
				{
					actorID = _self.ObjectID,
					inventoryInfos = GetInventoryInfos()
				}, includeSelf: true);
			}
		}
		if (sync)
		{
			_self.SendToMe(new ChangeEquipStatusRes(hashCode)
			{
				currentHandEquipmentInfo = equipmentItemElement.toItemInfo()
			});
		}
		return MsgErrorCode.Success;
	}

	public void OnDead()
	{
		Dictionary<int, ItemElement> dictionary = ExtractAllItemElement(turnOff: true);
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<int, ItemElement> item in dictionary)
			{
				PosWithRot floorPosWithRot = PhysicsUtility.GetFloorPosWithRot(_self.Position);
				floorPosWithRot.x += SimpleRandUtil.Next(-0.5f, 0.5f);
				floorPosWithRot.z += SimpleRandUtil.Next(-0.5f, 0.5f);
				floorPosWithRot.yaw = SimpleRandUtil.Next(0, 360);
				if (_self.VRoom.SpawnLootingObject(item.Value, floorPosWithRot, _self.IsIndoor, ReasonOfSpawn.ActorDying) == 0)
				{
					Logger.RError($"Failed to spawn looting object by releasing item. itemElement: {item}");
				}
			}
		}
		ClearAllInventory();
	}

	public void OnChangeInventory()
	{
		_totalWeight = 0;
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value != null)
			{
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(inventorySlot.Value.ItemMasterID);
				if (itemInfo != null)
				{
					_totalWeight += itemInfo.Weight;
				}
			}
		}
		if (_totalWeight == 0)
		{
			_self.StatControlUnit?.SetMoveSpeedDecreaseRateByWeight(0);
			return;
		}
		double x = Math.Min((double)_totalWeight / (double)Hub.s.dataman.ExcelDataManager.Consts.C_MaxCarryWeight, 1.0);
		double num = 1.0 - (double)Hub.s.dataman.ExcelDataManager.Consts.C_MinThresholdMoveSpeedRate * 0.0001;
		int moveSpeedDecreaseRateByWeight = (int)(Math.Min(Math.Pow(x, 3.0) * num, num) * 10000.0);
		_self.StatControlUnit?.SetMoveSpeedDecreaseRateByWeight(moveSpeedDecreaseRateByWeight);
	}

	public bool IsLightOn()
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return false;
		}
		if (!(value is EquipmentItemElement equipmentItemElement))
		{
			return false;
		}
		if (equipmentItemElement.PartsType != EquipPartsType.ToggleEquip && equipmentItemElement.PartsType != EquipPartsType.AutoToggleEquip)
		{
			return false;
		}
		return equipmentItemElement.TurnOnTime != 0;
	}

	public Dictionary<int, ItemElement> GetAllItemElements()
	{
		return _inventorySlots;
	}

	public bool CloneInven(Dictionary<int, ItemElement> items)
	{
		ClearAllInventory();
		foreach (KeyValuePair<int, ItemElement> item in items)
		{
			if (item.Value != null)
			{
				if (!CanAddItem(item.Value))
				{
					return false;
				}
				ItemElement itemElement = _self.VRoom.CloneNewFakeItemElement(item.Value);
				HandleAddItem(itemElement, out var _);
			}
		}
		return true;
	}

	public string GetDebugString()
	{
		return string.Empty;
	}

	public bool InvenFull()
	{
		if (_inventorySlots.Where<KeyValuePair<int, ItemElement>>((KeyValuePair<int, ItemElement> x) => x.Value != null).Count() >= 4)
		{
			return true;
		}
		return false;
	}

	private void OnAddItemByLooting(ItemElement itemElement)
	{
		if (!itemElement.IsFake && itemElement is EquipmentItemElement equipmentItemElement && Hub.s.dataman.ExcelDataManager.GetItemInfo(itemElement.ItemMasterID) is ItemEquipmentInfo itemEquipmentInfo)
		{
			if (itemEquipmentInfo.MinGaugeWhenLooting != 0 && itemEquipmentInfo.MaxGaugeWhenLooting != 0)
			{
				equipmentItemElement.SetAmount(UnityEngine.Random.Range(itemEquipmentInfo.MinGaugeWhenLooting, itemEquipmentInfo.MaxGaugeWhenLooting + 1));
			}
			if (itemEquipmentInfo.DecDurabilityWhenLooting > 0)
			{
				equipmentItemElement.SetDurability(equipmentItemElement.RemainDurability - itemEquipmentInfo.DecDurabilityWhenLooting);
			}
		}
	}

	public bool HasRechargableItem()
	{
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value != null && inventorySlot.Value is EquipmentItemElement { PartsType: not EquipPartsType.SkillEquip } equipmentItemElement)
			{
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement.ItemMasterID);
				if (itemInfo != null && itemInfo is ItemEquipmentInfo { UseCharge: not false })
				{
					return true;
				}
			}
		}
		return false;
	}

	public MsgErrorCode StartScrapMotion(PosWithRot pos, int hashCode, bool sync)
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (!(value is MiscellanyItemElement miscellanyItemElement))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(miscellanyItemElement.ItemMasterID);
		if (itemInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		ItemMiscellanyInfo itemMiscellanyInfo = itemInfo as ItemMiscellanyInfo;
		if (itemMiscellanyInfo == null)
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		if (string.IsNullOrEmpty(itemMiscellanyInfo.ScrapAnimationStateName))
		{
			return MsgErrorCode.ItemDataMismatch;
		}
		MsgErrorCode num = _self.ScrapMotionController.OnStartScrapMotion(itemMiscellanyInfo.MasterID, itemMiscellanyInfo.ScrapAnimationStateName, itemMiscellanyInfo.IsMovingScrapAnimation, value, pos, hashCode);
		if (num == MsgErrorCode.Success && itemMiscellanyInfo.SoundAggroPerUse > 0)
		{
			_self.VRoom.IterateAllMonster(delegate(VActor monster)
			{
				monster.AIControlUnit?.AddSoundAggroPoint(_self, itemMiscellanyInfo.SoundAggroPerUse);
			});
		}
		return num;
	}

	public MsgErrorCode EndScrapMotion(PosWithRot pos, int hashCode, bool sync)
	{
		ScrapMotionController? scrapMotionController = _self.ScrapMotionController;
		if (scrapMotionController != null && !scrapMotionController.IsPlaying)
		{
			return MsgErrorCode.CantAction;
		}
		return _self.ScrapMotionController.OnEndScrapMotion(pos, hashCode);
	}

	public long GetOnHandtemAggroValue()
	{
		if (_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) && value != null)
		{
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(value.ItemMasterID);
			if (itemInfo == null)
			{
				Logger.RWarn($"[GetOnHandtemAggroValue] itemInfo is null {value.ItemMasterID}");
				return 0L;
			}
			switch (value.ItemType)
			{
			case ItemType.Equipment:
				if (value is EquipmentItemElement { TurnOnTime: not 0L })
				{
					return itemInfo.SoundAggroInHandToggleOnPerTick;
				}
				return itemInfo.SoundAggroInHandPerTick;
			case ItemType.Consumable:
				return itemInfo.SoundAggroInHandPerTick;
			case ItemType.Miscellany:
				if (value.ItemMasterID == _self.ScrapMotionController?.PlayedMiscItemMasterID)
				{
					return itemInfo.SoundAggroInHandToggleOnPerTick;
				}
				return itemInfo.SoundAggroInHandPerTick;
			}
			Logger.RError($"[GetOnHandtemAggroValue] Unknown ItemType {value.ItemType}");
		}
		return 0L;
	}

	public void OnMoveDistance(float distance)
	{
		foreach (ItemElement item in _inventorySlots.Values.Where((ItemElement x) => x != null && !x.IsFake && x is EquipmentItemElement equipmentItemElement2 && equipmentItemElement2.RemainAmount != -1).ToList())
		{
			if (item == null || item.IsFake || !(item is EquipmentItemElement { RemainAmount: not -1 } equipmentItemElement))
			{
				continue;
			}
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(item.ItemMasterID);
			if (itemInfo == null || !(itemInfo is ItemEquipmentInfo { IncGaugePerMove: >0 } itemEquipmentInfo))
			{
				continue;
			}
			int num = (int)(distance * (float)itemEquipmentInfo.IncGaugePerMove);
			if (num > 0)
			{
				equipmentItemElement.IncGauge(num);
				if (equipmentItemElement.NeedToSyncGauge())
				{
					_self.SendInSight(new ChangeEquipStatusSig
					{
						actorID = _self.ObjectID,
						changedItemInfo = equipmentItemElement.toItemInfo()
					}, includeSelf: true);
				}
			}
		}
	}

	public bool IsAnySlotEmpty()
	{
		foreach (KeyValuePair<int, ItemElement> inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Value == null)
			{
				return true;
			}
		}
		return false;
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}

	public int GetCurrentInventorySlotItemMasterID()
	{
		if (!_inventorySlots.TryGetValue(CurrentInventorySlot, out ItemElement value) || value == null)
		{
			return 0;
		}
		return value.ItemMasterID;
	}
}
