using System;
using Bifrost.WeatherData;

namespace Bifrost.Cooked
{
	public class WeatherInfo
	{
		public readonly int MasterID;

		public readonly string Name;

		public readonly int ContaIncreaseRate;

		public readonly int BlackoutRate;

		public readonly SkyAndWeatherSystem.eWeatherPreset WeatherPreset;

		public WeatherInfo(WeatherData_MasterData masterData)
		{
			MasterID = masterData.id;
			Name = masterData.name;
			ContaIncreaseRate = masterData.conta_increase_rate;
			BlackoutRate = masterData.blackout_rate;
			if (!Enum.TryParse<SkyAndWeatherSystem.eWeatherPreset>(Name, ignoreCase: true, out WeatherPreset))
			{
				throw new Exception("[WeatherInfo] invalid weather name " + Name);
			}
		}
	}
}
