using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ItemMiscellany
{
	public class ItemMiscellany_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<ItemMiscellany_MasterData> dataHolder = new List<ItemMiscellany_MasterData>();

		public ItemMiscellany_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemMiscellany_MasterDataHolder()
			: base(97636827u, "ItemMiscellany_MasterDataHolder")
		{
		}

		public void CopyTo(ItemMiscellany_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (ItemMiscellany_MasterData item in dataHolder)
			{
				ItemMiscellany_MasterData itemMiscellany_MasterData = new ItemMiscellany_MasterData();
				item.CopyTo(itemMiscellany_MasterData);
				dest.dataHolder.Add(itemMiscellany_MasterData);
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
			foreach (ItemMiscellany_MasterData item in dataHolder)
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
				ItemMiscellany_MasterData itemMiscellany_MasterData = new ItemMiscellany_MasterData();
				itemMiscellany_MasterData.LoadInternal(br);
				dataHolder.Add(itemMiscellany_MasterData);
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
			foreach (ItemMiscellany_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(ItemMiscellany_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (ItemMiscellany_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (ItemMiscellany_MasterData item2 in comp.dataHolder)
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

		public ItemMiscellany_MasterDataHolder Clone()
		{
			ItemMiscellany_MasterDataHolder itemMiscellany_MasterDataHolder = new ItemMiscellany_MasterDataHolder();
			CopyTo(itemMiscellany_MasterDataHolder);
			return itemMiscellany_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
