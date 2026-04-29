using UnityEngine;

namespace EzySlice
{
	public struct Plane
	{
		private Vector3 m_normal;

		private float m_dist;

		public Vector3 normal => m_normal;

		public float dist => m_dist;

		public Plane(Vector3 pos, Vector3 norm)
		{
			m_normal = norm;
			m_dist = Vector3.Dot(norm, pos);
		}

		public Plane(Vector3 norm, float dot)
		{
			m_normal = norm;
			m_dist = dot;
		}

		public Plane(Vector3 a, Vector3 b, Vector3 c)
		{
			m_normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
			m_dist = 0f - Vector3.Dot(m_normal, a);
		}

		public void Compute(Vector3 pos, Vector3 norm)
		{
			m_normal = norm;
			m_dist = Vector3.Dot(norm, pos);
		}

		public void Compute(Transform trans)
		{
			Compute(trans.position, trans.up);
		}

		public void Compute(GameObject obj)
		{
			Compute(obj.transform);
		}

		public SideOfPlane SideOf(Vector3 pt)
		{
			float num = Vector3.Dot(m_normal, pt) - m_dist;
			if (num > 0.0001f)
			{
				return SideOfPlane.UP;
			}
			if (num < -0.0001f)
			{
				return SideOfPlane.DOWN;
			}
			return SideOfPlane.ON;
		}

		public void OnDebugDraw()
		{
			OnDebugDraw(Color.white);
		}

		public void OnDebugDraw(Color drawColor)
		{
		}
	}
}
