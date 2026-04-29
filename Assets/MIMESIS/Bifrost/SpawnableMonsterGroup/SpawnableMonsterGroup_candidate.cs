using System;
using System.IO;
using System.Net;

namespace Bifrost.SpawnableMonsterGroup
{
	public class SpawnableMonsterGroup_candidate : ISchema
	{
		public int monster_id;

		public int rate;

		public SpawnableMonsterGroup_candidate(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public SpawnableMonsterGroup_candidate()
			: base(1451699948u, "SpawnableMonsterGroup_candidate")
		{
		}

		public void CopyTo(SpawnableMonsterGroup_candidate dest)
		{
			dest.monster_id = monster_id;
			dest.rate = rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(monster_id) + Serializer.GetLength(rate);
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
			Serializer.Load(br, ref monster_id);
			Serializer.Load(br, ref rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, monster_id);
			Serializer.Save(bw, rate);
		}

		public bool Equal(SpawnableMonsterGroup_candidate comp)
		{
			if (monster_id != comp.monster_id)
			{
				return false;
			}
			if (rate != comp.rate)
			{
				return false;
			}
			return true;
		}

		public SpawnableMonsterGroup_candidate Clone()
		{
			SpawnableMonsterGroup_candidate spawnableMonsterGroup_candidate = new SpawnableMonsterGroup_candidate();
			CopyTo(spawnableMonsterGroup_candidate);
			return spawnableMonsterGroup_candidate;
		}

		public override void Clean()
		{
			monster_id = 0;
			rate = 0;
		}
	}
}
