using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.DefEventCondition
{
	public class DefEventCondition_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<DefEventCondition_MasterData> dataHolder = new List<DefEventCondition_MasterData>();

		public DefEventCondition_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public DefEventCondition_MasterDataHolder()
			: base(1301410674u, "DefEventCondition_MasterDataHolder")
		{
		}

		public void CopyTo(DefEventCondition_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (DefEventCondition_MasterData item in dataHolder)
			{
				DefEventCondition_MasterData defEventCondition_MasterData = new DefEventCondition_MasterData();
				item.CopyTo(defEventCondition_MasterData);
				dest.dataHolder.Add(defEventCondition_MasterData);
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
			foreach (DefEventCondition_MasterData item in dataHolder)
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
				DefEventCondition_MasterData defEventCondition_MasterData = new DefEventCondition_MasterData();
				defEventCondition_MasterData.LoadInternal(br);
				dataHolder.Add(defEventCondition_MasterData);
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
			foreach (DefEventCondition_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(DefEventCondition_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (DefEventCondition_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (DefEventCondition_MasterData item2 in comp.dataHolder)
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

		public DefEventCondition_MasterDataHolder Clone()
		{
			DefEventCondition_MasterDataHolder defEventCondition_MasterDataHolder = new DefEventCondition_MasterDataHolder();
			CopyTo(defEventCondition_MasterDataHolder);
			return defEventCondition_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
