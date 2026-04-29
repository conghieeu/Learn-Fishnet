using UnityEngine;

namespace BuildingMakerToolset
{
	[ExecuteInEditMode]
	public class ProcedualMeshCache : MonoBehaviour
	{
		public MeshFilter meshFilter;

		public MeshCollider meshCollider;

		public Vector3[] vertices;

		public int[] triangles;

		public Vector2[] uv1;

		public Vector2[] uv2;

		public void SaveMesh(Mesh mesh, MeshFilter mf, MeshCollider mc)
		{
		}

		private void Start()
		{
			LoadMesh();
		}

		public void LoadMesh()
		{
			if (!(meshFilter == null) && !(meshFilter.sharedMesh != null))
			{
				Mesh mesh = new Mesh();
				mesh.SetVertices(vertices);
				mesh.SetUVs(0, uv1);
				mesh.SetUVs(1, uv2);
				mesh.SetTriangles(triangles, 0);
				mesh.RecalculateNormals();
				mesh.RecalculateTangents();
				meshFilter.sharedMesh = mesh;
				if (meshCollider != null)
				{
					meshCollider.sharedMesh = mesh;
				}
			}
		}
	}
}
