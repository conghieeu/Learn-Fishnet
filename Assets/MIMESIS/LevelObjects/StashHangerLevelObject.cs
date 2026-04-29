using System;
using System.Collections.Generic;
using Bifrost.Cooked;
using Mimic;
using Mimic.Actors;
using MoreMountains.Feedbacks;
using ReluProtocol;
using UnityEngine;

public class StashHangerLevelObject : SwitchLevelObject, ITramUpgradeLevelObject
{
	[Serializable]
	private class HighlightInfo
	{
		public Animator animator;

		public string animatorStateName = "First";
	}

	[SerializeField]
	public int index;

	[SerializeField]
	private Transform socketForHangItem;

	[SerializeField]
	private MMF_Player feedbackEffect;

	[Header("L10N Texts")]
	[SerializeField]
	[Tooltip("업그레이드 연출이 진행 중인 경우의 텍스트")]
	private string upgradingStateTextID = "STRING_TRAMUPGRADE_UPGRADING";

	[Header("Upgrade Effects")]
	[SerializeField]
	[Tooltip("업그레이드 효과가 나오고 나서 지정된 시간동안 상호작용이 불가능하게 막는다.")]
	private float upgradeEffectDuration = 4f;

	[SerializeField]
	[Tooltip("업그레이드 효과가 나오기 전에 미리 행거가 접혀 들어가 있도록 유지하는 애니메이션")]
	private string foldedAnimState = "Folded";

	[SerializeField]
	private List<HighlightInfo> highlightInfos;

	private ItemInfo hangItemInfo;

	private GameObject hangItemGo;

	private bool isUpgradeActive;

	private float interactableTime;

	[SerializeField]
	private int tramUpgradeID = -1;

	[SerializeField]
	private string hangMasterAudioKey = string.Empty;

	[SerializeField]
	private string unhangMasterAudioKey = string.Empty;

	public override LevelObjectClientType LevelObjectType => LevelObjectClientType.StashHanger;

	public bool IsUpgradeActive => isUpgradeActive;

	public int TramUpgradeID => tramUpgradeID;

	public override bool ForServer => true;

	public void OnEnable()
	{
		isUpgradeActive = true;
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (Time.time < interactableTime)
		{
			return false;
		}
		InventoryItem selectedInventoryItem = protoActor.GetSelectedInventoryItem();
		if (hangItemInfo == null)
		{
			if (selectedInventoryItem == null)
			{
				return false;
			}
			if (!selectedInventoryItem.MasterInfo.IsVendingMachineExchange)
			{
				return false;
			}
		}
		else
		{
			if (selectedInventoryItem != null && selectedInventoryItem.MasterInfo.ForbidChange)
			{
				return false;
			}
			List<InventoryItem> inventoryItems = protoActor.GetInventoryItems();
			bool flag = false;
			for (int i = 0; i < inventoryItems.Count; i++)
			{
				if (inventoryItems[i] == null)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	protected override void Trigger(ProtoActor protoActor, int newState)
	{
		_ = base.State;
		if (hangItemInfo == null)
		{
			protoActor.TryHangItem(index);
			if (hangMasterAudioKey != string.Empty && Hub.s != null && Hub.s.audioman != null)
			{
				Hub.s.audioman.PlaySfx(hangMasterAudioKey, base.transform.position);
			}
		}
		else
		{
			protoActor.TryUnhangItem(index);
		}
	}

	public void OnHangItem(ItemInfo itemInfo)
	{
		if (hangItemGo != null)
		{
			UnityEngine.Object.Destroy(hangItemGo);
		}
		hangItemInfo = itemInfo;
		hangItemGo = InstantiateItem(itemInfo.itemMasterID);
		if (hangItemGo != null)
		{
			LootingEventFeedbackTrigger[] componentsInChildren = hangItemGo.GetComponentsInChildren<LootingEventFeedbackTrigger>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Trigger(LootingEvent.Release);
			}
		}
		if (feedbackEffect != null)
		{
			feedbackEffect.PlayFeedbacks();
		}
	}

	public void OnUnhangItem()
	{
		if (hangItemGo != null)
		{
			UnityEngine.Object.Destroy(hangItemGo);
		}
		hangItemGo = null;
		hangItemInfo = null;
		if (unhangMasterAudioKey != string.Empty && Hub.s != null && Hub.s.audioman != null)
		{
			Hub.s.audioman.PlaySfx(unhangMasterAudioKey, base.transform.position);
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Time.time < interactableTime)
		{
			return Hub.GetL10NText(upgradingStateTextID);
		}
		_ = string.Empty;
		InventoryItem selectedInventoryItem = protoActor.GetSelectedInventoryItem();
		if (hangItemInfo == null)
		{
			if (selectedInventoryItem == null || selectedInventoryItem.MasterInfo.IsVendingMachineExchange)
			{
				return Hub.GetL10NText("STASH_HANGER_LEVEL_OBJECT_CAN_HANG");
			}
			return Hub.GetL10NText("STASH_HANGER_LEVEL_OBJECT_CANNOT_HANG");
		}
		if (selectedInventoryItem != null && selectedInventoryItem.MasterInfo.ForbidChange)
		{
			return Hub.GetL10NText("STASH_HANGER_LEVEL_OBJECT_CANNOT_TAKE_OUT_TWOHANDS");
		}
		string newValue = string.Empty;
		if (Hub.s != null && Hub.s.dataman != null && Hub.s.dataman.ExcelDataManager != null)
		{
			ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(hangItemInfo.itemMasterID);
			if (itemInfo != null)
			{
				newValue = Hub.GetL10NText(itemInfo.Name);
			}
		}
		return Hub.GetL10NText("STASH_HANGER_LEVEL_OBJECT_TAKE_OUT", hangItemInfo.price.ToString()).Replace("[itemname:]", newValue);
	}

	private GameObject? InstantiateItem(int itemMasterID)
	{
		MMLootingObjectTable.SkinnedItemInfo item = Hub.s.tableman.lootingObject.FindSkinnedItemInfo(itemMasterID).skinnedItemInfo;
		if (item == null)
		{
			Logger.RError($"skinnedItemInfo is null @ InstantiateGameObject: ItemMasterID={itemMasterID}");
			return null;
		}
		GameObject? obj = UnityEngine.Object.Instantiate(item.prefab, Vector3.zero, Quaternion.identity);
		Transform transform = obj.transform;
		string socketName = $"HoldHangerSocket_Pivot{index}";
		Transform transform2 = SocketNodeMarker.FindFirstInHierarchy(transform, socketName);
		if (transform2 != null)
		{
			transform.localPosition = transform2.localPosition;
			transform.localRotation = transform2.localRotation;
			transform.localScale = transform2.localScale;
		}
		transform.SetParent(socketForHangItem, worldPositionStays: false);
		return obj;
	}

	public bool IsScrapHanging()
	{
		if (hangItemGo != null)
		{
			return hangItemInfo != null;
		}
		return false;
	}

	public void PrepareUpgradeEffect()
	{
		if (animator != null)
		{
			animator.Play(foldedAnimState);
		}
	}

	public void PlayUpgradeEffect()
	{
		foreach (HighlightInfo highlightInfo in highlightInfos)
		{
			if (highlightInfo.animator != null)
			{
				highlightInfo.animator.Play(highlightInfo.animatorStateName);
			}
		}
		interactableTime = Time.time + upgradeEffectDuration;
	}
}
