using System;
using System.Collections.Generic;
using Bifrost.Cooked;
using ReluProtocol;
using UnityEngine;

public abstract class ISkillContext : IDisposable
{
	protected AtomicFlag _disposed = new AtomicFlag(value: false);

	public int SkillMasterID => SkillInfo.MasterID;

	public SkillInfo SkillInfo => ContextInfo.SkillInfo;

	public long SkillSyncID => ContextInfo.SkillSyncID;

	public PosWithRot SkillBasedStartPosition => ContextInfo.SkillBasedStartPosition;

	public float SkillTime => ContextInfo.SkillTime;

	public SkillContextBaseInfo ContextInfo { get; }

	public long StartTick { get; }

	public long ExpiredTick { get; }

	public SkillContextStatus Status { get; protected set; }

	public int RemainHitCount { get; set; }

	public int TotalHitCount { get; set; }

	public int LastHitIndex { get; protected set; }

	public int LastProjectileIndex { get; protected set; }

	public int LastFieldSkillIndex { get; protected set; }

	public int LastDestroyItemIndex { get; protected set; }

	public int LastImmuneApplierIndex { get; protected set; }

	public int LastAuraIndex { get; protected set; }

	public int LastReloadWeaponIndex { get; protected set; }

	public HashSet<int> cancelableSkillMasterIDs { get; protected set; } = new HashSet<int>();

	public long HitCheckForDebugLifeTick { get; protected set; }

	public IHitCheck HitCheckForDebug { get; protected set; }

	public float AllowRange => SkillInfo.AllowRange;

	public ISkillContext(SkillContextBaseInfo info)
	{
		ContextInfo = info;
		StartTick = Hub.s.timeutil.GetCurrentTickMilliSec();
		Status = SkillContextStatus.Created;
		ExpiredTick = StartTick + Hub.s.timeutil.ChangeTimeSec2Milli(SkillTime);
		LastHitIndex = 0;
		LastProjectileIndex = 0;
		LastFieldSkillIndex = 0;
		LastDestroyItemIndex = 0;
		LastImmuneApplierIndex = 0;
		LastAuraIndex = 0;
		LastReloadWeaponIndex = 0;
		RemainHitCount = SkillInfo.HitCount;
		TotalHitCount = 0;
	}

	public abstract void Start();

	public abstract void Update(long delta);

	public abstract bool OnHit(HitInputData targetPack, DamageCauseType causeType, IsDamageImmuned isDamageImmuned, SkillPushDelegate skillPush, SkillSequenceHitLog? hitLog = null);

	public abstract void OnSkillCancel();

	public abstract void AddHitStop(float hitStop);

	public abstract SkillPushDelegate DefaultSkillPush();

	public abstract Vector3 GetMaxRangeVector();

	public abstract void OnResetSkillCoolTime(int skillID);

	public abstract void SetEndBasePosition(PosWithRot pos);

	public abstract void OnMoveSkillReq(PosWithRot pos);

	public virtual SkillContextExtract Extract()
	{
		return new SkillContextExtract(this);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual bool Dispose(bool disposing)
	{
		if (_disposed.On() && disposing)
		{
			cancelableSkillMasterIDs.Clear();
			ContextInfo.Dispose();
			return true;
		}
		return false;
	}
}
