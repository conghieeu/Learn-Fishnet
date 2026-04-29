using System.Collections.Generic;
using Bifrost.Cooked;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class VMonster : VCreature
{
	private string _overrideAIName;

	private string _overrideBTName;

	private MonsterInfo _monsterInfo;

	public VMonster(int masterID, int actorID, string actorName, PosWithRot position, bool isIndoor, IVroom room, string aiName = "", string btName = "", ReasonOfSpawn reasonOfSpawn = ReasonOfSpawn.Spawn)
		: base(ActorType.Monster, actorID, masterID, actorName, position, isIndoor, room, 0L, reasonOfSpawn)
	{
		MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(MasterID);
		if (monsterInfo == null)
		{
			Logger.RError("MonsterInfo is null. MasterID : " + MasterID);
			return;
		}
		_monsterInfo = monsterInfo;
		foreach (int faction in _monsterInfo.Factions)
		{
			m_DefaultFactions.Add(faction);
		}
		ResetFaction();
		if (!string.IsNullOrEmpty(aiName))
		{
			_overrideAIName = aiName;
		}
		if (!string.IsNullOrEmpty(btName))
		{
			_overrideBTName = btName;
		}
		InitController();
		base.PrefabName = monsterInfo.PuppetName;
		base.MoveCollisionRadius = monsterInfo.MoveCollisionRadius;
		base.HitCollisionRadius = monsterInfo.HitCollisionRadius;
		_spawningWaitTime = monsterInfo.SpawningWaitTime;
		_dyingWaitTime = monsterInfo.DyingWaittime;
		IHitCheck hurtBox = Hub.s.dataman.AnimNotiManager.GetHurtBox(base.PrefabName);
		if (hurtBox == null)
		{
			Logger.RError("HitCheck is null for monster prefab: " + base.PrefabName);
		}
		else
		{
			base.HitCheck = hurtBox;
		}
		PosWithRot eyeRaycastPos = Hub.s.dataman.AnimNotiManager.GetEyeRaycastPos(base.PrefabName);
		if (eyeRaycastPos == null)
		{
			base.Height = ((hurtBox != null && base.HitCheck is CapsuleHitCheck capsuleHitCheck) ? capsuleHitCheck.Length : 1.5f);
		}
		else
		{
			base.Height = eyeRaycastPos.y;
		}
	}

	protected override void InitController()
	{
		_controllerManager.AddController(new AIController(this));
		base.InitController();
	}

	protected override void InitDefaultParam()
	{
		if (string.IsNullOrEmpty(_overrideAIName))
		{
			base.AIControlUnit?.SetAiData(_monsterInfo.BTName);
		}
		else
		{
			base.AIControlUnit?.SetAiData(_overrideAIName, _overrideBTName);
		}
		base.AIControlUnit?.CreateAI();
	}

	public override void OnAlive()
	{
		base.OnAlive();
		if (_monsterInfo.AbnormalMasterIDOnSpawn != 0)
		{
			base.AbnormalControlUnit?.AppendAbnormal(base.ObjectID, _monsterInfo.AbnormalMasterIDOnSpawn, 0, ignoreImmuneCheck: true, 0, AbnormalReason.System);
		}
	}

	public override void FillSightInSig(ref SightInSig sig)
	{
		OtherCreatureInfo info = new OtherCreatureInfo();
		GetOtherActorInfo(ref info);
		sig.monsterInfos.Add(info);
	}

	public override SendResult SendToMe(IMsg msg)
	{
		return SendResult.Success;
	}

	public override void Update(long elapsed)
	{
		if (LifeCycle == VCreatureLifeCycle.Dead && Hub.s.timeutil.GetCurrentTickMilliSec() - _dyingTime > _dyingWaitTime)
		{
			VRoom.PendRemoveActor(base.ObjectID);
		}
		else
		{
			base.Update(elapsed);
		}
	}

	public void OnDamaged()
	{
	}

	public override void MoveToDying(ApplyDamageArgs args)
	{
		base.MoveToDying(args);
	}

	public override void CollectDebugInfo(ref DebugInfoSig sig)
	{
		sig.debugInfo = _controllerManager.GetDebugInfo();
		CollectHitCheckInfo(base.HitCheck, ref sig);
	}

	public override void OnDead()
	{
		base.OnDead();
		if (_monsterInfo.ItemDropMasterID == 0)
		{
			return;
		}
		ItemDropInfo itemDropInfo = Hub.s.dataman.ExcelDataManager.GetItemDropInfo(_monsterInfo.ItemDropMasterID);
		if (itemDropInfo == null)
		{
			return;
		}
		List<int> dropItemList = itemDropInfo.GetDropItemList();
		if (dropItemList.Count == 0 || !VRoom.FindNearestPoly(base.PositionVector, out var nearestPos))
		{
			return;
		}
		foreach (int item in dropItemList)
		{
			ItemElement newItemElement = VRoom.GetNewItemElement(item, isFake: false);
			if (newItemElement == null)
			{
				Logger.RWarn($"Failed to create item element for itemMasterID: {item}");
			}
			else if (VRoom.SpawnLootingObject(newItemElement, nearestPos.toPosWithRot(0f), _isIndoor, ReasonOfSpawn.ActorDying) == 0)
			{
				Logger.RWarn($"Failed to spawn looting object for itemMasterID: {item}");
			}
		}
	}
}
