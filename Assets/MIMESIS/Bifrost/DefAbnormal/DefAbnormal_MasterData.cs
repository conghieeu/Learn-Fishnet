using System;
using System.IO;
using System.Net;

namespace Bifrost.DefAbnormal
{
	public class DefAbnormal_MasterData : ISchema
	{
		public int id;

		public int category;

		public string key = string.Empty;

		public string name = string.Empty;

		public bool usable;

		public int motion_priority;

		public bool motion_cancel;

		public bool bt_pause;

		public bool unable_input;

		public bool unable_input_move;

		public bool unable_move;

		public bool unable_sound;

		public bool occur_detach;

		public bool drop_item;

		public DefAbnormal_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public DefAbnormal_MasterData()
			: base(3057642656u, "DefAbnormal_MasterData")
		{
		}

		public void CopyTo(DefAbnormal_MasterData dest)
		{
			dest.id = id;
			dest.category = category;
			dest.key = key;
			dest.name = name;
			dest.usable = usable;
			dest.motion_priority = motion_priority;
			dest.motion_cancel = motion_cancel;
			dest.bt_pause = bt_pause;
			dest.unable_input = unable_input;
			dest.unable_input_move = unable_input_move;
			dest.unable_move = unable_move;
			dest.unable_sound = unable_sound;
			dest.occur_detach = occur_detach;
			dest.drop_item = drop_item;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(category) + Serializer.GetLength(key) + Serializer.GetLength(name) + Serializer.GetLength(usable) + Serializer.GetLength(motion_priority) + Serializer.GetLength(motion_cancel) + Serializer.GetLength(bt_pause) + Serializer.GetLength(unable_input) + Serializer.GetLength(unable_input_move) + Serializer.GetLength(unable_move) + Serializer.GetLength(unable_sound) + Serializer.GetLength(occur_detach) + Serializer.GetLength(drop_item);
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
			Serializer.Load(br, ref category);
			Serializer.Load(br, ref key);
			Serializer.Load(br, ref name);
			Serializer.Load(br, ref usable);
			Serializer.Load(br, ref motion_priority);
			Serializer.Load(br, ref motion_cancel);
			Serializer.Load(br, ref bt_pause);
			Serializer.Load(br, ref unable_input);
			Serializer.Load(br, ref unable_input_move);
			Serializer.Load(br, ref unable_move);
			Serializer.Load(br, ref unable_sound);
			Serializer.Load(br, ref occur_detach);
			Serializer.Load(br, ref drop_item);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, category);
			Serializer.Save(bw, key);
			Serializer.Save(bw, name);
			Serializer.Save(bw, usable);
			Serializer.Save(bw, motion_priority);
			Serializer.Save(bw, motion_cancel);
			Serializer.Save(bw, bt_pause);
			Serializer.Save(bw, unable_input);
			Serializer.Save(bw, unable_input_move);
			Serializer.Save(bw, unable_move);
			Serializer.Save(bw, unable_sound);
			Serializer.Save(bw, occur_detach);
			Serializer.Save(bw, drop_item);
		}

		public bool Equal(DefAbnormal_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (category != comp.category)
			{
				return false;
			}
			if (key != comp.key)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (usable != comp.usable)
			{
				return false;
			}
			if (motion_priority != comp.motion_priority)
			{
				return false;
			}
			if (motion_cancel != comp.motion_cancel)
			{
				return false;
			}
			if (bt_pause != comp.bt_pause)
			{
				return false;
			}
			if (unable_input != comp.unable_input)
			{
				return false;
			}
			if (unable_input_move != comp.unable_input_move)
			{
				return false;
			}
			if (unable_move != comp.unable_move)
			{
				return false;
			}
			if (unable_sound != comp.unable_sound)
			{
				return false;
			}
			if (occur_detach != comp.occur_detach)
			{
				return false;
			}
			if (drop_item != comp.drop_item)
			{
				return false;
			}
			return true;
		}

		public DefAbnormal_MasterData Clone()
		{
			DefAbnormal_MasterData defAbnormal_MasterData = new DefAbnormal_MasterData();
			CopyTo(defAbnormal_MasterData);
			return defAbnormal_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			category = 0;
			key = string.Empty;
			name = string.Empty;
			usable = false;
			motion_priority = 0;
			motion_cancel = false;
			bt_pause = false;
			unable_input = false;
			unable_input_move = false;
			unable_move = false;
			unable_sound = false;
			occur_detach = false;
			drop_item = false;
		}
	}
}
