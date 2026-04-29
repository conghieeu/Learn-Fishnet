using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public sealed class SkillController : IVActorController, IDisposable
{
	private VCreature _self;

	private ISkillContext? _currentCtx;

	private ISkillContext? _prevCtx;

	private Dictionary<long, ISkillContext> _pendingDeleteSkillContexts;

	private Dictionary<long, long> _syncExpiredTime;

	private bool IgnoreCoolTime;

	private HashSet<int> _skillSlots = new HashSet<int>();

	private List<int> _defaultSkillList = new List<int>();

	public VActorControllerType type => VActorControllerType.Skill;

	public SkillController(VActor self)
	{
		if (self is VCreature self2)
		{
			_self = self2;
			_pendingDeleteSkillContexts = new Dictionary<long, ISkillContext>();
			_syncExpiredTime = new Dictionary<long, long>();
			if (!(self is VMonster))
			{
				return;
			}
			MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(self.MasterID);
			if (monsterInfo == null)
			{
				throw new ArgumentException("MonsterInfo is null");
			}
			_defaultSkillList.AddRange(monsterInfo.SkillList);
			{
				foreach (int defaultSkill in _defaultSkillList)
				{
					_skillSlots.Add(defaultSkill);
				}
				return;
			}
		}
		throw new ArgumentException("VActor is not VCreature");
	}

	public void Initialize()
	{
		IgnoreCoolTime = false;
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		switch (actionType)
		{
		case VActorActionType.Move:
			if (IsUsingSkillNow())
			{
				ISkillContext? currentCtx = _currentCtx;
				if (currentCtx != null && !currentCtx.SkillInfo.AllowMove)
				{
					return MsgErrorCode.CantActionByUsingSkill;
				}
			}
			return MsgErrorCode.Success;
		case VActorActionType.ChangeInvenSlot:
		case VActorActionType.UseItem:
		case VActorActionType.Looting:
		case VActorActionType.Emotion:
		case VActorActionType.ScrapMotion:
		case VActorActionType.Jump:
		case VActorActionType.UseLevelObject:
		case VActorActionType.ReleaseItem:
			if (!IsUsingSkillNow())
			{
				return MsgErrorCode.Success;
			}
			return MsgErrorCode.CantActionByUsingSkill;
		default:
			return MsgErrorCode.Success;
		}
	}

	public void Dispose()
	{
		CancelSkill(SkillCancelType.All, 0L);
	}

	public void Update(long deltaTime)
	{
		_currentCtx?.Update(deltaTime);
		ISkillContext? currentCtx = _currentCtx;
		if (currentCtx != null && currentCtx.Status == SkillContextStatus.Vanishing)
		{
			_prevCtx = _currentCtx;
			_currentCtx = null;
		}
		ISkillContext? prevCtx = _prevCtx;
		if (prevCtx != null && prevCtx.Status == SkillContextStatus.Terminated)
		{
			_prevCtx.Dispose();
			_prevCtx = null;
		}
		RemoveExpiredSkillContext();
	}

	public bool IsCancelableCurrentSkill(int skillMasterID)
	{
		if (_currentCtx == null)
		{
			return true;
		}
		return _currentCtx.cancelableSkillMasterIDs.Contains(skillMasterID);
	}

	public void WaitInitDone()
	{
	}

	private void RemoveExpiredSkillContext()
	{
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		List<long> list = new List<long>();
		foreach (KeyValuePair<long, ISkillContext> pendingDeleteSkillContext in _pendingDeleteSkillContexts)
		{
			if (pendingDeleteSkillContext.Value.ExpiredTick <= currentTickMilliSec)
			{
				list.Add(pendingDeleteSkillContext.Key);
			}
		}
		foreach (long item in list)
		{
			_pendingDeleteSkillContexts.Remove(item);
		}
		list.Clear();
		foreach (KeyValuePair<long, long> item2 in _syncExpiredTime)
		{
			if (item2.Value <= currentTickMilliSec)
			{
				list.Add(item2.Key);
			}
		}
		foreach (long item3 in list)
		{
			_syncExpiredTime.Remove(item3);
		}
	}

	public long GetNewSkillSyncID()
	{
		return _self.VRoom.GetNewSkillSyncID();
	}

	public MsgErrorCode CanUseSkill(SkillInfo skillInfo, SkillFlags flags = SkillFlags.None)
	{
		if ((flags & SkillFlags.IgnoreCoolTime) == 0 && !CanUseSkillByCooltime(skillInfo.MasterID))
		{
			return MsgErrorCode.SkillCooltime;
		}
		return MsgErrorCode.Success;
	}

	private (MsgErrorCode, ISkillContext?) PrepareSkillContext(UseSkillReq req, SkillFlags flags = SkillFlags.None)
	{
		MsgErrorCode msgErrorCode = MsgErrorCode.Success;
		int skillMasterID = req.skillMasterID;
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
		if (skillInfo == null)
		{
			return (MsgErrorCode.MasterDataNotFound, null);
		}
		SkillAnimInfo skillAnimInfo = Hub.s.dataman.AnimNotiManager.GetSkillAnimInfo(_self.PrefabName, skillInfo.SkillAnimationState);
		if (skillAnimInfo == null)
		{
			return (MsgErrorCode.SkillNotFoundInAnimData, null);
		}
		msgErrorCode = CanUseSkill(skillInfo, flags);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return (msgErrorCode, null);
		}
		if ((flags & SkillFlags.IgnoreValidation) == 0)
		{
			MovementController? movementControlUnit = _self.MovementControlUnit;
			if (movementControlUnit != null && !movementControlUnit.ValidateMoveDistance(req.startBasePosition))
			{
				msgErrorCode = MsgErrorCode.SkillMoverangeValidateFailed;
			}
		}
		long newSkillSyncID = GetNewSkillSyncID();
		if (skillInfo.CooltimeType == SkillCoolTimeType.SkillStart)
		{
			ApplySkillCoolTime(skillMasterID);
		}
		if (_currentCtx != null)
		{
			_currentCtx.OnSkillCancel();
			_currentCtx = null;
		}
		_self.MoveToPosition(req.startBasePosition, ActorMoveCause.Move);
		List<Vector3> targetPositions = new List<Vector3>(req.targetPos);
		return (MsgErrorCode.Success, new NormalSkillContext(new SkillContextBaseInfo(_self, skillInfo, newSkillSyncID, (skillInfo.TargetingType == SkillTargetingType.Targeting) ? req.targetID : 0, targetPositions, req.endBasePosition, flags, null, skillAnimInfo)));
	}

	public MsgErrorCode HandleUseSkillReq(UseSkillReq req)
	{
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(req.skillMasterID);
		if (skillInfo == null)
		{
			return MsgErrorCode.MasterDataNotFound;
		}
		MsgErrorCode msgErrorCode = _self.CanAction((!skillInfo.AllowMove) ? VActorActionType.Skill : VActorActionType.MoveSkill, req.skillMasterID);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		msgErrorCode = _self.InventoryControlUnit?.CanUseSkill(skillInfo.MasterID) ?? MsgErrorCode.InvalidErrorCode;
		if (msgErrorCode != MsgErrorCode.Success)
		{
			return msgErrorCode;
		}
		if (!_skillSlots.Contains(skillInfo.MasterID))
		{
			return MsgErrorCode.SkillNotFoundInSlot;
		}
		ISkillContext skillContext;
		(msgErrorCode, skillContext) = PrepareSkillContext(req);
		switch (msgErrorCode)
		{
		case MsgErrorCode.SkillMoverangeValidateFailed:
			_self.MovementControlUnit?.ForceSyncPosition(TeleportReason.ForceMoveSync);
			break;
		default:
			return msgErrorCode;
		case MsgErrorCode.Success:
			break;
		}
		if (skillContext == null)
		{
			return MsgErrorCode.CannotUsingSkill;
		}
		UseSkillRes useSkillRes = new UseSkillRes(req.hashCode)
		{
			skillSyncID = skillContext.SkillSyncID
		};
		skillContext.SkillBasedStartPosition.CopyTo(useSkillRes.startBasePos);
		_self.SendToMe(useSkillRes);
		UseSkill(skillContext);
		return MsgErrorCode.Success;
	}

	public bool CanUseSkillByRange(VActor target, float minRange, float maxRange, float allowRange = 0f)
	{
		float num = maxRange;
		if (num < allowRange)
		{
			num = allowRange;
		}
		float num2 = Misc.Distance(_self.PositionVector, target.PositionVector);
		if (num2 >= minRange)
		{
			return num2 <= num;
		}
		return false;
	}

	public ISkillContext? PrepareSkillContextByAI(VActor? target, List<Vector3>? targetPosList, int skillMasterID, ref long skillSyncID, SkillFlags flags, List<int>? multiTargetActorIDs = null)
	{
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
		if (skillInfo == null)
		{
			return null;
		}
		SkillAnimInfo skillAnimInfo = Hub.s.dataman.AnimNotiManager.GetSkillAnimInfo(_self.PrefabName, skillInfo.SkillAnimationState);
		if (skillAnimInfo == null)
		{
			return null;
		}
		_ = _self.MasterID;
		if (!ValidSkill(_self, skillMasterID))
		{
			return null;
		}
		if ((target != null && (target.UntargetReason & UntargetReason.Normal) == UntargetReason.Normal) || (target != null && (target.UntargetReason & UntargetReason.Invisible) == UntargetReason.Invisible))
		{
			return null;
		}
		if (CanUseSkill(skillInfo, flags) != MsgErrorCode.Success)
		{
			return null;
		}
		if (target != null || (targetPosList != null && targetPosList.Count > 0))
		{
			Vector3 end = default(Vector3);
			if (target != null)
			{
				end = target.PositionVector;
			}
			else if (targetPosList != null && targetPosList.Count > 0)
			{
				end = targetPosList[0];
			}
			float directionAngle = Misc.GetDirectionAngle(_self.PositionVector, end);
			if (MathF.Abs(Mathf.DeltaAngle(_self.Position.yaw, directionAngle)) > 10f)
			{
				_self.MovementControlUnit.TurnAngle(Convert.ToSingle(Misc.GetDirectionAngle(_self.PositionVector, end), CultureInfo.InvariantCulture), _self.Position.pitch);
			}
		}
		_self.MovementControlUnit?.StopMove();
		if (skillInfo.CooltimeType == SkillCoolTimeType.SkillStart)
		{
			ApplySkillCoolTime(skillMasterID, sync: false);
		}
		skillSyncID = GetNewSkillSyncID();
		PosWithRot posWithRot = new PosWithRot();
		_self.Position.CopyTo(posWithRot);
		return new NormalSkillContext(new SkillContextBaseInfo(_self, skillInfo, skillSyncID, (skillInfo.TargetingType == SkillTargetingType.Targeting) ? (target?.ObjectID ?? 0) : 0, targetPosList, posWithRot, flags, multiTargetActorIDs, skillAnimInfo));
	}

	public bool UseSkillReqByAI(VActor? target, List<Vector3>? targetPosList, int skillMasterID, ref long skillSyncID, SkillFlags flags, List<int>? multiTargetActorIDs = null)
	{
		ISkillContext skillContext = PrepareSkillContextByAI(target, targetPosList, skillMasterID, ref skillSyncID, flags, multiTargetActorIDs);
		if (skillContext == null)
		{
			return false;
		}
		UseSkill(skillContext);
		return true;
	}

	private void UseSkill(ISkillContext context)
	{
		_currentCtx?.OnSkillCancel();
		_prevCtx = null;
		_currentCtx = context;
		if (_pendingDeleteSkillContexts.ContainsKey(context.SkillSyncID))
		{
			_pendingDeleteSkillContexts.Remove(context.SkillSyncID);
		}
		_pendingDeleteSkillContexts.Add(context.SkillSyncID, context);
		_syncExpiredTime.Add(context.SkillSyncID, context.ExpiredTick);
		_currentCtx.Start();
		_self.InventoryControlUnit?.OnUseSkill(context.SkillMasterID);
		_self.EmotionControlUnit?.OnSkill();
		if (_currentCtx.Status == SkillContextStatus.Vanishing)
		{
			_currentCtx = null;
		}
	}

	private static bool ValidSkill(VActor actor, int skillMasterID)
	{
		ActorType actorType = actor.ActorType;
		if ((uint)(actorType - 1) <= 1u || actorType == ActorType.NPC)
		{
			return true;
		}
		return false;
	}

	public MsgErrorCode CancelSkill(SkillCancelType type = SkillCancelType.All, long skillSyncID = 0L)
	{
		if (_currentCtx == null)
		{
			Logger.RWarn("No Current Skill Context to Cancel");
			return MsgErrorCode.SkillNotUsed;
		}
		if (type == SkillCancelType.FixedSkill && GetUsingSkillSyncID() != skillSyncID)
		{
			Logger.RWarn("No Matching Skill Context to Cancel");
			return MsgErrorCode.SkillNotUsed;
		}
		_currentCtx.OnSkillCancel();
		_self.SendInSight(new CancelSkillSig
		{
			actorID = _self.ObjectID,
			syncID = _currentCtx.SkillSyncID
		}, includeSelf: true);
		_pendingDeleteSkillContexts.Remove(_currentCtx.SkillSyncID);
		_prevCtx = _currentCtx;
		_currentCtx = null;
		return MsgErrorCode.Success;
	}

	public void ApplySkillCoolTime(int skillMasterID, bool sync = true)
	{
		if (IgnoreCoolTime)
		{
			return;
		}
		SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID);
		if (skillInfo != null)
		{
			long num = skillInfo.Cooltime;
			if (num > 0)
			{
				_self.CooltimeControlUnit?.AddCooltime(CooltimeType.Skill, skillMasterID, 0L, num, sync);
			}
		}
	}

	public bool CanUseSkillByCooltime(int skillMasterID)
	{
		CooltimeController? cooltimeControlUnit = _self.CooltimeControlUnit;
		if (cooltimeControlUnit == null || cooltimeControlUnit.IsCooltime(CooltimeType.Skill, skillMasterID))
		{
			return IgnoreCoolTime;
		}
		return true;
	}

	public bool IsUsingSkillNow(bool skillCancelable = false)
	{
		if (_currentCtx == null)
		{
			return false;
		}
		if (skillCancelable && _currentCtx.cancelableSkillMasterIDs.Count > 0)
		{
			return false;
		}
		return true;
	}

	public int GetUsingSkillMasterID()
	{
		return _currentCtx?.SkillMasterID ?? 0;
	}

	public void OnResetCooltime(int skillMasterID)
	{
		_currentCtx?.OnResetSkillCoolTime(skillMasterID);
	}

	public long GetUsingSkillSyncID()
	{
		return _currentCtx?.SkillSyncID ?? 0;
	}

	public void PostUpdate(long deltaTime)
	{
		throw new NotImplementedException();
	}

	public void SetSkillFromWeapon(EquipmentItemElement? element)
	{
		ClearSkillMap();
		if (element == null)
		{
			return;
		}
		ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(element.ItemMasterID);
		if (itemInfo == null || !(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
		{
			Logger.RError($"ItemInfo is null. ItemMasterID: {element.ItemMasterID}");
			return;
		}
		ImmutableArray<SkillPair>.Enumerator enumerator = itemEquipmentInfo.SkillList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SkillPair current = enumerator.Current;
			if (current.SkillMasterIDWithGauge > 0)
			{
				_skillSlots.Add(current.SkillMasterIDWithGauge);
			}
			_skillSlots.Add(current.SkillMasterIDNoGague);
		}
	}

	public void ClearSkillMap()
	{
		_skillSlots.Clear();
		foreach (int defaultSkill in _defaultSkillList)
		{
			_skillSlots.Add(defaultSkill);
		}
	}

	public long GetCurrentSkillSyncID()
	{
		return _currentCtx?.SkillSyncID ?? 0;
	}

	public HashSet<int> GetSkillSlots()
	{
		return _skillSlots;
	}

	public int GetNonDefaultSkillMasterID()
	{
		if (_skillSlots.Count == 0)
		{
			return 0;
		}
		foreach (int skillSlot in _skillSlots)
		{
			if (!_defaultSkillList.Contains(skillSlot))
			{
				return skillSlot;
			}
		}
		return 0;
	}

	public MsgErrorCode HandleSyncSkillMove(SyncSkillMoveReq req)
	{
		ISkillContext skillContext = _currentCtx ?? _prevCtx;
		if (skillContext == null || skillContext.SkillSyncID != req.skillSyncID)
		{
			return MsgErrorCode.SkillNotUsed;
		}
		if (skillContext.ExpiredTick < Hub.s.timeutil.GetCurrentTickMilliSec())
		{
			_self.MovementControlUnit?.ForceSyncPosition(TeleportReason.ForceMoveSync);
			return MsgErrorCode.SkillAlreadyExpired;
		}
		skillContext.OnMoveSkillReq(req.targetPosition);
		_self.SetPosition(req.targetPosition, ActorMoveCause.Move);
		_self.SendToMe(new SyncSkillMoveRes(req.hashCode)
		{
			skillMasterID = req.skillMasterID,
			skillSyncID = req.skillSyncID
		});
		_self.SendInSight(new SyncSkillMoveSig
		{
			actorID = _self.ObjectID,
			skillMasterID = req.skillMasterID,
			skillSyncID = req.skillSyncID,
			targetPosition = req.targetPosition,
			moveSpeed = req.moveSpeed,
			targetActorID = req.targetActorID
		});
		return MsgErrorCode.Success;
	}

	public string GetDebugString()
	{
		return string.Empty;
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
		if (_currentCtx != null && _currentCtx.HitCheckForDebug != null)
		{
			VWorldUtil.CollectHitCheckDebugInfo(_self.ObjectID, _self.Position, _currentCtx.HitCheckForDebug, ref sig);
		}
	}
}
