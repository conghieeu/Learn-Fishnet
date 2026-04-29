using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.ConstEnum;
using ReluProtocol.Enum;

public class CondActionObjParser
{
	public static DefCondition GetGameConditionType(string typeString)
	{
		DefCondition result = DefCondition.Invalid;
		Enum.TryParse<DefCondition>(typeString, out result);
		if (result == DefCondition.Invalid)
		{
			Logger.RWarn("Error GetGameConditionType  Condition : " + typeString);
		}
		return result;
	}

	public static DefAction GetGameActionType(string typeString)
	{
		DefAction result = DefAction.Invalid;
		Enum.TryParse<DefAction>(typeString, out result);
		if (result == DefAction.Invalid)
		{
			Logger.RWarn("Error GetGameActionType  Action : " + typeString);
		}
		return result;
	}

	public static bool GenerateConditionGroup(List<string> datas, string context, out GameConditionGroup group, int actionRemainingCount = -1)
	{
		ImmutableDictionary<int, ImmutableDictionary<int, IGameCondition>.Builder>.Builder builder = ImmutableDictionary.CreateBuilder<int, ImmutableDictionary<int, IGameCondition>.Builder>();
		try
		{
			foreach (string data in datas)
			{
				IGameCondition detailInfo = null;
				if (data.Length == 0)
				{
					continue;
				}
				if (!ConvertString2Condition(data, ref detailInfo, out var index))
				{
					Logger.RWarn("Invalid Parsing occured. / Condition : " + data + " / " + context);
				}
				else if (detailInfo != null)
				{
					if (!builder.TryGetValue(index, out var value))
					{
						value = ImmutableDictionary.CreateBuilder<int, IGameCondition>();
						builder.Add(index, value);
					}
					int num = ((value.Keys.Count() == 0) ? 1 : (value.Keys.Max() + 1));
					detailInfo.SetIndex(num);
					value.Add(num, detailInfo);
				}
			}
		}
		catch (Exception e)
		{
			Logger.RError(e);
		}
		group = new GameConditionGroup(builder.ToImmutableDictionary((KeyValuePair<int, ImmutableDictionary<int, IGameCondition>.Builder> x) => x.Key, (KeyValuePair<int, ImmutableDictionary<int, IGameCondition>.Builder> x) => x.Value.ToImmutable()));
		return true;
	}

	public static bool GenerateActionGroup(List<string> datas, string context, out ImmutableArray<IGameAction> actionGroup)
	{
		ImmutableArray<IGameAction>.Builder builder = ImmutableArray.CreateBuilder<IGameAction>();
		try
		{
			foreach (string data in datas)
			{
				IGameAction action = null;
				if (data.Length != 0)
				{
					if (!ConvertString2Action(data, ref action))
					{
						Logger.RWarn("Invalid Parsing occured. / Action : " + data + " / " + context);
					}
					else if (action != null)
					{
						builder.Add(action);
					}
				}
			}
		}
		catch (Exception e)
		{
			Logger.RError(e);
		}
		actionGroup = builder.ToImmutable();
		return true;
	}

