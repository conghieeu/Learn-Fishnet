using System;
using System.Collections.Generic;
using Bifrost.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class CrowdControlManager : IAbnormalManager
{
	public ActorCCStatus CCStatus { get; private set; }

	public Dictionary<CCType, long> CCDict { get; private set; } = new Dictionary<CCType, long>();

	public CrowdControlManager(AbnormalController controller)
		: base(controller)
	{
	}

	public void Initialize()
	{
		foreach (CCType value in Enum.GetValues(typeof(CCType)))
		{
			CCDict[value] = 0L;
		}
	}

	public override void Update(long delta)
	{
		base.Update(delta);
		UpdateEffect();
	}

	private void UpdateEffect()
	{
		if (!IsMovable())
		{
			_abnormalController.Self.MovementControlUnit?.StopMove(sync: true, needToCancel: true);
		}
		if (!CanUseSkill() && _abnormalController.Self.LifeCycle == VCreatureLifeCycle.Alive && _abnormalController.Self.SkillControlUnit.GetUsingSkillSyncID() > 0)
		{
			_abnormalController.Self.SkillControlUnit?.CancelSkill(SkillCancelType.All, 0L);
		}
	}

	protected override void DisposeOnList()
	{
		foreach (long trashBucket in _trashBuckets)
		{
			if (GetAbnormalElement(trashBucket, out IAbnormalElement abnormalElement) && abnormalElement is CCElement cCElement)
			{
				if (CCDict[cCElement.CCType] == cCElement.FixedValue.SyncID)
				{
					CCDict[cCElement.CCType] = 0L;
				}
				else
				{
					ExistAbnormalElement(CCDict[cCElement.CCType]);
				}
			}
		}
	}

	public void ApplyActionPauseOnActionAbnormal(CCElement element)
	{
	}

	public bool IsActionAbnormal()
	{
		if ((CCStatus & ActorCCStatus.DOWN) == 0)
		{
			return (CCStatus & ActorCCStatus.AIRBORNE) != 0;
		}
		return true;
	}

	public bool IsMovable()
	{
		return (CCStatus & ActorCCStatus.MOVE_BLOCKED) == 0;
	}

	public bool IsStopOnPos()
	{
		return (CCStatus & ActorCCStatus.STOP_ON_POS) != 0;
	}

	public bool CanUseSkill()
	{
		return (CCStatus & ActorCCStatus.SKILL_BLOCKED) == 0;
	}

	public bool IsActionPossible()
	{
		return CCStatus == ActorCCStatus.NORMAL;
	}

	public bool IsInCC(ActorCCStatus status, bool any = false)
	{
		if (any)
		{
			return CCStatus != ActorCCStatus.NORMAL;
		}
		return (CCStatus & status) != 0;
	}

	public bool IsGetup()
	{
		return (CCStatus & ActorCCStatus.DOWN) == 0;
	}

	public bool IsAIPause()
	{
		return (CCStatus & ActorCCStatus.AI_PAUSE) != 0;
	}

	public bool IsDetach()
	{
		return (CCStatus & ActorCCStatus.DETACH) != 0;
	}

	public bool IsCantHoldItem()
	{
		return (CCStatus & ActorCCStatus.CANT_HOLD_ITEM) != 0;
	}

	public (int max, bool pushTimeExpired) GetHighestPriority()
	{
		int num = 0;
		bool item = false;
		foreach (KeyValuePair<CCType, long> item2 in CCDict)
		{
			if (item2.Value == 0L)
			{
				continue;
			}
			int priority = DefAbnormalUtil.GetPriority(item2.Key);
			if (priority > num)
			{
				num = priority;
				item = false;
				if (DefAbnormalUtil.IsActionAbnormal(item2.Key) && GetAbnormalElement(item2.Value, out IAbnormalElement abnormalElement) && abnormalElement is CCElement cCElement && cCElement.StartTime + cCElement.PushTime < Hub.s.timeutil.GetCurrentTickMilliSec())
				{
					item = true;
				}
			}
		}
		return (max: num, pushTimeExpired: item);
	}

	public bool AddCC(IAbnormalElement element)
	{
		switch (AddAbnormal(element))
		{
		case AbnormalHandleResult.Failed:
			return false;
		case AbnormalHandleResult.Defered:
			return true;
		default:
			if (!(element is CCElement cCElement))
			{
				return false;
			}
			if (CCDict[cCElement.CCType] != 0L)
			{
				if (!GetAbnormalElement(CCDict[cCElement.CCType], out IAbnormalElement abnormalElement))
				{
					Logger.RError($"CCType: {cCElement.CCType} is already exist but abnormalElement is null");
				}
				else
				{
					if (abnormalElement.EndTime > element.EndTime)
					{
						element.OverwriteEndTime(abnormalElement.EndTime);
					}
					_ = abnormalElement.FixedValue.Dispelable;
					abnormalElement.SetDelete();
				}
			}
			CCDict[cCElement.CCType] = element.FixedValue.SyncID;
			RefineInfo();
			UpdateEffect();
			ApplyActionPauseOnActionAbnormal(cCElement);
			if (IsStopOnPos())
			{
				DispelPushCC();
			}
			_abnormalController.Self.VRoom.OnActorEvent(new ApplyCrowdControlArgs(element.CasterObjectID, _abnormalController.Self));
			return true;
		}
	}

	public bool DispelCC(CCType ccType, bool all = false)
	{
		if (all)
		{
			DispelAll();
			return true;
		}
		if (CCDict[ccType] == 0L)
		{
			return true;
		}
		if (!GetAbnormalElement(CCDict[ccType], out IAbnormalElement abnormalElement))
		{
			return false;
		}
		if (!abnormalElement.FixedValue.Dispelable)
		{
			return false;
		}
		abnormalElement.SetDelete();
		return true;
	}

	protected override void RefineInfo()
	{
		CCStatus = ActorCCStatus.NORMAL;
		foreach (var (key, defAbnormal_MasterData2) in Hub.s.dataman.ExcelDataManager.CCAbnormalDict)
		{
			if (CCDict[key] != 0L)
			{
				if (defAbnormal_MasterData2.unable_move || defAbnormal_MasterData2.unable_input_move)
				{
					CCStatus |= ActorCCStatus.MOVE_BLOCKED;
				}
				if (defAbnormal_MasterData2.unable_input)
				{
					CCStatus |= ActorCCStatus.SKILL_BLOCKED;
				}
				if (defAbnormal_MasterData2.unable_move)
				{
					CCStatus |= ActorCCStatus.STOP_ON_POS;
				}
				if (defAbnormal_MasterData2.bt_pause)
				{
					CCStatus |= ActorCCStatus.AI_PAUSE;
				}
				if (defAbnormal_MasterData2.occur_detach)
				{
					CCStatus |= ActorCCStatus.DETACH;
				}
				if (defAbnormal_MasterData2.drop_item)
				{
					CCStatus |= ActorCCStatus.CANT_HOLD_ITEM;
				}
			}
		}
		if (CCDict[CCType.Airborne] != 0L)
		{
			CCStatus |= ActorCCStatus.AIRBORNE;
		}
		if (CCDict[CCType.Knockback] != 0L || CCDict[CCType.Knockdown] != 0L)
		{
			CCStatus |= ActorCCStatus.DOWN;
		}
	}

	public override void Clear()
	{
		base.Clear();
		CCDict.Clear();
		CCStatus = ActorCCStatus.NORMAL;
	}

	protected override bool CollectInfo(long syncID, AbnormalDataSyncType syncType, ref AbnormalSig sig)
	{
		if (!GetAbnormalElement(syncID, out IAbnormalElement abnormalElement))
		{
			return false;
		}
		if (!(abnormalElement is CCElement cCElement))
		{
			return false;
		}
		AbnormalCCInfo abnormalCCInfo = new AbnormalCCInfo
		{
			ccType = cCElement.CCType,
			abnormalMasterID = cCElement.AbnormalMasterID,
			changeType = syncType,
			duration = cCElement.Duration,
			remainTime = cCElement.GetRemainTime(),
			syncID = cCElement.FixedValue.SyncID,
			pushTime = cCElement.PushTime
		};
		_abnormalController.Self.Position.CopyTo(abnormalCCInfo.basePosition);
		cCElement.TargetPos.CopyTo(abnormalCCInfo.hitPosition);
		sig.ccList.Add(abnormalCCInfo);
		return true;
	}

	public void GetAllInfo(ref List<AbnormalCCInfo> list)
	{
		foreach (IAbnormalElement value in _abnormals.Values)
		{
			if (!value.DeferToDelete && value is CCElement cCElement)
			{
				AbnormalCCInfo item = new AbnormalCCInfo
				{
					ccType = cCElement.CCType,
					abnormalMasterID = cCElement.AbnormalMasterID,
					changeType = AbnormalDataSyncType.Add,
					duration = cCElement.Duration,
					remainTime = cCElement.GetRemainTime(),
					abnormalSyncID = cCElement.FixedValue.AbnormalObjectID,
					syncID = cCElement.FixedValue.SyncID,
					pushTime = cCElement.PushTime
				};
				list.Add(item);
			}
		}
	}

	public void DispelPushCC()
	{
		foreach (IAbnormalElement value in _abnormals.Values)
		{
			if (value is CCElement cCElement && cCElement.PushTime > 0)
			{
				cCElement.SetDelete();
			}
		}
	}

	public int GetCCCasterObjectID(CCType type)
	{
		if (CCDict[type] == 0L)
		{
			return 0;
		}
		if (!GetAbnormalElement(CCDict[type], out IAbnormalElement abnormalElement))
		{
			return 0;
		}
		return abnormalElement.CasterObjectID;
	}
}
