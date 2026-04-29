using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item7_val : ISchema
	{
		public int item7_discount_rate;

		public int item7_rate;

		public ShopGroup_item7_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item7_val()
			: base(1870854024u, "ShopGroup_item7_val")
		{
		}

		public void CopyTo(ShopGroup_item7_val dest)
		{
			dest.item7_discount_rate = item7_discount_rate;
			dest.item7_rate = item7_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item7_discount_rate) + Serializer.GetLength(item7_rate);
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
			Serializer.Load(br, ref item7_discount_rate);
			Serializer.Load(br, ref item7_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item7_discount_rate);
			Serializer.Save(bw, item7_rate);
		}

		public bool Equal(ShopGroup_item7_val comp)
		{
			if (item7_discount_rate != comp.item7_discount_rate)
			{
				return false;
			}
			if (item7_rate != comp.item7_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item7_val Clone()
		{
			ShopGroup_item7_val shopGroup_item7_val = new ShopGroup_item7_val();
			CopyTo(shopGroup_item7_val);
			return shopGroup_item7_val;
		}

		public override void Clean()
		{
			item7_discount_rate = 0;
			item7_rate = 0;
		}
	}
}
