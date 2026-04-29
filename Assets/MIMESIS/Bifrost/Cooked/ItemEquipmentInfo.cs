using System;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.ConstEnum;
using Bifrost.ItemEquipment;
using UnityEngine;

namespace Bifrost.Cooked
{
	public sealed class ItemEquipmentInfo : ItemMasterInfo
	{
		public readonly EquipPartsType EquipPartsType;

		public readonly bool IsTwoHanded;

		public readonly bool KeepStateWhenFocusOutInInven;

		public readonly ImmutableArray<SkillPair> SkillList = ImmutableArray<SkillPair>.Empty;

		public readonly int ReloadSkillMasterID;

		public readonly WeaponType WeaponType;

		public readonly int MaxGauge;

		public readonly int PriceIncPerGauge;

		public readonly int IncGaugePerMove;

		public readonly int OverflowPrice;

		public readonly int MinGaugeWhenLooting;

		public readonly int MaxGaugeWhenLooting;

		public readonly int MinDurability;

		public readonly int MaxDurability;

		public readonly int DecGaugePerUse;

		public readonly int DecGaugePerPeriod;

		public readonly int DecGaugeInitial;

		public readonly int DecDurabilityWhenLooting;

		public readonly int InitialGauge;

		public readonly ImmutableArray<(StatType, long)> StatList = ImmutableArray<(StatType, long)>.Empty;

		public readonly Vector3 DirectionWhenDestroyed;

		public ImmutableArray<IGameAction> GameActionsWhenDestroy = ImmutableArray<IGameAction>.Empty;

		public readonly bool UseDestroyByGauge;

		public readonly bool HandheldAuraSkillByGauge;

		public readonly bool HandheldAbnormalByGauge;

		public readonly bool UseCharge;

		public readonly int BlackoutRate;

		public readonly int ScrapRandomPlayCycle;

		public readonly int ScrapRandomPlayRate;

		public readonly int ScrapRandomPlayStartDelay;

		public readonly bool CanUpgrade;

		public readonly int UpgradeItemMasterID;

		public readonly int UpgradeCost;

		public readonly int FunctionGroupID;

