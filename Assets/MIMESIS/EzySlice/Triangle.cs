using UnityEngine;

namespace EzySlice
{
	public struct Triangle
	{
		private readonly Vector3 m_pos_a;

		private readonly Vector3 m_pos_b;

		private readonly Vector3 m_pos_c;

		private bool m_uv_set;

		private Vector2 m_uv_a;

		private Vector2 m_uv_b;

		private Vector2 m_uv_c;

		private bool m_nor_set;

		private Vector3 m_nor_a;

		private Vector3 m_nor_b;

		private Vector3 m_nor_c;

		private bool m_tan_set;

		private Vector4 m_tan_a;

		private Vector4 m_tan_b;

		private Vector4 m_tan_c;

		public Vector3 positionA => m_pos_a;

		public Vector3 positionB => m_pos_b;

		public Vector3 positionC => m_pos_c;

		public bool hasUV => m_uv_set;

		public Vector2 uvA => m_uv_a;

		public Vector2 uvB => m_uv_b;

		public Vector2 uvC => m_uv_c;

		public bool hasNormal => m_nor_set;

		public Vector3 normalA => m_nor_a;

		public Vector3 normalB => m_nor_b;

		public Vector3 normalC => m_nor_c;

		public bool hasTangent => m_tan_set;

		public Vector4 tangentA => m_tan_a;

		public Vector4 tangentB => m_tan_b;

		public Vector4 tangentC => m_tan_c;

		public Triangle(Vector3 posa, Vector3 posb, Vector3 posc)
		{
			m_pos_a = posa;
			m_pos_b = posb;
			m_pos_c = posc;
			m_uv_set = false;
			m_uv_a = Vector2.zero;
			m_uv_b = Vector2.zero;
			m_uv_c = Vector2.zero;
			m_nor_set = false;
			m_nor_a = Vector3.zero;
			m_nor_b = Vector3.zero;
			m_nor_c = Vector3.zero;
			m_tan_set = false;
			m_tan_a = Vector4.zero;
			m_tan_b = Vector4.zero;
			m_tan_c = Vector4.zero;
		}

		public void SetUV(Vector2 uvA, Vector2 uvB, Vector2 uvC)
		{
			m_uv_a = uvA;
			m_uv_b = uvB;
			m_uv_c = uvC;
			m_uv_set = true;
		}

		public void SetNormal(Vector3 norA, Vector3 norB, Vector3 norC)
		{
			m_nor_a = norA;
			m_nor_b = norB;
			m_nor_c = norC;
			m_nor_set = true;
		}

		public void SetTangent(Vector4 tanA, Vector4 tanB, Vector4 tanC)
		{
			m_tan_a = tanA;
			m_tan_b = tanB;
			m_tan_c = tanC;
			m_tan_set = true;
		}

		public void ComputeTangents()
		{
			if (m_nor_set && m_uv_set)
			{
				Vector3 pos_a = m_pos_a;
				Vector3 pos_b = m_pos_b;
				Vector3 pos_c = m_pos_c;
				Vector2 uv_a = m_uv_a;
				Vector2 uv_b = m_uv_b;
				Vector2 uv_c = m_uv_c;
				float num = pos_b.x - pos_a.x;
				float num2 = pos_c.x - pos_a.x;
				float num3 = pos_b.y - pos_a.y;
				float num4 = pos_c.y - pos_a.y;
				float num5 = pos_b.z - pos_a.z;
				float num6 = pos_c.z - pos_a.z;
				float num7 = uv_b.x - uv_a.x;
				float num8 = uv_c.x - uv_a.x;
				float num9 = uv_b.y - uv_a.y;
				float num10 = uv_c.y - uv_a.y;
				float num11 = 1f / (num7 * num10 - num8 * num9);
				Vector3 vector = new Vector3((num10 * num - num9 * num2) * num11, (num10 * num3 - num9 * num4) * num11, (num10 * num5 - num9 * num6) * num11);
				Vector3 rhs = new Vector3((num7 * num2 - num8 * num) * num11, (num7 * num4 - num8 * num3) * num11, (num7 * num6 - num8 * num5) * num11);
				Vector3 normal = m_nor_a;
				Vector3 tangent = vector;
				Vector3.OrthoNormalize(ref normal, ref tangent);
				Vector4 tanA = new Vector4(tangent.x, tangent.y, tangent.z, (Vector3.Dot(Vector3.Cross(normal, tangent), rhs) < 0f) ? (-1f) : 1f);
				Vector3 normal2 = m_nor_b;
				Vector3 tangent2 = vector;
				Vector3.OrthoNormalize(ref normal2, ref tangent2);
				Vector4 tanB = new Vector4(tangent2.x, tangent2.y, tangent2.z, (Vector3.Dot(Vector3.Cross(normal2, tangent2), rhs) < 0f) ? (-1f) : 1f);
				Vector3 normal3 = m_nor_c;
				Vector3 tangent3 = vector;
				Vector3.OrthoNormalize(ref normal3, ref tangent3);
				Vector4 tanC = new Vector4(tangent3.x, tangent3.y, tangent3.z, (Vector3.Dot(Vector3.Cross(normal3, tangent3), rhs) < 0f) ? (-1f) : 1f);
				SetTangent(tanA, tanB, tanC);
			}
		}

