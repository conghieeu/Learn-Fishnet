using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ItemMiscellany
{
	public class ItemMiscellany_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public string looting_object_id = string.Empty;

		public List<string> tool_tip_string = new List<string>();

		public string vending_machine_tooltip_string = string.Empty;

		public int price_for_sell_min;

		public int price_for_sell_max;

		public bool forbid_change;

		public int weight;

		public string key_group = string.Empty;

		public int item_drop_id;

		public string attach_socket_name = string.Empty;

		public int puppet_handheld_state;

		public bool use_vending_machine_exchange;

		public bool is_preserved_on_wipe;

		public int accessory_group;

		public bool is_hide_my_camera;

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

		public string scrap_animation_state_name = string.Empty;

		public bool is_moving_scrap_animation;

		public bool scrap_animation_loop;

		public bool use_bonus_item;

		public int sound_aggro_per_use;

		public int sound_aggro_in_hand_per_tick;

		public int sound_aggro_in_hand_toggle_on_per_tick;

		public bool hide_item_by_emote;

		public bool is_promotion_item;

		public bool is_promotion_item_hidden;

		public ItemMiscellany_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemMiscellany_MasterData()
			: base(920926196u, "ItemMiscellany_MasterData")
		{
		}

		public void CopyTo(ItemMiscellany_MasterData dest)
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
			dest.price_for_sell_min = price_for_sell_min;
			dest.price_for_sell_max = price_for_sell_max;
			dest.forbid_change = forbid_change;
			dest.weight = weight;
			dest.key_group = key_group;
			dest.item_drop_id = item_drop_id;
			dest.attach_socket_name = attach_socket_name;
			dest.puppet_handheld_state = puppet_handheld_state;
			dest.use_vending_machine_exchange = use_vending_machine_exchange;
			dest.is_preserved_on_wipe = is_preserved_on_wipe;
			dest.accessory_group = accessory_group;
			dest.is_hide_my_camera = is_hide_my_camera;
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
			dest.scrap_animation_state_name = scrap_animation_state_name;
			dest.is_moving_scrap_animation = is_moving_scrap_animation;
			dest.scrap_animation_loop = scrap_animation_loop;
			dest.use_bonus_item = use_bonus_item;
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
			num += Serializer.GetLength(price_for_sell_min);
			num += Serializer.GetLength(price_for_sell_max);
			num += Serializer.GetLength(forbid_change);
			num += Serializer.GetLength(weight);
			num += Serializer.GetLength(key_group);
			num += Serializer.GetLength(item_drop_id);
			num += Serializer.GetLength(attach_socket_name);
			num += Serializer.GetLength(puppet_handheld_state);
			num += Serializer.GetLength(use_vending_machine_exchange);
			num += Serializer.GetLength(is_preserved_on_wipe);
			num += Serializer.GetLength(accessory_group);
			num += Serializer.GetLength(is_hide_my_camera);
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
			num += Serializer.GetLength(scrap_animation_state_name);
			num += Serializer.GetLength(is_moving_scrap_animation);
			num += Serializer.GetLength(scrap_animation_loop);
			num += Serializer.GetLength(use_bonus_item);
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
			Serializer.Load(br, ref price_for_sell_min);
			Serializer.Load(br, ref price_for_sell_max);
			Serializer.Load(br, ref forbid_change);
			Serializer.Load(br, ref weight);
			Serializer.Load(br, ref key_group);
			Serializer.Load(br, ref item_drop_id);
			Serializer.Load(br, ref attach_socket_name);
			Serializer.Load(br, ref puppet_handheld_state);
			Serializer.Load(br, ref use_vending_machine_exchange);
			Serializer.Load(br, ref is_preserved_on_wipe);
			Serializer.Load(br, ref accessory_group);
			Serializer.Load(br, ref is_hide_my_camera);
			Serializer.Load(br, ref handheld_auraskill_id);
			Serializer.Load(br, ref handheld_abnormal_id);
			Serializer.Load(br, ref spawn_fieldskill_env_condition);
			Serializer.Load(br, ref spawn_fieldskill_enclosure_condition);
			Serializer.Load(br, ref spawn_fieldskill_time_min);
			Serializer.Load(br, ref spawn_fieldskill_time_max);
			Serializer.Load(br, ref fieldskill_spawn_rate);
			Serializer.Load(br, ref spawn_fieldskill_wait_time);
			Serializer.Load(br, ref spawn_fieldskill_id);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				spawn_fieldskill_wait_effect.Add(strValue2);
			}
			Serializer.Load(br, ref scrap_animation_state_name);
			Serializer.Load(br, ref is_moving_scrap_animation);
			Serializer.Load(br, ref scrap_animation_loop);
			Serializer.Load(br, ref use_bonus_item);
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
			Serializer.Save(bw, price_for_sell_min);
			Serializer.Save(bw, price_for_sell_max);
			Serializer.Save(bw, forbid_change);
			Serializer.Save(bw, weight);
			Serializer.Save(bw, key_group);
			Serializer.Save(bw, item_drop_id);
			Serializer.Save(bw, attach_socket_name);
			Serializer.Save(bw, puppet_handheld_state);
			Serializer.Save(bw, use_vending_machine_exchange);
			Serializer.Save(bw, is_preserved_on_wipe);
			Serializer.Save(bw, accessory_group);
			Serializer.Save(bw, is_hide_my_camera);
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
			Serializer.Save(bw, scrap_animation_state_name);
			Serializer.Save(bw, is_moving_scrap_animation);
			Serializer.Save(bw, scrap_animation_loop);
			Serializer.Save(bw, use_bonus_item);
			Serializer.Save(bw, sound_aggro_per_use);
			Serializer.Save(bw, sound_aggro_in_hand_per_tick);
			Serializer.Save(bw, sound_aggro_in_hand_toggle_on_per_tick);
			Serializer.Save(bw, hide_item_by_emote);
			Serializer.Save(bw, is_promotion_item);
			Serializer.Save(bw, is_promotion_item_hidden);
		}

		public bool Equal(ItemMiscellany_MasterData comp)
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
			if (weight != comp.weight)
			{
				return false;
			}
			if (key_group != comp.key_group)
			{
				return false;
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
			if (accessory_group != comp.accessory_group)
			{
				return false;
			}
			if (is_hide_my_camera != comp.is_hide_my_camera)
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
			if (scrap_animation_state_name != comp.scrap_animation_state_name)
			{
				return false;
			}
			if (is_moving_scrap_animation != comp.is_moving_scrap_animation)
			{
				return false;
			}
			if (scrap_animation_loop != comp.scrap_animation_loop)
			{
				return false;
			}
			if (use_bonus_item != comp.use_bonus_item)
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

		public ItemMiscellany_MasterData Clone()
		{
			ItemMiscellany_MasterData itemMiscellany_MasterData = new ItemMiscellany_MasterData();
			CopyTo(itemMiscellany_MasterData);
			return itemMiscellany_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			looting_object_id = string.Empty;
			tool_tip_string.Clear();
			vending_machine_tooltip_string = string.Empty;
			price_for_sell_min = 0;
			price_for_sell_max = 0;
			forbid_change = false;
			weight = 0;
			key_group = string.Empty;
			item_drop_id = 0;
			attach_socket_name = string.Empty;
			puppet_handheld_state = 0;
			use_vending_machine_exchange = false;
			is_preserved_on_wipe = false;
			accessory_group = 0;
			is_hide_my_camera = false;
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
			scrap_animation_state_name = string.Empty;
			is_moving_scrap_animation = false;
			scrap_animation_loop = false;
			use_bonus_item = false;
			sound_aggro_per_use = 0;
			sound_aggro_in_hand_per_tick = 0;
			sound_aggro_in_hand_toggle_on_per_tick = 0;
			hide_item_by_emote = false;
			is_promotion_item = false;
			is_promotion_item_hidden = false;
		}
	}
}
