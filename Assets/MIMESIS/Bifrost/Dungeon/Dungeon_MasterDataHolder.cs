using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.Dungeon
{
	public class Dungeon_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<Dungeon_MasterData> dataHolder = new List<Dungeon_MasterData>();

		public Dungeon_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Dungeon_MasterDataHolder()
			: base(3161929129u, "Dungeon_MasterDataHolder")
		{
		}

		public void CopyTo(Dungeon_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (Dungeon_MasterData item in dataHolder)
			{
				Dungeon_MasterData dungeon_MasterData = new Dungeon_MasterData();
				item.CopyTo(dungeon_MasterData);
				dest.dataHolder.Add(dungeon_MasterData);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(versionInfo);
			num += 4;
			foreach (Dungeon_MasterData item in dataHolder)
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
			Serializer.Load(br, ref versionInfo);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				Dungeon_MasterData dungeon_MasterData = new Dungeon_MasterData();
				dungeon_MasterData.LoadInternal(br);
				dataHolder.Add(dungeon_MasterData);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, versionInfo);
			Serializer.Save(bw, dataHolder.Count);
			foreach (Dungeon_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(Dungeon_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (Dungeon_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (Dungeon_MasterData item2 in comp.dataHolder)
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

		public Dungeon_MasterDataHolder Clone()
		{
			Dungeon_MasterDataHolder dungeon_MasterDataHolder = new Dungeon_MasterDataHolder();
			CopyTo(dungeon_MasterDataHolder);
			return dungeon_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
