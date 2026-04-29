using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item5_val : ISchema
	{
		public int item5_discount_rate;

		public int item5_rate;

		public ShopGroup_item5_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item5_val()
			: base(2633283452u, "ShopGroup_item5_val")
		{
		}

		public void CopyTo(ShopGroup_item5_val dest)
		{
			dest.item5_discount_rate = item5_discount_rate;
			dest.item5_rate = item5_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item5_discount_rate) + Serializer.GetLength(item5_rate);
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
			Serializer.Load(br, ref item5_discount_rate);
			Serializer.Load(br, ref item5_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item5_discount_rate);
			Serializer.Save(bw, item5_rate);
		}

		public bool Equal(ShopGroup_item5_val comp)
		{
			if (item5_discount_rate != comp.item5_discount_rate)
			{
				return false;
			}
			if (item5_rate != comp.item5_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item5_val Clone()
		{
			ShopGroup_item5_val shopGroup_item5_val = new ShopGroup_item5_val();
			CopyTo(shopGroup_item5_val);
			return shopGroup_item5_val;
		}

		public override void Clean()
		{
			item5_discount_rate = 0;
			item5_rate = 0;
		}
	}
}
