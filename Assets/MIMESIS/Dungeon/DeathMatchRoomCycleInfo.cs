using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.CycleCutsceneData;

public class DeathMatchRoomCycleInfo
{
	public int contaValue;

	public ImmutableArray<IGameAction> EnterActions = ImmutableArray<IGameAction>.Empty;

	public ImmutableArray<IGameAction> EndActions = ImmutableArray<IGameAction>.Empty;

	public ImmutableArray<IGameAction> AllMonsterDeadActions = ImmutableArray<IGameAction>.Empty;

	public ImmutableArray<string> EnterCutSceneNames = ImmutableArray<string>.Empty;

	public ImmutableArray<string> ExitCutSceneNames = ImmutableArray<string>.Empty;

	public DeathMatchRoomCycleInfo(CycleCutsceneData_MasterData data)
	{
		contaValue = (int)data.deathmatch_conta;
		if (!CondActionObjParser.GenerateActionGroup(data.deathmatch_enter_event_action, "", out EnterActions))
		{
			Logger.RWarn("DeathMatchRoomCycleInfo: GenerateActionGroup failed for EnterActions");
		}
		if (!CondActionObjParser.GenerateActionGroup(data.deathmatch_end_event_action, "", out EndActions))
		{
			Logger.RWarn("DeathMatchRoomCycleInfo: GenerateActionGroup failed for EndActions");
		}
		if (!CondActionObjParser.GenerateActionGroup(data.deathmatch_monster_all_death_event_action, "", out AllMonsterDeadActions))
		{
			Logger.RWarn("DeathMatchRoomCycleInfo: GenerateActionGroup failed for AllMonsterDeadActions");
		}
		ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>();
		foreach (string item in data.deathmatch_enter_cutscene_list)
		{
			if (!string.IsNullOrEmpty(item))
			{
				builder.Add(item);
			}
		}
		EnterCutSceneNames = builder.ToImmutable();
		ImmutableArray<string>.Builder builder2 = ImmutableArray.CreateBuilder<string>();
		foreach (string item2 in data.deathmatch_end_cutscene_list)
		{
			if (!string.IsNullOrEmpty(item2))
			{
				builder2.Add(item2);
			}
		}
		ExitCutSceneNames = builder2.ToImmutable();
	}

	public ImmutableArray<IGameAction> GetEnterActions()
	{
		return EnterActions;
	}

	public ImmutableArray<IGameAction> GetEndActions()
	{
		return EndActions;
	}

	public List<string> GetEnterCutSceneNames()
	{
		return EnterCutSceneNames.ToList();
	}

	public List<string> GetExitCutSceneNames()
	{
		return ExitCutSceneNames.ToList();
	}

	public ImmutableArray<IGameAction> GetAllMonsterDeadActions()
	{
		return AllMonsterDeadActions;
	}
}
