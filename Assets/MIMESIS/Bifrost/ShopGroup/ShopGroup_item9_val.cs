using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item9_val : ISchema
	{
		public int item9_discount_rate;

		public int item9_rate;

		public ShopGroup_item9_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item9_val()
			: base(3155810470u, "ShopGroup_item9_val")
		{
		}

		public void CopyTo(ShopGroup_item9_val dest)
		{
			dest.item9_discount_rate = item9_discount_rate;
			dest.item9_rate = item9_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item9_discount_rate) + Serializer.GetLength(item9_rate);
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
			Serializer.Load(br, ref item9_discount_rate);
			Serializer.Load(br, ref item9_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item9_discount_rate);
			Serializer.Save(bw, item9_rate);
		}

		public bool Equal(ShopGroup_item9_val comp)
		{
			if (item9_discount_rate != comp.item9_discount_rate)
			{
				return false;
			}
			if (item9_rate != comp.item9_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item9_val Clone()
		{
			ShopGroup_item9_val shopGroup_item9_val = new ShopGroup_item9_val();
			CopyTo(shopGroup_item9_val);
			return shopGroup_item9_val;
		}

		public override void Clean()
		{
			item9_discount_rate = 0;
			item9_rate = 0;
		}
	}
}
