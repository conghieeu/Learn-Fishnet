using UnityEngine;

public class MapMarker_TeleportStartPoint : MapMarker_TeleportPoint
{
	[SerializeField]
	private string _endCallSign;

	public string EndCallSign => _endCallSign;

	protected new void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		MapMarker_TeleportEndPoint[] array = Object.FindObjectsOfType<MapMarker_TeleportEndPoint>();
		foreach (MapMarker_TeleportEndPoint mapMarker_TeleportEndPoint in array)
		{
			if (mapMarker_TeleportEndPoint.CallSign == _endCallSign)
			{
				Gizmos.color = iconColor;
				Hub.DrawGizmo_Arrow(base.transform.position, mapMarker_TeleportEndPoint.transform.position, iconColor);
			}
		}
	}
}
