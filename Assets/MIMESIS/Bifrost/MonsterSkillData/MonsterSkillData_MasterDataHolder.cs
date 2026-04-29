using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.MonsterSkillData
{
	public class MonsterSkillData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<MonsterSkillData_MasterData> dataHolder = new List<MonsterSkillData_MasterData>();

		public MonsterSkillData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public MonsterSkillData_MasterDataHolder()
			: base(3348177512u, "MonsterSkillData_MasterDataHolder")
		{
		}

		public void CopyTo(MonsterSkillData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (MonsterSkillData_MasterData item in dataHolder)
			{
				MonsterSkillData_MasterData monsterSkillData_MasterData = new MonsterSkillData_MasterData();
				item.CopyTo(monsterSkillData_MasterData);
				dest.dataHolder.Add(monsterSkillData_MasterData);
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
			foreach (MonsterSkillData_MasterData item in dataHolder)
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
				MonsterSkillData_MasterData monsterSkillData_MasterData = new MonsterSkillData_MasterData();
				monsterSkillData_MasterData.LoadInternal(br);
				dataHolder.Add(monsterSkillData_MasterData);
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
			foreach (MonsterSkillData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(MonsterSkillData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (MonsterSkillData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (MonsterSkillData_MasterData item2 in comp.dataHolder)
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

		public MonsterSkillData_MasterDataHolder Clone()
		{
			MonsterSkillData_MasterDataHolder monsterSkillData_MasterDataHolder = new MonsterSkillData_MasterDataHolder();
			CopyTo(monsterSkillData_MasterDataHolder);
			return monsterSkillData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
