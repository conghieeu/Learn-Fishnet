using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item6_val : ISchema
	{
		public int item6_discount_rate;

		public int item6_rate;

		public ShopGroup_item6_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item6_val()
			: base(372889586u, "ShopGroup_item6_val")
		{
		}

		public void CopyTo(ShopGroup_item6_val dest)
		{
			dest.item6_discount_rate = item6_discount_rate;
			dest.item6_rate = item6_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item6_discount_rate) + Serializer.GetLength(item6_rate);
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
			Serializer.Load(br, ref item6_discount_rate);
			Serializer.Load(br, ref item6_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item6_discount_rate);
			Serializer.Save(bw, item6_rate);
		}

		public bool Equal(ShopGroup_item6_val comp)
		{
			if (item6_discount_rate != comp.item6_discount_rate)
			{
				return false;
			}
			if (item6_rate != comp.item6_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item6_val Clone()
		{
			ShopGroup_item6_val shopGroup_item6_val = new ShopGroup_item6_val();
			CopyTo(shopGroup_item6_val);
			return shopGroup_item6_val;
		}

		public override void Clean()
		{
			item6_discount_rate = 0;
			item6_rate = 0;
		}
	}
}
