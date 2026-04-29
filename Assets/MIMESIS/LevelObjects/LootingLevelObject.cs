using Bifrost.Cooked;
using Mimic;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class LootingLevelObject : LevelObject, Mimic.Actors.IActor
{
	public int itemMasterID;

	public int marketPrice;

	public bool isFake;

	public ActorType ActorType { get; } = ActorType.LootingObject;

	public int ActorID { get; set; }

	public override LevelObjectClientType LevelObjectType => LevelObjectClientType.Loot;

	public void Start()
	{
		base.crossHairType = CrosshairType.Scrap;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_pricetag", allowScaling: true, iconColor);
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (!CanGrap(protoActor))
		{
			return false;
		}
		protoActor.GrapLootingObject(ActorID);
		return true;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		ItemMasterInfo itemMasterInfo = ProtoActor.Inventory.GetItemMasterInfo(itemMasterID);
		if (itemMasterInfo == null)
		{
			return "";
		}
		if (!CanGrap(protoActor))
		{
			return Hub.GetL10NText("STRING_DO_NOT_LOOTING");
		}
		string text = $"${marketPrice,5}";
		return Hub.GetL10NText("STRING_ITEM_PICKUP").Replace("[itemname:]", Hub.GetL10NText(itemMasterInfo.Name)) + text;
	}

	public virtual void OnSpawn(ReasonOfSpawn reason)
	{
		if (reason == ReasonOfSpawn.Release)
		{
			LootingEventFeedbackTrigger[] componentsInChildren = GetComponentsInChildren<LootingEventFeedbackTrigger>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Trigger(LootingEvent.Release);
			}
		}
	}

	public virtual void OnDespawnByDestroy()
	{
	}

	private bool CanGrap(ProtoActor protoActor)
	{
		InventoryItem selectedInventoryItem = protoActor.GetSelectedInventoryItem();
		if (selectedInventoryItem != null)
		{
			return !selectedInventoryItem.MasterInfo.ForbidChange;
		}
		return true;
	}

	public bool IsVendingMachineExchange()
	{
		return ProtoActor.Inventory.GetItemMasterInfo(itemMasterID)?.IsVendingMachineExchange ?? false;
	}
}
