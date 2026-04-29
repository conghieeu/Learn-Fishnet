using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.MonsterData
{
	public class MonsterData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<MonsterData_MasterData> dataHolder = new List<MonsterData_MasterData>();

		public MonsterData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public MonsterData_MasterDataHolder()
			: base(316353466u, "MonsterData_MasterDataHolder")
		{
		}

		public void CopyTo(MonsterData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (MonsterData_MasterData item in dataHolder)
			{
				MonsterData_MasterData monsterData_MasterData = new MonsterData_MasterData();
				item.CopyTo(monsterData_MasterData);
				dest.dataHolder.Add(monsterData_MasterData);
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
			foreach (MonsterData_MasterData item in dataHolder)
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
				MonsterData_MasterData monsterData_MasterData = new MonsterData_MasterData();
				monsterData_MasterData.LoadInternal(br);
				dataHolder.Add(monsterData_MasterData);
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
			foreach (MonsterData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(MonsterData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (MonsterData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (MonsterData_MasterData item2 in comp.dataHolder)
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

		public MonsterData_MasterDataHolder Clone()
		{
			MonsterData_MasterDataHolder monsterData_MasterDataHolder = new MonsterData_MasterDataHolder();
			CopyTo(monsterData_MasterDataHolder);
			return monsterData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
