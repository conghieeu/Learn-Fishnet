using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_GUI : MonoBehaviour
	{
		public static Demo_GUI instance;

		private GameObject displayTitlePrefab;

		private RectTransform propertyRect;

		private CanvasGroup hudCG;

		private Slider slider;

		private float scrollArea;

		private float targetHeight;

		private void Awake()
		{
			instance = this;
			hudCG = base.transform.Find("Shader Hud").GetComponent<CanvasGroup>();
			displayTitlePrefab = base.transform.Find("Display Titles/Title Prefab").gameObject;
			propertyRect = base.transform.Find("Shader Hud/Properties/Rect").GetComponent<RectTransform>();
			slider = base.transform.Find("Shader Hud/Properties/Slider").GetComponent<Slider>();
		}

		private void Update()
		{
			hudCG.alpha = Mathf.Lerp(hudCG.alpha, Demo_Shaders.instance.FadeInGUI() ? 1.1f : (-0.1f), Time.unscaledDeltaTime * 7.5f);
			if (!(hudCG.alpha > 0.5f))
			{
				return;
			}
			if (slider.gameObject.activeInHierarchy)
			{
				float num = 100f / Mathf.Abs(scrollArea - 500f);
				if (Input.mouseScrollDelta.y > 0.01f)
				{
					slider.SetValueWithoutNotify(Mathf.Clamp01(slider.value - num));
					UpdateScroll();
				}
				else if ((double)Input.mouseScrollDelta.y < -0.01)
				{
					slider.SetValueWithoutNotify(Mathf.Clamp01(slider.value + num));
					UpdateScroll();
				}
			}
			propertyRect.anchoredPosition = new Vector2(0f, Mathf.Lerp(propertyRect.anchoredPosition.y, targetHeight, Time.unscaledDeltaTime * 8f));
		}

		public void CreateTitle(string title, Transform target)
		{
			GameObject obj = Object.Instantiate(displayTitlePrefab);
			obj.transform.SetParent(displayTitlePrefab.transform.parent, worldPositionStays: true);
			obj.name = title;
			obj.GetComponent<Demo_DisplayTitle>().target = target;
			obj.GetComponent<Text>().text = title;
			obj.SetActive(value: true);
		}

		public void UpdateHud()
		{
			bool blocksRaycasts;
			if (Demo_Display.selected == null)
			{
				CanvasGroup canvasGroup = hudCG;
				blocksRaycasts = (hudCG.interactable = false);
				canvasGroup.blocksRaycasts = blocksRaycasts;
				return;
			}
			CanvasGroup canvasGroup2 = hudCG;
			blocksRaycasts = (hudCG.interactable = true);
			canvasGroup2.blocksRaycasts = blocksRaycasts;
			Transform obj = base.transform.Find("Shader Hud");
			obj.Find("Shader Title").GetComponent<Text>().text = Demo_Display.selected.gameObject.name;
			float num = 240f;
			if (Demo_Display.selected.HasAlternatives())
			{
				propertyRect.Find("AlternativeButton").gameObject.SetActive(value: true);
				num = 180f;
			}
			else
			{
				propertyRect.Find("AlternativeButton").gameObject.SetActive(value: false);
			}
			Transform obj2 = obj.Find("Properties");
			GameObject gameObject = obj2.Find("Color").gameObject;
			gameObject.SetActive(value: false);
			GameObject gameObject2 = obj2.Find("Float").gameObject;
			gameObject2.SetActive(value: false);
			GameObject gameObject3 = obj2.Find("Vector").gameObject;
			gameObject3.SetActive(value: false);
			for (int i = 0; i < propertyRect.childCount; i++)
			{
				if (propertyRect.GetChild(i).gameObject.name != "AlternativeButton")
				{
					Object.Destroy(propertyRect.GetChild(i).gameObject);
				}
			}
			Material material = Demo_Display.selected.InstantiateMaterial();
			if (material == null)
			{
				slider.gameObject.SetActive(value: false);
				return;
			}
			int num2 = material.shader.FindPropertyIndex(Demo_Display.selected.firstProperty);
			int propertyCount = material.shader.GetPropertyCount();
			bool flag3 = false;
			while (num2 < propertyCount)
			{
				string propertyName = material.shader.GetPropertyName(num2);
				ShaderPropertyType propertyType = material.shader.GetPropertyType(num2);
				num2++;
				if (propertyName.StartsWith("_Enable"))
				{
					break;
				}
				if (!IsKeyword(propertyName))
				{
					if (flag3)
					{
						switch (propertyName)
						{
						case "_EnchantedLowColor":
						case "_EnchantedHighColor":
						case "_ShiftingColorA":
						case "_ShiftingColorB":
							break;
						default:
							continue;
						}
					}
					else
					{
						switch (propertyName)
						{
						case "_EnchantedLowColor":
						case "_EnchantedHighColor":
						case "_ShiftingColorA":
						case "_ShiftingColorB":
							continue;
						}
					}
					RectTransform rectTransform = null;
					switch (propertyType)
					{
					case ShaderPropertyType.Color:
					{
						GameObject obj5 = Object.Instantiate(gameObject);
						obj5.transform.SetParent(propertyRect, worldPositionStays: true);
						obj5.transform.position = gameObject.transform.position;
						obj5.transform.localScale = Vector3.one;
						obj5.SetActive(value: true);
						rectTransform = obj5.GetComponent<RectTransform>();
						obj5.GetComponent<Demo_ColorPicker>().SetTarget(material, propertyName, Demo_Display.selected.gameObject.name);
						break;
					}
					case ShaderPropertyType.Float:
					case ShaderPropertyType.Range:
					{
						GameObject obj4 = Object.Instantiate(gameObject2);
						obj4.transform.SetParent(propertyRect, worldPositionStays: true);
						obj4.transform.position = gameObject2.transform.position;
						obj4.transform.localScale = Vector3.one;
						obj4.SetActive(value: true);
						rectTransform = obj4.GetComponent<RectTransform>();
						obj4.GetComponent<Demo_FloatPicker>().SetTarget(material, propertyName, Demo_Display.selected.gameObject.name);
						break;
					}
					case ShaderPropertyType.Vector:
					{
						GameObject obj3 = Object.Instantiate(gameObject3);
						obj3.transform.SetParent(propertyRect, worldPositionStays: true);
						obj3.transform.position = gameObject3.transform.position;
						obj3.transform.localScale = Vector3.one;
						obj3.SetActive(value: true);
						rectTransform = obj3.GetComponent<RectTransform>();
						obj3.GetComponent<Demo_VectorPicker>().SetTarget(material, propertyName, Demo_Display.selected.gameObject.name);
						break;
					}
					}
					if (rectTransform != null)
					{
						Vector2 anchoredPosition = rectTransform.anchoredPosition;
						anchoredPosition.y = num - rectTransform.sizeDelta.y * 0.5f;
						num -= rectTransform.sizeDelta.y;
						rectTransform.anchoredPosition = anchoredPosition;
					}
				}
				else
				{
					flag3 = material.GetFloat(propertyName) < 0.5f;
				}
			}
			scrollArea = 240f - num;
			slider.SetValueWithoutNotify(0f);
			slider.gameObject.SetActive(scrollArea > 500f);
			targetHeight = 0f;
		}

		public static bool IsKeyword(string propName)
		{
			if (!propName.StartsWith("_Toggle") && !propName.EndsWith("Toggle") && !propName.EndsWith("Invert"))
			{
				switch (propName)
				{
				case "PixelSnap":
				case "_ShaderSpace":
				case "_SmokeVertexSeed":
				case "_ShaderFading":
				case "_BakedMaterial":
				case "_SpriteSheetFix":
				case "_ForceAlpha":
				case "_VertexTintFirst":
				case "_PixelPerfectSpace":
				case "_PixelPerfectUV":
				case "_WindLocalWind":
				case "_WindHighQualityNoise":
				case "_WindIsParallax":
				case "_WindFlip":
				case "_SquishFlip":
					break;
				default:
					return false;
				}
			}
			return true;
		}

		public void BackButton()
		{
			if (Demo_Display.selected != null)
			{
				Demo_Display.selected.Deselect();
			}
		}

		public void ResetMaterialButton()
		{
			if (Demo_Display.selected != null)
			{
				UpdateHud();
			}
		}

		public void AlternativeButton()
		{
			if (Demo_Display.selected != null)
			{
				Demo_Display.selected.InstantiateMaterial();
				Demo_Display.selected.ChangeIndex();
				UpdateHud();
			}
		}

		public void UpdateScroll()
		{
			targetHeight = slider.value * (scrollArea - 500f);
		}
	}
}
