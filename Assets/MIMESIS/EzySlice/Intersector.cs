using UnityEngine;

namespace EzySlice
{
	public sealed class Intersector
	{
		public const float Epsilon = 0.0001f;

		public static bool Intersect(Plane pl, Line ln, out Vector3 q)
		{
			return Intersect(pl, ln.positionA, ln.positionB, out q);
		}

		public static bool Intersect(Plane pl, Vector3 a, Vector3 b, out Vector3 q)
		{
			Vector3 normal = pl.normal;
			Vector3 vector = b - a;
			float num = (pl.dist - Vector3.Dot(normal, a)) / Vector3.Dot(normal, vector);
			if (num >= -0.0001f && num <= 1.0001f)
			{
				q = a + num * vector;
				return true;
			}
			q = Vector3.zero;
			return false;
		}

		public static float TriArea2D(float x1, float y1, float x2, float y2, float x3, float y3)
		{
			return (x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2);
		}

		public static void Intersect(Plane pl, Triangle tri, IntersectionResult result)
		{
			result.Clear();
			Vector3 positionA = tri.positionA;
			Vector3 positionB = tri.positionB;
			Vector3 positionC = tri.positionC;
			SideOfPlane sideOfPlane = pl.SideOf(positionA);
			SideOfPlane sideOfPlane2 = pl.SideOf(positionB);
			SideOfPlane sideOfPlane3 = pl.SideOf(positionC);
			if ((sideOfPlane == sideOfPlane2 && sideOfPlane2 == sideOfPlane3) || (sideOfPlane == SideOfPlane.ON && sideOfPlane == sideOfPlane2) || (sideOfPlane == SideOfPlane.ON && sideOfPlane == sideOfPlane3) || (sideOfPlane2 == SideOfPlane.ON && sideOfPlane2 == sideOfPlane3) || (sideOfPlane == SideOfPlane.ON && sideOfPlane2 != SideOfPlane.ON && sideOfPlane2 == sideOfPlane3) || (sideOfPlane2 == SideOfPlane.ON && sideOfPlane != SideOfPlane.ON && sideOfPlane == sideOfPlane3) || (sideOfPlane3 == SideOfPlane.ON && sideOfPlane != SideOfPlane.ON && sideOfPlane == sideOfPlane2))
			{
				return;
			}
			Vector3 q;
			Vector3 q2;
			if (sideOfPlane == SideOfPlane.ON)
			{
				if (Intersect(pl, positionB, positionC, out q))
				{
					result.AddIntersectionPoint(q);
					result.AddIntersectionPoint(positionA);
					Triangle tri2 = new Triangle(positionA, positionB, q);
					Triangle tri3 = new Triangle(positionA, q, positionC);
					if (tri.hasUV)
					{
						Vector2 vector = tri.GenerateUV(q);
						Vector2 uvA = tri.uvA;
						Vector2 uvB = tri.uvB;
						Vector2 uvC = tri.uvC;
						tri2.SetUV(uvA, uvB, vector);
						tri3.SetUV(uvA, vector, uvC);
					}
					if (tri.hasNormal)
					{
						Vector3 vector2 = tri.GenerateNormal(q);
						Vector3 normalA = tri.normalA;
						Vector3 normalB = tri.normalB;
						Vector3 normalC = tri.normalC;
						tri2.SetNormal(normalA, normalB, vector2);
						tri3.SetNormal(normalA, vector2, normalC);
					}
					if (tri.hasTangent)
					{
						Vector4 vector3 = tri.GenerateTangent(q);
						Vector4 tangentA = tri.tangentA;
						Vector4 tangentB = tri.tangentB;
						Vector4 tangentC = tri.tangentC;
						tri2.SetTangent(tangentA, tangentB, vector3);
						tri3.SetTangent(tangentA, vector3, tangentC);
					}
					switch (sideOfPlane2)
					{
					case SideOfPlane.UP:
						result.AddUpperHull(tri2).AddLowerHull(tri3);
						break;
					case SideOfPlane.DOWN:
						result.AddUpperHull(tri3).AddLowerHull(tri2);
						break;
					}
				}
			}
			else if (sideOfPlane2 == SideOfPlane.ON)
			{
				if (Intersect(pl, positionA, positionC, out q))
				{
					result.AddIntersectionPoint(q);
					result.AddIntersectionPoint(positionB);
					Triangle tri4 = new Triangle(positionA, positionB, q);
					Triangle tri5 = new Triangle(q, positionB, positionC);
					if (tri.hasUV)
					{
						Vector2 vector4 = tri.GenerateUV(q);
						Vector2 uvA2 = tri.uvA;
						Vector2 uvB2 = tri.uvB;
						Vector2 uvC2 = tri.uvC;
						tri4.SetUV(uvA2, uvB2, vector4);
						tri5.SetUV(vector4, uvB2, uvC2);
					}
					if (tri.hasNormal)
					{
						Vector3 vector5 = tri.GenerateNormal(q);
						Vector3 normalA2 = tri.normalA;
						Vector3 normalB2 = tri.normalB;
						Vector3 normalC2 = tri.normalC;
						tri4.SetNormal(normalA2, normalB2, vector5);
						tri5.SetNormal(vector5, normalB2, normalC2);
					}
					if (tri.hasTangent)
					{
						Vector4 vector6 = tri.GenerateTangent(q);
						Vector4 tangentA2 = tri.tangentA;
						Vector4 tangentB2 = tri.tangentB;
						Vector4 tangentC2 = tri.tangentC;
						tri4.SetTangent(tangentA2, tangentB2, vector6);
						tri5.SetTangent(vector6, tangentB2, tangentC2);
					}
					switch (sideOfPlane)
					{
					case SideOfPlane.UP:
						result.AddUpperHull(tri4).AddLowerHull(tri5);
						break;
					case SideOfPlane.DOWN:
						result.AddUpperHull(tri5).AddLowerHull(tri4);
						break;
					}
				}
			}
			else if (sideOfPlane3 == SideOfPlane.ON)
			{
				if (Intersect(pl, positionA, positionB, out q))
				{
					result.AddIntersectionPoint(q);
					result.AddIntersectionPoint(positionC);
					Triangle tri6 = new Triangle(positionA, q, positionC);
					Triangle tri7 = new Triangle(q, positionB, positionC);
					if (tri.hasUV)
					{
						Vector2 vector7 = tri.GenerateUV(q);
						Vector2 uvA3 = tri.uvA;
						Vector2 uvB3 = tri.uvB;
						Vector2 uvC3 = tri.uvC;
						tri6.SetUV(uvA3, vector7, uvC3);
						tri7.SetUV(vector7, uvB3, uvC3);
					}
					if (tri.hasNormal)
					{
						Vector3 vector8 = tri.GenerateNormal(q);
						Vector3 normalA3 = tri.normalA;
						Vector3 normalB3 = tri.normalB;
						Vector3 normalC3 = tri.normalC;
						tri6.SetNormal(normalA3, vector8, normalC3);
						tri7.SetNormal(vector8, normalB3, normalC3);
					}
					if (tri.hasTangent)
					{
						Vector4 vector9 = tri.GenerateTangent(q);
						Vector4 tangentA3 = tri.tangentA;
						Vector4 tangentB3 = tri.tangentB;
						Vector4 tangentC3 = tri.tangentC;
						tri6.SetTangent(tangentA3, vector9, tangentC3);
						tri7.SetTangent(vector9, tangentB3, tangentC3);
					}
					switch (sideOfPlane)
					{
					case SideOfPlane.UP:
						result.AddUpperHull(tri6).AddLowerHull(tri7);
						break;
					case SideOfPlane.DOWN:
						result.AddUpperHull(tri7).AddLowerHull(tri6);
						break;
					}
				}
			}
			else if (sideOfPlane != sideOfPlane2 && Intersect(pl, positionA, positionB, out q))
			{
				result.AddIntersectionPoint(q);
				if (sideOfPlane == sideOfPlane3)
				{
					if (Intersect(pl, positionB, positionC, out q2))
					{
						result.AddIntersectionPoint(q2);
						Triangle tri8 = new Triangle(q, positionB, q2);
						Triangle tri9 = new Triangle(positionA, q, q2);
						Triangle tri10 = new Triangle(positionA, q2, positionC);
						if (tri.hasUV)
						{
							Vector2 vector10 = tri.GenerateUV(q);
							Vector2 vector11 = tri.GenerateUV(q2);
							Vector2 uvA4 = tri.uvA;
							Vector2 uvB4 = tri.uvB;
							Vector2 uvC4 = tri.uvC;
							tri8.SetUV(vector10, uvB4, vector11);
							tri9.SetUV(uvA4, vector10, vector11);
							tri10.SetUV(uvA4, vector11, uvC4);
						}
						if (tri.hasNormal)
						{
							Vector3 vector12 = tri.GenerateNormal(q);
							Vector3 vector13 = tri.GenerateNormal(q2);
							Vector3 normalA4 = tri.normalA;
							Vector3 normalB4 = tri.normalB;
							Vector3 normalC4 = tri.normalC;
							tri8.SetNormal(vector12, normalB4, vector13);
							tri9.SetNormal(normalA4, vector12, vector13);
							tri10.SetNormal(normalA4, vector13, normalC4);
						}
						if (tri.hasTangent)
						{
							Vector4 vector14 = tri.GenerateTangent(q);
							Vector4 vector15 = tri.GenerateTangent(q2);
							Vector4 tangentA4 = tri.tangentA;
							Vector4 tangentB4 = tri.tangentB;
							Vector4 tangentC4 = tri.tangentC;
							tri8.SetTangent(vector14, tangentB4, vector15);
							tri9.SetTangent(tangentA4, vector14, vector15);
							tri10.SetTangent(tangentA4, vector15, tangentC4);
						}
						if (sideOfPlane == SideOfPlane.UP)
						{
							result.AddUpperHull(tri9).AddUpperHull(tri10).AddLowerHull(tri8);
						}
						else
						{
							result.AddLowerHull(tri9).AddLowerHull(tri10).AddUpperHull(tri8);
						}
					}
				}
				else if (Intersect(pl, positionA, positionC, out q2))
				{
					result.AddIntersectionPoint(q2);
					Triangle tri11 = new Triangle(positionA, q, q2);
					Triangle tri12 = new Triangle(q, positionB, positionC);
					Triangle tri13 = new Triangle(q2, q, positionC);
					if (tri.hasUV)
					{
						Vector2 vector16 = tri.GenerateUV(q);
						Vector2 vector17 = tri.GenerateUV(q2);
						Vector2 uvA5 = tri.uvA;
						Vector2 uvB5 = tri.uvB;
						Vector2 uvC5 = tri.uvC;
						tri11.SetUV(uvA5, vector16, vector17);
						tri12.SetUV(vector16, uvB5, uvC5);
						tri13.SetUV(vector17, vector16, uvC5);
					}
					if (tri.hasNormal)
					{
						Vector3 vector18 = tri.GenerateNormal(q);
						Vector3 vector19 = tri.GenerateNormal(q2);
						Vector3 normalA5 = tri.normalA;
						Vector3 normalB5 = tri.normalB;
						Vector3 normalC5 = tri.normalC;
						tri11.SetNormal(normalA5, vector18, vector19);
						tri12.SetNormal(vector18, normalB5, normalC5);
						tri13.SetNormal(vector19, vector18, normalC5);
					}
					if (tri.hasTangent)
					{
						Vector4 vector20 = tri.GenerateTangent(q);
						Vector4 vector21 = tri.GenerateTangent(q2);
						Vector4 tangentA5 = tri.tangentA;
						Vector4 tangentB5 = tri.tangentB;
						Vector4 tangentC5 = tri.tangentC;
						tri11.SetTangent(tangentA5, vector20, vector21);
						tri12.SetTangent(vector20, tangentB5, tangentC5);
						tri13.SetTangent(vector21, vector20, tangentC5);
					}
					if (sideOfPlane == SideOfPlane.UP)
					{
						result.AddUpperHull(tri11).AddLowerHull(tri12).AddLowerHull(tri13);
					}
					else
					{
						result.AddLowerHull(tri11).AddUpperHull(tri12).AddUpperHull(tri13);
					}
				}
			}
			else if (Intersect(pl, positionC, positionA, out q) && Intersect(pl, positionC, positionB, out q2))
			{
				result.AddIntersectionPoint(q);
				result.AddIntersectionPoint(q2);
				Triangle tri14 = new Triangle(q, q2, positionC);
				Triangle tri15 = new Triangle(positionA, q2, q);
				Triangle tri16 = new Triangle(positionA, positionB, q2);
				if (tri.hasUV)
				{
					Vector2 vector22 = tri.GenerateUV(q);
					Vector2 vector23 = tri.GenerateUV(q2);
					Vector2 uvA6 = tri.uvA;
					Vector2 uvB6 = tri.uvB;
					Vector2 uvC6 = tri.uvC;
					tri14.SetUV(vector22, vector23, uvC6);
					tri15.SetUV(uvA6, vector23, vector22);
					tri16.SetUV(uvA6, uvB6, vector23);
				}
				if (tri.hasNormal)
				{
					Vector3 vector24 = tri.GenerateNormal(q);
					Vector3 vector25 = tri.GenerateNormal(q2);
					Vector3 normalA6 = tri.normalA;
					Vector3 normalB6 = tri.normalB;
					Vector3 normalC6 = tri.normalC;
					tri14.SetNormal(vector24, vector25, normalC6);
					tri15.SetNormal(normalA6, vector25, vector24);
					tri16.SetNormal(normalA6, normalB6, vector25);
				}
				if (tri.hasTangent)
				{
					Vector4 vector26 = tri.GenerateTangent(q);
					Vector4 vector27 = tri.GenerateTangent(q2);
					Vector4 tangentA6 = tri.tangentA;
					Vector4 tangentB6 = tri.tangentB;
					Vector4 tangentC6 = tri.tangentC;
					tri14.SetTangent(vector26, vector27, tangentC6);
					tri15.SetTangent(tangentA6, vector27, vector26);
					tri16.SetTangent(tangentA6, tangentB6, vector27);
				}
				if (sideOfPlane == SideOfPlane.UP)
				{
					result.AddUpperHull(tri15).AddUpperHull(tri16).AddLowerHull(tri14);
				}
				else
				{
					result.AddLowerHull(tri15).AddLowerHull(tri16).AddUpperHull(tri14);
				}
			}
		}
	}
}
