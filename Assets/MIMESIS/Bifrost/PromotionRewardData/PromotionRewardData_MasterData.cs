using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.PromotionRewardData
{
	public class PromotionRewardData_MasterData : ISchema
	{
		public int id;

		public int itemdefid;

		public int tab_group_id;

		public string ui_list_reward_icon = string.Empty;

		public string reward_name = string.Empty;

		public string mouse_over_tooltip = string.Empty;

		public List<string> add_tram_parts_names = new List<string>();

		public List<string> change_item_prefab = new List<string>();

		public string start_item_id = string.Empty;

		public PromotionRewardData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public PromotionRewardData_MasterData()
			: base(1004504739u, "PromotionRewardData_MasterData")
		{
		}

		public void CopyTo(PromotionRewardData_MasterData dest)
		{
			dest.id = id;
			dest.itemdefid = itemdefid;
			dest.tab_group_id = tab_group_id;
			dest.ui_list_reward_icon = ui_list_reward_icon;
			dest.reward_name = reward_name;
			dest.mouse_over_tooltip = mouse_over_tooltip;
			dest.add_tram_parts_names.Clear();
			foreach (string add_tram_parts_name in add_tram_parts_names)
			{
				dest.add_tram_parts_names.Add(add_tram_parts_name);
			}
			dest.change_item_prefab.Clear();
			foreach (string item in change_item_prefab)
			{
				dest.change_item_prefab.Add(item);
			}
			dest.start_item_id = start_item_id;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += Serializer.GetLength(itemdefid);
			num += Serializer.GetLength(tab_group_id);
			num += Serializer.GetLength(ui_list_reward_icon);
			num += Serializer.GetLength(reward_name);
			num += Serializer.GetLength(mouse_over_tooltip);
			num += 4;
			foreach (string add_tram_parts_name in add_tram_parts_names)
			{
				num += Serializer.GetLength(add_tram_parts_name);
			}
			num += 4;
			foreach (string item in change_item_prefab)
			{
				num += Serializer.GetLength(item);
			}
			return num + Serializer.GetLength(start_item_id);
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
			Serializer.Load(br, ref itemdefid);
			Serializer.Load(br, ref tab_group_id);
			Serializer.Load(br, ref ui_list_reward_icon);
			Serializer.Load(br, ref reward_name);
			Serializer.Load(br, ref mouse_over_tooltip);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				add_tram_parts_names.Add(strValue);
			}
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				change_item_prefab.Add(strValue2);
			}
			Serializer.Load(br, ref start_item_id);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, itemdefid);
			Serializer.Save(bw, tab_group_id);
			Serializer.Save(bw, ui_list_reward_icon);
			Serializer.Save(bw, reward_name);
			Serializer.Save(bw, mouse_over_tooltip);
			Serializer.Save(bw, add_tram_parts_names.Count);
			foreach (string add_tram_parts_name in add_tram_parts_names)
			{
				Serializer.Save(bw, add_tram_parts_name);
			}
			Serializer.Save(bw, change_item_prefab.Count);
			foreach (string item in change_item_prefab)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, start_item_id);
		}

		public bool Equal(PromotionRewardData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (itemdefid != comp.itemdefid)
			{
				return false;
			}
			if (tab_group_id != comp.tab_group_id)
			{
				return false;
			}
			if (ui_list_reward_icon != comp.ui_list_reward_icon)
			{
				return false;
			}
			if (reward_name != comp.reward_name)
			{
				return false;
			}
			if (mouse_over_tooltip != comp.mouse_over_tooltip)
			{
				return false;
			}
			if (comp.add_tram_parts_names.Count != add_tram_parts_names.Count)
			{
				return false;
			}
			foreach (string add_tram_parts_name in add_tram_parts_names)
			{
				if (!comp.add_tram_parts_names.Contains(add_tram_parts_name))
				{
					return false;
				}
			}
			if (comp.change_item_prefab.Count != change_item_prefab.Count)
			{
				return false;
			}
			foreach (string item in change_item_prefab)
			{
				if (!comp.change_item_prefab.Contains(item))
				{
					return false;
				}
			}
			if (start_item_id != comp.start_item_id)
			{
				return false;
			}
			return true;
		}

		public PromotionRewardData_MasterData Clone()
		{
			PromotionRewardData_MasterData promotionRewardData_MasterData = new PromotionRewardData_MasterData();
			CopyTo(promotionRewardData_MasterData);
			return promotionRewardData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			itemdefid = 0;
			tab_group_id = 0;
			ui_list_reward_icon = string.Empty;
			reward_name = string.Empty;
			mouse_over_tooltip = string.Empty;
			add_tram_parts_names.Clear();
			change_item_prefab.Clear();
			start_item_id = string.Empty;
		}
	}
}
