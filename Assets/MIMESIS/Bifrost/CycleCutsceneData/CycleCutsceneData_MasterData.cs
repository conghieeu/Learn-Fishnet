using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.CycleCutsceneData
{
	public class CycleCutsceneData_MasterData : ISchema
	{
		public int id;

		public int cycle;

		public int cycle_quota;

		public int shop_group;

		public List<string> tram_add_parts_name = new List<string>();

		public string tram_inner_destroyed_set_name = string.Empty;

		public List<string> maintenance_enter_cutscene_list = new List<string>();

		public List<string> maintenance_enter_event_action = new List<string>();

		public List<string> maintenance_exit_cutscene_list = new List<string>();

		public List<string> maintenance_exit_event_action = new List<string>();

		public List<string> maintenance_repair_event_action = new List<string>();

		public List<string> deathmatch_enter_cutscene_list = new List<string>();

		public List<string> deathmatch_enter_event_action = new List<string>();

		public List<string> deathmatch_monster_all_death_event_action = new List<string>();

		public List<string> deathmatch_end_cutscene_list = new List<string>();

		public List<string> deathmatch_end_event_action = new List<string>();

		public long deathmatch_conta;

		public List<CycleCutsceneData_waiting_room> CycleCutsceneData_waiting_roomval = new List<CycleCutsceneData_waiting_room>();

		public List<CycleCutsceneData_dungeon> CycleCutsceneData_dungeonval = new List<CycleCutsceneData_dungeon>();

		public CycleCutsceneData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public CycleCutsceneData_MasterData()
			: base(1111047436u, "CycleCutsceneData_MasterData")
		{
		}

		public void CopyTo(CycleCutsceneData_MasterData dest)
		{
			dest.id = id;
			dest.cycle = cycle;
			dest.cycle_quota = cycle_quota;
			dest.shop_group = shop_group;
			dest.tram_add_parts_name.Clear();
			foreach (string item in tram_add_parts_name)
			{
				dest.tram_add_parts_name.Add(item);
			}
			dest.tram_inner_destroyed_set_name = tram_inner_destroyed_set_name;
			dest.maintenance_enter_cutscene_list.Clear();
			foreach (string item2 in maintenance_enter_cutscene_list)
			{
				dest.maintenance_enter_cutscene_list.Add(item2);
			}
			dest.maintenance_enter_event_action.Clear();
			foreach (string item3 in maintenance_enter_event_action)
			{
				dest.maintenance_enter_event_action.Add(item3);
			}
			dest.maintenance_exit_cutscene_list.Clear();
			foreach (string item4 in maintenance_exit_cutscene_list)
			{
				dest.maintenance_exit_cutscene_list.Add(item4);
			}
			dest.maintenance_exit_event_action.Clear();
			foreach (string item5 in maintenance_exit_event_action)
			{
				dest.maintenance_exit_event_action.Add(item5);
			}
			dest.maintenance_repair_event_action.Clear();
			foreach (string item6 in maintenance_repair_event_action)
			{
				dest.maintenance_repair_event_action.Add(item6);
			}
			dest.deathmatch_enter_cutscene_list.Clear();
			foreach (string item7 in deathmatch_enter_cutscene_list)
			{
				dest.deathmatch_enter_cutscene_list.Add(item7);
			}
			dest.deathmatch_enter_event_action.Clear();
			foreach (string item8 in deathmatch_enter_event_action)
			{
				dest.deathmatch_enter_event_action.Add(item8);
			}
			dest.deathmatch_monster_all_death_event_action.Clear();
			foreach (string item9 in deathmatch_monster_all_death_event_action)
			{
				dest.deathmatch_monster_all_death_event_action.Add(item9);
			}
			dest.deathmatch_end_cutscene_list.Clear();
			foreach (string item10 in deathmatch_end_cutscene_list)
			{
				dest.deathmatch_end_cutscene_list.Add(item10);
			}
			dest.deathmatch_end_event_action.Clear();
			foreach (string item11 in deathmatch_end_event_action)
			{
				dest.deathmatch_end_event_action.Add(item11);
			}
			dest.deathmatch_conta = deathmatch_conta;
			dest.CycleCutsceneData_waiting_roomval.Clear();
			foreach (CycleCutsceneData_waiting_room item12 in CycleCutsceneData_waiting_roomval)
			{
				CycleCutsceneData_waiting_room cycleCutsceneData_waiting_room = new CycleCutsceneData_waiting_room();
				item12.CopyTo(cycleCutsceneData_waiting_room);
				dest.CycleCutsceneData_waiting_roomval.Add(cycleCutsceneData_waiting_room);
			}
			dest.CycleCutsceneData_dungeonval.Clear();
			foreach (CycleCutsceneData_dungeon item13 in CycleCutsceneData_dungeonval)
			{
				CycleCutsceneData_dungeon cycleCutsceneData_dungeon = new CycleCutsceneData_dungeon();
				item13.CopyTo(cycleCutsceneData_dungeon);
				dest.CycleCutsceneData_dungeonval.Add(cycleCutsceneData_dungeon);
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
			num += Serializer.GetLength(cycle);
			num += Serializer.GetLength(cycle_quota);
			num += Serializer.GetLength(shop_group);
			num += 4;
			foreach (string item in tram_add_parts_name)
			{
				num += Serializer.GetLength(item);
			}
			num += Serializer.GetLength(tram_inner_destroyed_set_name);
			num += 4;
			foreach (string item2 in maintenance_enter_cutscene_list)
			{
				num += Serializer.GetLength(item2);
			}
			num += 4;
			foreach (string item3 in maintenance_enter_event_action)
			{
				num += Serializer.GetLength(item3);
			}
			num += 4;
			foreach (string item4 in maintenance_exit_cutscene_list)
			{
				num += Serializer.GetLength(item4);
			}
			num += 4;
			foreach (string item5 in maintenance_exit_event_action)
			{
				num += Serializer.GetLength(item5);
			}
			num += 4;
			foreach (string item6 in maintenance_repair_event_action)
			{
				num += Serializer.GetLength(item6);
			}
			num += 4;
			foreach (string item7 in deathmatch_enter_cutscene_list)
			{
				num += Serializer.GetLength(item7);
			}
			num += 4;
			foreach (string item8 in deathmatch_enter_event_action)
			{
				num += Serializer.GetLength(item8);
			}
			num += 4;
			foreach (string item9 in deathmatch_monster_all_death_event_action)
			{
				num += Serializer.GetLength(item9);
			}
			num += 4;
			foreach (string item10 in deathmatch_end_cutscene_list)
			{
				num += Serializer.GetLength(item10);
			}
			num += 4;
			foreach (string item11 in deathmatch_end_event_action)
			{
				num += Serializer.GetLength(item11);
			}
			num += Serializer.GetLength(deathmatch_conta);
			num += 4;
			foreach (CycleCutsceneData_waiting_room item12 in CycleCutsceneData_waiting_roomval)
			{
				num += item12.GetLengthInternal();
			}
			num += 4;
			foreach (CycleCutsceneData_dungeon item13 in CycleCutsceneData_dungeonval)
			{
				num += item13.GetLengthInternal();
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
			Serializer.Load(br, ref cycle);
			Serializer.Load(br, ref cycle_quota);
			Serializer.Load(br, ref shop_group);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				tram_add_parts_name.Add(strValue);
			}
			Serializer.Load(br, ref tram_inner_destroyed_set_name);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				maintenance_enter_cutscene_list.Add(strValue2);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				string strValue3 = string.Empty;
				Serializer.Load(br, ref strValue3);
				maintenance_enter_event_action.Add(strValue3);
			}
			int intValue4 = 0;
			Serializer.Load(br, ref intValue4);
			while (intValue4-- > 0)
			{
				string strValue4 = string.Empty;
				Serializer.Load(br, ref strValue4);
				maintenance_exit_cutscene_list.Add(strValue4);
			}
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				string strValue5 = string.Empty;
				Serializer.Load(br, ref strValue5);
				maintenance_exit_event_action.Add(strValue5);
			}
			int intValue6 = 0;
			Serializer.Load(br, ref intValue6);
			while (intValue6-- > 0)
			{
				string strValue6 = string.Empty;
				Serializer.Load(br, ref strValue6);
				maintenance_repair_event_action.Add(strValue6);
			}
			int intValue7 = 0;
			Serializer.Load(br, ref intValue7);
			while (intValue7-- > 0)
			{
				string strValue7 = string.Empty;
				Serializer.Load(br, ref strValue7);
				deathmatch_enter_cutscene_list.Add(strValue7);
			}
			int intValue8 = 0;
			Serializer.Load(br, ref intValue8);
			while (intValue8-- > 0)
			{
				string strValue8 = string.Empty;
				Serializer.Load(br, ref strValue8);
				deathmatch_enter_event_action.Add(strValue8);
			}
			int intValue9 = 0;
			Serializer.Load(br, ref intValue9);
			while (intValue9-- > 0)
			{
				string strValue9 = string.Empty;
				Serializer.Load(br, ref strValue9);
				deathmatch_monster_all_death_event_action.Add(strValue9);
			}
			int intValue10 = 0;
			Serializer.Load(br, ref intValue10);
			while (intValue10-- > 0)
			{
				string strValue10 = string.Empty;
				Serializer.Load(br, ref strValue10);
				deathmatch_end_cutscene_list.Add(strValue10);
			}
			int intValue11 = 0;
			Serializer.Load(br, ref intValue11);
			while (intValue11-- > 0)
			{
				string strValue11 = string.Empty;
				Serializer.Load(br, ref strValue11);
				deathmatch_end_event_action.Add(strValue11);
			}
			Serializer.Load(br, ref deathmatch_conta);
			int intValue12 = 0;
			Serializer.Load(br, ref intValue12);
			while (intValue12-- > 0)
			{
				CycleCutsceneData_waiting_room cycleCutsceneData_waiting_room = new CycleCutsceneData_waiting_room();
				cycleCutsceneData_waiting_room.LoadInternal(br);
				CycleCutsceneData_waiting_roomval.Add(cycleCutsceneData_waiting_room);
			}
			int intValue13 = 0;
			Serializer.Load(br, ref intValue13);
			while (intValue13-- > 0)
			{
				CycleCutsceneData_dungeon cycleCutsceneData_dungeon = new CycleCutsceneData_dungeon();
				cycleCutsceneData_dungeon.LoadInternal(br);
				CycleCutsceneData_dungeonval.Add(cycleCutsceneData_dungeon);
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
			Serializer.Save(bw, cycle);
			Serializer.Save(bw, cycle_quota);
			Serializer.Save(bw, shop_group);
			Serializer.Save(bw, tram_add_parts_name.Count);
			foreach (string item in tram_add_parts_name)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, tram_inner_destroyed_set_name);
			Serializer.Save(bw, maintenance_enter_cutscene_list.Count);
			foreach (string item2 in maintenance_enter_cutscene_list)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, maintenance_enter_event_action.Count);
			foreach (string item3 in maintenance_enter_event_action)
			{
				Serializer.Save(bw, item3);
			}
			Serializer.Save(bw, maintenance_exit_cutscene_list.Count);
			foreach (string item4 in maintenance_exit_cutscene_list)
			{
				Serializer.Save(bw, item4);
			}
			Serializer.Save(bw, maintenance_exit_event_action.Count);
			foreach (string item5 in maintenance_exit_event_action)
			{
				Serializer.Save(bw, item5);
			}
			Serializer.Save(bw, maintenance_repair_event_action.Count);
			foreach (string item6 in maintenance_repair_event_action)
			{
				Serializer.Save(bw, item6);
			}
			Serializer.Save(bw, deathmatch_enter_cutscene_list.Count);
			foreach (string item7 in deathmatch_enter_cutscene_list)
			{
				Serializer.Save(bw, item7);
			}
			Serializer.Save(bw, deathmatch_enter_event_action.Count);
			foreach (string item8 in deathmatch_enter_event_action)
			{
				Serializer.Save(bw, item8);
			}
			Serializer.Save(bw, deathmatch_monster_all_death_event_action.Count);
			foreach (string item9 in deathmatch_monster_all_death_event_action)
			{
				Serializer.Save(bw, item9);
			}
			Serializer.Save(bw, deathmatch_end_cutscene_list.Count);
			foreach (string item10 in deathmatch_end_cutscene_list)
			{
				Serializer.Save(bw, item10);
			}
			Serializer.Save(bw, deathmatch_end_event_action.Count);
			foreach (string item11 in deathmatch_end_event_action)
			{
				Serializer.Save(bw, item11);
			}
			Serializer.Save(bw, deathmatch_conta);
			Serializer.Save(bw, CycleCutsceneData_waiting_roomval.Count);
			foreach (CycleCutsceneData_waiting_room item12 in CycleCutsceneData_waiting_roomval)
			{
				item12.SaveInternal(bw);
			}
			Serializer.Save(bw, CycleCutsceneData_dungeonval.Count);
			foreach (CycleCutsceneData_dungeon item13 in CycleCutsceneData_dungeonval)
			{
				item13.SaveInternal(bw);
			}
		}

		public bool Equal(CycleCutsceneData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (cycle != comp.cycle)
			{
				return false;
			}
			if (cycle_quota != comp.cycle_quota)
			{
				return false;
			}
			if (shop_group != comp.shop_group)
			{
				return false;
			}
			if (comp.tram_add_parts_name.Count != tram_add_parts_name.Count)
			{
				return false;
			}
			foreach (string item in tram_add_parts_name)
			{
				if (!comp.tram_add_parts_name.Contains(item))
				{
					return false;
				}
			}
			if (tram_inner_destroyed_set_name != comp.tram_inner_destroyed_set_name)
			{
				return false;
			}
			if (comp.maintenance_enter_cutscene_list.Count != maintenance_enter_cutscene_list.Count)
			{
				return false;
			}
			foreach (string item2 in maintenance_enter_cutscene_list)
			{
				if (!comp.maintenance_enter_cutscene_list.Contains(item2))
				{
					return false;
				}
			}
			if (comp.maintenance_enter_event_action.Count != maintenance_enter_event_action.Count)
			{
				return false;
			}
			foreach (string item3 in maintenance_enter_event_action)
			{
				if (!comp.maintenance_enter_event_action.Contains(item3))
				{
					return false;
				}
			}
			if (comp.maintenance_exit_cutscene_list.Count != maintenance_exit_cutscene_list.Count)
			{
				return false;
			}
			foreach (string item4 in maintenance_exit_cutscene_list)
			{
				if (!comp.maintenance_exit_cutscene_list.Contains(item4))
				{
					return false;
				}
			}
			if (comp.maintenance_exit_event_action.Count != maintenance_exit_event_action.Count)
			{
				return false;
			}
			foreach (string item5 in maintenance_exit_event_action)
			{
				if (!comp.maintenance_exit_event_action.Contains(item5))
				{
					return false;
				}
			}
			if (comp.maintenance_repair_event_action.Count != maintenance_repair_event_action.Count)
			{
				return false;
			}
			foreach (string item6 in maintenance_repair_event_action)
			{
				if (!comp.maintenance_repair_event_action.Contains(item6))
				{
					return false;
				}
			}
			if (comp.deathmatch_enter_cutscene_list.Count != deathmatch_enter_cutscene_list.Count)
			{
				return false;
			}
			foreach (string item7 in deathmatch_enter_cutscene_list)
			{
				if (!comp.deathmatch_enter_cutscene_list.Contains(item7))
				{
					return false;
				}
			}
			if (comp.deathmatch_enter_event_action.Count != deathmatch_enter_event_action.Count)
			{
				return false;
			}
			foreach (string item8 in deathmatch_enter_event_action)
			{
				if (!comp.deathmatch_enter_event_action.Contains(item8))
				{
					return false;
				}
			}
			if (comp.deathmatch_monster_all_death_event_action.Count != deathmatch_monster_all_death_event_action.Count)
			{
				return false;
			}
			foreach (string item9 in deathmatch_monster_all_death_event_action)
			{
				if (!comp.deathmatch_monster_all_death_event_action.Contains(item9))
				{
					return false;
				}
			}
			if (comp.deathmatch_end_cutscene_list.Count != deathmatch_end_cutscene_list.Count)
			{
				return false;
			}
			foreach (string item10 in deathmatch_end_cutscene_list)
			{
				if (!comp.deathmatch_end_cutscene_list.Contains(item10))
				{
					return false;
				}
			}
			if (comp.deathmatch_end_event_action.Count != deathmatch_end_event_action.Count)
			{
				return false;
			}
			foreach (string item11 in deathmatch_end_event_action)
			{
				if (!comp.deathmatch_end_event_action.Contains(item11))
				{
					return false;
				}
			}
			if (deathmatch_conta != comp.deathmatch_conta)
			{
				return false;
			}
			if (comp.CycleCutsceneData_waiting_roomval.Count != CycleCutsceneData_waiting_roomval.Count)
			{
				return false;
			}
			foreach (CycleCutsceneData_waiting_room item12 in CycleCutsceneData_waiting_roomval)
			{
				bool flag = false;
				foreach (CycleCutsceneData_waiting_room item13 in comp.CycleCutsceneData_waiting_roomval)
				{
					if (item12.Equal(item13))
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
			if (comp.CycleCutsceneData_dungeonval.Count != CycleCutsceneData_dungeonval.Count)
			{
				return false;
			}
			foreach (CycleCutsceneData_dungeon item14 in CycleCutsceneData_dungeonval)
			{
				bool flag2 = false;
				foreach (CycleCutsceneData_dungeon item15 in comp.CycleCutsceneData_dungeonval)
				{
					if (item14.Equal(item15))
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
			return true;
		}

		public CycleCutsceneData_MasterData Clone()
		{
			CycleCutsceneData_MasterData cycleCutsceneData_MasterData = new CycleCutsceneData_MasterData();
			CopyTo(cycleCutsceneData_MasterData);
			return cycleCutsceneData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			cycle = 0;
			cycle_quota = 0;
			shop_group = 0;
			tram_add_parts_name.Clear();
			tram_inner_destroyed_set_name = string.Empty;
			maintenance_enter_cutscene_list.Clear();
			maintenance_enter_event_action.Clear();
			maintenance_exit_cutscene_list.Clear();
			maintenance_exit_event_action.Clear();
			maintenance_repair_event_action.Clear();
			deathmatch_enter_cutscene_list.Clear();
			deathmatch_enter_event_action.Clear();
			deathmatch_monster_all_death_event_action.Clear();
			deathmatch_end_cutscene_list.Clear();
			deathmatch_end_event_action.Clear();
			deathmatch_conta = 0L;
			CycleCutsceneData_waiting_roomval.Clear();
			CycleCutsceneData_dungeonval.Clear();
		}
	}
}
