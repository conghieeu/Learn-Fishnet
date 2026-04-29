using UnityEngine;

namespace Mimic.Actors
{
	public class ProjectileHit
	{
		public int ProjectileActorID { get; }

		public Vector3 FinalPosition { get; }

		public Quaternion FinalRotation { get; }

		public Vector3 HitPosition { get; }

		public Transform HitTransform { get; }

		public Vector3 SurfaceNormal { get; }

		public ProjectileHit(int projectileActorID, Vector3 finalPosition, Quaternion finalRotation, Vector3 hitPoint, Transform hitTransform, Vector3 surfaceNormal)
		{
			ProjectileActorID = projectileActorID;
			FinalPosition = finalPosition;
			FinalRotation = finalRotation;
			HitPosition = hitPoint;
			HitTransform = hitTransform;
			SurfaceNormal = surfaceNormal;
		}
	}
}
