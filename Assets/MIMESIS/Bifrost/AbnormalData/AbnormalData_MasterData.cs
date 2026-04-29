using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.AbnormalData
{
	public class AbnormalData_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public bool overlap;

		public bool dispelable;

		public int duration;

		public int chaining_abnormal_id;

		public List<AbnormalData_element> AbnormalData_elementval = new List<AbnormalData_element>();

		public int link_skill_target_effect_data_id;

		public List<float> rag_doll_external_force = new List<float>();

		public AbnormalData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public AbnormalData_MasterData()
			: base(3802821312u, "AbnormalData_MasterData")
		{
		}

		public void CopyTo(AbnormalData_MasterData dest)
		{
			dest.id = id;
			dest.name = name;
			dest.overlap = overlap;
			dest.dispelable = dispelable;
			dest.duration = duration;
			dest.chaining_abnormal_id = chaining_abnormal_id;
			dest.AbnormalData_elementval.Clear();
			foreach (AbnormalData_element item in AbnormalData_elementval)
			{
				AbnormalData_element abnormalData_element = new AbnormalData_element();
				item.CopyTo(abnormalData_element);
				dest.AbnormalData_elementval.Add(abnormalData_element);
			}
			dest.link_skill_target_effect_data_id = link_skill_target_effect_data_id;
			dest.rag_doll_external_force.Clear();
			foreach (float item2 in rag_doll_external_force)
			{
				dest.rag_doll_external_force.Add(item2);
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
			num += Serializer.GetLength(name);
			num += Serializer.GetLength(overlap);
			num += Serializer.GetLength(dispelable);
			num += Serializer.GetLength(duration);
			num += Serializer.GetLength(chaining_abnormal_id);
			num += 4;
			foreach (AbnormalData_element item in AbnormalData_elementval)
			{
				num += item.GetLengthInternal();
			}
			num += Serializer.GetLength(link_skill_target_effect_data_id);
			num += 4;
			foreach (float item2 in rag_doll_external_force)
			{
				num += Serializer.GetLength(item2);
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
			Serializer.Load(br, ref name);
			Serializer.Load(br, ref overlap);
			Serializer.Load(br, ref dispelable);
			Serializer.Load(br, ref duration);
			Serializer.Load(br, ref chaining_abnormal_id);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				AbnormalData_element abnormalData_element = new AbnormalData_element();
				abnormalData_element.LoadInternal(br);
				AbnormalData_elementval.Add(abnormalData_element);
			}
			Serializer.Load(br, ref link_skill_target_effect_data_id);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
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
			Serializer.Save(bw, name);
			Serializer.Save(bw, overlap);
			Serializer.Save(bw, dispelable);
			Serializer.Save(bw, duration);
			Serializer.Save(bw, chaining_abnormal_id);
			Serializer.Save(bw, AbnormalData_elementval.Count);
			foreach (AbnormalData_element item in AbnormalData_elementval)
			{
				item.SaveInternal(bw);
			}
			Serializer.Save(bw, link_skill_target_effect_data_id);
			Serializer.Save(bw, rag_doll_external_force.Count);
			foreach (float item2 in rag_doll_external_force)
			{
				Serializer.Save(bw, item2);
			}
		}

		public bool Equal(AbnormalData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (overlap != comp.overlap)
			{
				return false;
			}
			if (dispelable != comp.dispelable)
			{
				return false;
			}
			if (duration != comp.duration)
			{
				return false;
			}
			if (chaining_abnormal_id != comp.chaining_abnormal_id)
			{
				return false;
			}
			if (comp.AbnormalData_elementval.Count != AbnormalData_elementval.Count)
			{
				return false;
			}
			foreach (AbnormalData_element item in AbnormalData_elementval)
			{
				bool flag = false;
				foreach (AbnormalData_element item2 in comp.AbnormalData_elementval)
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

		public AbnormalData_MasterData Clone()
		{
			AbnormalData_MasterData abnormalData_MasterData = new AbnormalData_MasterData();
			CopyTo(abnormalData_MasterData);
			return abnormalData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			overlap = false;
			dispelable = false;
			duration = 0;
			chaining_abnormal_id = 0;
			AbnormalData_elementval.Clear();
			link_skill_target_effect_data_id = 0;
			rag_doll_external_force.Clear();
		}
	}
}
