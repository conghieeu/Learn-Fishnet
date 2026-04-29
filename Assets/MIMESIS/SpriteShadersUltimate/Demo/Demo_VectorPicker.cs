using UnityEngine;
using UnityEngine.UI;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_VectorPicker : MonoBehaviour
	{
		public Material targetMaterial;

		public string propertyName;

		private Slider slider1;

		private Slider slider2;

		private void Start()
		{
			if (slider1 == null)
			{
				slider1 = base.transform.Find("Slider 1").GetComponent<Slider>();
				slider2 = base.transform.Find("Slider 2").GetComponent<Slider>();
			}
		}

		public void SetTarget(Material newMaterial, string newProperty, string shaderName)
		{
			Start();
			targetMaterial = newMaterial;
			propertyName = newProperty;
			Vector2 vectorValue = targetMaterial.GetVector(propertyName);
			targetMaterial.shader.FindPropertyIndex(propertyName);
			float num = Mathf.Abs(vectorValue.x) + Mathf.Abs(vectorValue.y) * 0.5f;
			if (num < 1f)
			{
				Slider slider = slider1;
				float minValue = (this.slider2.minValue = -1f);
				slider.minValue = minValue;
				Slider slider2 = slider1;
				minValue = (this.slider2.maxValue = 1f);
				slider2.maxValue = minValue;
			}
			else if (num < 2f)
			{
				Slider slider3 = slider1;
				float minValue = (this.slider2.minValue = -2f);
				slider3.minValue = minValue;
				Slider slider4 = slider1;
				minValue = (this.slider2.maxValue = 2f);
				slider4.maxValue = minValue;
			}
			else
			{
				Slider slider5 = slider1;
				float minValue = (this.slider2.minValue = (0f - num) * 2f);
				slider5.minValue = minValue;
				Slider slider6 = slider1;
				minValue = (this.slider2.maxValue = num * 2f);
				slider6.maxValue = minValue;
			}
			if (propertyName.EndsWith("Scale"))
			{
				Slider slider7 = slider1;
				float minValue = (this.slider2.minValue = 0f);
				slider7.minValue = minValue;
			}
			LoadVector(vectorValue);
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

		public void LoadVector(Vector2 vectorValue)
		{
			slider1.SetValueWithoutNotify(vectorValue.x);
			slider2.SetValueWithoutNotify(vectorValue.y);
			UpdateVector(vectorValue);
		}

		public void UpdateVector(Vector2 vectorValue)
		{
			SetSliderValue(slider1, vectorValue.x);
			SetSliderValue(slider2, vectorValue.y);
			if (targetMaterial != null)
			{
				targetMaterial.SetVector(propertyName, vectorValue);
			}
		}

		private void SetSliderValue(Slider toSlider, float toValue)
		{
			string text = toValue.ToString().Replace(",", ".");
			string[] array = text.Split('.');
			if (array.Length > 1)
			{
				text = array[0] + "." + array[1].Substring(0, Mathf.Min(array[1].Length, (Mathf.Abs(toValue) >= 0.01f) ? 2 : 3));
			}
			toSlider.transform.Find("Value").GetComponent<Text>().text = text;
		}

		public void SliderChanged()
		{
			UpdateVector(new Vector2(slider1.value, slider2.value));
		}
	}
}
