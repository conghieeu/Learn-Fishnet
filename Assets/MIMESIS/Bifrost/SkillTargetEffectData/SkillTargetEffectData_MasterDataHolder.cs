using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.SkillTargetEffectData
{
	public class SkillTargetEffectData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<SkillTargetEffectData_MasterData> dataHolder = new List<SkillTargetEffectData_MasterData>();

		public SkillTargetEffectData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public SkillTargetEffectData_MasterDataHolder()
			: base(2396513727u, "SkillTargetEffectData_MasterDataHolder")
		{
		}

		public void CopyTo(SkillTargetEffectData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (SkillTargetEffectData_MasterData item in dataHolder)
			{
				SkillTargetEffectData_MasterData skillTargetEffectData_MasterData = new SkillTargetEffectData_MasterData();
				item.CopyTo(skillTargetEffectData_MasterData);
				dest.dataHolder.Add(skillTargetEffectData_MasterData);
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
			foreach (SkillTargetEffectData_MasterData item in dataHolder)
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
				SkillTargetEffectData_MasterData skillTargetEffectData_MasterData = new SkillTargetEffectData_MasterData();
				skillTargetEffectData_MasterData.LoadInternal(br);
				dataHolder.Add(skillTargetEffectData_MasterData);
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
			foreach (SkillTargetEffectData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(SkillTargetEffectData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (SkillTargetEffectData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (SkillTargetEffectData_MasterData item2 in comp.dataHolder)
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

		public SkillTargetEffectData_MasterDataHolder Clone()
		{
			SkillTargetEffectData_MasterDataHolder skillTargetEffectData_MasterDataHolder = new SkillTargetEffectData_MasterDataHolder();
			CopyTo(skillTargetEffectData_MasterDataHolder);
			return skillTargetEffectData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
