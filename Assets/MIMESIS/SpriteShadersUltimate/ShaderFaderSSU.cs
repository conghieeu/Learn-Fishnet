using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteShadersUltimate
{
	[AddComponentMenu("Sprite Shaders Ultimate/Utility/Shader Fader SSU")]
	public class ShaderFaderSSU : MonoBehaviour
	{
		public bool automaticFading = true;

		public bool isFaded;

		[Range(0f, 1f)]
		public float fadeValue;

		[Min(0.01f)]
		public float duration = 2f;

		public bool unscaledTime;

		public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f, 0.55f, 0.55f), new Keyframe(1f, 1f, 0f, 0f, 0.55f, 0.55f));

		public bool getChildObjects = true;

		public bool poolMaterials = true;

		public List<Renderer> renderers = new List<Renderer>();

		public List<Graphic> graphics = new List<Graphic>();

		public List<FloatFaderSSU> floatProperties = new List<FloatFaderSSU>();

		public List<VectorFaderSSU> vectorProperties = new List<VectorFaderSSU>();

		public List<ColorFaderSSU> colorProperties = new List<ColorFaderSSU>();

		private HashSet<Material> materials;

		private Dictionary<Material, Material> materialPool;

		private float lastFadeValue;

		private void Start()
		{
			ReloadMaterials();
		}

		private void Update()
		{
			if (automaticFading)
			{
				if (isFaded)
				{
					float num = 1f / duration;
					fadeValue += (unscaledTime ? (Time.unscaledDeltaTime * num) : (Time.deltaTime * num));
					if (fadeValue > 1f)
					{
						fadeValue = 1f;
					}
				}
				else
				{
					float num2 = 1f / duration;
					fadeValue -= (unscaledTime ? (Time.unscaledDeltaTime * num2) : (Time.deltaTime * num2));
					if (fadeValue < 0f)
					{
						fadeValue = 0f;
					}
				}
			}
			if (lastFadeValue != fadeValue)
			{
				lastFadeValue = fadeValue;
				UpdateMaterials();
			}
		}

		public void UpdateMaterials()
		{
			foreach (Material material in materials)
			{
				UpdateSingleMaterial(material, fadeValue);
			}
		}

		public void UpdateSingleMaterial(Material mat, float fadeFactor)
		{
			foreach (FloatFaderSSU floatProperty in floatProperties)
			{
				mat.SetFloat(floatProperty.propertyName, Mathf.LerpUnclamped(floatProperty.fromValue, floatProperty.toValue, ApplyTimeRange(fadeFactor, floatProperty.fromTime, floatProperty.toTime)));
			}
			foreach (VectorFaderSSU vectorProperty in vectorProperties)
			{
				mat.SetColor(vectorProperty.propertyName, Vector4.LerpUnclamped(vectorProperty.fromValue, vectorProperty.toValue, ApplyTimeRange(fadeFactor, vectorProperty.fromTime, vectorProperty.toTime)));
			}
			foreach (ColorFaderSSU colorProperty in colorProperties)
			{
				mat.SetColor(colorProperty.propertyName, Color.LerpUnclamped(colorProperty.fromValue, colorProperty.toValue, ApplyTimeRange(fadeFactor, colorProperty.fromTime, colorProperty.toTime)));
			}
		}

		private float ApplyTimeRange(float fadeFactor, float fromTime, float toTime)
		{
			return fadeCurve.Evaluate(Mathf.Clamp01((fadeFactor - fromTime) / (toTime - fromTime)));
		}

		public void ReloadMaterials()
		{
			materials = new HashSet<Material>();
			materialPool = new Dictionary<Material, Material>();
			lastFadeValue = -1f;
			if (getChildObjects)
			{
				Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
				foreach (Renderer renderer in componentsInChildren)
				{
					GetMaterialFromRenderer(renderer);
				}
				Graphic[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Graphic>(includeInactive: true);
				foreach (Graphic graphic in componentsInChildren2)
				{
					GetMaterialFromGraphic(graphic);
				}
				return;
			}
			if (renderers != null)
			{
				foreach (Renderer renderer2 in renderers)
				{
					if (renderer2 != null)
					{
						GetMaterialFromRenderer(renderer2);
					}
				}
			}
			if (graphics == null)
			{
				return;
			}
			foreach (Graphic graphic2 in graphics)
			{
				if (graphic2 != null)
				{
					GetMaterialFromGraphic(graphic2);
				}
			}
		}

		private void GetMaterialFromRenderer(Renderer renderer)
		{
			InstancerSSU component = renderer.GetComponent<InstancerSSU>();
			if (component != null)
			{
				materials.Add(component.runtimeMaterial);
				return;
			}
			Material material = (renderer.material = InstantiateMaterial(renderer.material));
			Material material3 = material;
			materials.Add(material3);
			renderer.gameObject.AddComponent<InstancerSSU>().runtimeMaterial = material3;
		}

		private void GetMaterialFromGraphic(Graphic graphic)
		{
			InstancerSSU component = graphic.GetComponent<InstancerSSU>();
			if (component != null)
			{
				materials.Add(component.runtimeMaterial);
				return;
			}
			Material material = (graphic.material = InstantiateMaterial(graphic.material));
			Material material3 = material;
			materials.Add(material3);
			graphic.gameObject.AddComponent<InstancerSSU>().runtimeMaterial = material3;
		}

		private Material InstantiateMaterial(Material sharedMaterial)
		{
			if (poolMaterials)
			{
				if (materialPool.ContainsKey(sharedMaterial))
				{
					return materialPool[sharedMaterial];
				}
				Material material = Object.Instantiate(sharedMaterial);
				material.name = sharedMaterial?.ToString() + " (Instance)";
				materialPool.Add(sharedMaterial, material);
				return material;
			}
			return Object.Instantiate(sharedMaterial);
		}
	}
}
