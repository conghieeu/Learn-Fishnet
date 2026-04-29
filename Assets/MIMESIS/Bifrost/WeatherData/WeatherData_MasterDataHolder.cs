using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.WeatherData
{
	public class WeatherData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<WeatherData_MasterData> dataHolder = new List<WeatherData_MasterData>();

		public WeatherData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public WeatherData_MasterDataHolder()
			: base(513570588u, "WeatherData_MasterDataHolder")
		{
		}

		public void CopyTo(WeatherData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (WeatherData_MasterData item in dataHolder)
			{
				WeatherData_MasterData weatherData_MasterData = new WeatherData_MasterData();
				item.CopyTo(weatherData_MasterData);
				dest.dataHolder.Add(weatherData_MasterData);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(versionInfo);
			num += 4;
			foreach (WeatherData_MasterData item in dataHolder)
			{
				num += item.GetLengthInternal();
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
			Serializer.Load(br, ref versionInfo);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				WeatherData_MasterData weatherData_MasterData = new WeatherData_MasterData();
				weatherData_MasterData.LoadInternal(br);
				dataHolder.Add(weatherData_MasterData);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, versionInfo);
			Serializer.Save(bw, dataHolder.Count);
			foreach (WeatherData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(WeatherData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (WeatherData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (WeatherData_MasterData item2 in comp.dataHolder)
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
			return true;
		}

		public WeatherData_MasterDataHolder Clone()
		{
			WeatherData_MasterDataHolder weatherData_MasterDataHolder = new WeatherData_MasterDataHolder();
			CopyTo(weatherData_MasterDataHolder);
			return weatherData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
