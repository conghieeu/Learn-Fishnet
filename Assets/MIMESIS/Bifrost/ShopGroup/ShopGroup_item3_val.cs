using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item3_val : ISchema
	{
		public int item3_discount_rate;

		public int item3_rate;

		public ShopGroup_item3_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item3_val()
			: base(2357330065u, "ShopGroup_item3_val")
		{
		}

		public void CopyTo(ShopGroup_item3_val dest)
		{
			dest.item3_discount_rate = item3_discount_rate;
			dest.item3_rate = item3_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item3_discount_rate) + Serializer.GetLength(item3_rate);
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
			Serializer.Load(br, ref item3_discount_rate);
			Serializer.Load(br, ref item3_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item3_discount_rate);
			Serializer.Save(bw, item3_rate);
		}

		public bool Equal(ShopGroup_item3_val comp)
		{
			if (item3_discount_rate != comp.item3_discount_rate)
			{
				return false;
			}
			if (item3_rate != comp.item3_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item3_val Clone()
		{
			ShopGroup_item3_val shopGroup_item3_val = new ShopGroup_item3_val();
			CopyTo(shopGroup_item3_val);
			return shopGroup_item3_val;
		}

		public override void Clean()
		{
			item3_discount_rate = 0;
			item3_rate = 0;
		}
	}
}
