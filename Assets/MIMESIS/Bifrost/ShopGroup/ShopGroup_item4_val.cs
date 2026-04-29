using System;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_item4_val : ISchema
	{
		public int item4_discount_rate;

		public int item4_rate;

		public ShopGroup_item4_val(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_item4_val()
			: base(3847198470u, "ShopGroup_item4_val")
		{
		}

		public void CopyTo(ShopGroup_item4_val dest)
		{
			dest.item4_discount_rate = item4_discount_rate;
			dest.item4_rate = item4_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item4_discount_rate) + Serializer.GetLength(item4_rate);
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
			Serializer.Load(br, ref item4_discount_rate);
			Serializer.Load(br, ref item4_rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item4_discount_rate);
			Serializer.Save(bw, item4_rate);
		}

		public bool Equal(ShopGroup_item4_val comp)
		{
			if (item4_discount_rate != comp.item4_discount_rate)
			{
				return false;
			}
			if (item4_rate != comp.item4_rate)
			{
				return false;
			}
			return true;
		}

		public ShopGroup_item4_val Clone()
		{
			ShopGroup_item4_val shopGroup_item4_val = new ShopGroup_item4_val();
			CopyTo(shopGroup_item4_val);
			return shopGroup_item4_val;
		}

		public override void Clean()
		{
			item4_discount_rate = 0;
			item4_rate = 0;
		}
	}
}
