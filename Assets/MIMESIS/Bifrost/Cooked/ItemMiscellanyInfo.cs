using Bifrost.ConstEnum;
using Bifrost.ItemMiscellany;

namespace Bifrost.Cooked
{
	public sealed class ItemMiscellanyInfo : ItemMasterInfo
	{
		public readonly int AccessoryGroup;

		public readonly bool IsHideMyCamera;

		public readonly string ScrapAnimationStateName;

		public readonly bool IsMovingScrapAnimation;

		public readonly bool IsLoopScrapAnimation;

		public ItemMiscellanyInfo(ItemMiscellany_MasterData masterData)
			: base(ItemType.Miscellany, masterData.id, masterData.name, masterData.weight, masterData.looting_object_id, masterData.item_drop_id, masterData.attach_socket_name, masterData.puppet_handheld_state, masterData.price_for_sell_min, masterData.price_for_sell_max, masterData.tool_tip_string, masterData.vending_machine_tooltip_string, masterData.use_vending_machine_exchange, masterData.forbid_change, masterData.is_preserved_on_wipe, visibleGaugeCount: false, visibleDurabilityCount: false, masterData.handheld_auraskill_id, masterData.handheld_abnormal_id, masterData.spawn_fieldskill_id, masterData.spawn_fieldskill_env_condition, masterData.spawn_fieldskill_enclosure_condition, masterData.spawn_fieldskill_time_min, masterData.spawn_fieldskill_time_max, masterData.spawn_fieldskill_wait_time, masterData.spawn_fieldskill_wait_effect, masterData.fieldskill_spawn_rate, masterData.key_group, masterData.use_bonus_item, masterData.sound_aggro_per_use, masterData.sound_aggro_in_hand_per_tick, masterData.sound_aggro_in_hand_toggle_on_per_tick, masterData.hide_item_by_emote, masterData.is_promotion_item, masterData.is_promotion_item_hidden)
		{
			AccessoryGroup = masterData.accessory_group;
			IsHideMyCamera = masterData.is_hide_my_camera;
			ScrapAnimationStateName = masterData.scrap_animation_state_name;
			IsMovingScrapAnimation = masterData.is_moving_scrap_animation;
			IsLoopScrapAnimation = masterData.scrap_animation_loop;
		}
	}
}