		public Vector3 Barycentric(Vector3 p)
		{
			Vector3 pos_a = m_pos_a;
			Vector3 pos_b = m_pos_b;
			Vector3 pos_c = m_pos_c;
			Vector3 vector = Vector3.Cross(pos_b - pos_a, pos_c - pos_a);
			float num = Mathf.Abs(vector.x);
			float num2 = Mathf.Abs(vector.y);
			float num3 = Mathf.Abs(vector.z);
			float num4;
			float num5;
			float num6;
			if (num >= num2 && num >= num3)
			{
				num4 = Intersector.TriArea2D(p.y, p.z, pos_b.y, pos_b.z, pos_c.y, pos_c.z);
				num5 = Intersector.TriArea2D(p.y, p.z, pos_c.y, pos_c.z, pos_a.y, pos_a.z);
				num6 = 1f / vector.x;
			}
			else if (num2 >= num && num2 >= num3)
			{
				num4 = Intersector.TriArea2D(p.x, p.z, pos_b.x, pos_b.z, pos_c.x, pos_c.z);
				num5 = Intersector.TriArea2D(p.x, p.z, pos_c.x, pos_c.z, pos_a.x, pos_a.z);
				num6 = 1f / (0f - vector.y);
			}
			else
			{
				num4 = Intersector.TriArea2D(p.x, p.y, pos_b.x, pos_b.y, pos_c.x, pos_c.y);
				num5 = Intersector.TriArea2D(p.x, p.y, pos_c.x, pos_c.y, pos_a.x, pos_a.y);
				num6 = 1f / vector.z;
			}
			float num7 = num4 * num6;
			float num8 = num5 * num6;
			float z = 1f - num7 - num8;
			return new Vector3(num7, num8, z);
		}

		public Vector2 GenerateUV(Vector3 pt)
		{
			if (!m_uv_set)
			{
				return Vector2.zero;
			}
			Vector3 vector = Barycentric(pt);
			return vector.x * m_uv_a + vector.y * m_uv_b + vector.z * m_uv_c;
		}

		public Vector3 GenerateNormal(Vector3 pt)
		{
			if (!m_nor_set)
			{
				return Vector3.zero;
			}
			Vector3 vector = Barycentric(pt);
			return vector.x * m_nor_a + vector.y * m_nor_b + vector.z * m_nor_c;
		}

		public Vector4 GenerateTangent(Vector3 pt)
		{
			if (!m_nor_set)
			{
				return Vector4.zero;
			}
			Vector3 vector = Barycentric(pt);
			return vector.x * m_tan_a + vector.y * m_tan_b + vector.z * m_tan_c;
		}

		public bool Split(Plane pl, IntersectionResult result)
		{
			Intersector.Intersect(pl, this, result);
			return result.isValid;
		}

		public bool IsCW()
		{
			return SignedSquare(m_pos_a, m_pos_b, m_pos_c) >= float.Epsilon;
		}

		public static float SignedSquare(Vector3 a, Vector3 b, Vector3 c)
		{
			return a.x * (b.y * c.z - b.z * c.y) - a.y * (b.x * c.z - b.z * c.x) + a.z * (b.x * c.y - b.y * c.x);
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
