using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ItemDrop
{
	public class ItemDrop_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<ItemDrop_MasterData> dataHolder = new List<ItemDrop_MasterData>();

		public ItemDrop_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemDrop_MasterDataHolder()
			: base(3518983031u, "ItemDrop_MasterDataHolder")
		{
		}

		public void CopyTo(ItemDrop_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (ItemDrop_MasterData item in dataHolder)
			{
				ItemDrop_MasterData itemDrop_MasterData = new ItemDrop_MasterData();
				item.CopyTo(itemDrop_MasterData);
				dest.dataHolder.Add(itemDrop_MasterData);
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
			foreach (ItemDrop_MasterData item in dataHolder)
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
				ItemDrop_MasterData itemDrop_MasterData = new ItemDrop_MasterData();
				itemDrop_MasterData.LoadInternal(br);
				dataHolder.Add(itemDrop_MasterData);
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
			foreach (ItemDrop_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(ItemDrop_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (ItemDrop_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (ItemDrop_MasterData item2 in comp.dataHolder)
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

		public ItemDrop_MasterDataHolder Clone()
		{
			ItemDrop_MasterDataHolder itemDrop_MasterDataHolder = new ItemDrop_MasterDataHolder();
			CopyTo(itemDrop_MasterDataHolder);
			return itemDrop_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
