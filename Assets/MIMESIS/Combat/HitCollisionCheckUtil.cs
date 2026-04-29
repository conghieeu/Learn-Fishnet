using System;
using UnityEngine;

public static class HitCollisionCheckUtil
{
	public static (bool isHit, Vector3 hitPos, Vector3 normal) IsHit(this IHitCheck collisionOrigin, HitCheckPos posOrigin, IHitCheck collisionTarget, HitCheckPos posTarget)
	{
		(posOrigin, posTarget) = AdjustHeightFromBottom(posOrigin, posTarget);
		if (!(collisionOrigin is SphereHitCheck sphere))
		{
			if (!(collisionOrigin is CubeHitCheck cube))
			{
				if (!(collisionOrigin is CapsuleHitCheck capsule))
				{
					if (!(collisionOrigin is FanHitCheck fan))
					{
						if (collisionOrigin is TorusHitCheck torus)
						{
							return HandleTorusCollision(torus, posOrigin, collisionTarget, posTarget);
						}
						throw new Exception("Invalid HitCheckShapeType");
					}
					return HandleFanCollision(fan, posOrigin, collisionTarget, posTarget);
				}
				return HandleCapsuleCollision(capsule, posOrigin, collisionTarget, posTarget);
			}
			return HandleCubeCollision(cube, posOrigin, collisionTarget, posTarget);
		}
		return HandleSphereCollision(sphere, posOrigin, collisionTarget, posTarget);
	}

