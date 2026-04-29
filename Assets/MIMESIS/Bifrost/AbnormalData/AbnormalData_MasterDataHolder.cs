using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.AbnormalData
{
	public class AbnormalData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<AbnormalData_MasterData> dataHolder = new List<AbnormalData_MasterData>();

		public AbnormalData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public AbnormalData_MasterDataHolder()
			: base(885090640u, "AbnormalData_MasterDataHolder")
		{
		}

		public void CopyTo(AbnormalData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (AbnormalData_MasterData item in dataHolder)
			{
				AbnormalData_MasterData abnormalData_MasterData = new AbnormalData_MasterData();
				item.CopyTo(abnormalData_MasterData);
				dest.dataHolder.Add(abnormalData_MasterData);
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
			foreach (AbnormalData_MasterData item in dataHolder)
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
				AbnormalData_MasterData abnormalData_MasterData = new AbnormalData_MasterData();
				abnormalData_MasterData.LoadInternal(br);
				dataHolder.Add(abnormalData_MasterData);
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
			foreach (AbnormalData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(AbnormalData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (AbnormalData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (AbnormalData_MasterData item2 in comp.dataHolder)
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

		public AbnormalData_MasterDataHolder Clone()
		{
			AbnormalData_MasterDataHolder abnormalData_MasterDataHolder = new AbnormalData_MasterDataHolder();
			CopyTo(abnormalData_MasterDataHolder);
			return abnormalData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
