using ReluProtocol;
using UnityEngine;

public static class PhysicsUtility
{
	private const int MaxHits = 32;

	private static readonly RaycastHit[] _hits = new RaycastHit[32];

	public static PosWithRot GetFloorPosWithRot(PosWithRot pos)
	{
		PosWithRot posWithRot = pos.Clone();
		Vector3 vector = Hub.s.navman.RayCastWithPhysics(pos.toVector3(), pos.toVector3() + Vector3.down * 10f);
		if (vector != Vector3.zero)
		{
			posWithRot.y = vector.y;
		}
		return posWithRot;
	}

	public static bool CheckVisibility(Vector3 origin, Vector3 target, float originRadius, float targetRadius, float originTop, float targetTop)
	{
		Vector3 lhs = target - origin;
		_ = lhs.magnitude;
		lhs.Normalize();
		Vector3 vector = new Vector3(origin.x, origin.y + originTop, origin.z);
		Vector3 vector2 = new Vector3(target.x, target.y + targetTop, target.z);
		Vector3 rayOrigin = new Vector3(origin.x, origin.y + originTop * 0.5f, origin.z);
		Vector3 rayTarget = new Vector3(target.x, target.y + targetTop * 0.5f, target.z);
		Vector3 vector3 = Vector3.Cross(lhs, Vector3.up);
		Vector3 rayOrigin2 = vector + vector3 * originRadius;
		Vector3 rayTarget2 = vector2 + vector3 * targetRadius;
		Vector3 rayOrigin3 = vector - vector3 * originRadius;
		Vector3 rayTarget3 = vector2 - vector3 * targetRadius;
		if (CheckBlockByWall(rayOrigin2, rayTarget2))
		{
			return true;
		}
		if (CheckBlockByWall(rayOrigin3, rayTarget3))
		{
			return true;
		}
		if (CheckBlockByWall(rayOrigin, rayTarget))
		{
			return true;
		}
		return false;
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) GetWallHitPos(Vector3 origin, Vector3 target)
	{
		Vector3 direction = target - origin;
		float magnitude = direction.magnitude;
		direction.Normalize();
		if (Physics.Raycast(origin, direction, out var hitInfo, magnitude, Hub.DefaultOnlyLayerMask, QueryTriggerInteraction.Ignore))
		{
			return (isHit: true, hitPos: hitInfo.point, normal: hitInfo.normal);
		}
		return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
	}

	private static bool CheckBlockByWall(Vector3 rayOrigin, Vector3 rayTarget)
	{
		Vector3 direction = rayTarget - rayOrigin;
		float magnitude = direction.magnitude;
		direction.Normalize();
		return !Physics.Raycast(rayOrigin, direction, magnitude, Hub.DefaultOnlyLayerMask, QueryTriggerInteraction.Ignore);
	}
}
