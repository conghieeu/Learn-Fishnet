using System;
using System.IO;
using System.Net;

namespace Bifrost.Interactor
{
	public class Interactor_state : ISchema
	{
		public string state_name = string.Empty;

		public string state_desc = string.Empty;

		public string state_mesh_name = string.Empty;

		public bool is_end_state;

		public string end_state_type = string.Empty;

		public int end_state_waiting_time;

		public string auto_transition_target_state = string.Empty;

		public Interactor_state(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Interactor_state()
			: base(560117539u, "Interactor_state")
		{
		}

		public void CopyTo(Interactor_state dest)
		{
			dest.state_name = state_name;
			dest.state_desc = state_desc;
			dest.state_mesh_name = state_mesh_name;
			dest.is_end_state = is_end_state;
			dest.end_state_type = end_state_type;
			dest.end_state_waiting_time = end_state_waiting_time;
			dest.auto_transition_target_state = auto_transition_target_state;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(state_name) + Serializer.GetLength(state_desc) + Serializer.GetLength(state_mesh_name) + Serializer.GetLength(is_end_state) + Serializer.GetLength(end_state_type) + Serializer.GetLength(end_state_waiting_time) + Serializer.GetLength(auto_transition_target_state);
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
			Serializer.Load(br, ref state_name);
			Serializer.Load(br, ref state_desc);
			Serializer.Load(br, ref state_mesh_name);
			Serializer.Load(br, ref is_end_state);
			Serializer.Load(br, ref end_state_type);
			Serializer.Load(br, ref end_state_waiting_time);
			Serializer.Load(br, ref auto_transition_target_state);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, state_name);
			Serializer.Save(bw, state_desc);
			Serializer.Save(bw, state_mesh_name);
			Serializer.Save(bw, is_end_state);
			Serializer.Save(bw, end_state_type);
			Serializer.Save(bw, end_state_waiting_time);
			Serializer.Save(bw, auto_transition_target_state);
		}

		public bool Equal(Interactor_state comp)
		{
			if (state_name != comp.state_name)
			{
				return false;
			}
			if (state_desc != comp.state_desc)
			{
				return false;
			}
			if (state_mesh_name != comp.state_mesh_name)
			{
				return false;
			}
			if (is_end_state != comp.is_end_state)
			{
				return false;
			}
			if (end_state_type != comp.end_state_type)
			{
				return false;
			}
			if (end_state_waiting_time != comp.end_state_waiting_time)
			{
				return false;
			}
			if (auto_transition_target_state != comp.auto_transition_target_state)
			{
				return false;
			}
			return true;
		}

		public Interactor_state Clone()
		{
			Interactor_state interactor_state = new Interactor_state();
			CopyTo(interactor_state);
			return interactor_state;
		}

		public override void Clean()
		{
			state_name = string.Empty;
			state_desc = string.Empty;
			state_mesh_name = string.Empty;
			is_end_state = false;
			end_state_type = string.Empty;
			end_state_waiting_time = 0;
			auto_transition_target_state = string.Empty;
		}
	}
}
