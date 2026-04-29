using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_ColorPicker : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		public Material targetMaterial;

		public string propertyName;

		private RectTransform colorArea;

		private Slider brightnessSlider;

		private RectTransform dotRect;

		private Image dotImage;

		private bool isHovered;

		private bool isDragging;

		private float lastHue;

		private float lastSaturation;

		private float maxBrightness;

		private void Start()
		{
			if (brightnessSlider == null || colorArea == null)
			{
				colorArea = base.transform.Find("Color Area").GetComponent<RectTransform>();
				brightnessSlider = base.transform.Find("Brightness Slider").GetComponent<Slider>();
				dotRect = base.transform.Find("Color Area/Dot").GetComponent<RectTransform>();
				dotImage = dotRect.GetComponent<Image>();
				dotImage.material = Object.Instantiate(dotImage.material);
			}
		}

		private void Update()
		{
			Vector2 localPoint = default(Vector2);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(colorArea, Input.mousePosition, Camera.main, out localPoint);
			if (isHovered && Input.GetMouseButtonDown(0) && localPoint.x > (0f - colorArea.sizeDelta.x) * 0.5f && localPoint.x < colorArea.sizeDelta.x * 0.5f && localPoint.y > (0f - colorArea.sizeDelta.y) * 0.5f && localPoint.y < colorArea.sizeDelta.y * 0.5f)
			{
				isDragging = true;
			}
			if (isDragging)
			{
				if (!Input.GetMouseButton(0))
				{
					isDragging = false;
				}
				Vector2 vector = (localPoint + colorArea.sizeDelta * 0.5f) / colorArea.sizeDelta;
				float hue = Mathf.Clamp01(vector.x);
				float saturation = Mathf.Clamp01(vector.y);
				UpdateColor(hue, saturation);
			}
		}

		public void SetTarget(Material newMaterial, string newProperty, string shaderName)
		{
			Start();
			targetMaterial = newMaterial;
			propertyName = newProperty;
			LoadColor(targetMaterial.GetColor(propertyName));
			string text = newProperty.Replace("_" + shaderName.Replace(" ", ""), "");
			char[] array = text.ToCharArray();
			text = "";
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0 && array[i].ToString().ToUpper() == array[i].ToString())
				{
					text += " ";
				}
				text += array[i];
			}
			base.transform.Find("Title").GetComponent<Text>().text = text;
		}

		public void LoadColor(Color color)
		{
			Color.RGBToHSV(color, out var H, out var S, out var V);
			maxBrightness = Mathf.Ceil(V * 0.5f) * 4f + 5f;
			if (V <= 1f)
			{
				brightnessSlider.SetValueWithoutNotify(V * 0.5f);
			}
			else
			{
				brightnessSlider.SetValueWithoutNotify(0.5f + (V - 1f) / maxBrightness);
			}
			UpdateColor(H, S);
		}

		public void UpdateColor(float hue, float saturation)
		{
			lastHue = hue;
			lastSaturation = saturation;
			dotRect.anchoredPosition = new Vector2(Mathf.Clamp(colorArea.sizeDelta.x * hue, 5f, colorArea.sizeDelta.x - 5f), Mathf.Clamp(colorArea.sizeDelta.y * saturation, 5f, colorArea.sizeDelta.y - 5f));
			dotImage.color = Color.HSVToRGB(hue, saturation, 1f);
			float num = Mathf.Min(brightnessSlider.value * 2f, 1f) + Mathf.Max((brightnessSlider.value - 0.5f) * maxBrightness, 0f);
			dotImage.materialForRendering.SetFloat("_Brightness", num);
			if (targetMaterial != null)
			{
				targetMaterial.SetColor(propertyName, Color.HSVToRGB(hue, saturation, num));
			}
		}

		public void SliderChanged()
		{
			if (Mathf.Abs(brightnessSlider.value - 0.5f) < 0.05f)
			{
				brightnessSlider.SetValueWithoutNotify(0.5f);
			}
			UpdateColor(lastHue, lastSaturation);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			isHovered = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			isHovered = false;
		}
	}
}
