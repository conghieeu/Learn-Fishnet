using System;
using System.Collections.Generic;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using Mimic.Character;
using ReluProtocol;
using UnityEngine;

namespace Mimic
{
	public class InventoryItem
	{
		[Flags]
		public enum ItemChangedValueFlags
		{
			None = 0,
			StackCount = 1,
			Durability = 2,
			RemainGauge = 4,
			IsTurnOn = 8,
			All = -1
		}

		public ItemChangedValueFlags ChangedValueFlags { get; private set; }

		public ItemMasterInfo MasterInfo { get; }

		public int ItemMasterID { get; }

		public long ItemID { get; }

		public string LootingObjectID { get; }

		public Transform? Transform { get; private set; }

		public bool IsWaitingEvent { get; private set; }

		public ItemType ItemType { get; private set; }

		public int StackCount { get; private set; }

		public int Durability { get; private set; }

		public int RemainGauge { get; private set; }

		public bool IsTurnOn { get; private set; }

		public bool IsFake { get; private set; }

		public int Price { get; private set; }

		public int AccessoryGroup
		{
			get
			{
				if (!(MasterInfo is ItemMiscellanyInfo itemMiscellanyInfo))
				{
					return 0;
				}
				return itemMiscellanyInfo.AccessoryGroup;
			}
		}

		public bool IsAccessory => AccessoryGroup != 0;

		public bool IsHideMyCamera
		{
			get
			{
				if (!(MasterInfo is ItemMiscellanyInfo itemMiscellanyInfo))
				{
					return false;
				}
				return itemMiscellanyInfo.IsHideMyCamera;
			}
		}

		public bool IsReinforceable
		{
			get
			{
				if (!(MasterInfo is ItemEquipmentInfo itemEquipmentInfo))
				{
					return false;
				}
				return itemEquipmentInfo.CanUpgrade;
			}
		}

		public int ReinforceCost
		{
			get
			{
				if (!(MasterInfo is ItemEquipmentInfo itemEquipmentInfo))
				{
					return 0;
				}
				return itemEquipmentInfo.UpgradeCost;
			}
		}

		public static InventoryItem? Create(ItemInfo? info)
		{
			if (info == null)
			{
				return null;
			}
			ItemMasterInfo masterInfo = GetMasterInfo(info.itemMasterID);
			if (masterInfo == null)
			{
				return null;
			}
			return new InventoryItem(info, masterInfo);
		}

		private InventoryItem(ItemInfo info, ItemMasterInfo masterInfo)
		{
			MasterInfo = masterInfo;
			ItemMasterID = info.itemMasterID;
			ItemID = info.itemID;
			LootingObjectID = masterInfo.LootingObjectID;
			Transform = null;
			IsWaitingEvent = false;
			UpdateInfo(info);
		}

		public void UpdateInfo(ItemInfo info)
		{
			if (ItemMasterID == info.itemMasterID && ItemID == info.itemID)
			{
				CheckAndMarkChangedValueFlags(info);
				ItemType = info.itemType;
				StackCount = info.stackCount;
				Durability = info.durability;
				RemainGauge = info.remainGauge;
				IsTurnOn = info.isTurnOn;
				IsFake = info.isFake;
				Price = info.price;
			}
			else
			{
				Logger.RError($"Cannot update InventoryItem with different ItemMasterID or ItemID. Current: {ItemMasterID}, {ItemID} | New: {info.itemMasterID}, {info.itemID}");
			}
		}

		public void UpdateWaitEvent(bool waitEvent)
		{
			IsWaitingEvent = waitEvent;
		}

		public GameObject? InstantiateGameObject()
		{
			MMLootingObjectTable.SkinnedItemInfo item = Hub.s.tableman.lootingObject.FindSkinnedItemInfo(ItemMasterID).skinnedItemInfo;
			if (item == null)
			{
				Logger.RError($"skinnedItemInfo is null @ InstantiateGameObject: ItemMasterID={ItemMasterID}");
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(item.prefab, Vector3.zero, Quaternion.identity);
			Transform = gameObject.transform;
			MarkAllChangedValueFlags();
			return gameObject;
		}

		public void DestroyGameObject()
		{
			if (Transform != null)
			{
				Hub.s.residualObject.PreserveAllInChildren(Transform);
				Transform.gameObject.SetActive(value: false);
				UnityEngine.Object.Destroy(Transform.gameObject);
				Transform = null;
				ResetChangedValueFlags();
			}
		}

		public void SetPersonViewMode(PersonViewMode mode)
		{
			switch (mode)
			{
			case PersonViewMode.First:
				if (IsHideMyCamera && Transform != null)
				{
					RendererPersonViewModeSwitcher rendererPersonViewModeSwitcher = Transform.GetComponent<RendererPersonViewModeSwitcher>();
					if (rendererPersonViewModeSwitcher == null)
					{
						rendererPersonViewModeSwitcher = Transform.gameObject.AddComponent<RendererPersonViewModeSwitcher>();
					}
					rendererPersonViewModeSwitcher.SwitchMode(PersonViewMode.First);
				}
				break;
			default:
				Logger.RError($"Invalid PersonViewMode: {mode}");
				break;
			case PersonViewMode.None:
			case PersonViewMode.Third:
				break;
			}
		}

		public bool TryCastMasterInfo<T>(out T? masterInfo) where T : ItemMasterInfo
		{
			masterInfo = MasterInfo as T;
			return masterInfo != null;
		}

		public bool TryGetComponent<T>(out T? component) where T : Component
		{
			component = null;
			if (Transform != null)
			{
				return Transform.TryGetComponent<T>(out component);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as InventoryItem);
		}

		public bool Equals(InventoryItem? other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (ItemMasterID == other.ItemMasterID)
			{
				return ItemID == other.ItemID;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ItemMasterID, ItemID);
		}

		private static ItemMasterInfo? GetMasterInfo(int masterID)
		{
			if (Hub.s != null && Hub.s.dataman != null && Hub.s.dataman.ExcelDataManager != null)
			{
				return Hub.s.dataman.ExcelDataManager.GetItemInfo(masterID);
			}
			return null;
		}

		public void ResetChangedValueFlags()
		{
			ChangedValueFlags = ItemChangedValueFlags.None;
		}

		public void MarkAllChangedValueFlags()
		{
			ChangedValueFlags = ItemChangedValueFlags.All;
		}

		private void CheckAndMarkChangedValueFlags(ItemInfo info)
		{
			if (!(Transform == null))
			{
				CheckAndMarkFlag(StackCount, info.stackCount, ItemChangedValueFlags.StackCount);
				CheckAndMarkFlag(Durability, info.durability, ItemChangedValueFlags.Durability);
				CheckAndMarkFlag(RemainGauge, info.remainGauge, ItemChangedValueFlags.RemainGauge);
				CheckAndMarkFlag(IsTurnOn, info.isTurnOn, ItemChangedValueFlags.IsTurnOn);
			}
		}

		private void CheckAndMarkFlag<T>(T currentValue, T newValue, ItemChangedValueFlags flag) where T : IEquatable<T>
		{
			if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
			{
				ChangedValueFlags |= flag;
			}
		}
	}
}
