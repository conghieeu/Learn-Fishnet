using UnityEngine;

namespace Mimic.Character.HitSystem
{
	public class FanHitbox : Hitbox
	{
		[SerializeField]
		[Range(0f, 360f)]
		private float angle = 30f;

		[SerializeField]
		private float height = 2f;

		[SerializeField]
		[Range(0f, float.MaxValue)]
		private float innerRadius = 1f;

		[SerializeField]
		[Range(0f, float.MaxValue)]
		private float outerRadius = 2f;

		[SerializeField]
		private bool drawInSceneView;

		public float Angle => angle;

		public float Height => height;

		public float InnerRadius => innerRadius;

		public float OuterRadius => outerRadius;

		public Vector3 Position => base.transform.position;

		public Vector3 AxialTop => base.transform.position + Vector3.up * height;

		public Vector3 AxialBottom => base.transform.position;

		public Vector3 Right => new Vector3(base.transform.right.x, 0f, base.transform.right.z).normalized;

		public Vector3 Up => Vector3.up;

		public Vector3 Forward => new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
	}
}
