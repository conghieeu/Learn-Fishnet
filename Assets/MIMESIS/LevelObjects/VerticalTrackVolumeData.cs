using System.Collections.Generic;
using UnityEngine;

public class VerticalTrackVolumeData
{
	public readonly Bounds Bounds;

	public readonly MapTrigger.eUsageType UsageType;

	public List<Vector3> HolePoints = new List<Vector3>();

	public VerticalTrackVolumeData(MapTrigger mapTriggerData, Bounds bounds)
	{
		Bounds = bounds;
		UsageType = mapTriggerData.usageType;
		foreach (MapMarker_HolePoint holePoint in mapTriggerData.holePoints)
		{
			HolePoints.Add(holePoint.Pos.toVector3());
		}
	}

	public bool IsInBounds(Vector3 position)
	{
		return Bounds.Contains(position);
	}
}
