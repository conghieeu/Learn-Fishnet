using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.MonsterSkillSequenceData
{
	public class MonsterSkillSequenceData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<MonsterSkillSequenceData_MasterData> dataHolder = new List<MonsterSkillSequenceData_MasterData>();

		public MonsterSkillSequenceData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public MonsterSkillSequenceData_MasterDataHolder()
			: base(3514097952u, "MonsterSkillSequenceData_MasterDataHolder")
		{
		}

		public void CopyTo(MonsterSkillSequenceData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (MonsterSkillSequenceData_MasterData item in dataHolder)
			{
				MonsterSkillSequenceData_MasterData monsterSkillSequenceData_MasterData = new MonsterSkillSequenceData_MasterData();
				item.CopyTo(monsterSkillSequenceData_MasterData);
				dest.dataHolder.Add(monsterSkillSequenceData_MasterData);
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
			foreach (MonsterSkillSequenceData_MasterData item in dataHolder)
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
				MonsterSkillSequenceData_MasterData monsterSkillSequenceData_MasterData = new MonsterSkillSequenceData_MasterData();
				monsterSkillSequenceData_MasterData.LoadInternal(br);
				dataHolder.Add(monsterSkillSequenceData_MasterData);
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
			foreach (MonsterSkillSequenceData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(MonsterSkillSequenceData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (MonsterSkillSequenceData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (MonsterSkillSequenceData_MasterData item2 in comp.dataHolder)
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

		public MonsterSkillSequenceData_MasterDataHolder Clone()
		{
			MonsterSkillSequenceData_MasterDataHolder monsterSkillSequenceData_MasterDataHolder = new MonsterSkillSequenceData_MasterDataHolder();
			CopyTo(monsterSkillSequenceData_MasterDataHolder);
			return monsterSkillSequenceData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
