using UnityEngine;

namespace EzySlice
{
	public sealed class IntersectionResult
	{
		private bool is_success;

		private readonly Triangle[] upper_hull;

		private readonly Triangle[] lower_hull;

		private readonly Vector3[] intersection_pt;

		private int upper_hull_count;

		private int lower_hull_count;

		private int intersection_pt_count;

		public Triangle[] upperHull => upper_hull;

		public Triangle[] lowerHull => lower_hull;

		public Vector3[] intersectionPoints => intersection_pt;

		public int upperHullCount => upper_hull_count;

		public int lowerHullCount => lower_hull_count;

		public int intersectionPointCount => intersection_pt_count;

		public bool isValid => is_success;

		public IntersectionResult()
		{
			is_success = false;
			upper_hull = new Triangle[2];
			lower_hull = new Triangle[2];
			intersection_pt = new Vector3[2];
			upper_hull_count = 0;
			lower_hull_count = 0;
			intersection_pt_count = 0;
		}

		public IntersectionResult AddUpperHull(Triangle tri)
		{
			upper_hull[upper_hull_count++] = tri;
			is_success = true;
			return this;
		}

		public IntersectionResult AddLowerHull(Triangle tri)
		{
			lower_hull[lower_hull_count++] = tri;
			is_success = true;
			return this;
		}

		public void AddIntersectionPoint(Vector3 pt)
		{
			intersection_pt[intersection_pt_count++] = pt;
		}

		public void Clear()
		{
			is_success = false;
			upper_hull_count = 0;
			lower_hull_count = 0;
			intersection_pt_count = 0;
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
