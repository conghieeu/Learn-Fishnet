using UnityEngine;
using UnityEngine.UI;

public class ExpandedHitArea : MaskableGraphic, ICanvasRaycastFilter
{
	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		return true;
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
