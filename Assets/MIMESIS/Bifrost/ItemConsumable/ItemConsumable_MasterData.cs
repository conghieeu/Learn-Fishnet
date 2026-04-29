using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ItemConsumable
{
	public class ItemConsumable_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public string looting_object_id = string.Empty;

		public List<string> tool_tip_string = new List<string>();

		public string vending_machine_tooltip_string = string.Empty;

		public int consume_type;

		public int bullet_type;

		public int max_stack_count;

		public int default_provide_count;

		public int weight;

		public int price_for_sell_min;

		public int price_for_sell_max;

		public List<string> actions = new List<string>();

		public int item_drop_id;

		public string attach_socket_name = string.Empty;

		public int puppet_handheld_state;

		public bool use_vending_machine_exchange;

		public bool is_preserved_on_wipe;

		public int link_skill_target_effect_data_id;

		public int handheld_auraskill_id;

		public int handheld_abnormal_id;

		public int spawn_fieldskill_env_condition;

		public int spawn_fieldskill_enclosure_condition;

		public int spawn_fieldskill_time_min;

		public int spawn_fieldskill_time_max;

		public int fieldskill_spawn_rate;

		public int spawn_fieldskill_wait_time;

		public int spawn_fieldskill_id;

		public List<string> spawn_fieldskill_wait_effect = new List<string>();

		public int sound_aggro_per_use;

		public int sound_aggro_in_hand_per_tick;

		public int sound_aggro_in_hand_toggle_on_per_tick;

		public bool hide_item_by_emote;

		public bool is_promotion_item;

		public bool is_promotion_item_hidden;

		public ItemConsumable_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemConsumable_MasterData()
			: base(4124252889u, "ItemConsumable_MasterData")
		{
		}

		public void CopyTo(ItemConsumable_MasterData dest)
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
			dest.consume_type = consume_type;
			dest.bullet_type = bullet_type;
			dest.max_stack_count = max_stack_count;
			dest.default_provide_count = default_provide_count;
			dest.weight = weight;
			dest.price_for_sell_min = price_for_sell_min;
			dest.price_for_sell_max = price_for_sell_max;
			dest.actions.Clear();
			foreach (string action in actions)
			{
				dest.actions.Add(action);
			}
			dest.item_drop_id = item_drop_id;
			dest.attach_socket_name = attach_socket_name;
			dest.puppet_handheld_state = puppet_handheld_state;
			dest.use_vending_machine_exchange = use_vending_machine_exchange;
			dest.is_preserved_on_wipe = is_preserved_on_wipe;
			dest.link_skill_target_effect_data_id = link_skill_target_effect_data_id;
			dest.handheld_auraskill_id = handheld_auraskill_id;
			dest.handheld_abnormal_id = handheld_abnormal_id;
			dest.spawn_fieldskill_env_condition = spawn_fieldskill_env_condition;
			dest.spawn_fieldskill_enclosure_condition = spawn_fieldskill_enclosure_condition;
			dest.spawn_fieldskill_time_min = spawn_fieldskill_time_min;
			dest.spawn_fieldskill_time_max = spawn_fieldskill_time_max;
			dest.fieldskill_spawn_rate = fieldskill_spawn_rate;
			dest.spawn_fieldskill_wait_time = spawn_fieldskill_wait_time;
			dest.spawn_fieldskill_id = spawn_fieldskill_id;
			dest.spawn_fieldskill_wait_effect.Clear();
			foreach (string item2 in spawn_fieldskill_wait_effect)
			{
				dest.spawn_fieldskill_wait_effect.Add(item2);
			}
			dest.sound_aggro_per_use = sound_aggro_per_use;
			dest.sound_aggro_in_hand_per_tick = sound_aggro_in_hand_per_tick;
			dest.sound_aggro_in_hand_toggle_on_per_tick = sound_aggro_in_hand_toggle_on_per_tick;
			dest.hide_item_by_emote = hide_item_by_emote;
			dest.is_promotion_item = is_promotion_item;
			dest.is_promotion_item_hidden = is_promotion_item_hidden;
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
			num += Serializer.GetLength(consume_type);
			num += Serializer.GetLength(bullet_type);
			num += Serializer.GetLength(max_stack_count);
			num += Serializer.GetLength(default_provide_count);
			num += Serializer.GetLength(weight);
			num += Serializer.GetLength(price_for_sell_min);
			num += Serializer.GetLength(price_for_sell_max);
			num += 4;
			foreach (string action in actions)
			{
				num += Serializer.GetLength(action);
			}
			num += Serializer.GetLength(item_drop_id);
			num += Serializer.GetLength(attach_socket_name);
			num += Serializer.GetLength(puppet_handheld_state);
			num += Serializer.GetLength(use_vending_machine_exchange);
			num += Serializer.GetLength(is_preserved_on_wipe);
			num += Serializer.GetLength(link_skill_target_effect_data_id);
			num += Serializer.GetLength(handheld_auraskill_id);
			num += Serializer.GetLength(handheld_abnormal_id);
			num += Serializer.GetLength(spawn_fieldskill_env_condition);
			num += Serializer.GetLength(spawn_fieldskill_enclosure_condition);
			num += Serializer.GetLength(spawn_fieldskill_time_min);
			num += Serializer.GetLength(spawn_fieldskill_time_max);
			num += Serializer.GetLength(fieldskill_spawn_rate);
			num += Serializer.GetLength(spawn_fieldskill_wait_time);
			num += Serializer.GetLength(spawn_fieldskill_id);
			num += 4;
			foreach (string item2 in spawn_fieldskill_wait_effect)
			{
				num += Serializer.GetLength(item2);
			}
			num += Serializer.GetLength(sound_aggro_per_use);
			num += Serializer.GetLength(sound_aggro_in_hand_per_tick);
			num += Serializer.GetLength(sound_aggro_in_hand_toggle_on_per_tick);
			num += Serializer.GetLength(hide_item_by_emote);
			num += Serializer.GetLength(is_promotion_item);
			return num + Serializer.GetLength(is_promotion_item_hidden);
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
			Serializer.Load(br, ref consume_type);
			Serializer.Load(br, ref bullet_type);
			Serializer.Load(br, ref max_stack_count);
			Serializer.Load(br, ref default_provide_count);
			Serializer.Load(br, ref weight);
			Serializer.Load(br, ref price_for_sell_min);
			Serializer.Load(br, ref price_for_sell_max);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				actions.Add(strValue2);
			}
			Serializer.Load(br, ref item_drop_id);
			Serializer.Load(br, ref attach_socket_name);
			Serializer.Load(br, ref puppet_handheld_state);
			Serializer.Load(br, ref use_vending_machine_exchange);
			Serializer.Load(br, ref is_preserved_on_wipe);
			Serializer.Load(br, ref link_skill_target_effect_data_id);
			Serializer.Load(br, ref handheld_auraskill_id);
			Serializer.Load(br, ref handheld_abnormal_id);
			Serializer.Load(br, ref spawn_fieldskill_env_condition);
			Serializer.Load(br, ref spawn_fieldskill_enclosure_condition);
			Serializer.Load(br, ref spawn_fieldskill_time_min);
			Serializer.Load(br, ref spawn_fieldskill_time_max);
			Serializer.Load(br, ref fieldskill_spawn_rate);
			Serializer.Load(br, ref spawn_fieldskill_wait_time);
			Serializer.Load(br, ref spawn_fieldskill_id);
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				string strValue3 = string.Empty;
				Serializer.Load(br, ref strValue3);
				spawn_fieldskill_wait_effect.Add(strValue3);
			}
			Serializer.Load(br, ref sound_aggro_per_use);
			Serializer.Load(br, ref sound_aggro_in_hand_per_tick);
			Serializer.Load(br, ref sound_aggro_in_hand_toggle_on_per_tick);
			Serializer.Load(br, ref hide_item_by_emote);
			Serializer.Load(br, ref is_promotion_item);
			Serializer.Load(br, ref is_promotion_item_hidden);
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
			Serializer.Save(bw, consume_type);
			Serializer.Save(bw, bullet_type);
			Serializer.Save(bw, max_stack_count);
			Serializer.Save(bw, default_provide_count);
			Serializer.Save(bw, weight);
			Serializer.Save(bw, price_for_sell_min);
			Serializer.Save(bw, price_for_sell_max);
			Serializer.Save(bw, actions.Count);
			foreach (string action in actions)
			{
				Serializer.Save(bw, action);
			}
			Serializer.Save(bw, item_drop_id);
			Serializer.Save(bw, attach_socket_name);
			Serializer.Save(bw, puppet_handheld_state);
			Serializer.Save(bw, use_vending_machine_exchange);
			Serializer.Save(bw, is_preserved_on_wipe);
			Serializer.Save(bw, link_skill_target_effect_data_id);
			Serializer.Save(bw, handheld_auraskill_id);
			Serializer.Save(bw, handheld_abnormal_id);
			Serializer.Save(bw, spawn_fieldskill_env_condition);
			Serializer.Save(bw, spawn_fieldskill_enclosure_condition);
			Serializer.Save(bw, spawn_fieldskill_time_min);
			Serializer.Save(bw, spawn_fieldskill_time_max);
			Serializer.Save(bw, fieldskill_spawn_rate);
			Serializer.Save(bw, spawn_fieldskill_wait_time);
			Serializer.Save(bw, spawn_fieldskill_id);
			Serializer.Save(bw, spawn_fieldskill_wait_effect.Count);
			foreach (string item2 in spawn_fieldskill_wait_effect)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, sound_aggro_per_use);
			Serializer.Save(bw, sound_aggro_in_hand_per_tick);
			Serializer.Save(bw, sound_aggro_in_hand_toggle_on_per_tick);
			Serializer.Save(bw, hide_item_by_emote);
			Serializer.Save(bw, is_promotion_item);
			Serializer.Save(bw, is_promotion_item_hidden);
		}

		public bool Equal(ItemConsumable_MasterData comp)
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
			if (consume_type != comp.consume_type)
			{
				return false;
			}
			if (bullet_type != comp.bullet_type)
			{
				return false;
			}
			if (max_stack_count != comp.max_stack_count)
			{
				return false;
			}
			if (default_provide_count != comp.default_provide_count)
			{
				return false;
			}
			if (weight != comp.weight)
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
			if (comp.actions.Count != actions.Count)
			{
				return false;
			}
			foreach (string action in actions)
			{
				if (!comp.actions.Contains(action))
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
			if (is_preserved_on_wipe != comp.is_preserved_on_wipe)
			{
				return false;
			}
			if (link_skill_target_effect_data_id != comp.link_skill_target_effect_data_id)
			{
				return false;
			}
			if (handheld_auraskill_id != comp.handheld_auraskill_id)
			{
				return false;
			}
			if (handheld_abnormal_id != comp.handheld_abnormal_id)
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
			foreach (string item2 in spawn_fieldskill_wait_effect)
			{
				if (!comp.spawn_fieldskill_wait_effect.Contains(item2))
				{
					return false;
				}
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
			return true;
		}

		public ItemConsumable_MasterData Clone()
		{
			ItemConsumable_MasterData itemConsumable_MasterData = new ItemConsumable_MasterData();
			CopyTo(itemConsumable_MasterData);
			return itemConsumable_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			looting_object_id = string.Empty;
			tool_tip_string.Clear();
			vending_machine_tooltip_string = string.Empty;
			consume_type = 0;
			bullet_type = 0;
			max_stack_count = 0;
			default_provide_count = 0;
			weight = 0;
			price_for_sell_min = 0;
			price_for_sell_max = 0;
			actions.Clear();
			item_drop_id = 0;
			attach_socket_name = string.Empty;
			puppet_handheld_state = 0;
			use_vending_machine_exchange = false;
			is_preserved_on_wipe = false;
			link_skill_target_effect_data_id = 0;
			handheld_auraskill_id = 0;
			handheld_abnormal_id = 0;
			spawn_fieldskill_env_condition = 0;
			spawn_fieldskill_enclosure_condition = 0;
			spawn_fieldskill_time_min = 0;
			spawn_fieldskill_time_max = 0;
			fieldskill_spawn_rate = 0;
			spawn_fieldskill_wait_time = 0;
			spawn_fieldskill_id = 0;
			spawn_fieldskill_wait_effect.Clear();
			sound_aggro_per_use = 0;
			sound_aggro_in_hand_per_tick = 0;
			sound_aggro_in_hand_toggle_on_per_tick = 0;
			hide_item_by_emote = false;
			is_promotion_item = false;
			is_promotion_item_hidden = false;
		}
	}
}
