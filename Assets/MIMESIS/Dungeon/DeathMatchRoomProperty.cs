using UnityEngine;

public class DeathMatchRoomProperty : IVRoomProperty
{
	public DeathMatchRoomProperty(long sessionID, Vector2 min, Vector2 max, int sectorSize, int targetCurrency)
		: base(sessionID, VRoomType.DeathMatch, min, max, sectorSize, targetCurrency)
	{
	}
}
