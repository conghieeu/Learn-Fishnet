using System;
using System.IO;
using System.Net;

namespace Bifrost.MonsterData
{
	public class MonsterData_aggro : ISchema
	{
		public int type;

		public int weight;

		public float range;

		public float increase_score_per_distance;

		public MonsterData_aggro(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public MonsterData_aggro()
			: base(4181334549u, "MonsterData_aggro")
		{
		}

		public void CopyTo(MonsterData_aggro dest)
		{
			dest.type = type;
			dest.weight = weight;
			dest.range = range;
			dest.increase_score_per_distance = increase_score_per_distance;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(type) + Serializer.GetLength(weight) + Serializer.GetLength(range) + Serializer.GetLength(increase_score_per_distance);
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
			Serializer.Load(br, ref type);
			Serializer.Load(br, ref weight);
			Serializer.Load(br, ref range);
			Serializer.Load(br, ref increase_score_per_distance);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, type);
			Serializer.Save(bw, weight);
			Serializer.Save(bw, range);
			Serializer.Save(bw, increase_score_per_distance);
		}

		public bool Equal(MonsterData_aggro comp)
		{
			if (type != comp.type)
			{
				return false;
			}
			if (weight != comp.weight)
			{
				return false;
			}
			if (range != comp.range)
			{
				return false;
			}
			if (increase_score_per_distance != comp.increase_score_per_distance)
			{
				return false;
			}
			return true;
		}

		public MonsterData_aggro Clone()
		{
			MonsterData_aggro monsterData_aggro = new MonsterData_aggro();
			CopyTo(monsterData_aggro);
			return monsterData_aggro;
		}

		public override void Clean()
		{
			type = 0;
			weight = 0;
			range = 0f;
			increase_score_per_distance = 0f;
		}
	}
}
