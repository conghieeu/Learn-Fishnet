using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.CycleCutsceneData;

public class MaintenanceRoomCycleInfo
{
	public readonly int ShopGroupID;

	public ImmutableArray<string> TramPartsNames = ImmutableArray<string>.Empty;

	public ImmutableArray<string> EnterCutSceneNames = ImmutableArray<string>.Empty;

	public ImmutableArray<string> ExitCutSceneNames = ImmutableArray<string>.Empty;

	public ImmutableArray<IGameAction> EnterActions = ImmutableArray<IGameAction>.Empty;

	public ImmutableArray<IGameAction> ExitActions = ImmutableArray<IGameAction>.Empty;

	public ImmutableArray<IGameAction> RepairActions = ImmutableArray<IGameAction>.Empty;

	public string DestructionPartsName = "";

	public MaintenanceRoomCycleInfo(CycleCutsceneData_MasterData data)
	{
		ShopGroupID = data.shop_group;
		ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>();
		foreach (string item in data.tram_add_parts_name)
		{
			if (!string.IsNullOrEmpty(item))
			{
				builder.Add(item);
			}
		}
		TramPartsNames = builder.ToImmutable();
		ImmutableArray<string>.Builder builder2 = ImmutableArray.CreateBuilder<string>();
		foreach (string item2 in data.maintenance_enter_cutscene_list)
		{
			if (!string.IsNullOrEmpty(item2))
			{
				builder2.Add(item2);
			}
		}
		EnterCutSceneNames = builder2.ToImmutable();
		ImmutableArray<string>.Builder builder3 = ImmutableArray.CreateBuilder<string>();
		foreach (string item3 in data.maintenance_exit_cutscene_list)
		{
			if (!string.IsNullOrEmpty(item3))
			{
				builder3.Add(item3);
			}
		}
		ExitCutSceneNames = builder3.ToImmutable();
		DestructionPartsName = data.tram_inner_destroyed_set_name;
		if (!CondActionObjParser.GenerateActionGroup(data.maintenance_enter_event_action, "", out EnterActions))
		{
			Logger.RWarn("MaintenenceRoomMasterInfo: GenerateActionGroup failed");
		}
		if (!CondActionObjParser.GenerateActionGroup(data.maintenance_exit_event_action, "", out ExitActions))
		{
			Logger.RWarn("MaintenenceRoomMasterInfo: GenerateActionGroup failed");
		}
		if (!CondActionObjParser.GenerateActionGroup(data.maintenance_repair_event_action, "", out RepairActions))
		{
			Logger.RWarn("MaintenenceRoomMasterInfo: GenerateActionGroup failed");
		}
	}

	public ImmutableArray<string> GetTramPartsNames()
	{
		return TramPartsNames;
	}

	public List<string> GetEnterCutSceneNames()
	{
		return EnterCutSceneNames.ToList();
	}

	public ImmutableArray<IGameAction> GetEnterActions()
	{
		return EnterActions;
	}

	public ImmutableArray<IGameAction> GetExitActions()
	{
		return ExitActions;
	}

	public ImmutableArray<IGameAction> GetRepairActions()
	{
		return RepairActions;
	}

	public List<string> GetExitCutSceneNames()
	{
		return ExitCutSceneNames.ToList();
	}
}
