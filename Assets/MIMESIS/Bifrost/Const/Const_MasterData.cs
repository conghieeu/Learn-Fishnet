using System;
using System.IO;
using System.Net;

namespace Bifrost.Const
{
	public class Const_MasterData : ISchema
	{
		public int id;

		public string key = string.Empty;

		public long value1;

		public long value2;

		public string valuestring = string.Empty;

		public Const_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Const_MasterData()
			: base(809263682u, "Const_MasterData")
		{
		}

		public void CopyTo(Const_MasterData dest)
		{
			dest.id = id;
			dest.key = key;
			dest.value1 = value1;
			dest.value2 = value2;
			dest.valuestring = valuestring;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(key) + Serializer.GetLength(value1) + Serializer.GetLength(value2) + Serializer.GetLength(valuestring);
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
			Serializer.Load(br, ref key);
			Serializer.Load(br, ref value1);
			Serializer.Load(br, ref value2);
			Serializer.Load(br, ref valuestring);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, key);
			Serializer.Save(bw, value1);
			Serializer.Save(bw, value2);
			Serializer.Save(bw, valuestring);
		}

		public bool Equal(Const_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (key != comp.key)
			{
				return false;
			}
			if (value1 != comp.value1)
			{
				return false;
			}
			if (value2 != comp.value2)
			{
				return false;
			}
			if (valuestring != comp.valuestring)
			{
				return false;
			}
			return true;
		}

		public Const_MasterData Clone()
		{
			Const_MasterData const_MasterData = new Const_MasterData();
			CopyTo(const_MasterData);
			return const_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			key = string.Empty;
			value1 = 0L;
			value2 = 0L;
			valuestring = string.Empty;
		}
	}
}
