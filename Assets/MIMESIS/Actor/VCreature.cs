using System.Collections.Generic;
using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public abstract class VCreature : VActor
{
	protected long _dyingWaitTime;

	protected long _spawningWaitTime;

	protected int _recoverWaitTime;

	protected long _spawningTime;

	protected long _dyingTime;

	public VCreatureLifeCycle LifeCycle;

	protected PassiveStateMachine<VCreatureLifeCycle, VCreatureEvent> _passiveStateMachine;

	public long SpawnWaitTime => _spawningWaitTime;

	public long DyingWaitTime => _dyingWaitTime;

	public ReasonOfDeath ReasonOfDeath { get; protected set; }

	public string PrefabName { get; protected set; }

	public IHitCheck HitCheck { get; protected set; }

	public bool IsEnableFallPath { get; private set; }

	public OccupiedLevelObjectInfo? OccupiedLevelObjectInfo { get; private set; }

	public VCreature(ActorType actorType, int actorID, int masterID, string actorName, PosWithRot spawnPosition, bool isIndoor, IVroom room, long UID, ReasonOfSpawn reasonOfSpawn)
		: base(actorType, actorID, masterID, actorName, spawnPosition, isIndoor, room, UID, reasonOfSpawn)
	{
		StateMachineDefinitionBuilder<VCreatureLifeCycle, VCreatureEvent> stateMachineDefinitionBuilder = new StateMachineDefinitionBuilder<VCreatureLifeCycle, VCreatureEvent>();
		stateMachineDefinitionBuilder.In(VCreatureLifeCycle.Ready).On(VCreatureEvent.Spawn).Goto(VCreatureLifeCycle.Alive);
		stateMachineDefinitionBuilder.In(VCreatureLifeCycle.Alive).ExecuteOnEntry(OnAlive).On(VCreatureEvent.Dying)
			.Goto(VCreatureLifeCycle.Dying)
			.On(VCreatureEvent.ForceDying)
			.Goto(VCreatureLifeCycle.ForceDying);
		stateMachineDefinitionBuilder.In(VCreatureLifeCycle.Dying).ExecuteOnEntry<ActorDyingSig>(OnDying).On(VCreatureEvent.Dead)
			.Goto(VCreatureLifeCycle.Dead);
		stateMachineDefinitionBuilder.In(VCreatureLifeCycle.ForceDying).ExecuteOnEntry(OnForceDying).On(VCreatureEvent.Dead)
			.Goto(VCreatureLifeCycle.Dead);
		stateMachineDefinitionBuilder.In(VCreatureLifeCycle.Dead).ExecuteOnEntry(OnDead).On(VCreatureEvent.Rescue)
			.Goto(VCreatureLifeCycle.Alive)
			.On(VCreatureEvent.Revive)
			.Goto(VCreatureLifeCycle.Alive);
		stateMachineDefinitionBuilder.WithInitialState(VCreatureLifeCycle.Ready);
		_passiveStateMachine = stateMachineDefinitionBuilder.Build().CreatePassiveStateMachine();
		_passiveStateMachine.Start();
	}

	protected virtual void InitController()
	{
		_controllerManager.AddController(new StatController(this));
		_controllerManager.AddController(new MovementController(this));
		_controllerManager.AddController(new SkillController(this));
		_controllerManager.AddController(new CooltimeController(this));
		_controllerManager.AddController(new InventoryController(this));
		_controllerManager.AddController(new AbnormalController(this));
		_controllerManager.AddController(new AttachController(this));
		_controllerManager.AddController(new EmotionController(this));
		_controllerManager.AddController(new AuraController(this));
		_controllerManager.AddController(new ScrapMotionController(this));
		_controllerManager.Initialize();
		InitDefaultParam();
		_spawningTime = Hub.s.timeutil.GetCurrentTickMilliSec();
	}

	protected abstract void InitDefaultParam();

	public virtual void OnAlive()
	{
		LifeCycle = VCreatureLifeCycle.Alive;
	}

	public void OnDying(ActorDyingSig sig)
	{
		LifeCycle = VCreatureLifeCycle.Dying;
		SendInSight(sig, includeSelf: true);
		VRoom.OnActorEvent(new GameActorDeadEventArgs(sig.attackerActorID, this));
		AddGameEventLog(new GELActorDead
		{
			RoomSessionID = VRoom.SessionID,
			SessionCycleCount = VRoom.SessionCycleCount,
			StageCount = VRoom.CurrentDay,
			ActorType = ActorType,
			reason = ReasonOfDeath,
			SkillMasterID = sig.linkedMasterID,
			Timestamp = Hub.s.timeutil.GetTimestamp()
		});
		ResetControllerByDying();
		_dyingTime = Hub.s.timeutil.GetCurrentTickMilliSec();
	}

	protected virtual void OnForceDying()
	{
		LifeCycle = VCreatureLifeCycle.ForceDying;
		ResetControllerByDying();
		VRoom.OnActorEvent(new GameActorDeadEventArgs(0, this));
		_dyingTime = Hub.s.timeutil.GetCurrentTickMilliSec();
		SendInSight(new ActorDyingSig
		{
			actorID = base.ObjectID,
			attackerActorID = 0,
			linkedMasterID = 0,
			reasonOfDeath = ReasonOfDeath.AdminKill
		}, includeSelf: true);
	}

	public virtual void OnDead()
	{
		LifeCycle = VCreatureLifeCycle.Dead;
	}

	public void OnGroggyExit()
	{
		SendToChannel(new GroggyStateSig
		{
			actorID = base.ObjectID,
			state = GroggyState.Normal
		});
	}

	public override void Update(long elapsed)
	{
		if (LifeCycle == VCreatureLifeCycle.Ready)
		{
			if (_spawningWaitTime > 0)
			{
				if (Hub.s.timeutil.GetCurrentTickMilliSec() - _spawningTime > _spawningWaitTime)
				{
					_passiveStateMachine.Fire(VCreatureEvent.Spawn);
				}
			}
			else
			{
				_passiveStateMachine.Fire(VCreatureEvent.Spawn);
			}
		}
		else if ((LifeCycle == VCreatureLifeCycle.Dying || LifeCycle == VCreatureLifeCycle.ForceDying) && Hub.s.timeutil.GetCurrentTickMilliSec() - _dyingTime > _dyingWaitTime)
		{
			_passiveStateMachine.Fire(VCreatureEvent.Dead);
		}
		base.Update(elapsed);
	}

	public override bool IsAliveStatus()
	{
		return LifeCycle == VCreatureLifeCycle.Alive;
	}

	public virtual MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		switch (actionType)
		{
		case VActorActionType.MoveSkill:
		case VActorActionType.Skill:
		case VActorActionType.UseItem:
		case VActorActionType.Move:
		case VActorActionType.Looting:
		case VActorActionType.Emotion:
		case VActorActionType.ScrapMotion:
		case VActorActionType.Jump:
		case VActorActionType.UseLevelObject:
			if (!IsAliveStatus())
			{
				return MsgErrorCode.ActorisDead;
			}
			if (this is VPlayer { LevelLoadCompleted: false })
			{
				return MsgErrorCode.CantAction;
			}
			break;
		}
		return _controllerManager.CanAction(actionType);
	}

	public void SetInvincible(long periodMillisec)
	{
		VRoom.AddEventTimer(delegate
		{
			SetVisible();
		}, periodMillisec);
		SetVisible(visible: false);
	}

	public void SyncChangePhase()
	{
		SendInSight(new ActorChangePhaseNoti
		{
			actorID = base.ObjectID,
			PhaseID = base.AIControlUnit.PhaseID
		});
	}

	public void SendDebugBTStateInfo()
	{
		SendInSight(new DebugBTStateInfoSig
		{
			actorID = base.ObjectID,
			aiDataName = base.AIControlUnit.AIDataName,
			templateName = base.AIControlUnit.CurrentBTTemplateName
		});
	}

	public override void MoveToDying(ApplyDamageArgs args)
	{
		if (LifeCycle == VCreatureLifeCycle.Alive)
		{
			ReasonOfDeath = VWorldUtil.ConvertMutableChangeCause(args.MutableStatChangeCause);
			_passiveStateMachine.Fire(VCreatureEvent.Dying, new ActorDyingSig
			{
				actorID = base.ObjectID,
				attackerActorID = (args.Attacker?.ObjectID ?? 0),
				linkedMasterID = args.SkillMasterID,
				reasonOfDeath = ReasonOfDeath
			});
		}
	}

	public void ForcedDying()
	{
		if (LifeCycle == VCreatureLifeCycle.Alive)
		{
			ReasonOfDeath = ReasonOfDeath.AdminKill;
			_passiveStateMachine.Fire(VCreatureEvent.ForceDying);
		}
	}

	protected virtual void GetOtherActorInfo(ref OtherCreatureInfo info)
	{
		ActorBaseInfo info2 = info;
		GetActorBaseInfo(ref info2);
		info.actorLifeCycle = LifeCycle;
		Dictionary<int, ItemInfo> info3 = info.inventories;
		base.InventoryControlUnit.GetInventoryInfo(ref info3);
		StatCollection statCollection = info.statInfoCollection;
		if (!base.StatControlUnit.GetStatCollection(ref statCollection))
		{
			Logger.RError("GetStatCollection failed");
		}
		foreach (KeyValuePair<int, HashSet<int>> faction in m_Factions)
		{
			faction.Deconstruct(out var _, out var value);
			foreach (int item in value)
			{
				info.factions.Add(item);
			}
		}
		info.activatedAuraMasterIDs = base.AuraControlUnit?.GetActivatedAuras();
		info.onHandItem = base.InventoryControlUnit.GetEquipInfo();
	}

	protected virtual void ResetControllerByDying()
	{
		base.SkillControlUnit?.CancelSkill(SkillCancelType.All, 0L);
		base.MovementControlUnit?.StopMove();
		base.AIControlUnit?.OnDeath();
		base.InventoryControlUnit?.OnDead();
		base.AttachControlUnit?.OnDead();
		base.AbnormalControlUnit?.OnDeath();
		base.EmotionControlUnit?.OnDead();
		base.AuraControlUnit?.OnDead();
		base.ScrapMotionController?.OnDead();
	}

	public void Kill()
	{
		base.StatControlUnit.ApplyDamage(new ApplyDamageArgs(this, this, MutableStatChangeCause.SystemNormal, base.StatControlUnit.GetCurrentHP(), 0L, 0, 0, HitType.Attack));
	}

	public MsgErrorCode HandleReleaseItem(int hashCode, int targetSlotNum = 0, bool releaseByCC = false)
	{
		MsgErrorCode msgErrorCode = CanAction(VActorActionType.ReleaseItem);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		Vector3 reachableDistancePos = VRoom.GetReachableDistancePos(base.PositionVector, _currentPosition.yaw, 1f);
		if (reachableDistancePos == Vector3.zero)
		{
			Logger.RError($"Failed to find a valid position for releasing item. actorID: {base.ObjectID}, targetSlotNum: {targetSlotNum}");
			return MsgErrorCode.InvalidPosition;
		}
		ItemElement itemElement = base.InventoryControlUnit.HandleExtractItem(targetSlotNum);
		if (itemElement == null)
		{
			return MsgErrorCode.ItemNotFound;
		}
		if (itemElement.ReserveDestroy)
		{
			Logger.RWarn($"Skip spawn looting object because releasing item reservedestroy was on. itemElement: {itemElement}");
		}
		else
		{
			PosWithRot posWithRot = reachableDistancePos.toPosWithRot(0f);
			posWithRot.yaw = _currentPosition.yaw - 180f;
			if (VRoom.SpawnLootingObject(itemElement, posWithRot, _isIndoor, releaseByCC ? ReasonOfSpawn.CC : ReasonOfSpawn.Release) == 0)
			{
				Logger.RError($"Failed to spawn looting object by releasing item. itemElement: {itemElement}");
			}
			if (itemElement.NeedCheckReleaseOnStartingVolume && this is VPlayer actor && VRoom.CheckStartingArea(actor))
			{
				itemElement.NeedCheckReleaseOnStartingVolume = false;
				VRoom.IncreaseItemCarryCount(this);
			}
		}
		SendToMe(new ReleaseItemRes(hashCode)
		{
			inventoryInfos = base.InventoryControlUnit.GetInventoryInfos(),
			currentInvenSlotIndex = base.InventoryControlUnit.GetCurrentInvenSlotIndex()
		});
		return MsgErrorCode.Success;
	}

	public bool SetFallPath(bool enable)
	{
		if (IsEnableFallPath == enable)
		{
			return false;
		}
		IsEnableFallPath = enable;
		return true;
	}

	public bool SetOccupiedLevelObjectInfo(OccupiedLevelObjectInfo? info)
	{
		if (OccupiedLevelObjectInfo == info)
		{
			return false;
		}
		OccupiedLevelObjectInfo = info;
		return true;
	}
}
