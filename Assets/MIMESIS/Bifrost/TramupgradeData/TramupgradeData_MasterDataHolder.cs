using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.TramupgradeData
{
	public class TramupgradeData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<TramupgradeData_MasterData> dataHolder = new List<TramupgradeData_MasterData>();

		public TramupgradeData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public TramupgradeData_MasterDataHolder()
			: base(1358389413u, "TramupgradeData_MasterDataHolder")
		{
		}

		public void CopyTo(TramupgradeData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (TramupgradeData_MasterData item in dataHolder)
			{
				TramupgradeData_MasterData tramupgradeData_MasterData = new TramupgradeData_MasterData();
				item.CopyTo(tramupgradeData_MasterData);
				dest.dataHolder.Add(tramupgradeData_MasterData);
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
			foreach (TramupgradeData_MasterData item in dataHolder)
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
				TramupgradeData_MasterData tramupgradeData_MasterData = new TramupgradeData_MasterData();
				tramupgradeData_MasterData.LoadInternal(br);
				dataHolder.Add(tramupgradeData_MasterData);
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
			foreach (TramupgradeData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(TramupgradeData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (TramupgradeData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (TramupgradeData_MasterData item2 in comp.dataHolder)
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

		public TramupgradeData_MasterDataHolder Clone()
		{
			TramupgradeData_MasterDataHolder tramupgradeData_MasterDataHolder = new TramupgradeData_MasterDataHolder();
			CopyTo(tramupgradeData_MasterDataHolder);
			return tramupgradeData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
