using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerVolumeData
{
	public readonly Bounds Bounds;

	public readonly MapTrigger.eUsageType UsageType;

	public readonly MapTrigger.eCheckTypeFlag CheckTypeFlag;

	public readonly int ServerData_RepeatCount;

	public readonly GameConditionGroup GameConditionGroup;

	public readonly bool StartActionCooltimeOnRateCheck;

	public readonly int ActionCooltime;

	private long _lastActionTime;

	public int TriggeredCount { get; private set; }

	public List<IGameAction> EventActionGroup { get; private set; } = new List<IGameAction>();

	public int ActionRate { get; private set; }

	public int ActionDelay { get; private set; }

	public TriggerVolumeData(MapTrigger mapTriggerData, Bounds bounds)
	{
		Bounds = bounds;
		UsageType = mapTriggerData.usageType;
		CheckTypeFlag = mapTriggerData.checkTypeFlag;
		ServerData_RepeatCount = mapTriggerData.serverData_repeatCount;
		if (!CondActionObjParser.GenerateConditionGroup(mapTriggerData.serverData_conditionString.Split(',').ToList(), "", out var group))
		{
			Logger.RWarn("TriggerVolumeData: GenerateConditionGroup failed");
		}
		else
		{
			GameConditionGroup = group;
		}
		if (!CondActionObjParser.GenerateActionGroup(mapTriggerData.serverData_actionString.Split(',').ToList(), "", out var actionGroup))
		{
			Logger.RWarn("TriggerVolumeData: GenerateActionGroup failed");
			return;
		}
		ActionRate = mapTriggerData.actionRate;
		ActionDelay = mapTriggerData.actionDelay;
		StartActionCooltimeOnRateCheck = mapTriggerData.startActionCooltimeOnRateCheck;
		ActionCooltime = mapTriggerData.actionCooltime;
		EventActionGroup = actionGroup.ToList();
	}

	public bool IsInBounds(Vector3 position)
	{
		return Bounds.Contains(position);
	}

	public bool IsTriggered(Vector3 prevPos, Vector3 currentPos, IVroom vroom, MapTrigger.eCheckTypeFlag checkTypeFlag)
	{
		if (EventActionGroup.Count == 0)
		{
			return false;
		}
		if (ServerData_RepeatCount >= 0 && TriggeredCount >= ServerData_RepeatCount)
		{
			return false;
		}
		if (CheckMapTriggerCheckTypeFlag(prevPos, currentPos, checkTypeFlag))
		{
			if (ActionCooltime > 0 && _lastActionTime + ActionCooltime > Hub.s.timeutil.GetCurrentTickMilliSec())
			{
				return false;
			}
			if (ActionRate != 10000)
			{
				if (StartActionCooltimeOnRateCheck)
				{
					_lastActionTime = Hub.s.timeutil.GetCurrentTickMilliSec();
				}
				int num = SimpleRandUtil.Next(0, 10001);
				if (ActionRate < num)
				{
					return false;
				}
			}
			if (GameConditionGroup != null && GameConditionGroup.ConditionDict.Count > 0)
			{
				List<int> aliveActorIDsInBound = new List<int>();
				vroom.IterateAllPlayer(delegate(VPlayer player)
				{
					if (player.IsAliveStatus() && IsInBounds(player.PositionVector))
					{
						aliveActorIDsInBound.Add(player.ObjectID);
					}
				});
				if (!vroom.CheckConditionGroup(GameConditionGroup, aliveActorIDsInBound))
				{
					return false;
				}
			}
			TriggeredCount++;
			_lastActionTime = Hub.s.timeutil.GetCurrentTickMilliSec();
			return true;
		}
		return false;
	}

	private bool CheckMapTriggerCheckTypeFlag(Vector3 prevPos, Vector3 currentPos, MapTrigger.eCheckTypeFlag checkTypeFlag)
	{
		if (checkTypeFlag.HasFlag(MapTrigger.eCheckTypeFlag.Enter) && CheckTypeFlag.HasFlag(MapTrigger.eCheckTypeFlag.Enter) && Bounds.Contains(currentPos) && !Bounds.Contains(prevPos))
		{
			return true;
		}
		if (checkTypeFlag.HasFlag(MapTrigger.eCheckTypeFlag.Inside) && CheckTypeFlag.HasFlag(MapTrigger.eCheckTypeFlag.Inside) && Bounds.Contains(currentPos))
		{
			return true;
		}
		return false;
	}
}
