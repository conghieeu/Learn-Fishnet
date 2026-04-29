using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.Cooked;

public class DungeonWeather
{
	private int _dungeonMasterID;

	private int _randomSeed;

	private int _count;

	private int _defaultWeatherID;

	private List<int> _weatherByHour;

	private List<bool> _weatherForecastByHour;

	private bool _isRandomOccured;

	private int _rangeMinHour = int.MaxValue;

	private int _rangeMaxHour = int.MinValue;

	public bool IsRandomOccured => _isRandomOccured;

	public DungeonWeather(int dungeonMasterID, int randomSeed)
	{
		_dungeonMasterID = dungeonMasterID;
		_randomSeed = randomSeed;
		DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(_dungeonMasterID);
		if (dungeonInfo == null)
		{
			throw new Exception($"DungeonWeather.Generate() dungeonMasterID: {_dungeonMasterID}");
		}
		_defaultWeatherID = dungeonInfo.DefaultWeatherID;
		_weatherByHour = Enumerable.Repeat(_defaultWeatherID, 24).ToList();
		_weatherForecastByHour = Enumerable.Repeat(element: false, 24).ToList();
		Random random = new Random(_randomSeed);
		ImmutableArray<WeatherTimeInfo>.Enumerator enumerator = dungeonInfo.WeatherChanges.GetEnumerator();
		while (enumerator.MoveNext())
		{
			WeatherTimeInfo current = enumerator.Current;
			int hours = TimeSpan.FromSeconds(current.rangeStartTimeSec).Hours;
			int hours2 = TimeSpan.FromSeconds(current.rangeEndTimeSec).Hours;
			for (int i = ((hours2 - hours > 0) ? random.Next(hours, hours2 + 1) : hours); i < _weatherByHour.Count; i++)
			{
				_weatherByHour[i] = current.weatherId;
			}
		}
		if (dungeonInfo.WeatherRandomProb < random.Next(0, 10001))
		{
			return;
		}
		_count = random.Next(dungeonInfo.WeatherRandomMin, dungeonInfo.WeatherRandomMax + 1);
		if (_count <= 0)
		{
			return;
		}
		_isRandomOccured = true;
		int hours3 = TimeSpan.FromSeconds(dungeonInfo.WeatherRandomDurationTimeSec).Hours;
		List<int> list = Enumerable.Repeat(-1, 24).ToList();
		enumerator = dungeonInfo.WeatherRandomChanges.GetEnumerator();
		while (enumerator.MoveNext())
		{
			WeatherTimeInfo current2 = enumerator.Current;
			int hours4 = TimeSpan.FromSeconds(current2.rangeStartTimeSec).Hours;
			int hours5 = TimeSpan.FromSeconds(current2.rangeEndTimeSec).Hours;
			for (int j = hours4; j < hours5 + hours3; j++)
			{
				list[j] = current2.weatherId;
			}
			if (_rangeMinHour > hours4)
			{
				_rangeMinHour = hours4;
			}
			if (_rangeMaxHour < hours5)
			{
				_rangeMaxHour = hours5;
			}
		}
		for (int k = 0; k < _count; k++)
		{
			List<int> list2 = new List<int>();
			for (int l = 0; l < 24; l++)
			{
				bool flag = true;
				if (l + hours3 >= 24)
				{
					flag = false;
				}
				else
				{
					for (int m = 0; m < hours3; m++)
					{
						int index = l + m;
						if (list[index] == -1)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					list2.Add(l);
				}
			}
			if (list2.Count != 0)
			{
				int index2 = random.Next(0, list2.Count);
				int num = list2[index2];
				for (int n = 0; n < hours3; n++)
				{
					int index3 = num + n;
					_weatherByHour[index3] = list[index3];
					list[index3] = -1;
				}
				_weatherForecastByHour[num] = true;
				continue;
			}
			break;
		}
	}

	public int GetWeatherMasterID(int hour)
	{
		if (hour < 0 || hour >= _weatherByHour.Count)
		{
			Logger.RWarn($"DungeonWeather.GetWeaterMasterID() hour:{hour}", sendToLogServer: false, useConsoleOut: true, "weather");
			return _defaultWeatherID;
		}
		return _weatherByHour[hour];
	}

	public int GetWeatherForecastMasterID(int hour)
	{
		if (hour < 0 || hour >= _weatherByHour.Count)
		{
			Logger.RWarn($"DungeonWeather.GetWeaterForecastMasterID() hour:{hour}", sendToLogServer: false, useConsoleOut: true, "weather");
			return _defaultWeatherID;
		}
		int num = hour + TimeSpan.FromMilliseconds(Hub.s.dataman.ExcelDataManager.Consts.C_AlarmTimeWeatherEventMessage).Hours;
		if (num < 0 || num > 23)
		{
			return 0;
		}
		if (!_weatherForecastByHour[num])
		{
			return 0;
		}
		return _weatherByHour[num];
	}

	public SkyAndWeatherSystem.eWeatherPreset GetWeatherPreset(int hour)
	{
		if (hour < 0 || hour >= _weatherByHour.Count)
		{
			Logger.RWarn($"DungeonWeather.GetWeatherPreset() hour:{hour}", sendToLogServer: false, useConsoleOut: true, "weather");
			return SkyAndWeatherSystem.eWeatherPreset.Sunny;
		}
		WeatherInfo? weatherInfo = Hub.s.dataman.ExcelDataManager.GetWeatherInfo(_weatherByHour[hour]);
		if (weatherInfo == null)
		{
			Logger.RWarn($"DungeonWeather.GetWeatherPreset() weatherID:{_weatherByHour[hour]}", sendToLogServer: false, useConsoleOut: true, "weather");
		}
		return weatherInfo.WeatherPreset;
	}

	public List<int> GetAllWeather()
	{
		return _weatherByHour;
	}

	public float GetCurrentContaRate(int hour)
	{
		WeatherInfo weatherInfo = Hub.s.dataman.ExcelDataManager.GetWeatherInfo(_weatherByHour[hour]);
		if (weatherInfo == null)
		{
			Logger.RWarn($"DungeonWeather.GetCurrentContaRate() hour:{hour}", sendToLogServer: false, useConsoleOut: true, "weather");
			return 1f;
		}
		return (float)weatherInfo.ContaIncreaseRate * 0.0001f;
	}

	public string ToLogString()
	{
		string text = string.Join(", ", _weatherByHour.Select((int value, int index) => ((index == _rangeMinHour) ? "<u>" : "") + (_weatherForecastByHour[index] ? $"<b>{index}:{value}</b>" : $"{index}:{value}") + ((index == _rangeMaxHour) ? "</u>" : "")));
		return $"Weather : DunID={_dungeonMasterID}, rndcnt={_count} [ {text} ] seed={_randomSeed}";
	}

	public void SetWeatherOverrideAll(int weatherID)
	{
		if (Hub.s.dataman.ExcelDataManager.GetWeatherInfo(weatherID) == null)
		{
			Logger.RWarn($"DungeonWeather.SetWeatherOverrideAll() weatherID:{weatherID}", sendToLogServer: false, useConsoleOut: true, "weather");
			return;
		}
		for (int i = 0; i < _weatherByHour.Count; i++)
		{
			_weatherByHour[i] = weatherID;
			_weatherForecastByHour[i] = false;
		}
	}

	public void SetWeatherOverride(int weatherID, int startHour, int durationHour, bool forecast)
	{
		if (Hub.s.dataman.ExcelDataManager.GetWeatherInfo(weatherID) == null)
		{
			Logger.RWarn($"DungeonWeather.SetWeatherOverride() weatherID:{weatherID}", sendToLogServer: false, useConsoleOut: true, "weather");
			return;
		}
		if (startHour < 0 || startHour > 23)
		{
			Logger.RWarn($"DungeonWeather.SetWeatherOverride() startHour:{startHour}", sendToLogServer: false, useConsoleOut: true, "weather");
			return;
		}
		if (durationHour <= 0)
		{
			durationHour = 1;
		}
		for (int i = 0; i < durationHour && startHour + i < _weatherByHour.Count; i++)
		{
			_weatherByHour[startHour + i] = weatherID;
		}
		if (forecast)
		{
			_weatherForecastByHour[startHour] = true;
		}
	}
}
