using System.Collections.Generic;
using System.Collections.Immutable;
using Bifrost.ConstEnum;
using Bifrost.ProjectileData;

public class ProjectileInfo
{
	public readonly int MasterID;

	public readonly ProjectileType ProjectileType;

	public readonly string PhysicsPrefabId = string.Empty;

	public readonly float PhysicsSpeed;

	public readonly float Gravity = -15f;

	public readonly float PhysicsLifetime;

	public readonly long PhysicsLifetimeMillisec;

	public readonly float RaycastDistance;

	public readonly List<int> HitFieldSkillMasterIDCandidates = new List<int>();

	public readonly bool PhysicsProjectileDestroy;

	public readonly float Radius;

	public readonly int PenetrateCount;

	public readonly MutableStatType MutableStatType;

	public readonly long MutableValue;

	public readonly bool Homing;

	public ImmutableArray<int> AbnormalMasterIDs;

	public readonly AbnormalApplyType AbnormalApplyType;

	public readonly string BattleActionKey;

	public ImmutableArray<HitTargetType> HitTargetTypes = ImmutableArray<HitTargetType>.Empty;

	public readonly string ProjectileCollisionEffectName;

	public readonly int LinkSkillTargetEffectDataId;

	public readonly int SpawnItemMasterIDonCollision;

	public readonly bool CanInheritDurability;

	public readonly int DecreaseDurabilityOnCollision;

	public readonly bool DestroyEffectOnDurabilityEnd;

	public readonly bool RemoveFromInventory;

	public readonly float SelfHitGraceDistance;

	public ProjectileInfo(ProjectileData_MasterData data)
	{
		MasterID = data.id;
		ProjectileType = (ProjectileType)data.projectile_type;
		PhysicsPrefabId = data.physics_projectile_prefab_name;
		PhysicsSpeed = data.physics_projectile_speed;
		Gravity = data.physics_projectile_gravity_value;
		PhysicsLifetime = data.physics_projectile_lifetime / 1000;
		PhysicsLifetimeMillisec = data.physics_projectile_lifetime;
		RaycastDistance = data.raycast_projectile_length;
		HitFieldSkillMasterIDCandidates = data.projectile_hit_field_skill_id;
		PhysicsProjectileDestroy = data.physics_projectile_destroy;
		if (ProjectileType == ProjectileType.Physics)
		{
			Radius = data.radius;
		}
		else
		{
			PenetrateCount = data.penetrate_count;
		}
		if (!StringUtil.ConvertStringToEnum<MutableStatType>(data.mutable_stat_type, out var result))
		{
			Logger.RError("[SkillSequenceInfo] unknown mutable_stat_type " + data.mutable_stat_type);
			MutableStatType = MutableStatType.HP;
		}
		else
		{
			MutableStatType = result;
		}
		MutableValue = data.mutable_stat_modify_value;
		ImmutableArray<int>.Builder builder = ImmutableArray.CreateBuilder<int>();
		foreach (int item in data.abnormal_master_id)
		{
			if (item != 0)
			{
				builder.Add(item);
			}
		}
		AbnormalMasterIDs = builder.ToImmutable();
		AbnormalApplyType = (AbnormalApplyType)data.abnormal_apply_type;
		BattleActionKey = data.battle_action_key;
		Homing = data.homing;
		ImmutableArray<HitTargetType>.Builder builder2 = ImmutableArray.CreateBuilder<HitTargetType>();
		builder2.Add(HitTargetType.ALL);
		HitTargetTypes = builder2.ToImmutable();
		ProjectileCollisionEffectName = data.projectile_collision_effect_name;
		LinkSkillTargetEffectDataId = data.link_skill_target_effect_data_id;
		SpawnItemMasterIDonCollision = data.projectile_collision_spawn_item_id;
		CanInheritDurability = data.is_same_spawn_item_inherit_durability;
		DecreaseDurabilityOnCollision = data.dec_durability_by_collision;
		DestroyEffectOnDurabilityEnd = data.projectile_collision_durability_destroy;
		RemoveFromInventory = data.remove_from_inventory_when_throw;
		SelfHitGraceDistance = data.self_hit_grace_distance;
		if (!RemoveFromInventory && SpawnItemMasterIDonCollision > 0)
		{
			Logger.RError($"[ProjectileInfo] Invalid RemoveFromInventory value. RemoveFromInventory must be true when SpawnItemMasterIDonCollision is set. ProjectileMasterID: {MasterID}");
		}
	}
}
