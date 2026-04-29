using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.AuraSkillData
{
	public class AuraSkillData_MasterData : ISchema
	{
		public int id;

		public bool is_once_abnormal_on_aura;

		public string aura_prefab_name = string.Empty;

		public long aura_prefab_duration;

		public string hit_box_shape = string.Empty;

		public List<float> aura_offset = new List<float>();

		public long aura_duration;

		public List<int> target_type = new List<int>();

		public int abnormal_master_id;

		public int link_skill_target_effect_data_id;

		public AuraSkillData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public AuraSkillData_MasterData()
			: base(414507588u, "AuraSkillData_MasterData")
		{
		}

		public void CopyTo(AuraSkillData_MasterData dest)
		{
			dest.id = id;
			dest.is_once_abnormal_on_aura = is_once_abnormal_on_aura;
			dest.aura_prefab_name = aura_prefab_name;
			dest.aura_prefab_duration = aura_prefab_duration;
			dest.hit_box_shape = hit_box_shape;
			dest.aura_offset.Clear();
			foreach (float item in aura_offset)
			{
				dest.aura_offset.Add(item);
			}
			dest.aura_duration = aura_duration;
			dest.target_type.Clear();
			foreach (int item2 in target_type)
			{
				dest.target_type.Add(item2);
			}
			dest.abnormal_master_id = abnormal_master_id;
			dest.link_skill_target_effect_data_id = link_skill_target_effect_data_id;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += Serializer.GetLength(is_once_abnormal_on_aura);
			num += Serializer.GetLength(aura_prefab_name);
			num += Serializer.GetLength(aura_prefab_duration);
			num += Serializer.GetLength(hit_box_shape);
			num += 4;
			foreach (float item in aura_offset)
			{
				num += Serializer.GetLength(item);
			}
			num += Serializer.GetLength(aura_duration);
			num += 4;
			foreach (int item2 in target_type)
			{
				num += Serializer.GetLength(item2);
			}
			num += Serializer.GetLength(abnormal_master_id);
			return num + Serializer.GetLength(link_skill_target_effect_data_id);
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
			Serializer.Load(br, ref is_once_abnormal_on_aura);
			Serializer.Load(br, ref aura_prefab_name);
			Serializer.Load(br, ref aura_prefab_duration);
			Serializer.Load(br, ref hit_box_shape);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				float nValue = 0f;
				Serializer.Load(br, ref nValue);
				aura_offset.Add(nValue);
			}
			Serializer.Load(br, ref aura_duration);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				int intValue3 = 0;
				Serializer.Load(br, ref intValue3);
				target_type.Add(intValue3);
			}
			Serializer.Load(br, ref abnormal_master_id);
			Serializer.Load(br, ref link_skill_target_effect_data_id);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, is_once_abnormal_on_aura);
			Serializer.Save(bw, aura_prefab_name);
			Serializer.Save(bw, aura_prefab_duration);
			Serializer.Save(bw, hit_box_shape);
			Serializer.Save(bw, aura_offset.Count);
			foreach (float item in aura_offset)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, aura_duration);
			Serializer.Save(bw, target_type.Count);
			foreach (int item2 in target_type)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, abnormal_master_id);
			Serializer.Save(bw, link_skill_target_effect_data_id);
		}

		public bool Equal(AuraSkillData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (is_once_abnormal_on_aura != comp.is_once_abnormal_on_aura)
			{
				return false;
			}
			if (aura_prefab_name != comp.aura_prefab_name)
			{
				return false;
			}
			if (aura_prefab_duration != comp.aura_prefab_duration)
			{
				return false;
			}
			if (hit_box_shape != comp.hit_box_shape)
			{
				return false;
			}
			if (comp.aura_offset.Count != aura_offset.Count)
			{
				return false;
			}
			foreach (float item in aura_offset)
			{
				if (!comp.aura_offset.Contains(item))
				{
					return false;
				}
			}
			if (aura_duration != comp.aura_duration)
			{
				return false;
			}
			if (comp.target_type.Count != target_type.Count)
			{
				return false;
			}
			foreach (int item2 in target_type)
			{
				if (!comp.target_type.Contains(item2))
				{
					return false;
				}
			}
			if (abnormal_master_id != comp.abnormal_master_id)
			{
				return false;
			}
			if (link_skill_target_effect_data_id != comp.link_skill_target_effect_data_id)
			{
				return false;
			}
			return true;
		}

		public AuraSkillData_MasterData Clone()
		{
			AuraSkillData_MasterData auraSkillData_MasterData = new AuraSkillData_MasterData();
			CopyTo(auraSkillData_MasterData);
			return auraSkillData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			is_once_abnormal_on_aura = false;
			aura_prefab_name = string.Empty;
			aura_prefab_duration = 0L;
			hit_box_shape = string.Empty;
			aura_offset.Clear();
			aura_duration = 0L;
			target_type.Clear();
			abnormal_master_id = 0;
			link_skill_target_effect_data_id = 0;
		}
	}
}
