using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ShopGroup
{
	public class ShopGroup_MasterData : ISchema
	{
		public int id;

		public int item1_masterid;

		public int item1_price;

		public List<ShopGroup_item1_val> ShopGroup_item1_valval = new List<ShopGroup_item1_val>();

		public int item2_masterid;

		public int item2_price;

		public List<ShopGroup_item2_val> ShopGroup_item2_valval = new List<ShopGroup_item2_val>();

		public int item3_masterid;

		public int item3_price;

		public List<ShopGroup_item3_val> ShopGroup_item3_valval = new List<ShopGroup_item3_val>();

		public int item4_masterid;

		public int item4_price;

		public List<ShopGroup_item4_val> ShopGroup_item4_valval = new List<ShopGroup_item4_val>();

		public int item5_masterid;

		public int item5_price;

		public List<ShopGroup_item5_val> ShopGroup_item5_valval = new List<ShopGroup_item5_val>();

		public int item6_masterid;

		public int item6_price;

		public List<ShopGroup_item6_val> ShopGroup_item6_valval = new List<ShopGroup_item6_val>();

		public int item7_masterid;

		public int item7_price;

		public List<ShopGroup_item7_val> ShopGroup_item7_valval = new List<ShopGroup_item7_val>();

		public int item8_masterid;

		public int item8_price;

		public List<ShopGroup_item8_val> ShopGroup_item8_valval = new List<ShopGroup_item8_val>();

		public int item9_masterid;

		public int item9_price;

		public List<ShopGroup_item9_val> ShopGroup_item9_valval = new List<ShopGroup_item9_val>();

		public ShopGroup_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ShopGroup_MasterData()
			: base(948982857u, "ShopGroup_MasterData")
		{
		}

		public void CopyTo(ShopGroup_MasterData dest)
		{
			dest.id = id;
			dest.item1_masterid = item1_masterid;
			dest.item1_price = item1_price;
			dest.ShopGroup_item1_valval.Clear();
			foreach (ShopGroup_item1_val item in ShopGroup_item1_valval)
			{
				ShopGroup_item1_val shopGroup_item1_val = new ShopGroup_item1_val();
				item.CopyTo(shopGroup_item1_val);
				dest.ShopGroup_item1_valval.Add(shopGroup_item1_val);
			}
			dest.item2_masterid = item2_masterid;
			dest.item2_price = item2_price;
			dest.ShopGroup_item2_valval.Clear();
			foreach (ShopGroup_item2_val item2 in ShopGroup_item2_valval)
			{
				ShopGroup_item2_val shopGroup_item2_val = new ShopGroup_item2_val();
				item2.CopyTo(shopGroup_item2_val);
				dest.ShopGroup_item2_valval.Add(shopGroup_item2_val);
			}
			dest.item3_masterid = item3_masterid;
			dest.item3_price = item3_price;
			dest.ShopGroup_item3_valval.Clear();
			foreach (ShopGroup_item3_val item3 in ShopGroup_item3_valval)
			{
				ShopGroup_item3_val shopGroup_item3_val = new ShopGroup_item3_val();
				item3.CopyTo(shopGroup_item3_val);
				dest.ShopGroup_item3_valval.Add(shopGroup_item3_val);
			}
			dest.item4_masterid = item4_masterid;
			dest.item4_price = item4_price;
			dest.ShopGroup_item4_valval.Clear();
			foreach (ShopGroup_item4_val item4 in ShopGroup_item4_valval)
			{
				ShopGroup_item4_val shopGroup_item4_val = new ShopGroup_item4_val();
				item4.CopyTo(shopGroup_item4_val);
				dest.ShopGroup_item4_valval.Add(shopGroup_item4_val);
			}
			dest.item5_masterid = item5_masterid;
			dest.item5_price = item5_price;
			dest.ShopGroup_item5_valval.Clear();
			foreach (ShopGroup_item5_val item5 in ShopGroup_item5_valval)
			{
				ShopGroup_item5_val shopGroup_item5_val = new ShopGroup_item5_val();
				item5.CopyTo(shopGroup_item5_val);
				dest.ShopGroup_item5_valval.Add(shopGroup_item5_val);
			}
			dest.item6_masterid = item6_masterid;
			dest.item6_price = item6_price;
			dest.ShopGroup_item6_valval.Clear();
			foreach (ShopGroup_item6_val item6 in ShopGroup_item6_valval)
			{
				ShopGroup_item6_val shopGroup_item6_val = new ShopGroup_item6_val();
				item6.CopyTo(shopGroup_item6_val);
				dest.ShopGroup_item6_valval.Add(shopGroup_item6_val);
			}
			dest.item7_masterid = item7_masterid;
			dest.item7_price = item7_price;
			dest.ShopGroup_item7_valval.Clear();
			foreach (ShopGroup_item7_val item7 in ShopGroup_item7_valval)
			{
				ShopGroup_item7_val shopGroup_item7_val = new ShopGroup_item7_val();
				item7.CopyTo(shopGroup_item7_val);
				dest.ShopGroup_item7_valval.Add(shopGroup_item7_val);
			}
			dest.item8_masterid = item8_masterid;
			dest.item8_price = item8_price;
			dest.ShopGroup_item8_valval.Clear();
			foreach (ShopGroup_item8_val item8 in ShopGroup_item8_valval)
			{
				ShopGroup_item8_val shopGroup_item8_val = new ShopGroup_item8_val();
				item8.CopyTo(shopGroup_item8_val);
				dest.ShopGroup_item8_valval.Add(shopGroup_item8_val);
			}
			dest.item9_masterid = item9_masterid;
			dest.item9_price = item9_price;
			dest.ShopGroup_item9_valval.Clear();
			foreach (ShopGroup_item9_val item9 in ShopGroup_item9_valval)
			{
				ShopGroup_item9_val shopGroup_item9_val = new ShopGroup_item9_val();
				item9.CopyTo(shopGroup_item9_val);
				dest.ShopGroup_item9_valval.Add(shopGroup_item9_val);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += Serializer.GetLength(item1_masterid);
			num += Serializer.GetLength(item1_price);
			num += 4;
			foreach (ShopGroup_item1_val item in ShopGroup_item1_valval)
			{
				num += item.GetLengthInternal();
			}
			num += Serializer.GetLength(item2_masterid);
			num += Serializer.GetLength(item2_price);
			num += 4;
			foreach (ShopGroup_item2_val item2 in ShopGroup_item2_valval)
			{
				num += item2.GetLengthInternal();
			}
			num += Serializer.GetLength(item3_masterid);
			num += Serializer.GetLength(item3_price);
			num += 4;
			foreach (ShopGroup_item3_val item3 in ShopGroup_item3_valval)
			{
				num += item3.GetLengthInternal();
			}
			num += Serializer.GetLength(item4_masterid);
			num += Serializer.GetLength(item4_price);
			num += 4;
			foreach (ShopGroup_item4_val item4 in ShopGroup_item4_valval)
			{
				num += item4.GetLengthInternal();
			}
			num += Serializer.GetLength(item5_masterid);
			num += Serializer.GetLength(item5_price);
			num += 4;
			foreach (ShopGroup_item5_val item5 in ShopGroup_item5_valval)
			{
				num += item5.GetLengthInternal();
			}
			num += Serializer.GetLength(item6_masterid);
			num += Serializer.GetLength(item6_price);
			num += 4;
			foreach (ShopGroup_item6_val item6 in ShopGroup_item6_valval)
			{
				num += item6.GetLengthInternal();
			}
			num += Serializer.GetLength(item7_masterid);
			num += Serializer.GetLength(item7_price);
			num += 4;
			foreach (ShopGroup_item7_val item7 in ShopGroup_item7_valval)
			{
				num += item7.GetLengthInternal();
			}
			num += Serializer.GetLength(item8_masterid);
			num += Serializer.GetLength(item8_price);
			num += 4;
			foreach (ShopGroup_item8_val item8 in ShopGroup_item8_valval)
			{
				num += item8.GetLengthInternal();
			}
			num += Serializer.GetLength(item9_masterid);
			num += Serializer.GetLength(item9_price);
			num += 4;
			foreach (ShopGroup_item9_val item9 in ShopGroup_item9_valval)
			{
				num += item9.GetLengthInternal();
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
			Serializer.Load(br, ref id);
			Serializer.Load(br, ref item1_masterid);
			Serializer.Load(br, ref item1_price);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				ShopGroup_item1_val shopGroup_item1_val = new ShopGroup_item1_val();
				shopGroup_item1_val.LoadInternal(br);
				ShopGroup_item1_valval.Add(shopGroup_item1_val);
			}
			Serializer.Load(br, ref item2_masterid);
			Serializer.Load(br, ref item2_price);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				ShopGroup_item2_val shopGroup_item2_val = new ShopGroup_item2_val();
				shopGroup_item2_val.LoadInternal(br);
				ShopGroup_item2_valval.Add(shopGroup_item2_val);
			}
			Serializer.Load(br, ref item3_masterid);
			Serializer.Load(br, ref item3_price);
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				ShopGroup_item3_val shopGroup_item3_val = new ShopGroup_item3_val();
				shopGroup_item3_val.LoadInternal(br);
				ShopGroup_item3_valval.Add(shopGroup_item3_val);
			}
			Serializer.Load(br, ref item4_masterid);
			Serializer.Load(br, ref item4_price);
			int intValue4 = 0;
			Serializer.Load(br, ref intValue4);
			while (intValue4-- > 0)
			{
				ShopGroup_item4_val shopGroup_item4_val = new ShopGroup_item4_val();
				shopGroup_item4_val.LoadInternal(br);
				ShopGroup_item4_valval.Add(shopGroup_item4_val);
			}
			Serializer.Load(br, ref item5_masterid);
			Serializer.Load(br, ref item5_price);
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				ShopGroup_item5_val shopGroup_item5_val = new ShopGroup_item5_val();
				shopGroup_item5_val.LoadInternal(br);
				ShopGroup_item5_valval.Add(shopGroup_item5_val);
			}
			Serializer.Load(br, ref item6_masterid);
			Serializer.Load(br, ref item6_price);
			int intValue6 = 0;
			Serializer.Load(br, ref intValue6);
			while (intValue6-- > 0)
			{
				ShopGroup_item6_val shopGroup_item6_val = new ShopGroup_item6_val();
				shopGroup_item6_val.LoadInternal(br);
				ShopGroup_item6_valval.Add(shopGroup_item6_val);
			}
			Serializer.Load(br, ref item7_masterid);
			Serializer.Load(br, ref item7_price);
			int intValue7 = 0;
			Serializer.Load(br, ref intValue7);
			while (intValue7-- > 0)
			{
				ShopGroup_item7_val shopGroup_item7_val = new ShopGroup_item7_val();
				shopGroup_item7_val.LoadInternal(br);
				ShopGroup_item7_valval.Add(shopGroup_item7_val);
			}
			Serializer.Load(br, ref item8_masterid);
			Serializer.Load(br, ref item8_price);
			int intValue8 = 0;
			Serializer.Load(br, ref intValue8);
			while (intValue8-- > 0)
			{
				ShopGroup_item8_val shopGroup_item8_val = new ShopGroup_item8_val();
				shopGroup_item8_val.LoadInternal(br);
				ShopGroup_item8_valval.Add(shopGroup_item8_val);
			}
			Serializer.Load(br, ref item9_masterid);
			Serializer.Load(br, ref item9_price);
			int intValue9 = 0;
			Serializer.Load(br, ref intValue9);
			while (intValue9-- > 0)
			{
				ShopGroup_item9_val shopGroup_item9_val = new ShopGroup_item9_val();
				shopGroup_item9_val.LoadInternal(br);
				ShopGroup_item9_valval.Add(shopGroup_item9_val);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, item1_masterid);
			Serializer.Save(bw, item1_price);
			Serializer.Save(bw, ShopGroup_item1_valval.Count);
			foreach (ShopGroup_item1_val item in ShopGroup_item1_valval)
			{
				item.SaveInternal(bw);
			}
			Serializer.Save(bw, item2_masterid);
			Serializer.Save(bw, item2_price);
			Serializer.Save(bw, ShopGroup_item2_valval.Count);
			foreach (ShopGroup_item2_val item2 in ShopGroup_item2_valval)
			{
				item2.SaveInternal(bw);
			}
			Serializer.Save(bw, item3_masterid);
			Serializer.Save(bw, item3_price);
			Serializer.Save(bw, ShopGroup_item3_valval.Count);
			foreach (ShopGroup_item3_val item3 in ShopGroup_item3_valval)
			{
				item3.SaveInternal(bw);
			}
			Serializer.Save(bw, item4_masterid);
			Serializer.Save(bw, item4_price);
			Serializer.Save(bw, ShopGroup_item4_valval.Count);
			foreach (ShopGroup_item4_val item4 in ShopGroup_item4_valval)
			{
				item4.SaveInternal(bw);
			}
			Serializer.Save(bw, item5_masterid);
			Serializer.Save(bw, item5_price);
			Serializer.Save(bw, ShopGroup_item5_valval.Count);
			foreach (ShopGroup_item5_val item5 in ShopGroup_item5_valval)
			{
				item5.SaveInternal(bw);
			}
			Serializer.Save(bw, item6_masterid);
			Serializer.Save(bw, item6_price);
			Serializer.Save(bw, ShopGroup_item6_valval.Count);
			foreach (ShopGroup_item6_val item6 in ShopGroup_item6_valval)
			{
				item6.SaveInternal(bw);
			}
			Serializer.Save(bw, item7_masterid);
			Serializer.Save(bw, item7_price);
			Serializer.Save(bw, ShopGroup_item7_valval.Count);
			foreach (ShopGroup_item7_val item7 in ShopGroup_item7_valval)
			{
				item7.SaveInternal(bw);
			}
			Serializer.Save(bw, item8_masterid);
			Serializer.Save(bw, item8_price);
			Serializer.Save(bw, ShopGroup_item8_valval.Count);
			foreach (ShopGroup_item8_val item8 in ShopGroup_item8_valval)
			{
				item8.SaveInternal(bw);
			}
			Serializer.Save(bw, item9_masterid);
			Serializer.Save(bw, item9_price);
			Serializer.Save(bw, ShopGroup_item9_valval.Count);
			foreach (ShopGroup_item9_val item9 in ShopGroup_item9_valval)
			{
				item9.SaveInternal(bw);
			}
		}

		public bool Equal(ShopGroup_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (item1_masterid != comp.item1_masterid)
			{
				return false;
			}
			if (item1_price != comp.item1_price)
			{
				return false;
			}
			if (comp.ShopGroup_item1_valval.Count != ShopGroup_item1_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item1_val item in ShopGroup_item1_valval)
			{
				bool flag = false;
				foreach (ShopGroup_item1_val item2 in comp.ShopGroup_item1_valval)
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
			if (item2_masterid != comp.item2_masterid)
			{
				return false;
			}
			if (item2_price != comp.item2_price)
			{
				return false;
			}
			if (comp.ShopGroup_item2_valval.Count != ShopGroup_item2_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item2_val item3 in ShopGroup_item2_valval)
			{
				bool flag2 = false;
				foreach (ShopGroup_item2_val item4 in comp.ShopGroup_item2_valval)
				{
					if (item3.Equal(item4))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					return false;
				}
			}
			if (item3_masterid != comp.item3_masterid)
			{
				return false;
			}
			if (item3_price != comp.item3_price)
			{
				return false;
			}
			if (comp.ShopGroup_item3_valval.Count != ShopGroup_item3_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item3_val item5 in ShopGroup_item3_valval)
			{
				bool flag3 = false;
				foreach (ShopGroup_item3_val item6 in comp.ShopGroup_item3_valval)
				{
					if (item5.Equal(item6))
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					return false;
				}
			}
			if (item4_masterid != comp.item4_masterid)
			{
				return false;
			}
			if (item4_price != comp.item4_price)
			{
				return false;
			}
			if (comp.ShopGroup_item4_valval.Count != ShopGroup_item4_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item4_val item7 in ShopGroup_item4_valval)
			{
				bool flag4 = false;
				foreach (ShopGroup_item4_val item8 in comp.ShopGroup_item4_valval)
				{
					if (item7.Equal(item8))
					{
						flag4 = true;
						break;
					}
				}
				if (!flag4)
				{
					return false;
				}
			}
			if (item5_masterid != comp.item5_masterid)
			{
				return false;
			}
			if (item5_price != comp.item5_price)
			{
				return false;
			}
			if (comp.ShopGroup_item5_valval.Count != ShopGroup_item5_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item5_val item9 in ShopGroup_item5_valval)
			{
				bool flag5 = false;
				foreach (ShopGroup_item5_val item10 in comp.ShopGroup_item5_valval)
				{
					if (item9.Equal(item10))
					{
						flag5 = true;
						break;
					}
				}
				if (!flag5)
				{
					return false;
				}
			}
			if (item6_masterid != comp.item6_masterid)
			{
				return false;
			}
			if (item6_price != comp.item6_price)
			{
				return false;
			}
			if (comp.ShopGroup_item6_valval.Count != ShopGroup_item6_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item6_val item11 in ShopGroup_item6_valval)
			{
				bool flag6 = false;
				foreach (ShopGroup_item6_val item12 in comp.ShopGroup_item6_valval)
				{
					if (item11.Equal(item12))
					{
						flag6 = true;
						break;
					}
				}
				if (!flag6)
				{
					return false;
				}
			}
			if (item7_masterid != comp.item7_masterid)
			{
				return false;
			}
			if (item7_price != comp.item7_price)
			{
				return false;
			}
			if (comp.ShopGroup_item7_valval.Count != ShopGroup_item7_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item7_val item13 in ShopGroup_item7_valval)
			{
				bool flag7 = false;
				foreach (ShopGroup_item7_val item14 in comp.ShopGroup_item7_valval)
				{
					if (item13.Equal(item14))
					{
						flag7 = true;
						break;
					}
				}
				if (!flag7)
				{
					return false;
				}
			}
			if (item8_masterid != comp.item8_masterid)
			{
				return false;
			}
			if (item8_price != comp.item8_price)
			{
				return false;
			}
			if (comp.ShopGroup_item8_valval.Count != ShopGroup_item8_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item8_val item15 in ShopGroup_item8_valval)
			{
				bool flag8 = false;
				foreach (ShopGroup_item8_val item16 in comp.ShopGroup_item8_valval)
				{
					if (item15.Equal(item16))
					{
						flag8 = true;
						break;
					}
				}
				if (!flag8)
				{
					return false;
				}
			}
			if (item9_masterid != comp.item9_masterid)
			{
				return false;
			}
			if (item9_price != comp.item9_price)
			{
				return false;
			}
			if (comp.ShopGroup_item9_valval.Count != ShopGroup_item9_valval.Count)
			{
				return false;
			}
			foreach (ShopGroup_item9_val item17 in ShopGroup_item9_valval)
			{
				bool flag9 = false;
				foreach (ShopGroup_item9_val item18 in comp.ShopGroup_item9_valval)
				{
					if (item17.Equal(item18))
					{
						flag9 = true;
						break;
					}
				}
				if (!flag9)
				{
					return false;
				}
			}
			return true;
		}

		public ShopGroup_MasterData Clone()
		{
			ShopGroup_MasterData shopGroup_MasterData = new ShopGroup_MasterData();
			CopyTo(shopGroup_MasterData);
			return shopGroup_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			item1_masterid = 0;
			item1_price = 0;
			ShopGroup_item1_valval.Clear();
			item2_masterid = 0;
			item2_price = 0;
			ShopGroup_item2_valval.Clear();
			item3_masterid = 0;
			item3_price = 0;
			ShopGroup_item3_valval.Clear();
			item4_masterid = 0;
			item4_price = 0;
			ShopGroup_item4_valval.Clear();
			item5_masterid = 0;
			item5_price = 0;
			ShopGroup_item5_valval.Clear();
			item6_masterid = 0;
			item6_price = 0;
			ShopGroup_item6_valval.Clear();
			item7_masterid = 0;
			item7_price = 0;
			ShopGroup_item7_valval.Clear();
			item8_masterid = 0;
			item8_price = 0;
			ShopGroup_item8_valval.Clear();
			item9_masterid = 0;
			item9_price = 0;
			ShopGroup_item9_valval.Clear();
		}
	}
}
