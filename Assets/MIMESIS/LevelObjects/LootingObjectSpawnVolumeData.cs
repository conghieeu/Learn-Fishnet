using UnityEngine;

public class LootingObjectSpawnVolumeData
{
	public readonly Bounds Bounds;

	public readonly Vector3 Position;

	public readonly Quaternion Rotation;

	public readonly float FloorY;

	public LootingObjectSpawnVolumeData(MapTrigger spawnPointData, Bounds bounds)
	{
		Bounds = bounds;
		Position = spawnPointData.transform.position;
		Rotation = spawnPointData.transform.rotation;
		FloorY = Position.y;
		Vector3 vector = Hub.s.navman.RayCastWithPhysics(Position, Position + Vector3.down * 10f);
		if (vector != Vector3.zero)
		{
			FloorY = vector.y;
		}
	}
}
