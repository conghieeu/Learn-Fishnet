using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.CycleCutsceneData;

public class WaitingRoomCycleInfo
{
	public ImmutableDictionary<int, ImmutableArray<IGameAction>> EnterActionDict = ImmutableDictionary<int, ImmutableArray<IGameAction>>.Empty;

	public ImmutableDictionary<int, ImmutableArray<IGameAction>> ExitActionDict = ImmutableDictionary<int, ImmutableArray<IGameAction>>.Empty;

	public ImmutableDictionary<int, ImmutableArray<string>> EnterCutsceneNames = ImmutableDictionary<int, ImmutableArray<string>>.Empty;

	public ImmutableDictionary<int, ImmutableArray<string>> ExitCutsceneNames = ImmutableDictionary<int, ImmutableArray<string>>.Empty;

	public ImmutableDictionary<int, string> DestructionPartsNames = ImmutableDictionary<int, string>.Empty;

	public WaitingRoomCycleInfo(CycleCutsceneData_MasterData data)
	{
		ImmutableDictionary<int, ImmutableArray<IGameAction>>.Builder builder = ImmutableDictionary.CreateBuilder<int, ImmutableArray<IGameAction>>();
		foreach (CycleCutsceneData_waiting_room item in data.CycleCutsceneData_waiting_roomval)
		{
			if (!CondActionObjParser.GenerateActionGroup(item.enter_event_action, "", out var actionGroup))
			{
				Logger.RWarn("WaitingRoomCycleInfo: GenerateActionGroup failed for EnterAction");
			}
			else
			{
				builder.Add(item.daycount, actionGroup);
			}
		}
		EnterActionDict = builder.ToImmutable();
		ImmutableDictionary<int, ImmutableArray<IGameAction>>.Builder builder2 = ImmutableDictionary.CreateBuilder<int, ImmutableArray<IGameAction>>();
		foreach (CycleCutsceneData_waiting_room item2 in data.CycleCutsceneData_waiting_roomval)
		{
			if (!CondActionObjParser.GenerateActionGroup(item2.exit_event_action, "", out var actionGroup2))
			{
				Logger.RWarn("WaitingRoomCycleInfo: GenerateActionGroup failed for ExitAction");
			}
			else
			{
				builder2.Add(item2.daycount, actionGroup2);
			}
		}
		ExitActionDict = builder2.ToImmutable();
		ImmutableDictionary<int, ImmutableArray<string>>.Builder builder3 = ImmutableDictionary.CreateBuilder<int, ImmutableArray<string>>();
		foreach (CycleCutsceneData_waiting_room item3 in data.CycleCutsceneData_waiting_roomval)
		{
			ImmutableArray<string> value = item3.enter_cutscenes.ToImmutableArray();
			builder3.Add(item3.daycount, value);
		}
		EnterCutsceneNames = builder3.ToImmutable();
		ImmutableDictionary<int, ImmutableArray<string>>.Builder builder4 = ImmutableDictionary.CreateBuilder<int, ImmutableArray<string>>();
		foreach (CycleCutsceneData_waiting_room item4 in data.CycleCutsceneData_waiting_roomval)
		{
			ImmutableArray<string> value2 = item4.exit_cutscenes.ToImmutableArray();
			builder4.Add(item4.daycount, value2);
		}
		ExitCutsceneNames = builder4.ToImmutable();
		ImmutableDictionary<int, string>.Builder builder5 = ImmutableDictionary.CreateBuilder<int, string>();
		foreach (CycleCutsceneData_waiting_room item5 in data.CycleCutsceneData_waiting_roomval)
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

	public List<string> GetEnterCutSceneNames(int dayCount)
	{
		if (EnterCutsceneNames.TryGetValue(dayCount, out ImmutableArray<string> value))
		{
			return value.ToList();
		}
		return new List<string>();
	}

	public ImmutableArray<IGameAction> GetExitAction(int dayCount)
	{
		if (ExitActionDict.TryGetValue(dayCount, out ImmutableArray<IGameAction> value))
		{
			return value;
		}
		return ImmutableArray<IGameAction>.Empty;
	}

	public List<string> GetExitCutSceneNames(int dayCount)
	{
		if (ExitCutsceneNames.TryGetValue(dayCount, out ImmutableArray<string> value))
		{
			return value.ToList();
		}
		return new List<string>();
	}
}
