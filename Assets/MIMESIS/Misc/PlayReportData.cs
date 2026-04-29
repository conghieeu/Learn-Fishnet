using System;
using System.Collections.Generic;
using System.Linq;
using ReluProtocol.Enum;

public class PlayReportData
{
	public readonly int PlayerActorID;

	public long TotalItemCarryCount { get; private set; }

	public long TotalDamageToAlly { get; private set; }

	public long TotalMimicEncounterCount { get; private set; }

	public long TotalTimeInStartingVolume { get; private set; }

	public PlayReportData(int actorID)
	{
		PlayerActorID = actorID;
	}

	public long GetValueByAwardType(AwardType awardType)
	{
		return awardType switch
		{
			AwardType.BestCarryItem => TotalItemCarryCount, 
			AwardType.BestDamageToAlly => TotalDamageToAlly, 
			AwardType.BestMimicEncounter => TotalMimicEncounterCount, 
			AwardType.BestCamper => TotalTimeInStartingVolume, 
			_ => throw new ArgumentOutOfRangeException("awardType", awardType, "지원되지 않는 AwardType입니다."), 
		};
	}

	public void IncreaseItemCarryCount(long count = 1L)
	{
		TotalItemCarryCount += count;
	}

	public void IncreaseDamageToAlly(long damage)
	{
		TotalDamageToAlly += damage;
	}

	public void IncreaseMimicEncounterCount(long count = 1L)
	{
		TotalMimicEncounterCount += count;
	}

	public void IncreaseTimeInStartingVolume(long second)
	{
		TotalTimeInStartingVolume += second;
	}

	public string ToLogString()
	{
		return $"ItemCarryCount:{TotalItemCarryCount,5}, DamageToAlly:{TotalDamageToAlly,5}, MimicEncounterCount:{TotalMimicEncounterCount,5}, TimeInStartingVolume:{TotalTimeInStartingVolume,5}";
	}

	public static Dictionary<int, AwardType> EvaluatePlayerAwards(Dictionary<int, PlayReportData> playReportDict)
	{
		List<AwardType> list = (from AwardType x in Enum.GetValues(typeof(AwardType))
			where x != AwardType.None
			select x).ToList();
		Dictionary<AwardType, long> dictionary = new Dictionary<AwardType, long>();
		Dictionary<AwardType, List<int>> dictionary2 = new Dictionary<AwardType, List<int>>();
		Dictionary<int, List<AwardType>> dictionary3 = new Dictionary<int, List<AwardType>>();
		Dictionary<AwardType, int> dictionary4 = new Dictionary<AwardType, int>();
		HashSet<int> assignedActorIDs = new HashSet<int>();
		Dictionary<AwardType, long> dictionary5 = new Dictionary<AwardType, long>
		{
			{
				AwardType.BestCarryItem,
				(!(Hub.s != null)) ? 1 : Hub.s.dataman.ExcelDataManager.Consts.C_ReportMinValueScrapCount
			},
			{
				AwardType.BestDamageToAlly,
				(!(Hub.s != null)) ? 1 : Hub.s.dataman.ExcelDataManager.Consts.C_ReportMinValueFriendlyAttack
			},
			{
				AwardType.BestMimicEncounter,
				(!(Hub.s != null)) ? 1 : Hub.s.dataman.ExcelDataManager.Consts.C_ReportMinValueMetMimesis
			},
			{
				AwardType.BestCamper,
				(Hub.s != null) ? Hub.s.dataman.ExcelDataManager.Consts.C_ReportMinValueStayTram : 90
			}
		};
		foreach (AwardType item3 in list)
		{
			dictionary.Add(item3, 0L);
			long num = long.MinValue;
			foreach (PlayReportData value in playReportDict.Values)
			{
				long valueByAwardType = value.GetValueByAwardType(item3);
				if (valueByAwardType > num)
				{
					num = valueByAwardType;
				}
			}
			dictionary[item3] = num;
		}
		foreach (AwardType item4 in list)
		{
			dictionary2.Add(item4, new List<int>());
			if (!dictionary5.ContainsKey(item4))
			{
				Logger.RError($"[EvaluatePlayerAwards] minThresholdPerAward not found {item4}. use 0");
				dictionary5.Add(item4, 0L);
			}
			float num2 = dictionary5[item4];
			long num3 = dictionary[item4];
			foreach (KeyValuePair<int, PlayReportData> item5 in playReportDict)
			{
				long valueByAwardType2 = item5.Value.GetValueByAwardType(item4);
				if (!((float)valueByAwardType2 < num2) && valueByAwardType2 >= num3)
				{
					dictionary2[item4].Add(item5.Key);
				}
			}
		}
		foreach (AwardType item6 in list)
		{
			foreach (int item7 in dictionary2[item6])
			{
				if (!dictionary3.ContainsKey(item7))
				{
					dictionary3[item7] = new List<AwardType>();
				}
				dictionary3[item7].Add(item6);
			}
		}
		foreach (AwardType item8 in list)
		{
			List<int> topActors = dictionary2[item8].Where((int x) => !assignedActorIDs.Contains(x)).ToList();
			if (topActors.Count == 0)
			{
				continue;
			}
			if (topActors.Count == 1)
			{
				int item = (dictionary4[item8] = topActors.First());
				assignedActorIDs.Add(item);
			}
			else
			{
				if (topActors.Count <= 1)
				{
					continue;
				}
				int num5 = dictionary3.Where((KeyValuePair<int, List<AwardType>> x) => topActors.Contains(x.Key) && !assignedActorIDs.Contains(x.Key)).Min((KeyValuePair<int, List<AwardType>> x) => x.Value.Count);
				List<int> list2 = new List<int>();
				foreach (KeyValuePair<int, List<AwardType>> item9 in dictionary3)
				{
					if (topActors.Contains(item9.Key) && !assignedActorIDs.Contains(item9.Key) && item9.Value.Count == num5)
					{
						list2.Add(item9.Key);
					}
				}
				if (list2.Count != 0)
				{
					int item2 = (dictionary4[item8] = ((list2.Count == 1) ? list2.First() : list2[SimpleRandUtil.Next(0, list2.Count)]));
					assignedActorIDs.Add(item2);
				}
			}
		}
		return dictionary4.ToDictionary((KeyValuePair<AwardType, int> x) => x.Value, (KeyValuePair<AwardType, int> x) => x.Key);
	}

	public static void LogPlayReport(Dictionary<int, AwardType> winners, Dictionary<int, PlayReportData> playReportDict)
	{
		foreach (KeyValuePair<int, PlayReportData> item in playReportDict)
		{
			int key = item.Key;
			_ = item.Value;
			winners.ContainsKey(key);
		}
	}
}
