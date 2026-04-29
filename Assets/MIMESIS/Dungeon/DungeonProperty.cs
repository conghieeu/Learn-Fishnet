using UnityEngine;

public class DungeonProperty : IVRoomProperty
{
	public readonly string HostToken;

	public readonly int DungeonMasterID;

	public readonly int RandomDungeonSeed;

	public readonly int DayCount;

	public DungeonProperty(long sessionID, int dungeonMasterID, int randomDungeonSeed, string hostToken, Vector2 min, Vector2 max, int sectorSize, int targetCurrency, int dayCount)
		: base(sessionID, VRoomType.Game, min, max, sectorSize, targetCurrency)
	{
		DungeonMasterID = dungeonMasterID;
		RandomDungeonSeed = randomDungeonSeed;
		HostToken = hostToken;
		DayCount = dayCount;
	}
}
