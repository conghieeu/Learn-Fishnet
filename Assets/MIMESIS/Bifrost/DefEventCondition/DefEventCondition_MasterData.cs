using System;
using System.IO;
using System.Net;

namespace Bifrost.DefEventCondition
{
	public class DefEventCondition_MasterData : ISchema
	{
		public int id;

		public string key = string.Empty;

		public DefEventCondition_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public DefEventCondition_MasterData()
			: base(3272731809u, "DefEventCondition_MasterData")
		{
		}

		public void CopyTo(DefEventCondition_MasterData dest)
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

		public bool Equal(DefEventCondition_MasterData comp)
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

		public DefEventCondition_MasterData Clone()
		{
			DefEventCondition_MasterData defEventCondition_MasterData = new DefEventCondition_MasterData();
			CopyTo(defEventCondition_MasterData);
			return defEventCondition_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			key = string.Empty;
		}
	}
}
