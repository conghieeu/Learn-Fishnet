using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.LocalizationData
{
	public class LocalizationData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<LocalizationData_MasterData> dataHolder = new List<LocalizationData_MasterData>();

		public LocalizationData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public LocalizationData_MasterDataHolder()
			: base(3385543696u, "LocalizationData_MasterDataHolder")
		{
		}

		public void CopyTo(LocalizationData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (LocalizationData_MasterData item in dataHolder)
			{
				LocalizationData_MasterData localizationData_MasterData = new LocalizationData_MasterData();
				item.CopyTo(localizationData_MasterData);
				dest.dataHolder.Add(localizationData_MasterData);
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
			foreach (LocalizationData_MasterData item in dataHolder)
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
				LocalizationData_MasterData localizationData_MasterData = new LocalizationData_MasterData();
				localizationData_MasterData.LoadInternal(br);
				dataHolder.Add(localizationData_MasterData);
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
			foreach (LocalizationData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(LocalizationData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (LocalizationData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (LocalizationData_MasterData item2 in comp.dataHolder)
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

		public LocalizationData_MasterDataHolder Clone()
		{
			LocalizationData_MasterDataHolder localizationData_MasterDataHolder = new LocalizationData_MasterDataHolder();
			CopyTo(localizationData_MasterDataHolder);
			return localizationData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
