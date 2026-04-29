using Bifrost.ConstEnum;
using Bifrost.Cooked;
using ReluProtocol;

public static class VWorldCombatUtil
{
	public static DamageAttribute CalculateDamage(VCreature victim, VActor? attacker, int skillSeqMasterID)
	{
		if (!victim.VRoom.DamageAppliable())
		{
			return new DamageAttribute(0L, 0L, critical: false);
		}
		SkillSequenceInfo skillSequenceInfo = Hub.s.dataman.ExcelDataManager.GetSkillSequenceInfo(skillSeqMasterID);
		if (skillSequenceInfo == null)
		{
			return new DamageAttribute(0L, 0L, critical: false);
		}
		long num = skillSequenceInfo.MutableValue * -1;
		float num2 = 1f;
		if (attacker != null)
		{
			if (attacker.StatControlUnit == null || victim.StatControlUnit == null)
			{
				Logger.RWarn($"[VWorldCombatUtil] CalculateDamage - Attacker StatControlUnit is null. Attacker ObjectID: {attacker.ObjectID}");
			}
			else
			{
				num2 = 1f + (float)((double)(attacker.StatControlUnit.GetSpecificStatValue(StatType.Attack) - victim.StatControlUnit.GetSpecificStatValue(StatType.Defense)) * 0.0001);
			}
		}
		float num3 = (float)num * num2;
		long abnormalTriggerValue = skillSequenceInfo.AbnormalTriggerValue;
		return new DamageAttribute((long)num3, abnormalTriggerValue, critical: false);
	}

	public static ImmuneCheckResult DefaultDamageImmuneChecker(VActor target)
	{
		AbnormalController? abnormalControlUnit = target.AbnormalControlUnit;
		if (abnormalControlUnit != null && abnormalControlUnit.IsImmortal())
		{
			return new ImmuneCheckResult(ImmuneType.Immortal, immuned: true);
		}
		AbnormalController? abnormalControlUnit2 = target.AbnormalControlUnit;
		if (abnormalControlUnit2 != null && abnormalControlUnit2.IsStatsImmune(MutableStatType.HP))
		{
			return new ImmuneCheckResult(ImmuneType.TargetMutableStat, immuned: true);
		}
		return new ImmuneCheckResult(ImmuneType.None, immuned: false);
	}

	public static SkillPushDelegate DefaultSkillPush(VActor caster)
	{
		return delegate(TargetHitInfo hitInfo, VActor targetActor, string battleActionKey, BattleActionDistanceType distType)
		{
			CCType actionAbnormalHitType = hitInfo.actionAbnormalHitType;
			if (actionAbnormalHitType != CCType.None)
			{
				BattleActionInfo battleActionData = Hub.s.dataman.ExcelDataManager.GetBattleActionData(battleActionKey);
				if (battleActionData != null)
				{
					long num = battleActionData.MoveTime + battleActionData.DownTime;
					PosWithRot targetPos = hitInfo.basePosition.Clone();
					AbnormalController? abnormalControlUnit = targetActor.AbnormalControlUnit;
					if (abnormalControlUnit == null || abnormalControlUnit.CalculatePushActionPos(caster.PositionVector, caster.Position.yaw, distType, battleActionData, ref targetPos))
					{
						AbnormalController? abnormalControlUnit2 = targetActor.AbnormalControlUnit;
						if (abnormalControlUnit2 != null && abnormalControlUnit2.AddCC(new AbnormalCCInputArgs
						{
							SyncID = targetActor.AbnormalControlUnit.GetNewAbnormalSyncID(),
							CCType = actionAbnormalHitType,
							CasterObjectID = caster.ObjectID,
							Dispelable = true,
							Duration = num,
							PushTime = battleActionData.MoveTime,
							DownTime = battleActionData.DownTime,
							TargetPos = targetPos,
							CurrentPos = targetActor.Position
						}))
						{
							hitInfo.pushTime = battleActionData.MoveTime;
							hitInfo.hitDelay = num;
							targetPos.CopyTo(hitInfo.hitPosition);
						}
					}
				}
			}
		};
	}

	public static DamageAttribute CalculateTrueDamage(VActor victim, SkillSequenceInfo seqInfo)
	{
		if (!(victim.VRoom is DungeonRoom))
		{
			return new DamageAttribute(0L, 0L, critical: false);
		}
		return new DamageAttribute(seqInfo.MutableValue * -1, 0L, critical: false);
	}
}
