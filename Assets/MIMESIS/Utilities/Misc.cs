using System;
using System.Collections.Generic;
using ReluProtocol;
using UnityEngine;

public static class Misc
{
	public static (double scalarSin, double scalarCos) SinCos(double angleRad)
	{
		return (scalarSin: Math.Sin(angleRad), scalarCos: Math.Cos(angleRad));
	}

	public static (float scalarSin, float scalarCos) SinCos(float angleRad)
	{
		return (scalarSin: Mathf.Sin(angleRad), scalarCos: Mathf.Cos(angleRad));
	}

	public static bool IsNearlyEqual(double A, double B, double tolerance = 9.99999993922529E-09)
	{
		return Math.Abs(A - B) <= tolerance;
	}

	public static bool IsSamePos(Vector3 posA, Vector3 posB)
	{
		if (IsNearlyEqual(posA.x, posB.x) && IsNearlyEqual(posA.y, posB.y))
		{
			return IsNearlyEqual(posA.z, posB.z);
		}
		return false;
	}

	public static double ClampAxis(double angle)
	{
		double num = Fmod(angle, 360.0);
		if (!(num < 0.0))
		{
			return num;
		}
		return num + 360.0;
	}

	public static float ClampAxis(float angle)
	{
		float num = Fmod(angle, 360f);
		if (!(num < 0f))
		{
			return num;
		}
		return num + 360f;
	}

	public static float Repeat(float value, float length)
	{
		return Mathf.Repeat(value, length);
	}

	public static float Fmod(float x, float y)
	{
		if (Mathf.Approximately(y, 0f))
		{
			return float.NaN;
		}
		return x % y;
	}

	public static double Fmod(double x, double y)
	{
		if (Math.Abs(y) < double.Epsilon)
		{
			return double.NaN;
		}
		return x % y;
	}

	public static Vector2 toVector2(Vector3 input)
	{
		return new Vector2(input.x, input.z);
	}

	public static bool IsVerticalFall(Vector3 from, Vector3 to, float horizontalTolerance = 0.01f)
	{
		bool num = Mathf.Abs(from.x - to.x) <= horizontalTolerance && Mathf.Abs(from.z - to.z) <= horizontalTolerance;
		bool flag = from.y > to.y;
		return num && flag;
	}

	public static bool IsFallingDistance(Vector3 from, Vector3 to, float minFallDistance)
	{
		return from.y - to.y > minFallDistance;
	}

	public static bool IsInFall(Vector3 from, Vector3 to, float minFallDistance = 0f, float horizontalTolerance = 0.01f)
	{
		if (IsVerticalFall(from, to, horizontalTolerance))
		{
			return IsFallingDistance(from, to, minFallDistance);
		}
		return false;
	}

	public static List<Vector3> GetGravityFallPath(Vector3 startPos, Vector3 endPos, bool exceptStartEnd = true)
	{
		List<Vector3> list = new List<Vector3>();
		float num = startPos.y - endPos.y;
		if (num <= 0f)
		{
			return list;
		}
		float num2 = Mathf.Sqrt(2f * num / 9.81f);
		if (num2 <= 30f)
		{
			if (!exceptStartEnd)
			{
				list.Add(startPos);
				list.Add(endPos);
			}
			return list;
		}
		for (float num3 = 0f; num3 <= num2; num3 += 30f)
		{
			float num4 = 4.905f * num3 * num3;
			if (num4 > num)
			{
				break;
			}
			Vector3 item = new Vector3(startPos.x, startPos.y - num4, startPos.z);
			if (num3 > 0f || !exceptStartEnd)
			{
				list.Add(item);
			}
		}
		if (list.Count > 0 && Vector3.Distance(list[list.Count - 1], endPos) > 0.1f)
		{
			if (!exceptStartEnd)
			{
				list.Add(endPos);
			}
		}
		else if (!exceptStartEnd && list.Count > 0)
		{
			list[list.Count - 1] = endPos;
		}
		return list;
	}

	public static List<Vector3> GetLinearMovementPath(Vector3 startPos, Vector3 endPos, float speed, bool exceptStartEnd = true)
	{
		List<Vector3> list = new List<Vector3>();
		if (speed <= 0f)
		{
			return list;
		}
		float num = Vector3.Distance(startPos, endPos);
		float num2 = num / speed;
		long num3 = 30L;
		float num4 = speed * (float)num3;
		if (num2 <= (float)num3 || num4 >= num)
		{
			if (!exceptStartEnd)
			{
				list.Add(startPos);
				list.Add(endPos);
			}
			return list;
		}
		Vector3 normalized = (endPos - startPos).normalized;
		for (float num5 = 0f; num5 <= num2; num5 += (float)num3)
		{
			float num6 = speed * num5;
			if (num6 > num)
			{
				break;
			}
			Vector3 item = startPos + normalized * num6;
			if (num5 > 0f || !exceptStartEnd)
			{
				list.Add(item);
			}
		}
		if (list.Count > 0 && Vector3.Distance(list[list.Count - 1], endPos) > 0.1f)
		{
			list.Add(endPos);
		}
		else if (list.Count > 0)
		{
			list[list.Count - 1] = endPos;
		}
		else if (!exceptStartEnd)
		{
			list.Add(endPos);
		}
		return list;
	}

