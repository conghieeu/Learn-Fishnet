using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.SpawnableMiscGroup
{
	public class SpawnableMiscGroup_MasterData : ISchema
	{
		public int id;

		public List<SpawnableMiscGroup_candidate> SpawnableMiscGroup_candidateval = new List<SpawnableMiscGroup_candidate>();

		public SpawnableMiscGroup_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public SpawnableMiscGroup_MasterData()
			: base(1181815834u, "SpawnableMiscGroup_MasterData")
		{
		}

		public void CopyTo(SpawnableMiscGroup_MasterData dest)
		{
			dest.id = id;
			dest.SpawnableMiscGroup_candidateval.Clear();
			foreach (SpawnableMiscGroup_candidate item in SpawnableMiscGroup_candidateval)
			{
				SpawnableMiscGroup_candidate spawnableMiscGroup_candidate = new SpawnableMiscGroup_candidate();
				item.CopyTo(spawnableMiscGroup_candidate);
				dest.SpawnableMiscGroup_candidateval.Add(spawnableMiscGroup_candidate);
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
			num += 4;
			foreach (SpawnableMiscGroup_candidate item in SpawnableMiscGroup_candidateval)
			{
				num += item.GetLengthInternal();
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
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				SpawnableMiscGroup_candidate spawnableMiscGroup_candidate = new SpawnableMiscGroup_candidate();
				spawnableMiscGroup_candidate.LoadInternal(br);
				SpawnableMiscGroup_candidateval.Add(spawnableMiscGroup_candidate);
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
			Serializer.Save(bw, SpawnableMiscGroup_candidateval.Count);
			foreach (SpawnableMiscGroup_candidate item in SpawnableMiscGroup_candidateval)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(SpawnableMiscGroup_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (comp.SpawnableMiscGroup_candidateval.Count != SpawnableMiscGroup_candidateval.Count)
			{
				return false;
			}
			foreach (SpawnableMiscGroup_candidate item in SpawnableMiscGroup_candidateval)
			{
				bool flag = false;
				foreach (SpawnableMiscGroup_candidate item2 in comp.SpawnableMiscGroup_candidateval)
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
			return true;
		}

		public SpawnableMiscGroup_MasterData Clone()
		{
			SpawnableMiscGroup_MasterData spawnableMiscGroup_MasterData = new SpawnableMiscGroup_MasterData();
			CopyTo(spawnableMiscGroup_MasterData);
			return spawnableMiscGroup_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			SpawnableMiscGroup_candidateval.Clear();
		}
	}
}
