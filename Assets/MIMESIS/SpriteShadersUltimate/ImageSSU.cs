using UnityEngine;
using UnityEngine.UI;

namespace SpriteShadersUltimate
{
	[AddComponentMenu("Sprite Shaders Ultimate/Utility/Image SSU")]
	public class ImageSSU : InstancerSSU
	{
		[Tooltip("Enable this if the size of the RectTransform will change.")]
		public bool updateChanges;

		private RectTransform rectTransform;

		private int rectWidthID;

		private int rectHeightID;

		private Vector2 lastSizeDelta;

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			Image component = GetComponent<Image>();
			component.material = Object.Instantiate(component.material);
			runtimeMaterial = component.materialForRendering;
			rectWidthID = Shader.PropertyToID("_RectWidth");
			rectHeightID = Shader.PropertyToID("_RectHeight");
		}

		private void Start()
		{
			UpdateMaterial();
		}

		private void Update()
		{
			if (updateChanges && lastSizeDelta != rectTransform.sizeDelta)
			{
				UpdateMaterial();
			}
		}

		public void UpdateMaterial()
		{
			lastSizeDelta = rectTransform.sizeDelta;
			runtimeMaterial.SetFloat(rectWidthID, lastSizeDelta.x);
			runtimeMaterial.SetFloat(rectHeightID, lastSizeDelta.y);
		}
	}
}
