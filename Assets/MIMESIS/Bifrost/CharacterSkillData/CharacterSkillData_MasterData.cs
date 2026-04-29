using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.CharacterSkillData
{
	public class CharacterSkillData_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public string skill_animation_state_name = string.Empty;

		public string skill_cancel_animation_state_name = string.Empty;

		public bool is_moving_skill;

		public List<string> skill_sequence_set = new List<string>();

		public List<string> projectile_event_set = new List<string>();

		public List<string> field_skill_event_set = new List<string>();

		public List<string> aura_skill_event_set = new List<string>();

		public int target_type;

		public int hit_count;

		public int target_count;

		public float min_range;

		public float max_range;

		public float allow_range;

		public int cooltime;

		public int cooltime_type;

		public List<string> immune = new List<string>();

		public bool hide_hand_item;

		public List<string> skill_dead_camera = new List<string>();

		public int dec_gauge;

		public int consume_item_type;

		public int bullet_type;

		public int dec_durability_per_use;

		public int dec_durability_type;

		public bool use_mesh_break_by_durability;

		public CharacterSkillData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public CharacterSkillData_MasterData()
			: base(110670473u, "CharacterSkillData_MasterData")
		{
		}

		public void CopyTo(CharacterSkillData_MasterData dest)
		{
			dest.id = id;
			dest.name = name;
			dest.skill_animation_state_name = skill_animation_state_name;
			dest.skill_cancel_animation_state_name = skill_cancel_animation_state_name;
			dest.is_moving_skill = is_moving_skill;
			dest.skill_sequence_set.Clear();
			foreach (string item in skill_sequence_set)
			{
				dest.skill_sequence_set.Add(item);
			}
			dest.projectile_event_set.Clear();
			foreach (string item2 in projectile_event_set)
			{
				dest.projectile_event_set.Add(item2);
			}
			dest.field_skill_event_set.Clear();
			foreach (string item3 in field_skill_event_set)
			{
				dest.field_skill_event_set.Add(item3);
			}
			dest.aura_skill_event_set.Clear();
			foreach (string item4 in aura_skill_event_set)
			{
				dest.aura_skill_event_set.Add(item4);
			}
			dest.target_type = target_type;
			dest.hit_count = hit_count;
			dest.target_count = target_count;
			dest.min_range = min_range;
			dest.max_range = max_range;
			dest.allow_range = allow_range;
			dest.cooltime = cooltime;
			dest.cooltime_type = cooltime_type;
			dest.immune.Clear();
			foreach (string item5 in immune)
			{
				dest.immune.Add(item5);
			}
			dest.hide_hand_item = hide_hand_item;
			dest.skill_dead_camera.Clear();
			foreach (string item6 in skill_dead_camera)
			{
				dest.skill_dead_camera.Add(item6);
			}
			dest.dec_gauge = dec_gauge;
			dest.consume_item_type = consume_item_type;
			dest.bullet_type = bullet_type;
			dest.dec_durability_per_use = dec_durability_per_use;
			dest.dec_durability_type = dec_durability_type;
			dest.use_mesh_break_by_durability = use_mesh_break_by_durability;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += Serializer.GetLength(name);
			num += Serializer.GetLength(skill_animation_state_name);
			num += Serializer.GetLength(skill_cancel_animation_state_name);
			num += Serializer.GetLength(is_moving_skill);
			num += 4;
			foreach (string item in skill_sequence_set)
			{
				num += Serializer.GetLength(item);
			}
			num += 4;
			foreach (string item2 in projectile_event_set)
			{
				num += Serializer.GetLength(item2);
			}
			num += 4;
			foreach (string item3 in field_skill_event_set)
			{
				num += Serializer.GetLength(item3);
			}
			num += 4;
			foreach (string item4 in aura_skill_event_set)
			{
				num += Serializer.GetLength(item4);
			}
			num += Serializer.GetLength(target_type);
			num += Serializer.GetLength(hit_count);
			num += Serializer.GetLength(target_count);
			num += Serializer.GetLength(min_range);
			num += Serializer.GetLength(max_range);
			num += Serializer.GetLength(allow_range);
			num += Serializer.GetLength(cooltime);
			num += Serializer.GetLength(cooltime_type);
			num += 4;
			foreach (string item5 in immune)
			{
				num += Serializer.GetLength(item5);
			}
			num += Serializer.GetLength(hide_hand_item);
			num += 4;
			foreach (string item6 in skill_dead_camera)
			{
				num += Serializer.GetLength(item6);
			}
			num += Serializer.GetLength(dec_gauge);
			num += Serializer.GetLength(consume_item_type);
			num += Serializer.GetLength(bullet_type);
			num += Serializer.GetLength(dec_durability_per_use);
			num += Serializer.GetLength(dec_durability_type);
			return num + Serializer.GetLength(use_mesh_break_by_durability);
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
			Serializer.Load(br, ref name);
			Serializer.Load(br, ref skill_animation_state_name);
			Serializer.Load(br, ref skill_cancel_animation_state_name);
			Serializer.Load(br, ref is_moving_skill);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				skill_sequence_set.Add(strValue);
			}
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				projectile_event_set.Add(strValue2);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				string strValue3 = string.Empty;
				Serializer.Load(br, ref strValue3);
				field_skill_event_set.Add(strValue3);
			}
			int intValue4 = 0;
			Serializer.Load(br, ref intValue4);
			while (intValue4-- > 0)
			{
				string strValue4 = string.Empty;
				Serializer.Load(br, ref strValue4);
				aura_skill_event_set.Add(strValue4);
			}
			Serializer.Load(br, ref target_type);
			Serializer.Load(br, ref hit_count);
			Serializer.Load(br, ref target_count);
			Serializer.Load(br, ref min_range);
			Serializer.Load(br, ref max_range);
			Serializer.Load(br, ref allow_range);
			Serializer.Load(br, ref cooltime);
			Serializer.Load(br, ref cooltime_type);
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				string strValue5 = string.Empty;
				Serializer.Load(br, ref strValue5);
				immune.Add(strValue5);
			}
			Serializer.Load(br, ref hide_hand_item);
			int intValue6 = 0;
			Serializer.Load(br, ref intValue6);
			while (intValue6-- > 0)
			{
				string strValue6 = string.Empty;
				Serializer.Load(br, ref strValue6);
				skill_dead_camera.Add(strValue6);
			}
			Serializer.Load(br, ref dec_gauge);
			Serializer.Load(br, ref consume_item_type);
			Serializer.Load(br, ref bullet_type);
			Serializer.Load(br, ref dec_durability_per_use);
			Serializer.Load(br, ref dec_durability_type);
			Serializer.Load(br, ref use_mesh_break_by_durability);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, name);
			Serializer.Save(bw, skill_animation_state_name);
			Serializer.Save(bw, skill_cancel_animation_state_name);
			Serializer.Save(bw, is_moving_skill);
			Serializer.Save(bw, skill_sequence_set.Count);
			foreach (string item in skill_sequence_set)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, projectile_event_set.Count);
			foreach (string item2 in projectile_event_set)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, field_skill_event_set.Count);
			foreach (string item3 in field_skill_event_set)
			{
				Serializer.Save(bw, item3);
			}
			Serializer.Save(bw, aura_skill_event_set.Count);
			foreach (string item4 in aura_skill_event_set)
			{
				Serializer.Save(bw, item4);
			}
			Serializer.Save(bw, target_type);
			Serializer.Save(bw, hit_count);
			Serializer.Save(bw, target_count);
			Serializer.Save(bw, min_range);
			Serializer.Save(bw, max_range);
			Serializer.Save(bw, allow_range);
			Serializer.Save(bw, cooltime);
			Serializer.Save(bw, cooltime_type);
			Serializer.Save(bw, immune.Count);
			foreach (string item5 in immune)
			{
				Serializer.Save(bw, item5);
			}
			Serializer.Save(bw, hide_hand_item);
			Serializer.Save(bw, skill_dead_camera.Count);
			foreach (string item6 in skill_dead_camera)
			{
				Serializer.Save(bw, item6);
			}
			Serializer.Save(bw, dec_gauge);
			Serializer.Save(bw, consume_item_type);
			Serializer.Save(bw, bullet_type);
			Serializer.Save(bw, dec_durability_per_use);
			Serializer.Save(bw, dec_durability_type);
			Serializer.Save(bw, use_mesh_break_by_durability);
		}

		public bool Equal(CharacterSkillData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (skill_animation_state_name != comp.skill_animation_state_name)
			{
				return false;
			}
			if (skill_cancel_animation_state_name != comp.skill_cancel_animation_state_name)
			{
				return false;
			}
			if (is_moving_skill != comp.is_moving_skill)
			{
				return false;
			}
			if (comp.skill_sequence_set.Count != skill_sequence_set.Count)
			{
				return false;
			}
			foreach (string item in skill_sequence_set)
			{
				if (!comp.skill_sequence_set.Contains(item))
				{
					return false;
				}
			}
			if (comp.projectile_event_set.Count != projectile_event_set.Count)
			{
				return false;
			}
			foreach (string item2 in projectile_event_set)
			{
				if (!comp.projectile_event_set.Contains(item2))
				{
					return false;
				}
			}
			if (comp.field_skill_event_set.Count != field_skill_event_set.Count)
			{
				return false;
			}
			foreach (string item3 in field_skill_event_set)
			{
				if (!comp.field_skill_event_set.Contains(item3))
				{
					return false;
				}
			}
			if (comp.aura_skill_event_set.Count != aura_skill_event_set.Count)
			{
				return false;
			}
			foreach (string item4 in aura_skill_event_set)
			{
				if (!comp.aura_skill_event_set.Contains(item4))
				{
					return false;
				}
			}
			if (target_type != comp.target_type)
			{
				return false;
			}
			if (hit_count != comp.hit_count)
			{
				return false;
			}
			if (target_count != comp.target_count)
			{
				return false;
			}
			if (min_range != comp.min_range)
			{
				return false;
			}
			if (max_range != comp.max_range)
			{
				return false;
			}
			if (allow_range != comp.allow_range)
			{
				return false;
			}
			if (cooltime != comp.cooltime)
			{
				return false;
			}
			if (cooltime_type != comp.cooltime_type)
			{
				return false;
			}
			if (comp.immune.Count != immune.Count)
			{
				return false;
			}
			foreach (string item5 in immune)
			{
				if (!comp.immune.Contains(item5))
				{
					return false;
				}
			}
			if (hide_hand_item != comp.hide_hand_item)
			{
				return false;
			}
			if (comp.skill_dead_camera.Count != skill_dead_camera.Count)
			{
				return false;
			}
			foreach (string item6 in skill_dead_camera)
			{
				if (!comp.skill_dead_camera.Contains(item6))
				{
					return false;
				}
			}
			if (dec_gauge != comp.dec_gauge)
			{
				return false;
			}
			if (consume_item_type != comp.consume_item_type)
			{
				return false;
			}
			if (bullet_type != comp.bullet_type)
			{
				return false;
			}
			if (dec_durability_per_use != comp.dec_durability_per_use)
			{
				return false;
			}
			if (dec_durability_type != comp.dec_durability_type)
			{
				return false;
			}
			if (use_mesh_break_by_durability != comp.use_mesh_break_by_durability)
			{
				return false;
			}
			return true;
		}

		public CharacterSkillData_MasterData Clone()
		{
			CharacterSkillData_MasterData characterSkillData_MasterData = new CharacterSkillData_MasterData();
			CopyTo(characterSkillData_MasterData);
			return characterSkillData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			skill_animation_state_name = string.Empty;
			skill_cancel_animation_state_name = string.Empty;
			is_moving_skill = false;
			skill_sequence_set.Clear();
			projectile_event_set.Clear();
			field_skill_event_set.Clear();
			aura_skill_event_set.Clear();
			target_type = 0;
			hit_count = 0;
			target_count = 0;
			min_range = 0f;
			max_range = 0f;
			allow_range = 0f;
			cooltime = 0;
			cooltime_type = 0;
			immune.Clear();
			hide_hand_item = false;
			skill_dead_camera.Clear();
			dec_gauge = 0;
			consume_item_type = 0;
			bullet_type = 0;
			dec_durability_per_use = 0;
			dec_durability_type = 0;
			use_mesh_break_by_durability = false;
		}
	}
}
