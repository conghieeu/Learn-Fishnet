using System;
using System.IO;
using System.Net;

namespace Bifrost.DefEventAction
{
	public class DefEventAction_MasterData : ISchema
	{
		public int id;

		public string key = string.Empty;

		public DefEventAction_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public DefEventAction_MasterData()
			: base(2967780006u, "DefEventAction_MasterData")
		{
		}

		public void CopyTo(DefEventAction_MasterData dest)
		{
			dest.id = id;
			dest.key = key;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(key);
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
		}

		public bool Equal(DefEventAction_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (key != comp.key)
			{
				return false;
			}
			return true;
		}

		public DefEventAction_MasterData Clone()
		{
			DefEventAction_MasterData defEventAction_MasterData = new DefEventAction_MasterData();
			CopyTo(defEventAction_MasterData);
			return defEventAction_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			key = string.Empty;
		}
	}
}