	public static bool ConvertString2Condition(string data, ref IGameCondition? detailInfo, out int index)
	{
		string[] array = data.Split('/');
		int num = 0;
		if (int.TryParse(array[num], out var result))
		{
			num++;
		}
		if (result == 0)
		{
			result = 1;
		}
		index = result;
		DefCondition gameConditionType = GetGameConditionType(array[num++]);
		if (gameConditionType == DefCondition.Invalid)
		{
			return false;
		}
		switch (gameConditionType)
		{
		case DefCondition.Invalid:
			detailInfo = new GameConditionInvalid();
			break;
		case DefCondition.CHECK_ITEM:
		{
			if (array.Length - num != 2)
			{
				Logger.RWarn($"Conditionstring CHECK_ITEM Format Error : {data}");
				return false;
			}
			int itemMasterID = int.Parse(array[num++]);
			int count = int.Parse(array[num++]);
			detailInfo = new GameConditionCheckItem(itemMasterID, count);
			break;
		}
		case DefCondition.CHECK_SCRAP_WEIGHT:
		{
			if (array.Length - num != 1)
			{
				Logger.RWarn($"Conditionstring CHECK_SCRAP_WEIGHT Format Error : {data}");
				return false;
			}
			int weight = int.Parse(array[num++]);
			detailInfo = new GameConditionCheckScrapWeight(weight);
			break;
		}
		case DefCondition.CHECK_ACTOR_TYPE:
		{
			if (array.Length - num != 1)
			{
				Logger.RWarn($"Conditionstring CHECK_ACTOR_TYPE Format Error : {data}");
				return false;
			}
			string text2 = array[num++];
			if (!Enum.TryParse<ActorType>(text2, out var result3))
			{
				Logger.RWarn($"Conditionstring CHECK_ACTOR_TYPE Format Error : {data}, {text2}");
				return false;
			}
			detailInfo = new GameConditionCheckActorType(result3);
			break;
		}
		case DefCondition.CHECK_TRAM_REPAIR:
		{
			if (array.Length - num != 1)
			{
				Logger.RWarn($"Conditionstring CHECK_TRAM_REPAIR Format Error : {data}");
				return false;
			}
			string text = array[num++];
			if (!bool.TryParse(text, out var result2))
			{
				Logger.RWarn($"Conditionstring CHECK_TRAM_REPAIR Format Error : {data}, {text}");
				return false;
			}
			detailInfo = new GameConditionCheckTramRepair(result2);
			break;
		}
		case DefCondition.CHECK_EVIDENCE_STATE:
		{
			if (array.Length < 4)
			{
				Logger.RWarn($"Conditionstring CHECK_EVIDENCE_STATE Format Error : {data}");
				return false;
			}
			int evidenceMasterID = int.Parse(array[num++]);
			int state = int.Parse(array[num++]);
			detailInfo = new CheckEvidenceStateCondition(evidenceMasterID, state);
			break;
		}
		}
		if (detailInfo == null)
		{
			return false;
		}
		return true;
	}

