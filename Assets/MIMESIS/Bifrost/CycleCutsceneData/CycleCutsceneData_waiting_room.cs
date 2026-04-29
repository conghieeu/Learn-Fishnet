using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.CycleCutsceneData
{
	public class CycleCutsceneData_waiting_room : ISchema
	{
		public int daycount;

		public string tram_inner_destroyed_set_name = string.Empty;

		public List<string> enter_cutscenes = new List<string>();

		public List<string> enter_event_action = new List<string>();

		public List<string> exit_cutscenes = new List<string>();

		public List<string> exit_event_action = new List<string>();

		public CycleCutsceneData_waiting_room(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public CycleCutsceneData_waiting_room()
			: base(2457122399u, "CycleCutsceneData_waiting_room")
		{
		}

		public void CopyTo(CycleCutsceneData_waiting_room dest)
		{
			dest.daycount = daycount;
			dest.tram_inner_destroyed_set_name = tram_inner_destroyed_set_name;
			dest.enter_cutscenes.Clear();
			foreach (string enter_cutscene in enter_cutscenes)
			{
				dest.enter_cutscenes.Add(enter_cutscene);
			}
			dest.enter_event_action.Clear();
			foreach (string item in enter_event_action)
			{
				dest.enter_event_action.Add(item);
			}
			dest.exit_cutscenes.Clear();
			foreach (string exit_cutscene in exit_cutscenes)
			{
				dest.exit_cutscenes.Add(exit_cutscene);
			}
			dest.exit_event_action.Clear();
			foreach (string item2 in exit_event_action)
			{
				dest.exit_event_action.Add(item2);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(daycount);
			num += Serializer.GetLength(tram_inner_destroyed_set_name);
			num += 4;
			foreach (string enter_cutscene in enter_cutscenes)
			{
				num += Serializer.GetLength(enter_cutscene);
			}
			num += 4;
			foreach (string item in enter_event_action)
			{
				num += Serializer.GetLength(item);
			}
			num += 4;
			foreach (string exit_cutscene in exit_cutscenes)
			{
				num += Serializer.GetLength(exit_cutscene);
			}
			num += 4;
			foreach (string item2 in exit_event_action)
			{
				num += Serializer.GetLength(item2);
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
			Serializer.Load(br, ref daycount);
			Serializer.Load(br, ref tram_inner_destroyed_set_name);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				enter_cutscenes.Add(strValue);
			}
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				enter_event_action.Add(strValue2);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				string strValue3 = string.Empty;
				Serializer.Load(br, ref strValue3);
				exit_cutscenes.Add(strValue3);
			}
			int intValue4 = 0;
			Serializer.Load(br, ref intValue4);
			while (intValue4-- > 0)
			{
				string strValue4 = string.Empty;
				Serializer.Load(br, ref strValue4);
				exit_event_action.Add(strValue4);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, daycount);
			Serializer.Save(bw, tram_inner_destroyed_set_name);
			Serializer.Save(bw, enter_cutscenes.Count);
			foreach (string enter_cutscene in enter_cutscenes)
			{
				Serializer.Save(bw, enter_cutscene);
			}
			Serializer.Save(bw, enter_event_action.Count);
			foreach (string item in enter_event_action)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, exit_cutscenes.Count);
			foreach (string exit_cutscene in exit_cutscenes)
			{
				Serializer.Save(bw, exit_cutscene);
			}
			Serializer.Save(bw, exit_event_action.Count);
			foreach (string item2 in exit_event_action)
			{
				Serializer.Save(bw, item2);
			}
		}

		public bool Equal(CycleCutsceneData_waiting_room comp)
		{
			if (daycount != comp.daycount)
			{
				return false;
			}
			if (tram_inner_destroyed_set_name != comp.tram_inner_destroyed_set_name)
			{
				return false;
			}
			if (comp.enter_cutscenes.Count != enter_cutscenes.Count)
			{
				return false;
			}
			foreach (string enter_cutscene in enter_cutscenes)
			{
				if (!comp.enter_cutscenes.Contains(enter_cutscene))
				{
					return false;
				}
			}
			if (comp.enter_event_action.Count != enter_event_action.Count)
			{
				return false;
			}
			foreach (string item in enter_event_action)
			{
				if (!comp.enter_event_action.Contains(item))
				{
					return false;
				}
			}
			if (comp.exit_cutscenes.Count != exit_cutscenes.Count)
			{
				return false;
			}
			foreach (string exit_cutscene in exit_cutscenes)
			{
				if (!comp.exit_cutscenes.Contains(exit_cutscene))
				{
					return false;
				}
			}
			if (comp.exit_event_action.Count != exit_event_action.Count)
			{
				return false;
			}
			foreach (string item2 in exit_event_action)
			{
				if (!comp.exit_event_action.Contains(item2))
				{
					return false;
				}
			}
			return true;
		}

		public CycleCutsceneData_waiting_room Clone()
		{
			CycleCutsceneData_waiting_room cycleCutsceneData_waiting_room = new CycleCutsceneData_waiting_room();
			CopyTo(cycleCutsceneData_waiting_room);
			return cycleCutsceneData_waiting_room;
		}

		public override void Clean()
		{
			daycount = 0;
			tram_inner_destroyed_set_name = string.Empty;
			enter_cutscenes.Clear();
			enter_event_action.Clear();
			exit_cutscenes.Clear();
			exit_event_action.Clear();
		}
	}
}
