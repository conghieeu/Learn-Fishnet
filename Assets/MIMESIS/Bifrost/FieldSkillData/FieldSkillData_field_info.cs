using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.FieldSkillData
{
	public class FieldSkillData_field_info : ISchema
	{
		public int field_id;

		public string field_prefab_name = string.Empty;

		public long field_prefab_duration;

		public List<string> decal_prefab_name = new List<string>();

		public List<long> decal_prefab_duration = new List<long>();

		public string hit_box_shape = string.Empty;

		public List<float> field_offset = new List<float>();

		public long field_duration;

		public long hit_tick;

		public long hit_start_delay;

		public bool is_hit_duplicatable;

		public int field_skill_sequence_id;

		public FieldSkillData_field_info(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public FieldSkillData_field_info()
			: base(2144066209u, "FieldSkillData_field_info")
		{
		}

		public void CopyTo(FieldSkillData_field_info dest)
		{
			dest.field_id = field_id;
			dest.field_prefab_name = field_prefab_name;
			dest.field_prefab_duration = field_prefab_duration;
			dest.decal_prefab_name.Clear();
			foreach (string item in decal_prefab_name)
			{
				dest.decal_prefab_name.Add(item);
			}
			dest.decal_prefab_duration.Clear();
			foreach (long item2 in decal_prefab_duration)
			{
				dest.decal_prefab_duration.Add(item2);
			}
			dest.hit_box_shape = hit_box_shape;
			dest.field_offset.Clear();
			foreach (float item3 in field_offset)
			{
				dest.field_offset.Add(item3);
			}
			dest.field_duration = field_duration;
			dest.hit_tick = hit_tick;
			dest.hit_start_delay = hit_start_delay;
			dest.is_hit_duplicatable = is_hit_duplicatable;
			dest.field_skill_sequence_id = field_skill_sequence_id;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(field_id);
			num += Serializer.GetLength(field_prefab_name);
			num += Serializer.GetLength(field_prefab_duration);
			num += 4;
			foreach (string item in decal_prefab_name)
			{
				num += Serializer.GetLength(item);
			}
			num += 4;
			foreach (long item2 in decal_prefab_duration)
			{
				num += Serializer.GetLength(item2);
			}
			num += Serializer.GetLength(hit_box_shape);
			num += 4;
			foreach (float item3 in field_offset)
			{
				num += Serializer.GetLength(item3);
			}
			num += Serializer.GetLength(field_duration);
			num += Serializer.GetLength(hit_tick);
			num += Serializer.GetLength(hit_start_delay);
			num += Serializer.GetLength(is_hit_duplicatable);
			return num + Serializer.GetLength(field_skill_sequence_id);
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
			Serializer.Load(br, ref field_id);
			Serializer.Load(br, ref field_prefab_name);
			Serializer.Load(br, ref field_prefab_duration);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				decal_prefab_name.Add(strValue);
			}
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				long nValue = 0L;
				Serializer.Load(br, ref nValue);
				decal_prefab_duration.Add(nValue);
			}
			Serializer.Load(br, ref hit_box_shape);
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				float nValue2 = 0f;
				Serializer.Load(br, ref nValue2);
				field_offset.Add(nValue2);
			}
			Serializer.Load(br, ref field_duration);
			Serializer.Load(br, ref hit_tick);
			Serializer.Load(br, ref hit_start_delay);
			Serializer.Load(br, ref is_hit_duplicatable);
			Serializer.Load(br, ref field_skill_sequence_id);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, field_id);
			Serializer.Save(bw, field_prefab_name);
			Serializer.Save(bw, field_prefab_duration);
			Serializer.Save(bw, decal_prefab_name.Count);
			foreach (string item in decal_prefab_name)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, decal_prefab_duration.Count);
			foreach (long item2 in decal_prefab_duration)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, hit_box_shape);
			Serializer.Save(bw, field_offset.Count);
			foreach (float item3 in field_offset)
			{
				Serializer.Save(bw, item3);
			}
			Serializer.Save(bw, field_duration);
			Serializer.Save(bw, hit_tick);
			Serializer.Save(bw, hit_start_delay);
			Serializer.Save(bw, is_hit_duplicatable);
			Serializer.Save(bw, field_skill_sequence_id);
		}

		public bool Equal(FieldSkillData_field_info comp)
		{
			if (field_id != comp.field_id)
			{
				return false;
			}
			if (field_prefab_name != comp.field_prefab_name)
			{
				return false;
			}
			if (field_prefab_duration != comp.field_prefab_duration)
			{
				return false;
			}
			if (comp.decal_prefab_name.Count != decal_prefab_name.Count)
			{
				return false;
			}
			foreach (string item in decal_prefab_name)
			{
				if (!comp.decal_prefab_name.Contains(item))
				{
					return false;
				}
			}
			if (comp.decal_prefab_duration.Count != decal_prefab_duration.Count)
			{
				return false;
			}
			foreach (long item2 in decal_prefab_duration)
			{
				if (!comp.decal_prefab_duration.Contains(item2))
				{
					return false;
				}
			}
			if (hit_box_shape != comp.hit_box_shape)
			{
				return false;
			}
			if (comp.field_offset.Count != field_offset.Count)
			{
				return false;
			}
			foreach (float item3 in field_offset)
			{
				if (!comp.field_offset.Contains(item3))
				{
					return false;
				}
			}
			if (field_duration != comp.field_duration)
			{
				return false;
			}
			if (hit_tick != comp.hit_tick)
			{
				return false;
			}
			if (hit_start_delay != comp.hit_start_delay)
			{
				return false;
			}
			if (is_hit_duplicatable != comp.is_hit_duplicatable)
			{
				return false;
			}
			if (field_skill_sequence_id != comp.field_skill_sequence_id)
			{
				return false;
			}
			return true;
		}

		public FieldSkillData_field_info Clone()
		{
			FieldSkillData_field_info fieldSkillData_field_info = new FieldSkillData_field_info();
			CopyTo(fieldSkillData_field_info);
			return fieldSkillData_field_info;
		}

		public override void Clean()
		{
			field_id = 0;
			field_prefab_name = string.Empty;
			field_prefab_duration = 0L;
			decal_prefab_name.Clear();
			decal_prefab_duration.Clear();
			hit_box_shape = string.Empty;
			field_offset.Clear();
			field_duration = 0L;
			hit_tick = 0L;
			hit_start_delay = 0L;
			is_hit_duplicatable = false;
			field_skill_sequence_id = 0;
		}
	}
}
