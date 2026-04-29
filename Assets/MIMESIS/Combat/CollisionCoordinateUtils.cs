using UnityEngine;

public static class CollisionCoordinateUtils
{
	public static (Vector3 fromLocal, Vector3 toLocal) TransformToOriginLocal(Vector3 originWorld, float originAngleRad, Vector3 targetFromWorld, Vector3 targetToWorld, bool applyRotation = true)
	{
		Vector3 vector = targetFromWorld - originWorld;
		Vector3 vector2 = targetToWorld - originWorld;
		if (applyRotation)
		{
			Vector3 item = vector.RotateY(0f - originAngleRad);
			Vector3 item2 = vector2.RotateY(0f - originAngleRad);
			return (fromLocal: item, toLocal: item2);
		}
		return (fromLocal: vector, toLocal: vector2);
	}

	public static (Vector3 fromLocal, Vector3 toLocal) TransformToOriginLocalWithCenter(Vector3 originWorld, float originAngleRad, Vector3 originHitCheckCenter, float originHitBoxYawRad, Vector3 targetFromWorld, Vector3 targetToWorld, Vector3 targetHitCheckCenter, float targetFromAngleRad, float targetToAngleRad, bool applyRotation = true)
	{
		Vector3 vector = originWorld;
		if (applyRotation)
		{
			Vector3 vector2 = originHitCheckCenter.RotateY(originHitBoxYawRad).RotateY(originAngleRad);
			vector += vector2;
		}
		else
		{
			vector += originHitCheckCenter;
		}
		Vector3 vector3 = targetFromWorld;
		Vector3 vector4 = targetToWorld;
		if (applyRotation)
		{
			Vector3 vector5 = targetHitCheckCenter.RotateY(targetFromAngleRad);
			vector3 += vector5;
			Vector3 vector6 = targetHitCheckCenter.RotateY(targetToAngleRad);
			vector4 += vector6;
		}
		else
		{
			vector3 += targetHitCheckCenter;
			vector4 += targetHitCheckCenter;
		}
		Vector3 vector7 = vector3 - vector;
		Vector3 vector8 = vector4 - vector;
		if (applyRotation)
		{
			float num = originHitBoxYawRad + originAngleRad;
			Vector3 item = vector7.RotateY(0f - num);
			Vector3 item2 = vector8.RotateY(0f - num);
			return (fromLocal: item, toLocal: item2);
		}
		return (fromLocal: vector7, toLocal: vector8);
	}

	public static (Vector3 fromLocal, Vector3 toLocal) TransformToOriginLocalWithCenter(Vector3 originWorld, float originAngleRad, Vector3 originHitCheckCenter, float originHitBoxYawRad, Vector3 targetFromWorld, Vector3 targetToWorld, Vector3 targetHitCheckCenter, float targetAngleRad, bool applyRotation = true)
	{
		return TransformToOriginLocalWithCenter(originWorld, originAngleRad, originHitCheckCenter, originHitBoxYawRad, targetFromWorld, targetToWorld, targetHitCheckCenter, targetAngleRad, targetAngleRad, applyRotation);
	}

	public static Vector3 TransformDirectionToOriginLocal(Vector3 worldDirection, float originAngleRad)
	{
		return worldDirection.RotateY(0f - originAngleRad);
	}

	public static Vector3 TransformToWorldFromOriginLocal(Vector3 localPos, Vector3 originWorld, float originAngleRad)
	{
		Vector3 vector = localPos.RotateY(originAngleRad);
		return originWorld + vector;
	}

	public static Vector3 TransformToWorldFromOriginLocalWithCenter(Vector3 localPos, Vector3 originWorld, float originAngleRad, Vector3 originHitCheckCenter)
	{
		Vector3 vector = localPos.RotateY(originAngleRad);
		Vector3 vector2 = originWorld;
		if (originHitCheckCenter != Vector3.zero)
		{
			Vector3 vector3 = originHitCheckCenter.RotateY(originAngleRad);
			vector2 += vector3;
		}
		return vector2 + vector;
	}
}
