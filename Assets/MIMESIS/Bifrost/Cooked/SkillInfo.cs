using System;
using System.Collections.Immutable;
using System.Globalization;
using Bifrost.CharacterSkillData;
using Bifrost.ConstEnum;
using Bifrost.MonsterSkillData;

namespace Bifrost.Cooked
{
	public class SkillInfo
	{
		public readonly int MasterID;

		public readonly string Name;

		public readonly string SkillAnimationState;

		public readonly string SkillCancelAnimationState;

		public readonly bool CastableWhenMoving;

		public readonly SkillTargetingType TargetingType;

		public readonly int HitCount;

		public readonly int TargetCount;

		public readonly float MinRange;

		public readonly float MaxRange;

		public readonly float AllowRange;

		public readonly int Cooltime;

		public readonly SkillCoolTimeType CooltimeType;

		public readonly SkillTargetFocusType TargetFocusRule;

		public ImmutableList<ImmuneElementInfo> Immunes;

		public readonly ImmutableDictionary<int, int> SkillHitboxSequenceDictionary;

		public readonly ImmutableDictionary<int, int> SkillProjectileEventDictionary;

		public readonly ImmutableDictionary<int, int> SkillFieldSkillEventDictionary;

		public readonly bool AllowMove;

		public readonly bool HideHandItemWhenSkillCast;

		public readonly string DeadCameraSocketName;

		public readonly string DeadCameraLookAt;

		public readonly float DeadCameraBlendTime;

		public readonly int DecGauge;

		public readonly DecreaseDurabilityType DecDurabilityTypeWhenSkillUse;

		public readonly ConsumeItemType ConsumeItemType;

		public readonly BulletType BulletType;

		public readonly int DecDurabilityPerUse;

		public readonly bool ShowDestroyEffect;

		public SkillInfo(CharacterSkillData_MasterData masterData)
		{
			MasterID = masterData.id;
			Name = masterData.name;
			SkillAnimationState = masterData.skill_animation_state_name;
			SkillCancelAnimationState = masterData.skill_cancel_animation_state_name;
			CastableWhenMoving = masterData.is_moving_skill;
			TargetingType = (SkillTargetingType)masterData.target_type;
			HitCount = masterData.hit_count;
			TargetCount = masterData.target_count;
			MinRange = masterData.min_range;
			MaxRange = masterData.max_range;
			AllowRange = masterData.allow_range;
			Cooltime = masterData.cooltime;
			CooltimeType = (SkillCoolTimeType)masterData.cooltime_type;
			DecGauge = masterData.dec_gauge;
			TargetFocusRule = SkillTargetFocusType.Invalid;
			ShowDestroyEffect = masterData.use_mesh_break_by_durability;
			ImmutableList<ImmuneElementInfo>.Builder builder = ImmutableList.CreateBuilder<ImmuneElementInfo>();
			foreach (string item in masterData.immune)
			{
				builder.Add(new ImmuneElementInfo(item));
			}
			Immunes = builder.ToImmutable();
			ImmutableDictionary<int, int>.Builder builder2 = ImmutableDictionary.CreateBuilder<int, int>();
			foreach (string item2 in masterData.skill_sequence_set)
			{
				if (!string.IsNullOrEmpty(item2))
				{
					string[] array = item2.Split('/');
					int key = Convert.ToInt32(array[0].Trim());
					int value = Convert.ToInt32(array[1].Trim());
					builder2.Add(key, value);
				}
			}
			SkillHitboxSequenceDictionary = builder2.ToImmutable();
			ImmutableDictionary<int, int>.Builder builder3 = ImmutableDictionary.CreateBuilder<int, int>();
			foreach (string item3 in masterData.projectile_event_set)
			{
				if (!string.IsNullOrEmpty(item3))
				{
					string[] array2 = item3.Split('/');
					int key2 = Convert.ToInt32(array2[0].Trim());
					int value2 = Convert.ToInt32(array2[1].Trim());
					builder3.Add(key2, value2);
				}
			}
			SkillProjectileEventDictionary = builder3.ToImmutable();
			ImmutableDictionary<int, int>.Builder builder4 = ImmutableDictionary.CreateBuilder<int, int>();
			foreach (string item4 in masterData.field_skill_event_set)
			{
				if (!string.IsNullOrEmpty(item4))
				{
					string[] array3 = item4.Split('/');
					int key3 = Convert.ToInt32(array3[0].Trim());
					int value3 = Convert.ToInt32(array3[1].Trim());
					builder4.Add(key3, value3);
				}
			}
			SkillFieldSkillEventDictionary = builder4.ToImmutable();
			AllowMove = masterData.is_moving_skill;
			HideHandItemWhenSkillCast = masterData.hide_hand_item;
			if (masterData.skill_dead_camera != null && masterData.skill_dead_camera.Count >= 3)
			{
				DeadCameraSocketName = masterData.skill_dead_camera[0].Trim();
				DeadCameraLookAt = masterData.skill_dead_camera[1].Trim();
				DeadCameraBlendTime = (string.IsNullOrWhiteSpace(masterData.skill_dead_camera[2]) ? 0f : float.Parse(masterData.skill_dead_camera[2].Trim(), CultureInfo.InvariantCulture));
			}
			else
			{
				DeadCameraSocketName = string.Empty;
				DeadCameraLookAt = string.Empty;
				DeadCameraBlendTime = 0f;
			}
			ConsumeItemType = (ConsumeItemType)masterData.consume_item_type;
			BulletType = (BulletType)masterData.bullet_type;
			DecDurabilityTypeWhenSkillUse = (DecreaseDurabilityType)masterData.dec_durability_type;
			DecDurabilityPerUse = masterData.dec_durability_per_use;
		}

