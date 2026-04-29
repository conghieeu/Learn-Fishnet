using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ProjectileData
{
	public class ProjectileData_MasterData : ISchema
	{
		public int id;

		public int projectile_type;

		public float radius;

		public int seq_type;

		public int hit_type;

		public List<int> target_type = new List<int>();

		public string physics_projectile_prefab_name = string.Empty;

		public float physics_projectile_speed;

		public float physics_projectile_gravity_value;

		public long physics_projectile_lifetime;

		public float raycast_projectile_length;

		public int penetrate_count;

		public string battle_action_key = string.Empty;

		public int abnormal_apply_type;

		public List<int> abnormal_master_id = new List<int>();

		public string mutable_stat_type = string.Empty;

		public int mutable_stat_modify_value;

		public string projectile_collision_effect_name = string.Empty;

		public int link_skill_target_effect_data_id;

		public bool homing;

		public List<int> projectile_hit_field_skill_id = new List<int>();

		public bool physics_projectile_destroy;

		public int projectile_collision_spawn_item_id;

		public bool is_same_spawn_item_inherit_durability;

		public int dec_durability_by_collision;

		public bool projectile_collision_durability_destroy;

		public bool remove_from_inventory_when_throw;

		public long self_hit_grace_distance;

		public ProjectileData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ProjectileData_MasterData()
			: base(1290410187u, "ProjectileData_MasterData")
		{
		}

		public void CopyTo(ProjectileData_MasterData dest)
		{
			dest.id = id;
			dest.projectile_type = projectile_type;
			dest.radius = radius;
			dest.seq_type = seq_type;
			dest.hit_type = hit_type;
			dest.target_type.Clear();
			foreach (int item in target_type)
			{
				dest.target_type.Add(item);
			}
			dest.physics_projectile_prefab_name = physics_projectile_prefab_name;
			dest.physics_projectile_speed = physics_projectile_speed;
			dest.physics_projectile_gravity_value = physics_projectile_gravity_value;
			dest.physics_projectile_lifetime = physics_projectile_lifetime;
			dest.raycast_projectile_length = raycast_projectile_length;
			dest.penetrate_count = penetrate_count;
			dest.battle_action_key = battle_action_key;
			dest.abnormal_apply_type = abnormal_apply_type;
			dest.abnormal_master_id.Clear();
			foreach (int item2 in abnormal_master_id)
			{
				dest.abnormal_master_id.Add(item2);
			}
			dest.mutable_stat_type = mutable_stat_type;
			dest.mutable_stat_modify_value = mutable_stat_modify_value;
			dest.projectile_collision_effect_name = projectile_collision_effect_name;
			dest.link_skill_target_effect_data_id = link_skill_target_effect_data_id;
			dest.homing = homing;
			dest.projectile_hit_field_skill_id.Clear();
			foreach (int item3 in projectile_hit_field_skill_id)
			{
				dest.projectile_hit_field_skill_id.Add(item3);
			}
			dest.physics_projectile_destroy = physics_projectile_destroy;
			dest.projectile_collision_spawn_item_id = projectile_collision_spawn_item_id;
			dest.is_same_spawn_item_inherit_durability = is_same_spawn_item_inherit_durability;
			dest.dec_durability_by_collision = dec_durability_by_collision;
			dest.projectile_collision_durability_destroy = projectile_collision_durability_destroy;
			dest.remove_from_inventory_when_throw = remove_from_inventory_when_throw;
			dest.self_hit_grace_distance = self_hit_grace_distance;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += Serializer.GetLength(projectile_type);
			num += Serializer.GetLength(radius);
			num += Serializer.GetLength(seq_type);
			num += Serializer.GetLength(hit_type);
			num += 4;
			foreach (int item in target_type)
			{
				num += Serializer.GetLength(item);
			}
			num += Serializer.GetLength(physics_projectile_prefab_name);
			num += Serializer.GetLength(physics_projectile_speed);
			num += Serializer.GetLength(physics_projectile_gravity_value);
			num += Serializer.GetLength(physics_projectile_lifetime);
			num += Serializer.GetLength(raycast_projectile_length);
			num += Serializer.GetLength(penetrate_count);
			num += Serializer.GetLength(battle_action_key);
			num += Serializer.GetLength(abnormal_apply_type);
			num += 4;
			foreach (int item2 in abnormal_master_id)
			{
				num += Serializer.GetLength(item2);
			}
			num += Serializer.GetLength(mutable_stat_type);
			num += Serializer.GetLength(mutable_stat_modify_value);
			num += Serializer.GetLength(projectile_collision_effect_name);
			num += Serializer.GetLength(link_skill_target_effect_data_id);
			num += Serializer.GetLength(homing);
			num += 4;
			foreach (int item3 in projectile_hit_field_skill_id)
			{
				num += Serializer.GetLength(item3);
			}
			num += Serializer.GetLength(physics_projectile_destroy);
			num += Serializer.GetLength(projectile_collision_spawn_item_id);
			num += Serializer.GetLength(is_same_spawn_item_inherit_durability);
			num += Serializer.GetLength(dec_durability_by_collision);
			num += Serializer.GetLength(projectile_collision_durability_destroy);
			num += Serializer.GetLength(remove_from_inventory_when_throw);
			return num + Serializer.GetLength(self_hit_grace_distance);
		}

		public override bool Load(BinaryReader br)
		{
			uint uintValue = 0u;
			Serializer.Load(br, ref uintValue);
			if (MsgID != (uint)IPAddress.NetworkToHostOrder((int)uintValue))
			{
				return false;
			}
			try
			{
				LoadInternal(br);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void LoadInternal(BinaryReader br)
		{
			Serializer.Load(br, ref id);
			Serializer.Load(br, ref projectile_type);
			Serializer.Load(br, ref radius);
			Serializer.Load(br, ref seq_type);
			Serializer.Load(br, ref hit_type);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				int intValue2 = 0;
				Serializer.Load(br, ref intValue2);
				target_type.Add(intValue2);
			}
			Serializer.Load(br, ref physics_projectile_prefab_name);
			Serializer.Load(br, ref physics_projectile_speed);
			Serializer.Load(br, ref physics_projectile_gravity_value);
			Serializer.Load(br, ref physics_projectile_lifetime);
			Serializer.Load(br, ref raycast_projectile_length);
			Serializer.Load(br, ref penetrate_count);
			Serializer.Load(br, ref battle_action_key);
			Serializer.Load(br, ref abnormal_apply_type);
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				int intValue4 = 0;
				Serializer.Load(br, ref intValue4);
				abnormal_master_id.Add(intValue4);
			}
			Serializer.Load(br, ref mutable_stat_type);
			Serializer.Load(br, ref mutable_stat_modify_value);
			Serializer.Load(br, ref projectile_collision_effect_name);
			Serializer.Load(br, ref link_skill_target_effect_data_id);
			Serializer.Load(br, ref homing);
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				int intValue6 = 0;
				Serializer.Load(br, ref intValue6);
				projectile_hit_field_skill_id.Add(intValue6);
			}
			Serializer.Load(br, ref physics_projectile_destroy);
			Serializer.Load(br, ref projectile_collision_spawn_item_id);
			Serializer.Load(br, ref is_same_spawn_item_inherit_durability);
			Serializer.Load(br, ref dec_durability_by_collision);
			Serializer.Load(br, ref projectile_collision_durability_destroy);
			Serializer.Load(br, ref remove_from_inventory_when_throw);
			Serializer.Load(br, ref self_hit_grace_distance);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, projectile_type);
			Serializer.Save(bw, radius);
			Serializer.Save(bw, seq_type);
			Serializer.Save(bw, hit_type);
			Serializer.Save(bw, target_type.Count);
			foreach (int item in target_type)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, physics_projectile_prefab_name);
			Serializer.Save(bw, physics_projectile_speed);
			Serializer.Save(bw, physics_projectile_gravity_value);
			Serializer.Save(bw, physics_projectile_lifetime);
			Serializer.Save(bw, raycast_projectile_length);
			Serializer.Save(bw, penetrate_count);
			Serializer.Save(bw, battle_action_key);
			Serializer.Save(bw, abnormal_apply_type);
			Serializer.Save(bw, abnormal_master_id.Count);
			foreach (int item2 in abnormal_master_id)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, mutable_stat_type);
			Serializer.Save(bw, mutable_stat_modify_value);
			Serializer.Save(bw, projectile_collision_effect_name);
			Serializer.Save(bw, link_skill_target_effect_data_id);
			Serializer.Save(bw, homing);
			Serializer.Save(bw, projectile_hit_field_skill_id.Count);
			foreach (int item3 in projectile_hit_field_skill_id)
			{
				Serializer.Save(bw, item3);
			}
			Serializer.Save(bw, physics_projectile_destroy);
			Serializer.Save(bw, projectile_collision_spawn_item_id);
			Serializer.Save(bw, is_same_spawn_item_inherit_durability);
			Serializer.Save(bw, dec_durability_by_collision);
			Serializer.Save(bw, projectile_collision_durability_destroy);
			Serializer.Save(bw, remove_from_inventory_when_throw);
			Serializer.Save(bw, self_hit_grace_distance);
		}

		public bool Equal(ProjectileData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (projectile_type != comp.projectile_type)
			{
				return false;
			}
			if (radius != comp.radius)
			{
				return false;
			}
			if (seq_type != comp.seq_type)
			{
				return false;
			}
			if (hit_type != comp.hit_type)
			{
				return false;
			}
			if (comp.target_type.Count != target_type.Count)
			{
				return false;
			}
			foreach (int item in target_type)
			{
				if (!comp.target_type.Contains(item))
				{
					return false;
				}
			}
			if (physics_projectile_prefab_name != comp.physics_projectile_prefab_name)
			{
				return false;
			}
			if (physics_projectile_speed != comp.physics_projectile_speed)
			{
				return false;
			}
			if (physics_projectile_gravity_value != comp.physics_projectile_gravity_value)
			{
				return false;
			}
			if (physics_projectile_lifetime != comp.physics_projectile_lifetime)
			{
				return false;
			}
			if (raycast_projectile_length != comp.raycast_projectile_length)
			{
				return false;
			}
			if (penetrate_count != comp.penetrate_count)
			{
				return false;
			}
			if (battle_action_key != comp.battle_action_key)
			{
				return false;
			}
			if (abnormal_apply_type != comp.abnormal_apply_type)
			{
				return false;
			}
			if (comp.abnormal_master_id.Count != abnormal_master_id.Count)
			{
				return false;
			}
			foreach (int item2 in abnormal_master_id)
			{
				if (!comp.abnormal_master_id.Contains(item2))
				{
					return false;
				}
			}
			if (mutable_stat_type != comp.mutable_stat_type)
			{
				return false;
			}
			if (mutable_stat_modify_value != comp.mutable_stat_modify_value)
			{
				return false;
			}
			if (projectile_collision_effect_name != comp.projectile_collision_effect_name)
			{
				return false;
			}
			if (link_skill_target_effect_data_id != comp.link_skill_target_effect_data_id)
			{
				return false;
			}
			if (homing != comp.homing)
			{
				return false;
			}
			if (comp.projectile_hit_field_skill_id.Count != projectile_hit_field_skill_id.Count)
			{
				return false;
			}
			foreach (int item3 in projectile_hit_field_skill_id)
			{
				if (!comp.projectile_hit_field_skill_id.Contains(item3))
				{
					return false;
				}
			}
			if (physics_projectile_destroy != comp.physics_projectile_destroy)
			{
				return false;
			}
			if (projectile_collision_spawn_item_id != comp.projectile_collision_spawn_item_id)
			{
				return false;
			}
			if (is_same_spawn_item_inherit_durability != comp.is_same_spawn_item_inherit_durability)
			{
				return false;
			}
			if (dec_durability_by_collision != comp.dec_durability_by_collision)
			{
				return false;
			}
			if (projectile_collision_durability_destroy != comp.projectile_collision_durability_destroy)
			{
				return false;
			}
			if (remove_from_inventory_when_throw != comp.remove_from_inventory_when_throw)
			{
				return false;
			}
			if (self_hit_grace_distance != comp.self_hit_grace_distance)
			{
				return false;
			}
			return true;
		}

		public ProjectileData_MasterData Clone()
		{
			ProjectileData_MasterData projectileData_MasterData = new ProjectileData_MasterData();
			CopyTo(projectileData_MasterData);
			return projectileData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			projectile_type = 0;
			radius = 0f;
			seq_type = 0;
			hit_type = 0;
			target_type.Clear();
			physics_projectile_prefab_name = string.Empty;
			physics_projectile_speed = 0f;
			physics_projectile_gravity_value = 0f;
			physics_projectile_lifetime = 0L;
			raycast_projectile_length = 0f;
			penetrate_count = 0;
			battle_action_key = string.Empty;
			abnormal_apply_type = 0;
			abnormal_master_id.Clear();
			mutable_stat_type = string.Empty;
			mutable_stat_modify_value = 0;
			projectile_collision_effect_name = string.Empty;
			link_skill_target_effect_data_id = 0;
			homing = false;
			projectile_hit_field_skill_id.Clear();
			physics_projectile_destroy = false;
			projectile_collision_spawn_item_id = 0;
			is_same_spawn_item_inherit_durability = false;
			dec_durability_by_collision = 0;
			projectile_collision_durability_destroy = false;
			remove_from_inventory_when_throw = false;
			self_hit_grace_distance = 0L;
		}
	}
}
