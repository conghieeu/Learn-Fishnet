using System;
using System.IO;
using System.Net;

namespace Bifrost.ItemDrop
{
	public class ItemDrop_candidate : ISchema
	{
		public int item_id;

		public int rate;

		public ItemDrop_candidate(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ItemDrop_candidate()
			: base(2187838508u, "ItemDrop_candidate")
		{
		}

		public void CopyTo(ItemDrop_candidate dest)
		{
			dest.item_id = item_id;
			dest.rate = rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(item_id) + Serializer.GetLength(rate);
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
			Serializer.Load(br, ref item_id);
			Serializer.Load(br, ref rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, item_id);
			Serializer.Save(bw, rate);
		}

		public bool Equal(ItemDrop_candidate comp)
		{
			if (item_id != comp.item_id)
			{
				return false;
			}
			if (rate != comp.rate)
			{
				return false;
			}
			return true;
		}

		public ItemDrop_candidate Clone()
		{
			ItemDrop_candidate itemDrop_candidate = new ItemDrop_candidate();
			CopyTo(itemDrop_candidate);
			return itemDrop_candidate;
		}

		public override void Clean()
		{
			item_id = 0;
			rate = 0;
		}
	}
}
