using System;
using System.Collections.Generic;
using UnityEngine;

namespace EzySlice
{
	public sealed class Triangulator
	{
		internal struct Mapped2D
		{
			private readonly Vector3 original;

			private readonly Vector2 mapped;

			public Vector2 mappedValue => mapped;

			public Vector3 originalValue => original;

			public Mapped2D(Vector3 newOriginal, Vector3 u, Vector3 v)
			{
				original = newOriginal;
				mapped = new Vector2(Vector3.Dot(newOriginal, u), Vector3.Dot(newOriginal, v));
			}
		}

		public static bool MonotoneChain(List<Vector3> vertices, Vector3 normal, out List<Triangle> tri)
		{
			return MonotoneChain(vertices, normal, out tri, new TextureRegion(0f, 0f, 1f, 1f));
		}

		public static bool MonotoneChain(List<Vector3> vertices, Vector3 normal, out List<Triangle> tri, TextureRegion texRegion)
		{
			int count = vertices.Count;
			if (count < 3)
			{
				tri = null;
				return false;
			}
			Vector3 vector = Vector3.Normalize(Vector3.Cross(normal, Vector3.up));
			if (Vector3.zero == vector)
			{
				vector = Vector3.Normalize(Vector3.Cross(normal, Vector3.forward));
			}
			Vector3 v = Vector3.Cross(vector, normal);
			Mapped2D[] array = new Mapped2D[count];
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MaxValue;
			float num4 = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				Vector3 newOriginal = vertices[i];
				Mapped2D mapped2D = new Mapped2D(newOriginal, vector, v);
				Vector2 mappedValue = mapped2D.mappedValue;
				num = Mathf.Max(num, mappedValue.x);
				num2 = Mathf.Max(num2, mappedValue.y);
				num3 = Mathf.Min(num3, mappedValue.x);
				num4 = Mathf.Min(num4, mappedValue.y);
				array[i] = mapped2D;
			}
			Array.Sort(array, delegate(Mapped2D a, Mapped2D b)
			{
				Vector2 mappedValue11 = a.mappedValue;
				Vector2 mappedValue12 = b.mappedValue;
				return (!(mappedValue11.x < mappedValue12.x) && (mappedValue11.x != mappedValue12.x || !(mappedValue11.y < mappedValue12.y))) ? 1 : (-1);
			});
			Mapped2D[] array2 = new Mapped2D[count + 1];
			int num5 = 0;
			for (int num6 = 0; num6 < count; num6++)
			{
				while (num5 >= 2)
				{
					Vector2 mappedValue2 = array2[num5 - 2].mappedValue;
					Vector2 mappedValue3 = array2[num5 - 1].mappedValue;
					Vector2 mappedValue4 = array[num6].mappedValue;
					if (Intersector.TriArea2D(mappedValue2.x, mappedValue2.y, mappedValue3.x, mappedValue3.y, mappedValue4.x, mappedValue4.y) > 0f)
					{
						break;
					}
					num5--;
				}
				array2[num5++] = array[num6];
			}
			int num7 = count - 2;
			int num8 = num5 + 1;
			while (num7 >= 0)
			{
				while (num5 >= num8)
				{
					Vector2 mappedValue5 = array2[num5 - 2].mappedValue;
					Vector2 mappedValue6 = array2[num5 - 1].mappedValue;
					Vector2 mappedValue7 = array[num7].mappedValue;
					if (Intersector.TriArea2D(mappedValue5.x, mappedValue5.y, mappedValue6.x, mappedValue6.y, mappedValue7.x, mappedValue7.y) > 0f)
					{
						break;
					}
					num5--;
				}
				array2[num5++] = array[num7];
				num7--;
			}
			int num9 = num5 - 1;
			int num10 = (num9 - 2) * 3;
			if (num9 < 3)
			{
				tri = null;
				return false;
			}
			tri = new List<Triangle>(num10 / 3);
			float num11 = num - num3;
			float num12 = num2 - num4;
			int num13 = 1;
			for (int num14 = 0; num14 < num10; num14 += 3)
			{
				Mapped2D mapped2D2 = array2[0];
				Mapped2D mapped2D3 = array2[num13];
				Mapped2D mapped2D4 = array2[num13 + 1];
				Vector2 mappedValue8 = mapped2D2.mappedValue;
				Vector2 mappedValue9 = mapped2D3.mappedValue;
				Vector2 mappedValue10 = mapped2D4.mappedValue;
				mappedValue8.x = (mappedValue8.x - num3) / num11;
				mappedValue8.y = (mappedValue8.y - num4) / num12;
				mappedValue9.x = (mappedValue9.x - num3) / num11;
				mappedValue9.y = (mappedValue9.y - num4) / num12;
				mappedValue10.x = (mappedValue10.x - num3) / num11;
				mappedValue10.y = (mappedValue10.y - num4) / num12;
				Triangle item = new Triangle(mapped2D2.originalValue, mapped2D3.originalValue, mapped2D4.originalValue);
				item.SetUV(texRegion.Map(mappedValue8), texRegion.Map(mappedValue9), texRegion.Map(mappedValue10));
				item.SetNormal(normal, normal, normal);
				item.ComputeTangents();
				tri.Add(item);
				num13++;
			}
			return true;
		}
	}
}
