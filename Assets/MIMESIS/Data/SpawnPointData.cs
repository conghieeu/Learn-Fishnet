using ReluProtocol;

public class SpawnPointData
{
	public readonly int Index;

	public readonly PosWithRot Pos;

	public readonly bool IsInDoor;

	public readonly int MasterID;

	public readonly bool IsFirstSpawnPoint;

	public SpawnPointData(int index, PosWithRot pos, bool isInDoor, int masterID = 0, bool firstSpawnPoint = false)
	{
		Index = index;
		Pos = pos.Clone();
		IsInDoor = isInDoor;
		MasterID = masterID;
		IsFirstSpawnPoint = firstSpawnPoint;
	}
}