	public static float Distance(Vector3 posA, Vector3 posB, bool ignoreHeight = false)
	{
		if (!ignoreHeight)
		{
			return Vector3.Distance(posA, posB);
		}
		return Vector2.Distance(toVector2(posA), toVector2(posB));
	}

	public static float GetDirectionAngle(float originAngle, float angleDiff)
	{
		return ClampAxis(originAngle + angleDiff);
	}

	public static double GetDirectionAngle(double originAngle, double angleDiff)
	{
		return ClampAxis(originAngle + angleDiff);
	}

	public static float GetDirectionAngle(Vector3 start, Vector3 end)
	{
		Vector3 vector = end - start;
		return Mathf.Atan2(vector.x, vector.z) * 57.29578f;
	}

	public static float DeltaAngle(float current, float target)
	{
		return Mathf.DeltaAngle(current, target);
	}

	public static float AngleBetween(Vector3 from, Vector3 to, bool signed = false)
	{
		if (!signed)
		{
			return Vector3.Angle(from, to);
		}
		return Vector3.SignedAngle(from, to, Vector3.up);
	}

	public static float FacingAngle(Vector3 from, Vector3 to)
	{
		return 180f - AngleBetween(from, to);
	}

	public static Vector3 GetRandomPositionInRange(Vector3 centerPos, double minRange, double maxRange)
	{
		double num = minRange + SimpleRandUtil.Next(0.0, 1.0) * (maxRange - minRange);
		double num2 = SimpleRandUtil.Next(0.0, 360.0).toRadian();
		double num3 = (double)centerPos.x + num * Math.Sin(num2);
		double num4 = (double)centerPos.z + num * Math.Cos(num2);
		double num5 = centerPos.y;
		return new Vector3((float)num3, (float)num5, (float)num4);
	}

	public static double AngleLimitHalf(this Vector3 posA, Vector3 posB)
	{
		if (!(posA == posB) && !(posA == Vector3.zero) && !(posB == Vector3.zero))
		{
			return Math.Acos(Math.Min(1f, Vector3.Dot(Vector3.Normalize(posA), Vector3.Normalize(posB))));
		}
		return 0.0;
	}

	public static Vector3 RotateVector(this Quaternion quat, Vector3 v)
	{
		return quat * v;
	}

	public static Vector3 AxisX(this Quaternion q)
	{
		return q * Vector3.right;
	}

	public static Vector3 AxisY(this Quaternion q)
	{
		return q * Vector3.up;
	}

	public static Vector3 AxisZ(this Quaternion q)
	{
		return q * Vector3.forward;
	}

	public static double toRadian(this double angle)
	{
		return angle * Math.PI / 180.0;
	}

	public static float toRadian(this float angle)
	{
		return angle * (MathF.PI / 180f);
	}

	public static double toDegree(this double rad)
	{
		return rad * 180.0 / Math.PI;
	}

	public static float toDegree(this float rad)
	{
		return rad * 57.29578f;
	}

	public static Vector3 GetPosWithVectorDistance(Vector3 start, Vector3 direction, float distance)
	{
		if (direction.magnitude == 0f)
		{
			return start;
		}
		Vector3 vector = Vector3.Normalize(direction);
		return start + vector * distance;
	}

	public static Vector3 GetPosWithAngleDistance(Vector3 start, double angleDeg, float distance)
	{
		double num = angleDeg.toRadian();
		Vector3 direction = new Vector3((float)Math.Sin(num), 0f, (float)Math.Cos(num));
		return GetPosWithVectorDistance(start, direction, distance);
	}

	public static double GetRandomAngle(double angle, double delta)
	{
		ClampAxis(angle);
		return SimpleRandUtil.Next(angle - delta, angle + delta);
	}

	public static double FastAsin(double value)
	{
		bool num = value >= 0.0;
		double num2 = Math.Abs(value);
		double num3 = 1.0 - num2;
		if (num3 < 0.0)
		{
			num3 = 0.0;
		}
		Math.Sqrt(num3);
		double num4 = ((((((-0.0012624911 * num2 + 0.0066700901) * num2 - 0.0170881256) * num2 + 0.030891881) * num2 - 0.0501743046) * num2 + 0.0889789874) * num2 - 0.2145988016) * num2 + 1.570796305;
		if (!num)
		{
			return num4 - 1.570796305;
		}
		return 1.570796305 - num4;
	}

