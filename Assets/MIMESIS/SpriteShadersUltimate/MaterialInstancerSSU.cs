using UnityEngine;
using UnityEngine.UI;

namespace SpriteShadersUltimate
{
	[AddComponentMenu("Sprite Shaders Ultimate/Utility/Material Instancer SSU")]
	public class MaterialInstancerSSU : InstancerSSU
	{
		private void Awake()
		{
			Graphic component = GetComponent<Graphic>();
			if (component != null)
			{
				component.material = Object.Instantiate(component.material);
				runtimeMaterial = component.materialForRendering;
			}
			Renderer component2 = GetComponent<Renderer>();
			if (component2 != null)
			{
				Material[] sharedMaterials = component2.sharedMaterials;
				for (int i = 0; i < sharedMaterials.Length; i++)
				{
					sharedMaterials[i] = Object.Instantiate(sharedMaterials[i]);
				}
				Material[] materials = (component2.sharedMaterials = sharedMaterials);
				component2.materials = materials;
				runtimeMaterial = sharedMaterials[0];
			}
		}
	}
}
