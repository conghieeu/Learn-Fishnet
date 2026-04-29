using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.DefAbnormal
{
	public class DefAbnormal_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<DefAbnormal_MasterData> dataHolder = new List<DefAbnormal_MasterData>();

		public DefAbnormal_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public DefAbnormal_MasterDataHolder()
			: base(3782695003u, "DefAbnormal_MasterDataHolder")
		{
		}

		public void CopyTo(DefAbnormal_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (DefAbnormal_MasterData item in dataHolder)
			{
				DefAbnormal_MasterData defAbnormal_MasterData = new DefAbnormal_MasterData();
				item.CopyTo(defAbnormal_MasterData);
				dest.dataHolder.Add(defAbnormal_MasterData);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(versionInfo);
			num += 4;
			foreach (DefAbnormal_MasterData item in dataHolder)
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
			Serializer.Load(br, ref versionInfo);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				DefAbnormal_MasterData defAbnormal_MasterData = new DefAbnormal_MasterData();
				defAbnormal_MasterData.LoadInternal(br);
				dataHolder.Add(defAbnormal_MasterData);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, versionInfo);
			Serializer.Save(bw, dataHolder.Count);
			foreach (DefAbnormal_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(DefAbnormal_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (DefAbnormal_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (DefAbnormal_MasterData item2 in comp.dataHolder)
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

		public DefAbnormal_MasterDataHolder Clone()
		{
			DefAbnormal_MasterDataHolder defAbnormal_MasterDataHolder = new DefAbnormal_MasterDataHolder();
			CopyTo(defAbnormal_MasterDataHolder);
			return defAbnormal_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
