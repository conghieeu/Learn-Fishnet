using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item1_val : ISchema
	{
		public int item1_discount_rate;

		public int item1_rate;

		public ShopGroup_item1_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item1_val()
			: base(2146713701u, "ShopGroup_item1_val")
		{
		}

		public void CopyTo(ShopGroup_item1_val dest)
		{
			dest.item1_discount_rate = item1_discount_rate;
			dest.item1_rate = item1_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item1_discount_rate) + Serializer.GetLength(item1_rate);
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
			Serializer.Load(br, ref item1_discount_rate);
			Serializer.Load(br, ref item1_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item1_discount_rate);
			Serializer.Save(bw, item1_rate);
		}

		public bool Equal(ShopGroup_item1_val comp)
		{
			if (item1_discount_rate != comp.item1_discount_rate)
			{
				return false;
			}
			if (item1_rate != comp.item1_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item1_val Clone()
		{
			ShopGroup_item1_val shopGroup_item1_val = new ShopGroup_item1_val();
			CopyTo(shopGroup_item1_val);
			return shopGroup_item1_val;
		}

		public override void Clean()
		{
			item1_discount_rate = 0;
			item1_rate = 0;
		}
	}
}
