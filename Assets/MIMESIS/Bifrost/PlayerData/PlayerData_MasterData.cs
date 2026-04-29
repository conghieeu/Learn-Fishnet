using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.PlayerData
{
	public class PlayerData_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public string fpp_model_path = string.Empty;

		public string tpp_model_path = string.Empty;

		public long hp;

		public long attack_power;

		public long defense;

		public long move_speed_walk;

		public long move_speed_run;

		public List<int> factions = new List<int>();

		public int spawn_wait_time;

		public int dying_wait_time;

		public float move_radius;

		public float hit_radius;

		public PlayerData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public PlayerData_MasterData()
			: base(1527286077u, "PlayerData_MasterData")
		{
		}

		public void CopyTo(PlayerData_MasterData dest)
		{
			dest.id = id;
			dest.name = name;
			dest.fpp_model_path = fpp_model_path;
			dest.tpp_model_path = tpp_model_path;
			dest.hp = hp;
			dest.attack_power = attack_power;
			dest.defense = defense;
			dest.move_speed_walk = move_speed_walk;
			dest.move_speed_run = move_speed_run;
			dest.factions.Clear();
			foreach (int faction in factions)
			{
				dest.factions.Add(faction);
			}
			dest.spawn_wait_time = spawn_wait_time;
			dest.dying_wait_time = dying_wait_time;
			dest.move_radius = move_radius;
			dest.hit_radius = hit_radius;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += Serializer.GetLength(name);
			num += Serializer.GetLength(fpp_model_path);
			num += Serializer.GetLength(tpp_model_path);
			num += Serializer.GetLength(hp);
			num += Serializer.GetLength(attack_power);
			num += Serializer.GetLength(defense);
			num += Serializer.GetLength(move_speed_walk);
			num += Serializer.GetLength(move_speed_run);
			num += 4;
			foreach (int faction in factions)
			{
				num += Serializer.GetLength(faction);
			}
			num += Serializer.GetLength(spawn_wait_time);
			num += Serializer.GetLength(dying_wait_time);
			num += Serializer.GetLength(move_radius);
			return num + Serializer.GetLength(hit_radius);
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
			Serializer.Load(br, ref name);
			Serializer.Load(br, ref fpp_model_path);
			Serializer.Load(br, ref tpp_model_path);
			Serializer.Load(br, ref hp);
			Serializer.Load(br, ref attack_power);
			Serializer.Load(br, ref defense);
			Serializer.Load(br, ref move_speed_walk);
			Serializer.Load(br, ref move_speed_run);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				int intValue2 = 0;
				Serializer.Load(br, ref intValue2);
				factions.Add(intValue2);
			}
			Serializer.Load(br, ref spawn_wait_time);
			Serializer.Load(br, ref dying_wait_time);
			Serializer.Load(br, ref move_radius);
			Serializer.Load(br, ref hit_radius);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, name);
			Serializer.Save(bw, fpp_model_path);
			Serializer.Save(bw, tpp_model_path);
			Serializer.Save(bw, hp);
			Serializer.Save(bw, attack_power);
			Serializer.Save(bw, defense);
			Serializer.Save(bw, move_speed_walk);
			Serializer.Save(bw, move_speed_run);
			Serializer.Save(bw, factions.Count);
			foreach (int faction in factions)
			{
				Serializer.Save(bw, faction);
			}
			Serializer.Save(bw, spawn_wait_time);
			Serializer.Save(bw, dying_wait_time);
			Serializer.Save(bw, move_radius);
			Serializer.Save(bw, hit_radius);
		}

		public bool Equal(PlayerData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (fpp_model_path != comp.fpp_model_path)
			{
				return false;
			}
			if (tpp_model_path != comp.tpp_model_path)
			{
				return false;
			}
			if (hp != comp.hp)
			{
				return false;
			}
			if (attack_power != comp.attack_power)
			{
				return false;
			}
			if (defense != comp.defense)
			{
				return false;
			}
			if (move_speed_walk != comp.move_speed_walk)
			{
				return false;
			}
			if (move_speed_run != comp.move_speed_run)
			{
				return false;
			}
			if (comp.factions.Count != factions.Count)
			{
				return false;
			}
			foreach (int faction in factions)
			{
				if (!comp.factions.Contains(faction))
				{
					return false;
				}
			}
			if (spawn_wait_time != comp.spawn_wait_time)
			{
				return false;
			}
			if (dying_wait_time != comp.dying_wait_time)
			{
				return false;
			}
			if (move_radius != comp.move_radius)
			{
				return false;
			}
			if (hit_radius != comp.hit_radius)
			{
				return false;
			}
			return true;
		}

		public PlayerData_MasterData Clone()
		{
			PlayerData_MasterData playerData_MasterData = new PlayerData_MasterData();
			CopyTo(playerData_MasterData);
			return playerData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			fpp_model_path = string.Empty;
			tpp_model_path = string.Empty;
			hp = 0L;
			attack_power = 0L;
			defense = 0L;
			move_speed_walk = 0L;
			move_speed_run = 0L;
			factions.Clear();
			spawn_wait_time = 0;
			dying_wait_time = 0;
			move_radius = 0f;
			hit_radius = 0f;
		}
	}
}
