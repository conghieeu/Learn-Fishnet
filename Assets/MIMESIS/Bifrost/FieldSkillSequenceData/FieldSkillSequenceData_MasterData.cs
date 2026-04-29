using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.FieldSkillSequenceData
{
	public class FieldSkillSequenceData_MasterData : ISchema
	{
		public int id;

		public string battle_action_key = string.Empty;

		public int move_origin;

		public int seq_type;

		public int hit_type;

		public List<int> target_type = new List<int>();

		public string mutable_stat_type = string.Empty;

		public int mutable_stat_modify_value;

		public int hit_count;

		public int abnormal_apply_type;

		public List<int> abnormal_master_id = new List<int>();

		public int grab_master_id;

		public int link_skill_target_effect_data_id;

		public List<float> rag_doll_external_force = new List<float>();

		public FieldSkillSequenceData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public FieldSkillSequenceData_MasterData()
			: base(191487248u, "FieldSkillSequenceData_MasterData")
		{
		}

		public void CopyTo(FieldSkillSequenceData_MasterData dest)
		{
			dest.id = id;
			dest.battle_action_key = battle_action_key;
			dest.move_origin = move_origin;
			dest.seq_type = seq_type;
			dest.hit_type = hit_type;
			dest.target_type.Clear();
			foreach (int item in target_type)
			{
				dest.target_type.Add(item);
			}
			dest.mutable_stat_type = mutable_stat_type;
			dest.mutable_stat_modify_value = mutable_stat_modify_value;
			dest.hit_count = hit_count;
			dest.abnormal_apply_type = abnormal_apply_type;
			dest.abnormal_master_id.Clear();
			foreach (int item2 in abnormal_master_id)
			{
				dest.abnormal_master_id.Add(item2);
			}
			dest.grab_master_id = grab_master_id;
			dest.link_skill_target_effect_data_id = link_skill_target_effect_data_id;
			dest.rag_doll_external_force.Clear();
			foreach (float item3 in rag_doll_external_force)
			{
				dest.rag_doll_external_force.Add(item3);
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
			num += Serializer.GetLength(battle_action_key);
			num += Serializer.GetLength(move_origin);
			num += Serializer.GetLength(seq_type);
			num += Serializer.GetLength(hit_type);
			num += 4;
			foreach (int item in target_type)
			{
				num += Serializer.GetLength(item);
			}
			num += Serializer.GetLength(mutable_stat_type);
			num += Serializer.GetLength(mutable_stat_modify_value);
			num += Serializer.GetLength(hit_count);
			num += Serializer.GetLength(abnormal_apply_type);
			num += 4;
			foreach (int item2 in abnormal_master_id)
			{
				num += Serializer.GetLength(item2);
			}
			num += Serializer.GetLength(grab_master_id);
			num += Serializer.GetLength(link_skill_target_effect_data_id);
			num += 4;
			foreach (float item3 in rag_doll_external_force)
			{
				num += Serializer.GetLength(item3);
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
			Serializer.Load(br, ref battle_action_key);
			Serializer.Load(br, ref move_origin);
			Serializer.Load(br, ref seq_type);
			Serializer.Load(br, ref hit_type);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				int intValue2 = 0;
				Serializer.Load(br, ref intValue2);
				target_type.Add(intValue2);
			}
			Serializer.Load(br, ref mutable_stat_type);
			Serializer.Load(br, ref mutable_stat_modify_value);
			Serializer.Load(br, ref hit_count);
			Serializer.Load(br, ref abnormal_apply_type);
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				int intValue4 = 0;
				Serializer.Load(br, ref intValue4);
				abnormal_master_id.Add(intValue4);
			}
			Serializer.Load(br, ref grab_master_id);
			Serializer.Load(br, ref link_skill_target_effect_data_id);
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				float nValue = 0f;
				Serializer.Load(br, ref nValue);
				rag_doll_external_force.Add(nValue);
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
			Serializer.Save(bw, battle_action_key);
			Serializer.Save(bw, move_origin);
			Serializer.Save(bw, seq_type);
			Serializer.Save(bw, hit_type);
			Serializer.Save(bw, target_type.Count);
			foreach (int item in target_type)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, mutable_stat_type);
			Serializer.Save(bw, mutable_stat_modify_value);
			Serializer.Save(bw, hit_count);
			Serializer.Save(bw, abnormal_apply_type);
			Serializer.Save(bw, abnormal_master_id.Count);
			foreach (int item2 in abnormal_master_id)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, grab_master_id);
			Serializer.Save(bw, link_skill_target_effect_data_id);
			Serializer.Save(bw, rag_doll_external_force.Count);
			foreach (float item3 in rag_doll_external_force)
			{
				Serializer.Save(bw, item3);
			}
		}

		public bool Equal(FieldSkillSequenceData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (battle_action_key != comp.battle_action_key)
			{
				return false;
			}
			if (move_origin != comp.move_origin)
			{
				return false;
			}
			if (seq_type != comp.seq_type)
			{
				return false;
			}
			if (hit_type != comp.hit_type)
			{
				return false;
			}
			if (comp.target_type.Count != target_type.Count)
			{
				return false;
			}
			foreach (int item in target_type)
			{
				if (!comp.target_type.Contains(item))
				{
					return false;
				}
			}
			if (mutable_stat_type != comp.mutable_stat_type)
			{
				return false;
			}
			if (mutable_stat_modify_value != comp.mutable_stat_modify_value)
			{
				return false;
			}
			if (hit_count != comp.hit_count)
			{
				return false;
			}
			if (abnormal_apply_type != comp.abnormal_apply_type)
			{
				return false;
			}
			if (comp.abnormal_master_id.Count != abnormal_master_id.Count)
			{
				return false;
			}
			foreach (int item2 in abnormal_master_id)
			{
				if (!comp.abnormal_master_id.Contains(item2))
				{
					return false;
				}
			}
			if (grab_master_id != comp.grab_master_id)
			{
				return false;
			}
			if (link_skill_target_effect_data_id != comp.link_skill_target_effect_data_id)
			{
				return false;
			}
			if (comp.rag_doll_external_force.Count != rag_doll_external_force.Count)
			{
				return false;
			}
			foreach (float item3 in rag_doll_external_force)
			{
				if (!comp.rag_doll_external_force.Contains(item3))
				{
					return false;
				}
			}
			return true;
		}

		public FieldSkillSequenceData_MasterData Clone()
		{
			FieldSkillSequenceData_MasterData fieldSkillSequenceData_MasterData = new FieldSkillSequenceData_MasterData();
			CopyTo(fieldSkillSequenceData_MasterData);
			return fieldSkillSequenceData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			battle_action_key = string.Empty;
			move_origin = 0;
			seq_type = 0;
			hit_type = 0;
			target_type.Clear();
			mutable_stat_type = string.Empty;
			mutable_stat_modify_value = 0;
			hit_count = 0;
			abnormal_apply_type = 0;
			abnormal_master_id.Clear();
			grab_master_id = 0;
			link_skill_target_effect_data_id = 0;
			rag_doll_external_force.Clear();
		}
	}
}
