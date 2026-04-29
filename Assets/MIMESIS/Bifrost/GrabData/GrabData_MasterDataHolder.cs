using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.GrabData
{
	public class GrabData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<GrabData_MasterData> dataHolder = new List<GrabData_MasterData>();

		public GrabData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public GrabData_MasterDataHolder()
			: base(37187660u, "GrabData_MasterDataHolder")
		{
		}

		public void CopyTo(GrabData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (GrabData_MasterData item in dataHolder)
			{
				GrabData_MasterData grabData_MasterData = new GrabData_MasterData();
				item.CopyTo(grabData_MasterData);
				dest.dataHolder.Add(grabData_MasterData);
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
			foreach (GrabData_MasterData item in dataHolder)
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
				GrabData_MasterData grabData_MasterData = new GrabData_MasterData();
				grabData_MasterData.LoadInternal(br);
				dataHolder.Add(grabData_MasterData);
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
			foreach (GrabData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(GrabData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (GrabData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (GrabData_MasterData item2 in comp.dataHolder)
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

		public GrabData_MasterDataHolder Clone()
		{
			GrabData_MasterDataHolder grabData_MasterDataHolder = new GrabData_MasterDataHolder();
			CopyTo(grabData_MasterDataHolder);
			return grabData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
