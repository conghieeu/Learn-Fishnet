using System.Collections.Generic;
using Bifrost.PromotionRewardData;

namespace Bifrost.Cooked
{
	public class PromotionRewardInfo
	{
		public readonly int MasterID;

		public readonly int ItemDefID;

		public readonly int TabGroupId;

		public readonly string UIIconName;

		public readonly string MouseOverTooltip;

		public readonly List<string> AddTramPartsNames;

		public readonly (int itemMasterId, string skinName)? ChangeItemPrefab;

		public readonly int? StartItemId;

		public readonly string? RewardName;

		public PromotionRewardInfo(PromotionRewardData_MasterData masterData)
		{
			MasterID = masterData.id;
			ItemDefID = masterData.itemdefid;
			TabGroupId = masterData.tab_group_id;
			UIIconName = masterData.ui_list_reward_icon;
			MouseOverTooltip = masterData.mouse_over_tooltip;
			if (masterData.add_tram_parts_names.Count > 0)
			{
				AddTramPartsNames = masterData.add_tram_parts_names;
			}
			if (masterData.change_item_prefab.Count > 0)
			{
				ChangeItemPrefab = (int.Parse(masterData.change_item_prefab[0]), masterData.change_item_prefab[1]);
			}
			if (masterData.start_item_id.Length > 0)
			{
				StartItemId = int.Parse(masterData.start_item_id);
			}
			if (masterData.reward_name.Length > 0)
			{
				RewardName = masterData.reward_name;
			}
		}

		public bool IsTramSkin()
		{
			return TabGroupId == 1;
		}

		public bool IsItemSkin()
		{
			return TabGroupId == 2;
		}

		public bool IsStartItem()
		{
			return TabGroupId == 3;
		}
	}
}
