using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.SpawnableMonsterGroup
{
	public class SpawnableMonsterGroup_MasterData : ISchema
	{
		public int id;

		public List<SpawnableMonsterGroup_candidate> SpawnableMonsterGroup_candidateval = new List<SpawnableMonsterGroup_candidate>();

		public SpawnableMonsterGroup_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public SpawnableMonsterGroup_MasterData()
			: base(1864179235u, "SpawnableMonsterGroup_MasterData")
		{
		}

		public void CopyTo(SpawnableMonsterGroup_MasterData dest)
		{
			dest.id = id;
			dest.SpawnableMonsterGroup_candidateval.Clear();
			foreach (SpawnableMonsterGroup_candidate item in SpawnableMonsterGroup_candidateval)
			{
				SpawnableMonsterGroup_candidate spawnableMonsterGroup_candidate = new SpawnableMonsterGroup_candidate();
				item.CopyTo(spawnableMonsterGroup_candidate);
				dest.SpawnableMonsterGroup_candidateval.Add(spawnableMonsterGroup_candidate);
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
			foreach (SpawnableMonsterGroup_candidate item in SpawnableMonsterGroup_candidateval)
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
				SpawnableMonsterGroup_candidate spawnableMonsterGroup_candidate = new SpawnableMonsterGroup_candidate();
				spawnableMonsterGroup_candidate.LoadInternal(br);
				SpawnableMonsterGroup_candidateval.Add(spawnableMonsterGroup_candidate);
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
			Serializer.Save(bw, SpawnableMonsterGroup_candidateval.Count);
			foreach (SpawnableMonsterGroup_candidate item in SpawnableMonsterGroup_candidateval)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(SpawnableMonsterGroup_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (comp.SpawnableMonsterGroup_candidateval.Count != SpawnableMonsterGroup_candidateval.Count)
			{
				return false;
			}
			foreach (SpawnableMonsterGroup_candidate item in SpawnableMonsterGroup_candidateval)
			{
				bool flag = false;
				foreach (SpawnableMonsterGroup_candidate item2 in comp.SpawnableMonsterGroup_candidateval)
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

		public SpawnableMonsterGroup_MasterData Clone()
		{
			SpawnableMonsterGroup_MasterData spawnableMonsterGroup_MasterData = new SpawnableMonsterGroup_MasterData();
			CopyTo(spawnableMonsterGroup_MasterData);
			return spawnableMonsterGroup_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			SpawnableMonsterGroup_candidateval.Clear();
		}
	}
}
