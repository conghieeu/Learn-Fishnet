using System;
using System.IO;
using System.Net;

namespace Bifrost.Dungeon
{
	public class Dungeon_candidate : ISchema
	{
		public string spawnable_dungen = string.Empty;

		public int spawnable_dungen_rate;

		public Dungeon_candidate(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Dungeon_candidate()
			: base(898543434u, "Dungeon_candidate")
		{
		}

		public void CopyTo(Dungeon_candidate dest)
		{
			dest.spawnable_dungen = spawnable_dungen;
			dest.spawnable_dungen_rate = spawnable_dungen_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(spawnable_dungen) + Serializer.GetLength(spawnable_dungen_rate);
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
			Serializer.Load(br, ref spawnable_dungen);
			Serializer.Load(br, ref spawnable_dungen_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, spawnable_dungen);
			Serializer.Save(bw, spawnable_dungen_rate);
		}

		public bool Equal(Dungeon_candidate comp)
		{
			if (spawnable_dungen != comp.spawnable_dungen)
			{
				return false;
			}
			if (spawnable_dungen_rate != comp.spawnable_dungen_rate)
			{
				return false;
			}
			return true;
		}

		public Dungeon_candidate Clone()
		{
			Dungeon_candidate dungeon_candidate = new Dungeon_candidate();
			CopyTo(dungeon_candidate);
			return dungeon_candidate;
		}

		public override void Clean()
		{
			spawnable_dungen = string.Empty;
			spawnable_dungen_rate = 0;
		}
	}
}
