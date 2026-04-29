using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item8_val : ISchema
	{
		public int item8_discount_rate;

		public int item8_rate;

		public ShopGroup_item8_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item8_val()
			: base(3315792092u, "ShopGroup_item8_val")
		{
		}

		public void CopyTo(ShopGroup_item8_val dest)
		{
			dest.item8_discount_rate = item8_discount_rate;
			dest.item8_rate = item8_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item8_discount_rate) + Serializer.GetLength(item8_rate);
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
			Serializer.Load(br, ref item8_discount_rate);
			Serializer.Load(br, ref item8_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item8_discount_rate);
			Serializer.Save(bw, item8_rate);
		}

		public bool Equal(ShopGroup_item8_val comp)
		{
			if (item8_discount_rate != comp.item8_discount_rate)
			{
				return false;
			}
			if (item8_rate != comp.item8_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item8_val Clone()
		{
			ShopGroup_item8_val shopGroup_item8_val = new ShopGroup_item8_val();
			CopyTo(shopGroup_item8_val);
			return shopGroup_item8_val;
		}

		public override void Clean()
		{
			item8_discount_rate = 0;
			item8_rate = 0;
		}
	}
}
