using System.Collections.Generic;
using Bifrost.Cooked;
using ReluProtocol;

public class SkillContextExtract
{
	public int SkillMasterID { get; }

	public SkillInfo SkillInfo { get; }

	public long SkillSyncID { get; }

	public PosWithRot SkillBasedStartPosition { get; }

	public float SkillTime { get; }

	public SkillContextBaseInfo ContextInfo { get; }

	public long StartTick { get; }

	public long ExpiredTick { get; }

	public SkillContextStatus Status { get; }

	public int RemainHitCount { get; }

	public int TotalHitCount { get; }

	public int LastHitIndex { get; }

	public int LastProjectileIndex { get; }

	public int LastFieldSkillIndex { get; }

	public int LastDestroyItemIndex { get; }

	public int LastImmuneApplierIndex { get; }

	public int LastAuraIndex { get; }

	public int LastReloadWeaponIndex { get; }

	public HashSet<int> CancelableSkillMasterIDs { get; }

	public float AllowRange { get; }

	public long ExtractedTick { get; }

	public SkillContextExtract(ISkillContext context)
	{
		SkillMasterID = context.SkillMasterID;
		SkillInfo = context.SkillInfo;
		SkillSyncID = context.SkillSyncID;
		SkillBasedStartPosition = context.SkillBasedStartPosition.Clone();
		SkillTime = context.SkillTime;
		ContextInfo = context.ContextInfo.Clone();
		StartTick = context.StartTick;
		ExpiredTick = context.ExpiredTick;
		Status = context.Status;
		RemainHitCount = context.RemainHitCount;
		TotalHitCount = context.TotalHitCount;
		LastHitIndex = context.LastHitIndex;
		LastProjectileIndex = context.LastProjectileIndex;
		LastFieldSkillIndex = context.LastFieldSkillIndex;
		LastDestroyItemIndex = context.LastDestroyItemIndex;
		LastImmuneApplierIndex = context.LastImmuneApplierIndex;
		LastAuraIndex = context.LastAuraIndex;
		LastReloadWeaponIndex = context.LastReloadWeaponIndex;
		AllowRange = context.AllowRange;
		CancelableSkillMasterIDs = new HashSet<int>(context.cancelableSkillMasterIDs);
		ExtractedTick = Hub.s.timeutil.GetCurrentTickMilliSec();
	}
}
