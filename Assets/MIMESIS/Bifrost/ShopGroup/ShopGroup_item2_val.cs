using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item2_val : ISchema
	{
		public int item2_discount_rate;

		public int item2_rate;

		public ShopGroup_item2_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item2_val()
			: base(4114161899u, "ShopGroup_item2_val")
		{
		}

		public void CopyTo(ShopGroup_item2_val dest)
		{
			dest.item2_discount_rate = item2_discount_rate;
			dest.item2_rate = item2_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item2_discount_rate) + Serializer.GetLength(item2_rate);
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
			Serializer.Load(br, ref item2_discount_rate);
			Serializer.Load(br, ref item2_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item2_discount_rate);
			Serializer.Save(bw, item2_rate);
		}

		public bool Equal(ShopGroup_item2_val comp)
		{
			if (item2_discount_rate != comp.item2_discount_rate)
			{
				return false;
			}
			if (item2_rate != comp.item2_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item2_val Clone()
		{
			ShopGroup_item2_val shopGroup_item2_val = new ShopGroup_item2_val();
			CopyTo(shopGroup_item2_val);
			return shopGroup_item2_val;
		}

		public override void Clean()
		{
			item2_discount_rate = 0;
			item2_rate = 0;
		}
	}
}
