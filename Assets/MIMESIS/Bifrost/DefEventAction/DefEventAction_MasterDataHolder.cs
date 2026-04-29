using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.DefEventAction
{
	public class DefEventAction_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<DefEventAction_MasterData> dataHolder = new List<DefEventAction_MasterData>();

		public DefEventAction_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public DefEventAction_MasterDataHolder()
			: base(3528736918u, "DefEventAction_MasterDataHolder")
		{
		}

		public void CopyTo(DefEventAction_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (DefEventAction_MasterData item in dataHolder)
			{
				DefEventAction_MasterData defEventAction_MasterData = new DefEventAction_MasterData();
				item.CopyTo(defEventAction_MasterData);
				dest.dataHolder.Add(defEventAction_MasterData);
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
			foreach (DefEventAction_MasterData item in dataHolder)
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
				DefEventAction_MasterData defEventAction_MasterData = new DefEventAction_MasterData();
				defEventAction_MasterData.LoadInternal(br);
				dataHolder.Add(defEventAction_MasterData);
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
			foreach (DefEventAction_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(DefEventAction_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (DefEventAction_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (DefEventAction_MasterData item2 in comp.dataHolder)
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

		public DefEventAction_MasterDataHolder Clone()
		{
			DefEventAction_MasterDataHolder defEventAction_MasterDataHolder = new DefEventAction_MasterDataHolder();
			CopyTo(defEventAction_MasterDataHolder);
			return defEventAction_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
