using UnityEngine;

public class LevelObjectBoundElement : MonoBehaviour
{
	public bool onlyUseFront;

	public bool pickable = true;

	public bool blockable;

	public Bounds bound;

	public Vector3 GetGlobalBottomCenterPos(float y)
	{
		Vector3 globalCenterPos = GetGlobalCenterPos();
		globalCenterPos.y = y;
		return globalCenterPos;
	}

	public Vector3 GetGlobalCenterPos()
	{
		return base.transform.position + Vector3.Scale(bound.center, base.transform.lossyScale);
	}

	public Vector3 GetGlobalSize()
	{
		return Vector3.Scale(bound.size, base.transform.lossyScale);
	}

	public bool IsContainPosition(Vector3 targetPosition)
	{
		Vector3 vector = base.transform.InverseTransformPoint(targetPosition);
		Vector3 vector2 = bound.size * 0.5f;
		Vector3 center = bound.center;
		if (Mathf.Abs(vector.x - center.x) <= vector2.x && Mathf.Abs(vector.y - center.y) <= vector2.y)
		{
			return Mathf.Abs(vector.z - center.z) <= vector2.z;
		}
		return false;
	}

	public void DrawGizmos(Color color)
	{
		Gizmos.color = color;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(bound.center, bound.extents * 2f);
		if (onlyUseFront)
		{
			Color color2 = Gizmos.color;
			Gizmos.color = Color.red;
			var (vector, vector2) = GetFrontPlaneLocal();
			if (vector.x == vector2.x)
			{
				Gizmos.DrawLine(vector, new Vector3(vector.x, vector2.y, vector.z));
				Gizmos.DrawLine(new Vector3(vector.x, vector2.y, vector.z), new Vector3(vector.x, vector2.y, vector2.z));
				Gizmos.DrawLine(new Vector3(vector.x, vector2.y, vector2.z), new Vector3(vector.x, vector.y, vector2.z));
				Gizmos.DrawLine(new Vector3(vector.x, vector.y, vector2.z), vector);
			}
			else if (vector.y == vector2.y)
			{
				Gizmos.DrawLine(vector, new Vector3(vector.x, vector2.y, vector.z));
				Gizmos.DrawLine(new Vector3(vector2.x, vector.y, vector.z), new Vector3(vector2.y, vector.z, vector2.z));
				Gizmos.DrawLine(new Vector3(vector2.x, vector.y, vector2.z), new Vector3(vector.y, vector.z, vector2.z));
				Gizmos.DrawLine(new Vector3(vector.x, vector.y, vector2.z), vector);
			}
			else if (vector.z == vector2.z)
			{
				Gizmos.DrawLine(vector, new Vector3(vector.x, vector2.y, vector.z));
				Gizmos.DrawLine(new Vector3(vector.x, vector2.y, vector.z), new Vector3(vector2.x, vector2.y, vector.z));
				Gizmos.DrawLine(new Vector3(vector2.x, vector2.y, vector.z), new Vector3(vector2.x, vector.y, vector.z));
				Gizmos.DrawLine(new Vector3(vector2.x, vector.y, vector.z), vector);
			}
			Gizmos.color = color2;
		}
	}

	private (Vector3, Vector3) GetFrontPlaneLocal()
	{
		Vector3 min = bound.min;
		Vector3 max = bound.max;
		return (new Vector3(min.x, min.y, max.z), new Vector3(max.x, max.y, max.z));
	}
}
