using Bifrost.ConstEnum;
using Bifrost.FieldSkillData;
using UnityEngine;

namespace Bifrost.Cooked
{
	public class FieldSkillMemberInfo
	{
		public readonly int Index;

		public readonly string PrefabName;

		public readonly string DecalId;

		public readonly string DecalColorId;

		public readonly bool IsDecalRotateRandomly;

		public readonly float DecalDistanceFromSpawnPoint = 1f;

		public readonly long DecalDurationMSec;

		public readonly long DecalFadeoutMSec;

		public readonly long PrefabDisplayDuration;

		public readonly IHitCheck HitBoxShape;

		public readonly FieldAttachType FieldAttachType;

		public readonly Vector3 FieldOffset;

		public readonly long Duration;

		public readonly long HitPeriod;

		public readonly long HitStartDelay;

		public readonly bool DuplicateHit;

		public readonly int SequenceID;

		public FieldSkillMemberInfo(FieldSkillData_field_info info)
		{
			Index = info.field_id;
			PrefabName = info.field_prefab_name;
			if (info.decal_prefab_name == null || info.decal_prefab_name.Count < 2)
			{
				DecalId = "";
				DecalColorId = "";
				DecalDistanceFromSpawnPoint = 0f;
				DecalDurationMSec = 0L;
				DecalFadeoutMSec = 0L;
			}
			else
			{
				DecalId = info.decal_prefab_name[0].Trim();
				DecalColorId = info.decal_prefab_name[1].Trim();
				IsDecalRotateRandomly = info.decal_prefab_name[2].Trim() == "1";
				DecalDistanceFromSpawnPoint = float.Parse(info.decal_prefab_name[3].Trim());
				DecalDurationMSec = info.decal_prefab_duration[0];
				DecalFadeoutMSec = info.decal_prefab_duration[1];
			}
			PrefabDisplayDuration = info.field_prefab_duration;
			HitBoxShape = HitCheckBuilder.Build(info.hit_box_shape);
			FieldOffset = new Vector3(info.field_offset[0], info.field_offset[1], info.field_offset[2]);
			Duration = info.field_duration;
			HitPeriod = info.hit_tick;
			HitStartDelay = info.hit_start_delay;
			DuplicateHit = info.is_hit_duplicatable;
			SequenceID = info.field_skill_sequence_id;
		}
	}
}
