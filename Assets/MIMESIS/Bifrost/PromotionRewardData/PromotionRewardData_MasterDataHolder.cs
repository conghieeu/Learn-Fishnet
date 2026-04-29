using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.PromotionRewardData
{
	public class PromotionRewardData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<PromotionRewardData_MasterData> dataHolder = new List<PromotionRewardData_MasterData>();

		public PromotionRewardData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public PromotionRewardData_MasterDataHolder()
			: base(3445838854u, "PromotionRewardData_MasterDataHolder")
		{
		}

		public void CopyTo(PromotionRewardData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (PromotionRewardData_MasterData item in dataHolder)
			{
				PromotionRewardData_MasterData promotionRewardData_MasterData = new PromotionRewardData_MasterData();
				item.CopyTo(promotionRewardData_MasterData);
				dest.dataHolder.Add(promotionRewardData_MasterData);
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
			foreach (PromotionRewardData_MasterData item in dataHolder)
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
				PromotionRewardData_MasterData promotionRewardData_MasterData = new PromotionRewardData_MasterData();
				promotionRewardData_MasterData.LoadInternal(br);
				dataHolder.Add(promotionRewardData_MasterData);
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
			foreach (PromotionRewardData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(PromotionRewardData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (PromotionRewardData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (PromotionRewardData_MasterData item2 in comp.dataHolder)
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

		public PromotionRewardData_MasterDataHolder Clone()
		{
			PromotionRewardData_MasterDataHolder promotionRewardData_MasterDataHolder = new PromotionRewardData_MasterDataHolder();
			CopyTo(promotionRewardData_MasterDataHolder);
			return promotionRewardData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
