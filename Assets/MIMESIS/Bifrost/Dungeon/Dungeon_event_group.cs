using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.Dungeon
{
	public class Dungeon_event_group : ISchema
	{
		public string event_time = string.Empty;

		public List<string> event_actions = new List<string>();

		public Dungeon_event_group(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Dungeon_event_group()
			: base(851558408u, "Dungeon_event_group")
		{
		}

		public void CopyTo(Dungeon_event_group dest)
		{
			dest.event_time = event_time;
			dest.event_actions.Clear();
			foreach (string event_action in event_actions)
			{
				dest.event_actions.Add(event_action);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(event_time);
			num += 4;
			foreach (string event_action in event_actions)
			{
				num += Serializer.GetLength(event_action);
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
			Serializer.Load(br, ref event_time);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				event_actions.Add(strValue);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, event_time);
			Serializer.Save(bw, event_actions.Count);
			foreach (string event_action in event_actions)
			{
				Serializer.Save(bw, event_action);
			}
		}

		public bool Equal(Dungeon_event_group comp)
		{
			if (event_time != comp.event_time)
			{
				return false;
			}
			if (comp.event_actions.Count != event_actions.Count)
			{
				return false;
			}
			foreach (string event_action in event_actions)
			{
				if (!comp.event_actions.Contains(event_action))
				{
					return false;
				}
			}
			return true;
		}

		public Dungeon_event_group Clone()
		{
			Dungeon_event_group dungeon_event_group = new Dungeon_event_group();
			CopyTo(dungeon_event_group);
			return dungeon_event_group;
		}

		public override void Clean()
		{
			event_time = string.Empty;
			event_actions.Clear();
		}
	}
}
