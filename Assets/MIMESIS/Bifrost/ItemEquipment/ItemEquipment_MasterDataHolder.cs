using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ItemEquipment
{
	public class ItemEquipment_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<ItemEquipment_MasterData> dataHolder = new List<ItemEquipment_MasterData>();

		public ItemEquipment_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemEquipment_MasterDataHolder()
			: base(609721697u, "ItemEquipment_MasterDataHolder")
		{
		}

		public void CopyTo(ItemEquipment_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (ItemEquipment_MasterData item in dataHolder)
			{
				ItemEquipment_MasterData itemEquipment_MasterData = new ItemEquipment_MasterData();
				item.CopyTo(itemEquipment_MasterData);
				dest.dataHolder.Add(itemEquipment_MasterData);
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
			foreach (ItemEquipment_MasterData item in dataHolder)
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
				ItemEquipment_MasterData itemEquipment_MasterData = new ItemEquipment_MasterData();
				itemEquipment_MasterData.LoadInternal(br);
				dataHolder.Add(itemEquipment_MasterData);
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
			foreach (ItemEquipment_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(ItemEquipment_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (ItemEquipment_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (ItemEquipment_MasterData item2 in comp.dataHolder)
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

		public ItemEquipment_MasterDataHolder Clone()
		{
			ItemEquipment_MasterDataHolder itemEquipment_MasterDataHolder = new ItemEquipment_MasterDataHolder();
			CopyTo(itemEquipment_MasterDataHolder);
			return itemEquipment_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
