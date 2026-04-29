using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<ShopGroup_MasterData> dataHolder = new List<ShopGroup_MasterData>();

		public ShopGroup_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_MasterDataHolder()
			: base(1949235412u, "ShopGroup_MasterDataHolder")
		{
		}

		public void CopyTo(ShopGroup_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (ShopGroup_MasterData item in dataHolder)
			{
				ShopGroup_MasterData shopGroup_MasterData = new ShopGroup_MasterData();
				item.CopyTo(shopGroup_MasterData);
				dest.dataHolder.Add(shopGroup_MasterData);
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
			foreach (ShopGroup_MasterData item in dataHolder)
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
				ShopGroup_MasterData shopGroup_MasterData = new ShopGroup_MasterData();
				shopGroup_MasterData.LoadInternal(br);
				dataHolder.Add(shopGroup_MasterData);
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
			foreach (ShopGroup_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(ShopGroup_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (ShopGroup_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (ShopGroup_MasterData item2 in comp.dataHolder)
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

		public ShopGroup_MasterDataHolder Clone()
		{
			ShopGroup_MasterDataHolder shopGroup_MasterDataHolder = new ShopGroup_MasterDataHolder();
			CopyTo(shopGroup_MasterDataHolder);
			return shopGroup_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
