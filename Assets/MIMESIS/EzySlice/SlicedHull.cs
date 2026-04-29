using System;
using UnityEngine;

namespace EzySlice
{
	public sealed class SlicedHull
	{
		private Mesh upper_hull;

		private Mesh lower_hull;

		public Mesh upperHull => upper_hull;

		public Mesh lowerHull => lower_hull;

		public SlicedHull(Mesh upperHull, Mesh lowerHull)
		{
			upper_hull = upperHull;
			lower_hull = lowerHull;
		}

		public GameObject CreateUpperHull(GameObject original)
		{
			return CreateUpperHull(original, null);
		}

		public GameObject CreateUpperHull(GameObject original, Material crossSectionMat)
		{
			GameObject gameObject = CreateUpperHull();
			if (gameObject != null)
			{
				gameObject.transform.localPosition = original.transform.localPosition;
				gameObject.transform.localRotation = original.transform.localRotation;
				gameObject.transform.localScale = original.transform.localScale;
				Material[] sharedMaterials = original.GetComponent<MeshRenderer>().sharedMaterials;
				if (original.GetComponent<MeshFilter>().sharedMesh.subMeshCount == upper_hull.subMeshCount)
				{
					gameObject.GetComponent<Renderer>().sharedMaterials = sharedMaterials;
					return gameObject;
				}
				Material[] array = new Material[sharedMaterials.Length + 1];
				Array.Copy(sharedMaterials, array, sharedMaterials.Length);
				array[sharedMaterials.Length] = crossSectionMat;
				gameObject.GetComponent<Renderer>().sharedMaterials = array;
			}
			return gameObject;
		}

		public GameObject CreateLowerHull(GameObject original)
		{
			return CreateLowerHull(original, null);
		}

		public GameObject CreateLowerHull(GameObject original, Material crossSectionMat)
		{
			GameObject gameObject = CreateLowerHull();
			if (gameObject != null)
			{
				gameObject.transform.localPosition = original.transform.localPosition;
				gameObject.transform.localRotation = original.transform.localRotation;
				gameObject.transform.localScale = original.transform.localScale;
				Material[] sharedMaterials = original.GetComponent<MeshRenderer>().sharedMaterials;
				if (original.GetComponent<MeshFilter>().sharedMesh.subMeshCount == lower_hull.subMeshCount)
				{
					gameObject.GetComponent<Renderer>().sharedMaterials = sharedMaterials;
					return gameObject;
				}
				Material[] array = new Material[sharedMaterials.Length + 1];
				Array.Copy(sharedMaterials, array, sharedMaterials.Length);
				array[sharedMaterials.Length] = crossSectionMat;
				gameObject.GetComponent<Renderer>().sharedMaterials = array;
			}
			return gameObject;
		}

		public GameObject CreateUpperHull()
		{
			return CreateEmptyObject("Upper_Hull", upper_hull);
		}

		public GameObject CreateLowerHull()
		{
			return CreateEmptyObject("Lower_Hull", lower_hull);
		}

		private static GameObject CreateEmptyObject(string name, Mesh hull)
		{
			if (hull == null)
			{
				return null;
			}
			GameObject gameObject = new GameObject(name);
			gameObject.AddComponent<MeshRenderer>();
			gameObject.AddComponent<MeshFilter>().mesh = hull;
			return gameObject;
		}
	}
}