	public static bool IsPointInAABB(Vector3 target, Vector3 start, Vector3 end)
	{
		if (target.x >= start.x && target.x <= end.x && target.y >= start.y && target.y <= end.y)
		{
			if (target.z >= start.z)
			{
				return target.z <= end.z;
			}
			return false;
		}
		return false;
	}

	public static double CCW(double x1, double y1, double x2, double y2, double x3, double y3)
	{
		return x1 * y2 + x2 * y3 + x3 * y1 - (y1 * x2 + y2 * x3 + y3 * x1);
	}

	public static Vector3 GetRotatedAABB(Vector3 extent, double angleRad)
	{
		double num = Math.Abs(Math.Cos(angleRad));
		double num2 = Math.Abs(Math.Sin(angleRad));
		return new Vector3((float)((double)extent.x * num + (double)extent.z * num2), extent.y, (float)((double)extent.x * num2 + (double)extent.z * num));
	}

	public static Vector3 RotateX(this Vector3 pos, double rad)
	{
		double num = Math.Cos(rad);
		double num2 = Math.Sin(rad);
		double num3 = pos.x;
		double num4 = (double)pos.y * num - (double)pos.z * num2;
		double num5 = (double)pos.y * num2 + (double)pos.z * num;
		return new Vector3((float)num3, (float)num4, (float)num5);
	}

	public static Vector3 RotateY(this Vector3 pos, double rad)
	{
		double num = Math.Cos(rad);
		double num2 = Math.Sin(rad);
		double num3 = (double)pos.x * num + (double)pos.z * num2;
		double num4 = pos.y;
		double num5 = (double)(0f - pos.x) * num2 + (double)pos.z * num;
		return new Vector3((float)num3, (float)num4, (float)num5);
	}

	public static Vector3 RotateZ(this Vector3 pos, double rad)
	{
		double num = Math.Cos(rad);
		double num2 = Math.Sin(rad);
		double num3 = (double)pos.x * num - (double)pos.y * num2;
		double num4 = (double)pos.x * num2 + (double)pos.y * num;
		double num5 = pos.z;
		return new Vector3((float)num3, (float)num4, (float)num5);
	}

	public static (float small, float big, bool has) RootFormula(float a, float b, float c)
	{
		if (Math.Abs(a) < 1E-08f)
		{
			if (Math.Abs(b) < 1E-08f)
			{
				return (small: float.NaN, big: float.NaN, has: false);
			}
			float num = (0f - c) / b;
			return (small: num, big: num, has: true);
		}
		float num2 = 0.5f * b;
		float num3 = num2 * num2 - a * c;
		if (num3 < -1E-08f || float.IsNaN(num3))
		{
			return (small: float.NaN, big: float.NaN, has: false);
		}
		float num4 = (float)Math.Sqrt(Math.Max(0f, num3));
		float val = (0f - num2 - num4) / a;
		float val2 = (0f - num2 + num4) / a;
		return (small: Math.Min(val, val2), big: Math.Max(val, val2), has: true);
	}

	public static Vector3? GetReachableRandomPosWithLimit(Vector3 origin, double _minRange, double _maxRange)
	{
		Vector3? result = null;
		for (int i = 0; i < 5; i++)
		{
			Vector3 randomPositionInRange = GetRandomPositionInRange(origin, _minRange, _maxRange);
			Vector3 vector = Hub.s.vworld.FindNearestPoly(randomPositionInRange);
			if (vector != Vector3.zero && Hub.s.vworld.FindPath(origin, vector, out List<Vector3> path))
			{
				int num = 0;
				for (int j = 0; j < path.Count - 1; j++)
				{
					num += (int)Distance(path[j], path[j + 1]);
				}
				if ((float)num < Distance(origin, vector) * 2f)
				{
					result = vector;
					break;
				}
			}
		}
		return result;
	}

	public static Vector3? GetReachableRandomPosForTeleport(Vector3 origin, double _minRange, double _maxRange)
	{
		for (int i = 0; i < 5; i++)
		{
			Vector3 randomPositionInRange = GetRandomPositionInRange(origin, _minRange, _maxRange);
			Vector3 vector = Hub.s.vworld.FindNearestPoly(randomPositionInRange);
			if (vector != Vector3.zero)
			{
				return vector;
			}
		}
		return null;
	}

