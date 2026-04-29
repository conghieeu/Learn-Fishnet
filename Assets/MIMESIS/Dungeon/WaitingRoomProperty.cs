using UnityEngine;

public class WaitingRoomProperty : IVRoomProperty
{
	public WaitingRoomProperty(long sessionID, Vector2 min, Vector2 max, int sectorSize, int targetCurrency)
		: base(sessionID, VRoomType.Waiting, min, max, sectorSize, targetCurrency)
	{
	}
}
