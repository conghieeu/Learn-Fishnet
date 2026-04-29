using System.Collections.Immutable;
using Bifrost.ConstEnum;
using Bifrost.ItemConsumable;

namespace Bifrost.Cooked
{
	public sealed class ItemConsumableInfo : ItemMasterInfo
	{
		public readonly ConsumeItemType ConsumeType;

		public readonly BulletType BulletType;

		public readonly int MaxStackCount;

		public readonly int DefaultProvideCount;

		public readonly int UseEffectId;

		public ImmutableArray<IGameAction> GameActions = ImmutableArray<IGameAction>.Empty;

		public ItemConsumableInfo(ItemConsumable_MasterData masterData)
			: base(ItemType.Consumable, masterData.id, masterData.name, masterData.weight, masterData.looting_object_id, masterData.item_drop_id, masterData.attach_socket_name, masterData.puppet_handheld_state, masterData.price_for_sell_min, masterData.price_for_sell_max, masterData.tool_tip_string, masterData.vending_machine_tooltip_string, masterData.use_vending_machine_exchange, forbidChange: false, masterData.is_preserved_on_wipe, visibleGaugeCount: false, visibleDurabilityCount: false, masterData.handheld_auraskill_id, masterData.handheld_abnormal_id, masterData.spawn_fieldskill_id, masterData.spawn_fieldskill_env_condition, masterData.spawn_fieldskill_enclosure_condition, masterData.spawn_fieldskill_time_min, masterData.spawn_fieldskill_time_max, masterData.spawn_fieldskill_wait_time, masterData.spawn_fieldskill_wait_effect, masterData.fieldskill_spawn_rate, string.Empty, isBoostItemCandidate: false, masterData.sound_aggro_per_use, masterData.sound_aggro_in_hand_per_tick, masterData.sound_aggro_in_hand_toggle_on_per_tick, masterData.hide_item_by_emote, masterData.is_promotion_item, masterData.is_promotion_item_hidden)
		{
			ConsumeType = (ConsumeItemType)masterData.consume_type;
			BulletType = (BulletType)masterData.bullet_type;
			MaxStackCount = masterData.max_stack_count;
			DefaultProvideCount = masterData.default_provide_count;
			UseEffectId = masterData.link_skill_target_effect_data_id;
			if (!CondActionObjParser.GenerateActionGroup(masterData.actions, "", out GameActions))
			{
				Logger.RWarn("DungeonMasterInfo: GenerateActionGroup failed");
			}
		}
	}
}
