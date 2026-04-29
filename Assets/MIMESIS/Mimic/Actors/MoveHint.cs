using ReluProtocol;
using UnityEngine;

namespace Mimic.Actors
{
	public struct MoveHint
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public float Time;

		public MoveHint(Vector3 position, Quaternion rotation, float time)
		{
			Position = position;
			Rotation = rotation;
			Time = time;
		}

		public void UpdatePositionAndRotation(Vector3 position, Quaternion rotation, float time)
		{
			Position = position;
			Rotation = rotation;
			Time = time;
		}

		public void UpdatePositionAndRotation(PosWithRot posWithRot, float time)
		{
			Position = posWithRot.toVector3();
			Rotation = Quaternion.Euler(posWithRot.pitch, posWithRot.yaw, 0f);
			Time = time;
		}
	}
}
