using Bifrost.CycleCutsceneData;

public class CycleMasterInfo
{
	public readonly int MasterID;

	public readonly int CycleCount;

	public readonly int Quota;

	public MaintenanceRoomCycleInfo MaintenanceRoomCycleInfo;

	public WaitingRoomCycleInfo WaitingRoomCycleInfo;

	public DungeonCycleInfo DungeonCycleInfo;

	public DeathMatchRoomCycleInfo DeathMatchRoomCycleInfo;

	public CycleMasterInfo(CycleCutsceneData_MasterData data)
	{
		MasterID = data.id;
		CycleCount = data.cycle;
		Quota = data.cycle_quota;
		MaintenanceRoomCycleInfo = new MaintenanceRoomCycleInfo(data);
		WaitingRoomCycleInfo = new WaitingRoomCycleInfo(data);
		DungeonCycleInfo = new DungeonCycleInfo(data);
		DeathMatchRoomCycleInfo = new DeathMatchRoomCycleInfo(data);
	}
}
