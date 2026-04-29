using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bifrost.Dungeon;

public class DungeonMasterInfo
{
	public readonly int ID;

	public readonly int MapID;

	public readonly string StartDisplayTime;

	public readonly string EndTime;

	public readonly long Duration;

	public readonly int MiscMinVal;

	public readonly int MiscMaxVal;

	public SpawnableItemInfo? SpawnableItemInfo;

	public SpawnableMonsterInfo? SpawnableMonsterInfo;

	public ImmutableDictionary<long, ImmutableArray<IGameAction>> EventGroupDict = ImmutableDictionary<long, ImmutableArray<IGameAction>>.Empty;

	public readonly int ContaIncreaseValueIdle;

	public readonly int ContaIncreaseValueRun;

	public readonly bool IsActive;

	public readonly int MinSessionCount;

	public readonly int MaxSessionCount;

	public readonly int HazardLevel;

	public readonly int PollutionLevel;

	public readonly int RewardLevel;

	public readonly int ShopGroupID;

	public readonly string mapName;

	public readonly int CanopyCount;

	public readonly int DefaultWeatherID;

	public ImmutableArray<WeatherTimeInfo> WeatherChanges = ImmutableArray<WeatherTimeInfo>.Empty;

	public readonly int WeatherRandomProb;

	public readonly int WeatherRandomMin;

	public readonly int WeatherRandomMax;

	public ImmutableArray<WeatherTimeInfo> WeatherRandomChanges = ImmutableArray<WeatherTimeInfo>.Empty;

	public readonly long WeatherRandomDurationTimeSec;

	public int ThreatMin;

	public int ThreatMax;

	public int NormalMonsterSpawnPeriod;

	public int NormalMonsterSpawnTryCount;

	public int NormalMonsterSpawnRate;

	public int MimicSpawnCountMin;

	public int MimicSpawnCountMax;

	public int MimicSpawnPeriod;

	public int MimicSpawnTryCount;

	public int MimicSpawnRate;

	public ImmutableDictionary<string, int> DungenCandidates = ImmutableDictionary<string, int>.Empty;

	public readonly int MaxDungenRate;

	public readonly int randomTeleporterChance;

	public readonly int randomTeleporterStartMin;

	public readonly int randomTeleporterStartMax;

	public readonly int randomTeleporterEndMin;

	public readonly int randomTeleporterEndMax;

	public readonly string LoadingSceneName;

