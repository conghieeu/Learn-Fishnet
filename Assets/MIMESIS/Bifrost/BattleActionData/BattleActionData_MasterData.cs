using System;
using System.IO;
using System.Net;

namespace Bifrost.BattleActionData
{
	public class BattleActionData_MasterData : ISchema
	{
		public int id;

		public string key = string.Empty;

		public string type = string.Empty;

		public long distance;

		public long move_time;

		public long down_time;

		public bool turn_to_attacker;

		public BattleActionData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public BattleActionData_MasterData()
			: base(4245325500u, "BattleActionData_MasterData")
		{
		}

		public void CopyTo(BattleActionData_MasterData dest)
		{
			dest.id = id;
			dest.key = key;
			dest.type = type;
			dest.distance = distance;
			dest.move_time = move_time;
			dest.down_time = down_time;
			dest.turn_to_attacker = turn_to_attacker;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(key) + Serializer.GetLength(type) + Serializer.GetLength(distance) + Serializer.GetLength(move_time) + Serializer.GetLength(down_time) + Serializer.GetLength(turn_to_attacker);
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
			Serializer.Load(br, ref key);
			Serializer.Load(br, ref type);
			Serializer.Load(br, ref distance);
			Serializer.Load(br, ref move_time);
			Serializer.Load(br, ref down_time);
			Serializer.Load(br, ref turn_to_attacker);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, key);
			Serializer.Save(bw, type);
			Serializer.Save(bw, distance);
			Serializer.Save(bw, move_time);
			Serializer.Save(bw, down_time);
			Serializer.Save(bw, turn_to_attacker);
		}

		public bool Equal(BattleActionData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (key != comp.key)
			{
				return false;
			}
			if (type != comp.type)
			{
				return false;
			}
			if (distance != comp.distance)
			{
				return false;
			}
			if (move_time != comp.move_time)
			{
				return false;
			}
			if (down_time != comp.down_time)
			{
				return false;
			}
			if (turn_to_attacker != comp.turn_to_attacker)
			{
				return false;
			}
			return true;
		}

		public BattleActionData_MasterData Clone()
		{
			BattleActionData_MasterData battleActionData_MasterData = new BattleActionData_MasterData();
			CopyTo(battleActionData_MasterData);
			return battleActionData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			key = string.Empty;
			type = string.Empty;
			distance = 0L;
			move_time = 0L;
			down_time = 0L;
			turn_to_attacker = false;
		}
	}
}
