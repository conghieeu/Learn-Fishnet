using System;
using System.IO;
using System.Net;

namespace Bifrost.Interactor
{
	public class Interactor_action : ISchema
	{
		public string action_name = string.Empty;

		public int point;

		public Interactor_action(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Interactor_action()
			: base(1955084361u, "Interactor_action")
		{
		}

		public void CopyTo(Interactor_action dest)
		{
			dest.action_name = action_name;
			dest.point = point;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(action_name) + Serializer.GetLength(point);
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
			Serializer.Load(br, ref action_name);
			Serializer.Load(br, ref point);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, action_name);
			Serializer.Save(bw, point);
		}

		public bool Equal(Interactor_action comp)
		{
			if (action_name != comp.action_name)
			{
				return false;
			}
			if (point != comp.point)
			{
				return false;
			}
			return true;
		}

		public Interactor_action Clone()
		{
			Interactor_action interactor_action = new Interactor_action();
			CopyTo(interactor_action);
			return interactor_action;
		}

		public override void Clean()
		{
			action_name = string.Empty;
			point = 0;
		}
	}
}
