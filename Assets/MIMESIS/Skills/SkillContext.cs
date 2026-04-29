using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public class SkillContext<T> : ISkillContext where T : UseSkillSig, new()
{
	private List<long> _abnormalImmuneSyncIDs = new List<long>();

	private NewTargetLogger? _targetLogger;

	private long _consumeStaminaElapsedTime;

	private bool _refreshCoolTimeOnEnd;

	protected ProcessNotifyHandler? _notifyHandler;

	protected long _totalHitStopMilliSec;

	private long _lastNotifyProcessTick;

	private SkillSequenceHitLog? _hitLog;

	private bool _abnormalImmuned => _abnormalImmuneSyncIDs.Count > 0;

	public SkillContext(SkillContextBaseInfo info)
		: base(info)
	{
		_refreshCoolTimeOnEnd = base.SkillInfo.CooltimeType == SkillCoolTimeType.SkillEnd;
		InitNotifyHandler();
	}

	public virtual void FillUseSkillSig(T sig)
	{
		sig.actorID = base.ContextInfo.Creature.ObjectID;
		sig.skillSyncID = base.SkillSyncID;
		sig.skillMasterID = base.SkillMasterID;
		sig.targetID = base.ContextInfo.TargetObjectID;
		sig.startBasePosition = base.ContextInfo.SkillBasedStartPosition.Clone();
		sig.endBasePosition = base.ContextInfo.SkillBasedEndPosition.Clone();
	}

	public IEnumerable<VActor> ChooseTargets()
	{
		HashSet<VActor> actors = new HashSet<VActor>();
		if (base.SkillInfo.TargetCount == 0)
		{
			return actors;
		}
		VActor vActor = base.ContextInfo.Creature.VRoom.FindActorByObjectID(base.ContextInfo.TargetObjectID);
		if (vActor != null)
		{
			actors.Add(vActor);
		}
		base.ContextInfo.Creature.AIControlUnit?.IterateAggroActorOrderByPoint(delegate(VCreature actor)
		{
			if (base.SkillInfo.TargetCount > actors.Count)
			{
				actors.Add(actor);
			}
		});
		base.ContextInfo.Creature.IterateAroundActors(delegate(VActor actor)
		{
			if (base.SkillInfo.TargetCount > actors.Count && base.ContextInfo.Creature.IsValidTarget(HitTargetType.Enemy, actor))
			{
				actors.Add(actor);
			}
		});
		return actors;
	}

	public virtual void SendUseSkillSig(T sig)
	{
		base.ContextInfo.Creature.SendInSight(sig, includeSelf: true);
	}

	public override void Start()
	{
		if (base.Status != SkillContextStatus.Created)
		{
			throw new Exception($"[SkillContext.Start] SkillContextStatus is not Created. Status: {base.Status}");
		}
		base.Status = SkillContextStatus.Casting;
		if (base.ContextInfo.Creature is VMonster)
		{
			base.ContextInfo.Creature.AIControlUnit?.SetTriggerSkillInfo(base.SkillMasterID, base.SkillSyncID);
		}
		T sig = new T();
		FillUseSkillSig(sig);
		SendUseSkillSig(sig);
	}

	public override bool OnHit(HitInputData targetPack, DamageCauseType causeType, IsDamageImmuned isDamageImmuned, SkillPushDelegate skillPush, SkillSequenceHitLog? hitLog = null)
	{
		if (base.SkillInfo.HitCount != 0 && base.RemainHitCount <= 0)
		{
			return false;
		}
		if (hitLog != null && !hitLog.CheckHitCountLimit())
		{
			return false;
		}
		SkillSequenceInfo skillSequenceInfo = targetPack.SkillSequenceInfo;
		if (skillSequenceInfo.MutableValue == 0L && skillSequenceInfo.AbnormalMasterIDs.Count() == 0 && skillSequenceInfo.LinkedGrabMasterID == 0)
		{
			return false;
		}
		if (skillSequenceInfo.MutableStatType == MutableStatType.HP)
		{
			List<TargetHitInfo> list = OnDamageImpl(base.ContextInfo.Creature, base.SkillInfo.Name, targetPack, causeType, isDamageImmuned, skillPush, hitLog);
			if (list.Count == 0)
			{
				return false;
			}
			int count = list.Count;
			base.RemainHitCount -= count;
			base.TotalHitCount += count;
			HitTargetSig hitTargetSig = new HitTargetSig
			{
				actorID = base.ContextInfo.Creature.ObjectID,
				skillSyncID = base.SkillSyncID,
				skillSequenceMasterID = skillSequenceInfo.MasterID
			};
			base.ContextInfo.Creature.InventoryControlUnit.OnHitSkill(targetPack.SkillInfo.MasterID, count);
			_targetLogger?.Update(list);
			hitTargetSig.targetHitInfos.AddRange(list);
			base.ContextInfo.Creature.SendInSight(hitTargetSig, includeSelf: true);
		}
		else if (skillSequenceInfo.MutableStatType == MutableStatType.Conta)
		{
			foreach (TargetInfo target in targetPack.Targets)
			{
				VActor vActor = base.ContextInfo.Creature.VRoom.FindActorByObjectID(target.targetObjectID);
				if (vActor != null && vActor is VCreature && base.ContextInfo.Creature.IsValidTarget(skillSequenceInfo.HitTargetTypes, vActor) && vActor.IsAliveStatus())
				{
					vActor.StatControlUnit.IncreaseConta(skillSequenceInfo.MutableValue);
				}
			}
		}
		return true;
	}

	public static List<TargetHitInfo> OnDamageImpl(VActor actor, string skillName, HitInputData targetPack, DamageCauseType causeType, IsDamageImmuned isDamageImmuned, SkillPushDelegate skillPush, SkillSequenceHitLog? hitLog = null)
	{
		SkillInfo skillInfo = targetPack.SkillInfo;
		SkillSequenceInfo skillSequenceInfo = targetPack.SkillSequenceInfo;
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		List<TargetHitInfo> list = new List<TargetHitInfo>();
		List<(TargetInfo, VActor)> list2 = new List<(TargetInfo, VActor)>();
		foreach (TargetInfo target in targetPack.Targets)
		{
			int targetObjectID = target.targetObjectID;
			VActor vActor = actor.VRoom.FindActorByObjectID(targetObjectID);
			if (vActor != null && vActor is VCreature && actor.IsValidTarget(skillSequenceInfo.HitTargetTypes, vActor) && vActor.IsAliveStatus() && (hitLog == null || hitLog.CheckHitTerm(targetObjectID, currentTickMilliSec)))
			{
				list2.Add((target, vActor));
			}
		}
		_ = list2.Count;
		long num = 0L;
		foreach (var item3 in list2)
		{
			TargetInfo item = item3.Item1;
			VActor item2 = item3.Item2;
			int targetObjectID2 = item.targetObjectID;
			bool flag = false;
			if (skillSequenceInfo.SequenceType != SkillSeqType.Abnormal)
			{
				TargetHitInfo hitInfo = new TargetHitInfo
				{
					targetID = targetObjectID2,
					basePosition = item2.Position
				};
				ImmuneCheckResult immuneCheckResult = isDamageImmuned(item2);
				flag = immuneCheckResult.Immuned;
				if (flag)
				{
					hitInfo.immuneType = immuneCheckResult.ImmuneType;
					hitInfo.actionAbnormalHitType = CCType.None;
				}
				else
				{
					DamageAttribute damageAttribute = item2.StatControlUnit?.EstimateDamage(actor, MutableStatChangeCause.ActiveAttack, skillSequenceInfo.MasterID) ?? throw new Exception($"[OnDamageImpl] targetActor.StatControlUnit?.EstimateDamage is null. targetID: {targetObjectID2}");
					CCType actionAbnormalHitType = CCType.None;
					if (!flag && skillSequenceInfo.BattleActionKey != "NO_ACTION")
					{
						BattleActionInfo battleActionData = Hub.s.dataman.ExcelDataManager.GetBattleActionData(skillSequenceInfo.BattleActionKey);
						if (battleActionData != null)
						{
							actionAbnormalHitType = battleActionData.CCType;
							hitInfo.hitDelay = battleActionData.MoveTime + battleActionData.DownTime;
							hitInfo.pushTime = battleActionData.MoveTime;
						}
					}
					hitInfo.actionAbnormalHitType = actionAbnormalHitType;
					skillPush(hitInfo, item2, skillSequenceInfo.BattleActionKey, skillSequenceInfo.BattleActionDistType);
					if (damageAttribute.Damage > 0)
					{
						item2.StatControlUnit?.ApplyDamage(new ApplyDamageArgs(actor, item2, MutableStatChangeCause.ActiveAttack, damageAttribute.Damage, damageAttribute.GroggyValue, skillInfo.MasterID, skillSequenceInfo.MasterID, skillSequenceInfo.HitType, skillName));
						num += hitInfo.damage;
					}
					if (skillSequenceInfo.LinkedGrabMasterID > 0)
					{
						AttachMasterInfo attachInfo = Hub.s.dataman.ExcelDataManager.GetAttachInfo(skillSequenceInfo.LinkedGrabMasterID);
						if (attachInfo == null)
						{
							continue;
						}
						if (attachInfo.IsReverseGrab)
						{
							item2.AttachControlUnit.AttachBySkill(skillSequenceInfo.LinkedGrabMasterID, actor.ObjectID);
						}
						else
						{
							actor.AttachControlUnit.AttachBySkill(skillSequenceInfo.LinkedGrabMasterID, item2.ObjectID);
						}
					}
					if (skillSequenceInfo.LinkedFieldSkillMasterID != 0)
					{
						actor.VRoom.SpawnFieldSkill(skillSequenceInfo.LinkedFieldSkillMasterID, actor.Position, actor.IsIndoor, null, item2, null, ReasonOfSpawn.Linked);
					}
					damageAttribute.Convert2HitInfo(ref hitInfo);
					list.Add(hitInfo);
				}
			}
			if (skillSequenceInfo.SequenceType != SkillSeqType.Hit && !flag)
			{
				ApplyAbnormal(actor, skillSequenceInfo, item2);
			}
		}
		return list;
	}

	private static void ApplyAbnormal(VActor caster, SkillSequenceInfo skillSeqData, VActor victimActor)
	{
		if (skillSeqData.AbnormalMasterIDs.Count() == 0 || skillSeqData.AbnormalMasterIDs.Count() <= 0)
		{
			return;
		}
		if (skillSeqData.AbnormalApplyType == AbnormalApplyType.All)
		{
			ImmutableArray<int>.Enumerator enumerator = skillSeqData.AbnormalMasterIDs.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int current = enumerator.Current;
				if (current > 0 && Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(current) != null)
				{
					victimActor.AbnormalControlUnit?.AppendAbnormal(caster.ObjectID, current);
				}
			}
		}
		else if (skillSeqData.AbnormalApplyType == AbnormalApplyType.SelectiveRandom)
		{
			int num = skillSeqData.AbnormalMasterIDs[SimpleRandUtil.Next(0, skillSeqData.AbnormalMasterIDs.Count())];
			if (num > 0 && Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(num) != null)
			{
				victimActor.AbnormalControlUnit?.AppendAbnormal(caster.ObjectID, num);
			}
		}
	}

	private static void SpawnFieldSkill(VActor caster, int fieldSkillMasterID, VActor? target, ISkillContext? context, ReasonOfSpawn reasonOfSpawn)
	{
		if (Hub.s.dataman.ExcelDataManager.GetFieldSkillData(fieldSkillMasterID) != null)
		{
			caster.VRoom.SpawnFieldSkill(fieldSkillMasterID, caster.Position, caster.IsIndoor, null, target, context, reasonOfSpawn);
		}
	}

	private static VProjectileObject? SpawnProjectile(VActor caster, int projectileMasterID, string socketName, VActor? target, ISkillContext? context, ReasonOfSpawn reasonOfSpawn)
	{
		if (Hub.s.dataman.ExcelDataManager.GetProjectileInfo(projectileMasterID) == null)
		{
			return null;
		}
		PosWithRot posWithRot = caster.Position.Clone();
		if (!string.IsNullOrEmpty(socketName))
		{
			if (!(caster is VCreature vCreature))
			{
				return null;
			}
			(Vector3, Vector3)? projectilePosInfo = Hub.s.dataman.AnimNotiManager.GetProjectilePosInfo(vCreature.PrefabName, socketName);
			if (!projectilePosInfo.HasValue)
			{
				Logger.RWarn("[SkillContext.SpawnProjectile] ProjectilePosInfo is null. SocketName: " + socketName);
			}
			else
			{
				Vector3 item = projectilePosInfo.Value.Item1;
				Vector3 item2 = projectilePosInfo.Value.Item2;
				Vector3 vector = new Vector3(0f, 1f, 0f);
				Vector3 pos = item - vector;
				float num = caster.Position.pitch;
				if (num > 180f)
				{
					num -= 360f;
				}
				Vector3 vector2 = pos.RotateX(num.toRadian()).RotateY(caster.Position.yaw.toRadian());
				_ = caster.PositionVector + vector.RotateY(caster.Position.yaw.toRadian());
				Vector3 pos2 = caster.PositionVector + new Vector3(0f, 1f, 0f) + vector2;
				float num2 = ((item2.x > 180f) ? (item2.x - 360f) : item2.x);
				Vector3 rot = new Vector3(Misc.ClampAxis(num2 + num), Misc.ClampAxis(item2.y + caster.Position.yaw), 0f);
				posWithRot.pos = pos2;
				posWithRot.rot = rot;
			}
		}
		return caster.VRoom.CreateProjectileObject(projectileMasterID, posWithRot, caster.IsIndoor, context, reasonOfSpawn);
	}

	public override void Update(long delta)
	{
		if (base.Status != SkillContextStatus.Casting)
		{
			return;
		}
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (base.HitCheckForDebug != null && base.HitCheckForDebugLifeTick <= currentTickMilliSec)
		{
			base.HitCheckForDebugLifeTick = 0L;
			base.HitCheckForDebug = null;
		}
		ProcessAnimNotify(currentTickMilliSec);
		if (!UpdateConsumeStamina(delta))
		{
			return;
		}
		if (base.Status == SkillContextStatus.Casting && Hub.s.timeutil.ChangeTimeSec2Milli(base.SkillTime) < currentTickMilliSec - base.StartTick - _totalHitStopMilliSec)
		{
			base.Status = SkillContextStatus.Vanishing;
			if (!base.SkillInfo.AllowMove)
			{
				base.ContextInfo.Creature.SetPosition(base.ContextInfo.SkillBasedEndPosition, ActorMoveCause.Move);
			}
			OnSkillEnd();
		}
		else if (base.Status == SkillContextStatus.Vanishing && Hub.s.timeutil.ChangeTimeSec2Milli(base.SkillTime * 2f) < currentTickMilliSec - base.StartTick - _totalHitStopMilliSec)
		{
			base.Status = SkillContextStatus.Terminated;
		}
	}

	public virtual void OnSkillEnd()
	{
		if (_refreshCoolTimeOnEnd)
		{
			base.ContextInfo.Creature.SkillControlUnit?.ApplySkillCoolTime(base.SkillMasterID);
		}
		foreach (long abnormalImmuneSyncID in _abnormalImmuneSyncIDs)
		{
			base.ContextInfo.Creature.AbnormalControlUnit?.ImmuneManager.DispelAbnormal(abnormalImmuneSyncID, force: true);
		}
	}

	public override void OnSkillCancel()
	{
		if (base.Status != SkillContextStatus.Casting)
		{
			Logger.RWarn($"[SkillContext.OnSkillCancel] SkillContextStatus is not Casting. Status: {base.Status}");
			return;
		}
		SkillAnimInfo? animInfo = base.ContextInfo.AnimInfo;
		if (animInfo != null && animInfo.WeaponDestroyes.Length > 0)
		{
			base.ContextInfo.Creature.InventoryControlUnit.OnAnimationEventDestroy(base.ContextInfo.SkillInfo.MasterID);
		}
		base.Status = SkillContextStatus.Vanishing;
		OnSkillEnd();
	}

	public override void AddHitStop(float hitStop)
	{
		if (base.ContextInfo.Creature is VPlayer)
		{
			_totalHitStopMilliSec += Hub.s.timeutil.ChangeTimeSec2Milli(hitStop);
		}
	}

	public override void SetEndBasePosition(PosWithRot pos)
	{
		pos.CopyTo(base.ContextInfo.SkillBasedEndPosition);
	}

	private bool UpdateConsumeStamina(long delta)
	{
		return true;
	}

	public override SkillPushDelegate DefaultSkillPush()
	{
		return VWorldCombatUtil.DefaultSkillPush(base.ContextInfo.Creature);
	}

	public override Vector3 GetMaxRangeVector()
	{
		return Vector3.right.RotateY(base.ContextInfo.Creature.Position.yaw.toRadian()) * base.SkillInfo.MaxRange;
	}

	public override void OnResetSkillCoolTime(int skillID)
	{
		if (base.SkillInfo.MasterID == skillID)
		{
			_refreshCoolTimeOnEnd = false;
		}
	}

	public override void OnMoveSkillReq(PosWithRot pos)
	{
		SetEndBasePosition(pos);
	}

	public virtual void InitNotifyHandler()
	{
		_notifyHandler = (ProcessNotifyHandler)Delegate.Combine(_notifyHandler, new ProcessNotifyHandler(ProcessNotifyHit));
		_notifyHandler = (ProcessNotifyHandler)Delegate.Combine(_notifyHandler, new ProcessNotifyHandler(ProcessNotifyProjectile));
		_notifyHandler = (ProcessNotifyHandler)Delegate.Combine(_notifyHandler, new ProcessNotifyHandler(ProcessNotifySpawnFieldSkill));
		_notifyHandler = (ProcessNotifyHandler)Delegate.Combine(_notifyHandler, new ProcessNotifyHandler(ProcessNotifyImmune));
		_notifyHandler = (ProcessNotifyHandler)Delegate.Combine(_notifyHandler, new ProcessNotifyHandler(ProcessNotifyActivateAura));
		_notifyHandler = (ProcessNotifyHandler)Delegate.Combine(_notifyHandler, new ProcessNotifyHandler(ProcessNotifyReloadWeapon));
		_notifyHandler = (ProcessNotifyHandler)Delegate.Combine(_notifyHandler, new ProcessNotifyHandler(ProcessNotifyDestroyItem));
	}

	protected void ProcessNotifyHit(float elapsed)
	{
		for (int i = base.LastHitIndex; i < base.ContextInfo.HitNotifyCount; i++)
		{
			if (_hitLog == null)
			{
				_hitLog = new SkillSequenceHitLog(base.ContextInfo.SkillInfo.HitCount, 0f);
			}
			AnimNotifyHitCheck animNotifyHitCheck = base.ContextInfo.AnimInfo?.HitChecks[i] ?? throw new Exception($"[SkillContext.ProcessNotifyHit] HitCheck is null. SkillID: {base.SkillMasterID}");
			if (animNotifyHitCheck.Start > (double)elapsed)
			{
				break;
			}
			if ((double)elapsed > animNotifyHitCheck.Start && animNotifyHitCheck.Duration + animNotifyHitCheck.Start > (double)elapsed)
			{
				if (!base.ContextInfo.SequenceInfos.TryGetValue(animNotifyHitCheck.sequenceIndex, out SkillSequenceInfo value))
				{
					continue;
				}
				List<TargetInfo> retVal = new List<TargetInfo>();
				if (value.HitTargetTypes.Contains(HitTargetType.Self))
				{
					retVal.Add(new TargetInfo
					{
						targetObjectID = base.ContextInfo.Creature.ObjectID,
						hitPosition = base.ContextInfo.Creature.PositionVector
					});
				}
				CollectSkillTargetInfoByHit(animNotifyHitCheck.socketName, ref retVal);
				if (retVal.Count > 0)
				{
					HitInputData targetPack = new HitInputData(base.ContextInfo.SkillInfo, value, retVal);
					OnHit(targetPack, DamageCauseType.Collision, VWorldCombatUtil.DefaultDamageImmuneChecker, DefaultSkillPush(), _hitLog);
				}
			}
			if (i + 1 >= base.ContextInfo.HitNotifyCount)
			{
				break;
			}
			AnimNotifyHitCheck obj = base.ContextInfo.AnimInfo?.HitChecks[i + 1];
			AnimNotifyHitCheck animNotifyHitCheck2 = base.ContextInfo.AnimInfo?.HitChecks[i];
			if (obj != null && animNotifyHitCheck2.Start + animNotifyHitCheck2.Duration < (double)elapsed)
			{
				_hitLog = null;
				base.LastHitIndex = i + 1;
			}
		}
	}

	protected bool CanHitCandidate(VCreature candidate)
	{
		if (base.ContextInfo.Creature is VPlayer && base.ContextInfo.Creature.AttachControlUnit.IsAttaching() && base.ContextInfo.Creature.AttachControlUnit.AttachingActorID == candidate.ObjectID)
		{
			return false;
		}
		if (base.ContextInfo.Creature.VRoom.IsHitWall(base.ContextInfo.Creature.PositionVector, candidate.PositionVector, out var _))
		{
			return false;
		}
		return true;
	}

	protected void CollectSkillTargetInfoByHit(string socketName, ref List<TargetInfo> retVal)
	{
		IHitCheck hitBox = Hub.s.dataman.AnimNotiManager.GetHitBox(base.ContextInfo.Creature.PrefabName, socketName);
		if (hitBox == null)
		{
			return;
		}
		base.HitCheckForDebug = hitBox;
		base.HitCheckForDebugLifeTick = Hub.s.timeutil.GetCurrentTickMilliSec() + 1000;
		List<VCreature> creaturesInRange = base.ContextInfo.Creature.VRoom.GetCreaturesInRange(base.ContextInfo.Creature.PositionVector, base.ContextInfo.SkillInfo.MinRange, base.ContextInfo.SkillInfo.MaxRange, checkHeight: true, base.ContextInfo.Creature);
		if (creaturesInRange.Count == 0)
		{
			return;
		}
		foreach (VCreature item in creaturesInRange)
		{
			if (CanHitCandidate(item))
			{
				IHitCheck hitCheck = item.HitCheck;
				HitCheckPos posOrigin = new HitCheckPos
				{
					Start = base.ContextInfo.Creature.PositionVector,
					End = base.ContextInfo.Creature.PositionVector,
					AngleRad = base.ContextInfo.Creature.Angle.toRadian()
				};
				HitCheckPos posTarget = new HitCheckPos
				{
					Start = item.PositionVector,
					End = item.PositionVector,
					AngleRad = item.Angle.toRadian()
				};
				var (flag, hitPosition, _) = hitBox.IsHit(posOrigin, hitCheck, posTarget);
				if (flag)
				{
					retVal.Add(new TargetInfo
					{
						targetObjectID = item.ObjectID,
						hitPosition = hitPosition,
						targetPosition = item.PositionVector
					});
				}
			}
		}
	}

	protected void ProcessNotifyProjectile(float elapsed)
	{
		if (base.ContextInfo.AnimInfo == null)
		{
			return;
		}
		for (int i = base.LastProjectileIndex; i < base.ContextInfo.AnimInfo.Projectiles.Length; i++)
		{
			AnimNotifyProjectile animNotifyProjectile = base.ContextInfo.AnimInfo.Projectiles[i];
			if (animNotifyProjectile.Start > (double)elapsed)
			{
				break;
			}
			base.LastProjectileIndex = i + 1;
			if (base.ContextInfo.ProjectileInfos.TryGetValue(animNotifyProjectile.sequenceIndex, out ProjectileInfo value))
			{
				VProjectileObject vProjectileObject = SpawnProjectile(base.ContextInfo.Creature, value.MasterID, animNotifyProjectile.SocketName, null, this, ReasonOfSpawn.Skill);
				if (vProjectileObject != null)
				{
					base.ContextInfo.Creature.InventoryControlUnit.OnUseSkill_SpawnProjectile(base.SkillMasterID, vProjectileObject);
				}
			}
		}
	}

	protected void ProcessNotifySpawnFieldSkill(float elapsed)
	{
		if (base.ContextInfo.AnimInfo == null)
		{
			return;
		}
		for (int i = base.LastFieldSkillIndex; i < base.ContextInfo.AnimInfo.FieldSkills.Length; i++)
		{
			AnimNotifyFieldSkill animNotifyFieldSkill = base.ContextInfo.AnimInfo.FieldSkills[i];
			if (!(animNotifyFieldSkill.Start > (double)elapsed))
			{
				base.LastFieldSkillIndex = i + 1;
				if (base.ContextInfo.FieldSkillInfos.TryGetValue(animNotifyFieldSkill.sequenceIndex, out FieldSkillInfo value))
				{
					SpawnFieldSkill(base.ContextInfo.Creature, value.MasterID, null, this, ReasonOfSpawn.Skill);
				}
				continue;
			}
			break;
		}
	}

	protected void ProcessNotifyDestroyItem(float elapsed)
	{
		if (base.ContextInfo.AnimInfo != null)
		{
			for (int i = base.LastDestroyItemIndex; i < base.ContextInfo.AnimInfo.WeaponDestroyes.Length && !(base.ContextInfo.AnimInfo.WeaponDestroyes[i].Start > (double)elapsed); i++)
			{
				base.LastDestroyItemIndex = i + 1;
				base.ContextInfo.Creature.InventoryControlUnit.OnAnimationEventDestroy(base.ContextInfo.SkillInfo.MasterID);
			}
		}
	}

	protected void ProcessNotifyActivateAura(float elapsed)
	{
		if (base.ContextInfo.AnimInfo == null)
		{
			return;
		}
		for (int i = base.LastAuraIndex; i < base.ContextInfo.AnimInfo.ActivateAuras.Length; i++)
		{
			AnimNotifyActivateAura animNotifyActivateAura = base.ContextInfo.AnimInfo.ActivateAuras[i];
			if (!(animNotifyActivateAura.Start > (double)elapsed))
			{
				base.LastAuraIndex = i + 1;
				if (base.ContextInfo.Creature.AuraControlUnit != null)
				{
					base.ContextInfo.Creature.AuraControlUnit.AddAura(animNotifyActivateAura.AuraMasterID, defaultFlag: false);
				}
				continue;
			}
			break;
		}
	}

	protected void ProcessNotifyReloadWeapon(float elapsed)
	{
		if (base.ContextInfo.AnimInfo != null)
		{
			for (int i = base.LastReloadWeaponIndex; i < base.ContextInfo.AnimInfo.ReloadWeapons.Length && !(base.ContextInfo.AnimInfo.ReloadWeapons[i].Start > (double)elapsed); i++)
			{
				base.LastReloadWeaponIndex = i + 1;
				base.ContextInfo.Creature.InventoryControlUnit.HandleReloadWeapon();
			}
		}
	}

	protected void ProcessNotifyImmune(float elapsed)
	{
		if (base.ContextInfo.AnimInfo == null)
		{
			return;
		}
		for (int i = base.LastImmuneApplierIndex; i < base.ContextInfo.AnimInfo.ImmuneAppliers.Length; i++)
		{
			AnimNotifyImmuneApplier animNotifyImmuneApplier = base.ContextInfo.AnimInfo.ImmuneAppliers[i];
			if (animNotifyImmuneApplier.Start > (double)elapsed)
			{
				break;
			}
			base.LastImmuneApplierIndex = i + 1;
			if (animNotifyImmuneApplier.Flag)
			{
				if (_abnormalImmuned)
				{
					break;
				}
				foreach (ImmuneElementInfo immune in base.SkillInfo.Immunes)
				{
					if (base.ContextInfo.Creature.AbnormalControlUnit != null)
					{
						long newAbnormalSyncID = base.ContextInfo.Creature.AbnormalControlUnit.GetNewAbnormalSyncID();
						_abnormalImmuneSyncIDs.Add(newAbnormalSyncID);
						base.ContextInfo.Creature.AbnormalControlUnit.AddImmune(immune.ImmuneType, new ImmuneInputArgs
						{
							SyncID = newAbnormalSyncID,
							Duration = Hub.s.timeutil.ChangeTimeSec2Milli(base.SkillTime),
							CasterObjectID = base.ContextInfo.Creature.ObjectID,
							CCType = immune.CCType,
							Dispelable = false,
							ImmuneType = immune.ImmuneType,
							ImmutableStatType = immune.ImmutableStatType,
							MutableStatType = immune.MutableStatType
						});
					}
				}
				continue;
			}
			foreach (long abnormalImmuneSyncID in _abnormalImmuneSyncIDs)
			{
				base.ContextInfo.Creature.AbnormalControlUnit?.ImmuneManager.DispelAbnormal(abnormalImmuneSyncID, force: true);
			}
			_abnormalImmuneSyncIDs.Clear();
		}
	}

	public void ProcessAnimNotify(long currentTick)
	{
		if (currentTick - _lastNotifyProcessTick >= 50)
		{
			float elapsedTime = Hub.s.timeutil.ChangeTimeMilli2Sec(currentTick - base.StartTick - _totalHitStopMilliSec);
			if (base.ContextInfo.AnimInfo != null)
			{
				_notifyHandler?.Invoke(elapsedTime);
				_lastNotifyProcessTick = currentTick;
			}
		}
	}

	protected override bool Dispose(bool disposing)
	{
		if (!base.Dispose(disposing))
		{
			return false;
		}
		_notifyHandler = null;
		_hitLog?.Dispose();
		_abnormalImmuneSyncIDs.Clear();
		return true;
	}
}
