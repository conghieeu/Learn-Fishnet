using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.FieldSkillSequenceData
{
	public class FieldSkillSequenceData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<FieldSkillSequenceData_MasterData> dataHolder = new List<FieldSkillSequenceData_MasterData>();

		public FieldSkillSequenceData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public FieldSkillSequenceData_MasterDataHolder()
			: base(3216584365u, "FieldSkillSequenceData_MasterDataHolder")
		{
		}

		public void CopyTo(FieldSkillSequenceData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (FieldSkillSequenceData_MasterData item in dataHolder)
			{
				FieldSkillSequenceData_MasterData fieldSkillSequenceData_MasterData = new FieldSkillSequenceData_MasterData();
				item.CopyTo(fieldSkillSequenceData_MasterData);
				dest.dataHolder.Add(fieldSkillSequenceData_MasterData);
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
			foreach (FieldSkillSequenceData_MasterData item in dataHolder)
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
				FieldSkillSequenceData_MasterData fieldSkillSequenceData_MasterData = new FieldSkillSequenceData_MasterData();
				fieldSkillSequenceData_MasterData.LoadInternal(br);
				dataHolder.Add(fieldSkillSequenceData_MasterData);
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
			foreach (FieldSkillSequenceData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(FieldSkillSequenceData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (FieldSkillSequenceData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (FieldSkillSequenceData_MasterData item2 in comp.dataHolder)
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

		public FieldSkillSequenceData_MasterDataHolder Clone()
		{
			FieldSkillSequenceData_MasterDataHolder fieldSkillSequenceData_MasterDataHolder = new FieldSkillSequenceData_MasterDataHolder();
			CopyTo(fieldSkillSequenceData_MasterDataHolder);
			return fieldSkillSequenceData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
