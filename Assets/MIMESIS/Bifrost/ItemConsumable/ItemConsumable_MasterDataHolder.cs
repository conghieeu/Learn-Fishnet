using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ItemConsumable
{
	public class ItemConsumable_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<ItemConsumable_MasterData> dataHolder = new List<ItemConsumable_MasterData>();

		public ItemConsumable_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemConsumable_MasterDataHolder()
			: base(871969768u, "ItemConsumable_MasterDataHolder")
		{
		}

		public void CopyTo(ItemConsumable_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (ItemConsumable_MasterData item in dataHolder)
			{
				ItemConsumable_MasterData itemConsumable_MasterData = new ItemConsumable_MasterData();
				item.CopyTo(itemConsumable_MasterData);
				dest.dataHolder.Add(itemConsumable_MasterData);
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
			foreach (ItemConsumable_MasterData item in dataHolder)
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
				ItemConsumable_MasterData itemConsumable_MasterData = new ItemConsumable_MasterData();
				itemConsumable_MasterData.LoadInternal(br);
				dataHolder.Add(itemConsumable_MasterData);
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
			foreach (ItemConsumable_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(ItemConsumable_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (ItemConsumable_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (ItemConsumable_MasterData item2 in comp.dataHolder)
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

		public ItemConsumable_MasterDataHolder Clone()
		{
			ItemConsumable_MasterDataHolder itemConsumable_MasterDataHolder = new ItemConsumable_MasterDataHolder();
			CopyTo(itemConsumable_MasterDataHolder);
			return itemConsumable_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
