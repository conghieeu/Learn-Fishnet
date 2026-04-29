using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BuildingMakerToolset.Geometry
{
	public class CompositeShape
	{
		public class ShapeMeshData
		{
			public Mesh backedMesh;

			public Material material;

			public int[] triangles;

			public Vector3[] vertices;

			public Vector2[] uv;
		}

		public class CompositeShapeData
		{
			public struct LineSegment
			{
				public readonly Vector2 a;

				public readonly Vector2 b;

				public LineSegment(Vector2 a, Vector2 b)
				{
					this.a = a;
					this.b = b;
				}
			}

			public Vector2[] points;

			public Polygon polygon;

			public int[] triangles;

			public Shape baseShape;

			public Material material;

			public bool isOtherSideOfThickShape;

			public List<CompositeShapeData> parents = new List<CompositeShapeData>();

			public List<CompositeShapeData> holes = new List<CompositeShapeData>();

			public bool IsValidShape { get; private set; }

			public CompositeShapeData(Vector2[] points, Transform transform, Shape shape)
			{
				SetData(points, transform, shape);
			}

			private void SetData(Vector2[] points, Transform transform, Shape shape)
			{
				baseShape = shape;
				this.points = points;
				IsValidShape = points.Length >= 3 && !IntersectsWithSelf();
				if (IsValidShape)
				{
					polygon = new Polygon(this.points);
					Triangulator triangulator = new Triangulator(polygon);
					triangles = triangulator.Triangulate();
				}
			}

			public void ValidateHoles()
			{
				for (int i = 0; i < holes.Count; i++)
				{
					for (int j = i + 1; j < holes.Count; j++)
					{
						if (holes[i].OverlapsPartially(holes[j]))
						{
							holes[i].IsValidShape = false;
							break;
						}
					}
				}
				for (int num = holes.Count - 1; num >= 0; num--)
				{
					if (!holes[num].IsValidShape)
					{
						holes.RemoveAt(num);
					}
				}
				bool flag = false;
				while (!flag)
				{
					bool flag2 = false;
					for (int k = 0; k < holes.Count; k++)
					{
						if (holes[k].parents == null || holes[k].parents.Count == 0)
						{
							continue;
						}
						for (int l = 0; l < holes[k].parents.Count; l++)
						{
							if (holes.Contains(holes[k].parents[l]))
							{
								holes.Remove(holes[k]);
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						flag = true;
					}
				}
			}

			public bool IsInCutrangeOfHoleShape(CompositeShapeData holeShape)
			{
				float num = (isOtherSideOfThickShape ? (baseShape.hightOffset + baseShape.thickness) : baseShape.hightOffset);
				if (Mathf.Min(holeShape.baseShape.hightOffset, holeShape.baseShape.hightOffset + holeShape.baseShape.thickness) <= num && num <= Mathf.Max(holeShape.baseShape.hightOffset, holeShape.baseShape.hightOffset + holeShape.baseShape.thickness))
				{
					return true;
				}
				return false;
			}

			public bool IsParentOf(CompositeShapeData otherShape)
			{
				if (triangles == null || triangles.Length == 0)
				{
					return false;
				}
				if (otherShape.parents.Contains(this))
				{
					return true;
				}
				if (parents.Contains(otherShape))
				{
					return false;
				}
				bool flag = false;
				for (int i = 0; i < triangles.Length; i += 3)
				{
					if (Maths2D.PointInTriangle(polygon.points[triangles[i]], polygon.points[triangles[i + 1]], polygon.points[triangles[i + 2]], otherShape.points[0]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
				for (int j = 0; j < points.Length; j++)
				{
					LineSegment lineSegment = new LineSegment(points[j], points[(j + 1) % points.Length]);
					for (int k = 0; k < otherShape.points.Length; k++)
					{
						LineSegment lineSegment2 = new LineSegment(otherShape.points[k], otherShape.points[(k + 1) % otherShape.points.Length]);
						if (Maths2D.LineSegmentsIntersect(lineSegment.a, lineSegment.b, lineSegment2.a, lineSegment2.b))
						{
							return false;
						}
					}
				}
				return true;
			}

			public bool CheckIfSameShape(CompositeShapeData otherShape)
			{
				if (otherShape == this)
				{
					return false;
				}
				if (points.Length != otherShape.points.Length)
				{
					return false;
				}
				for (int i = 0; i < points.Length; i++)
				{
					if (points[i] != otherShape.points[i])
					{
						return false;
					}
				}
				return true;
			}

			public bool OverlapsPartially(CompositeShapeData otherShape)
			{
				for (int i = 0; i < points.Length; i++)
				{
					LineSegment lineSegment = new LineSegment(points[i], points[(i + 1) % points.Length]);
					for (int j = 0; j < otherShape.points.Length; j++)
					{
						LineSegment lineSegment2 = new LineSegment(otherShape.points[j], otherShape.points[(j + 1) % otherShape.points.Length]);
						if (Maths2D.LineSegmentsIntersect(lineSegment.a, lineSegment.b, lineSegment2.a, lineSegment2.b))
						{
							return true;
						}
					}
				}
				return false;
			}

			public bool IntersectsWithSelf()
			{
				for (int i = 0; i < points.Length; i++)
				{
					LineSegment lineSegment = new LineSegment(points[i], points[(i + 1) % points.Length]);
					for (int j = i + 2; j < points.Length; j++)
					{
						if ((j + 1) % points.Length != i)
						{
							LineSegment lineSegment2 = new LineSegment(points[j], points[(j + 1) % points.Length]);
							if (Maths2D.LineSegmentsIntersect(lineSegment.a, lineSegment.b, lineSegment2.a, lineSegment2.b))
							{
								return true;
							}
						}
					}
				}
				return false;
			}
		}

		public List<ShapeMeshData> shapeMeshes;

		private Shape[] shapes;

		private Transform transform;

		private float height;

		public CompositeShape(IEnumerable<Shape> shapes)
		{
			this.shapes = shapes.ToArray();
		}

		public ShapeMeshData[] GetShapeMeshData(Transform tr = null, bool generateUV2 = true)
		{
			transform = tr;
			if (!Process())
			{
				return null;
			}
			for (int i = 0; i < shapeMeshes.Count; i++)
			{
				Mesh mesh = new Mesh();
				mesh.SetVertices(shapeMeshes[i].vertices);
				mesh.SetUVs(0, shapeMeshes[i].uv);
				mesh.SetTriangles(shapeMeshes[i].triangles, 0);
				mesh.RecalculateNormals();
				mesh.RecalculateTangents();
				shapeMeshes[i].backedMesh = mesh;
			}
			return shapeMeshes.ToArray();
		}

		public bool Process()
		{
			List<CompositeShapeData> list = (from shape in shapes
				select new CompositeShapeData(Maths2D.ToXZ(shape.points.ToArray()), transform, shape) into x
				where x.IsValidShape
				select x).ToList();
			List<CompositeShapeData> list2 = new List<CompositeShapeData>();
			for (int num = 0; num < list.Count; num++)
			{
				if (list[num].baseShape.thickness != 0f && !list[num].isOtherSideOfThickShape)
				{
					if (list[num].baseShape.wall)
					{
						list2.Add(list[num]);
					}
					if (!list[num].baseShape.hole)
					{
						CompositeShapeData compositeShapeData = new CompositeShapeData(list[num].points, transform, list[num].baseShape);
						compositeShapeData.isOtherSideOfThickShape = true;
						list.Add(compositeShapeData);
					}
				}
			}
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					if (num2 != num3 && list[num2].IsParentOf(list[num3]))
					{
						list[num3].parents.Add(list[num2]);
					}
				}
			}
			List<CompositeShapeData> list3 = list.Where((CompositeShapeData x) => x.baseShape.hole).ToList();
			List<CompositeShapeData> list4 = new List<CompositeShapeData>();
			foreach (CompositeShapeData item in list3)
			{
				for (int num4 = 0; num4 < list3.Count; num4++)
				{
					if (list3[num4].CheckIfSameShape(item) && !list4.Contains(list3[num4]))
					{
						list4.Add(list3[num4]);
					}
				}
			}
			foreach (CompositeShapeData item2 in list4)
			{
				list3.Remove(item2);
			}
			CompositeShapeData[] solidShapes = list.Where((CompositeShapeData x) => !x.baseShape.hole).ToArray();
			for (int num5 = 0; num5 < solidShapes.Length; num5++)
			{
				for (int num6 = 0; num6 < list3.Count; num6++)
				{
					if (solidShapes[num5].IsInCutrangeOfHoleShape(list3[num6]) && solidShapes[num5].IsParentOf(list3[num6]))
					{
						solidShapes[num5].holes.Add(list3[num6]);
					}
				}
			}
			CompositeShapeData[] array = solidShapes;
			for (int num7 = 0; num7 < array.Length; num7++)
			{
				array[num7].ValidateHoles();
			}
			shapeMeshes = new List<ShapeMeshData>();
			int i;
			for (i = 0; i < solidShapes.Length; i++)
			{
				ShapeMeshData shapeMeshData = new ShapeMeshData();
				Polygon polygon = new Polygon(solidShapes[i].polygon.points, solidShapes[i].holes.Select((CompositeShapeData h) => h.polygon.points).ToArray());
				shapeMeshData.vertices = polygon.points.Select((Vector2 v2) => new Vector3(v2.x, height + solidShapes[i].baseShape.hightOffset + (solidShapes[i].isOtherSideOfThickShape ? solidShapes[i].baseShape.thickness : 0f), v2.y)).ToArray();
				Vector2 uvOffset = ((!solidShapes[i].isOtherSideOfThickShape) ? solidShapes[i].baseShape.uvOffset : solidShapes[i].baseShape.uvOffset2) * 0.3f;
				Vector2 uvScale = ((!solidShapes[i].isOtherSideOfThickShape) ? solidShapes[i].baseShape.uvScale : solidShapes[i].baseShape.uvScale2) * 0.3f;
				shapeMeshData.uv = polygon.points.Select((Vector2 v2) => new Vector2(uvOffset.x + v2.x * uvScale.x, uvOffset.y + v2.y * uvScale.y)).ToArray();
				List<int> list5 = new List<int>();
				int[] array2 = new Triangulator(polygon).Triangulate();
				if ((!solidShapes[i].isOtherSideOfThickShape && solidShapes[i].baseShape.flip) || (solidShapes[i].isOtherSideOfThickShape && !solidShapes[i].baseShape.flip))
				{
					Array.Reverse(array2);
				}
				if (array2 == null)
				{
					return false;
				}
				for (int num8 = 0; num8 < array2.Length; num8++)
				{
					list5.Add(array2[num8]);
				}
				shapeMeshData.material = (solidShapes[i].isOtherSideOfThickShape ? solidShapes[i].baseShape.downMaterial : solidShapes[i].baseShape.upMaterial);
				shapeMeshData.triangles = list5.ToArray();
				shapeMeshes.Add(shapeMeshData);
			}
			for (int num9 = 0; num9 < list2.Count; num9++)
			{
				shapeMeshes.Add(EdgeMesh(list2[num9]));
			}
			for (int num10 = 0; num10 < shapeMeshes.Count; num10++)
			{
				for (int num11 = 0; num11 < shapeMeshes.Count; num11++)
				{
					if (num11 != num10 && shapeMeshes[num10].material == shapeMeshes[num11].material)
					{
						MergeShapeMeshData(num10, num11);
						shapeMeshes.RemoveAt(num11);
						num11--;
					}
				}
			}
			return true;
		}

		private ShapeMeshData EdgeMesh(CompositeShapeData wallData)
		{
			Vector3[] array = new Vector3[wallData.points.Length * 4 + 4];
			Vector2[] array2 = new Vector2[array.Length];
			int[] array3 = new int[(array.Length / 2 + 2) * 3];
			int num = 0;
			int num2 = 0;
			Vector2 vector = wallData.baseShape.wallUvOffset * 0.3f;
			Vector2 vector2 = wallData.baseShape.wallUvScale * 0.3f;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < wallData.points.Length + 1; i++)
			{
				Vector3 a = wallData.points[i % wallData.points.Length];
				array[num] = new Vector3(a.x, height + wallData.baseShape.hightOffset, a.y);
				array2[num] = new Vector2(vector.x + num4 * vector2.x, vector.y + num3 * vector2.y);
				num++;
				array[num] = new Vector3(a.x, height + wallData.baseShape.hightOffset + wallData.baseShape.thickness, a.y);
				array2[num] = new Vector2(vector.x + num4 * vector2.x, vector.y + (num3 + Mathf.Abs(wallData.baseShape.thickness) * vector2.y));
				num++;
				if (i < wallData.points.Length)
				{
					array3[num2] = num;
					array3[num2 + 1] = (num + 2) % array.Length;
					array3[num2 + 2] = num + 1;
					array3[num2 + 3] = num + 1;
					array3[num2 + 4] = (num + 2) % array.Length;
					array3[num2 + 5] = (num + 3) % array.Length;
				}
				num2 += 6;
				array[num] = new Vector3(a.x, height + wallData.baseShape.hightOffset, a.y);
				array2[num] = new Vector2(vector.x + num4 * vector2.x, vector.y + num3 * vector2.y);
				num++;
				array[num] = new Vector3(a.x, height + wallData.baseShape.hightOffset + wallData.baseShape.thickness, a.y);
				array2[num] = new Vector2(vector.x + num4 * vector2.x, vector.y + (num3 + Mathf.Abs(wallData.baseShape.thickness) * vector2.y));
				num++;
				num4 += Vector3.Distance(a, wallData.points[(i + 1) % wallData.points.Length]);
			}
			if (Polygon.PointsAreCounterClockwise(wallData.points))
			{
				if ((wallData.baseShape.hole && !wallData.baseShape.flip) || (!wallData.baseShape.hole && wallData.baseShape.flip))
				{
					Array.Reverse(array3);
				}
			}
			else if ((!wallData.baseShape.hole && !wallData.baseShape.flip) || (wallData.baseShape.hole && wallData.baseShape.flip))
			{
				Array.Reverse(array3);
			}
			return new ShapeMeshData
			{
				material = wallData.baseShape.sideMaterial,
				vertices = array,
				uv = array2,
				triangles = array3
			};
		}

		private void MergeShapeMeshData(int addTo, int remove)
		{
			ShapeMeshData shapeMeshData = shapeMeshes[addTo];
			ShapeMeshData shapeMeshData2 = shapeMeshes[remove];
			Vector3[] array = new Vector3[shapeMeshData.vertices.Length + shapeMeshData2.vertices.Length];
			Vector2[] array2 = new Vector2[array.Length];
			int num = shapeMeshData.vertices.Length;
			for (int i = 0; i < array.Length; i++)
			{
				if (i < num)
				{
					array[i] = shapeMeshData.vertices[i];
					array2[i] = shapeMeshData.uv[i];
				}
				else
				{
					array[i] = shapeMeshData2.vertices[i - num];
					array2[i] = shapeMeshData2.uv[i - num];
				}
			}
			int[] array3 = new int[shapeMeshData.triangles.Length + shapeMeshData2.triangles.Length];
			for (int j = 0; j < array3.Length; j++)
			{
				if (j < shapeMeshData.triangles.Length)
				{
					array3[j] = shapeMeshData.triangles[j];
				}
				else
				{
					array3[j] = shapeMeshData2.triangles[j - shapeMeshData.triangles.Length] + num;
				}
			}
			shapeMeshData.vertices = array;
			shapeMeshData.uv = array2;
			shapeMeshData.triangles = array3;
		}
	}
}
