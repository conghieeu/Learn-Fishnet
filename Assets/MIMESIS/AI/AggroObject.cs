using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bifrost.ConstEnum;
using Bifrost.Cooked;

public class AggroObject
{
	public readonly VCreature Self;

	public readonly VCreature Target;

	private Dictionary<AggroType, int> _aggroValueDict = new Dictionary<AggroType, int>();

	private Dictionary<AggroType, long> _aggroOccurElapsedDict = new Dictionary<AggroType, long>();

	private Dictionary<AggroType, long> _lastAggroDecreaseTimeDict = new Dictionary<AggroType, long>();

	private ImmutableDictionary<AggroType, AggroInfo> AggroInfoDict;

	private MonsterInfo _monsterInfo;

	public int AggroValue;

	private bool _enableAggroLog => Self.VRoom.EnableAggroLog;

	public AggroObject(VCreature self, VCreature aggroTarget)
	{
		Self = self;
		Target = aggroTarget;
		_monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(Self.MasterID);
		if (_monsterInfo == null)
		{
			throw new Exception($"MonsterInfo is null, MasterID : {Self.MasterID}");
		}
		foreach (AggroType value in Enum.GetValues(typeof(AggroType)))
		{
			_aggroValueDict.Add(value, 0);
			_aggroOccurElapsedDict.Add(value, 0L);
			_lastAggroDecreaseTimeDict.Add(value, 0L);
		}
		AggroInfoDict = _monsterInfo.AggroInfoDict;
	}

	public bool CanTarget(bool checkAggro, bool checkHeight, float maxDistance)
	{
		if (Target.AttachControlUnit.IsAttached() || Target.AttachControlUnit.IsAttaching())
		{
			return false;
		}
		if (Target.IsIndoor != Self.IsIndoor)
		{
			return false;
		}
		if (checkHeight && Math.Abs(Self.PositionVector.y - Target.PositionVector.y) > (float)Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold)
		{
			return false;
		}
		if (Misc.Distance(Self.PositionVector, Target.PositionVector) > maxDistance)
		{
			return false;
		}
		if (checkAggro)
		{
			return AggroValue > _monsterInfo.AggroHuddleForTargeting;
		}
		return true;
	}

	public AggroObject(VCreature self, VCreature target, Dictionary<AggroType, int> aggroValueDict, Dictionary<AggroType, long> aggroOccurElapsedDict, Dictionary<AggroType, long> lastAggroDecreaseTimeDict, ImmutableDictionary<AggroType, AggroInfo> aggroInfoDict, int aggroValue)
	{
		Self = self;
		Target = target;
		foreach (AggroType key in (Hub.s.dataman.ExcelDataManager.GetMonsterInfo(Self.MasterID) ?? throw new Exception($"MonsterInfo is null, MasterID : {Self.MasterID}")).AggroInfoDict.Keys)
		{
			_aggroValueDict.Add(key, aggroValueDict[key]);
			_aggroOccurElapsedDict.Add(key, aggroOccurElapsedDict[key]);
			_lastAggroDecreaseTimeDict.Add(key, lastAggroDecreaseTimeDict[key]);
		}
		AggroInfoDict = aggroInfoDict;
	}

	public AggroObject Clone()
	{
		return new AggroObject(Self, Target, _aggroValueDict, _aggroOccurElapsedDict, _lastAggroDecreaseTimeDict, AggroInfoDict, AggroValue);
	}

