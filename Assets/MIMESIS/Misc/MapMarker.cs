using UnityEngine;

[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=5177084409")]
public class MapMarker : MonoBehaviour
{
	[SerializeField]
	protected Color iconColor = Color.white;

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "icon_mapMarker.png", allowScaling: true, iconColor);
	}
}
