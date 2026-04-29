using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.Interactor
{
	public class Interactor_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public List<Interactor_state> Interactor_stateval = new List<Interactor_state>();

		public List<Interactor_transition> Interactor_transitionval = new List<Interactor_transition>();

		public List<Interactor_action> Interactor_actionval = new List<Interactor_action>();

		public Interactor_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Interactor_MasterData()
			: base(200793132u, "Interactor_MasterData")
		{
		}

		public void CopyTo(Interactor_MasterData dest)
		{
			dest.id = id;
			dest.name = name;
			dest.Interactor_stateval.Clear();
			foreach (Interactor_state item in Interactor_stateval)
			{
				Interactor_state interactor_state = new Interactor_state();
				item.CopyTo(interactor_state);
				dest.Interactor_stateval.Add(interactor_state);
			}
			dest.Interactor_transitionval.Clear();
			foreach (Interactor_transition item2 in Interactor_transitionval)
			{
				Interactor_transition interactor_transition = new Interactor_transition();
				item2.CopyTo(interactor_transition);
				dest.Interactor_transitionval.Add(interactor_transition);
			}
			dest.Interactor_actionval.Clear();
			foreach (Interactor_action item3 in Interactor_actionval)
			{
				Interactor_action interactor_action = new Interactor_action();
				item3.CopyTo(interactor_action);
				dest.Interactor_actionval.Add(interactor_action);
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
			num += 4;
			foreach (Interactor_state item in Interactor_stateval)
			{
				num += item.GetLengthInternal();
			}
			num += 4;
			foreach (Interactor_transition item2 in Interactor_transitionval)
			{
				num += item2.GetLengthInternal();
			}
			num += 4;
			foreach (Interactor_action item3 in Interactor_actionval)
			{
				num += item3.GetLengthInternal();
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
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				Interactor_state interactor_state = new Interactor_state();
				interactor_state.LoadInternal(br);
				Interactor_stateval.Add(interactor_state);
			}
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				Interactor_transition interactor_transition = new Interactor_transition();
				interactor_transition.LoadInternal(br);
				Interactor_transitionval.Add(interactor_transition);
			}
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				Interactor_action interactor_action = new Interactor_action();
				interactor_action.LoadInternal(br);
				Interactor_actionval.Add(interactor_action);
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
			Serializer.Save(bw, Interactor_stateval.Count);
			foreach (Interactor_state item in Interactor_stateval)
			{
				item.SaveInternal(bw);
			}
			Serializer.Save(bw, Interactor_transitionval.Count);
			foreach (Interactor_transition item2 in Interactor_transitionval)
			{
				item2.SaveInternal(bw);
			}
			Serializer.Save(bw, Interactor_actionval.Count);
			foreach (Interactor_action item3 in Interactor_actionval)
			{
				item3.SaveInternal(bw);
			}
		}

		public bool Equal(Interactor_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (comp.Interactor_stateval.Count != Interactor_stateval.Count)
			{
				return false;
			}
			foreach (Interactor_state item in Interactor_stateval)
			{
				bool flag = false;
				foreach (Interactor_state item2 in comp.Interactor_stateval)
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
			if (comp.Interactor_transitionval.Count != Interactor_transitionval.Count)
			{
				return false;
			}
			foreach (Interactor_transition item3 in Interactor_transitionval)
			{
				bool flag2 = false;
				foreach (Interactor_transition item4 in comp.Interactor_transitionval)
				{
					if (item3.Equal(item4))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					return false;
				}
			}
			if (comp.Interactor_actionval.Count != Interactor_actionval.Count)
			{
				return false;
			}
			foreach (Interactor_action item5 in Interactor_actionval)
			{
				bool flag3 = false;
				foreach (Interactor_action item6 in comp.Interactor_actionval)
				{
					if (item5.Equal(item6))
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					return false;
				}
			}
			return true;
		}

		public Interactor_MasterData Clone()
		{
			Interactor_MasterData interactor_MasterData = new Interactor_MasterData();
			CopyTo(interactor_MasterData);
			return interactor_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			Interactor_stateval.Clear();
			Interactor_transitionval.Clear();
			Interactor_actionval.Clear();
		}
	}
}
