using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.SpawnableMiscGroup
{
	public class SpawnableMiscGroup_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<SpawnableMiscGroup_MasterData> dataHolder = new List<SpawnableMiscGroup_MasterData>();

		public SpawnableMiscGroup_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public SpawnableMiscGroup_MasterDataHolder()
			: base(4032176455u, "SpawnableMiscGroup_MasterDataHolder")
		{
		}

		public void CopyTo(SpawnableMiscGroup_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (SpawnableMiscGroup_MasterData item in dataHolder)
			{
				SpawnableMiscGroup_MasterData spawnableMiscGroup_MasterData = new SpawnableMiscGroup_MasterData();
				item.CopyTo(spawnableMiscGroup_MasterData);
				dest.dataHolder.Add(spawnableMiscGroup_MasterData);
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
			foreach (SpawnableMiscGroup_MasterData item in dataHolder)
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
				SpawnableMiscGroup_MasterData spawnableMiscGroup_MasterData = new SpawnableMiscGroup_MasterData();
				spawnableMiscGroup_MasterData.LoadInternal(br);
				dataHolder.Add(spawnableMiscGroup_MasterData);
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
			foreach (SpawnableMiscGroup_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(SpawnableMiscGroup_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (SpawnableMiscGroup_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (SpawnableMiscGroup_MasterData item2 in comp.dataHolder)
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

		public SpawnableMiscGroup_MasterDataHolder Clone()
		{
			SpawnableMiscGroup_MasterDataHolder spawnableMiscGroup_MasterDataHolder = new SpawnableMiscGroup_MasterDataHolder();
			CopyTo(spawnableMiscGroup_MasterDataHolder);
			return spawnableMiscGroup_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
