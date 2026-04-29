using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.Interactor
{
	public class Interactor_transition : ISchema
	{
		public string from_state = string.Empty;

		public List<string> action_name = new List<string>();

		public string to_state = string.Empty;

		public int gather_point;

		public List<string> action_on_transition = new List<string>();

		public string animation_name = string.Empty;

		public Interactor_transition(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Interactor_transition()
			: base(4038215479u, "Interactor_transition")
		{
		}

		public void CopyTo(Interactor_transition dest)
		{
			dest.from_state = from_state;
			dest.action_name.Clear();
			foreach (string item in action_name)
			{
				dest.action_name.Add(item);
			}
			dest.to_state = to_state;
			dest.gather_point = gather_point;
			dest.action_on_transition.Clear();
			foreach (string item2 in action_on_transition)
			{
				dest.action_on_transition.Add(item2);
			}
			dest.animation_name = animation_name;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(from_state);
			num += 4;
			foreach (string item in action_name)
			{
				num += Serializer.GetLength(item);
			}
			num += Serializer.GetLength(to_state);
			num += Serializer.GetLength(gather_point);
			num += 4;
			foreach (string item2 in action_on_transition)
			{
				num += Serializer.GetLength(item2);
			}
			return num + Serializer.GetLength(animation_name);
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
			Serializer.Load(br, ref from_state);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				action_name.Add(strValue);
			}
			Serializer.Load(br, ref to_state);
			Serializer.Load(br, ref gather_point);
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				action_on_transition.Add(strValue2);
			}
			Serializer.Load(br, ref animation_name);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, from_state);
			Serializer.Save(bw, action_name.Count);
			foreach (string item in action_name)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, to_state);
			Serializer.Save(bw, gather_point);
			Serializer.Save(bw, action_on_transition.Count);
			foreach (string item2 in action_on_transition)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, animation_name);
		}

		public bool Equal(Interactor_transition comp)
		{
			if (from_state != comp.from_state)
			{
				return false;
			}
			if (comp.action_name.Count != action_name.Count)
			{
				return false;
			}
			foreach (string item in action_name)
			{
				if (!comp.action_name.Contains(item))
				{
					return false;
				}
			}
			if (to_state != comp.to_state)
			{
				return false;
			}
			if (gather_point != comp.gather_point)
			{
				return false;
			}
			if (comp.action_on_transition.Count != action_on_transition.Count)
			{
				return false;
			}
			foreach (string item2 in action_on_transition)
			{
				if (!comp.action_on_transition.Contains(item2))
				{
					return false;
				}
			}
			if (animation_name != comp.animation_name)
			{
				return false;
			}
			return true;
		}

		public Interactor_transition Clone()
		{
			Interactor_transition interactor_transition = new Interactor_transition();
			CopyTo(interactor_transition);
			return interactor_transition;
		}

		public override void Clean()
		{
			from_state = string.Empty;
			action_name.Clear();
			to_state = string.Empty;
			gather_point = 0;
			action_on_transition.Clear();
			animation_name = string.Empty;
		}
	}
}
