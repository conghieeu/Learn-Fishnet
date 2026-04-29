using System.Collections.Generic;
using UnityEngine;

public class CalcCollision
{
	public static bool RatioPointToSegByDistance2D(Vector2 p, Vector2 a, Vector2 b, float dist, out float t)
	{
		t = 0f;
		Vector2 vector = b - a;
		float num = Vector2.Dot(vector, vector);
		float num2 = dist * dist;
		if (num < 1E-08f)
		{
			if (Mathf.Abs((p - a).sqrMagnitude - num2) <= 1E-08f)
			{
				t = 0f;
				return true;
			}
			return false;
		}
		Vector2 vector2 = a - p;
		float b2 = 2f * Vector2.Dot(vector, vector2);
		float c = Vector2.Dot(vector2, vector2) - num2;
		(float, float, bool) tuple = Misc.RootFormula(num, b2, c);
		if (!tuple.Item3)
		{
			return false;
		}
		bool num3 = tuple.Item1 >= -1E-08f && tuple.Item1 <= 1f;
		bool flag = tuple.Item2 >= -1E-08f && tuple.Item2 <= 1f;
		if (num3)
		{
			t = Mathf.Clamp01(tuple.Item1);
			return true;
		}
		if (flag)
		{
			t = Mathf.Clamp01(tuple.Item2);
			return true;
		}
		return false;
	}

	private static (IntersectionType intersectionType, float ratio) GetSideToBoxExtent(float origin, float extent, float direction)
	{
		if (origin < 0f - extent)
		{
			if (Mathf.Abs(direction) < 1E-07f)
			{
				return (intersectionType: IntersectionType.NoIntersection, ratio: -1f);
			}
			float item = (0f - extent - origin) / direction;
			return (intersectionType: IntersectionType.NegativeFace, ratio: item);
		}
		if (origin > extent)
		{
			if (Mathf.Abs(direction) < 1E-07f)
			{
				return (intersectionType: IntersectionType.NoIntersection, ratio: -1f);
			}
			float item2 = (extent - origin) / direction;
			return (intersectionType: IntersectionType.PositiveFace, ratio: item2);
		}
		return (intersectionType: IntersectionType.InsideBox, ratio: 0f);
	}

	private static Vector3 CalculateInsideBoxNormal(Vector3 point, Vector3 halfExtent)
	{
		float num = halfExtent.x - Mathf.Abs(point.x);
		float num2 = halfExtent.y - Mathf.Abs(point.y);
		float num3 = halfExtent.z - Mathf.Abs(point.z);
		if (num <= num2 && num <= num3)
		{
			if (!(point.x > 0f))
			{
				return Vector3.left;
			}
			return Vector3.right;
		}
		if (num2 <= num && num2 <= num3)
		{
			if (!(point.y > 0f))
			{
				return Vector3.down;
			}
			return Vector3.up;
		}
		if (!(point.z > 0f))
		{
			return Vector3.back;
		}
		return Vector3.forward;
	}