	public DungeonMasterInfo(Dungeon_MasterData data)
	{
		ID = data.id;
		MapID = data.map_id;
		StartDisplayTime = data.start_display_time;
		EndTime = data.end_time;
		mapName = data.map_name;
		ContaIncreaseValueIdle = data.conta_increase_idle;
		ContaIncreaseValueRun = data.conta_increase_run;
		IsActive = data.is_active;
		MinSessionCount = data.min_session_count;
		MaxSessionCount = data.max_session_count;
		HazardLevel = data.hazard_level;
		PollutionLevel = data.env_level;
		RewardLevel = data.reward_level;
		ShopGroupID = data.shop_group;
		(long, long) tuple = VWorldUtil.ConvertTimeToSeconds(StartDisplayTime, EndTime);
		long item = tuple.Item1;
		long item2 = tuple.Item2;
		long c_GameTimeScaleFactor = Hub.s.dataman.ExcelDataManager.Consts.C_GameTimeScaleFactor;
		Duration = (item2 - item) / (c_GameTimeScaleFactor / 1000);
		ImmutableDictionary<long, ImmutableArray<IGameAction>>.Builder builder = ImmutableDictionary.CreateBuilder<long, ImmutableArray<IGameAction>>();
		foreach (Dungeon_event_group item3 in data.Dungeon_event_groupval)
		{
			long key = (VWorldUtil.ConvertTimeToSeconds(item3.event_time) - item) / (c_GameTimeScaleFactor / 1000);
			if (!EventGroupDict.ContainsKey(key))
			{
				if (!CondActionObjParser.GenerateActionGroup(item3.event_actions, "", out var actionGroup))
				{
					Logger.RWarn("DungeonMasterInfo: GenerateActionGroup failed");
				}
				else
				{
					builder.Add(key, actionGroup);
				}
			}
			else
			{
				Logger.RError("DungeonMasterInfo: EventGroupDict already contains key: " + key);
			}
		}
		EventGroupDict = builder.ToImmutable();
		MiscMinVal = data.misc_value_min;
		MiscMaxVal = data.misc_value_max;
		if (data.spawnable_misc_id > 0)
		{
			SpawnableItemInfo = Hub.s.dataman.ExcelDataManager.GetSpawnableItemData(data.spawnable_misc_id);
			if (SpawnableItemInfo == null)
			{
				throw new Exception($"DungeonMasterInfo: SpawnableItemInfo is null for ID: {data.spawnable_misc_id}");
			}
		}
		if (data.normal_master_ids > 0)
		{
			SpawnableMonsterInfo = Hub.s.dataman.ExcelDataManager.GetSpawnableMonsterData(data.normal_master_ids);
			if (SpawnableMonsterInfo == null)
			{
				throw new Exception($"DungeonMasterInfo: SpawnableMonsterInfo is null for ID: {data.normal_master_ids}");
			}
		}
		CanopyCount = data.canopy_count;
		DefaultWeatherID = data.default_weather_id;
		WeatherChanges = ConvertWeatherRangeValues(data.weather_change);
		WeatherRandomProb = data.weather_random_prob;
		WeatherRandomMin = data.weather_random_min;
		WeatherRandomMax = data.weather_random_max;
		WeatherRandomChanges = ConvertWeatherRangeValues(data.weather_random);
		WeatherRandomDurationTimeSec = data.weather_random_duration / 1000;
		ThreatMin = data.threat_min;
		ThreatMax = data.threat_max;
		NormalMonsterSpawnPeriod = data.normal_monster_spawn_period;
		NormalMonsterSpawnTryCount = data.normal_monster_spawn_try_count;
		NormalMonsterSpawnRate = data.normal_monster_spawn_rate;
		MimicSpawnCountMin = data.mimic_spawn_count_min;
		MimicSpawnCountMax = data.mimic_spawn_count_max;
		MimicSpawnPeriod = data.mimic_spawn_period;
		MimicSpawnTryCount = data.mimic_spawn_try_count;
		MimicSpawnRate = data.mimic_spawn_rate;
		ImmutableDictionary<string, int>.Builder builder2 = ImmutableDictionary.CreateBuilder<string, int>();
		int num = 0;
		foreach (Dungeon_candidate item4 in data.Dungeon_candidateval)
		{
			if (!builder2.ContainsKey(item4.spawnable_dungen))
			{
				num += item4.spawnable_dungen_rate;
				builder2.Add(item4.spawnable_dungen, num);
			}
			else
			{
				Logger.RError("DungeonMasterInfo: DungenCandidates already contains key: " + item4.spawnable_dungen);
			}
		}
		MaxDungenRate = num;
		DungenCandidates = builder2.ToImmutable();
		randomTeleporterChance = data.random_teleport_rate;
		if (data.random_teleport_start_count.Count >= 2)
		{
			randomTeleporterStartMin = data.random_teleport_start_count[0];
			randomTeleporterStartMax = data.random_teleport_start_count[1];
		}
		else
		{
			randomTeleporterStartMin = 0;
			randomTeleporterStartMax = 0;
		}
		if (data.random_teleport_end_count.Count >= 2)
		{
			randomTeleporterEndMin = data.random_teleport_end_count[0];
			randomTeleporterEndMax = data.random_teleport_end_count[1];
		}
		else
		{
			randomTeleporterEndMin = 0;
			randomTeleporterEndMax = 0;
		}
		LoadingSceneName = data.loading_scene_name;
	}

	public string GetRandomDungenName(int randVal)
	{
		foreach (KeyValuePair<string, int> dungenCandidate in DungenCandidates)
		{
			if (dungenCandidate.Value > randVal)
			{
				return dungenCandidate.Key;
			}
		}
		Logger.RError("DungeonMasterInfo: GetRandomDungenName failed to find a valid dungen name. MaxDungenRate: " + MaxDungenRate + ", randVal: " + randVal);
		return string.Empty;
	}

	private ImmutableArray<WeatherTimeInfo> ConvertWeatherRangeValues(List<string> inlist)
	{
		List<WeatherTimeInfo> list = new List<WeatherTimeInfo>();
		foreach (string item in inlist)
		{
			string[] array = item.Split(';');
			string startTime = array[0];
			string endTime = array[1];
			int.TryParse(array[2], out var result);
			var (rangeStartTimeSec, rangeEndTimeSec) = VWorldUtil.ConvertTimeToSeconds(startTime, endTime);
			list.Add(new WeatherTimeInfo
			{
				rangeStartTimeSec = rangeStartTimeSec,
				rangeEndTimeSec = rangeEndTimeSec,
				weatherId = result
			});
		}
		return list.ToImmutableArray();
	}
}
