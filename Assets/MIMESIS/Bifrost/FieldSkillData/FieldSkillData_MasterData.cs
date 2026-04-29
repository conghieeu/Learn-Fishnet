using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.FieldSkillData
{
	public class FieldSkillData_MasterData : ISchema
	{
		public int id;

		public List<int> factions = new List<int>();

		public int field_count;

		public List<FieldSkillData_field_info> FieldSkillData_field_infoval = new List<FieldSkillData_field_info>();

		public FieldSkillData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public FieldSkillData_MasterData()
			: base(16994666u, "FieldSkillData_MasterData")
		{
		}

		public void CopyTo(FieldSkillData_MasterData dest)
		{
			dest.id = id;
			dest.factions.Clear();
			foreach (int faction in factions)
			{
				dest.factions.Add(faction);
			}
			dest.field_count = field_count;
			dest.FieldSkillData_field_infoval.Clear();
			foreach (FieldSkillData_field_info item in FieldSkillData_field_infoval)
			{
				FieldSkillData_field_info fieldSkillData_field_info = new FieldSkillData_field_info();
				item.CopyTo(fieldSkillData_field_info);
				dest.FieldSkillData_field_infoval.Add(fieldSkillData_field_info);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += 4;
			foreach (int faction in factions)
			{
				num += Serializer.GetLength(faction);
			}
			num += Serializer.GetLength(field_count);
			num += 4;
			foreach (FieldSkillData_field_info item in FieldSkillData_field_infoval)
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
			Serializer.Load(br, ref id);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				int intValue2 = 0;
				Serializer.Load(br, ref intValue2);
				factions.Add(intValue2);
			}
			Serializer.Load(br, ref field_count);
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				FieldSkillData_field_info fieldSkillData_field_info = new FieldSkillData_field_info();
				fieldSkillData_field_info.LoadInternal(br);
				FieldSkillData_field_infoval.Add(fieldSkillData_field_info);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, factions.Count);
			foreach (int faction in factions)
			{
				Serializer.Save(bw, faction);
			}
			Serializer.Save(bw, field_count);
			Serializer.Save(bw, FieldSkillData_field_infoval.Count);
			foreach (FieldSkillData_field_info item in FieldSkillData_field_infoval)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(FieldSkillData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (comp.factions.Count != factions.Count)
			{
				return false;
			}
			foreach (int faction in factions)
			{
				if (!comp.factions.Contains(faction))
				{
					return false;
				}
			}
			if (field_count != comp.field_count)
			{
				return false;
			}
			if (comp.FieldSkillData_field_infoval.Count != FieldSkillData_field_infoval.Count)
			{
				return false;
			}
			foreach (FieldSkillData_field_info item in FieldSkillData_field_infoval)
			{
				bool flag = false;
				foreach (FieldSkillData_field_info item2 in comp.FieldSkillData_field_infoval)
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

		public FieldSkillData_MasterData Clone()
		{
			FieldSkillData_MasterData fieldSkillData_MasterData = new FieldSkillData_MasterData();
			CopyTo(fieldSkillData_MasterData);
			return fieldSkillData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			factions.Clear();
			field_count = 0;
			FieldSkillData_field_infoval.Clear();
		}
	}
}
