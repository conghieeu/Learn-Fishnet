using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class VLootingObject : VActor
{
	private readonly ItemElement _itemElement;

	private bool _isAssigned;

	public bool Assigned => _isAssigned;

	public VLootingObject(int actorID, PosWithRot position, bool isIndoor, ItemElement element, IVroom room, ReasonOfSpawn reasonOfSpawn)
		: base(ActorType.LootingObject, actorID, element.ItemMasterID, element.Name, position, isIndoor, room, 0L, reasonOfSpawn)
	{
		_itemElement = element;
		if (reasonOfSpawn == ReasonOfSpawn.Spawn || reasonOfSpawn == ReasonOfSpawn.Admin)
		{
			_itemElement.NeedCheckReleaseOnStartingVolume = true;
		}
	}

	public override void FillSightInSig(ref SightInSig sig)
	{
		LootingObjectInfo info = new LootingObjectInfo();
		GetInfo(ref info);
		sig.lootingObjectInfos.Add(info);
	}

	public override bool IsAliveStatus()
	{
		if (_itemElement == null)
		{
			return false;
		}
		return true;
	}

	public override SendResult SendToMe(IMsg msg)
	{
		return SendResult.Success;
	}

	public ItemElement GetItemElement()
	{
		return _itemElement;
	}

	protected void GetInfo(ref LootingObjectInfo info)
	{
		ActorBaseInfo info2 = info;
		GetActorBaseInfo(ref info2);
		info.linkedItemInfo = _itemElement.toItemInfo();
	}

	public bool IsFake()
	{
		return _itemElement.IsFake;
	}

	public override void CollectDebugInfo(ref DebugInfoSig sig)
	{
		DebugInfoSig obj = sig;
		obj.debugInfo = obj.debugInfo + "ItemElement : " + _itemElement.ToString();
	}

	public void SetAssigned()
	{
		_isAssigned = true;
	}

	public override void Update(long deltaTick)
	{
		base.Update(deltaTick);
		if (_itemElement is EquipmentItemElement { PartsType: EquipPartsType.AutoReverseToggleEquip, TurnOnTime: not 0L } equipmentItemElement && equipmentItemElement.CheckElapsed() && Hub.s.dataman.ExcelDataManager.GetItemInfo(equipmentItemElement.ItemMasterID) is ItemEquipmentInfo itemEquipmentInfo)
		{
			equipmentItemElement.SetAmount(equipmentItemElement.RemainAmount - itemEquipmentInfo.DecGaugePerUse);
			if (equipmentItemElement.RemainAmount <= 0)
			{
				SendInSight(new DestroyActorSig
				{
					actorID = base.ObjectID
				});
				VRoom.PendRemoveActor(base.ObjectID);
			}
		}
		if (!(VRoom is VWaitingRoom) && _itemElement.IsItemCheckSpawnFieldSkill)
		{
			_itemElement.CheckSpawnFieldSkill(VRoom, this, deltaTick);
		}
	}

	public void CheckBlackoutByItem(int ownerActorID)
	{
		if ((ReasonOfSpawn == ReasonOfSpawn.Spawn || ReasonOfSpawn == ReasonOfSpawn.Admin) && _itemElement.IsItemCheckBlackout && VRoom is DungeonRoom dungeonRoom && Hub.s.dataman.ExcelDataManager.GetItemInfo(_itemElement.ItemMasterID) is ItemEquipmentInfo itemEquipmentInfo)
		{
			dungeonRoom.CheckBlackout(needCheckNewBlackout: true, itemEquipmentInfo.BlackoutRate, ReasonOfBlackout.Item, ownerActorID);
		}
	}

	public override void OnEnterSpace(VSpace space)
	{
		base.OnEnterSpace(space);
		if (!(VRoom is VWaitingRoom) && _itemElement.IsItemCheckSpawnFieldSkill)
		{
			_itemElement.CheckSpawnFieldSkillWaitPeriod(VRoom, this);
		}
	}
}
