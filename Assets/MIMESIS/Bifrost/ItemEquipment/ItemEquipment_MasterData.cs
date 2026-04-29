using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ItemEquipment
{
	public class ItemEquipment_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public string looting_object_id = string.Empty;

		public List<string> tool_tip_string = new List<string>();

		public string vending_machine_tooltip_string = string.Empty;

		public int equipment_function_group_id;

		public int price_for_sell_min;

		public int price_for_sell_max;

		public bool forbid_change;

		public int equip_type;

		public string key_group = string.Empty;

		public bool is_two_hand;

		public bool is_remains_active_in_inven;

		public int hand_weapon_type;

		public List<int> skill_list = new List<int>();

		public List<int> skill_gauge_on = new List<int>();

		public int skill_reload;

		public int max_gauge;

		public int price_inc_per_gauge;

		public int overflow_price;

		public bool use_charge;

		public bool visible_gauge_count;

		public bool visible_durability_count;

		public int dec_gauge_per_use;

		public int dec_gauge_initial_only;

		public int default_provide_gauge;

		public int min_durability;

		public int max_durability;

		public int weight;

		public List<string> stat_list = new List<string>();

		public int item_drop_id;

		public string attach_socket_name = string.Empty;

		public int puppet_handheld_state;

		public bool use_vending_machine_exchange;

		public List<float> item_slice_external_force = new List<float>();

		public int inc_gauge_when_move;

		public int dec_gauge_use_period;

		public int min_gauge_when_looting;

		public int max_gauge_when_looting;

		public int dec_durability_per_looting;

		public List<string> action_when_destroy = new List<string>();

		public bool use_destroy_by_gauge;

		public bool is_preserved_on_wipe;

		public int handheld_auraskill_id;

		public bool handheld_auraskill_by_gauge;

		public int handheld_abnormal_id;

		public bool handheld_abnormal_by_gauge;

		public int spawn_fieldskill_env_condition;

		public int spawn_fieldskill_enclosure_condition;

		public int spawn_fieldskill_time_min;

		public int spawn_fieldskill_time_max;

		public int fieldskill_spawn_rate;

		public int spawn_fieldskill_wait_time;

		public int spawn_fieldskill_id;

		public List<string> spawn_fieldskill_wait_effect = new List<string>();

		public bool use_bonus_item;

		public int blackout_rate;

		public int sound_aggro_per_use;

		public int sound_aggro_in_hand_per_tick;

		public int sound_aggro_in_hand_toggle_on_per_tick;

		public int scrap_random_play_cycle;

		public int scrap_random_play_rate;

		public int scrap_random_play_start_delay;

		public bool hide_item_by_emote;

		public bool is_promotion_item;

		public bool is_promotion_item_hidden;

		public bool use_item_upgrade;

		public int item_upgradedid;

		public int item_upgrade_cost;

		public ItemEquipment_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemEquipment_MasterData()
			: base(647824871u, "ItemEquipment_MasterData")
		{
		}

		public void CopyTo(ItemEquipment_MasterData dest)
		{
			dest.id = id;
			dest.name = name;
			dest.looting_object_id = looting_object_id;
			dest.tool_tip_string.Clear();
			foreach (string item in tool_tip_string)
			{
				dest.tool_tip_string.Add(item);
			}
			dest.vending_machine_tooltip_string = vending_machine_tooltip_string;
			dest.equipment_function_group_id = equipment_function_group_id;
			dest.price_for_sell_min = price_for_sell_min;
			dest.price_for_sell_max = price_for_sell_max;
			dest.forbid_change = forbid_change;
			dest.equip_type = equip_type;
			dest.key_group = key_group;
			dest.is_two_hand = is_two_hand;
			dest.is_remains_active_in_inven = is_remains_active_in_inven;
			dest.hand_weapon_type = hand_weapon_type;
			dest.skill_list.Clear();
			foreach (int item2 in skill_list)
			{
				dest.skill_list.Add(item2);
			}
			dest.skill_gauge_on.Clear();
			foreach (int item3 in skill_gauge_on)
			{
				dest.skill_gauge_on.Add(item3);
			}
			dest.skill_reload = skill_reload;
			dest.max_gauge = max_gauge;
			dest.price_inc_per_gauge = price_inc_per_gauge;
			dest.overflow_price = overflow_price;
			dest.use_charge = use_charge;
			dest.visible_gauge_count = visible_gauge_count;
			dest.visible_durability_count = visible_durability_count;
			dest.dec_gauge_per_use = dec_gauge_per_use;
			dest.dec_gauge_initial_only = dec_gauge_initial_only;
			dest.default_provide_gauge = default_provide_gauge;
			dest.min_durability = min_durability;
			dest.max_durability = max_durability;
			dest.weight = weight;
			dest.stat_list.Clear();
			foreach (string item4 in stat_list)
			{
				dest.stat_list.Add(item4);
			}
			dest.item_drop_id = item_drop_id;
			dest.attach_socket_name = attach_socket_name;
			dest.puppet_handheld_state = puppet_handheld_state;
			dest.use_vending_machine_exchange = use_vending_machine_exchange;
			dest.item_slice_external_force.Clear();
			foreach (float item5 in item_slice_external_force)
			{
				dest.item_slice_external_force.Add(item5);
			}
			dest.inc_gauge_when_move = inc_gauge_when_move;
			dest.dec_gauge_use_period = dec_gauge_use_period;
			dest.min_gauge_when_looting = min_gauge_when_looting;
			dest.max_gauge_when_looting = max_gauge_when_looting;
			dest.dec_durability_per_looting = dec_durability_per_looting;
			dest.action_when_destroy.Clear();
			foreach (string item6 in action_when_destroy)
			{
				dest.action_when_destroy.Add(item6);
			}
			dest.use_destroy_by_gauge = use_destroy_by_gauge;
			dest.is_preserved_on_wipe = is_preserved_on_wipe;
			dest.handheld_auraskill_id = handheld_auraskill_id;
			dest.handheld_auraskill_by_gauge = handheld_auraskill_by_gauge;
			dest.handheld_abnormal_id = handheld_abnormal_id;
			dest.handheld_abnormal_by_gauge = handheld_abnormal_by_gauge;
			dest.spawn_fieldskill_env_condition = spawn_fieldskill_env_condition;
			dest.spawn_fieldskill_enclosure_condition = spawn_fieldskill_enclosure_condition;
			dest.spawn_fieldskill_time_min = spawn_fieldskill_time_min;
			dest.spawn_fieldskill_time_max = spawn_fieldskill_time_max;
			dest.fieldskill_spawn_rate = fieldskill_spawn_rate;
			dest.spawn_fieldskill_wait_time = spawn_fieldskill_wait_time;
			dest.spawn_fieldskill_id = spawn_fieldskill_id;
			dest.spawn_fieldskill_wait_effect.Clear();
			foreach (string item7 in spawn_fieldskill_wait_effect)
			{
				dest.spawn_fieldskill_wait_effect.Add(item7);
			}
			dest.use_bonus_item = use_bonus_item;
			dest.blackout_rate = blackout_rate;
			dest.sound_aggro_per_use = sound_aggro_per_use;
			dest.sound_aggro_in_hand_per_tick = sound_aggro_in_hand_per_tick;
			dest.sound_aggro_in_hand_toggle_on_per_tick = sound_aggro_in_hand_toggle_on_per_tick;
			dest.scrap_random_play_cycle = scrap_random_play_cycle;
			dest.scrap_random_play_rate = scrap_random_play_rate;
			dest.scrap_random_play_start_delay = scrap_random_play_start_delay;
			dest.hide_item_by_emote = hide_item_by_emote;
			dest.is_promotion_item = is_promotion_item;
			dest.is_promotion_item_hidden = is_promotion_item_hidden;
			dest.use_item_upgrade = use_item_upgrade;
			dest.item_upgradedid = item_upgradedid;
			dest.item_upgrade_cost = item_upgrade_cost;
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
			num += Serializer.GetLength(looting_object_id);
			num += 4;
			foreach (string item in tool_tip_string)
			{
				num += Serializer.GetLength(item);
			}
			num += Serializer.GetLength(vending_machine_tooltip_string);
			num += Serializer.GetLength(equipment_function_group_id);
			num += Serializer.GetLength(price_for_sell_min);
			num += Serializer.GetLength(price_for_sell_max);
			num += Serializer.GetLength(forbid_change);
			num += Serializer.GetLength(equip_type);
			num += Serializer.GetLength(key_group);
			num += Serializer.GetLength(is_two_hand);
			num += Serializer.GetLength(is_remains_active_in_inven);
			num += Serializer.GetLength(hand_weapon_type);
			num += 4;
			foreach (int item2 in skill_list)
			{
				num += Serializer.GetLength(item2);
			}
			num += 4;
			foreach (int item3 in skill_gauge_on)
			{
				num += Serializer.GetLength(item3);
			}
			num += Serializer.GetLength(skill_reload);
			num += Serializer.GetLength(max_gauge);
			num += Serializer.GetLength(price_inc_per_gauge);
			num += Serializer.GetLength(overflow_price);
			num += Serializer.GetLength(use_charge);
			num += Serializer.GetLength(visible_gauge_count);
			num += Serializer.GetLength(visible_durability_count);
			num += Serializer.GetLength(dec_gauge_per_use);
			num += Serializer.GetLength(dec_gauge_initial_only);
			num += Serializer.GetLength(default_provide_gauge);
			num += Serializer.GetLength(min_durability);
			num += Serializer.GetLength(max_durability);
			num += Serializer.GetLength(weight);
			num += 4;
			foreach (string item4 in stat_list)
			{
				num += Serializer.GetLength(item4);
			}
			num += Serializer.GetLength(item_drop_id);
			num += Serializer.GetLength(attach_socket_name);
			num += Serializer.GetLength(puppet_handheld_state);
			num += Serializer.GetLength(use_vending_machine_exchange);
			num += 4;
			foreach (float item5 in item_slice_external_force)
			{
				num += Serializer.GetLength(item5);
			}
			num += Serializer.GetLength(inc_gauge_when_move);
			num += Serializer.GetLength(dec_gauge_use_period);
			num += Serializer.GetLength(min_gauge_when_looting);
			num += Serializer.GetLength(max_gauge_when_looting);
			num += Serializer.GetLength(dec_durability_per_looting);
			num += 4;
			foreach (string item6 in action_when_destroy)
			{
				num += Serializer.GetLength(item6);
			}
			num += Serializer.GetLength(use_destroy_by_gauge);
			num += Serializer.GetLength(is_preserved_on_wipe);
			num += Serializer.GetLength(handheld_auraskill_id);
			num += Serializer.GetLength(handheld_auraskill_by_gauge);
			num += Serializer.GetLength(handheld_abnormal_id);
			num += Serializer.GetLength(handheld_abnormal_by_gauge);
			num += Serializer.GetLength(spawn_fieldskill_env_condition);
			num += Serializer.GetLength(spawn_fieldskill_enclosure_condition);
			num += Serializer.GetLength(spawn_fieldskill_time_min);
			num += Serializer.GetLength(spawn_fieldskill_time_max);
			num += Serializer.GetLength(fieldskill_spawn_rate);
			num += Serializer.GetLength(spawn_fieldskill_wait_time);
			num += Serializer.GetLength(spawn_fieldskill_id);
			num += 4;
			foreach (string item7 in spawn_fieldskill_wait_effect)
			{
				num += Serializer.GetLength(item7);
			}
			num += Serializer.GetLength(use_bonus_item);
			num += Serializer.GetLength(blackout_rate);
			num += Serializer.GetLength(sound_aggro_per_use);
			num += Serializer.GetLength(sound_aggro_in_hand_per_tick);
			num += Serializer.GetLength(sound_aggro_in_hand_toggle_on_per_tick);
			num += Serializer.GetLength(scrap_random_play_cycle);
			num += Serializer.GetLength(scrap_random_play_rate);
			num += Serializer.GetLength(scrap_random_play_start_delay);
			num += Serializer.GetLength(hide_item_by_emote);
			num += Serializer.GetLength(is_promotion_item);
			num += Serializer.GetLength(is_promotion_item_hidden);
			num += Serializer.GetLength(use_item_upgrade);
			num += Serializer.GetLength(item_upgradedid);
			return num + Serializer.GetLength(item_upgrade_cost);
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
			Serializer.Load(br, ref looting_object_id);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				tool_tip_string.Add(strValue);
			}
			Serializer.Load(br, ref vending_machine_tooltip_string);
			Serializer.Load(br, ref equipment_function_group_id);
			Serializer.Load(br, ref price_for_sell_min);
			Serializer.Load(br, ref price_for_sell_max);
			Serializer.Load(br, ref forbid_change);
			Serializer.Load(br, ref equip_type);
			Serializer.Load(br, ref key_group);
			Serializer.Load(br, ref is_two_hand);
			Serializer.Load(br, ref is_remains_active_in_inven);
			Serializer.Load(br, ref hand_weapon_type);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				int intValue3 = 0;
				Serializer.Load(br, ref intValue3);
				skill_list.Add(intValue3);
			}
			int intValue4 = 0;
			Serializer.Load(br, ref intValue4);
			while (intValue4-- > 0)
			{
				int intValue5 = 0;
				Serializer.Load(br, ref intValue5);
				skill_gauge_on.Add(intValue5);
			}
			Serializer.Load(br, ref skill_reload);
			Serializer.Load(br, ref max_gauge);
			Serializer.Load(br, ref price_inc_per_gauge);
			Serializer.Load(br, ref overflow_price);
			Serializer.Load(br, ref use_charge);
			Serializer.Load(br, ref visible_gauge_count);
			Serializer.Load(br, ref visible_durability_count);
			Serializer.Load(br, ref dec_gauge_per_use);
			Serializer.Load(br, ref dec_gauge_initial_only);
			Serializer.Load(br, ref default_provide_gauge);
			Serializer.Load(br, ref min_durability);
			Serializer.Load(br, ref max_durability);
			Serializer.Load(br, ref weight);
			int intValue6 = 0;
			Serializer.Load(br, ref intValue6);
			while (intValue6-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				stat_list.Add(strValue2);
			}
			Serializer.Load(br, ref item_drop_id);
			Serializer.Load(br, ref attach_socket_name);
			Serializer.Load(br, ref puppet_handheld_state);
			Serializer.Load(br, ref use_vending_machine_exchange);
			int intValue7 = 0;
			Serializer.Load(br, ref intValue7);
			while (intValue7-- > 0)
			{
				float nValue = 0f;
				Serializer.Load(br, ref nValue);
				item_slice_external_force.Add(nValue);
			}
			Serializer.Load(br, ref inc_gauge_when_move);
			Serializer.Load(br, ref dec_gauge_use_period);
			Serializer.Load(br, ref min_gauge_when_looting);
			Serializer.Load(br, ref max_gauge_when_looting);
			Serializer.Load(br, ref dec_durability_per_looting);
			int intValue8 = 0;
			Serializer.Load(br, ref intValue8);
			while (intValue8-- > 0)
			{
				string strValue3 = string.Empty;
				Serializer.Load(br, ref strValue3);
				action_when_destroy.Add(strValue3);
			}
			Serializer.Load(br, ref use_destroy_by_gauge);
			Serializer.Load(br, ref is_preserved_on_wipe);
			Serializer.Load(br, ref handheld_auraskill_id);
			Serializer.Load(br, ref handheld_auraskill_by_gauge);
			Serializer.Load(br, ref handheld_abnormal_id);
			Serializer.Load(br, ref handheld_abnormal_by_gauge);
			Serializer.Load(br, ref spawn_fieldskill_env_condition);
			Serializer.Load(br, ref spawn_fieldskill_enclosure_condition);
			Serializer.Load(br, ref spawn_fieldskill_time_min);
			Serializer.Load(br, ref spawn_fieldskill_time_max);
			Serializer.Load(br, ref fieldskill_spawn_rate);
			Serializer.Load(br, ref spawn_fieldskill_wait_time);
			Serializer.Load(br, ref spawn_fieldskill_id);
			int intValue9 = 0;
			Serializer.Load(br, ref intValue9);
			while (intValue9-- > 0)
			{
				string strValue4 = string.Empty;
				Serializer.Load(br, ref strValue4);
				spawn_fieldskill_wait_effect.Add(strValue4);
			}
			Serializer.Load(br, ref use_bonus_item);
			Serializer.Load(br, ref blackout_rate);
			Serializer.Load(br, ref sound_aggro_per_use);
			Serializer.Load(br, ref sound_aggro_in_hand_per_tick);
			Serializer.Load(br, ref sound_aggro_in_hand_toggle_on_per_tick);
			Serializer.Load(br, ref scrap_random_play_cycle);
			Serializer.Load(br, ref scrap_random_play_rate);
			Serializer.Load(br, ref scrap_random_play_start_delay);
			Serializer.Load(br, ref hide_item_by_emote);
			Serializer.Load(br, ref is_promotion_item);
			Serializer.Load(br, ref is_promotion_item_hidden);
			Serializer.Load(br, ref use_item_upgrade);
			Serializer.Load(br, ref item_upgradedid);
			Serializer.Load(br, ref item_upgrade_cost);
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
			Serializer.Save(bw, looting_object_id);
			Serializer.Save(bw, tool_tip_string.Count);
			foreach (string item in tool_tip_string)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, vending_machine_tooltip_string);
			Serializer.Save(bw, equipment_function_group_id);
			Serializer.Save(bw, price_for_sell_min);
			Serializer.Save(bw, price_for_sell_max);
			Serializer.Save(bw, forbid_change);
			Serializer.Save(bw, equip_type);
			Serializer.Save(bw, key_group);
			Serializer.Save(bw, is_two_hand);
			Serializer.Save(bw, is_remains_active_in_inven);
			Serializer.Save(bw, hand_weapon_type);
			Serializer.Save(bw, skill_list.Count);
			foreach (int item2 in skill_list)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, skill_gauge_on.Count);
			foreach (int item3 in skill_gauge_on)
			{
				Serializer.Save(bw, item3);
			}
			Serializer.Save(bw, skill_reload);
			Serializer.Save(bw, max_gauge);
			Serializer.Save(bw, price_inc_per_gauge);
			Serializer.Save(bw, overflow_price);
			Serializer.Save(bw, use_charge);
			Serializer.Save(bw, visible_gauge_count);
			Serializer.Save(bw, visible_durability_count);
			Serializer.Save(bw, dec_gauge_per_use);
			Serializer.Save(bw, dec_gauge_initial_only);
			Serializer.Save(bw, default_provide_gauge);
			Serializer.Save(bw, min_durability);
			Serializer.Save(bw, max_durability);
			Serializer.Save(bw, weight);
			Serializer.Save(bw, stat_list.Count);
			foreach (string item4 in stat_list)
			{
				Serializer.Save(bw, item4);
			}
			Serializer.Save(bw, item_drop_id);
			Serializer.Save(bw, attach_socket_name);
			Serializer.Save(bw, puppet_handheld_state);
			Serializer.Save(bw, use_vending_machine_exchange);
			Serializer.Save(bw, item_slice_external_force.Count);
			foreach (float item5 in item_slice_external_force)
			{
				Serializer.Save(bw, item5);
			}
			Serializer.Save(bw, inc_gauge_when_move);
			Serializer.Save(bw, dec_gauge_use_period);
			Serializer.Save(bw, min_gauge_when_looting);
			Serializer.Save(bw, max_gauge_when_looting);
			Serializer.Save(bw, dec_durability_per_looting);
			Serializer.Save(bw, action_when_destroy.Count);
			foreach (string item6 in action_when_destroy)
			{
				Serializer.Save(bw, item6);
			}
			Serializer.Save(bw, use_destroy_by_gauge);
			Serializer.Save(bw, is_preserved_on_wipe);
			Serializer.Save(bw, handheld_auraskill_id);
			Serializer.Save(bw, handheld_auraskill_by_gauge);
			Serializer.Save(bw, handheld_abnormal_id);
			Serializer.Save(bw, handheld_abnormal_by_gauge);
			Serializer.Save(bw, spawn_fieldskill_env_condition);
			Serializer.Save(bw, spawn_fieldskill_enclosure_condition);
			Serializer.Save(bw, spawn_fieldskill_time_min);
			Serializer.Save(bw, spawn_fieldskill_time_max);
			Serializer.Save(bw, fieldskill_spawn_rate);
			Serializer.Save(bw, spawn_fieldskill_wait_time);
			Serializer.Save(bw, spawn_fieldskill_id);
			Serializer.Save(bw, spawn_fieldskill_wait_effect.Count);
			foreach (string item7 in spawn_fieldskill_wait_effect)
			{
				Serializer.Save(bw, item7);
			}
			Serializer.Save(bw, use_bonus_item);
			Serializer.Save(bw, blackout_rate);
			Serializer.Save(bw, sound_aggro_per_use);
			Serializer.Save(bw, sound_aggro_in_hand_per_tick);
			Serializer.Save(bw, sound_aggro_in_hand_toggle_on_per_tick);
			Serializer.Save(bw, scrap_random_play_cycle);
			Serializer.Save(bw, scrap_random_play_rate);
			Serializer.Save(bw, scrap_random_play_start_delay);
			Serializer.Save(bw, hide_item_by_emote);
			Serializer.Save(bw, is_promotion_item);
			Serializer.Save(bw, is_promotion_item_hidden);
			Serializer.Save(bw, use_item_upgrade);
			Serializer.Save(bw, item_upgradedid);
			Serializer.Save(bw, item_upgrade_cost);
		}

		public bool Equal(ItemEquipment_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (looting_object_id != comp.looting_object_id)
			{
				return false;
			}
			if (comp.tool_tip_string.Count != tool_tip_string.Count)
			{
				return false;
			}
			foreach (string item in tool_tip_string)
			{
				if (!comp.tool_tip_string.Contains(item))
				{
					return false;
				}
			}
			if (vending_machine_tooltip_string != comp.vending_machine_tooltip_string)
			{
				return false;
			}
			if (equipment_function_group_id != comp.equipment_function_group_id)
			{
				return false;
			}
			if (price_for_sell_min != comp.price_for_sell_min)
			{
				return false;
			}
			if (price_for_sell_max != comp.price_for_sell_max)
			{
				return false;
			}
			if (forbid_change != comp.forbid_change)
			{
				return false;
			}
			if (equip_type != comp.equip_type)
			{
				return false;
			}
			if (key_group != comp.key_group)
			{
				return false;
			}
			if (is_two_hand != comp.is_two_hand)
			{
				return false;
			}
			if (is_remains_active_in_inven != comp.is_remains_active_in_inven)
			{
				return false;
			}
			if (hand_weapon_type != comp.hand_weapon_type)
			{
				return false;
			}
			if (comp.skill_list.Count != skill_list.Count)
			{
				return false;
			}
			foreach (int item2 in skill_list)
			{
				if (!comp.skill_list.Contains(item2))
				{
					return false;
				}
			}
			if (comp.skill_gauge_on.Count != skill_gauge_on.Count)
			{
				return false;
			}
			foreach (int item3 in skill_gauge_on)
			{
				if (!comp.skill_gauge_on.Contains(item3))
				{
					return false;
				}
			}
			if (skill_reload != comp.skill_reload)
			{
				return false;
			}
			if (max_gauge != comp.max_gauge)
			{
				return false;
			}
			if (price_inc_per_gauge != comp.price_inc_per_gauge)
			{
				return false;
			}
			if (overflow_price != comp.overflow_price)
			{
				return false;
			}
			if (use_charge != comp.use_charge)
			{
				return false;
			}
			if (visible_gauge_count != comp.visible_gauge_count)
			{
				return false;
			}
			if (visible_durability_count != comp.visible_durability_count)
			{
				return false;
			}
			if (dec_gauge_per_use != comp.dec_gauge_per_use)
			{
				return false;
			}
			if (dec_gauge_initial_only != comp.dec_gauge_initial_only)
			{
				return false;
			}
			if (default_provide_gauge != comp.default_provide_gauge)
			{
				return false;
			}
			if (min_durability != comp.min_durability)
			{
				return false;
			}
			if (max_durability != comp.max_durability)
			{
				return false;
			}
			if (weight != comp.weight)
			{
				return false;
			}
			if (comp.stat_list.Count != stat_list.Count)
			{
				return false;
			}
			foreach (string item4 in stat_list)
			{
				if (!comp.stat_list.Contains(item4))
				{
					return false;
				}
			}
			if (item_drop_id != comp.item_drop_id)
			{
				return false;
			}
			if (attach_socket_name != comp.attach_socket_name)
			{
				return false;
			}
			if (puppet_handheld_state != comp.puppet_handheld_state)
			{
				return false;
			}
			if (use_vending_machine_exchange != comp.use_vending_machine_exchange)
			{
				return false;
			}
			if (comp.item_slice_external_force.Count != item_slice_external_force.Count)
			{
				return false;
			}
			foreach (float item5 in item_slice_external_force)
			{
				if (!comp.item_slice_external_force.Contains(item5))
				{
					return false;
				}
			}
			if (inc_gauge_when_move != comp.inc_gauge_when_move)
			{
				return false;
			}
			if (dec_gauge_use_period != comp.dec_gauge_use_period)
			{
				return false;
			}
			if (min_gauge_when_looting != comp.min_gauge_when_looting)
			{
				return false;
			}
			if (max_gauge_when_looting != comp.max_gauge_when_looting)
			{
				return false;
			}
			if (dec_durability_per_looting != comp.dec_durability_per_looting)
			{
				return false;
			}
			if (comp.action_when_destroy.Count != action_when_destroy.Count)
			{
				return false;
			}
			foreach (string item6 in action_when_destroy)
			{
				if (!comp.action_when_destroy.Contains(item6))
				{
					return false;
				}
			}
			if (use_destroy_by_gauge != comp.use_destroy_by_gauge)
			{
				return false;
			}
			if (is_preserved_on_wipe != comp.is_preserved_on_wipe)
			{
				return false;
			}
			if (handheld_auraskill_id != comp.handheld_auraskill_id)
			{
				return false;
			}
			if (handheld_auraskill_by_gauge != comp.handheld_auraskill_by_gauge)
			{
				return false;
			}
			if (handheld_abnormal_id != comp.handheld_abnormal_id)
			{
				return false;
			}
			if (handheld_abnormal_by_gauge != comp.handheld_abnormal_by_gauge)
			{
				return false;
			}
			if (spawn_fieldskill_env_condition != comp.spawn_fieldskill_env_condition)
			{
				return false;
			}
			if (spawn_fieldskill_enclosure_condition != comp.spawn_fieldskill_enclosure_condition)
			{
				return false;
			}
			if (spawn_fieldskill_time_min != comp.spawn_fieldskill_time_min)
			{
				return false;
			}
			if (spawn_fieldskill_time_max != comp.spawn_fieldskill_time_max)
			{
				return false;
			}
			if (fieldskill_spawn_rate != comp.fieldskill_spawn_rate)
			{
				return false;
			}
			if (spawn_fieldskill_wait_time != comp.spawn_fieldskill_wait_time)
			{
				return false;
			}
			if (spawn_fieldskill_id != comp.spawn_fieldskill_id)
			{
				return false;
			}
			if (comp.spawn_fieldskill_wait_effect.Count != spawn_fieldskill_wait_effect.Count)
			{
				return false;
			}
			foreach (string item7 in spawn_fieldskill_wait_effect)
			{
				if (!comp.spawn_fieldskill_wait_effect.Contains(item7))
				{
					return false;
				}
			}
			if (use_bonus_item != comp.use_bonus_item)
			{
				return false;
			}
			if (blackout_rate != comp.blackout_rate)
			{
				return false;
			}
			if (sound_aggro_per_use != comp.sound_aggro_per_use)
			{
				return false;
			}
			if (sound_aggro_in_hand_per_tick != comp.sound_aggro_in_hand_per_tick)
			{
				return false;
			}
			if (sound_aggro_in_hand_toggle_on_per_tick != comp.sound_aggro_in_hand_toggle_on_per_tick)
			{
				return false;
			}
			if (scrap_random_play_cycle != comp.scrap_random_play_cycle)
			{
				return false;
			}
			if (scrap_random_play_rate != comp.scrap_random_play_rate)
			{
				return false;
			}
			if (scrap_random_play_start_delay != comp.scrap_random_play_start_delay)
			{
				return false;
			}
			if (hide_item_by_emote != comp.hide_item_by_emote)
			{
				return false;
			}
			if (is_promotion_item != comp.is_promotion_item)
			{
				return false;
			}
			if (is_promotion_item_hidden != comp.is_promotion_item_hidden)
			{
				return false;
			}
			if (use_item_upgrade != comp.use_item_upgrade)
			{
				return false;
			}
			if (item_upgradedid != comp.item_upgradedid)
			{
				return false;
			}
			if (item_upgrade_cost != comp.item_upgrade_cost)
			{
				return false;
			}
			return true;
		}

		public ItemEquipment_MasterData Clone()
		{
			ItemEquipment_MasterData itemEquipment_MasterData = new ItemEquipment_MasterData();
			CopyTo(itemEquipment_MasterData);
			return itemEquipment_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			looting_object_id = string.Empty;
			tool_tip_string.Clear();
			vending_machine_tooltip_string = string.Empty;
			equipment_function_group_id = 0;
			price_for_sell_min = 0;
			price_for_sell_max = 0;
			forbid_change = false;
			equip_type = 0;
			key_group = string.Empty;
			is_two_hand = false;
			is_remains_active_in_inven = false;
			hand_weapon_type = 0;
			skill_list.Clear();
			skill_gauge_on.Clear();
			skill_reload = 0;
			max_gauge = 0;
			price_inc_per_gauge = 0;
			overflow_price = 0;
			use_charge = false;
			visible_gauge_count = false;
			visible_durability_count = false;
			dec_gauge_per_use = 0;
			dec_gauge_initial_only = 0;
			default_provide_gauge = 0;
			min_durability = 0;
			max_durability = 0;
			weight = 0;
			stat_list.Clear();
			item_drop_id = 0;
			attach_socket_name = string.Empty;
			puppet_handheld_state = 0;
			use_vending_machine_exchange = false;
			item_slice_external_force.Clear();
			inc_gauge_when_move = 0;
			dec_gauge_use_period = 0;
			min_gauge_when_looting = 0;
			max_gauge_when_looting = 0;
			dec_durability_per_looting = 0;
			action_when_destroy.Clear();
			use_destroy_by_gauge = false;
			is_preserved_on_wipe = false;
			handheld_auraskill_id = 0;
			handheld_auraskill_by_gauge = false;
			handheld_abnormal_id = 0;
			handheld_abnormal_by_gauge = false;
			spawn_fieldskill_env_condition = 0;
			spawn_fieldskill_enclosure_condition = 0;
			spawn_fieldskill_time_min = 0;
			spawn_fieldskill_time_max = 0;
			fieldskill_spawn_rate = 0;
			spawn_fieldskill_wait_time = 0;
			spawn_fieldskill_id = 0;
			spawn_fieldskill_wait_effect.Clear();
			use_bonus_item = false;
			blackout_rate = 0;
			sound_aggro_per_use = 0;
			sound_aggro_in_hand_per_tick = 0;
			sound_aggro_in_hand_toggle_on_per_tick = 0;
			scrap_random_play_cycle = 0;
			scrap_random_play_rate = 0;
			scrap_random_play_start_delay = 0;
			hide_item_by_emote = false;
			is_promotion_item = false;
			is_promotion_item_hidden = false;
			use_item_upgrade = false;
			item_upgradedid = 0;
			item_upgrade_cost = 0;
		}
	}
}
