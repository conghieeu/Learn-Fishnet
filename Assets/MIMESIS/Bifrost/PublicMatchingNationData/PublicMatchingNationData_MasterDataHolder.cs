using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.PublicMatchingNationData
{
	public class PublicMatchingNationData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<PublicMatchingNationData_MasterData> dataHolder = new List<PublicMatchingNationData_MasterData>();

		public PublicMatchingNationData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public PublicMatchingNationData_MasterDataHolder()
			: base(2935417124u, "PublicMatchingNationData_MasterDataHolder")
		{
		}

		public void CopyTo(PublicMatchingNationData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (PublicMatchingNationData_MasterData item in dataHolder)
			{
				PublicMatchingNationData_MasterData publicMatchingNationData_MasterData = new PublicMatchingNationData_MasterData();
				item.CopyTo(publicMatchingNationData_MasterData);
				dest.dataHolder.Add(publicMatchingNationData_MasterData);
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
			foreach (PublicMatchingNationData_MasterData item in dataHolder)
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
				PublicMatchingNationData_MasterData publicMatchingNationData_MasterData = new PublicMatchingNationData_MasterData();
				publicMatchingNationData_MasterData.LoadInternal(br);
				dataHolder.Add(publicMatchingNationData_MasterData);
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
			foreach (PublicMatchingNationData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(PublicMatchingNationData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (PublicMatchingNationData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (PublicMatchingNationData_MasterData item2 in comp.dataHolder)
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

		public PublicMatchingNationData_MasterDataHolder Clone()
		{
			PublicMatchingNationData_MasterDataHolder publicMatchingNationData_MasterDataHolder = new PublicMatchingNationData_MasterDataHolder();
			CopyTo(publicMatchingNationData_MasterDataHolder);
			return publicMatchingNationData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
