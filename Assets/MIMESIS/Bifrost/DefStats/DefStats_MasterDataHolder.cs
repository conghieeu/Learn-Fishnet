using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.DefStats
{
	public class DefStats_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<DefStats_MasterData> dataHolder = new List<DefStats_MasterData>();

		public DefStats_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public DefStats_MasterDataHolder()
			: base(2611216290u, "DefStats_MasterDataHolder")
		{
		}

		public void CopyTo(DefStats_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (DefStats_MasterData item in dataHolder)
			{
				DefStats_MasterData defStats_MasterData = new DefStats_MasterData();
				item.CopyTo(defStats_MasterData);
				dest.dataHolder.Add(defStats_MasterData);
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
			foreach (DefStats_MasterData item in dataHolder)
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
				DefStats_MasterData defStats_MasterData = new DefStats_MasterData();
				defStats_MasterData.LoadInternal(br);
				dataHolder.Add(defStats_MasterData);
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
			foreach (DefStats_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(DefStats_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (DefStats_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (DefStats_MasterData item2 in comp.dataHolder)
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

		public DefStats_MasterDataHolder Clone()
		{
			DefStats_MasterDataHolder defStats_MasterDataHolder = new DefStats_MasterDataHolder();
			CopyTo(defStats_MasterDataHolder);
			return defStats_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