	public static bool ConvertString2Action(string data, ref IGameAction? action)
	{
		string[] array = data.Split('/');
		int num = 0;
		try
		{
			if (int.TryParse(array[num], out var _))
			{
				num++;
			}
			DefAction gameActionType = GetGameActionType(array[num++]);
			if (gameActionType == DefAction.Invalid)
			{
				return false;
			}
			switch (gameActionType)
			{
			case DefAction.Invalid:
				action = new GameActionInvalid();
				break;
			case DefAction.MAP_COMPLETE:
				action = new GameActionMapComplete();
				break;
			case DefAction.ACTIVATE_SPAWNPOINT:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"Actionstring ACTIVATE_SPAWNPOINT Format Error : {data}");
					return false;
				}
				List<string> list = new List<string>();
				string[] array2 = array[1].Split(';');
				foreach (string item in array2)
				{
					list.Add(item);
				}
				action = new GameActionActivateSpawnPoint(list);
				break;
			}
			case DefAction.SESSION_DECISION:
				action = new GameActionSessionDecision();
				break;
			case DefAction.SETINVINCIBLE:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"Actionstring SETINVINCIBLE Format Error : {data}");
					return false;
				}
				if (long.TryParse(array[1], out var result3))
				{
					action = new GameActionSetInvincible(result3);
				}
				break;
			}
			case DefAction.PLAY_SOUND:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"Actionstring PLAY_SOUND Format Error : {data}");
					return false;
				}
				string soundName = array[1];
				action = new GameActionPlaySound(soundName);
				break;
			}
			case DefAction.CHANGE_MUTABLE_STAT:
			{
				if (array.Length != 3)
				{
					Logger.RWarn($"Actionstring ChangeMutableStat Format Error : {data}");
					return false;
				}
				if (!Enum.TryParse<MutableStatType>(array[1], ignoreCase: true, out var result4))
				{
					Logger.RWarn($"Actionstring ChangeMutableStat Format Error : {data}");
					return false;
				}
				long value = long.Parse(array[2]);
				action = new GameActionChangeMutableStat(result4, value);
				break;
			}
			case DefAction.DECREASE_ITEM_COUNT:
			{
				if (array.Length != 3)
				{
					Logger.RWarn($"Actionstring DecreaseItemCount Format Error : {data}");
					return false;
				}
				int itemMasterID = int.Parse(array[1]);
				int count = int.Parse(array[2]);
				action = new GameActionDecreaseItem(itemMasterID, count);
				break;
			}
			case DefAction.ADD_ABNORMAL:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"Actionstring AddAbnormal Format Error : {data}");
					return false;
				}
				int abnormalMasterID = int.Parse(array[1]);
				action = new GameActionAddAbnormal(abnormalMasterID);
				break;
			}
			case DefAction.CHANGE_MUTABLE_STAT_RANDOM:
			{
				if (array.Length != 4)
				{
					Logger.RWarn($"Actionstring ChangeMutableStatRandom Format Error : {data}");
					return false;
				}
				if (!Enum.TryParse<MutableStatType>(array[1], ignoreCase: true, out var result2))
				{
					Logger.RWarn($"Actionstring ChangeMutableStatRandom Format Error : {data}");
					return false;
				}
				long minValue = long.Parse(array[2]);
				long maxValue = long.Parse(array[3]);
				action = new GameActionChangeMutableStatRandom(result2, minValue, maxValue);
				break;
			}
			case DefAction.TELEPORT:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"Actionstring Teleport Format Error : {data}");
					return false;
				}
				string teleportStartPointCallSign = array[1];
				action = new GameActionTeleport(teleportStartPointCallSign);
				break;
			}
			case DefAction.RANDOM_TELEPORT:
				if (array.Length != 1)
				{
					Logger.RWarn($"Actionstring RandomTeleport Format Error : {data}");
					return false;
				}
				action = new GameActionRandomTeleport();
				break;
			case DefAction.HANDLE_LEVELOBJECT:
			{
				if (array.Length != 3 && array.Length != 4 && array.Length != 5)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				string levelObjectName = array[1];
				int state = int.Parse(array[2]);
				bool occupy = array.Length == 4 && bool.Parse(array[3]);
				bool resetRemainCount = array.Length == 5 && bool.Parse(array[4]);
				action = new GameActionHandleLevelObject(levelObjectName, state, occupy, resetRemainCount);
				break;
			}
			case DefAction.SPAWN_FIELD_SKILL:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				int fieldSkillMasterID = int.Parse(array[1]);
				action = new GameActionSpawnFieldSkill(fieldSkillMasterID);
				break;
			}
			case DefAction.SPAWN_FIELD_SKILL_RANDOM:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				List<int> list2 = array[1].Split('|').Select(int.Parse).ToList();
				if (list2.Count == 0)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				action = new GameActionSpawnFieldSkillRandom(list2);
				break;
			}
			case DefAction.TELEPORT_TO_SPAWNPOINT:
				action = new GameActionTeleportToSpawnPoint();
				break;
			case DefAction.SPAWN_FIELD_SKILL_MAPMARKER:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				string mapMarkerName = array[1];
				action = new GameActionSpawnFieldSkillMapMarker(mapMarkerName);
				break;
			}
			case DefAction.INSTANT_KILL:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				string levelObjectName2 = array[1];
				action = new GameActionInstantKill(levelObjectName2);
				break;
			}
			case DefAction.WAIT:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				long duration = long.Parse(array[1]);
				action = new GameActionWait(duration);
				break;
			}
			case DefAction.ARCHIVE_LOOTING_OBJECT:
				if (array.Length != 1)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				action = new GameActionArchiveLootingObject();
				break;
			case DefAction.CHANGE_TRAM_PARTS:
				action = new GameActionChangeTramParts();
				break;
			case DefAction.REBUILD_NAVMESH:
				action = new GameActionRebuildNavMesh();
				break;
			case DefAction.RESPAWN_ARCHIVED_OBJECT:
				if (array.Length != 1)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				action = new GameActionRespawnArchivedObject();
				break;
			case DefAction.PLAY_ANIMATION:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				string animationName = array[1];
				action = new GameActionPlayAnimation(animationName);
				break;
			}
			case DefAction.PLAY_CUTSCENE:
			{
				if (array.Length != 2)
				{
					Logger.RWarn($"[ConvertString2Action] Format Error : {data}");
					return false;
				}
				string cutsceneName = array[1];
				action = new GameActionPlayCutscene(cutsceneName, needToBroadCast: true);
				break;
			}
			case DefAction.CHARGE_ITEM:
				action = new GameActionChargeItem();
				break;
			}
			if (action == null)
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			Logger.RWarn("token string " + data + " ex : " + ex.Message);
			return false;
		}
		return true;
	}
}