	private static (HitCheckPos, HitCheckPos) AdjustHeightFromBottom(HitCheckPos origin, HitCheckPos target)
	{
		return (new HitCheckPos
		{
			Start = origin.Start + Vector3.up * 0.6f,
			End = origin.End + Vector3.up * 0.6f,
			AngleRad = origin.AngleRad
		}, new HitCheckPos
		{
			Start = target.Start + Vector3.up * 0.6f,
			End = target.End + Vector3.up * 0.6f,
			AngleRad = target.AngleRad
		});
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleSphereCollision(SphereHitCheck sphere, HitCheckPos posOrigin, IHitCheck collisionTarget, HitCheckPos posTarget)
	{
		if (!(collisionTarget is CapsuleHitCheck targetCapsule))
		{
			if (!(collisionTarget is SphereHitCheck targetSphere))
			{
				if (collisionTarget is CubeHitCheck targetCube)
				{
					return HandleSphere2CubeCollision(sphere, posOrigin, targetCube, posTarget);
				}
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			return HandleSphere2SphereCollision(sphere, posOrigin, targetSphere, posTarget);
		}
		return HandleSphere2CapsuleCollision(sphere, posOrigin, targetCapsule, posTarget);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleSphere2CapsuleCollision(SphereHitCheck sphere, HitCheckPos posOrigin, CapsuleHitCheck targetCapsule, HitCheckPos posTarget)
	{
		(Vector3 fromLocal, Vector3 toLocal) tuple = CollisionCoordinateUtils.TransformToOriginLocal(posTarget.Start, posTarget.AngleRad, posOrigin.Start, posOrigin.End, applyRotation: false);
		Vector3 item = tuple.fromLocal;
		Vector3 item2 = tuple.toLocal;
		(bool, Vector3, Vector3) tuple2 = CalcCollision.CalcMovingCapsule2MovingSphere(posTarget.End - posTarget.Start, capsuleAxis: targetCapsule.Rotation.ToDirectionVector().RotateY(posTarget.AngleRad), capsuleRadius: targetCapsule.Rad, capsuleLength: targetCapsule.Length, sphereFromLocal: item, sphereToLocal: item2, sphereRadius: sphere.Rad);
		if (!tuple2.Item1)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 item3 = CollisionCoordinateUtils.TransformToWorldFromOriginLocal(tuple2.Item2, posTarget.Start, posTarget.AngleRad);
		Vector3 item4 = CollisionCoordinateUtils.TransformDirectionToOriginLocal(tuple2.Item3, 0f - posTarget.AngleRad);
		return (isHit: true, hitPos: item3, normal: item4);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleSphere2SphereCollision(SphereHitCheck sphere, HitCheckPos posOrigin, SphereHitCheck targetSphere, HitCheckPos posTarget)
	{
		(bool, Vector3, Vector3) tuple = CalcCollision.CalcMovingSphere2MovingSphere(posOrigin.Start, posOrigin.End, sphere.Rad, posTarget.Start, posTarget.End, targetSphere.Rad);
		if (!tuple.Item1)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return (isHit: true, hitPos: tuple.Item2, normal: tuple.Item3);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleSphere2CubeCollision(SphereHitCheck sphere, HitCheckPos posOrigin, CubeHitCheck targetCube, HitCheckPos posTarget)
	{
		(Vector3 fromLocal, Vector3 toLocal) tuple = CollisionCoordinateUtils.TransformToOriginLocalWithCenter(posTarget.Start, posTarget.AngleRad, targetCube.Center, 0f, posOrigin.Start, posOrigin.End, sphere.Center, posOrigin.AngleRad);
		Vector3 item = tuple.fromLocal;
		Vector3 item2 = tuple.toLocal;
		Vector3 boxMovementLocal = CollisionCoordinateUtils.TransformDirectionToOriginLocal(posTarget.End - posTarget.Start, posTarget.AngleRad);
		var (flag, localPos, worldDirection) = CalcCollision.CalcMovingBox2MovingSphereXZ(targetCube.Extent, boxMovementLocal, item, item2, sphere.Rad);
		if (!flag)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 item3 = CollisionCoordinateUtils.TransformToWorldFromOriginLocalWithCenter(localPos, posTarget.Start, posTarget.AngleRad, targetCube.Center);
		Vector3 item4 = CollisionCoordinateUtils.TransformDirectionToOriginLocal(worldDirection, 0f - posTarget.AngleRad);
		return (isHit: true, hitPos: item3, normal: item4);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleCubeCollision(CubeHitCheck cube, HitCheckPos posOrigin, IHitCheck collisionTarget, HitCheckPos posTarget)
	{
		if (!(collisionTarget is CapsuleHitCheck targetCapsule))
		{
			if (!(collisionTarget is SphereHitCheck targetSphere))
			{
				if (collisionTarget is CubeHitCheck targetCube)
				{
					return HandleCube2CubeCollision(cube, posOrigin, targetCube, posTarget);
				}
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			return HandleCube2SphereCollision(cube, posOrigin, targetSphere, posTarget);
		}
		return HandleCube2CapsuleCollision(cube, posOrigin, targetCapsule, posTarget);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleCube2CapsuleCollision(CubeHitCheck cube, HitCheckPos posOrigin, CapsuleHitCheck targetCapsule, HitCheckPos posTarget)
	{
		(Vector3 fromLocal, Vector3 toLocal) tuple = CollisionCoordinateUtils.TransformToOriginLocalWithCenter(posOrigin.Start, posOrigin.AngleRad, cube.Center, cube.Rotation.Yaw.toRadian(), posTarget.Start, posTarget.End, targetCapsule.Center, posTarget.AngleRad);
		Vector3 item = tuple.fromLocal;
		Vector3 item2 = tuple.toLocal;
		Vector3 normalized = CollisionCoordinateUtils.TransformDirectionToOriginLocal(targetCapsule.Rotation.ToDirectionVector(), posOrigin.AngleRad).normalized;
		var (flag, localPos, worldDirection) = CalcCollision.CalcMovingCapsule2OriginBox(cube.Extent, item, item2, normalized, targetCapsule.Rad, targetCapsule.Length);
		if (!flag)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 item3 = CollisionCoordinateUtils.TransformToWorldFromOriginLocalWithCenter(localPos, posOrigin.Start, posOrigin.AngleRad, cube.Center);
		Vector3 item4 = CollisionCoordinateUtils.TransformDirectionToOriginLocal(worldDirection, 0f - posOrigin.AngleRad);
		return (isHit: true, hitPos: item3, normal: item4);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleCube2SphereCollision(CubeHitCheck cube, HitCheckPos posOrigin, SphereHitCheck targetSphere, HitCheckPos posTarget)
	{
		(Vector3 fromLocal, Vector3 toLocal) tuple = CollisionCoordinateUtils.TransformToOriginLocalWithCenter(posOrigin.End, posOrigin.AngleRad, cube.Center, cube.Rotation.Yaw.toRadian(), posTarget.Start, posTarget.End, targetSphere.Center, posTarget.AngleRad);
		Vector3 item = tuple.fromLocal;
		Vector3 item2 = tuple.toLocal;
		Vector3 boxMovementLocal = posOrigin.End - posOrigin.Start;
		(bool, Vector3, Vector3) tuple2 = CalcCollision.CalcMovingBox2MovingSphereXZ(cube.Extent * 0.5f, boxMovementLocal, item, item2, targetSphere.Rad);
		if (!tuple2.Item1)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 item3 = CollisionCoordinateUtils.TransformToWorldFromOriginLocalWithCenter(tuple2.Item2, posOrigin.Start, posOrigin.AngleRad, cube.Center);
		Vector3 item4 = CollisionCoordinateUtils.TransformDirectionToOriginLocal(tuple2.Item3, 0f - posOrigin.AngleRad);
		return (isHit: true, hitPos: item3, normal: item4);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleCube2CubeCollision(CubeHitCheck cube, HitCheckPos posOrigin, CubeHitCheck targetCube, HitCheckPos posTarget)
	{
		Vector3 item = CollisionCoordinateUtils.TransformToOriginLocalWithCenter(posOrigin.End, posOrigin.AngleRad, cube.Center, cube.Rotation.Yaw.toRadian(), posTarget.Start, posTarget.End, targetCube.Center, posTarget.AngleRad).toLocal;
		Quaternion quaternion = targetCube.Rotation.ToQuaternion() * Quaternion.Euler(0f, posTarget.AngleRad * 57.29578f, 0f);
		Vector3[] array = new Vector3[3]
		{
			quaternion * Vector3.right * (targetCube.Extent.x * 0.5f),
			quaternion * Vector3.up * (targetCube.Extent.y * 0.5f),
			quaternion * Vector3.forward * (targetCube.Extent.z * 0.5f)
		};
		Vector3[] array2 = new Vector3[3];
		for (int i = 0; i < 3; i++)
		{
			array2[i] = CollisionCoordinateUtils.TransformDirectionToOriginLocal(array[i], posOrigin.AngleRad);
		}
		(bool, Vector3, Vector3) tuple = CalcCollision.CalcStoppedBox2StoppedBox(cube.Extent, targetCube.Extent, item, array2);
		if (!tuple.Item1)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 item2 = CollisionCoordinateUtils.TransformToWorldFromOriginLocalWithCenter(tuple.Item2, posOrigin.Start, posOrigin.AngleRad, cube.Center);
		Vector3 item3 = CollisionCoordinateUtils.TransformDirectionToOriginLocal(tuple.Item3, 0f - posOrigin.AngleRad);
		return (isHit: true, hitPos: item2, normal: item3);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleCapsuleCollision(CapsuleHitCheck capsule, HitCheckPos posOrigin, IHitCheck collisionTarget, HitCheckPos posTarget)
	{
		if (!(collisionTarget is CapsuleHitCheck targetCapsule))
		{
			if (!(collisionTarget is SphereHitCheck targetSphere))
			{
				if (collisionTarget is CubeHitCheck targetCube)
				{
					return HandleCapsule2CubeCollision(capsule, posOrigin, targetCube, posTarget);
				}
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			return HandleCapsule2SphereCollision(capsule, posOrigin, targetSphere, posTarget);
		}
		return HandleCapsule2CapsuleCollision(capsule, posOrigin, targetCapsule, posTarget);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleCapsule2CapsuleCollision(CapsuleHitCheck capsule, HitCheckPos posOrigin, CapsuleHitCheck targetCapsule, HitCheckPos posTarget)
	{
		(Vector3 fromLocal, Vector3 toLocal) tuple = CollisionCoordinateUtils.TransformToOriginLocal(posOrigin.Start, posOrigin.AngleRad, posTarget.Start, posTarget.End, applyRotation: false);
		Vector3 item = tuple.fromLocal;
		Vector3 item2 = tuple.toLocal;
		Vector3 capsuleMovementLocal = posOrigin.End - posOrigin.Start;
		Vector3 capsuleAxis = capsule.Rotation.ToDirectionVector();
		targetCapsule.Rotation.ToDirectionVector();
		(bool, Vector3, Vector3) tuple2 = CalcCollision.CalcMovingCapsule2MovingSphere(capsuleMovementLocal, capsule.Rad, capsule.Length, capsuleAxis, item, item2, targetCapsule.Rad);
		if (!tuple2.Item1)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return (isHit: true, hitPos: tuple2.Item2, normal: tuple2.Item3);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleCapsule2SphereCollision(CapsuleHitCheck capsule, HitCheckPos posOrigin, SphereHitCheck targetSphere, HitCheckPos posTarget)
	{
		(Vector3 fromLocal, Vector3 toLocal) tuple = CollisionCoordinateUtils.TransformToOriginLocal(posOrigin.Start, posOrigin.AngleRad, posTarget.Start, posTarget.End, applyRotation: false);
		Vector3 item = tuple.fromLocal;
		Vector3 item2 = tuple.toLocal;
		(bool, Vector3, Vector3) tuple2 = CalcCollision.CalcMovingCapsule2MovingSphere(posOrigin.End - posOrigin.Start, capsuleAxis: capsule.Rotation.ToDirectionVector(), capsuleRadius: capsule.Rad, capsuleLength: capsule.Length, sphereFromLocal: item, sphereToLocal: item2, sphereRadius: targetSphere.Rad);
		if (!tuple2.Item1)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return (isHit: true, hitPos: tuple2.Item2, normal: tuple2.Item3);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleCapsule2CubeCollision(CapsuleHitCheck capsule, HitCheckPos posOrigin, CubeHitCheck targetCube, HitCheckPos posTarget)
	{
		(bool, Vector3, Vector3) tuple = HandleCube2CapsuleCollision(targetCube, posTarget, capsule, posOrigin);
		if (!tuple.Item1)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return (isHit: true, hitPos: tuple.Item2, normal: -tuple.Item3);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleFanCollision(FanHitCheck fan, HitCheckPos posOrigin, IHitCheck collisionTarget, HitCheckPos posTarget)
	{
		Vector3 end = posOrigin.End;
		float angleRad = posOrigin.AngleRad;
		if (!(collisionTarget is SphereHitCheck sphereHitCheck))
		{
			if (!(collisionTarget is CapsuleHitCheck capsuleHitCheck))
			{
				if (collisionTarget is CubeHitCheck)
				{
					return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
				}
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			float num = angleRad + fan.Rotation.Yaw.toRadian();
			Vector3 item = CollisionCoordinateUtils.TransformToOriginLocalWithCenter(end, num, fan.Center, 0f, posTarget.Start, posTarget.End, capsuleHitCheck.Center, posTarget.AngleRad).toLocal;
			Vector3 normalized = CollisionCoordinateUtils.TransformDirectionToOriginLocal(capsuleHitCheck.Rotation.ToDirectionVector(), num).normalized;
			(bool, Vector3, Vector3) tuple = CalcCollision.CalcStoppedOriginFan2StoppedCapsule(fan.Height, fan.InnerRad, fan.OuterRad, fan.Angle, item, normalized, capsuleHitCheck.Rad, capsuleHitCheck.Length);
			if (!tuple.Item1)
			{
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			Vector3 item2 = CollisionCoordinateUtils.TransformToWorldFromOriginLocalWithCenter(tuple.Item2, end, num, fan.Center);
			Vector3 item3 = CollisionCoordinateUtils.TransformDirectionToOriginLocal(tuple.Item3, 0f - num);
			return (isHit: true, hitPos: item2, normal: item3);
		}
		Vector3 item4 = CollisionCoordinateUtils.TransformToOriginLocalWithCenter(end, angleRad, fan.Center, fan.Rotation.Yaw.toRadian(), posTarget.Start, posTarget.End, sphereHitCheck.Center, posTarget.AngleRad).toLocal;
		(bool, Vector3, Vector3) tuple2 = CalcCollision.CalcStoppedOriginFan2StoppedSphere(fan.Height, fan.InnerRad, fan.OuterRad, fan.Angle, item4, sphereHitCheck.Rad);
		if (!tuple2.Item1)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 item5 = CollisionCoordinateUtils.TransformToWorldFromOriginLocalWithCenter(tuple2.Item2, end, angleRad, fan.Center);
		Vector3 item6 = CollisionCoordinateUtils.TransformDirectionToOriginLocal(tuple2.Item3, 0f - angleRad);
		return (isHit: true, hitPos: item5, normal: item6);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) HandleTorusCollision(TorusHitCheck torus, HitCheckPos posOrigin, IHitCheck collisionTarget, HitCheckPos posTarget)
	{
		Vector3 end = posOrigin.End;
		float angleRad = posOrigin.AngleRad;
		if (collisionTarget is CapsuleHitCheck capsuleHitCheck)
		{
			Vector3 item = CollisionCoordinateUtils.TransformToOriginLocalWithCenter(end, angleRad, torus.Center, 0f, posTarget.Start, posTarget.End, capsuleHitCheck.Center, posTarget.AngleRad).toLocal;
			Vector3 normalized = CollisionCoordinateUtils.TransformDirectionToOriginLocal(capsuleHitCheck.Rotation.ToDirectionVector().RotateY(posTarget.AngleRad), angleRad).normalized;
			(bool, Vector3, Vector3) tuple = CalcCollision.CalcOriginXZTorus2XZCapsule(torus.OuterRad, torus.InnerRad, torus.Height, item, normalized, capsuleHitCheck.Rad, capsuleHitCheck.Length);
			if (!tuple.Item1)
			{
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			Vector3 item2 = CollisionCoordinateUtils.TransformToWorldFromOriginLocalWithCenter(tuple.Item2, end, angleRad, torus.Center);
			Vector3 item3 = CollisionCoordinateUtils.TransformDirectionToOriginLocal(tuple.Item3, 0f - angleRad);
			if (item3.sqrMagnitude > 1E-12f)
			{
				item3.Normalize();
			}
			return (isHit: true, hitPos: item2, normal: item3);
		}
		return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
	}
}
