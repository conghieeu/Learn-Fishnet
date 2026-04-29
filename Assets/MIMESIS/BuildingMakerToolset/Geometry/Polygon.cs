using UnityEngine;

namespace BuildingMakerToolset.Geometry
{
	public class Polygon
	{
		public readonly Vector2[] points;

		public readonly int numPoints;

		public readonly int numHullPoints;

		public readonly int[] numPointsPerHole;

		public readonly int numHoles;

		private readonly int[] holeStartIndices;

		public Polygon(Vector2[] hull, Vector2[][] holes)
		{
			numHullPoints = hull.Length;
			numHoles = holes.GetLength(0);
			numPointsPerHole = new int[numHoles];
			holeStartIndices = new int[numHoles];
			int num = 0;
			for (int i = 0; i < holes.GetLength(0); i++)
			{
				numPointsPerHole[i] = holes[i].Length;
				holeStartIndices[i] = numHullPoints + num;
				num += numPointsPerHole[i];
			}
			numPoints = numHullPoints + num;
			points = new Vector2[numPoints];
			bool flag = !PointsAreCounterClockwise(hull);
			for (int j = 0; j < numHullPoints; j++)
			{
				points[j] = hull[flag ? (numHullPoints - 1 - j) : j];
			}
			for (int k = 0; k < numHoles; k++)
			{
				bool flag2 = PointsAreCounterClockwise(holes[k]);
				for (int l = 0; l < holes[k].Length; l++)
				{
					points[IndexOfPointInHole(l, k)] = holes[k][flag2 ? (holes[k].Length - l - 1) : l];
				}
			}
		}

		public Polygon(Vector2[] hull)
			: this(hull, new Vector2[0][])
		{
		}

		public static bool PointsAreCounterClockwise(Vector2[] testPoints)
		{
			float num = 0f;
			for (int i = 0; i < testPoints.Length; i++)
			{
				int num2 = (i + 1) % testPoints.Length;
				num += (testPoints[num2].x - testPoints[i].x) * (testPoints[num2].y + testPoints[i].y);
			}
			return num < 0f;
		}

		public int IndexOfFirstPointInHole(int holeIndex)
		{
			return holeStartIndices[holeIndex];
		}

		public int IndexOfPointInHole(int index, int holeIndex)
		{
			return holeStartIndices[holeIndex] + index;
		}

		public Vector2 GetHolePoint(int index, int holeIndex)
		{
			return points[holeStartIndices[holeIndex] + index];
		}
	}
}
