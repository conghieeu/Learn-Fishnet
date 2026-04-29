using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.CharacterSkillSequenceData
{
	public class CharacterSkillSequenceData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<CharacterSkillSequenceData_MasterData> dataHolder = new List<CharacterSkillSequenceData_MasterData>();

		public CharacterSkillSequenceData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public CharacterSkillSequenceData_MasterDataHolder()
			: base(1167027372u, "CharacterSkillSequenceData_MasterDataHolder")
		{
		}

		public void CopyTo(CharacterSkillSequenceData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (CharacterSkillSequenceData_MasterData item in dataHolder)
			{
				CharacterSkillSequenceData_MasterData characterSkillSequenceData_MasterData = new CharacterSkillSequenceData_MasterData();
				item.CopyTo(characterSkillSequenceData_MasterData);
				dest.dataHolder.Add(characterSkillSequenceData_MasterData);
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
			foreach (CharacterSkillSequenceData_MasterData item in dataHolder)
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
				CharacterSkillSequenceData_MasterData characterSkillSequenceData_MasterData = new CharacterSkillSequenceData_MasterData();
				characterSkillSequenceData_MasterData.LoadInternal(br);
				dataHolder.Add(characterSkillSequenceData_MasterData);
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
			foreach (CharacterSkillSequenceData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(CharacterSkillSequenceData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (CharacterSkillSequenceData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (CharacterSkillSequenceData_MasterData item2 in comp.dataHolder)
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

		public CharacterSkillSequenceData_MasterDataHolder Clone()
		{
			CharacterSkillSequenceData_MasterDataHolder characterSkillSequenceData_MasterDataHolder = new CharacterSkillSequenceData_MasterDataHolder();
			CopyTo(characterSkillSequenceData_MasterDataHolder);
			return characterSkillSequenceData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