	private static Vector3 CalculateBoxFaceNormal(Vector3 hitPoint, Vector3 halfExtent, int hitPlane, IntersectionType intersectionType)
	{
		Vector3 result = Vector3.zero;
		switch (hitPlane)
		{
		case 0:
			result = ((intersectionType != IntersectionType.PositiveFace) ? Vector3.left : Vector3.right);
			break;
		case 1:
			result = ((intersectionType != IntersectionType.PositiveFace) ? Vector3.down : Vector3.up);
			break;
		case 2:
			result = ((intersectionType != IntersectionType.PositiveFace) ? Vector3.back : Vector3.forward);
			break;
		}
		return result;
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedSphere2StoppedSphere(Vector3 center1, float radius1, Vector3 center2, float radius2)
	{
		Vector3 vector = center2 - center1;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = radius1 + radius2;
		float num2 = num * num;
		if (sqrMagnitude > num2)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		if (sqrMagnitude < 1E-08f)
		{
			Vector3 forward = Vector3.forward;
			Vector3 item = center1 + forward * radius1;
			return (isHit: true, hitPos: item, normal: forward);
		}
		Vector3 normalized = vector.normalized;
		Vector3 item2 = center1 + normalized * radius1;
		return (isHit: true, hitPos: item2, normal: normalized);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcMovingSphere2MovingSphere(Vector3 center1From, Vector3 center1To, float radius1, Vector3 center2From, Vector3 center2To, float radius2)
	{
		Vector3 vector = center1To - center1From;
		Vector3 vector2 = center2To - center2From;
		Vector3 lhs = vector2 - vector;
		float num = radius1 + radius2;
		float num2 = num * num;
		if (lhs.sqrMagnitude < 1E-08f)
		{
			Vector3 vector3 = center2From - center1From;
			float sqrMagnitude = vector3.sqrMagnitude;
			if (sqrMagnitude > num2)
			{
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			Vector3 item = ((sqrMagnitude < 1E-08f) ? Vector3.forward : vector3.normalized);
			return (isHit: true, hitPos: center1From, normal: item);
		}
		Vector3 rhs = center2From - center1From;
		float sqrMagnitude2 = lhs.sqrMagnitude;
		float b = 2f * Vector3.Dot(lhs, rhs);
		float c = rhs.sqrMagnitude - num2;
		(float, float, bool) tuple = Misc.RootFormula(sqrMagnitude2, b, c);
		if (!tuple.Item3)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		float num3 = Mathf.Max(tuple.Item1, 0f);
		float num4 = Mathf.Min(tuple.Item2, 1f);
		if (num3 > 1f || num4 < 0f)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		float num5 = num3;
		Vector3 vector4 = center1From + vector * num5;
		Vector3 vector5 = center2From + vector2 * num5 - vector4;
		Vector3 vector6;
		Vector3 item2;
		if (vector5.sqrMagnitude < 1E-08f)
		{
			vector6 = Vector3.forward;
			item2 = vector4;
		}
		else
		{
			vector6 = vector5.normalized;
			item2 = vector4 + vector6 * radius1;
		}
		return (isHit: true, hitPos: item2, normal: vector6);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedBox2StoppedSphere(Vector3 boxExtent, Vector3 localSphereCenter, float sphereRadius)
	{
		return CalcStoppedOriginBox2StoppedSphere(boxExtent, localSphereCenter, sphereRadius);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedOriginBox2StoppedSphere(Vector3 boxExtent, Vector3 localSphereCenter, float sphereRadius)
	{
		Vector3 vector = boxExtent * 0.5f;
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Clamp(localSphereCenter.x, 0f - vector.x, vector.x);
		zero.y = Mathf.Clamp(localSphereCenter.y, 0f - vector.y, vector.y);
		zero.z = Mathf.Clamp(localSphereCenter.z, 0f - vector.z, vector.z);
		Vector3 vector2 = localSphereCenter - zero;
		float sqrMagnitude = vector2.sqrMagnitude;
		float num = sphereRadius * sphereRadius;
		if (sqrMagnitude > num)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		if (sqrMagnitude < 1E-08f)
		{
			Vector3 vector3 = Vector3.zero;
			float num2 = float.MaxValue;
			float num3 = vector.x - Mathf.Abs(localSphereCenter.x);
			if (num3 < num2)
			{
				num2 = num3;
				vector3 = ((localSphereCenter.x > 0f) ? Vector3.right : Vector3.left);
			}
			float num4 = vector.y - Mathf.Abs(localSphereCenter.y);
			if (num4 < num2)
			{
				num2 = num4;
				vector3 = ((localSphereCenter.y > 0f) ? Vector3.up : Vector3.down);
			}
			float num5 = vector.z - Mathf.Abs(localSphereCenter.z);
			if (num5 < num2)
			{
				num2 = num5;
				vector3 = ((localSphereCenter.z > 0f) ? Vector3.forward : Vector3.back);
			}
			Vector3 item = localSphereCenter - vector3 * sphereRadius;
			return (isHit: true, hitPos: item, normal: -vector3);
		}
		Vector3 normalized = vector2.normalized;
		Vector3 item2 = localSphereCenter - normalized * sphereRadius;
		return (isHit: true, hitPos: item2, normal: normalized);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedBox2StoppedBox(Vector3 box1Extent, Vector3 box2Extent, Vector3 box2LocalCenter, Vector3[] box2Axes)
	{
		if (box2Axes.Length < 3)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 vector = box1Extent * 0.5f;
		Vector3 vector2 = box2Extent * 0.5f;
		Vector3[] array = new Vector3[3]
		{
			Vector3.right,
			Vector3.up,
			Vector3.forward
		};
		List<Vector3> list = new List<Vector3>(15);
		list.AddRange(array);
		list.AddRange(box2Axes);
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Vector3 vector3 = Vector3.Cross(array[i], box2Axes[j]);
				if (vector3.sqrMagnitude > 1E-08f)
				{
					list.Add(vector3.normalized);
				}
			}
		}
		float num = float.MaxValue;
		Vector3 vector4 = Vector3.zero;
		bool flag = false;
		foreach (Vector3 item2 in list)
		{
			float num2 = Mathf.Abs(item2.x) * vector.x + Mathf.Abs(item2.y) * vector.y + Mathf.Abs(item2.z) * vector.z;
			float num3 = 0f;
			for (int k = 0; k < 3; k++)
			{
				num3 += Mathf.Abs(Vector3.Dot(item2, box2Axes[k])) * vector2[k];
			}
			float num4 = Mathf.Abs(Vector3.Dot(item2, box2LocalCenter));
			float num5 = num2 + num3;
			if (num4 > num5 + 1E-08f)
			{
				flag = true;
				break;
			}
			float num6 = num5 - num4;
			if (num6 < num)
			{
				num = num6;
				vector4 = item2;
				if (Vector3.Dot(vector4, box2LocalCenter) < 0f)
				{
					vector4 = -vector4;
				}
			}
		}
		if (flag)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 item = vector4 * num * 0.5f;
		return (isHit: true, hitPos: item, normal: vector4.normalized);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedBox2StoppedCapsule(Vector3 boxExtent, Vector3 capsuleLocalCenter, Vector3 capsuleLocalDirection, float capsuleRadius, float capsuleLength)
	{
		_ = boxExtent * 0.5f;
		float num = capsuleLength * 0.5f;
		Vector3 b = capsuleLocalCenter + capsuleLocalDirection * num;
		Vector3 a = capsuleLocalCenter - capsuleLocalDirection * num;
		int num2 = Mathf.Max(2, (int)Mathf.Ceil(capsuleLength / capsuleRadius));
		float num3 = 1f / (float)(num2 - 1);
		bool flag = false;
		float num4 = float.MaxValue;
		Vector3 item = Vector3.zero;
		Vector3 item2 = Vector3.zero;
		for (int i = 0; i < num2; i++)
		{
			float t = (float)i * num3;
			Vector3 localSphereCenter = Vector3.Lerp(a, b, t);
			(bool, Vector3, Vector3) tuple = CalcStoppedOriginBox2StoppedSphere(boxExtent, localSphereCenter, capsuleRadius);
			if (tuple.Item1)
			{
				flag = true;
				float sqrMagnitude = tuple.Item2.sqrMagnitude;
				if (sqrMagnitude < num4)
				{
					num4 = sqrMagnitude;
					item = tuple.Item2;
					item2 = tuple.Item3;
				}
			}
		}
		if (!flag)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return (isHit: true, hitPos: item, normal: item2);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedBox2MovingCapsule(Vector3 boxExtent, Vector3 capsuleFromLocalCenter, Vector3 capsuleToLocalCenter, Vector3 capsuleLocalDirection, float capsuleRadius, float capsuleLength)
	{
		float num = capsuleLength * 0.5f;
		int num2 = Mathf.Max(2, (int)Mathf.Ceil(capsuleLength / capsuleRadius));
		float magnitude = (capsuleToLocalCenter - capsuleFromLocalCenter).magnitude;
		int num3 = Mathf.Max(2, (int)Mathf.Ceil(magnitude / capsuleRadius));
		bool flag = false;
		float num4 = float.MaxValue;
		Vector3 item = Vector3.zero;
		Vector3 item2 = Vector3.zero;
		for (int i = 0; i < num3; i++)
		{
			float num5 = (float)i / (float)(num3 - 1);
			Vector3 vector = Vector3.Lerp(capsuleFromLocalCenter, capsuleToLocalCenter, num5);
			for (int j = 0; j < num2; j++)
			{
				float t = (float)j / (float)(num2 - 1);
				Vector3 b = vector + capsuleLocalDirection * num;
				Vector3 localSphereCenter = Vector3.Lerp(vector - capsuleLocalDirection * num, b, t);
				(bool, Vector3, Vector3) tuple = CalcStoppedOriginBox2StoppedSphere(boxExtent, localSphereCenter, capsuleRadius);
				if (tuple.Item1)
				{
					flag = true;
					if (num5 < num4)
					{
						num4 = num5;
						item = tuple.Item2;
						item2 = tuple.Item3;
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (!flag)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return (isHit: true, hitPos: item, normal: item2);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcMovingBox2MovingSphereXZ(Vector3 boxExtent, Vector3 boxMovementLocal, Vector3 sphereFromLocalCenter, Vector3 sphereToLocalCenter, float sphereRadius)
	{
		Vector3 vector = sphereToLocalCenter - sphereFromLocalCenter;
		Vector3 vector2 = vector - boxMovementLocal;
		Vector3 sphereToLocal = sphereFromLocalCenter + vector2;
		if (vector2.sqrMagnitude < 1E-08f)
		{
			(bool, Vector3, Vector3) tuple = CalcStoppedOriginBox2StoppedSphere(boxExtent, sphereFromLocalCenter, sphereRadius);
			if (!tuple.Item1)
			{
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			return (isHit: true, hitPos: tuple.Item2, normal: tuple.Item3);
		}
		if (!CalcRatioOriginBox2MovingSphereXZ(boxExtent, sphereFromLocalCenter, sphereToLocal, sphereRadius, out var minRatio))
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 vector3 = boxMovementLocal * minRatio;
		Vector3 vector4 = sphereFromLocalCenter + vector * minRatio;
		(bool, Vector3, Vector3) tuple2 = CalcStoppedOriginBox2StoppedSphere(boxExtent, vector4 - vector3, sphereRadius);
		Vector3 item = vector3 + tuple2.Item2;
		return (isHit: true, hitPos: item, normal: tuple2.Item3);
	}

	public static bool CalcRatioOriginBox2MovingSphereXZ(Vector3 boxExtent, Vector3 sphereFromLocal, Vector3 sphereToLocal, float sphereRadius, out float minRatio)
	{
		minRatio = float.MaxValue;
		Vector3 vector = boxExtent * 0.5f;
		float num = Mathf.Abs(sphereFromLocal.y) - vector.y;
		float num2 = sphereRadius * sphereRadius;
		if (num > 0f)
		{
			num2 -= num * num;
		}
		if (num2 < 0f)
		{
			return false;
		}
		float num3 = Mathf.Sqrt(num2);
		Vector2 vector2 = new Vector2(sphereFromLocal.x, sphereFromLocal.z);
		Vector2 vector3 = new Vector2(sphereToLocal.x, sphereToLocal.z);
		if (vector2.x < 0f)
		{
			vector2.x = 0f - vector2.x;
			vector3.x = 0f - vector3.x;
		}
		if (vector2.y < 0f)
		{
			vector2.y = 0f - vector2.y;
			vector3.y = 0f - vector3.y;
		}
		Vector2 vector4 = new Vector2(vector.x, vector.z);
		Vector2 vector5 = vector2 - vector4;
		vector5.x = Mathf.Max(0f, vector5.x);
		vector5.y = Mathf.Max(0f, vector5.y);
		if (vector5.sqrMagnitude <= num2)
		{
			minRatio = 0f;
			return true;
		}
		Vector2 vector6 = vector3 - vector2;
		if (RatioPointToSegByDistance2D(vector4, vector2, vector3, num3, out var t))
		{
			minRatio = Mathf.Min(minRatio, t);
		}
		if (vector2.y > vector4.y && vector6.y < 0f)
		{
			float num4 = 0f - vector6.y;
			if (num4 > 1E-08f)
			{
				t = (vector2.y - vector4.y - num3) / num4;
				if (t > -1E-08f && t <= 1f && Mathf.Abs(vector2.x + vector6.x * Mathf.Clamp01(t)) <= vector4.x + 1E-08f)
				{
					minRatio = Mathf.Min(minRatio, Mathf.Clamp01(t));
				}
			}
		}
		if (vector2.x > vector4.x && vector6.x < 0f)
		{
			float num5 = 0f - vector6.x;
			if (num5 > 1E-08f)
			{
				t = (vector2.x - vector4.x - num3) / num5;
				if (t > -1E-08f && t <= 1f && Mathf.Abs(vector2.y + vector6.y * Mathf.Clamp01(t)) <= vector4.y + 1E-08f)
				{
					minRatio = Mathf.Min(minRatio, Mathf.Clamp01(t));
				}
			}
		}
		return minRatio <= 1f;
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcOriginBox2Seg(Vector3 boxExtent, Vector3 origin, Vector3 dir)
	{
		if (dir.sqrMagnitude < 1E-08f)
		{
			Vector3 halfExtent = boxExtent * 0.5f;
			if (!(Mathf.Abs(origin.x) <= halfExtent.x) || !(Mathf.Abs(origin.y) <= halfExtent.y) || !(Mathf.Abs(origin.z) <= halfExtent.z))
			{
				return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
			}
			Vector3 item = CalculateInsideBoxNormal(origin, halfExtent);
			return (isHit: true, hitPos: origin, normal: item);
		}
		Vector3 halfExtent2 = boxExtent * 0.5f;
		IntersectionType[] array = new IntersectionType[3];
		float[] array2 = new float[3];
		ref IntersectionType reference = ref array[0];
		ref float reference2 = ref array2[0];
		(IntersectionType, float) sideToBoxExtent = GetSideToBoxExtent(origin.x, halfExtent2.x, dir.x);
		reference = sideToBoxExtent.Item1;
		reference2 = sideToBoxExtent.Item2;
		reference = ref array[1];
		ref float reference3 = ref array2[1];
		sideToBoxExtent = GetSideToBoxExtent(origin.y, halfExtent2.y, dir.y);
		reference = sideToBoxExtent.Item1;
		reference3 = sideToBoxExtent.Item2;
		reference = ref array[2];
		ref float reference4 = ref array2[2];
		(reference, reference4) = GetSideToBoxExtent(origin.z, halfExtent2.z, dir.z);
		if (array[0] == IntersectionType.InsideBox && array[1] == IntersectionType.InsideBox && array[2] == IntersectionType.InsideBox)
		{
			Vector3 item2 = CalculateInsideBoxNormal(origin, halfExtent2);
			return (isHit: true, hitPos: origin, normal: item2);
		}
		int num = 0;
		for (int i = 1; i < 3; i++)
		{
			if (array2[num] < array2[i])
			{
				num = i;
			}
		}
		float num2 = array2[num];
		if (num2 < 0f || num2 > 1f || float.IsNaN(num2))
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 vector = origin + dir * num2;
		if (num != 0 && Mathf.Abs(vector.x) > halfExtent2.x + 1E-08f)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		if (num != 1 && Mathf.Abs(vector.y) > halfExtent2.y + 1E-08f)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		if (num != 2 && Mathf.Abs(vector.z) > halfExtent2.z + 1E-08f)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 item3 = CalculateBoxFaceNormal(vector, halfExtent2, num, array[num]);
		return (isHit: true, hitPos: vector, normal: item3);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedCapsule2StoppedSphere(float capsuleRadius, float capsuleHalfLength, Vector3 localSphereCenter, float sphereRadius)
	{
		Vector3 segTo = Vector3.up * capsuleHalfLength;
		Vector3 segFrom = Vector3.down * capsuleHalfLength;
		Vector3 vector = Misc.NearestPoint2Segment(localSphereCenter, segFrom, segTo);
		Vector3 vector2 = localSphereCenter - vector;
		float sqrMagnitude = vector2.sqrMagnitude;
		float num = capsuleRadius + sphereRadius;
		float num2 = num * num;
		if (sqrMagnitude > num2)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 vector3;
		Vector3 item;
		if (sqrMagnitude < 1E-08f)
		{
			vector3 = Vector3.right;
			item = vector + vector3 * capsuleRadius;
		}
		else
		{
			float num3 = Mathf.Sqrt(sqrMagnitude);
			vector3 = vector2 / num3;
			item = vector + vector3 * capsuleRadius;
		}
		return (isHit: true, hitPos: item, normal: vector3);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcMovingCapsule2OriginBox(Vector3 boxExtent, Vector3 capsuleFromCenter, Vector3 capsuleToCenter, Vector3 capsuleLocalDirection, float capsuleRadius, float capsuleLength)
	{
		Vector3 vector = capsuleToCenter - capsuleFromCenter;
		float magnitude = vector.magnitude;
		if (magnitude < 0.2f)
		{
			return CalcStoppedBox2StoppedCapsule(boxExtent, capsuleFromCenter, capsuleLocalDirection, capsuleRadius, capsuleLength);
		}
		int num = (int)Mathf.Ceil(magnitude / capsuleRadius);
		if (num < 3 || magnitude < 0.5f)
		{
			return CalcStoppedBox2StoppedCapsule(boxExtent, capsuleFromCenter, capsuleLocalDirection, capsuleRadius, capsuleLength);
		}
		bool flag = false;
		Vector3 item = Vector3.zero;
		Vector3 item2 = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			if (flag)
			{
				break;
			}
			float num2 = ((i != num - 1) ? ((float)i / (float)(num - 1)) : 1f);
			Vector3 capsuleLocalCenter = capsuleFromCenter + vector * num2;
			(bool, Vector3, Vector3) tuple = CalcStoppedBox2StoppedCapsule(boxExtent, capsuleLocalCenter, capsuleLocalDirection, capsuleRadius, capsuleLength);
			if (tuple.Item1)
			{
				flag = true;
				item = tuple.Item2;
				item2 = tuple.Item3;
				break;
			}
		}
		if (!flag)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return (isHit: true, hitPos: item, normal: item2);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcMovingCapsule2MovingSphere(Vector3 capsuleMovementLocal, float capsuleRadius, float capsuleLength, Vector3 capsuleAxis, Vector3 sphereFromLocal, Vector3 sphereToLocal, float sphereRadius)
	{
		float num = capsuleRadius + sphereRadius;
		float num2 = capsuleLength * 0.5f;
		Vector3 vector = capsuleAxis * num2;
		Vector3 vector2 = -capsuleAxis * num2;
		Vector3 vector3 = Misc.NearestPoint2Segment(sphereFromLocal, vector2, vector);
		if (Vector3.Distance(vector3, sphereFromLocal) <= num)
		{
			Vector3 item = (sphereFromLocal - vector3).normalized;
			if (item.sqrMagnitude < 1E-08f)
			{
				item = Vector3.up;
			}
			return (isHit: true, hitPos: sphereFromLocal, normal: item);
		}
		Vector3 vector4 = sphereToLocal - sphereFromLocal - capsuleMovementLocal;
		if (vector4.sqrMagnitude < 1E-08f)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 vector5 = sphereFromLocal + vector4;
		var (a, vector6) = Misc.NearestPointsSeg2Seg(sphereFromLocal, vector5, vector2, vector);
		if (Vector3.Distance(a, vector6) > num)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		if (!Misc.RatioPointToSegByDistance(vector6, sphereFromLocal, vector5, num, out var ratio))
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 vector7 = sphereFromLocal + vector4 * ratio;
		Vector3 vector8 = capsuleMovementLocal * ratio;
		Vector3 segTo = vector8 + capsuleAxis * num2;
		Vector3 segFrom = vector8 - capsuleAxis * num2;
		Vector3 vector9 = Misc.NearestPoint2Segment(vector7, segFrom, segTo);
		Vector3 item2 = (vector7 - vector9).normalized;
		if (item2.sqrMagnitude < 1E-08f)
		{
			item2 = Vector3.up;
		}
		return (isHit: true, hitPos: vector7, normal: item2);
	}

	public static List<(bool isHit, Vector3 hitPos, Vector3 normal, CapsuleHitCheck capsule)> CheckSegmentHitWithCapsules(Vector3 actorCenter, Vector3 direction, float radius, IEnumerable<CapsuleHitCheck> capsuleTargets)
	{
		direction = direction.normalized;
		Vector3 segmentEnd = actorCenter + direction * radius;
		List<(bool, Vector3, Vector3, CapsuleHitCheck, float)> list = new List<(bool, Vector3, Vector3, CapsuleHitCheck, float)>();
		foreach (CapsuleHitCheck capsuleTarget in capsuleTargets)
		{
			var (flag, vector, item) = IsSegmentIntersectCapsule(actorCenter, segmentEnd, capsuleTarget);
			if (flag)
			{
				float item2 = Vector3.Distance(actorCenter, vector);
				list.Add((flag, vector, item, capsuleTarget, item2));
			}
		}
		list.Sort(((bool isHit, Vector3 hitPos, Vector3 normal, CapsuleHitCheck capsule, float distance) a, (bool isHit, Vector3 hitPos, Vector3 normal, CapsuleHitCheck capsule, float distance) b) => a.distance.CompareTo(b.distance));
		List<(bool, Vector3, Vector3, CapsuleHitCheck)> list2 = new List<(bool, Vector3, Vector3, CapsuleHitCheck)>();
		foreach (var (item3, item4, item5, item6, _) in list)
		{
			list2.Add((item3, item4, item5, item6));
		}
		return list2;
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) IsSegmentIntersectCapsule(Vector3 segmentStart, Vector3 segmentEnd, CapsuleHitCheck capsule)
	{
		float num = capsule.Length * 0.5f;
		Vector3 vector = capsule.Center + Vector3.up * num;
		Vector3 vector2 = capsule.Center + Vector3.down * num;
		float num2 = float.MaxValue;
		Vector3 item = Vector3.zero;
		Vector3 item2 = Vector3.zero;
		bool item3 = false;
		(Vector3, Vector3) tuple = Misc.NearestPointsSeg2Seg(segmentStart, segmentEnd, vector2, vector);
		Vector3 item4 = tuple.Item1;
		Vector3 item5 = tuple.Item2;
		float num3 = Vector3.Distance(item4, item5);
		if (num3 <= capsule.Rad)
		{
			item3 = true;
			if (num3 < num2)
			{
				num2 = num3;
				item = item5;
				Vector3 normalized = (item4 - item5).normalized;
				item += normalized * capsule.Rad;
				item2 = normalized;
			}
		}
		Vector3 vector3 = Misc.NearestPoint2Segment(vector, segmentStart, segmentEnd);
		float num4 = Vector3.Distance(vector, vector3);
		if (num4 <= capsule.Rad)
		{
			item3 = true;
			if (num4 < num2)
			{
				num2 = num4;
				Vector3 normalized2 = (vector3 - vector).normalized;
				item = vector + normalized2 * capsule.Rad;
				item2 = normalized2;
			}
		}
		Vector3 vector4 = Misc.NearestPoint2Segment(vector2, segmentStart, segmentEnd);
		float num5 = Vector3.Distance(vector2, vector4);
		if (num5 <= capsule.Rad)
		{
			item3 = true;
			if (num5 < num2)
			{
				num2 = num5;
				Vector3 normalized3 = (vector4 - vector2).normalized;
				item = vector2 + normalized3 * capsule.Rad;
				item2 = normalized3;
			}
		}
		return (isHit: item3, hitPos: item, normal: item2);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedOriginFan2StoppedCapsule(float fanHeight, float innerRad, float outerRad, float fanAngleDegrees, Vector3 capsuleCenter, Vector3 capsuleAxis, float capsuleRadius, float capsuleLength)
	{
		float num = fanHeight * 0.5f;
		float num2 = capsuleLength * 0.5f;
		float num3 = capsuleCenter.y - num2;
		if (capsuleCenter.y + num2 < 0f - num || num3 > num)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return CheckFan2DCollision(new Vector2(capsuleCenter.x, capsuleCenter.z), capsuleRadius, innerRad, outerRad, fanAngleDegrees, capsuleCenter.y, 0f - num, num);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcStoppedOriginFan2StoppedSphere(float fanHeight, float innerRad, float outerRad, float fanAngleDegrees, Vector3 sphereCenter, float sphereRadius)
	{
		float num = fanHeight * 0.5f;
		if (sphereCenter.y + sphereRadius < 0f - num || sphereCenter.y - sphereRadius > num)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return CheckFan2DCollision(new Vector2(sphereCenter.x, sphereCenter.z), sphereRadius, innerRad, outerRad, fanAngleDegrees, sphereCenter.y, 0f - num, num);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) CheckFan2DCollision(Vector2 targetCenter2D, float targetRadius, float fanInnerRad, float fanOuterRad, float fanAngleDegrees, float targetCenterY, float fanBottomY, float fanTopY)
	{
		float magnitude = targetCenter2D.magnitude;
		float num = fanAngleDegrees * 0.5f;
		bool num2 = magnitude < fanInnerRad - targetRadius;
		bool flag = magnitude > fanOuterRad + targetRadius;
		if (num2 || flag)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		bool flag2 = false;
		if (magnitude < 1E-08f)
		{
			flag2 = true;
		}
		else
		{
			Vector2 vector = targetCenter2D / magnitude;
			float num3;
			for (num3 = Mathf.Atan2(vector.x, vector.y) * 57.29578f; num3 > 180f; num3 -= 360f)
			{
			}
			for (; num3 < -180f; num3 += 360f)
			{
			}
			float num4 = Mathf.Abs(num3);
			float num5 = ((magnitude > targetRadius) ? (Mathf.Atan2(targetRadius, magnitude) * 57.29578f) : 90f);
			float num6 = num4 - num5;
			flag2 = num4 + num5 >= 0f && num6 <= num;
		}
		if (!flag2)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return CreateHitResult(targetCenter2D, magnitude, fanInnerRad, fanOuterRad, targetCenterY, fanBottomY, fanTopY);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) CreateHitResult(Vector2 targetCenter2D, float distanceToOrigin, float fanInnerRad, float fanOuterRad, float targetCenterY, float fanBottomY, float fanTopY)
	{
		float num = Mathf.Clamp(distanceToOrigin, fanInnerRad, fanOuterRad);
		Vector2 vector = ((!(distanceToOrigin > 1E-08f)) ? (Vector2.up * num) : (targetCenter2D / distanceToOrigin * num));
		float y = Mathf.Clamp(targetCenterY, fanBottomY, fanTopY);
		Vector3 vector2 = new Vector3(vector.x, y, vector.y);
		Vector3 vector3 = new Vector3(targetCenter2D.x, targetCenterY, targetCenter2D.y) - vector2;
		if (vector3.sqrMagnitude < 1E-08f)
		{
			vector3 = new Vector3(targetCenter2D.x, 0f, targetCenter2D.y);
			if (vector3.sqrMagnitude < 1E-08f)
			{
				vector3 = Vector3.forward;
			}
		}
		vector3 = vector3.normalized;
		return (isHit: true, hitPos: vector2, normal: vector3);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) CalcOriginXZTorus2XZCapsule(float torusOuterRad, float torusInnerRad, float torusHeight, Vector3 capsuleCenterLocal, Vector3 capsuleAxis, float capsuleRadius, float capsuleLength)
	{
		float num = torusHeight * 0.5f;
		float num2 = capsuleLength * 0.5f;
		Vector3 a = capsuleCenterLocal - capsuleAxis * num2;
		Vector3 b = capsuleCenterLocal + capsuleAxis * num2;
		float num3 = Mathf.Min(a.y, b.y) - capsuleRadius;
		if (Mathf.Max(a.y, b.y) + capsuleRadius < 0f - num || num3 > num)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		int num4 = Mathf.Max(3, (int)Mathf.Ceil(capsuleLength / capsuleRadius));
		bool flag = false;
		float num5 = float.MaxValue;
		Vector3 item = Vector3.zero;
		Vector3 item2 = Vector3.zero;
		for (int i = 0; i < num4; i++)
		{
			float t = (float)i / (float)(num4 - 1);
			Vector3 sphereCenter = Vector3.Lerp(a, b, t);
			(bool, Vector3, Vector3) tuple = CalcOriginXZTorus2Sphere(torusOuterRad, torusInnerRad, torusHeight, sphereCenter, capsuleRadius);
			if (tuple.Item1)
			{
				flag = true;
				float sqrMagnitude = tuple.Item2.sqrMagnitude;
				if (sqrMagnitude < num5)
				{
					num5 = sqrMagnitude;
					item = tuple.Item2;
					item2 = tuple.Item3;
				}
			}
		}
		if (!flag)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		return (isHit: true, hitPos: item, normal: item2);
	}

	private static (bool isHit, Vector3 hitPos, Vector3 normal) CalcOriginXZTorus2Sphere(float torusOuterRad, float torusInnerRad, float torusHeight, Vector3 sphereCenter, float sphereRadius)
	{
		float num = torusHeight * 0.5f;
		float num2 = Mathf.Clamp(sphereCenter.y, 0f - num, num);
		if (sphereCenter.y != num2)
		{
			Mathf.Abs(sphereCenter.y - num2);
		}
		Vector3 vector = new Vector3(sphereCenter.x, 0f, sphereCenter.z);
		float magnitude = vector.magnitude;
		float num3 = (torusOuterRad + torusInnerRad) * 0.5f;
		float num4 = (torusOuterRad - torusInnerRad) * 0.5f;
		Vector3 zero = Vector3.zero;
		zero = ((!(magnitude > 1E-08f)) ? (Vector3.forward * num3) : (vector.normalized * num3));
		zero.y = num2;
		float num5 = Vector3.Distance(sphereCenter, zero);
		float num6 = num4 + sphereRadius;
		if (num5 > num6)
		{
			return (isHit: false, hitPos: Vector3.zero, normal: Vector3.zero);
		}
		Vector3 vector2 = (sphereCenter - zero).normalized;
		if (vector2.sqrMagnitude < 1E-08f)
		{
			vector2 = Vector3.up;
		}
		Vector3 item = zero + vector2 * num4;
		Vector3 item2 = vector2;
		return (isHit: true, hitPos: item, normal: item2);
	}
}