	public static Vector3? GetDropPosition(Vector3 origin, float yaw, bool isFrontSide, float distance)
	{
		float angleDiff = (isFrontSide ? 0f : 180f);
		float f = GetDirectionAngle(yaw, angleDiff) * (MathF.PI / 180f);
		Vector3 vector = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f)) * distance;
		Vector3 pos = origin + vector;
		Vector3 vector2 = Hub.s.vworld.FindNearestPoly(pos);
		if (vector2 != Vector3.zero)
		{
			return vector2;
		}
		return null;
	}

	public static Vector3? GetReachableRandomPos(Vector3 origin, double _minRange, double _maxRange)
	{
		Vector3? result = null;
		for (int i = 0; i < 5; i++)
		{
			Vector3 randomPositionInRange = GetRandomPositionInRange(origin, _minRange, _maxRange);
			if (Hub.s.vworld.FindNearestPoly(randomPositionInRange) != Vector3.zero)
			{
				break;
			}
		}
		return result;
	}

	public static Vector3 NearestPoint2Segment(Vector3 point, Vector3 segFrom, Vector3 segTo)
	{
		Vector3 vector = segTo - segFrom;
		if (vector.sqrMagnitude < 1E-08f)
		{
			return segFrom;
		}
		Vector3 rhs = point - segFrom;
		float value = Vector3.Dot(vector, rhs) / vector.sqrMagnitude;
		value = Mathf.Clamp01(value);
		return segFrom + vector * value;
	}

	public static (Vector3, Vector3) NearestPointsSeg2Seg(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
	{
		Vector3 vector = q1 - p1;
		Vector3 vector2 = q2 - p2;
		Vector3 rhs = p1 - p2;
		float num = Vector3.Dot(vector, vector);
		float num2 = Vector3.Dot(vector, vector2);
		float num3 = Vector3.Dot(vector2, vector2);
		float num4 = Vector3.Dot(vector, rhs);
		float num5 = Vector3.Dot(vector2, rhs);
		float num6 = num * num3 - num2 * num2;
		float num7 = 0f;
		float num8 = 0f;
		if (num6 != 0f)
		{
			num7 = Mathf.Clamp01((num2 * num5 - num3 * num4) / num6);
		}
		num8 = (num2 * num7 + num5) / num3;
		if (num8 < 0f)
		{
			num8 = 0f;
			num7 = Mathf.Clamp01((0f - num4) / num);
		}
		else if (num8 > 1f)
		{
			num8 = 1f;
			num7 = Mathf.Clamp01((num2 - num4) / num);
		}
		Vector3 item = p1 + vector * num7;
		Vector3 item2 = p2 + vector2 * num8;
		return (item, item2);
	}

	public static bool AxisSeparated(Vector3 axis, Vector3 posDiff, Vector3[] axis1, Vector3[] axis2)
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < 3; i++)
		{
			num += Mathf.Abs(Vector3.Dot(axis, axis1[i]));
			num2 += Mathf.Abs(Vector3.Dot(axis, axis2[i]));
		}
		return Mathf.Abs(Vector3.Dot(axis, posDiff)) > num + num2;
	}

	public static bool RatioPointToSegByDistance(Vector3 point, Vector3 segFrom, Vector3 segTo, float distance, out float ratio)
	{
		ratio = 0f;
		Vector3 lhs = segTo - segFrom;
		float sqrMagnitude = lhs.sqrMagnitude;
		float num = distance * distance;
		if (sqrMagnitude < 1E-08f)
		{
			return Mathf.Abs((point - segFrom).sqrMagnitude - num) < 1E-08f;
		}
		Vector3 rhs = segFrom - point;
		(float, float, bool) tuple = RootFormula(sqrMagnitude, 2f * Vector3.Dot(lhs, rhs), rhs.sqrMagnitude - num);
		if (!tuple.Item3)
		{
			return false;
		}
		if ((double)tuple.Item1 >= 0.0 && (double)tuple.Item1 <= 1.0)
		{
			(ratio, _, _) = tuple;
			return true;
		}
		if ((double)tuple.Item2 >= 0.0 && (double)tuple.Item2 <= 1.0)
		{
			ratio = tuple.Item2;
			return true;
		}
		return false;
	}

	public static float DistanceSeg2Seg(Vector3 seg1Start, Vector3 seg1End, Vector3 seg2Start, Vector3 seg2End)
	{
		(Vector3, Vector3) tuple = NearestPointsSeg2Seg(seg1Start, seg1End, seg2Start, seg2End);
		return Vector3.Distance(tuple.Item1, tuple.Item2);
	}

	public static (bool valid, float remainDistance) ValidateMoveSpped(PosWithRot startPos, PosWithRot endPos, float creditDistance)
	{
		float num = Distance(startPos.pos, endPos.pos, ignoreHeight: true);
		if (num <= creditDistance)
		{
			return (valid: true, remainDistance: Math.Max(0f, creditDistance - num));
		}
		return (valid: false, remainDistance: 0f);
	}
}