		public SkillInfo(MonsterSkillData_MasterData masterData)
		{
			MasterID = masterData.id;
			Name = masterData.name;
			SkillAnimationState = masterData.skill_animation_state_name;
			SkillCancelAnimationState = masterData.skill_cancel_animation_state_name;
			CastableWhenMoving = masterData.is_moving_skill;
			TargetingType = (SkillTargetingType)masterData.target_type;
			HitCount = masterData.hit_count;
			TargetCount = masterData.target_count;
			MinRange = masterData.min_range;
			MaxRange = masterData.max_range;
			AllowRange = masterData.allow_range;
			Cooltime = masterData.cooltime;
			CooltimeType = (SkillCoolTimeType)masterData.cooltime_type;
			TargetFocusRule = (SkillTargetFocusType)masterData.target_focus_rule;
			ImmutableList<ImmuneElementInfo>.Builder builder = ImmutableList.CreateBuilder<ImmuneElementInfo>();
			foreach (string item in masterData.immune)
			{
				builder.Add(new ImmuneElementInfo(item));
			}
			Immunes = builder.ToImmutable();
			ImmutableDictionary<int, int>.Builder builder2 = ImmutableDictionary.CreateBuilder<int, int>();
			foreach (string item2 in masterData.skill_sequence_set)
			{
				if (!string.IsNullOrEmpty(item2))
				{
					string[] array = item2.Split('/');
					int key = Convert.ToInt32(array[0].Trim());
					int value = Convert.ToInt32(array[1].Trim());
					builder2.Add(key, value);
				}
			}
			SkillHitboxSequenceDictionary = builder2.ToImmutable();
			ImmutableDictionary<int, int>.Builder builder3 = ImmutableDictionary.CreateBuilder<int, int>();
			foreach (string item3 in masterData.projectile_event_set)
			{
				if (!string.IsNullOrEmpty(item3))
				{
					string[] array2 = item3.Split('/');
					int key2 = Convert.ToInt32(array2[0].Trim());
					int value2 = Convert.ToInt32(array2[1].Trim());
					builder3.Add(key2, value2);
				}
			}
			SkillProjectileEventDictionary = builder3.ToImmutable();
			ImmutableDictionary<int, int>.Builder builder4 = ImmutableDictionary.CreateBuilder<int, int>();
			foreach (string item4 in masterData.field_skill_event_set)
			{
				if (!string.IsNullOrEmpty(item4))
				{
					string[] array3 = item4.Split('/');
					int key3 = Convert.ToInt32(array3[0].Trim());
					int value3 = Convert.ToInt32(array3[1].Trim());
					builder4.Add(key3, value3);
				}
			}
			SkillFieldSkillEventDictionary = builder4.ToImmutable();
			AllowMove = masterData.is_moving_skill;
			HideHandItemWhenSkillCast = masterData.hide_hand_item;
			if (masterData.skill_dead_camera != null && masterData.skill_dead_camera.Count >= 3)
			{
				DeadCameraSocketName = masterData.skill_dead_camera[0].Trim();
				DeadCameraLookAt = masterData.skill_dead_camera[1].Trim();
				DeadCameraBlendTime = (string.IsNullOrWhiteSpace(masterData.skill_dead_camera[2]) ? 0f : float.Parse(masterData.skill_dead_camera[2].Trim(), CultureInfo.InvariantCulture));
			}
			else
			{
				DeadCameraSocketName = string.Empty;
				DeadCameraLookAt = string.Empty;
				DeadCameraBlendTime = 0f;
			}
		}

		public bool IsGrabSkill()
		{
			foreach (int value in SkillHitboxSequenceDictionary.Values)
			{
				SkillSequenceInfo skillSequenceInfo = Hub.s.dataman.ExcelDataManager.GetSkillSequenceInfo(value);
				if (skillSequenceInfo != null && skillSequenceInfo.LinkedGrabMasterID > 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
