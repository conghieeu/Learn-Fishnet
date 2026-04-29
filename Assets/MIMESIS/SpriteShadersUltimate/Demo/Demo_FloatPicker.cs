using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_FloatPicker : MonoBehaviour
	{
		public Material targetMaterial;

		public string propertyName;

		private Slider slider;

		private void Start()
		{
			if (slider == null)
			{
				slider = base.transform.Find("Slider").GetComponent<Slider>();
			}
		}

		public void SetTarget(Material newMaterial, string newProperty, string shaderName)
		{
			Start();
			targetMaterial = newMaterial;
			propertyName = newProperty;
			float num = targetMaterial.GetFloat(propertyName);
			int propertyIndex = targetMaterial.shader.FindPropertyIndex(propertyName);
			if (targetMaterial.shader.GetPropertyType(propertyIndex) == ShaderPropertyType.Range)
			{
				Vector2 propertyRangeLimits = targetMaterial.shader.GetPropertyRangeLimits(propertyIndex);
				slider.minValue = propertyRangeLimits.x;
				slider.maxValue = propertyRangeLimits.y;
			}
			else if (newProperty.EndsWith("Contrast"))
			{
				slider.minValue = 0f;
				slider.maxValue = 3f;
			}
			else if (newProperty.EndsWith("Saturation"))
			{
				slider.minValue = 0f;
				slider.maxValue = 2f;
			}
			else if (newProperty.EndsWith("Brightness"))
			{
				slider.minValue = 0f;
				slider.maxValue = 5f;
			}
			else if (newProperty.EndsWith("PixelDensity"))
			{
				slider.minValue = 1f;
				slider.maxValue = 32f;
			}
			else
			{
				float num2 = 1f;
				while (Mathf.Abs(num) > num2)
				{
					num2 *= 10f;
				}
				slider.minValue = 0f - num2;
				slider.maxValue = num2;
			}
			if (newProperty.EndsWith("Width"))
			{
				slider.minValue = 0f;
			}
			LoadFloat(num);
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
			if (text == "")
			{
				text = "Value";
			}
			base.transform.Find("Title").GetComponent<Text>().text = text;
		}

		public void LoadFloat(float floatValue)
		{
			slider.SetValueWithoutNotify(floatValue);
			UpdateFloat(floatValue);
		}

		public void UpdateFloat(float floatValue)
		{
			string text = floatValue.ToString().Replace(",", ".");
			string[] array = text.Split('.');
			if (array.Length > 1)
			{
				text = array[0] + "." + array[1].Substring(0, Mathf.Min(array[1].Length, 2));
			}
			base.transform.Find("Value").GetComponent<Text>().text = text;
			if (targetMaterial != null)
			{
				targetMaterial.SetFloat(propertyName, floatValue);
			}
		}

		public void SliderChanged()
		{
			UpdateFloat(slider.value);
		}
	}
}
