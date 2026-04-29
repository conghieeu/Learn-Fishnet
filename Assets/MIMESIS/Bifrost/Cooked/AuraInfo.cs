using System;
using System.Collections.Immutable;
using Bifrost.AuraSkillData;
using UnityEngine;

namespace Bifrost.Cooked
{
	public class AuraInfo
	{
		public readonly int MasterID;

		public readonly string AuraPrefabName;

		public readonly long AuraPrefabDuration;

		public readonly IHitCheck HitCheck;

		public readonly Vector3 AuraOffset;

		public readonly long AuraDuration;

		public ImmutableArray<HitTargetType> HitTargetTypes = ImmutableArray<HitTargetType>.Empty;

		public readonly int AbnormalMasterID;

		public readonly int LinkSkillTargetEffectDataID;

		public readonly bool RemoveAbnormalOnAuraEnd;

		public AuraInfo(AuraSkillData_MasterData masterData)
		{
			MasterID = masterData.id;
			AuraPrefabName = masterData.aura_prefab_name;
			AuraPrefabDuration = masterData.aura_prefab_duration;
			HitCheck = HitCheckBuilder.Build(masterData.hit_box_shape);
			AuraOffset = new Vector3(masterData.aura_offset[0], masterData.aura_offset[1], masterData.aura_offset[2]);
			AuraDuration = masterData.aura_duration;
			RemoveAbnormalOnAuraEnd = !masterData.is_once_abnormal_on_aura;
			ImmutableArray<HitTargetType>.Builder builder = ImmutableArray.CreateBuilder<HitTargetType>();
			foreach (int item in masterData.target_type)
			{
				if (Enum.IsDefined(typeof(HitTargetType), item))
				{
					builder.Add((HitTargetType)item);
				}
				else
				{
					Logger.RError($"[SkillSequenceInfo] unknown target_type {item}");
				}
			}
			HitTargetTypes = builder.ToImmutable();
			AbnormalMasterID = masterData.abnormal_master_id;
			LinkSkillTargetEffectDataID = masterData.link_skill_target_effect_data_id;
		}
	}
}
