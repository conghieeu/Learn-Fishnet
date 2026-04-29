using System.Collections.Generic;
using UnityEngine;

namespace BuildingMakerToolset.Geometry
{
	public class Triangulator
	{
		public struct HoleData
		{
			public readonly int holeIndex;

			public readonly int bridgeIndex;

			public readonly Vector2 bridgePoint;

			public HoleData(int holeIndex, int bridgeIndex, Vector2 bridgePoint)
			{
				this.holeIndex = holeIndex;
				this.bridgeIndex = bridgeIndex;
				this.bridgePoint = bridgePoint;
			}
		}

		public class Vertex
		{
			public readonly Vector2 position;

			public readonly int index;

			public bool isConvex;

			public Vertex(Vector2 position, int index, bool isConvex)
			{
				this.position = position;
				this.index = index;
				this.isConvex = isConvex;
			}
		}

		private LinkedList<Vertex> vertsInClippedPolygon;

		private int[] tris;

		private int triIndex;

		public Triangulator(Polygon polygon)
		{
			int num = 2 * polygon.numHoles;
			int num2 = polygon.numPoints + num;
			tris = new int[(num2 - 2) * 3];
			vertsInClippedPolygon = GenerateVertexList(polygon);
		}

		public int[] Triangulate()
		{
			while (vertsInClippedPolygon.Count >= 3)
			{
				bool flag = false;
				LinkedListNode<Vertex> linkedListNode = vertsInClippedPolygon.First;
				for (int i = 0; i < vertsInClippedPolygon.Count; i++)
				{
					LinkedListNode<Vertex> linkedListNode2 = linkedListNode.Previous ?? vertsInClippedPolygon.Last;
					LinkedListNode<Vertex> linkedListNode3 = linkedListNode.Next ?? vertsInClippedPolygon.First;
					if (linkedListNode.Value.isConvex && !TriangleContainsVertex(linkedListNode2.Value, linkedListNode.Value, linkedListNode3.Value))
					{
						if (!linkedListNode2.Value.isConvex)
						{
							LinkedListNode<Vertex> linkedListNode4 = linkedListNode2.Previous ?? vertsInClippedPolygon.Last;
							linkedListNode2.Value.isConvex = IsConvex(linkedListNode4.Value.position, linkedListNode2.Value.position, linkedListNode3.Value.position);
						}
						if (!linkedListNode3.Value.isConvex)
						{
							LinkedListNode<Vertex> linkedListNode5 = linkedListNode3.Next ?? vertsInClippedPolygon.First;
							linkedListNode3.Value.isConvex = IsConvex(linkedListNode2.Value.position, linkedListNode3.Value.position, linkedListNode5.Value.position);
						}
						tris[triIndex * 3 + 2] = linkedListNode2.Value.index;
						tris[triIndex * 3 + 1] = linkedListNode.Value.index;
						tris[triIndex * 3] = linkedListNode3.Value.index;
						triIndex++;
						flag = true;
						vertsInClippedPolygon.Remove(linkedListNode);
						break;
					}
					linkedListNode = linkedListNode3;
				}
				if (!flag)
				{
					return null;
				}
			}
			return tris;
		}

