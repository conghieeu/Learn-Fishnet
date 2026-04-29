using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ItemDrop
{
	public class ItemDrop_MasterData : ISchema
	{
		public int id;

		public int drop_count_min;

		public int drop_count_max;

		public List<ItemDrop_candidate> ItemDrop_candidateval = new List<ItemDrop_candidate>();

		public ItemDrop_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemDrop_MasterData()
			: base(3534260431u, "ItemDrop_MasterData")
		{
		}

		public void CopyTo(ItemDrop_MasterData dest)
		{
			dest.id = id;
			dest.drop_count_min = drop_count_min;
			dest.drop_count_max = drop_count_max;
			dest.ItemDrop_candidateval.Clear();
			foreach (ItemDrop_candidate item in ItemDrop_candidateval)
			{
				ItemDrop_candidate itemDrop_candidate = new ItemDrop_candidate();
				item.CopyTo(itemDrop_candidate);
				dest.ItemDrop_candidateval.Add(itemDrop_candidate);
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
			num += Serializer.GetLength(drop_count_min);
			num += Serializer.GetLength(drop_count_max);
			num += 4;
			foreach (ItemDrop_candidate item in ItemDrop_candidateval)
			{
				num += item.GetLengthInternal();
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
			Serializer.Load(br, ref drop_count_min);
			Serializer.Load(br, ref drop_count_max);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				ItemDrop_candidate itemDrop_candidate = new ItemDrop_candidate();
				itemDrop_candidate.LoadInternal(br);
				ItemDrop_candidateval.Add(itemDrop_candidate);
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
			Serializer.Save(bw, drop_count_min);
			Serializer.Save(bw, drop_count_max);
			Serializer.Save(bw, ItemDrop_candidateval.Count);
			foreach (ItemDrop_candidate item in ItemDrop_candidateval)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(ItemDrop_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (drop_count_min != comp.drop_count_min)
			{
				return false;
			}
			if (drop_count_max != comp.drop_count_max)
			{
				return false;
			}
			if (comp.ItemDrop_candidateval.Count != ItemDrop_candidateval.Count)
			{
				return false;
			}
			foreach (ItemDrop_candidate item in ItemDrop_candidateval)
			{
				bool flag = false;
				foreach (ItemDrop_candidate item2 in comp.ItemDrop_candidateval)
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
			return true;
		}

		public ItemDrop_MasterData Clone()
		{
			ItemDrop_MasterData itemDrop_MasterData = new ItemDrop_MasterData();
			CopyTo(itemDrop_MasterData);
			return itemDrop_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			drop_count_min = 0;
			drop_count_max = 0;
			ItemDrop_candidateval.Clear();
		}
	}
}
