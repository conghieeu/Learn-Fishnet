using UnityEngine;

namespace Mimic.Character.HitSystem
{
	public class CubeHitbox : Hitbox
	{
		[SerializeField]
		private Vector3 extent = Vector3.one * 0.5f;

		[SerializeField]
		private bool drawInSceneView;

		public Vector3 Center => base.transform.position;

		public Quaternion Rotation => base.transform.rotation;

		public float[] Extent => new float[3] { extent.x, extent.y, extent.z };
	}
}
