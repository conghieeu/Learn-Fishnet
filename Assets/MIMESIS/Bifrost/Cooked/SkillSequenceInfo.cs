using System;
using System.Collections.Immutable;
using Bifrost.CharacterSkillSequenceData;
using Bifrost.ConstEnum;
using Bifrost.FieldSkillSequenceData;
using Bifrost.MonsterSkillSequenceData;
using ReluProtocol.Enum;
using UnityEngine;

namespace Bifrost.Cooked
{
	public class SkillSequenceInfo
	{
		public readonly int MasterID;

		public readonly SkillSeqType SequenceType;

		public readonly HitType HitType;

		public readonly MutableStatType MutableStatType;

		public readonly long MutableValue;

		public readonly int HitCount;

		public readonly long AbnormalTriggerValue;

		public ImmutableArray<int> AbnormalMasterIDs = ImmutableArray<int>.Empty;

		public ImmutableArray<HitTargetType> HitTargetTypes = ImmutableArray<HitTargetType>.Empty;

		public readonly string BattleActionKey;

		public readonly BattleActionDistanceType BattleActionDistType;

		public readonly int SkillTagetEffectId;

		public readonly int LinkedGrabMasterID;

		public readonly Vector3 RagDollForceDirection;

		public readonly int LinkedFieldSkillMasterID;

		public readonly AbnormalApplyType AbnormalApplyType;

		public SkillSequenceInfo(CharacterSkillSequenceData_MasterData masterData)
		{
			MasterID = masterData.id;
			SequenceType = (SkillSeqType)masterData.seq_type;
			HitType = (HitType)masterData.hit_type;
			if (!StringUtil.ConvertStringToEnum<MutableStatType>(masterData.mutable_stat_type, out var result))
			{
				Logger.RError("[SkillSequenceInfo] unknown mutable_stat_type " + masterData.mutable_stat_type);
				MutableStatType = MutableStatType.HP;
			}
			else
			{
				MutableStatType = result;
			}
			MutableValue = masterData.mutable_stat_modify_value;
			HitCount = masterData.hit_count;
			AbnormalTriggerValue = masterData.abnormal_trigger_value;
			AbnormalApplyType = (AbnormalApplyType)masterData.abnormal_apply_type;
			ImmutableArray<int>.Builder builder = ImmutableArray.CreateBuilder<int>();
			foreach (int item in masterData.abnormal_master_id)
			{
				if (item > 0)
				{
					builder.Add(item);
				}
			}
			AbnormalMasterIDs = builder.ToImmutable();
			ImmutableArray<HitTargetType>.Builder builder2 = ImmutableArray.CreateBuilder<HitTargetType>();
			foreach (int item2 in masterData.target_type)
			{
				if (Enum.IsDefined(typeof(HitTargetType), item2))
				{
					builder2.Add((HitTargetType)item2);
				}
				else
				{
					Logger.RError($"[SkillSequenceInfo] unknown target_type {item2}");
				}
			}
			HitTargetTypes = builder2.ToImmutable();
			BattleActionKey = masterData.battle_action_key;
			BattleActionDistType = (BattleActionDistanceType)masterData.move_origin;
			SkillTagetEffectId = masterData.link_skill_target_effect_data_id;
			LinkedGrabMasterID = masterData.grab_master_id;
			RagDollForceDirection = new Vector3(masterData.rag_doll_external_force[0], masterData.rag_doll_external_force[1], masterData.rag_doll_external_force[2]);
		}

		public SkillSequenceInfo(MonsterSkillSequenceData_MasterData masterData)
		{
			MasterID = masterData.id;
			SequenceType = (SkillSeqType)masterData.seq_type;
			HitType = (HitType)masterData.hit_type;
			if (!StringUtil.ConvertStringToEnum<MutableStatType>(masterData.mutable_stat_type, out var result))
			{
				Logger.RError("[SkillSequenceInfo] unknown mutable_stat_type " + masterData.mutable_stat_type);
				MutableStatType = MutableStatType.HP;
			}
			else
			{
				MutableStatType = result;
			}
			MutableValue = masterData.mutable_stat_modify_value;
			HitCount = masterData.hit_count;
			AbnormalTriggerValue = 0L;
			AbnormalApplyType = (AbnormalApplyType)masterData.abnormal_apply_type;
			ImmutableArray<int>.Builder builder = ImmutableArray.CreateBuilder<int>();
			foreach (int item in masterData.abnormal_master_id)
			{
				if (item > 0)
				{
					builder.Add(item);
				}
			}
			AbnormalMasterIDs = builder.ToImmutable();
			ImmutableArray<HitTargetType>.Builder builder2 = ImmutableArray.CreateBuilder<HitTargetType>();
			foreach (int item2 in masterData.target_type)
			{
				if (Enum.IsDefined(typeof(HitTargetType), item2))
				{
					builder2.Add((HitTargetType)item2);
				}
				else
				{
					Logger.RError($"[SkillSequenceInfo] unknown target_type {item2}");
				}
			}
			HitTargetTypes = builder2.ToImmutable();
			BattleActionKey = masterData.battle_action_key;
			BattleActionDistType = (BattleActionDistanceType)masterData.move_origin;
			SkillTagetEffectId = masterData.link_skill_target_effect_data_id;
			LinkedGrabMasterID = masterData.grab_master_id;
			RagDollForceDirection = new Vector3(masterData.rag_doll_external_force[0], masterData.rag_doll_external_force[1], masterData.rag_doll_external_force[2]);
		}

		public SkillSequenceInfo(FieldSkillSequenceData_MasterData masterData)
		{
			MasterID = masterData.id;
			SequenceType = (SkillSeqType)masterData.seq_type;
			HitType = (HitType)masterData.hit_type;
			if (!StringUtil.ConvertStringToEnum<MutableStatType>(masterData.mutable_stat_type, out var result))
			{
				Logger.RError("[SkillSequenceInfo] unknown mutable_stat_type " + masterData.mutable_stat_type);
				MutableStatType = MutableStatType.HP;
			}
			else
			{
				MutableStatType = result;
			}
			MutableValue = masterData.mutable_stat_modify_value;
			HitCount = masterData.hit_count;
			AbnormalTriggerValue = 0L;
			AbnormalApplyType = (AbnormalApplyType)masterData.abnormal_apply_type;
			ImmutableArray<int>.Builder builder = ImmutableArray.CreateBuilder<int>();
			foreach (int item in masterData.abnormal_master_id)
			{
				if (item > 0)
				{
					builder.Add(item);
				}
			}
			AbnormalMasterIDs = builder.ToImmutable();
			ImmutableArray<HitTargetType>.Builder builder2 = ImmutableArray.CreateBuilder<HitTargetType>();
			foreach (int item2 in masterData.target_type)
			{
				if (Enum.IsDefined(typeof(HitTargetType), item2))
				{
					builder2.Add((HitTargetType)item2);
				}
				else
				{
					Logger.RError($"[SkillSequenceInfo] unknown target_type {item2}");
				}
			}
			HitTargetTypes = builder2.ToImmutable();
			BattleActionKey = masterData.battle_action_key;
			BattleActionDistType = (BattleActionDistanceType)masterData.move_origin;
			SkillTagetEffectId = masterData.link_skill_target_effect_data_id;
			RagDollForceDirection = new Vector3(masterData.rag_doll_external_force[0], masterData.rag_doll_external_force[1], masterData.rag_doll_external_force[2]);
		}
	}
}
