using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.FieldSkillData
{
	public class FieldSkillData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<FieldSkillData_MasterData> dataHolder = new List<FieldSkillData_MasterData>();

		public FieldSkillData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public FieldSkillData_MasterDataHolder()
			: base(3111274461u, "FieldSkillData_MasterDataHolder")
		{
		}

		public void CopyTo(FieldSkillData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (FieldSkillData_MasterData item in dataHolder)
			{
				FieldSkillData_MasterData fieldSkillData_MasterData = new FieldSkillData_MasterData();
				item.CopyTo(fieldSkillData_MasterData);
				dest.dataHolder.Add(fieldSkillData_MasterData);
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
			foreach (FieldSkillData_MasterData item in dataHolder)
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
				FieldSkillData_MasterData fieldSkillData_MasterData = new FieldSkillData_MasterData();
				fieldSkillData_MasterData.LoadInternal(br);
				dataHolder.Add(fieldSkillData_MasterData);
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
			foreach (FieldSkillData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(FieldSkillData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (FieldSkillData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (FieldSkillData_MasterData item2 in comp.dataHolder)
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

		public FieldSkillData_MasterDataHolder Clone()
		{
			FieldSkillData_MasterDataHolder fieldSkillData_MasterDataHolder = new FieldSkillData_MasterDataHolder();
			CopyTo(fieldSkillData_MasterDataHolder);
			return fieldSkillData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
