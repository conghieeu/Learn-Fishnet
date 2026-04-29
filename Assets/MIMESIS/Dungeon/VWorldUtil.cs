using System;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public static class VWorldUtil
{
	public static double Distance(PosWithRot posA, PosWithRot posB, bool ignoreHeight = false)
	{
		if (!ignoreHeight)
		{
			return Vector3.Distance(posA.toVector3(), posB.toVector3());
		}
		return Vector2.Distance(toXYVector2(posA.toVector3()), toXYVector2(posB.toVector3()));
	}

	public static bool CheckTargetLookingAtMe(Vector3 myPosition, Vector3 targetPosition, float targetYaw, float maxAngleDegrees)
	{
		Vector3 normalized = (myPosition - targetPosition).normalized;
		Vector3 vector = new Vector3(Mathf.Sin(targetYaw * (MathF.PI / 180f)), 0f, Mathf.Cos(targetYaw * (MathF.PI / 180f)));
		Vector3 normalized2 = new Vector3(normalized.x, 0f, normalized.z).normalized;
		return Misc.FacingAngle(vector, normalized2) <= maxAngleDegrees * 0.5f;
	}

	public static bool CheckVisible(Vector3 origin, Vector3 target, float originHeight, float targetHeight, float originRadius, float targetRadius)
	{
		return PhysicsUtility.CheckVisibility(origin, target, originRadius, targetRadius, originHeight, targetHeight);
	}

	public static (bool isHit, Vector3 hitPos, Vector3 normal) GetWallHitPos(Vector3 origin, Vector3 target)
	{
		return PhysicsUtility.GetWallHitPos(origin, target);
	}

	public static double Distance(Vector3 posA, Vector3 posB, bool ignoreHeight = false)
	{
		if (!ignoreHeight)
		{
			return Vector3.Distance(posA, posB);
		}
		return Vector2.Distance(toXYVector2(posA), toXYVector2(posB));
	}

	public static Vector2 toXYVector2(Vector3 input)
	{
		return new Vector2(input.x, input.z);
	}

	public static Vector3 toVector3(this PosWithRot pos)
	{
		return new Vector3(pos.x, pos.y, pos.z);
	}

	public static Rotator toRotation(this PosWithRot pos)
	{
		return new Rotator(pos.pitch, pos.yaw, pos.roll);
	}

	public static bool IsNaN(PosWithRot pos)
	{
		if (!float.IsNaN(pos.x) && !float.IsNaN(pos.y) && !float.IsNaN(pos.z) && !float.IsNaN(pos.yaw) && !float.IsNaN(pos.pitch))
		{
			return float.IsNaN(pos.roll);
		}
		return true;
	}

	public static PosWithRot toPosWithRot(this Vector3 pos, float angle, float pitch = 0f, float roll = 0f)
	{
		PosWithRot posWithRot = new PosWithRot();
		posWithRot.x = pos.x;
		posWithRot.y = pos.y;
		posWithRot.z = pos.z;
		posWithRot.yaw = angle;
		posWithRot.pitch = pitch;
		posWithRot.roll = roll;
		return posWithRot;
	}

	public static PosWithRot toPosWithRot(this Transform transform, bool ignoreRoll = true)
	{
		PosWithRot posWithRot = new PosWithRot();
		posWithRot.x = transform.position.x;
		posWithRot.y = transform.position.y;
		posWithRot.z = transform.position.z;
		posWithRot.yaw = transform.rotation.eulerAngles.y;
		posWithRot.pitch = transform.rotation.eulerAngles.x;
		posWithRot.roll = (ignoreRoll ? 0f : transform.rotation.eulerAngles.z);
		return posWithRot;
	}

	public static long ConvertTimeToSeconds(string time)
	{
		if (TimeSpan.TryParse(time, out var result))
		{
			return (long)result.TotalSeconds;
		}
		return 0L;
	}

	public static (long, long) ConvertTimeToSeconds(string startTime, string endTime)
	{
		long num = ConvertTimeToSeconds(startTime);
		long num2 = ConvertTimeToSeconds(endTime);
		if (num2 < num)
		{
			num2 += 86400;
		}
		return (num, num2);
	}

	public static void HandleError<T>(this MsgErrorCode errorCode, VPlayer player, int hashCode) where T : IResMsg, new()
	{
		if (errorCode != MsgErrorCode.Success)
		{
			Logger.RWarn($"Player {player.ObjectID} HandleError<{typeof(T).Name}> ErrorCode: {errorCode}, HashCode: {hashCode}");
			T val = new T
			{
				errorCode = errorCode
			};
			val.hashCode = hashCode;
			player.SendToMe(val);
		}
	}

	public static ReasonOfDeath ConvertMutableChangeCause(MutableStatChangeCause cause)
	{
		switch (cause)
		{
		case MutableStatChangeCause.ActiveAttack:
			return ReasonOfDeath.Skill;
		case MutableStatChangeCause.AdminCommand:
			return ReasonOfDeath.AdminKill;
		case MutableStatChangeCause.AdminKill:
			return ReasonOfDeath.AdminKill;
		case MutableStatChangeCause.UseItem:
			return ReasonOfDeath.GameSystem;
		case MutableStatChangeCause.SystemNormal:
			return ReasonOfDeath.GameSystem;
		case MutableStatChangeCause.SystemDungeon:
			return ReasonOfDeath.DungeonEnd;
		case MutableStatChangeCause.AbnormalDamage:
			return ReasonOfDeath.Abnormal;
		case MutableStatChangeCause.Conta:
			return ReasonOfDeath.Conta;
		case MutableStatChangeCause.FieldSkill:
			return ReasonOfDeath.FieldSkill;
		case MutableStatChangeCause.Fall:
			return ReasonOfDeath.Fall;
		default:
			Logger.RError($"Unknown ReasonOfDeath : {cause}");
			return ReasonOfDeath.None;
		}
	}

	public static PosWithRot GetTargetPos(PosWithRot origin, Vector3 posDiff, float angleDiff, float pitchDiff, float rollDiff = 0f)
	{
		PosWithRot posWithRot = new PosWithRot();
		posWithRot.x = origin.x + posDiff.x;
		posWithRot.y = origin.y + posDiff.y;
		posWithRot.z = origin.z + posDiff.z;
		posWithRot.yaw = Misc.GetDirectionAngle(origin.yaw, angleDiff);
		posWithRot.pitch = Misc.GetDirectionAngle(origin.pitch, pitchDiff);
		posWithRot.roll = Misc.GetDirectionAngle(origin.roll, rollDiff);
		return posWithRot;
	}

	public static void CollectHitCheckDebugInfo(int ObjectID, PosWithRot posWithRot, IHitCheck hitcheck, ref DebugInfoSig sig)
	{
		if (posWithRot == null || hitcheck == null)
		{
			return;
		}
		switch (hitcheck.ShapeType)
		{
		case HitCheckShapeType.Cube:
			if (hitcheck is CubeHitCheck cubeHitCheck)
			{
				sig.hitCheckDrawInfos.Add(new CubeHitCheckDrawInfo
				{
					actorID = ObjectID,
					center = posWithRot.toVector3() + Quaternion.Euler(0f, posWithRot.yaw, 0f) * hitcheck.Center,
					rotation = new Rotator(hitcheck.Rotation),
					shapeType = HitCheckShapeType.Cube,
					Extent = cubeHitCheck.Extent
				});
			}
			break;
		case HitCheckShapeType.Capsule:
			if (hitcheck is CapsuleHitCheck capsuleHitCheck)
			{
				sig.hitCheckDrawInfos.Add(new CapsuleHitCheckDrawInfo
				{
					actorID = ObjectID,
					center = posWithRot.toVector3() + Quaternion.Euler(0f, posWithRot.yaw, 0f) * hitcheck.Center,
					rotation = new Rotator(hitcheck.Rotation.ToQuaternion() * Quaternion.Euler(0f, posWithRot.yaw, 0f)),
					shapeType = HitCheckShapeType.Capsule,
					Radius = capsuleHitCheck.Rad,
					Length = capsuleHitCheck.Length
				});
			}
			break;
		case HitCheckShapeType.Sphere:
			if (hitcheck is SphereHitCheck sphereHitCheck)
			{
				sig.hitCheckDrawInfos.Add(new SphereHitCheckDrawInfo
				{
					actorID = ObjectID,
					center = posWithRot.toVector3() + Quaternion.Euler(0f, posWithRot.yaw, 0f) * hitcheck.Center,
					rotation = new Rotator(hitcheck.Rotation.ToQuaternion() * Quaternion.Euler(0f, posWithRot.yaw, 0f)),
					shapeType = HitCheckShapeType.Sphere,
					Radius = sphereHitCheck.Rad
				});
			}
			break;
		case HitCheckShapeType.Fan:
			if (hitcheck is FanHitCheck fanHitCheck)
			{
				sig.hitCheckDrawInfos.Add(new FanHitCheckDrawInfo
				{
					actorID = ObjectID,
					center = posWithRot.toVector3() + Quaternion.Euler(0f, posWithRot.yaw, 0f) * hitcheck.Center,
					rotation = new Rotator(hitcheck.Rotation.ToQuaternion() * Quaternion.Euler(0f, posWithRot.yaw, 0f)),
					shapeType = HitCheckShapeType.Fan,
					InnerRad = fanHitCheck.InnerRad,
					OuterRad = fanHitCheck.OuterRad,
					Height = fanHitCheck.Height,
					Angle = fanHitCheck.Angle
				});
			}
			break;
		case HitCheckShapeType.Torus:
			if (hitcheck is TorusHitCheck torusHitCheck)
			{
				sig.hitCheckDrawInfos.Add(new TorusHitCheckDrawInfo
				{
					actorID = ObjectID,
					center = posWithRot.toVector3() + Quaternion.Euler(0f, posWithRot.yaw, 0f) * hitcheck.Center,
					rotation = new Rotator(hitcheck.Rotation.ToQuaternion() * Quaternion.Euler(0f, posWithRot.yaw, 0f)),
					shapeType = HitCheckShapeType.Fan,
					InnerRad = torusHitCheck.InnerRad,
					OuterRad = torusHitCheck.OuterRad,
					Height = torusHitCheck.Height
				});
			}
			break;
		}
	}
}
