using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ReluProtocol;
using UnityEngine;

public abstract class ILevelObjectInfo
{
	public class ActionMapData
	{
		public readonly GameConditionGroup GameConditionGroup;

		public readonly ImmutableList<IGameAction> GameActions;

		public readonly int ActionRemainingCount;

		public readonly int ActionDelay;

		public int ActionRemainingCurrentCount;

		public ActionMapData(GameConditionGroup gameConditionGroup, List<IGameAction> gameActions, int actionRemainingCount, int actionDelay)
		{
			GameConditionGroup = gameConditionGroup;
			GameActions = gameActions.ToImmutableList();
			ActionRemainingCount = actionRemainingCount;
			ActionDelay = actionDelay;
			ActionRemainingCurrentCount = actionRemainingCount;
		}
	}

	public int ID;

	public string Name;

	public LevelObjectType ObjectType;

	public Vector3 Pos;

	public LevelObject DataOrigin;

	public Dictionary<int, Dictionary<int, ActionMapData>> actionsMap = new Dictionary<int, Dictionary<int, ActionMapData>>();

	public Dictionary<(int, int), ActionMapData> a = new Dictionary<(int, int), ActionMapData>();

	public ILevelObjectInfo(LevelObjectType type, LevelObject origin)
	{
		ID = origin.levelObjectID;
		Name = origin.levelObjectName;
		ObjectType = type;
		DataOrigin = origin;
		Pos = origin.GetAIHandlePos() ?? origin.Pos.toVector3();
		foreach (KeyValuePair<int, Dictionary<int, LevelObject.StateActionInfo>> item in origin.StateActionsMap)
		{
			int key = item.Key;
			foreach (KeyValuePair<int, LevelObject.StateActionInfo> item2 in item.Value)
			{
				int key2 = item2.Key;
				LevelObject.StateActionInfo value = item2.Value;
				if (!actionsMap.ContainsKey(key))
				{
					actionsMap.Add(key, new Dictionary<int, ActionMapData>());
				}
				if (!actionsMap[key].ContainsKey(key2))
				{
					string condition = value.condition;
					string action = value.action;
					int delay = value.delay;
					int actionRemainingCount = value.actionRemainingCount;
					if (condition == string.Empty && action == string.Empty)
					{
						continue;
					}
					if (condition != string.Empty && action == string.Empty)
					{
						Logger.RError($"Condition is exist but action is empty. {key} -> {key2}");
						continue;
					}
					CondActionObjParser.GenerateConditionGroup(condition.Split(',').ToList(), $"{type}:{item.Key}", out var group);
					List<IGameAction> list = new List<IGameAction>();
					string[] array = action.Split(',');
					foreach (string text in array)
					{
						IGameAction action2 = null;
						if (!CondActionObjParser.ConvertString2Action(text, ref action2))
						{
							Logger.RError($"Failed to convert action string to IGameAction. {text} {key} -> {key2} // levelObject : {origin.levelObjectName}");
						}
						if (action2 != null)
						{
							list.Add(action2);
						}
					}
					actionsMap[key][key2] = new ActionMapData(group, list, actionRemainingCount, delay);
				}
				else
				{
					Logger.RError($"Same state already exist. {key} -> {key2}");
				}
			}
		}
	}

	public virtual LevelObjectInfo toLevelObjectInfo()
	{
		return new LevelObjectInfo
		{
			levelObjectID = ID,
			CurrentState = 0,
			OccupiedActorID = 0,
			position = Pos.toPosWithRot(0f)
		};
	}

	public bool CheckActionRemainingCurrentCount(int fromState, int toState)
	{
		if (actionsMap.ContainsKey(fromState) && actionsMap[fromState].ContainsKey(toState))
		{
			if (actionsMap[fromState][toState].ActionRemainingCurrentCount != -1)
			{
				return actionsMap[fromState][toState].ActionRemainingCurrentCount > 0;
			}
			return true;
		}
		return true;
	}

	public int DecreaseActionRemainingCurrentCount(int fromState, int toState)
	{
		if (actionsMap.ContainsKey(fromState) && actionsMap[fromState].ContainsKey(toState))
		{
			ActionMapData actionMapData = actionsMap[fromState][toState];
			if (actionMapData.ActionRemainingCurrentCount != -1 && actionMapData.ActionRemainingCurrentCount > 0)
			{
				actionMapData.ActionRemainingCurrentCount--;
				return actionMapData.ActionRemainingCurrentCount;
			}
			return actionMapData.ActionRemainingCurrentCount;
		}
		return 0;
	}

	public void ResetActionRemainingCurrentCount(int fromState, int toState)
	{
		if (actionsMap.ContainsKey(fromState) && actionsMap[fromState].ContainsKey(toState))
		{
			ActionMapData actionMapData = actionsMap[fromState][toState];
			actionMapData.ActionRemainingCurrentCount = actionMapData.ActionRemainingCount;
		}
	}

	public GameConditionGroup? GetGameConditionGroup(int fromState, int toState)
	{
		if (actionsMap.ContainsKey(fromState) && actionsMap[fromState].ContainsKey(toState))
		{
			return actionsMap[fromState][toState].GameConditionGroup;
		}
		return null;
	}

	public int GetGameActionDelay(int fromState, int toState)
	{
		if (actionsMap.ContainsKey(fromState) && actionsMap[fromState].ContainsKey(toState))
		{
			return actionsMap[fromState][toState].ActionDelay;
		}
		return 0;
	}

	public ImmutableList<IGameAction>? GetGameActions(int fromState, int toState)
	{
		if (actionsMap.ContainsKey(fromState) && actionsMap[fromState].ContainsKey(toState))
		{
			return actionsMap[fromState][toState].GameActions;
		}
		return null;
	}
}
