using System;
using UnityEngine;

public static class RotatorUtil
{
	public static Vector3 toRotatorVector(this Vector3 direction)
	{
		return direction.ToQuaternionFromDirVector().eulerAngles;
	}

	public static Vector3 ToDirectionVector(this Rotator rotator)
	{
		double num = rotator.Pitch.toRadian();
		double num2 = rotator.Yaw.toRadian();
		rotator.Roll.toRadian();
		double num3 = Math.Cos(num);
		double num4 = Math.Sin(num);
		double num5 = Math.Cos(num2);
		return new Vector3((float)(Math.Sin(num2) * num4), (float)num3, (float)(num5 * num4));
	}

	public static Quaternion ToQuaternionFromEuler(this Vector3 rot)
	{
		return Quaternion.Euler(rot);
	}

	public static Quaternion ToQuaternionFromDirVector(this Vector3 dir)
	{
		if (dir == Vector3.zero)
		{
			return Quaternion.identity;
		}
		return Quaternion.LookRotation(dir);
	}

	public static Vector3 toVector3(this Rotator rotator)
	{
		return new Vector3(rotator.Pitch, rotator.Yaw, rotator.Roll);
	}
}
