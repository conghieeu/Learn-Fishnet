using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.AbnormalData
{
	public class AbnormalData_element : ISchema
	{
		public int index;

		public int category;

		public string type = string.Empty;

		public string sub_type = string.Empty;

		public int activate_delay;

		public int apply_period_type;

		public int interval;

		public int modify_type;

		public int modify_value;

		public List<int> dot_val_per_tick = new List<int>();

		public List<int> target_abnormal_id = new List<int>();

		public int move_origin;

		public AbnormalData_element(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public AbnormalData_element()
			: base(2621074562u, "AbnormalData_element")
		{
		}

		public void CopyTo(AbnormalData_element dest)
		{
			dest.index = index;
			dest.category = category;
			dest.type = type;
			dest.sub_type = sub_type;
			dest.activate_delay = activate_delay;
			dest.apply_period_type = apply_period_type;
			dest.interval = interval;
			dest.modify_type = modify_type;
			dest.modify_value = modify_value;
			dest.dot_val_per_tick.Clear();
			foreach (int item in dot_val_per_tick)
			{
				dest.dot_val_per_tick.Add(item);
			}
			dest.target_abnormal_id.Clear();
			foreach (int item2 in target_abnormal_id)
			{
				dest.target_abnormal_id.Add(item2);
			}
			dest.move_origin = move_origin;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(index);
			num += Serializer.GetLength(category);
			num += Serializer.GetLength(type);
			num += Serializer.GetLength(sub_type);
			num += Serializer.GetLength(activate_delay);
			num += Serializer.GetLength(apply_period_type);
			num += Serializer.GetLength(interval);
			num += Serializer.GetLength(modify_type);
			num += Serializer.GetLength(modify_value);
			num += 4;
			foreach (int item in dot_val_per_tick)
			{
				num += Serializer.GetLength(item);
			}
			num += 4;
			foreach (int item2 in target_abnormal_id)
			{
				num += Serializer.GetLength(item2);
			}
			return num + Serializer.GetLength(move_origin);
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
			Serializer.Load(br, ref index);
			Serializer.Load(br, ref category);
			Serializer.Load(br, ref type);
			Serializer.Load(br, ref sub_type);
			Serializer.Load(br, ref activate_delay);
			Serializer.Load(br, ref apply_period_type);
			Serializer.Load(br, ref interval);
			Serializer.Load(br, ref modify_type);
			Serializer.Load(br, ref modify_value);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				int intValue2 = 0;
				Serializer.Load(br, ref intValue2);
				dot_val_per_tick.Add(intValue2);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				int intValue4 = 0;
				Serializer.Load(br, ref intValue4);
				target_abnormal_id.Add(intValue4);
			}
			Serializer.Load(br, ref move_origin);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, index);
			Serializer.Save(bw, category);
			Serializer.Save(bw, type);
			Serializer.Save(bw, sub_type);
			Serializer.Save(bw, activate_delay);
			Serializer.Save(bw, apply_period_type);
			Serializer.Save(bw, interval);
			Serializer.Save(bw, modify_type);
			Serializer.Save(bw, modify_value);
			Serializer.Save(bw, dot_val_per_tick.Count);
			foreach (int item in dot_val_per_tick)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, target_abnormal_id.Count);
			foreach (int item2 in target_abnormal_id)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, move_origin);
		}

		public bool Equal(AbnormalData_element comp)
		{
			if (index != comp.index)
			{
				return false;
			}
			if (category != comp.category)
			{
				return false;
			}
			if (type != comp.type)
			{
				return false;
			}
			if (sub_type != comp.sub_type)
			{
				return false;
			}
			if (activate_delay != comp.activate_delay)
			{
				return false;
			}
			if (apply_period_type != comp.apply_period_type)
			{
				return false;
			}
			if (interval != comp.interval)
			{
				return false;
			}
			if (modify_type != comp.modify_type)
			{
				return false;
			}
			if (modify_value != comp.modify_value)
			{
				return false;
			}
			if (comp.dot_val_per_tick.Count != dot_val_per_tick.Count)
			{
				return false;
			}
			foreach (int item in dot_val_per_tick)
			{
				if (!comp.dot_val_per_tick.Contains(item))
				{
					return false;
				}
			}
			if (comp.target_abnormal_id.Count != target_abnormal_id.Count)
			{
				return false;
			}
			foreach (int item2 in target_abnormal_id)
			{
				if (!comp.target_abnormal_id.Contains(item2))
				{
					return false;
				}
			}
			if (move_origin != comp.move_origin)
			{
				return false;
			}
			return true;
		}

		public AbnormalData_element Clone()
		{
			AbnormalData_element abnormalData_element = new AbnormalData_element();
			CopyTo(abnormalData_element);
			return abnormalData_element;
		}

		public override void Clean()
		{
			index = 0;
			category = 0;
			type = string.Empty;
			sub_type = string.Empty;
			activate_delay = 0;
			apply_period_type = 0;
			interval = 0;
			modify_type = 0;
			modify_value = 0;
			dot_val_per_tick.Clear();
			target_abnormal_id.Clear();
			move_origin = 0;
		}
	}
}
