using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.Dungeon
{
	public class Dungeon_MasterData : ISchema
	{
		public int id;

		public int map_id;

		public string map_name = string.Empty;

		public string start_display_time = string.Empty;

		public string end_time = string.Empty;

		public List<Dungeon_event_group> Dungeon_event_groupval = new List<Dungeon_event_group>();

		public int misc_value_min;

		public int misc_value_max;

		public List<Dungeon_candidate> Dungeon_candidateval = new List<Dungeon_candidate>();

		public int spawnable_misc_id;

		public int random_teleport_rate;

		public List<int> random_teleport_start_count = new List<int>();

		public List<int> random_teleport_end_count = new List<int>();

		public int conta_increase_idle;

		public int conta_increase_run;

		public bool is_active;

		public int min_session_count;

		public int max_session_count;

		public int hazard_level;

		public int env_level;

		public int reward_level;

		public int shop_group;

		public int canopy_count;

		public int default_weather_id;

		public List<string> weather_change = new List<string>();

		public List<string> weather_random = new List<string>();

		public int weather_random_prob;

		public int weather_random_min;

		public int weather_random_max;

		public int weather_random_duration;

		public int threat_min;

		public int threat_max;

		public int normal_monster_spawn_period;

		public int normal_monster_spawn_try_count;

		public int normal_monster_spawn_rate;

		public int normal_master_ids;

		public int mimic_spawn_count_min;

		public int mimic_spawn_count_max;

		public int mimic_spawn_period;

		public int mimic_spawn_try_count;

		public int mimic_spawn_rate;

		public string loading_scene_name = string.Empty;

		public Dungeon_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Dungeon_MasterData()
			: base(524815186u, "Dungeon_MasterData")
		{
		}

		public void CopyTo(Dungeon_MasterData dest)
		{
			dest.id = id;
			dest.map_id = map_id;
			dest.map_name = map_name;
			dest.start_display_time = start_display_time;
			dest.end_time = end_time;
			dest.Dungeon_event_groupval.Clear();
			foreach (Dungeon_event_group item in Dungeon_event_groupval)
			{
				Dungeon_event_group dungeon_event_group = new Dungeon_event_group();
				item.CopyTo(dungeon_event_group);
				dest.Dungeon_event_groupval.Add(dungeon_event_group);
			}
			dest.misc_value_min = misc_value_min;
			dest.misc_value_max = misc_value_max;
			dest.Dungeon_candidateval.Clear();
			foreach (Dungeon_candidate item2 in Dungeon_candidateval)
			{
				Dungeon_candidate dungeon_candidate = new Dungeon_candidate();
				item2.CopyTo(dungeon_candidate);
				dest.Dungeon_candidateval.Add(dungeon_candidate);
			}
			dest.spawnable_misc_id = spawnable_misc_id;
			dest.random_teleport_rate = random_teleport_rate;
			dest.random_teleport_start_count.Clear();
			foreach (int item3 in random_teleport_start_count)
			{
				dest.random_teleport_start_count.Add(item3);
			}
			dest.random_teleport_end_count.Clear();
			foreach (int item4 in random_teleport_end_count)
			{
				dest.random_teleport_end_count.Add(item4);
			}
			dest.conta_increase_idle = conta_increase_idle;
			dest.conta_increase_run = conta_increase_run;
			dest.is_active = is_active;
			dest.min_session_count = min_session_count;
			dest.max_session_count = max_session_count;
			dest.hazard_level = hazard_level;
			dest.env_level = env_level;
			dest.reward_level = reward_level;
			dest.shop_group = shop_group;
			dest.canopy_count = canopy_count;
			dest.default_weather_id = default_weather_id;
			dest.weather_change.Clear();
			foreach (string item5 in weather_change)
			{
				dest.weather_change.Add(item5);
			}
			dest.weather_random.Clear();
			foreach (string item6 in weather_random)
			{
				dest.weather_random.Add(item6);
			}
			dest.weather_random_prob = weather_random_prob;
			dest.weather_random_min = weather_random_min;
			dest.weather_random_max = weather_random_max;
			dest.weather_random_duration = weather_random_duration;
			dest.threat_min = threat_min;
			dest.threat_max = threat_max;
			dest.normal_monster_spawn_period = normal_monster_spawn_period;
			dest.normal_monster_spawn_try_count = normal_monster_spawn_try_count;
			dest.normal_monster_spawn_rate = normal_monster_spawn_rate;
			dest.normal_master_ids = normal_master_ids;
			dest.mimic_spawn_count_min = mimic_spawn_count_min;
			dest.mimic_spawn_count_max = mimic_spawn_count_max;
			dest.mimic_spawn_period = mimic_spawn_period;
			dest.mimic_spawn_try_count = mimic_spawn_try_count;
			dest.mimic_spawn_rate = mimic_spawn_rate;
			dest.loading_scene_name = loading_scene_name;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += Serializer.GetLength(map_id);
			num += Serializer.GetLength(map_name);
			num += Serializer.GetLength(start_display_time);
			num += Serializer.GetLength(end_time);
			num += 4;
			foreach (Dungeon_event_group item in Dungeon_event_groupval)
			{
				num += item.GetLengthInternal();
			}
			num += Serializer.GetLength(misc_value_min);
			num += Serializer.GetLength(misc_value_max);
			num += 4;
			foreach (Dungeon_candidate item2 in Dungeon_candidateval)
			{
				num += item2.GetLengthInternal();
			}
			num += Serializer.GetLength(spawnable_misc_id);
			num += Serializer.GetLength(random_teleport_rate);
			num += 4;
			foreach (int item3 in random_teleport_start_count)
			{
				num += Serializer.GetLength(item3);
			}
			num += 4;
			foreach (int item4 in random_teleport_end_count)
			{
				num += Serializer.GetLength(item4);
			}
			num += Serializer.GetLength(conta_increase_idle);
			num += Serializer.GetLength(conta_increase_run);
			num += Serializer.GetLength(is_active);
			num += Serializer.GetLength(min_session_count);
			num += Serializer.GetLength(max_session_count);
			num += Serializer.GetLength(hazard_level);
			num += Serializer.GetLength(env_level);
			num += Serializer.GetLength(reward_level);
			num += Serializer.GetLength(shop_group);
			num += Serializer.GetLength(canopy_count);
			num += Serializer.GetLength(default_weather_id);
			num += 4;
			foreach (string item5 in weather_change)
			{
				num += Serializer.GetLength(item5);
			}
			num += 4;
			foreach (string item6 in weather_random)
			{
				num += Serializer.GetLength(item6);
			}
			num += Serializer.GetLength(weather_random_prob);
			num += Serializer.GetLength(weather_random_min);
			num += Serializer.GetLength(weather_random_max);
			num += Serializer.GetLength(weather_random_duration);
			num += Serializer.GetLength(threat_min);
			num += Serializer.GetLength(threat_max);
			num += Serializer.GetLength(normal_monster_spawn_period);
			num += Serializer.GetLength(normal_monster_spawn_try_count);
			num += Serializer.GetLength(normal_monster_spawn_rate);
			num += Serializer.GetLength(normal_master_ids);
			num += Serializer.GetLength(mimic_spawn_count_min);
			num += Serializer.GetLength(mimic_spawn_count_max);
			num += Serializer.GetLength(mimic_spawn_period);
			num += Serializer.GetLength(mimic_spawn_try_count);
			num += Serializer.GetLength(mimic_spawn_rate);
			return num + Serializer.GetLength(loading_scene_name);
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
			Serializer.Load(br, ref map_id);
			Serializer.Load(br, ref map_name);
			Serializer.Load(br, ref start_display_time);
			Serializer.Load(br, ref end_time);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				Dungeon_event_group dungeon_event_group = new Dungeon_event_group();
				dungeon_event_group.LoadInternal(br);
				Dungeon_event_groupval.Add(dungeon_event_group);
			}
			Serializer.Load(br, ref misc_value_min);
			Serializer.Load(br, ref misc_value_max);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				Dungeon_candidate dungeon_candidate = new Dungeon_candidate();
				dungeon_candidate.LoadInternal(br);
				Dungeon_candidateval.Add(dungeon_candidate);
			}
			Serializer.Load(br, ref spawnable_misc_id);
			Serializer.Load(br, ref random_teleport_rate);
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				int intValue4 = 0;
				Serializer.Load(br, ref intValue4);
				random_teleport_start_count.Add(intValue4);
			}
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				int intValue6 = 0;
				Serializer.Load(br, ref intValue6);
				random_teleport_end_count.Add(intValue6);
			}
			Serializer.Load(br, ref conta_increase_idle);
			Serializer.Load(br, ref conta_increase_run);
			Serializer.Load(br, ref is_active);
			Serializer.Load(br, ref min_session_count);
			Serializer.Load(br, ref max_session_count);
			Serializer.Load(br, ref hazard_level);
			Serializer.Load(br, ref env_level);
			Serializer.Load(br, ref reward_level);
			Serializer.Load(br, ref shop_group);
			Serializer.Load(br, ref canopy_count);
			Serializer.Load(br, ref default_weather_id);
			int intValue7 = 0;
			Serializer.Load(br, ref intValue7);
			while (intValue7-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				weather_change.Add(strValue);
			}
			int intValue8 = 0;
			Serializer.Load(br, ref intValue8);
			while (intValue8-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				weather_random.Add(strValue2);
			}
			Serializer.Load(br, ref weather_random_prob);
			Serializer.Load(br, ref weather_random_min);
			Serializer.Load(br, ref weather_random_max);
			Serializer.Load(br, ref weather_random_duration);
			Serializer.Load(br, ref threat_min);
			Serializer.Load(br, ref threat_max);
			Serializer.Load(br, ref normal_monster_spawn_period);
			Serializer.Load(br, ref normal_monster_spawn_try_count);
			Serializer.Load(br, ref normal_monster_spawn_rate);
			Serializer.Load(br, ref normal_master_ids);
			Serializer.Load(br, ref mimic_spawn_count_min);
			Serializer.Load(br, ref mimic_spawn_count_max);
			Serializer.Load(br, ref mimic_spawn_period);
			Serializer.Load(br, ref mimic_spawn_try_count);
			Serializer.Load(br, ref mimic_spawn_rate);
			Serializer.Load(br, ref loading_scene_name);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, map_id);
			Serializer.Save(bw, map_name);
			Serializer.Save(bw, start_display_time);
			Serializer.Save(bw, end_time);
			Serializer.Save(bw, Dungeon_event_groupval.Count);
			foreach (Dungeon_event_group item in Dungeon_event_groupval)
			{
				item.SaveInternal(bw);
			}
			Serializer.Save(bw, misc_value_min);
			Serializer.Save(bw, misc_value_max);
			Serializer.Save(bw, Dungeon_candidateval.Count);
			foreach (Dungeon_candidate item2 in Dungeon_candidateval)
			{
				item2.SaveInternal(bw);
			}
			Serializer.Save(bw, spawnable_misc_id);
			Serializer.Save(bw, random_teleport_rate);
			Serializer.Save(bw, random_teleport_start_count.Count);
			foreach (int item3 in random_teleport_start_count)
			{
				Serializer.Save(bw, item3);
			}
			Serializer.Save(bw, random_teleport_end_count.Count);
			foreach (int item4 in random_teleport_end_count)
			{
				Serializer.Save(bw, item4);
			}
			Serializer.Save(bw, conta_increase_idle);
			Serializer.Save(bw, conta_increase_run);
			Serializer.Save(bw, is_active);
			Serializer.Save(bw, min_session_count);
			Serializer.Save(bw, max_session_count);
			Serializer.Save(bw, hazard_level);
			Serializer.Save(bw, env_level);
			Serializer.Save(bw, reward_level);
			Serializer.Save(bw, shop_group);
			Serializer.Save(bw, canopy_count);
			Serializer.Save(bw, default_weather_id);
			Serializer.Save(bw, weather_change.Count);
			foreach (string item5 in weather_change)
			{
				Serializer.Save(bw, item5);
			}
			Serializer.Save(bw, weather_random.Count);
			foreach (string item6 in weather_random)
			{
				Serializer.Save(bw, item6);
			}
			Serializer.Save(bw, weather_random_prob);
			Serializer.Save(bw, weather_random_min);
			Serializer.Save(bw, weather_random_max);
			Serializer.Save(bw, weather_random_duration);
			Serializer.Save(bw, threat_min);
			Serializer.Save(bw, threat_max);
			Serializer.Save(bw, normal_monster_spawn_period);
			Serializer.Save(bw, normal_monster_spawn_try_count);
			Serializer.Save(bw, normal_monster_spawn_rate);
			Serializer.Save(bw, normal_master_ids);
			Serializer.Save(bw, mimic_spawn_count_min);
			Serializer.Save(bw, mimic_spawn_count_max);
			Serializer.Save(bw, mimic_spawn_period);
			Serializer.Save(bw, mimic_spawn_try_count);
			Serializer.Save(bw, mimic_spawn_rate);
			Serializer.Save(bw, loading_scene_name);
		}

		public bool Equal(Dungeon_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (map_id != comp.map_id)
			{
				return false;
			}
			if (map_name != comp.map_name)
			{
				return false;
			}
			if (start_display_time != comp.start_display_time)
			{
				return false;
			}
			if (end_time != comp.end_time)
			{
				return false;
			}
			if (comp.Dungeon_event_groupval.Count != Dungeon_event_groupval.Count)
			{
				return false;
			}
			foreach (Dungeon_event_group item in Dungeon_event_groupval)
			{
				bool flag = false;
				foreach (Dungeon_event_group item2 in comp.Dungeon_event_groupval)
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
			if (misc_value_min != comp.misc_value_min)
			{
				return false;
			}
			if (misc_value_max != comp.misc_value_max)
			{
				return false;
			}
			if (comp.Dungeon_candidateval.Count != Dungeon_candidateval.Count)
			{
				return false;
			}
			foreach (Dungeon_candidate item3 in Dungeon_candidateval)
			{
				bool flag2 = false;
				foreach (Dungeon_candidate item4 in comp.Dungeon_candidateval)
				{
					if (item3.Equal(item4))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					return false;
				}
			}
			if (spawnable_misc_id != comp.spawnable_misc_id)
			{
				return false;
			}
			if (random_teleport_rate != comp.random_teleport_rate)
			{
				return false;
			}
			if (comp.random_teleport_start_count.Count != random_teleport_start_count.Count)
			{
				return false;
			}
			foreach (int item5 in random_teleport_start_count)
			{
				if (!comp.random_teleport_start_count.Contains(item5))
				{
					return false;
				}
			}
			if (comp.random_teleport_end_count.Count != random_teleport_end_count.Count)
			{
				return false;
			}
			foreach (int item6 in random_teleport_end_count)
			{
				if (!comp.random_teleport_end_count.Contains(item6))
				{
					return false;
				}
			}
			if (conta_increase_idle != comp.conta_increase_idle)
			{
				return false;
			}
			if (conta_increase_run != comp.conta_increase_run)
			{
				return false;
			}
			if (is_active != comp.is_active)
			{
				return false;
			}
			if (min_session_count != comp.min_session_count)
			{
				return false;
			}
			if (max_session_count != comp.max_session_count)
			{
				return false;
			}
			if (hazard_level != comp.hazard_level)
			{
				return false;
			}
			if (env_level != comp.env_level)
			{
				return false;
			}
			if (reward_level != comp.reward_level)
			{
				return false;
			}
			if (shop_group != comp.shop_group)
			{
				return false;
			}
			if (canopy_count != comp.canopy_count)
			{
				return false;
			}
			if (default_weather_id != comp.default_weather_id)
			{
				return false;
			}
			if (comp.weather_change.Count != weather_change.Count)
			{
				return false;
			}
			foreach (string item7 in weather_change)
			{
				if (!comp.weather_change.Contains(item7))
				{
					return false;
				}
			}
			if (comp.weather_random.Count != weather_random.Count)
			{
				return false;
			}
			foreach (string item8 in weather_random)
			{
				if (!comp.weather_random.Contains(item8))
				{
					return false;
				}
			}
			if (weather_random_prob != comp.weather_random_prob)
			{
				return false;
			}
			if (weather_random_min != comp.weather_random_min)
			{
				return false;
			}
			if (weather_random_max != comp.weather_random_max)
			{
				return false;
			}
			if (weather_random_duration != comp.weather_random_duration)
			{
				return false;
			}
			if (threat_min != comp.threat_min)
			{
				return false;
			}
			if (threat_max != comp.threat_max)
			{
				return false;
			}
			if (normal_monster_spawn_period != comp.normal_monster_spawn_period)
			{
				return false;
			}
			if (normal_monster_spawn_try_count != comp.normal_monster_spawn_try_count)
			{
				return false;
			}
			if (normal_monster_spawn_rate != comp.normal_monster_spawn_rate)
			{
				return false;
			}
			if (normal_master_ids != comp.normal_master_ids)
			{
				return false;
			}
			if (mimic_spawn_count_min != comp.mimic_spawn_count_min)
			{
				return false;
			}
			if (mimic_spawn_count_max != comp.mimic_spawn_count_max)
			{
				return false;
			}
			if (mimic_spawn_period != comp.mimic_spawn_period)
			{
				return false;
			}
			if (mimic_spawn_try_count != comp.mimic_spawn_try_count)
			{
				return false;
			}
			if (mimic_spawn_rate != comp.mimic_spawn_rate)
			{
				return false;
			}
			if (loading_scene_name != comp.loading_scene_name)
			{
				return false;
			}
			return true;
		}

		public Dungeon_MasterData Clone()
		{
			Dungeon_MasterData dungeon_MasterData = new Dungeon_MasterData();
			CopyTo(dungeon_MasterData);
			return dungeon_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			map_id = 0;
			map_name = string.Empty;
			start_display_time = string.Empty;
			end_time = string.Empty;
			Dungeon_event_groupval.Clear();
			misc_value_min = 0;
			misc_value_max = 0;
			Dungeon_candidateval.Clear();
			spawnable_misc_id = 0;
			random_teleport_rate = 0;
			random_teleport_start_count.Clear();
			random_teleport_end_count.Clear();
			conta_increase_idle = 0;
			conta_increase_run = 0;
			is_active = false;
			min_session_count = 0;
			max_session_count = 0;
			hazard_level = 0;
			env_level = 0;
			reward_level = 0;
			shop_group = 0;
			canopy_count = 0;
			default_weather_id = 0;
			weather_change.Clear();
			weather_random.Clear();
			weather_random_prob = 0;
			weather_random_min = 0;
			weather_random_max = 0;
			weather_random_duration = 0;
			threat_min = 0;
			threat_max = 0;
			normal_monster_spawn_period = 0;
			normal_monster_spawn_try_count = 0;
			normal_monster_spawn_rate = 0;
			normal_master_ids = 0;
			mimic_spawn_count_min = 0;
			mimic_spawn_count_max = 0;
			mimic_spawn_period = 0;
			mimic_spawn_try_count = 0;
			mimic_spawn_rate = 0;
			loading_scene_name = string.Empty;
		}
	}
}
