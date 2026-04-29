using UnityEngine;

public class MaintenanceRoomProperty : IVRoomProperty
{
	public readonly string HostToken;

	public readonly int SaveSlotID;

	public MaintenanceRoomProperty(long sessionID, string hostToken, int saveSlotID, Vector2 min, Vector2 max, int sectorSize, int targetCurrency)
		: base(sessionID, VRoomType.Maintenance, min, max, sectorSize, targetCurrency)
	{
		HostToken = hostToken;
		SaveSlotID = saveSlotID;
	}
}
