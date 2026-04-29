using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.CycleCutsceneData;

public class DungeonCycleInfo
{
	public ImmutableDictionary<int, ImmutableArray<string>> EnterCutSceneNameDict = ImmutableDictionary<int, ImmutableArray<string>>.Empty;

	public ImmutableDictionary<int, ImmutableArray<string>> ExitCutSceneNameDict = ImmutableDictionary<int, ImmutableArray<string>>.Empty;

	public ImmutableDictionary<int, ImmutableArray<IGameAction>> EnterActionDict = ImmutableDictionary<int, ImmutableArray<IGameAction>>.Empty;

	public ImmutableDictionary<int, ImmutableArray<IGameAction>> ExitActionDict = ImmutableDictionary<int, ImmutableArray<IGameAction>>.Empty;

	public ImmutableDictionary<int, string> DestructionPartsNames = ImmutableDictionary<int, string>.Empty;

	public DungeonCycleInfo(CycleCutsceneData_MasterData data)
	{
		ImmutableDictionary<int, ImmutableArray<IGameAction>>.Builder builder = ImmutableDictionary.CreateBuilder<int, ImmutableArray<IGameAction>>();
		foreach (CycleCutsceneData_dungeon item in data.CycleCutsceneData_dungeonval)
		{
			if (!CondActionObjParser.GenerateActionGroup(item.enter_event_action, "", out var actionGroup))
			{
				Logger.RWarn("DungeonCycleInfo: GenerateActionGroup failed for EnterAction");
			}
			else
			{
				builder.Add(item.daycount, actionGroup);
			}
		}
		EnterActionDict = builder.ToImmutable();
		ImmutableDictionary<int, ImmutableArray<IGameAction>>.Builder builder2 = ImmutableDictionary.CreateBuilder<int, ImmutableArray<IGameAction>>();
		foreach (CycleCutsceneData_dungeon item2 in data.CycleCutsceneData_dungeonval)
		{
			if (!CondActionObjParser.GenerateActionGroup(item2.exit_event_action, "", out var actionGroup2))
			{
				Logger.RWarn("DungeonCycleInfo: GenerateActionGroup failed for ExitAction");
			}
			else
			{
				builder2.Add(item2.daycount, actionGroup2);
			}
		}
		ExitActionDict = builder2.ToImmutable();
		ImmutableDictionary<int, ImmutableArray<string>>.Builder builder3 = ImmutableDictionary.CreateBuilder<int, ImmutableArray<string>>();
		foreach (CycleCutsceneData_dungeon item3 in data.CycleCutsceneData_dungeonval)
		{
			ImmutableArray<string> value = item3.enter_cutscenes.ToImmutableArray();
			builder3.Add(item3.daycount, value);
		}
		EnterCutSceneNameDict = builder3.ToImmutable();
		ImmutableDictionary<int, ImmutableArray<string>>.Builder builder4 = ImmutableDictionary.CreateBuilder<int, ImmutableArray<string>>();
		foreach (CycleCutsceneData_dungeon item4 in data.CycleCutsceneData_dungeonval)
		{
			ImmutableArray<string> value2 = item4.exit_cutscenes.ToImmutableArray();
			builder4.Add(item4.daycount, value2);
		}
		ExitCutSceneNameDict = builder4.ToImmutable();
		ImmutableDictionary<int, string>.Builder builder5 = ImmutableDictionary.CreateBuilder<int, string>();
		foreach (CycleCutsceneData_dungeon item5 in data.CycleCutsceneData_dungeonval)
		{
			builder5.Add(item5.daycount, item5.tram_inner_destroyed_set_name);
		}
		DestructionPartsNames = builder5.ToImmutable();
	}

	public ImmutableArray<IGameAction> GetEnterAction(int dayCount)
	{
		if (EnterActionDict.TryGetValue(dayCount, out ImmutableArray<IGameAction> value))
		{
			return value;
		}
		return ImmutableArray<IGameAction>.Empty;
	}

	public ImmutableArray<IGameAction> GetExitAction(int dayCount)
	{
		if (ExitActionDict.TryGetValue(dayCount, out ImmutableArray<IGameAction> value))
		{
			return value;
		}
		return ImmutableArray<IGameAction>.Empty;
	}

	public List<string> GetEnterCutSceneNames(int dayCount)
	{
		if (EnterCutSceneNameDict.TryGetValue(dayCount, out ImmutableArray<string> value))
		{
			return value.ToList();
		}
		return new List<string>();
	}

	public List<string> GetExitCutSceneNames(int dayCount)
	{
		if (ExitCutSceneNameDict.TryGetValue(dayCount, out ImmutableArray<string> value))
		{
			return value.ToList();
		}
		return new List<string>();
	}
}
