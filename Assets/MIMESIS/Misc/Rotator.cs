using System;
using UnityEngine;

public struct Rotator
{
	public float Pitch;

	public float Yaw;

	public float Roll;

	public static Rotator Zero => new Rotator(0f, 0f, 0f);

	public Rotator(float pitch, float yaw, float roll)
	{
		Pitch = pitch;
		Yaw = yaw;
		Roll = roll;
	}

	public Rotator(Rotator other)
	{
		Pitch = other.Pitch;
		Yaw = other.Yaw;
		Roll = other.Roll;
	}

	public override string ToString()
	{
		return $"Pitch : {Pitch} / Yaw : {Yaw} / Roll : {Roll}";
	}

	public Rotator(Quaternion quaternion)
	{
		Vector3 eulerAngles = quaternion.eulerAngles;
		Pitch = NormalizeAngle(eulerAngles.x);
		Yaw = NormalizeAngle(eulerAngles.y);
		Roll = NormalizeAngle(eulerAngles.z);
	}

	public Rotator(Vector3 eulerAngles)
	{
		Pitch = NormalizeAngle(eulerAngles.x);
		Yaw = NormalizeAngle(eulerAngles.y);
		Roll = NormalizeAngle(eulerAngles.z);
	}

	public static float NormalizeAngle(float angle)
	{
		angle %= 360f;
		if (angle > 180f)
		{
			angle -= 360f;
		}
		else if (angle < -180f)
		{
			angle += 360f;
		}
		return angle;
	}

	public Quaternion ToQuaternion()
	{
		return Quaternion.Euler(Pitch, Yaw, Roll);
	}

	public Vector3 GetForwardVector()
	{
		return ToQuaternion() * Vector3.forward;
	}

	public Vector3 GetRightVector()
	{
		return ToQuaternion() * Vector3.right;
	}

	public Vector3 GetUpVector()
	{
		return ToQuaternion() * Vector3.up;
	}

	public Matrix4x4 ToMatrix()
	{
		return Matrix4x4.Rotate(ToQuaternion());
	}

	public Rotator GetDifference(Rotator other)
	{
		return new Rotator(NormalizeAngle(Pitch - other.Pitch), NormalizeAngle(Yaw - other.Yaw), NormalizeAngle(Roll - other.Roll));
	}

	public static Rotator Lerp(Rotator from, Rotator to, float t)
	{
		return new Rotator(Quaternion.Lerp(from.ToQuaternion(), to.ToQuaternion(), t));
	}

	public static Rotator Slerp(Rotator from, Rotator to, float t)
	{
		return new Rotator(Quaternion.Slerp(from.ToQuaternion(), to.ToQuaternion(), t));
	}

	public static Rotator operator +(Rotator a, Rotator b)
	{
		return new Rotator(NormalizeAngle(a.Pitch + b.Pitch), NormalizeAngle(a.Yaw + b.Yaw), NormalizeAngle(a.Roll + b.Roll));
	}

	public static Rotator operator -(Rotator a, Rotator b)
	{
		return new Rotator(NormalizeAngle(a.Pitch - b.Pitch), NormalizeAngle(a.Yaw - b.Yaw), NormalizeAngle(a.Roll - b.Roll));
	}

	public static Rotator operator *(Rotator rotator, float scale)
	{
		return new Rotator(rotator.Pitch * scale, rotator.Yaw * scale, rotator.Roll * scale);
	}

	public static implicit operator Quaternion(Rotator rotator)
	{
		return rotator.ToQuaternion();
	}

	public static implicit operator Rotator(Quaternion quaternion)
	{
		return new Rotator(quaternion);
	}

	public static implicit operator Vector3(Rotator rotator)
	{
		return new Vector3(rotator.Pitch, rotator.Yaw, rotator.Roll);
	}

	public static implicit operator Rotator(Vector3 eulerAngles)
	{
		return new Rotator(eulerAngles);
	}

	public override bool Equals(object obj)
	{
		if (obj is Rotator rotator)
		{
			if (Mathf.Approximately(Pitch, rotator.Pitch) && Mathf.Approximately(Yaw, rotator.Yaw))
			{
				return Mathf.Approximately(Roll, rotator.Roll);
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Pitch, Yaw, Roll);
	}
}
