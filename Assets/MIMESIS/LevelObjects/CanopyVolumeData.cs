using UnityEngine;

public class CanopyVolumeData
{
	public readonly Bounds Bounds;

	public CanopyVolumeData(MapTrigger spawnPointData, Bounds bounds)
	{
		Bounds = bounds;
	}
}
