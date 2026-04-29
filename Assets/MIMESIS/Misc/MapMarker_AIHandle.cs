using UnityEngine;

public class MapMarker_AIHandle : MapMarker
{
	protected void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon(base.transform.position, "icon_AIHandle.png", allowScaling: true, iconColor);
		LevelObject componentInParent = GetComponentInParent<LevelObject>();
		if (!(componentInParent == null))
		{
			Hub.DrawGizmo_Arrow(base.transform.position, componentInParent.transform.position, iconColor);
		}
	}
}
