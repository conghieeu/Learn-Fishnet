using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_VectorFollow : MonoBehaviour
	{
		public string propertyName;

		public Transform trackedTransform;

		private Material mat;

		private void Start()
		{
			Renderer componentInChildren = GetComponentInChildren<Renderer>();
			if (componentInChildren.sharedMaterial.name.EndsWith("(Instance)"))
			{
				mat = componentInChildren.sharedMaterial;
			}
			else
			{
				mat = componentInChildren.material;
			}
		}

		private void FixedUpdate()
		{
			mat.SetVector(propertyName, trackedTransform.position);
		}
	}
}
