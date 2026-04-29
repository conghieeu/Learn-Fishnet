using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.Faction
{
	public class Faction_MasterData : ISchema
	{
		public int id;

		public int group;

		public List<int> ally = new List<int>();

		public List<int> neutral = new List<int>();

		public List<int> enemy = new List<int>();

		public Faction_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Faction_MasterData()
			: base(2998136386u, "Faction_MasterData")
		{
		}

		public void CopyTo(Faction_MasterData dest)
		{
			dest.id = id;
			dest.group = group;
			dest.ally.Clear();
			foreach (int item in ally)
			{
				dest.ally.Add(item);
			}
			dest.neutral.Clear();
			foreach (int item2 in neutral)
			{
				dest.neutral.Add(item2);
			}
			dest.enemy.Clear();
			foreach (int item3 in enemy)
			{
				dest.enemy.Add(item3);
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
			num += Serializer.GetLength(group);
			num += 4;
			foreach (int item in ally)
			{
				num += Serializer.GetLength(item);
			}
			num += 4;
			foreach (int item2 in neutral)
			{
				num += Serializer.GetLength(item2);
			}
			num += 4;
			foreach (int item3 in enemy)
			{
				num += Serializer.GetLength(item3);
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
			Serializer.Load(br, ref group);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				int intValue2 = 0;
				Serializer.Load(br, ref intValue2);
				ally.Add(intValue2);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				int intValue4 = 0;
				Serializer.Load(br, ref intValue4);
				neutral.Add(intValue4);
			}
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				int intValue6 = 0;
				Serializer.Load(br, ref intValue6);
				enemy.Add(intValue6);
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
			Serializer.Save(bw, group);
			Serializer.Save(bw, ally.Count);
			foreach (int item in ally)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, neutral.Count);
			foreach (int item2 in neutral)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, enemy.Count);
			foreach (int item3 in enemy)
			{
				Serializer.Save(bw, item3);
			}
		}

		public bool Equal(Faction_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (group != comp.group)
			{
				return false;
			}
			if (comp.ally.Count != ally.Count)
			{
				return false;
			}
			foreach (int item in ally)
			{
				if (!comp.ally.Contains(item))
				{
					return false;
				}
			}
			if (comp.neutral.Count != neutral.Count)
			{
				return false;
			}
			foreach (int item2 in neutral)
			{
				if (!comp.neutral.Contains(item2))
				{
					return false;
				}
			}
			if (comp.enemy.Count != enemy.Count)
			{
				return false;
			}
			foreach (int item3 in enemy)
			{
				if (!comp.enemy.Contains(item3))
				{
					return false;
				}
			}
			return true;
		}

		public Faction_MasterData Clone()
		{
			Faction_MasterData faction_MasterData = new Faction_MasterData();
			CopyTo(faction_MasterData);
			return faction_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			group = 0;
			ally.Clear();
			neutral.Clear();
			enemy.Clear();
		}
	}
}
