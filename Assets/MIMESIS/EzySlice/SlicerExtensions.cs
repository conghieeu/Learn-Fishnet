using UnityEngine;

namespace EzySlice
{
	public static class SlicerExtensions
	{
		public static SlicedHull Slice(this GameObject obj, Plane pl, Material crossSectionMaterial = null)
		{
			return obj.Slice(pl, new TextureRegion(0f, 0f, 1f, 1f), crossSectionMaterial);
		}

		public static SlicedHull Slice(this GameObject obj, Vector3 position, Vector3 direction, Material crossSectionMaterial = null)
		{
			return obj.Slice(position, direction, new TextureRegion(0f, 0f, 1f, 1f), crossSectionMaterial);
		}

		public static SlicedHull Slice(this GameObject obj, Vector3 position, Vector3 direction, TextureRegion textureRegion, Material crossSectionMaterial = null)
		{
			Plane pl = default(Plane);
			Vector3 normalized = obj.transform.worldToLocalMatrix.transpose.inverse.MultiplyVector(direction).normalized;
			Vector3 pos = obj.transform.InverseTransformPoint(position);
			pl.Compute(pos, normalized);
			return obj.Slice(pl, textureRegion, crossSectionMaterial);
		}

		public static SlicedHull Slice(this GameObject obj, Plane pl, TextureRegion textureRegion, Material crossSectionMaterial = null)
		{
			return Slicer.Slice(obj, pl, textureRegion, crossSectionMaterial);
		}

		public static GameObject[] SliceInstantiate(this GameObject obj, Plane pl)
		{
			return obj.SliceInstantiate(pl, new TextureRegion(0f, 0f, 1f, 1f));
		}

		public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction)
		{
			return obj.SliceInstantiate(position, direction, null);
		}

		public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction, Material crossSectionMat)
		{
			return obj.SliceInstantiate(position, direction, new TextureRegion(0f, 0f, 1f, 1f), crossSectionMat);
		}

		public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction, TextureRegion cuttingRegion, Material crossSectionMaterial = null)
		{
			Plane pl = default(Plane);
			Vector3 normalized = obj.transform.worldToLocalMatrix.transpose.inverse.MultiplyVector(direction).normalized;
			Vector3 pos = obj.transform.InverseTransformPoint(position);
			pl.Compute(pos, normalized);
			return obj.SliceInstantiate(pl, cuttingRegion, crossSectionMaterial);
		}

		public static GameObject[] SliceInstantiate(this GameObject obj, Plane pl, TextureRegion cuttingRegion, Material crossSectionMaterial = null)
		{
			SlicedHull slicedHull = Slicer.Slice(obj, pl, cuttingRegion, crossSectionMaterial);
			if (slicedHull == null)
			{
				return null;
			}
			GameObject gameObject = slicedHull.CreateUpperHull(obj, crossSectionMaterial);
			GameObject gameObject2 = slicedHull.CreateLowerHull(obj, crossSectionMaterial);
			if (gameObject != null && gameObject2 != null)
			{
				return new GameObject[2] { gameObject, gameObject2 };
			}
			if (gameObject != null)
			{
				return new GameObject[1] { gameObject };
			}
			if (gameObject2 != null)
			{
				return new GameObject[1] { gameObject2 };
			}
			return null;
		}
	}
}
