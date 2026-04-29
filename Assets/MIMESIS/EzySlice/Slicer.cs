using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace EzySlice
{
	public sealed class Slicer
	{
		internal class SlicedSubmesh
		{
			public readonly List<Triangle> upperHull = new List<Triangle>();

			public readonly List<Triangle> lowerHull = new List<Triangle>();

			public bool hasUV
			{
				get
				{
					if (upperHull.Count <= 0)
					{
						if (lowerHull.Count > 0)
						{
							return lowerHull[0].hasUV;
						}
						return false;
					}
					return upperHull[0].hasUV;
				}
			}

			public bool hasNormal
			{
				get
				{
					if (upperHull.Count <= 0)
					{
						if (lowerHull.Count > 0)
						{
							return lowerHull[0].hasNormal;
						}
						return false;
					}
					return upperHull[0].hasNormal;
				}
			}

			public bool hasTangent
			{
				get
				{
					if (upperHull.Count <= 0)
					{
						if (lowerHull.Count > 0)
						{
							return lowerHull[0].hasTangent;
						}
						return false;
					}
					return upperHull[0].hasTangent;
				}
			}

			public bool isValid
			{
				get
				{
					if (upperHull.Count > 0)
					{
						return lowerHull.Count > 0;
					}
					return false;
				}
			}
		}

		public static SlicedHull Slice(GameObject obj, Plane pl, TextureRegion crossRegion, Material crossMaterial)
		{
			if (!obj.TryGetComponent<MeshFilter>(out var component))
			{
				Debug.LogWarning("EzySlice::Slice -> Provided GameObject must have a MeshFilter Component.");
				return null;
			}
			if (!obj.TryGetComponent<MeshRenderer>(out var component2))
			{
				Debug.LogWarning("EzySlice::Slice -> Provided GameObject must have a MeshRenderer Component.");
				return null;
			}
			Material[] sharedMaterials = component2.sharedMaterials;
			Mesh sharedMesh = component.sharedMesh;
			if (sharedMesh == null)
			{
				Debug.LogWarning("EzySlice::Slice -> Provided GameObject must have a Mesh that is not NULL.");
				return null;
			}
			int subMeshCount = sharedMesh.subMeshCount;
			if (sharedMaterials.Length != subMeshCount)
			{
				Debug.LogWarning("EzySlice::Slice -> Provided Material array must match the length of submeshes.");
				return null;
			}
			int num = sharedMaterials.Length;
			if (crossMaterial != null)
			{
				for (int i = 0; i < num; i++)
				{
					if (sharedMaterials[i] == crossMaterial)
					{
						num = i;
						break;
					}
				}
			}
			return Slice(sharedMesh, pl, crossRegion, num);
		}

		public static SlicedHull Slice(Mesh sharedMesh, Plane pl, TextureRegion region, int crossIndex)
		{
			if (sharedMesh == null)
			{
				return null;
			}
			Vector3[] vertices = sharedMesh.vertices;
			Vector2[] uv = sharedMesh.uv;
			Vector3[] normals = sharedMesh.normals;
			Vector4[] tangents = sharedMesh.tangents;
			int subMeshCount = sharedMesh.subMeshCount;
			SlicedSubmesh[] array = new SlicedSubmesh[subMeshCount];
			List<Vector3> list = new List<Vector3>();
			IntersectionResult intersectionResult = new IntersectionResult();
			bool flag = vertices.Length == uv.Length;
			bool flag2 = vertices.Length == normals.Length;
			bool flag3 = vertices.Length == tangents.Length;
			for (int i = 0; i < subMeshCount; i++)
			{
				int[] triangles = sharedMesh.GetTriangles(i);
				int num = triangles.Length;
				SlicedSubmesh slicedSubmesh = new SlicedSubmesh();
				for (int j = 0; j < num; j += 3)
				{
					int num2 = triangles[j];
					int num3 = triangles[j + 1];
					int num4 = triangles[j + 2];
					Triangle item = new Triangle(vertices[num2], vertices[num3], vertices[num4]);
					if (flag)
					{
						item.SetUV(uv[num2], uv[num3], uv[num4]);
					}
					if (flag2)
					{
						item.SetNormal(normals[num2], normals[num3], normals[num4]);
					}
					if (flag3)
					{
						item.SetTangent(tangents[num2], tangents[num3], tangents[num4]);
					}
					if (item.Split(pl, intersectionResult))
					{
						int upperHullCount = intersectionResult.upperHullCount;
						int lowerHullCount = intersectionResult.lowerHullCount;
						int intersectionPointCount = intersectionResult.intersectionPointCount;
						for (int k = 0; k < upperHullCount; k++)
						{
							slicedSubmesh.upperHull.Add(intersectionResult.upperHull[k]);
						}
						for (int l = 0; l < lowerHullCount; l++)
						{
							slicedSubmesh.lowerHull.Add(intersectionResult.lowerHull[l]);
						}
						for (int m = 0; m < intersectionPointCount; m++)
						{
							list.Add(intersectionResult.intersectionPoints[m]);
						}
						continue;
					}
					SideOfPlane sideOfPlane = pl.SideOf(vertices[num2]);
					SideOfPlane sideOfPlane2 = pl.SideOf(vertices[num3]);
					SideOfPlane sideOfPlane3 = pl.SideOf(vertices[num4]);
					SideOfPlane sideOfPlane4 = SideOfPlane.ON;
					if (sideOfPlane != SideOfPlane.ON)
					{
						sideOfPlane4 = sideOfPlane;
					}
					if (sideOfPlane2 != SideOfPlane.ON)
					{
						sideOfPlane4 = sideOfPlane2;
					}
					if (sideOfPlane3 != SideOfPlane.ON)
					{
						sideOfPlane4 = sideOfPlane3;
					}
					if (sideOfPlane4 == SideOfPlane.UP || sideOfPlane4 == SideOfPlane.ON)
					{
						slicedSubmesh.upperHull.Add(item);
					}
					else
					{
						slicedSubmesh.lowerHull.Add(item);
					}
				}
				array[i] = slicedSubmesh;
			}
			for (int n = 0; n < array.Length; n++)
			{
				if (array[n] != null && array[n].isValid)
				{
					return CreateFrom(array, CreateFrom(list, pl.normal, region), crossIndex);
				}
			}
			return null;
		}

		private static SlicedHull CreateFrom(SlicedSubmesh[] meshes, List<Triangle> cross, int crossSectionIndex)
		{
			int num = meshes.Length;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 += meshes[i].upperHull.Count;
				num3 += meshes[i].lowerHull.Count;
			}
			Mesh upperHull = CreateUpperHull(meshes, num2, cross, crossSectionIndex);
			Mesh lowerHull = CreateLowerHull(meshes, num3, cross, crossSectionIndex);
			return new SlicedHull(upperHull, lowerHull);
		}

		private static Mesh CreateUpperHull(SlicedSubmesh[] mesh, int total, List<Triangle> crossSection, int crossSectionIndex)
		{
			return CreateHull(mesh, total, crossSection, crossSectionIndex, isUpper: true);
		}

		private static Mesh CreateLowerHull(SlicedSubmesh[] mesh, int total, List<Triangle> crossSection, int crossSectionIndex)
		{
			return CreateHull(mesh, total, crossSection, crossSectionIndex, isUpper: false);
		}

		private static Mesh CreateHull(SlicedSubmesh[] meshes, int total, List<Triangle> crossSection, int crossIndex, bool isUpper)
		{
			if (total <= 0)
			{
				return null;
			}
			int num = meshes.Length;
			int num2 = crossSection?.Count ?? 0;
			Mesh mesh = new Mesh();
			mesh.indexFormat = IndexFormat.UInt32;
			int num3 = (total + num2) * 3;
			bool hasUV = meshes[0].hasUV;
			bool hasNormal = meshes[0].hasNormal;
			bool hasTangent = meshes[0].hasTangent;
			Vector3[] array = new Vector3[num3];
			Vector2[] array2 = (hasUV ? new Vector2[num3] : null);
			Vector3[] array3 = (hasNormal ? new Vector3[num3] : null);
			Vector4[] array4 = (hasTangent ? new Vector4[num3] : null);
			List<int[]> list = new List<int[]>(num);
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				List<Triangle> list2 = (isUpper ? meshes[i].upperHull : meshes[i].lowerHull);
				int count = list2.Count;
				int[] array5 = new int[count * 3];
				int num5 = 0;
				int num6 = 0;
				while (num5 < count)
				{
					Triangle triangle = list2[num5];
					int num7 = num4;
					int num8 = num4 + 1;
					int num9 = num4 + 2;
					array[num7] = triangle.positionA;
					array[num8] = triangle.positionB;
					array[num9] = triangle.positionC;
					if (hasUV)
					{
						array2[num7] = triangle.uvA;
						array2[num8] = triangle.uvB;
						array2[num9] = triangle.uvC;
					}
					if (hasNormal)
					{
						array3[num7] = triangle.normalA;
						array3[num8] = triangle.normalB;
						array3[num9] = triangle.normalC;
					}
					if (hasTangent)
					{
						array4[num7] = triangle.tangentA;
						array4[num8] = triangle.tangentB;
						array4[num9] = triangle.tangentC;
					}
					array5[num6] = num7;
					array5[num6 + 1] = num8;
					array5[num6 + 2] = num9;
					num4 += 3;
					num5++;
					num6 += 3;
				}
				list.Add(array5);
			}
			if (crossSection != null && num2 > 0)
			{
				int[] array6 = new int[num2 * 3];
				int num10 = 0;
				int num11 = 0;
				while (num10 < num2)
				{
					Triangle triangle2 = crossSection[num10];
					int num12 = num4;
					int num13 = num4 + 1;
					int num14 = num4 + 2;
					array[num12] = triangle2.positionA;
					array[num13] = triangle2.positionB;
					array[num14] = triangle2.positionC;
					if (hasUV)
					{
						array2[num12] = triangle2.uvA;
						array2[num13] = triangle2.uvB;
						array2[num14] = triangle2.uvC;
					}
					if (hasNormal)
					{
						if (isUpper)
						{
							array3[num12] = -triangle2.normalA;
							array3[num13] = -triangle2.normalB;
							array3[num14] = -triangle2.normalC;
						}
						else
						{
							array3[num12] = triangle2.normalA;
							array3[num13] = triangle2.normalB;
							array3[num14] = triangle2.normalC;
						}
					}
					if (hasTangent)
					{
						array4[num12] = triangle2.tangentA;
						array4[num13] = triangle2.tangentB;
						array4[num14] = triangle2.tangentC;
					}
					if (isUpper)
					{
						array6[num11] = num12;
						array6[num11 + 1] = num13;
						array6[num11 + 2] = num14;
					}
					else
					{
						array6[num11] = num12;
						array6[num11 + 1] = num14;
						array6[num11 + 2] = num13;
					}
					num4 += 3;
					num10++;
					num11 += 3;
				}
				if (list.Count <= crossIndex)
				{
					list.Add(array6);
				}
				else
				{
					int[] array7 = list[crossIndex];
					int[] array8 = new int[array7.Length + array6.Length];
					Array.Copy(array7, array8, array7.Length);
					Array.Copy(array6, 0, array8, array7.Length, array6.Length);
					list[crossIndex] = array8;
				}
			}
			int num15 = (mesh.subMeshCount = list.Count);
			mesh.vertices = array;
			if (hasUV)
			{
				mesh.uv = array2;
			}
			if (hasNormal)
			{
				mesh.normals = array3;
			}
			if (hasTangent)
			{
				mesh.tangents = array4;
			}
			for (int j = 0; j < num15; j++)
			{
				mesh.SetTriangles(list[j], j, calculateBounds: false);
			}
			return mesh;
		}

		private static List<Triangle> CreateFrom(List<Vector3> intPoints, Vector3 planeNormal, TextureRegion region)
		{
			if (!Triangulator.MonotoneChain(intPoints, planeNormal, out var tri, region))
			{
				return null;
			}
			return tri;
		}
	}
}
