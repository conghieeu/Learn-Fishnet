using System;
using System.IO;
using System.Net;

namespace Bifrost.DefStats
{
	public class DefStats_MasterData : ISchema
	{
		public int id;

		public string key = string.Empty;

		public string mutable_key = string.Empty;

		public bool sync_immutable;

		public bool sync_mutable;

		public DefStats_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public DefStats_MasterData()
			: base(566842442u, "DefStats_MasterData")
		{
		}

		public void CopyTo(DefStats_MasterData dest)
		{
			dest.id = id;
			dest.key = key;
			dest.mutable_key = mutable_key;
			dest.sync_immutable = sync_immutable;
			dest.sync_mutable = sync_mutable;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(key) + Serializer.GetLength(mutable_key) + Serializer.GetLength(sync_immutable) + Serializer.GetLength(sync_mutable);
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
			Serializer.Load(br, ref mutable_key);
			Serializer.Load(br, ref sync_immutable);
			Serializer.Load(br, ref sync_mutable);
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
			Serializer.Save(bw, mutable_key);
			Serializer.Save(bw, sync_immutable);
			Serializer.Save(bw, sync_mutable);
		}

		public bool Equal(DefStats_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (key != comp.key)
			{
				return false;
			}
			if (mutable_key != comp.mutable_key)
			{
				return false;
			}
			if (sync_immutable != comp.sync_immutable)
			{
				return false;
			}
			if (sync_mutable != comp.sync_mutable)
			{
				return false;
			}
			return true;
		}

		public DefStats_MasterData Clone()
		{
			DefStats_MasterData defStats_MasterData = new DefStats_MasterData();
			CopyTo(defStats_MasterData);
			return defStats_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			key = string.Empty;
			mutable_key = string.Empty;
			sync_immutable = false;
			sync_mutable = false;
		}
	}
}
