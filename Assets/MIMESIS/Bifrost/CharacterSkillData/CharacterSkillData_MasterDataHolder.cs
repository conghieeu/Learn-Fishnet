using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.CharacterSkillData
{
	public class CharacterSkillData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<CharacterSkillData_MasterData> dataHolder = new List<CharacterSkillData_MasterData>();

		public CharacterSkillData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public CharacterSkillData_MasterDataHolder()
			: base(2129783965u, "CharacterSkillData_MasterDataHolder")
		{
		}

		public void CopyTo(CharacterSkillData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (CharacterSkillData_MasterData item in dataHolder)
			{
				CharacterSkillData_MasterData characterSkillData_MasterData = new CharacterSkillData_MasterData();
				item.CopyTo(characterSkillData_MasterData);
				dest.dataHolder.Add(characterSkillData_MasterData);
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
			foreach (CharacterSkillData_MasterData item in dataHolder)
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
				CharacterSkillData_MasterData characterSkillData_MasterData = new CharacterSkillData_MasterData();
				characterSkillData_MasterData.LoadInternal(br);
				dataHolder.Add(characterSkillData_MasterData);
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
			foreach (CharacterSkillData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(CharacterSkillData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (CharacterSkillData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (CharacterSkillData_MasterData item2 in comp.dataHolder)
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

		public CharacterSkillData_MasterDataHolder Clone()
		{
			CharacterSkillData_MasterDataHolder characterSkillData_MasterDataHolder = new CharacterSkillData_MasterDataHolder();
			CopyTo(characterSkillData_MasterDataHolder);
			return characterSkillData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
