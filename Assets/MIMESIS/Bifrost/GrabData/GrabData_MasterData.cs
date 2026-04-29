using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.GrabData
{
	public class GrabData_MasterData : ISchema
	{
		public int id;

		public bool is_reverse_grab;

		public int socket_count;

		public List<GrabData_socket_info> GrabData_socket_infoval = new List<GrabData_socket_info>();

		public long grab_attach_duration;

		public long grab_detatch_duration;

		public long duration;

		public int caster_starttime_abnormal_id;

		public int caster_endtime_abnormal_id;

		public int victim_starttime_abnormal_id;

		public int victim_endtime_abnormal_id;

		public int caster_force_detach_abnormal_id;

		public int victim_force_detach_abnormal_id;

		public List<string> caster_ongrab_immune = new List<string>();

		public List<string> victim_ongrab_immune = new List<string>();

		public string ungrab_animation_state_name = string.Empty;

		public bool can_use_skill;

		public bool is_victim_ungrap_direction_same_caster;

		public long victim_ungrap_distance;

		public GrabData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public GrabData_MasterData()
			: base(877478596u, "GrabData_MasterData")
		{
		}

		public void CopyTo(GrabData_MasterData dest)
		{
			dest.id = id;
			dest.is_reverse_grab = is_reverse_grab;
			dest.socket_count = socket_count;
			dest.GrabData_socket_infoval.Clear();
			foreach (GrabData_socket_info item in GrabData_socket_infoval)
			{
				GrabData_socket_info grabData_socket_info = new GrabData_socket_info();
				item.CopyTo(grabData_socket_info);
				dest.GrabData_socket_infoval.Add(grabData_socket_info);
			}
			dest.grab_attach_duration = grab_attach_duration;
			dest.grab_detatch_duration = grab_detatch_duration;
			dest.duration = duration;
			dest.caster_starttime_abnormal_id = caster_starttime_abnormal_id;
			dest.caster_endtime_abnormal_id = caster_endtime_abnormal_id;
			dest.victim_starttime_abnormal_id = victim_starttime_abnormal_id;
			dest.victim_endtime_abnormal_id = victim_endtime_abnormal_id;
			dest.caster_force_detach_abnormal_id = caster_force_detach_abnormal_id;
			dest.victim_force_detach_abnormal_id = victim_force_detach_abnormal_id;
			dest.caster_ongrab_immune.Clear();
			foreach (string item2 in caster_ongrab_immune)
			{
				dest.caster_ongrab_immune.Add(item2);
			}
			dest.victim_ongrab_immune.Clear();
			foreach (string item3 in victim_ongrab_immune)
			{
				dest.victim_ongrab_immune.Add(item3);
			}
			dest.ungrab_animation_state_name = ungrab_animation_state_name;
			dest.can_use_skill = can_use_skill;
			dest.is_victim_ungrap_direction_same_caster = is_victim_ungrap_direction_same_caster;
			dest.victim_ungrap_distance = victim_ungrap_distance;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += Serializer.GetLength(is_reverse_grab);
			num += Serializer.GetLength(socket_count);
			num += 4;
			foreach (GrabData_socket_info item in GrabData_socket_infoval)
			{
				num += item.GetLengthInternal();
			}
			num += Serializer.GetLength(grab_attach_duration);
			num += Serializer.GetLength(grab_detatch_duration);
			num += Serializer.GetLength(duration);
			num += Serializer.GetLength(caster_starttime_abnormal_id);
			num += Serializer.GetLength(caster_endtime_abnormal_id);
			num += Serializer.GetLength(victim_starttime_abnormal_id);
			num += Serializer.GetLength(victim_endtime_abnormal_id);
			num += Serializer.GetLength(caster_force_detach_abnormal_id);
			num += Serializer.GetLength(victim_force_detach_abnormal_id);
			num += 4;
			foreach (string item2 in caster_ongrab_immune)
			{
				num += Serializer.GetLength(item2);
			}
			num += 4;
			foreach (string item3 in victim_ongrab_immune)
			{
				num += Serializer.GetLength(item3);
			}
			num += Serializer.GetLength(ungrab_animation_state_name);
			num += Serializer.GetLength(can_use_skill);
			num += Serializer.GetLength(is_victim_ungrap_direction_same_caster);
			return num + Serializer.GetLength(victim_ungrap_distance);
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
			Serializer.Load(br, ref is_reverse_grab);
			Serializer.Load(br, ref socket_count);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				GrabData_socket_info grabData_socket_info = new GrabData_socket_info();
				grabData_socket_info.LoadInternal(br);
				GrabData_socket_infoval.Add(grabData_socket_info);
			}
			Serializer.Load(br, ref grab_attach_duration);
			Serializer.Load(br, ref grab_detatch_duration);
			Serializer.Load(br, ref duration);
			Serializer.Load(br, ref caster_starttime_abnormal_id);
			Serializer.Load(br, ref caster_endtime_abnormal_id);
			Serializer.Load(br, ref victim_starttime_abnormal_id);
			Serializer.Load(br, ref victim_endtime_abnormal_id);
			Serializer.Load(br, ref caster_force_detach_abnormal_id);
			Serializer.Load(br, ref victim_force_detach_abnormal_id);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				caster_ongrab_immune.Add(strValue);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				victim_ongrab_immune.Add(strValue2);
			}
			Serializer.Load(br, ref ungrab_animation_state_name);
			Serializer.Load(br, ref can_use_skill);
			Serializer.Load(br, ref is_victim_ungrap_direction_same_caster);
			Serializer.Load(br, ref victim_ungrap_distance);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, is_reverse_grab);
			Serializer.Save(bw, socket_count);
			Serializer.Save(bw, GrabData_socket_infoval.Count);
			foreach (GrabData_socket_info item in GrabData_socket_infoval)
			{
				item.SaveInternal(bw);
			}
			Serializer.Save(bw, grab_attach_duration);
			Serializer.Save(bw, grab_detatch_duration);
			Serializer.Save(bw, duration);
			Serializer.Save(bw, caster_starttime_abnormal_id);
			Serializer.Save(bw, caster_endtime_abnormal_id);
			Serializer.Save(bw, victim_starttime_abnormal_id);
			Serializer.Save(bw, victim_endtime_abnormal_id);
			Serializer.Save(bw, caster_force_detach_abnormal_id);
			Serializer.Save(bw, victim_force_detach_abnormal_id);
			Serializer.Save(bw, caster_ongrab_immune.Count);
			foreach (string item2 in caster_ongrab_immune)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, victim_ongrab_immune.Count);
			foreach (string item3 in victim_ongrab_immune)
			{
				Serializer.Save(bw, item3);
			}
			Serializer.Save(bw, ungrab_animation_state_name);
			Serializer.Save(bw, can_use_skill);
			Serializer.Save(bw, is_victim_ungrap_direction_same_caster);
			Serializer.Save(bw, victim_ungrap_distance);
		}

		public bool Equal(GrabData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (is_reverse_grab != comp.is_reverse_grab)
			{
				return false;
			}
			if (socket_count != comp.socket_count)
			{
				return false;
			}
			if (comp.GrabData_socket_infoval.Count != GrabData_socket_infoval.Count)
			{
				return false;
			}
			foreach (GrabData_socket_info item in GrabData_socket_infoval)
			{
				bool flag = false;
				foreach (GrabData_socket_info item2 in comp.GrabData_socket_infoval)
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
			if (grab_attach_duration != comp.grab_attach_duration)
			{
				return false;
			}
			if (grab_detatch_duration != comp.grab_detatch_duration)
			{
				return false;
			}
			if (duration != comp.duration)
			{
				return false;
			}
			if (caster_starttime_abnormal_id != comp.caster_starttime_abnormal_id)
			{
				return false;
			}
			if (caster_endtime_abnormal_id != comp.caster_endtime_abnormal_id)
			{
				return false;
			}
			if (victim_starttime_abnormal_id != comp.victim_starttime_abnormal_id)
			{
				return false;
			}
			if (victim_endtime_abnormal_id != comp.victim_endtime_abnormal_id)
			{
				return false;
			}
			if (caster_force_detach_abnormal_id != comp.caster_force_detach_abnormal_id)
			{
				return false;
			}
			if (victim_force_detach_abnormal_id != comp.victim_force_detach_abnormal_id)
			{
				return false;
			}
			if (comp.caster_ongrab_immune.Count != caster_ongrab_immune.Count)
			{
				return false;
			}
			foreach (string item3 in caster_ongrab_immune)
			{
				if (!comp.caster_ongrab_immune.Contains(item3))
				{
					return false;
				}
			}
			if (comp.victim_ongrab_immune.Count != victim_ongrab_immune.Count)
			{
				return false;
			}
			foreach (string item4 in victim_ongrab_immune)
			{
				if (!comp.victim_ongrab_immune.Contains(item4))
				{
					return false;
				}
			}
			if (ungrab_animation_state_name != comp.ungrab_animation_state_name)
			{
				return false;
			}
			if (can_use_skill != comp.can_use_skill)
			{
				return false;
			}
			if (is_victim_ungrap_direction_same_caster != comp.is_victim_ungrap_direction_same_caster)
			{
				return false;
			}
			if (victim_ungrap_distance != comp.victim_ungrap_distance)
			{
				return false;
			}
			return true;
		}

		public GrabData_MasterData Clone()
		{
			GrabData_MasterData grabData_MasterData = new GrabData_MasterData();
			CopyTo(grabData_MasterData);
			return grabData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			is_reverse_grab = false;
			socket_count = 0;
			GrabData_socket_infoval.Clear();
			grab_attach_duration = 0L;
			grab_detatch_duration = 0L;
			duration = 0L;
			caster_starttime_abnormal_id = 0;
			caster_endtime_abnormal_id = 0;
			victim_starttime_abnormal_id = 0;
			victim_endtime_abnormal_id = 0;
			caster_force_detach_abnormal_id = 0;
			victim_force_detach_abnormal_id = 0;
			caster_ongrab_immune.Clear();
			victim_ongrab_immune.Clear();
			ungrab_animation_state_name = string.Empty;
			can_use_skill = false;
			is_victim_ungrap_direction_same_caster = false;
			victim_ungrap_distance = 0L;
		}
	}
}
