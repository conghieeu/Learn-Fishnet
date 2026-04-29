using System;
using System.IO;
using System.Net;

namespace Bifrost.WeatherData
{
	public class WeatherData_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public int conta_increase_rate;

		public int blackout_rate;

		public WeatherData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public WeatherData_MasterData()
			: base(1461885370u, "WeatherData_MasterData")
		{
		}

		public void CopyTo(WeatherData_MasterData dest)
		{
			dest.id = id;
			dest.name = name;
			dest.conta_increase_rate = conta_increase_rate;
			dest.blackout_rate = blackout_rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(name) + Serializer.GetLength(conta_increase_rate) + Serializer.GetLength(blackout_rate);
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
			Serializer.Load(br, ref conta_increase_rate);
			Serializer.Load(br, ref blackout_rate);
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
			Serializer.Save(bw, conta_increase_rate);
			Serializer.Save(bw, blackout_rate);
		}

		public bool Equal(WeatherData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (conta_increase_rate != comp.conta_increase_rate)
			{
				return false;
			}
			if (blackout_rate != comp.blackout_rate)
			{
				return false;
			}
			return true;
		}

		public WeatherData_MasterData Clone()
		{
			WeatherData_MasterData weatherData_MasterData = new WeatherData_MasterData();
			CopyTo(weatherData_MasterData);
			return weatherData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			conta_increase_rate = 0;
			blackout_rate = 0;
		}
	}
}
