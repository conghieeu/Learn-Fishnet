using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.MonsterData
{
	public class MonsterData_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public int monster_type;

		public string model_path = string.Empty;

		public long hp;

		public long attack_power;

		public long defense;

		public long move_speed_walk;

		public long move_speed_run;

		public string btname = string.Empty;

		public List<int> factions = new List<int>();

		public List<int> default_skill_ids = new List<int>();

		public int abnormal_trigger_threshold;

		public int abnormal_trigger_id;

		public List<MonsterData_aggro> MonsterData_aggroval = new List<MonsterData_aggro>();

		public int targeting_aggro_score;

		public int pick_target;

		public int threat_value;

		public int spawn_wait_time;

		public int start_aura_skill_id;

		public int inborn_abnormal;

		public int drop_id;

		public int recover_mode;

		public int despawn_delay;

		public float move_radius;

		public float hit_radius;

		public bool remain_after_death;

		public List<string> eye_raycast_socket = new List<string>();

		public MonsterData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public MonsterData_MasterData()
			: base(107862665u, "MonsterData_MasterData")
		{
		}

		public void CopyTo(MonsterData_MasterData dest)
		{
			dest.id = id;
			dest.name = name;
			dest.monster_type = monster_type;
			dest.model_path = model_path;
			dest.hp = hp;
			dest.attack_power = attack_power;
			dest.defense = defense;
			dest.move_speed_walk = move_speed_walk;
			dest.move_speed_run = move_speed_run;
			dest.btname = btname;
			dest.factions.Clear();
			foreach (int faction in factions)
			{
				dest.factions.Add(faction);
			}
			dest.default_skill_ids.Clear();
			foreach (int default_skill_id in default_skill_ids)
			{
				dest.default_skill_ids.Add(default_skill_id);
			}
			dest.abnormal_trigger_threshold = abnormal_trigger_threshold;
			dest.abnormal_trigger_id = abnormal_trigger_id;
			dest.MonsterData_aggroval.Clear();
			foreach (MonsterData_aggro item in MonsterData_aggroval)
			{
				MonsterData_aggro monsterData_aggro = new MonsterData_aggro();
				item.CopyTo(monsterData_aggro);
				dest.MonsterData_aggroval.Add(monsterData_aggro);
			}
			dest.targeting_aggro_score = targeting_aggro_score;
			dest.pick_target = pick_target;
			dest.threat_value = threat_value;
			dest.spawn_wait_time = spawn_wait_time;
			dest.start_aura_skill_id = start_aura_skill_id;
			dest.inborn_abnormal = inborn_abnormal;
			dest.drop_id = drop_id;
			dest.recover_mode = recover_mode;
			dest.despawn_delay = despawn_delay;
			dest.move_radius = move_radius;
			dest.hit_radius = hit_radius;
			dest.remain_after_death = remain_after_death;
			dest.eye_raycast_socket.Clear();
			foreach (string item2 in eye_raycast_socket)
			{
				dest.eye_raycast_socket.Add(item2);
			}
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
			num += Serializer.GetLength(monster_type);
			num += Serializer.GetLength(model_path);
			num += Serializer.GetLength(hp);
			num += Serializer.GetLength(attack_power);
			num += Serializer.GetLength(defense);
			num += Serializer.GetLength(move_speed_walk);
			num += Serializer.GetLength(move_speed_run);
			num += Serializer.GetLength(btname);
			num += 4;
			foreach (int faction in factions)
			{
				num += Serializer.GetLength(faction);
			}
			num += 4;
			foreach (int default_skill_id in default_skill_ids)
			{
				num += Serializer.GetLength(default_skill_id);
			}
			num += Serializer.GetLength(abnormal_trigger_threshold);
			num += Serializer.GetLength(abnormal_trigger_id);
			num += 4;
			foreach (MonsterData_aggro item in MonsterData_aggroval)
			{
				num += item.GetLengthInternal();
			}
			num += Serializer.GetLength(targeting_aggro_score);
			num += Serializer.GetLength(pick_target);
			num += Serializer.GetLength(threat_value);
			num += Serializer.GetLength(spawn_wait_time);
			num += Serializer.GetLength(start_aura_skill_id);
			num += Serializer.GetLength(inborn_abnormal);
			num += Serializer.GetLength(drop_id);
			num += Serializer.GetLength(recover_mode);
			num += Serializer.GetLength(despawn_delay);
			num += Serializer.GetLength(move_radius);
			num += Serializer.GetLength(hit_radius);
			num += Serializer.GetLength(remain_after_death);
			num += 4;
			foreach (string item2 in eye_raycast_socket)
			{
				num += Serializer.GetLength(item2);
			}
			return num;
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
			Serializer.Load(br, ref monster_type);
			Serializer.Load(br, ref model_path);
			Serializer.Load(br, ref hp);
			Serializer.Load(br, ref attack_power);
			Serializer.Load(br, ref defense);
			Serializer.Load(br, ref move_speed_walk);
			Serializer.Load(br, ref move_speed_run);
			Serializer.Load(br, ref btname);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				int intValue2 = 0;
				Serializer.Load(br, ref intValue2);
				factions.Add(intValue2);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				int intValue4 = 0;
				Serializer.Load(br, ref intValue4);
				default_skill_ids.Add(intValue4);
			}
			Serializer.Load(br, ref abnormal_trigger_threshold);
			Serializer.Load(br, ref abnormal_trigger_id);
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				MonsterData_aggro monsterData_aggro = new MonsterData_aggro();
				monsterData_aggro.LoadInternal(br);
				MonsterData_aggroval.Add(monsterData_aggro);
			}
			Serializer.Load(br, ref targeting_aggro_score);
			Serializer.Load(br, ref pick_target);
			Serializer.Load(br, ref threat_value);
			Serializer.Load(br, ref spawn_wait_time);
			Serializer.Load(br, ref start_aura_skill_id);
			Serializer.Load(br, ref inborn_abnormal);
			Serializer.Load(br, ref drop_id);
			Serializer.Load(br, ref recover_mode);
			Serializer.Load(br, ref despawn_delay);
			Serializer.Load(br, ref move_radius);
			Serializer.Load(br, ref hit_radius);
			Serializer.Load(br, ref remain_after_death);
			int intValue6 = 0;
			Serializer.Load(br, ref intValue6);
			while (intValue6-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				eye_raycast_socket.Add(strValue);
			}
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
			Serializer.Save(bw, monster_type);
			Serializer.Save(bw, model_path);
			Serializer.Save(bw, hp);
			Serializer.Save(bw, attack_power);
			Serializer.Save(bw, defense);
			Serializer.Save(bw, move_speed_walk);
			Serializer.Save(bw, move_speed_run);
			Serializer.Save(bw, btname);
			Serializer.Save(bw, factions.Count);
			foreach (int faction in factions)
			{
				Serializer.Save(bw, faction);
			}
			Serializer.Save(bw, default_skill_ids.Count);
			foreach (int default_skill_id in default_skill_ids)
			{
				Serializer.Save(bw, default_skill_id);
			}
			Serializer.Save(bw, abnormal_trigger_threshold);
			Serializer.Save(bw, abnormal_trigger_id);
			Serializer.Save(bw, MonsterData_aggroval.Count);
			foreach (MonsterData_aggro item in MonsterData_aggroval)
			{
				item.SaveInternal(bw);
			}
			Serializer.Save(bw, targeting_aggro_score);
			Serializer.Save(bw, pick_target);
			Serializer.Save(bw, threat_value);
			Serializer.Save(bw, spawn_wait_time);
			Serializer.Save(bw, start_aura_skill_id);
			Serializer.Save(bw, inborn_abnormal);
			Serializer.Save(bw, drop_id);
			Serializer.Save(bw, recover_mode);
			Serializer.Save(bw, despawn_delay);
			Serializer.Save(bw, move_radius);
			Serializer.Save(bw, hit_radius);
			Serializer.Save(bw, remain_after_death);
			Serializer.Save(bw, eye_raycast_socket.Count);
			foreach (string item2 in eye_raycast_socket)
			{
				Serializer.Save(bw, item2);
			}
		}

		public bool Equal(MonsterData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (monster_type != comp.monster_type)
			{
				return false;
			}
			if (model_path != comp.model_path)
			{
				return false;
			}
			if (hp != comp.hp)
			{
				return false;
			}
			if (attack_power != comp.attack_power)
			{
				return false;
			}
			if (defense != comp.defense)
			{
				return false;
			}
			if (move_speed_walk != comp.move_speed_walk)
			{
				return false;
			}
			if (move_speed_run != comp.move_speed_run)
			{
				return false;
			}
			if (btname != comp.btname)
			{
				return false;
			}
			if (comp.factions.Count != factions.Count)
			{
				return false;
			}
			foreach (int faction in factions)
			{
				if (!comp.factions.Contains(faction))
				{
					return false;
				}
			}
			if (comp.default_skill_ids.Count != default_skill_ids.Count)
			{
				return false;
			}
			foreach (int default_skill_id in default_skill_ids)
			{
				if (!comp.default_skill_ids.Contains(default_skill_id))
				{
					return false;
				}
			}
			if (abnormal_trigger_threshold != comp.abnormal_trigger_threshold)
			{
				return false;
			}
			if (abnormal_trigger_id != comp.abnormal_trigger_id)
			{
				return false;
			}
			if (comp.MonsterData_aggroval.Count != MonsterData_aggroval.Count)
			{
				return false;
			}
			foreach (MonsterData_aggro item in MonsterData_aggroval)
			{
				bool flag = false;
				foreach (MonsterData_aggro item2 in comp.MonsterData_aggroval)
				{
					if (item.Equal(item2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			if (targeting_aggro_score != comp.targeting_aggro_score)
			{
				return false;
			}
			if (pick_target != comp.pick_target)
			{
				return false;
			}
			if (threat_value != comp.threat_value)
			{
				return false;
			}
			if (spawn_wait_time != comp.spawn_wait_time)
			{
				return false;
			}
			if (start_aura_skill_id != comp.start_aura_skill_id)
			{
				return false;
			}
			if (inborn_abnormal != comp.inborn_abnormal)
			{
				return false;
			}
			if (drop_id != comp.drop_id)
			{
				return false;
			}
			if (recover_mode != comp.recover_mode)
			{
				return false;
			}
			if (despawn_delay != comp.despawn_delay)
			{
				return false;
			}
			if (move_radius != comp.move_radius)
			{
				return false;
			}
			if (hit_radius != comp.hit_radius)
			{
				return false;
			}
			if (remain_after_death != comp.remain_after_death)
			{
				return false;
			}
			if (comp.eye_raycast_socket.Count != eye_raycast_socket.Count)
			{
				return false;
			}
			foreach (string item3 in eye_raycast_socket)
			{
				if (!comp.eye_raycast_socket.Contains(item3))
				{
					return false;
				}
			}
			return true;
		}

		public MonsterData_MasterData Clone()
		{
			MonsterData_MasterData monsterData_MasterData = new MonsterData_MasterData();
			CopyTo(monsterData_MasterData);
			return monsterData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			monster_type = 0;
			model_path = string.Empty;
			hp = 0L;
			attack_power = 0L;
			defense = 0L;
			move_speed_walk = 0L;
			move_speed_run = 0L;
			btname = string.Empty;
			factions.Clear();
			default_skill_ids.Clear();
			abnormal_trigger_threshold = 0;
			abnormal_trigger_id = 0;
			MonsterData_aggroval.Clear();
			targeting_aggro_score = 0;
			pick_target = 0;
			threat_value = 0;
			spawn_wait_time = 0;
			start_aura_skill_id = 0;
			inborn_abnormal = 0;
			drop_id = 0;
			recover_mode = 0;
			despawn_delay = 0;
			move_radius = 0f;
			hit_radius = 0f;
			remain_after_death = false;
			eye_raycast_socket.Clear();
		}
	}
}