	public void Update()
	{
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		long c_IncreaseAggroTick = Hub.s.dataman.ExcelDataManager.Consts.C_IncreaseAggroTick;
		long c_DecreaseAggroTick = Hub.s.dataman.ExcelDataManager.Consts.C_DecreaseAggroTick;
		float num = Misc.Distance(Self.PositionVector, Target.PositionVector);
		if (AggroInfoDict.ContainsKey(AggroType.Sight))
		{
			AggroInfo aggroInfo = AggroInfoDict[AggroType.Sight];
			if ((double)num < aggroInfo.Range && aggroInfo.Range != 0.0 && currentTickMilliSec - _aggroOccurElapsedDict[AggroType.Sight] > c_IncreaseAggroTick)
			{
				int num2 = (int)(100.0 * (1.0 - (double)num / aggroInfo.Range) * aggroInfo.Weight);
				if (num2 > 0)
				{
					_aggroValueDict[AggroType.Sight] += num2;
					_aggroOccurElapsedDict[AggroType.Sight] = currentTickMilliSec;
				}
			}
		}
		if (AggroInfoDict.ContainsKey(AggroType.Sound))
		{
			AggroInfo aggroInfo2 = AggroInfoDict[AggroType.Sound];
			if (aggroInfo2.Range == 0.0)
			{
				Logger.RWarn($"AggroObject Update: Sound Aggro Range is 0 for MasterID {Self.MasterID}. This should not happen.");
			}
			else if ((double)num <= aggroInfo2.Range && currentTickMilliSec - _aggroOccurElapsedDict[AggroType.Sound] > c_IncreaseAggroTick)
			{
				int num3 = (int)((double)Target.StatControlUnit.GetCurrentVoiceResonance() * (1.0 - (double)num / aggroInfo2.Range) * aggroInfo2.Weight);
				if (num3 > 0)
				{
					_aggroValueDict[AggroType.Sound] += num3;
					_aggroOccurElapsedDict[AggroType.Sound] = currentTickMilliSec;
				}
				num3 = (int)((double)Target.InventoryControlUnit.GetOnHandtemAggroValue() * (1.0 - (double)num / aggroInfo2.Range) * aggroInfo2.Weight);
				if (num3 > 0)
				{
					_aggroValueDict[AggroType.Sound] += num3;
					_aggroOccurElapsedDict[AggroType.Sound] = currentTickMilliSec;
				}
			}
		}
		long c_NoAggroDuration = Hub.s.dataman.ExcelDataManager.Consts.C_NoAggroDuration;
		long num4 = currentTickMilliSec - _aggroOccurElapsedDict[AggroType.Sight];
		long num5 = currentTickMilliSec - _aggroOccurElapsedDict[AggroType.Hit];
		long num6 = currentTickMilliSec - _aggroOccurElapsedDict[AggroType.Sound];
		float num7 = (float)Hub.s.dataman.ExcelDataManager.Consts.C_DecreaseAggroRatePerTick * 0.0001f;
		if (num4 > c_NoAggroDuration && currentTickMilliSec - _lastAggroDecreaseTimeDict[AggroType.Sight] > c_DecreaseAggroTick)
		{
			_aggroValueDict[AggroType.Sight] -= (int)Math.Ceiling((float)_aggroValueDict[AggroType.Sight] * num7);
			_lastAggroDecreaseTimeDict[AggroType.Sight] = currentTickMilliSec;
		}
		if (num5 > c_NoAggroDuration && currentTickMilliSec - _lastAggroDecreaseTimeDict[AggroType.Hit] > c_DecreaseAggroTick)
		{
			_aggroValueDict[AggroType.Hit] -= (int)Math.Ceiling((float)_aggroValueDict[AggroType.Hit] * num7);
			_lastAggroDecreaseTimeDict[AggroType.Hit] = currentTickMilliSec;
		}
		if (num6 > c_NoAggroDuration && currentTickMilliSec - _lastAggroDecreaseTimeDict[AggroType.Sound] > c_DecreaseAggroTick)
		{
			_aggroValueDict[AggroType.Sound] -= (int)Math.Ceiling((float)_aggroValueDict[AggroType.Sound] * num7);
			_lastAggroDecreaseTimeDict[AggroType.Sound] = currentTickMilliSec;
		}
		int aggroValue = AggroValue;
		AggroValue = 0;
		foreach (AggroType key in _aggroValueDict.Keys)
		{
			if (AggroInfoDict.ContainsKey(key))
			{
				AggroValue += _aggroValueDict[key];
			}
		}
		if (aggroValue != AggroValue)
		{
			_ = _enableAggroLog;
		}
	}

	public void AddAggroPointByHit(int hitAggroPoint)
	{
		if (AggroInfoDict.ContainsKey(AggroType.Hit))
		{
			AggroInfo aggroInfo = AggroInfoDict[AggroType.Hit];
			int num = (int)((double)hitAggroPoint * aggroInfo.Weight);
			if (num > 0)
			{
				_aggroValueDict[AggroType.Hit] += num;
				_aggroOccurElapsedDict[AggroType.Hit] = Hub.s.timeutil.GetCurrentTickMilliSec();
			}
		}
	}

	public void AddAggroPointBySound(int soundAggroPoint)
	{
		if (AggroInfoDict.ContainsKey(AggroType.Sound))
		{
			AggroInfo aggroInfo = AggroInfoDict[AggroType.Sound];
			int num = (int)((double)soundAggroPoint * (1.0 - (double)Misc.Distance(Self.PositionVector, Target.PositionVector) / aggroInfo.Range) * aggroInfo.Weight);
			if (num > 0)
			{
				_aggroValueDict[AggroType.Sound] += num;
				_aggroOccurElapsedDict[AggroType.Sound] = Hub.s.timeutil.GetCurrentTickMilliSec();
			}
		}
	}

	public void AddAggroPointByMovement(float distance)
	{
		if (!AggroInfoDict.ContainsKey(AggroType.Sound))
		{
			return;
		}
		AggroInfo aggroInfo = AggroInfoDict[AggroType.Sound];
		float distanceForScore = aggroInfo.DistanceForScore;
		if (!(distanceForScore <= 0f))
		{
			int num = (int)((double)(distance / distanceForScore * 100f) * (1.0 - (double)Misc.Distance(Self.PositionVector, Target.PositionVector) / aggroInfo.Range) * aggroInfo.Weight);
			if (num > 0)
			{
				_aggroValueDict[AggroType.Sound] += num;
				_aggroOccurElapsedDict[AggroType.Sound] = Hub.s.timeutil.GetCurrentTickMilliSec();
			}
		}
	}
}