		private LinkedList<Vertex> GenerateVertexList(Polygon polygon)
		{
			LinkedList<Vertex> linkedList = new LinkedList<Vertex>();
			LinkedListNode<Vertex> linkedListNode = null;
			for (int i = 0; i < polygon.numHullPoints; i++)
			{
				int num = (i - 1 + polygon.numHullPoints) % polygon.numHullPoints;
				int num2 = (i + 1) % polygon.numHullPoints;
				bool isConvex = IsConvex(polygon.points[num], polygon.points[i], polygon.points[num2]);
				Vertex value = new Vertex(polygon.points[i], i, isConvex);
				linkedListNode = ((linkedListNode != null) ? linkedList.AddAfter(linkedListNode, value) : linkedList.AddFirst(value));
			}
			List<HoleData> list = new List<HoleData>();
			for (int j = 0; j < polygon.numHoles; j++)
			{
				Vector2 bridgePoint = new Vector2(float.MinValue, 0f);
				int bridgeIndex = 0;
				for (int k = 0; k < polygon.numPointsPerHole[j]; k++)
				{
					if (polygon.GetHolePoint(k, j).x > bridgePoint.x)
					{
						bridgePoint = polygon.GetHolePoint(k, j);
						bridgeIndex = k;
					}
				}
				list.Add(new HoleData(j, bridgeIndex, bridgePoint));
			}
			list.Sort((HoleData x, HoleData holeData) => (!(x.bridgePoint.x > holeData.bridgePoint.x)) ? 1 : (-1));
			foreach (HoleData item in list)
			{
				Vector2 b = new Vector2(float.MaxValue, item.bridgePoint.y);
				List<LinkedListNode<Vertex>> list2 = new List<LinkedListNode<Vertex>>();
				LinkedListNode<Vertex> linkedListNode2 = null;
				for (linkedListNode = linkedList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					LinkedListNode<Vertex> linkedListNode3 = ((linkedListNode.Next == null) ? linkedList.First : linkedListNode.Next);
					Vector2 position = linkedListNode.Value.position;
					Vector2 position2 = linkedListNode3.Value.position;
					if ((position.x > item.bridgePoint.x || position2.x > item.bridgePoint.x) && position.y > item.bridgePoint.y != position2.y > item.bridgePoint.y)
					{
						float num3 = position2.x;
						if (!Mathf.Approximately(position.x, position2.x))
						{
							float y = item.bridgePoint.y;
							float num4 = (position.y - position2.y) / (position.x - position2.x);
							float num5 = position2.y - num4 * position2.x;
							num3 = (y - num5) / num4;
						}
						if (num3 > item.bridgePoint.x)
						{
							LinkedListNode<Vertex> linkedListNode4 = ((position.x > position2.x) ? linkedListNode : linkedListNode3);
							bool flag = Mathf.Approximately(num3, b.x);
							bool flag2 = false;
							if (linkedListNode4.Previous != null)
							{
								flag2 = item.bridgePoint.y > linkedListNode4.Previous.Value.position.y;
							}
							if ((!flag || flag2) && (num3 < b.x || flag))
							{
								b.x = num3;
								linkedListNode2 = linkedListNode4;
							}
						}
					}
					if (linkedListNode != linkedListNode2 && !linkedListNode.Value.isConvex && position.x > item.bridgePoint.x)
					{
						list2.Add(linkedListNode);
					}
				}
				LinkedListNode<Vertex> linkedListNode5 = linkedListNode2;
				foreach (LinkedListNode<Vertex> item2 in list2)
				{
					if (item2.Value.index != linkedListNode2.Value.index && Maths2D.PointInTriangle(item.bridgePoint, b, linkedListNode2.Value.position, item2.Value.position))
					{
						bool flag3 = linkedListNode5.Value.position == item2.Value.position;
						float num6 = Mathf.Abs(item.bridgePoint.y - linkedListNode5.Value.position.y);
						if (Mathf.Abs(item.bridgePoint.y - item2.Value.position.y) < num6 || flag3)
						{
							linkedListNode5 = item2;
						}
					}
				}
				linkedListNode = linkedListNode5;
				if (linkedListNode == null)
				{
					continue;
				}
				for (int num7 = item.bridgeIndex; num7 <= polygon.numPointsPerHole[item.holeIndex] + item.bridgeIndex; num7++)
				{
					int index = linkedListNode.Value.index;
					int num8 = polygon.IndexOfPointInHole(num7 % polygon.numPointsPerHole[item.holeIndex], item.holeIndex);
					int num9 = polygon.IndexOfPointInHole((num7 + 1) % polygon.numPointsPerHole[item.holeIndex], item.holeIndex);
					if (num7 == polygon.numPointsPerHole[item.holeIndex] + item.bridgeIndex)
					{
						num9 = linkedListNode5.Value.index;
					}
					bool isConvex2 = IsConvex(polygon.points[index], polygon.points[num8], polygon.points[num9]);
					Vertex value2 = new Vertex(polygon.points[num8], num8, isConvex2);
					linkedListNode = linkedList.AddAfter(linkedListNode, value2);
				}
				Vector2 v = ((linkedListNode.Next == null) ? linkedList.First.Value.position : linkedListNode.Next.Value.position);
				bool isConvex3 = IsConvex(item.bridgePoint, linkedListNode5.Value.position, v);
				Vertex value3 = new Vertex(linkedListNode5.Value.position, linkedListNode5.Value.index, isConvex3);
				linkedList.AddAfter(linkedListNode, value3);
				LinkedListNode<Vertex> linkedListNode6 = ((linkedListNode5.Previous == null) ? linkedList.Last : linkedListNode5.Previous);
				LinkedListNode<Vertex> linkedListNode7 = ((linkedListNode5.Next == null) ? linkedList.First : linkedListNode5.Next);
				linkedListNode5.Value.isConvex = IsConvex(linkedListNode6.Value.position, linkedListNode5.Value.position, linkedListNode7.Value.position);
			}
			return linkedList;
		}

		private bool TriangleContainsVertex(Vertex v0, Vertex v1, Vertex v2)
		{
			LinkedListNode<Vertex> linkedListNode = vertsInClippedPolygon.First;
			for (int i = 0; i < vertsInClippedPolygon.Count; i++)
			{
				if (!linkedListNode.Value.isConvex)
				{
					Vertex value = linkedListNode.Value;
					if (value.index != v0.index && value.index != v1.index && value.index != v2.index && Maths2D.PointInTriangle(v0.position, v1.position, v2.position, value.position))
					{
						return true;
					}
				}
				linkedListNode = linkedListNode.Next;
			}
			return false;
		}

		private bool IsConvex(Vector2 v0, Vector2 v1, Vector2 v2)
		{
			return Maths2D.SideOfLine(v0, v2, v1) == -1;
		}
	}
}
