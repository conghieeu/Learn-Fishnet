using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public class FieldSkillObject : VActor
{
	private FieldSkillMemberInfo _fieldSkillMemberInfo;

	private FieldSkillInfo _fieldSkillInfo;

	private ISkillContext? _parentSkillContext;

	private SkillSequenceInfo _skillSequenceInfo;

	private Dictionary<int, long> _actorHitTick = new Dictionary<int, long>();

	private int _index;

	private IMutableHitCheck _mutableHitcheck;

	private long _startTick;

	private bool _active;

	private Vector3? _surfaceNormal;

	private SkillSequenceHitLog _hitLog;

	public FieldSkillObject(int actorID, int masterID, int index, string actorName, PosWithRot position, bool isIndoor, Vector3? surfaceNormal, IVroom room, ISkillContext? skillContext, ReasonOfSpawn reasonOfSpawn)
		: base(ActorType.FieldSkill, actorID, masterID, actorName, position, isIndoor, room, 0L, reasonOfSpawn)
	{
		_fieldSkillInfo = Hub.s.dataman.ExcelDataManager.GetFieldSkillData(masterID);
		if (_fieldSkillInfo == null)
		{
			throw new Exception("FieldSkillObject : FieldSkillInfo is null");
		}
		if (!_fieldSkillInfo.FieldSkillMemberInfos.TryGetValue(index, out _fieldSkillMemberInfo))
		{
			throw new Exception("FieldSkillObject : FieldSkillMemberInfo is null");
		}
		_skillSequenceInfo = Hub.s.dataman.ExcelDataManager.GetSkillSequenceInfo(_fieldSkillMemberInfo.SequenceID);
		if (_skillSequenceInfo == null)
		{
			throw new Exception("FieldSkillObject : SkillSequenceInfo is null");
		}
		_mutableHitcheck = _fieldSkillMemberInfo.HitBoxShape.Clone();
		_parentSkillContext = skillContext;
		if (_fieldSkillInfo.Factions.Count > 0 && _fieldSkillInfo.Factions[0] != 0)
		{
			m_DefaultFactions = new HashSet<int>(_fieldSkillInfo.Factions);
		}
		else if (_parentSkillContext != null)
		{
			m_DefaultFactions = new HashSet<int>((_parentSkillContext?.ContextInfo.Creature).GetDefaultFactions());
		}
		ResetFaction();
		_index = index;
		_startTick = Hub.s.timeutil.GetCurrentTickMilliSec();
		_active = true;
		_surfaceNormal = surfaceNormal;
		_hitLog = new SkillSequenceHitLog(_skillSequenceInfo.HitCount, _fieldSkillMemberInfo.HitPeriod);
	}

	public override void Update(long deltaTick)
	{
		base.Update(deltaTick);
		long duration = _fieldSkillMemberInfo.Duration;
		long hitStartDelay = _fieldSkillMemberInfo.HitStartDelay;
		if (!_active || _startTick + hitStartDelay > Hub.s.timeutil.GetCurrentTickMilliSec())
		{
			return;
		}
		if (_fieldSkillMemberInfo.Duration > 0 && _startTick + duration < Hub.s.timeutil.GetCurrentTickMilliSec() && _active)
		{
			_active = false;
			VRoom.PendRemoveActor(base.ObjectID);
			return;
		}
		List<VCreature> creaturesInRange = VRoom.GetCreaturesInRange(base.PositionVector, 0f, _mutableHitcheck.CheckRadius, checkHeight: true);
		FieldSkillHitInputData fieldSkillHitInputData = new FieldSkillHitInputData(_fieldSkillInfo, _index, _skillSequenceInfo);
		foreach (VCreature item in creaturesInRange)
		{
			if (item == null)
			{
				continue;
			}
			VCreature vCreature = item;
			if (!_actorHitTick.TryGetValue(vCreature.ObjectID, out var value) || (_fieldSkillMemberInfo.DuplicateHit && value + _fieldSkillMemberInfo.HitPeriod <= Hub.s.timeutil.GetCurrentTickMilliSec()))
			{
				HitCheckPos posOrigin = new HitCheckPos
				{
					Start = base.PositionVector,
					End = base.PositionVector,
					AngleRad = base.Position.yaw.toRadian()
				};
				HitCheckPos posTarget = new HitCheckPos
				{
					Start = vCreature.PositionVector,
					End = vCreature.PositionVector,
					AngleRad = vCreature.Position.yaw.toRadian()
				};
				(bool, Vector3, Vector3) tuple = _mutableHitcheck.IsHit(posOrigin, vCreature.HitCheck, posTarget);
				if (tuple.Item1)
				{
					_actorHitTick[vCreature.ObjectID] = Hub.s.timeutil.GetCurrentTickMilliSec();
					fieldSkillHitInputData.Targets.Add(new TargetInfo
					{
						targetObjectID = vCreature.ObjectID,
						targetPosition = vCreature.PositionVector,
						hitPosition = tuple.Item2
					});
				}
			}
		}
		OnHit(fieldSkillHitInputData, VWorldCombatUtil.DefaultDamageImmuneChecker, VWorldCombatUtil.DefaultSkillPush(this));
	}

	public void OnHit(FieldSkillHitInputData hitInfo, IsDamageImmuned isDamageImmuned, SkillPushDelegate skillPush)
	{
		SkillSequenceInfo skillSequenceInfo = hitInfo.SkillSequenceInfo;
		if (skillSequenceInfo.MutableValue != 0L || skillSequenceInfo.AbnormalMasterIDs.Count() != 0)
		{
			List<TargetHitInfo> list = ProcessMutableValue(hitInfo, isDamageImmuned, skillPush);
			if (list.Count != 0)
			{
				FieldHitTargetSig fieldHitTargetSig = new FieldHitTargetSig
				{
					fieldSkillObjectID = base.ObjectID,
					fieldSkillMasterID = _fieldSkillInfo.MasterID,
					fieldSkillIndex = _index,
					skillSequenceMasterID = skillSequenceInfo.MasterID
				};
				fieldHitTargetSig.targetHitInfos.AddRange(list);
				SendInSight(fieldHitTargetSig);
			}
		}
	}

	private List<TargetHitInfo> ProcessMutableValue(FieldSkillHitInputData inputdata, IsDamageImmuned isDamageImmuned, SkillPushDelegate skillPush)
	{
		FieldSkillInfo fieldSkillInfo = inputdata.FieldSkillInfo;
		SkillSequenceInfo skillSequenceInfo = inputdata.SkillSequenceInfo;
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		List<TargetHitInfo> list = new List<TargetHitInfo>();
		List<(TargetInfo, VActor)> list2 = new List<(TargetInfo, VActor)>();
		foreach (TargetInfo target in inputdata.Targets)
		{
			int targetObjectID = target.targetObjectID;
			VActor vActor = VRoom.FindActorByObjectID(targetObjectID);
			if (vActor != null && vActor is VCreature && IsValidTarget(skillSequenceInfo.HitTargetTypes, vActor) && vActor.IsAliveStatus() && _hitLog.CheckHitTerm(targetObjectID, currentTickMilliSec))
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
					if (skillSequenceInfo.MutableStatType == MutableStatType.HP)
					{
						DamageAttribute damageAttribute = ((_parentSkillContext != null) ? item2.StatControlUnit.EstimateDamage(_parentSkillContext?.ContextInfo.Creature ?? null, MutableStatChangeCause.ActiveAttack, skillSequenceInfo.MasterID) : VWorldCombatUtil.CalculateTrueDamage(item2, skillSequenceInfo));
						if (damageAttribute.Damage > 0)
						{
							item2.StatControlUnit.ApplyDamage(new ApplyDamageArgs(this, item2, MutableStatChangeCause.FieldSkill, damageAttribute.Damage, damageAttribute.GroggyValue, fieldSkillInfo.MasterID, skillSequenceInfo.MasterID, skillSequenceInfo.HitType));
							num += hitInfo.damage;
						}
						damageAttribute.Convert2HitInfo(ref hitInfo);
						list.Add(hitInfo);
					}
					else if (skillSequenceInfo.MutableStatType == MutableStatType.Conta)
					{
						item2.StatControlUnit.IncreaseConta(skillSequenceInfo.MutableValue);
					}
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
				}
			}
			if (skillSequenceInfo.SequenceType == SkillSeqType.Hit || flag || skillSequenceInfo.AbnormalMasterIDs.Count() == 0 || skillSequenceInfo.AbnormalMasterIDs.Count() <= 0)
			{
				continue;
			}
			if (skillSequenceInfo.AbnormalApplyType == AbnormalApplyType.All)
			{
				ImmutableArray<int>.Enumerator enumerator3 = skillSequenceInfo.AbnormalMasterIDs.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					int current3 = enumerator3.Current;
					if (current3 > 0 && Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(current3) != null)
					{
						item2.AbnormalControlUnit?.AppendAbnormal(base.ObjectID, current3);
					}
				}
			}
			else if (skillSequenceInfo.AbnormalApplyType == AbnormalApplyType.SelectiveRandom)
			{
				int num2 = skillSequenceInfo.AbnormalMasterIDs[SimpleRandUtil.Next(0, skillSequenceInfo.AbnormalMasterIDs.Count())];
				if (num2 > 0 && Hub.s.dataman.ExcelDataManager.GetAbnormalInfo(num2) != null)
				{
					item2.AbnormalControlUnit?.AppendAbnormal(base.ObjectID, num2);
				}
			}
		}
		return list;
	}

	public override void CollectDebugInfo(ref DebugInfoSig sig)
	{
		CollectHitCheckInfo(_mutableHitcheck, ref sig);
	}

	protected void GetInfo(ref FieldSkillObjectInfo info)
	{
		ActorBaseInfo info2 = info;
		GetActorBaseInfo(ref info2);
		info.parentActorID = ((_parentSkillContext != null) ? _parentSkillContext.ContextInfo.Creature.ObjectID : 0);
		info.fieldSkillMasterID = _fieldSkillInfo.MasterID;
		info.fieldSkillIndex = _index;
		info.endTime = _startTick + _fieldSkillMemberInfo.Duration;
		info.surfaceNormal = (_surfaceNormal.HasValue ? new Vector3(_surfaceNormal.Value.x, _surfaceNormal.Value.y, _surfaceNormal.Value.z) : default(Vector3));
	}

	public override void FillSightInSig(ref SightInSig sig)
	{
		FieldSkillObjectInfo info = new FieldSkillObjectInfo();
		GetInfo(ref info);
		sig.fieldSkillObjectInfos.Add(info);
	}

	public override bool IsAliveStatus()
	{
		return true;
	}

	public override SendResult SendToMe(IMsg msg)
	{
		return SendResult.Success;
	}

	public int GetFieldSkillMasterID()
	{
		return _fieldSkillInfo.MasterID;
	}
}