		public ItemEquipmentInfo(ItemEquipment_MasterData masterData)
			: base(ItemType.Equipment, masterData.id, masterData.name, masterData.weight, masterData.looting_object_id, masterData.item_drop_id, masterData.attach_socket_name, masterData.puppet_handheld_state, masterData.price_for_sell_min, masterData.price_for_sell_max, masterData.tool_tip_string, masterData.vending_machine_tooltip_string, masterData.use_vending_machine_exchange, masterData.forbid_change, masterData.is_preserved_on_wipe, masterData.visible_gauge_count, masterData.visible_durability_count, masterData.handheld_auraskill_id, masterData.handheld_abnormal_id, masterData.spawn_fieldskill_id, masterData.spawn_fieldskill_env_condition, masterData.spawn_fieldskill_enclosure_condition, masterData.spawn_fieldskill_time_min, masterData.spawn_fieldskill_time_max, masterData.spawn_fieldskill_wait_time, masterData.spawn_fieldskill_wait_effect, masterData.fieldskill_spawn_rate, masterData.key_group, masterData.use_bonus_item, masterData.sound_aggro_per_use, masterData.sound_aggro_in_hand_per_tick, masterData.sound_aggro_in_hand_toggle_on_per_tick, masterData.hide_item_by_emote, masterData.is_promotion_item, masterData.is_promotion_item_hidden)
		{
			EquipPartsType = (EquipPartsType)masterData.equip_type;
			IsTwoHanded = masterData.is_two_hand;
			KeepStateWhenFocusOutInInven = masterData.is_remains_active_in_inven;
			if (EquipPartsType == EquipPartsType.SkillEquip)
			{
				WeaponType = (WeaponType)masterData.hand_weapon_type;
			}
			ImmutableArray<SkillPair>.Builder builder = ImmutableArray.CreateBuilder<SkillPair>();
			for (int i = 0; i < masterData.skill_list.Count; i++)
			{
				int skillMasterIDNoGauge = masterData.skill_list[i];
				int skillMasterIDWithGauge = 0;
				if (i < masterData.skill_gauge_on.Count)
				{
					skillMasterIDWithGauge = masterData.skill_gauge_on[i];
				}
				SkillPair item = new SkillPair(skillMasterIDNoGauge, skillMasterIDWithGauge);
				builder.Add(item);
			}
			if (masterData.skill_reload != 0)
			{
				ReloadSkillMasterID = masterData.skill_reload;
				builder.Add(new SkillPair(ReloadSkillMasterID, 0));
			}
			SkillList = builder.ToImmutable();
			MaxGauge = masterData.max_gauge;
			MinGaugeWhenLooting = masterData.min_gauge_when_looting;
			MaxGaugeWhenLooting = masterData.max_gauge_when_looting;
			MinDurability = masterData.min_durability;
			MaxDurability = masterData.max_durability;
			DecGaugePerUse = masterData.dec_gauge_per_use;
			DecGaugePerPeriod = masterData.dec_gauge_use_period;
			DecGaugeInitial = masterData.dec_gauge_initial_only;
			DecDurabilityWhenLooting = masterData.dec_durability_per_looting;
			InitialGauge = masterData.default_provide_gauge;
			PriceIncPerGauge = masterData.price_inc_per_gauge;
			OverflowPrice = masterData.overflow_price;
			IncGaugePerMove = masterData.inc_gauge_when_move;
			CanUpgrade = masterData.use_item_upgrade;
			UpgradeItemMasterID = masterData.item_upgradedid;
			UpgradeCost = masterData.item_upgrade_cost;
			FunctionGroupID = masterData.equipment_function_group_id;
			ImmutableArray<(StatType, long)>.Builder builder2 = ImmutableArray.CreateBuilder<(StatType, long)>();
			foreach (string item3 in masterData.stat_list)
			{
				if (!string.IsNullOrEmpty(item3))
				{
					string[] array = item3.Split(',');
					if (Enum.TryParse<StatType>(array[0], out var result))
					{
						long item2 = Convert.ToInt64(array[1]);
						builder2.Add((result, item2));
					}
					else
					{
						Logger.RError("Failed to parse StatType: " + array[0]);
					}
				}
			}
			StatList = builder2.ToImmutable();
			DirectionWhenDestroyed = new Vector3(masterData.item_slice_external_force[0], masterData.item_slice_external_force[1], masterData.item_slice_external_force[2]);
			if (!CondActionObjParser.GenerateActionGroup(masterData.action_when_destroy, "", out GameActionsWhenDestroy))
			{
				Logger.RError($"ItemEquipmentInfo: action_when_destroy GenerateActionGroup failed. ID:{masterData.id}");
			}
			UseDestroyByGauge = masterData.use_destroy_by_gauge;
			HandheldAuraSkillByGauge = masterData.handheld_auraskill_by_gauge;
			HandheldAbnormalByGauge = masterData.handheld_abnormal_by_gauge;
			if (SkillList.Count() > 0)
			{
				if (HandheldAuraSkillID != 0 && HandheldAuraSkillByGauge)
				{
					Logger.RError($"ItemEquipmentInfo: ID:{MasterID}, SkillList.Count:{SkillList.Count()}, HandheldAuraSkillID:{HandheldAuraSkillID}");
				}
				if (HandheldAbnormalID != 0 && HandheldAbnormalByGauge)
				{
					Logger.RError($"ItemEquipmentInfo: ID:{MasterID}, SkillList.Count:{SkillList.Count()}, HandheldAbnormalID:{HandheldAbnormalID}");
				}
			}
			UseCharge = masterData.use_charge;
			BlackoutRate = masterData.blackout_rate;
			ScrapRandomPlayCycle = masterData.scrap_random_play_cycle;
			ScrapRandomPlayRate = masterData.scrap_random_play_rate;
			ScrapRandomPlayStartDelay = masterData.scrap_random_play_start_delay;
		}

		public bool HasSkill(int skillMasterID)
		{
			ImmutableArray<SkillPair>.Enumerator enumerator = SkillList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SkillPair current = enumerator.Current;
				if (current.SkillMasterIDNoGague == skillMasterID || current.SkillMasterIDWithGauge == skillMasterID)
				{
					return true;
				}
			}
			return false;
		}
	}
}
