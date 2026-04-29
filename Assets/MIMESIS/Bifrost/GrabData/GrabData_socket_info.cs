using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.GrabData
{
	public class GrabData_socket_info : ISchema
	{
		public int index;

		public string grab_socket_name = string.Empty;

		public string grab_target_attach_object_name = string.Empty;

		public string grab_socket_name_third_person = string.Empty;

		public string grab_target_attach_object_name_third_person = string.Empty;

		public List<float> grab_target_attach_object_position_offset = new List<float>();

		public List<float> grab_target_attach_object_rotation_offset = new List<float>();

		public List<float> grab_target_attach_object_scale_offset = new List<float>();

		public GrabData_socket_info(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public GrabData_socket_info()
			: base(1258775875u, "GrabData_socket_info")
		{
		}

		public void CopyTo(GrabData_socket_info dest)
		{
			dest.index = index;
			dest.grab_socket_name = grab_socket_name;
			dest.grab_target_attach_object_name = grab_target_attach_object_name;
			dest.grab_socket_name_third_person = grab_socket_name_third_person;
			dest.grab_target_attach_object_name_third_person = grab_target_attach_object_name_third_person;
			dest.grab_target_attach_object_position_offset.Clear();
			foreach (float item in grab_target_attach_object_position_offset)
			{
				dest.grab_target_attach_object_position_offset.Add(item);
			}
			dest.grab_target_attach_object_rotation_offset.Clear();
			foreach (float item2 in grab_target_attach_object_rotation_offset)
			{
				dest.grab_target_attach_object_rotation_offset.Add(item2);
			}
			dest.grab_target_attach_object_scale_offset.Clear();
			foreach (float item3 in grab_target_attach_object_scale_offset)
			{
				dest.grab_target_attach_object_scale_offset.Add(item3);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(index);
			num += Serializer.GetLength(grab_socket_name);
			num += Serializer.GetLength(grab_target_attach_object_name);
			num += Serializer.GetLength(grab_socket_name_third_person);
			num += Serializer.GetLength(grab_target_attach_object_name_third_person);
			num += 4;
			foreach (float item in grab_target_attach_object_position_offset)
			{
				num += Serializer.GetLength(item);
			}
			num += 4;
			foreach (float item2 in grab_target_attach_object_rotation_offset)
			{
				num += Serializer.GetLength(item2);
			}
			num += 4;
			foreach (float item3 in grab_target_attach_object_scale_offset)
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
			Serializer.Load(br, ref index);
			Serializer.Load(br, ref grab_socket_name);
			Serializer.Load(br, ref grab_target_attach_object_name);
			Serializer.Load(br, ref grab_socket_name_third_person);
			Serializer.Load(br, ref grab_target_attach_object_name_third_person);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				float nValue = 0f;
				Serializer.Load(br, ref nValue);
				grab_target_attach_object_position_offset.Add(nValue);
			}
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				float nValue2 = 0f;
				Serializer.Load(br, ref nValue2);
				grab_target_attach_object_rotation_offset.Add(nValue2);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				float nValue3 = 0f;
				Serializer.Load(br, ref nValue3);
				grab_target_attach_object_scale_offset.Add(nValue3);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, index);
			Serializer.Save(bw, grab_socket_name);
			Serializer.Save(bw, grab_target_attach_object_name);
			Serializer.Save(bw, grab_socket_name_third_person);
			Serializer.Save(bw, grab_target_attach_object_name_third_person);
			Serializer.Save(bw, grab_target_attach_object_position_offset.Count);
			foreach (float item in grab_target_attach_object_position_offset)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, grab_target_attach_object_rotation_offset.Count);
			foreach (float item2 in grab_target_attach_object_rotation_offset)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, grab_target_attach_object_scale_offset.Count);
			foreach (float item3 in grab_target_attach_object_scale_offset)
			{
				Serializer.Save(bw, item3);
			}
		}

		public bool Equal(GrabData_socket_info comp)
		{
			if (index != comp.index)
			{
				return false;
			}
			if (grab_socket_name != comp.grab_socket_name)
			{
				return false;
			}
			if (grab_target_attach_object_name != comp.grab_target_attach_object_name)
			{
				return false;
			}
			if (grab_socket_name_third_person != comp.grab_socket_name_third_person)
			{
				return false;
			}
			if (grab_target_attach_object_name_third_person != comp.grab_target_attach_object_name_third_person)
			{
				return false;
			}
			if (comp.grab_target_attach_object_position_offset.Count != grab_target_attach_object_position_offset.Count)
			{
				return false;
			}
			foreach (float item in grab_target_attach_object_position_offset)
			{
				if (!comp.grab_target_attach_object_position_offset.Contains(item))
				{
					return false;
				}
			}
			if (comp.grab_target_attach_object_rotation_offset.Count != grab_target_attach_object_rotation_offset.Count)
			{
				return false;
			}
			foreach (float item2 in grab_target_attach_object_rotation_offset)
			{
				if (!comp.grab_target_attach_object_rotation_offset.Contains(item2))
				{
					return false;
				}
			}
			if (comp.grab_target_attach_object_scale_offset.Count != grab_target_attach_object_scale_offset.Count)
			{
				return false;
			}
			foreach (float item3 in grab_target_attach_object_scale_offset)
			{
				if (!comp.grab_target_attach_object_scale_offset.Contains(item3))
				{
					return false;
				}
			}
			return true;
		}

		public GrabData_socket_info Clone()
		{
			GrabData_socket_info grabData_socket_info = new GrabData_socket_info();
			CopyTo(grabData_socket_info);
			return grabData_socket_info;
		}

		public override void Clean()
		{
			index = 0;
			grab_socket_name = string.Empty;
			grab_target_attach_object_name = string.Empty;
			grab_socket_name_third_person = string.Empty;
			grab_target_attach_object_name_third_person = string.Empty;
			grab_target_attach_object_position_offset.Clear();
			grab_target_attach_object_rotation_offset.Clear();
			grab_target_attach_object_scale_offset.Clear();
		}
	}
}
