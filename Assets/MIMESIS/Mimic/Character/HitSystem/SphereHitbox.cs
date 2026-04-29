using UnityEngine;

namespace Mimic.Character.HitSystem
{
	public class SphereHitbox : Hitbox
	{
		[SerializeField]
		[Range(0f, float.MaxValue)]
		private float radius = 1f;

		[SerializeField]
		private bool drawInSceneView;

		public Vector3 Center => base.transform.position;

		public float Radius => radius;
	}
}
